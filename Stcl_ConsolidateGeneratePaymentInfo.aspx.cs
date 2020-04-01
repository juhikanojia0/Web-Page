/*************************************************************
* Project   : CIFMIS_GeneratePayment Web Portal for E10.1
* Author    : Mahesh D. Deore
* Date      : 10 Dec 2015
* Purpose   : to get database connection [modified the code which is already developed for ERP 9]
* Version   : 1.0.0.0
*
* Version   Modified By        Modified Date   Purpose
 *--------   ------------------ --------------- -------------------------------------------------
 * 1.0.0.2   Mahesh Deore       16-Jun-2016     added dynamic code as per CG Module
 * 1.0.0.3   Shekhar Chaudhari  19-Mar-2018     Incorported 10.1.600.18 changes in 10.2.100.9 ver.
 *                                              1) Resolved VSO Bug No :-13046  Generate Payment Web portal users should not be mandatory to have Ministries access rights in Epicor.
                                                2) Treasury company rights validation added.
 * 1.0.0.4   Shekhar Chaudhari  14-Jun-2018     1) Task ID 17102 :- Included Salary Payment processing functionality
 * 1.0.0.5   Mahesh Deore       21-Jun-2019     23570 - GPW Portal Issue - System is throwing an error 'Invalid Company XXXX for user XXXXXXXX' while more sessions are get activated
 * 2.0.0.0   Rajesh             19-Nov-2019     25907 - GOTG: GRA and SAP Payment Generation 
************************************************************************************************/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Ice.Proxy.Lib;
using Epicor.ServiceModel.Channels;
using System.Linq;
using Stcl.Global.GlobalMethods;

namespace Stcl.EpicorErp10.GeneratePaymentWebPortal.WebPages
{
    public partial class Stcl_ConsolidateGeneratePaymentInfo : System.Web.UI.Page
    {
        string PaymentGeneratorUserId = string.Empty;
        string CompanyId = string.Empty;
        string Password = string.Empty;
        string SessionId = string.Empty;

        #region Public variable declaration Added by MDD
        string EpicorUserID = string.Empty;
        string EpicorUserPassword = string.Empty;
        string TreasuryCompany = string.Empty;
        string CommitCtrlLvl = string.Empty;

        string AppSrvUrl = string.Empty;       // This should be the url to your appserver        
        string EndpointBinding = string.Empty; // This is case sensitive. Valid values are "UsernameWindowsChannel", "Windows" and "UsernameSslChannel"

        static string EpicCNString = System.Configuration.ConfigurationManager.ConnectionStrings["cons"].ConnectionString.ToString();
        SqlConnection EpicCon = new SqlConnection(Convert.ToString(EpicCNString));
        string CompanyID = string.Empty;
        SqlDataAdapter DACOASeg = new SqlDataAdapter();
        SqlCommand sqlCmd = new SqlCommand();
        DataSet DsCOASegTMP = new DataSet();
        string SqlStr = string.Empty;        
        #endregion

        BusinessAccessLayer.GeneratePaymentBusinessObjects objBo = new BusinessAccessLayer.GeneratePaymentBusinessObjects();
        DataAccessLayer.GeneratePaymentDataObjects objDo = new DataAccessLayer.GeneratePaymentDataObjects();
        EventLog ObjEvent = new EventLog();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                TreasuryCompany = Convert.ToString(objDo.GetSysParam("TreasuryCompany"));
                CommitCtrlLvl = Convert.ToString(objDo.GetSysParam("CommitCtrlLvl"));

                string IsSalaryPaymentRequired = Convert.ToString(objDo.GetSysParam("IsSalaryPaymentRequired")).ToUpper();

                if (!string.IsNullOrEmpty(IsSalaryPaymentRequired))
                {
                    Session["IsSalaryPaymentRequired"] = IsSalaryPaymentRequired;
                }
                else
                {
                    Session["TransMessage"] = Convert.ToString("Configuration of Is Salary Payment Required missing in sysparam. please contact System Administrator.");
                    Response.Redirect("Stcl_ErrorMessage.aspx", false);
                    return;
                }

