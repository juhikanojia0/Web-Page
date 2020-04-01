/*************************************************************
* Project   : CIFMIS_GeneratePayment Web Portal for E10.1
* Author    : Mahesh D. Deore
* Date      : 
* Purpose   : 
* Version   : 1.0.0.0
Modified History
Version   Modified by       Modified on     Description
1.0.0.1   Mahesh Deore      16-Jun-2016     added CPO User Group rights logic with dynamic logic
1.0.0.2   Mahesh            15-Feb-2017     code changes as per shekhar's comment & code resolution for Maximum user session error Bug Id:9813
1.0.0.3   Shekhar Chaudhari 19-Mar-2018     Incorported 10.1.600.18 changes in 10.2.100.9 ver.
                                            Resolved VSO Bug No 16330 Remove hard coded validation for rights to access Generate Payment Portal.
1.0.0.4   Shekhar Chaudhari 20-Jun-2018     Task ID 16237 :- Include Event Viewer information log recording functionality.
1.0.0.5   Shekhar Chaudhari 25-Jun-2018     Task Id 17103 :- Implement SSO Functionality and handle hybrid functionality with SSO and NON SSO.
***********************************************************************************************************************************************/
using System;
using System.Web.UI;
using System.Data;
using Ice.Proxy.Lib;
using Epicor.ServiceModel.Channels;
using System.Data.SqlClient;
using Stcl.Global.GlobalMethods;
using System.Web;
using System.Text;

namespace Stcl.EpicorErp10.GeneratePaymentWebPortal.WebPages
{
    public partial class Stcl_Login : System.Web.UI.Page
    {
        #region Public variable declaration Added by MDD
        static string EpicorUserID = string.Empty;
        static string EpicorUserPassword = string.Empty;
        static string CSharpCodeServerLogSwitch = string.Empty;
        static string TreasuryCompany = string.Empty;

        static string IsSSOLoginCompulsory = string.Empty;
        static string AppSrvUrl = string.Empty; // This should be the url to your appserver        
        static string EndpointBinding = string.Empty; // This is case sensitive. Valid values are "UsernameWindowsChannel", "Windows" and "UsernameSslChannel"

        string CompanyID = string.Empty;
        static SqlDataAdapter DACOASeg = new SqlDataAdapter();
        static SqlCommand sqlCmd = new SqlCommand();
        static DataSet DsCOASegTMP = new DataSet();
        static string SqlStr = string.Empty;
        DataAccessLayer.GeneratePaymentDataObjects ObjDO = new DataAccessLayer.GeneratePaymentDataObjects();
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            EventLog ObjEvent = new EventLog();
            StringBuilder LogMsg = new StringBuilder();

            IsSSOLoginCompulsory = Convert.ToString(ObjDO.GetSysParam("IsSSOLoginCompulsory")).ToUpper();
            EpicorUserID = Convert.ToString(ObjDO.GetSysParam("epicorUserID"));
            EpicorUserPassword = Convert.ToString(ObjDO.GetSysParam("epicorUserPassword"));
            CSharpCodeServerLogSwitch = Convert.ToString(ObjDO.GetSysParam("CSharpCodeServerLogSwitch")).ToUpper();
            TreasuryCompany = Convert.ToString(ObjDO.GetSysParam("TreasuryCompany"));
            string WriteEventLog = CSharpCodeServerLogSwitch;

            LogMsg.AppendLine("Is SSO Login Compulsory =" + IsSSOLoginCompulsory);

            if (!string.IsNullOrEmpty(IsSSOLoginCompulsory))
            {
                Session["IsSSOLoginCompulsory"] = IsSSOLoginCompulsory;
            }
            else
            {
                Session["TransMessage"] = Convert.ToString("Configuration of Is SSO Login Compulsory missing in sysparam, please contact System Administrator.");
                lblErrMsg.Text = Convert.ToString(Session["TransMessage"]);
                Response.Redirect("Stcl_ErrorMessage.aspx", false);
                return;
            }