                if (string.IsNullOrEmpty(CommitCtrlLvl))
                {
                    Session["TransMessage"] = Convert.ToString("Configuration of Commitment Control Level is missing in sysparam. please contact System Administrator.");
                    Response.Redirect("Stcl_ErrorMessage.aspx", false);
                    return;
                }

                if (IsSalaryPaymentRequired == "TRUE")
                {
                    lblSalaryPayment.Visible = true;
                    chkSalaryPay.Visible = true;
                    if (chkSalaryPay.Checked == true)
                    {
                        ddlCompany.Enabled = false;
                        btnGetData.Text = "Search Payment Ref Number";
                    }
                    else
                    {
                        ddlCompany.Enabled = true;
                        btnGetData.Text = "Search Voucher List Number";
                    }
                }
                else
                {
                    lblSalaryPayment.Visible = false;
                    chkSalaryPay.Visible = false;
                }

                if (!IsPostBack)
                {
                    if (Session["UserId"] != null)
                    {
                        if (CommitCtrlLvl == "3")
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(TreasuryCompany).Trim()))
                            {
                                Session["TreasuryCompany"] = TreasuryCompany;
                            }
                            else
                            {
                                Session["TreasuryCompany"] = "";
                                ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('Treasury Company Id is missing. Please Contact Administrator.');", true);
                            }
                        }
                        else if (CommitCtrlLvl == "2")
                        {
                            Session["TreasuryCompany"] = "";
                        }
                        else if (CommitCtrlLvl == "1")
                        {
                            Session["TreasuryCompany"] = "";
                        }
                        else
                        {
                            Session["TreasuryCompany"] = "";
                            ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('Commitment control level is missing. Please Contact Administrator.');", true);
                        }
                        EpicorUserID = Session["UserId"].ToString();
                        EpicorUserPassword = Session["Password"].ToString();
                        LoadData();
                    }
                    else
                    {
                        Session.Clear();
                        Response.Redirect("Stcl_Login.aspx");
                    }
                }

                if ((ddlCompany.Items.Count > 0) && (ddlCompany.SelectedIndex < 1) && (string.IsNullOrEmpty(Convert.ToString(Session["SrcCompany"])) == false))
                {
                    ddlCompany.SelectedValue = Session["SrcCompany"].ToString();
                    btnGetData_Click(null, null);
                }
                Page.ClientScript.RegisterStartupScript(GetType(), "progressBar", "HideProgressPanel();", true);
            }
            catch (Exception ex)
            {
                Session["TransMessage"] = "Stcl_ConsolidateGeneratePaymentInfo.aspx=>Page_Load=>" + Convert.ToString(ex.Message);
                Response.Redirect("Stcl_ErrorMessage.aspx");
            }
        }

        protected void grvGeneratePaymentConsolidatedData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Proceed")
                {
                    SessionId = Convert.ToString(Session["SessionId"]);

                    int index = Convert.ToInt32(e.CommandArgument.ToString());
                    Label lblBankAcctID = (Label)grvGeneratePaymentConsolidatedData.Rows[index].FindControl("lblBankAcctID");
                    Label lblSrcCompany = (Label)grvGeneratePaymentConsolidatedData.Rows[index].FindControl("lblSrcCompany");
                    Label lblTotalRecord = (Label)grvGeneratePaymentConsolidatedData.Rows[index].FindControl("lblTotalRecord");
                    Label lblVoucherListNum = (Label)grvGeneratePaymentConsolidatedData.Rows[index].FindControl("lblVoucherListNum");

                    if (CommitCtrlLvl == "3")
                    {
                        Session["CompanyId"] = TreasuryCompany;
                    }
                    else if (CommitCtrlLvl == "2")
                    {
                        Session["CompanyId"] = ddlCompany.SelectedValue.ToString();
                    }
                    else if (CommitCtrlLvl == "1")
                    {
                        Session["CompanyId"] = ddlCompany.SelectedValue.ToString();
                    }
                    else
                    {
                        Session["TransMessage"] = "Generate Payment Web Portal is not allowed for this company, Please contact Administrator.";
                        Response.Redirect("Stcl_ErrorMessage.aspx");
                        return;
                    }

                    Session["BankAcctID"] = Convert.ToString(lblBankAcctID.Text).Trim();
                    Session["SrcCompany"] = Convert.ToString(lblSrcCompany.Text).Trim();
                    Session["VoucherListNum"] = Convert.ToString(lblVoucherListNum.Text).Trim();

                    DataSet Ds = new DataSet();
                    DataTable Dt = new DataTable();
                    DataRow Dr;

                    if (Application["DtCurrentSession"] == null)
                    {
                        DataColumn SrNo = new DataColumn();
                        SrNo.DataType = System.Type.GetType("System.Int32");
                        SrNo.AutoIncrement = true;
                        SrNo.AutoIncrementSeed = 1;
                        SrNo.AutoIncrementStep = 1;
                        Dt.Columns.Add(SrNo);
                        Dt.Columns.Add(new System.Data.DataColumn("BankAcctID", typeof(String)));
                        Dt.Columns.Add(new System.Data.DataColumn("SrcCompany", typeof(String)));
                        Dt.Columns.Add(new System.Data.DataColumn("CurrentSession", typeof(String)));
                        Dt.Columns.Add(new System.Data.DataColumn("VoucherListNum", typeof(String)));

                        Dr = Dt.NewRow();
                        Dr[1] = Convert.ToString(Session["BankAcctID"]);
                        Dr[2] = Convert.ToString(Session["SrcCompany"]);
                        Dr[3] = Convert.ToString(Session.SessionID);
                        Dr[4] = Convert.ToString(Session["VoucherListNum"]);
                        Dt.Rows.Add(Dr);

                        Application["DtCurrentSession"] = Dt;
                    }
                    else
                    {
                        Dt = (DataTable)Application["DtCurrentSession"];

                        DataRow[] drr = Dt.Select("CurrentSession='" + Convert.ToString(Session.SessionID) + "'");
                        if (drr.Length > 0)
                        {
                            for (int i = 0; i < drr.Length; i++)
                            {
                                drr[i].Delete();
                            }
                            Dt.AcceptChanges();
                        }

                        Dr = Dt.NewRow();
                        Dr[1] = Convert.ToString(Session["BankAcctID"]);
                        Dr[2] = Convert.ToString(Session["SrcCompany"]);
                        Dr[3] = Convert.ToString(Session.SessionID);
                        Dr[4] = Convert.ToString(Session["VoucherListNum"]);
                        Dt.Rows.Add(Dr);
                        Application["DtCurrentSession"] = Dt;
                    }
                    Response.Redirect("Stcl_GeneratePayment.aspx", false);
                }
            }
            catch (Exception ex)
            {
                Session["TransMessage"] = "Stcl_ConsolidateGeneratePaymentInfo.aspx=>grvGeneratePaymentConsolidatedData_RowCommand=>" + Convert.ToString(ex.Message);
                Response.Redirect("Stcl_ErrorMessage.aspx");
            }
        }

        protected void grvGeneratePaymentConsolidatedData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    LinkButton LnkGenPay = (LinkButton)e.Row.FindControl("linkGeneratePay");
                    Label LblSrcCompany = (Label)e.Row.FindControl("lblSrcCompany");
                    Label LblBankAcctID = (Label)e.Row.FindControl("lblBankAcctID");
                    Label LblVoucherListNum = (Label)e.Row.FindControl("lblVoucherListNum");
                    string SessionId = Convert.ToString(Session.SessionID);

                    DataTable Dt = new DataTable();

                    if (Application["DtCurrentSession"] != null)
                    {
                        Dt = (DataTable)Application["DtCurrentSession"];

                        DataRow[] DataRowToDelete = Dt.Select("CurrentSession='" + Convert.ToString(Session.SessionID) + "'");
                        if (DataRowToDelete.Length > 0)
                        {
                            for (int i = 0; i < DataRowToDelete.Length; i++)
                            {
                                DataRowToDelete[i].Delete();
                            }
                            Dt.AcceptChanges();
                        }
                    }

                    if (Application["DtCurrentSession"] != null)
                    {
                        Dt = (DataTable)Application["DtCurrentSession"];

                        DataRow[] DataRow = Dt.Select(" BankAcctID='" + Convert.ToString(LblBankAcctID.Text).Trim() + "' AND SrcCompany='" + Convert.ToString(LblSrcCompany.Text).Trim() + "' AND CurrentSession <> '" + Convert.ToString(Session.SessionID) + "' AND VoucherListNum = '" + Convert.ToString(LblVoucherListNum.Text).Trim() + "'");
                        if ((DataRow.Length > 0) && string.IsNullOrEmpty(LblVoucherListNum.Text))
                        {
                            LnkGenPay.Visible = false;
                        }
                        else
                        {
                            LnkGenPay.Visible = true;
                        }
                    }
                    else if (string.IsNullOrEmpty(LblVoucherListNum.Text)) //If voucher list number does not exist don't allow to generate payment
                    {
                        LnkGenPay.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Session["TransMessage"] = Convert.ToString(ex.Message);
                Response.Redirect("Stcl_ErrorMessage.aspx");
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Stcl_Login.aspx");
        }

        private void FillDdlCompany()
        {
            DataSet dsCompany = new DataSet();
            try
            {
                dsCompany = objDo.GetUserCompany(Session["UserId"].ToString());
                if (dsCompany.Tables.Count > 0 && dsCompany.Tables[0].Rows.Count > 0)
                {
                    ddlCompany.DataValueField = "Company";
                    ddlCompany.DataTextField = "Name";
                    ddlCompany.DataSource = dsCompany.Tables[0];
                    ddlCompany.DataBind();
                    ddlCompany.Items.Insert(0, new ListItem("--Select--", String.Empty));

                    bool exists = dsCompany.Tables[0].AsEnumerable().Where(c => c.Field<string>("Company").Equals(Convert.ToString(Session["TreasuryCompany"]))).Count() > 0;

                    if (exists == true)
                    {
                        Session["HaveTreasuryCompanyRights"] = true;
                    }
                    else
                    {
                        Session["HaveTreasuryCompanyRights"] = false;
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('No data for generate payment.');", true);
                }
            }
            catch (Exception ex)
            {
                Session["TransMessage"] = Convert.ToString(ex.Message);
                Response.Redirect("Stcl_ErrorMessage.aspx");
            }
            finally
            {
                dsCompany.Dispose();
            }
        }

        private void GetGenPmntData()
        {
            try
            {
                PaymentGeneratorUserId = Convert.ToString(Session["UserId"]);
                Password = Convert.ToString(Session["Password"]);
                CompanyId = ddlCompany.SelectedValue.ToString();               

                if (Convert.ToString(Session["IsSalaryPaymentRequired"]) == "TRUE")
                {
                    if (chkSalaryPay.Checked == true)
                    {
                        Session["IsSalaryPay"] = true;
                        CompanyId = "Salary Payment";
                    }
                    else
                    {
                        Session["IsSalaryPay"] = false;
                    }
                }
                else
                {
                    Session["IsSalaryPay"] = false;
                }

                if (!string.IsNullOrEmpty(Convert.ToString(Session["AppSrvUrl"])) && !string.IsNullOrEmpty(Convert.ToString(Session["EndpointBinding"])))
                {
                    #region Inside Connection
                    EpicorUserID = PaymentGeneratorUserId;
                    EpicorUserPassword = Password;
                    AppSrvUrl = Convert.ToString(Session["AppSrvUrl"]);
                    EndpointBinding = Convert.ToString(Session["EndpointBinding"]);

                    SessionModImpl SessionModImpl = CreateBusObj<SessionModImpl>(Guid.Empty, SessionModImpl.UriPath);
                    try
                    {
                        Guid sessionId = SessionModImpl.Login();
                        SessionModImpl.SessionID = sessionId;

                        if (SessionModImpl.SessionID != null)
                        {
                            Session["SessionId"] = SessionModImpl.SessionID;
                            SessionId = Convert.ToString(Session["SessionId"]);

                            if (!String.IsNullOrEmpty(CompanyId.Trim()) && !String.IsNullOrEmpty(PaymentGeneratorUserId.Trim()) && !string.IsNullOrEmpty(Convert.ToString(SessionId.Trim())))
                            {
                                DataSet DsGeneratePaymentConsolidatedData = new DataSet();
                                DsGeneratePaymentConsolidatedData = objDo.GetConsolidatedGeneratePaymentData(Convert.ToString(PaymentGeneratorUserId).Trim(), ddlCompany.SelectedValue.ToString(), txtBankAccId.Text.ToString().Replace("'", "''"), Convert.ToBoolean(Session["IsSalaryPay"]));
                                if (DsGeneratePaymentConsolidatedData.Tables.Count > 0 && DsGeneratePaymentConsolidatedData.Tables[0].Rows.Count > 0)
                                {
                                    grvGeneratePaymentConsolidatedData.DataSource = DsGeneratePaymentConsolidatedData.Tables[0];
                                    grvGeneratePaymentConsolidatedData.DataBind();
                                }
                                else
                                {
                                    grvGeneratePaymentConsolidatedData.DataSource = DsGeneratePaymentConsolidatedData.Tables[0];
                                    grvGeneratePaymentConsolidatedData.DataBind();
                                    ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('No data for generate payment.');", true);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.ToUpper() == "Invalid user ID or password.".ToUpper())
                        {
                            Session["TransMessage"] = Convert.ToString(ex.Message);
                            Response.Redirect("Stcl_ErrorMessage.aspx");
                        }
                        else
                        {
                            Session["TransMessage"] = Convert.ToString(ex.Message);
                            Response.Redirect("Stcl_ErrorMessage.aspx");
                        }
                    }
                    finally
                    {
                        SessionModImpl.Logout();
                        SessionModImpl.Dispose();
                    }
                    #endregion Connection Pool                  
                }
                else
                {
                    Session["TransMessage"] = "Session is expired!";
                    Response.Redirect("Stcl_ErrorMessage.aspx", false);
                }
            }
            catch (Exception ex)
            {
                Session["TransMessage"] = Convert.ToString(ex.Message);
                Response.Redirect("Stcl_ErrorMessage.aspx");
            }
        }

        #region Internal Methods (CreateBusObj,ShowCallerInfo,GetSysParam)
                
        public T CreateBusObj<T>(Guid sessionId, string uriPath) where T : ImplBase
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

        protected void btnGetData_Click(object sender, EventArgs e)
        {
            try
            {
                if (CommitCtrlLvl == "3")
                {
                    if (Convert.ToBoolean(Session["HaveTreasuryCompanyRights"]) == true)
                    {
                        GetGenPmntData();
                    }
                    else
                    {
                        if (Session["HaveTreasuryCompanyRights"] != null && Convert.ToBoolean(Session["HaveTreasuryCompanyRights"]) == false)
                        {
                            Session["TransMessage"] = Convert.ToString("You don't have treasury company rights. Please contact System Administrator!");
                        }
                        else
                        {
                            Session["TransMessage"] = "Session is expired!";
                        }
                        Response.Redirect("Stcl_ErrorMessage.aspx", false);
                    }
                }
                else if (CommitCtrlLvl == "2")
                {
                    GetGenPmntData();
                }
                else if (CommitCtrlLvl == "1")
                {
                    GetGenPmntData();
                }
            }
            catch (Exception ex)
            {
                Session["TransMessage"] = Convert.ToString(ex.Message);
                Response.Redirect("Stcl_ErrorMessage.aspx");
            }
        }

        private void LoadData()
        {
            try
            {
                if (Convert.ToString(Session["IsSSOLoginCompulsory"]) == "TRUE" && Convert.ToBoolean(Session["IsSSOUser"]) == false)
                {
                    if (Convert.ToBoolean(Session["IsSSOUser"]) == false)
                    {
                        Session["TransMessage"] = "You are not a valid SSO User!";
                    }
                    else
                    {
                        Session["TransMessage"] = "Session is expired!";
                    }
                    Response.Redirect("Stcl_ErrorMessage.aspx", false);
                }

                if (!string.IsNullOrEmpty(Convert.ToString(TreasuryCompany).Trim()))
                {
                    Session["TreasuryCompany"] = TreasuryCompany;
                    FillDdlCompany();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('Treasury Company Id is missing. Please Contact Administrator.');", true);
                }

                if (Session["SrcCompany"] != null)
                {
                    ddlCompany.SelectedValue = Convert.ToString(Session["SrcCompany"]);
                    GetGenPmntData();
                }
            }
            catch (Exception ex)
            {                
                ObjEvent.ExceptionEvent("GeneratePayment", "Stcl_ConsolidateGeneratePaymentInfo => LoadData", ex.Message.ToString(), ex);
            }
        }
    }
}