            if (!IsPostBack)
            {
                try
                {      
                    string WinLogin = string.Empty;
                    string WinDomain = string.Empty;                                       
                   
                    DataSet DsValidateSSO = new DataSet();

                    LogMsg.AppendLine("HttpContext.Current.User.Identity.Name =" + HttpContext.Current.User.Identity.Name);

                    LogMsg.AppendLine("User.Identity.Name =" + User.Identity.Name.ToString());               


                    if (!string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
                    {
                        WinLogin = ObjDO.LoginIDNoDomain(HttpContext.Current.User.Identity.Name);
                        WinDomain = ObjDO.LoginDomain(HttpContext.Current.User.Identity.Name);
                    }
                    else
                    {
                        WinLogin = "";
                        WinDomain = "";
                    }                    

                    LogMsg.AppendLine("WinLogin =" + WinLogin);
                    LogMsg.AppendLine("WinDomain =" + WinDomain);

                    if (!string.IsNullOrEmpty(WinLogin))
                    {
                        DsValidateSSO = ObjDO.ValidateSingleSignOn(WinLogin, WinDomain);
                        LogMsg.AppendLine("DsValidateSSO =" + Convert.ToString(DsValidateSSO.Tables[0].Rows.Count));
                    }

                    if (DsValidateSSO.Tables.Count > 0 && DsValidateSSO.Tables[0].Rows.Count > 0)
                    {
                        if (Convert.ToBoolean(DsValidateSSO.Tables[0].Rows[0]["RequireSso"]) == true)
                        {
                            EndpointBinding = "Windows";
                            AppSrvUrl = "net.tcp://" + Convert.ToString(ObjDO.GetSysParam("ServerUrlSSO")); //Single Sign On URL

                            if (string.IsNullOrEmpty(AppSrvUrl))
                            {                                
                                Session["TransMessage"] = Convert.ToString("Configuration of Server URL for SSO missing in sysparam, please contact System Administrator.");
                                lblErrMsg.Text = Convert.ToString(Session["TransMessage"]);
                                Response.Redirect("Stcl_ErrorMessage.aspx", false);
                                return;
                            }
                            

                            LogMsg.AppendLine("RequireSso =" + Convert.ToString(DsValidateSSO.Tables[0].Rows[0]["RequireSso"]));

                            string UserId = string.Empty;

                            EpicorUserID = WinLogin;
                            EpicorUserPassword = "";
                            SessionModImpl SessionModImpl = CreateBusObj<SessionModImpl>(Guid.Empty, SessionModImpl.UriPath);
                            try
                            {
                                Guid sessionId = SessionModImpl.Login();
                                SessionModImpl.SessionID = sessionId;
                                
                                if (SessionModImpl.SessionID != null)
                                {
                                    if (ObjDO.CheckCPOUserYN(EpicorUserID))
                                    {
                                        Session["AppSrvUrl"] = AppSrvUrl;
                                        Session["EndpointBinding"] = EndpointBinding;
                                        Session["UserId"] = WinLogin;
                                        Session["Password"] = "";
                                        Session["IsSSOUser"] = true;
                                        Session["SessionId"] = SessionModImpl.SessionID;
                                        Response.Redirect("Stcl_ConsolidateGeneratePaymentInfo.aspx", false);
                                    }
                                    else
                                    {
                                        Session["TransMessage"] = Convert.ToString("Sorry, You don't have Payment Generation rights.");
                                        lblErrMsg.Text = Convert.ToString(Session["TransMessage"]);
                                        Response.Redirect("Stcl_ErrorMessage.aspx", false);
                                        return;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Session["TransMessage"] = ex.Message;
                                Response.Redirect("Stcl_ErrorMessage.aspx", false);
                                return;
                            }
                            finally
                            {
                                SessionModImpl.Logout();
                                SessionModImpl.Dispose();
                            }
                        }
                        else
                        {
                            Session["TransMessage"] = "You are not a valid SSO User 1. Please contact system administrator for login credentials.";
                            Response.Redirect("Stcl_ErrorMessage.aspx", false);
                        }
                    }
                    else
                    {                        
                        ObjEvent.InformationEvent("GeneratePayment", "Stcl_Login => Page_Load", LogMsg.ToString(), "GeneratePayment", WriteEventLog, 1);
                       
                        if (Convert.ToString(Session["IsSSOLoginCompulsory"]).ToUpper() == "TRUE")
                        {                           
                            Session["TransMessage"] = "You are not a valid SSO User 2. Please contact system administrator for login credentials.";
                            Response.Redirect("Stcl_ErrorMessage.aspx", false);
                        }
                        else
                        {
                            Session["IsSSOUser"] = false;
                            EpicorUserID = txtLoginUserId.Text.ToString();
                            EpicorUserPassword = txtPassword.Text.ToString();
                        }
                    }                    
                }
                catch (Exception ex)
                {
                    ObjEvent.ExceptionEvent("GeneratePayment","Stcl_Login => Page_Load", LogMsg.ToString(), ex);                  
                    Session["TransMessage"] = ex.Message;
                    Response.Redirect("Stcl_ErrorMessage.aspx", false);
                }
                finally
                {
                    ObjEvent.InformationEvent("GeneratePayment", "Stcl_Login => Page_Load", LogMsg.ToString(), "GeneratePayment", WriteEventLog, 1);
                }
            }
        }

        protected void btnProceed_Click(object sender, EventArgs e)
        {
            if (Convert.ToString(Session["IsSSOLoginCompulsory"]).ToUpper() == "FALSE")
            {
                if (!string.IsNullOrEmpty(Convert.ToString(txtLoginUserId.Text)) && !string.IsNullOrEmpty(Convert.ToString(txtPassword.Text)))
                {
                    AppSrvUrl = "net.tcp://" + Convert.ToString(ObjDO.GetSysParam("ServerUrl"));  //NON Single Sign On URL
                    EndpointBinding = "UsernameWindowsChannel";

                    EpicorUserID = txtLoginUserId.Text.ToString();
                    EpicorUserPassword = txtPassword.Text.ToString();

                    SessionModImpl SessionModImpl = CreateBusObj<SessionModImpl>(Guid.Empty, SessionModImpl.UriPath);
                    try
                    {
                        Guid sessionId = SessionModImpl.Login();
                        SessionModImpl.SessionID = sessionId;

                        if (SessionModImpl.SessionID != null)
                        {
                            if (ObjDO.CheckCPOUserYN(EpicorUserID))
                            {
                                Session["IsSSOUser"] = false;
                                Session["AppSrvUrl"] = AppSrvUrl;
                                Session["EndpointBinding"] = EndpointBinding;
                                Session["SessionId"] = SessionModImpl.SessionID;
                            }
                            else
                            {
                                Session["TransMessage"] = Convert.ToString("Sorry, You don't have Payment Generation rights.");
                                lblErrMsg.Text = Convert.ToString(Session["TransMessage"]);
                                Response.Redirect("Stcl_ErrorMessage.aspx", false);
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.ToUpper() == "Invalid username or password.".ToUpper())
                        {
                            Session["TransMessage"] = Convert.ToString(ex.Message);
                            lblErrMsg.Text = Convert.ToString(Session["TransMessage"]);
                            Response.Redirect("Stcl_ErrorMessage.aspx");
                        }
                        else
                        {
                            Session["TransMessage"] = Convert.ToString(ex.Message);

                            lblErrMsg.Text = Convert.ToString(Session["TransMessage"]);
                            Response.Redirect("Stcl_ErrorMessage.aspx");
                        }
                    }
                    finally
                    {
                        SessionModImpl.Logout();
                        SessionModImpl.Dispose();
                    }

                    Session["UserId"] = Convert.ToString(txtLoginUserId.Text).Trim();
                    Session["Password"] = Convert.ToString(txtPassword.Text).Trim();
                    Response.Redirect("Stcl_ConsolidateGeneratePaymentInfo.aspx", false);
                }
            }
            else
            {
                Session["TransMessage"] = "You are not a valid SSO User 2. Please contact system administrator for login credentials.";
                Response.Redirect("Stcl_ErrorMessage.aspx", false);
            }
        }

     
        #region Internal Methods (CreateBusObj)
        public static T CreateBusObj<T>(Guid sessionId, string uriPath) where T : ImplBase
        {
            int OperationTimeout = 300;
            // the next two values are only used if the binding is set to UsernameSslChannel
            bool ValidateWcfCertificate = false; // should the certificate be validated as coming from a known certificate authority
            string DnsEndpointIdentity = string.Empty; // if the idenitity of the certificate does not match the machine name used in the url, you can specify it here.

            Guid SessionGuid = sessionId != null ? sessionId : Guid.Empty;

            T BO = ImplFactory.CreateImpl<T>(
                    uriPath,
                    appServerUrl: AppSrvUrl,
                    submitUser: string.Empty,
                    endpointBinding: EndpointBinding,
                    sessionId: SessionGuid,
                    userId: EpicorUserID,
                    password: EpicorUserPassword,
                    operationTimeout: OperationTimeout,
                    validateWcfCertificate: ValidateWcfCertificate,
                    dnsIdentity: DnsEndpointIdentity,
                    licenseTypeId: Ice.License.LicensableUserCounts.DefaultUser);

            return BO;
        }
        #endregion
    }
}