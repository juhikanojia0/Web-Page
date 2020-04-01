/*************************************************************
* Project   : CIFMIS_GeneratePayment Web Portal for E10.1
* Author    : Mahesh D. Deore
* Date      : 10 Dec 2015
* Purpose   : to Generate Payment process for selected Invoices [modified the code which is already developed for ERP 9]
* Comments : Some code is commented for Comments column. Keep it as it is, It may required
             if client want Comments to be save in Invoice.
             Some logical code is commented and comment is written to state the use of that code. 
             If required that logic we can use it after confirmation with Mr.Nitesh Parmar.
* Version   : 1.0.0.0

* Modified History
Version   Modified by           Modified on     Description
-------   ---------------       --------------  ------------------------------------------------------------------------
1.0.0.1   Mahesh Deore          31-Mar-2016     added code to uncheck checkboxes which was showing after generating payment for some records of same group (code modified at line No 743)
1.0.0.2   Mahesh Deore          08-Jun-2016     added code to show columns data with Invoice Amount in Invoice Currency & Invoice Amount in Base Currency as per requirement by Majid
1.0.0.3   Mahesh Deore          16-Jun-2016     added dynamic code as per CG Module
1.0.0.4   Rajesh                28-Jan-2017     VSO BUG ID 8628
1.0.0.5   Shekhar Chaudhari     20-Jun-2018     Task ID 16237:- Include Event Viewer information log recording functionality.
1.0.0.6   Shekhar Chaudhari     02-Jul-2018     Bug ID 17699:-  MOFTZ- Generate Payment Web Portal-After generating Payment from webportal,
                                                submit for approval checkbox of created headers of payment entry group are not auto ticked
1.0.0.7   Shekhar Chaudhari     14-Jan-2019     Task ID 21080:- Incorporate SiteID Chages related to Cost Center Segregation in GPW.
1.0.0.8   Mahesh Deore          21-Jun-2019     23570 - GPW Portal Issue - System is throwing an error 'Invalid Company XXXX for user XXXXXXXX' while more sessions are get activated
 * **************************************************************************************************************/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Ice.Proxy.Lib;
using Epicor.ServiceModel.Channels;
using Erp.Proxy.BO;
using Erp.BO;
using Stcl.Global.GlobalMethods;
using System.Text;

namespace Stcl.EpicorErp10.GeneratePaymentWebPortal.WebPages
{
    public partial class Stcl_GeneratePayment : System.Web.UI.Page
    {        
        string PaymentGeneratorUserId = string.Empty;
        string CompanyId = string.Empty;
        string SessionId = string.Empty;
        string BankAccctId = string.Empty;
        string SourceCompany = string.Empty;
        string Password = string.Empty;
        Decimal SumOfInvoiceAmount = 0;
        string VoucherListNum = string.Empty;
        bool GroupIdExist = false;

        //// Public variable declaration Added by MDD
        #region Public variable declaration Added by MDD
        string EpicorUserID = string.Empty;
        string EpicorUserPassword = string.Empty;        

        string AppSrvUrl = string.Empty;      // This should be the url to your appserver
        string EndpointBinding = string.Empty;// This is case sensitive. Valid values are "UsernameWindowsChannel", "Windows" and "UsernameSslChannel"

        public static string EpicCNString = System.Configuration.ConfigurationManager.ConnectionStrings["cons"].ConnectionString.ToString();

        public SqlConnection EpicCon = new SqlConnection(Convert.ToString(EpicCNString));
        public string CompanyID = string.Empty;
        public SqlDataAdapter DACOASeg = new SqlDataAdapter();
        public SqlCommand sqlCmd = new SqlCommand();
        public DataSet DsCOASegTMP = new DataSet();
        public string SqlStr = string.Empty;        
        #endregion

        BusinessAccessLayer.GeneratePaymentBusinessObjects ObjBO = new BusinessAccessLayer.GeneratePaymentBusinessObjects();
        DataAccessLayer.GeneratePaymentDataObjects ObjDO = new DataAccessLayer.GeneratePaymentDataObjects();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {   
                if (hfGPValue.Value == "1")
                {
                    GetData();
                }
                if (!IsPostBack)
                {
                    if (Session["UserId"] != null)
                    {
                        DivGeneratePaymentData.Visible = false;
                        DivAction.Visible = false;

                        EpicorUserID = Session["UserId"].ToString();
                        EpicorUserPassword = Session["Password"].ToString();

                        CompanyId = Convert.ToString(Session["CompanyId"]);
                        PaymentGeneratorUserId = Convert.ToString(Session["UserId"]);
                        SessionId = Convert.ToString(Session["SessionId"]);
                        BankAccctId = Convert.ToString(Session["BankAcctId"]);
                        SourceCompany = Convert.ToString(Session["SrcCompany"]);
                        Password = Convert.ToString(Session["Password"]);
                        VoucherListNum = Convert.ToString(Session["VoucherListNum"]);
                        ObjBO.IsSalaryPay = Convert.ToBoolean(Session["IsSalaryPay"]);

                        if (!String.IsNullOrEmpty(CompanyId) && !String.IsNullOrEmpty(PaymentGeneratorUserId) && !String.IsNullOrEmpty(SessionId))
                        {
                            lblTxtSourceCompany.Text = SourceCompany;
                            lblTxtBankAccountId.Text = BankAccctId;
                            lblTxtGenerator.Text = PaymentGeneratorUserId;
                            lblTxtSourceGroupId.Text = VoucherListNum;

                            if (ObjBO.IsSalaryPay == true)
                            {
                                lblTxtSourceCompany.Text = "Salary Payment";
                                lblSourceGroupId.Text = "Payment Ref No";
                            }
                        }
                        else
                        {
                            Session["TransMessage"] = "Session Expired!";
                            Response.Redirect("Stcl_ErrorMessage.aspx");
                        }
                        GetData();
                    }
                    else
                    {
                        Session.Clear();
                        Response.Redirect("Stcl_Login.aspx");
                    }
                }

                Session["CSharpCodeServerLogSwitch"] = Convert.ToString(ObjDO.GetSysParam("CSharpCodeServerLogSwitch")).ToUpper();

                if (string.IsNullOrEmpty(Convert.ToString(Session["CSharpCodeServerLogSwitch"])))
                {
                    Session["TransMessage"] = Convert.ToString("Configuration of CSharpCodeServerLogSwitch is missing in sysparam. please contact System Administrator.");
                    Response.Redirect("Stcl_ErrorMessage.aspx", false);
                    return;
                }

                Session["IsAutoSubmitForApproval"] = Convert.ToString(ObjDO.GetSysParam("IsAutoSubmitForApproval")).ToUpper();

                if (string.IsNullOrEmpty(Convert.ToString(Session["IsAutoSubmitForApproval"])))
                {
                    Session["TransMessage"] = Convert.ToString("Configuration of Is Auto Submit For Approval is missing in sysparam. please contact System Administrator.");
                    Response.Redirect("Stcl_ErrorMessage.aspx", false);
                    return;
                }
            }
            catch (Exception ex)
            {
                Session["TransMessage"] = "Stcl_GeneratePayment==>Page_Load=>" + Convert.ToString(ex.Message);
                Response.Redirect("Stcl_ErrorMessage.aspx");
                throw;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string StartTime = string.Empty;
            string EndTime = string.Empty;
            string WriteEventLog = string.Empty;

            EventLog ObjEvent = new EventLog();

            StartTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
            DataSet DsRecordsForGeneratePay = new DataSet();

            if (Session["CSharpCodeServerLogSwitch"] != null)
            {
                WriteEventLog = Convert.ToString(Session["CSharpCodeServerLogSwitch"]);
            }
            else
            {
                Session["TransMessage"] = "Session Expired!";
                Response.Redirect("Stcl_ErrorMessage.aspx", false);
            }

            if (WriteEventLog.ToUpper() == "ON")
            {
                ObjBO.AllowLogAppending = true;
            }
            StringBuilder LogMsg = new StringBuilder();

            if ((Convert.ToBoolean(Session["IsSSOUser"]) == false) && string.IsNullOrEmpty(Convert.ToString(Session["Password"])))
            {
                Session["TransMessage"] = "Invalid Session!";
                Response.Redirect("Stcl_ErrorMessage.aspx", false);
            }

            #region Save Process Start
            if (!String.IsNullOrEmpty(Convert.ToString(Session["CompanyId"])) && !String.IsNullOrEmpty(Convert.ToString(Session["UserId"])) && !string.IsNullOrEmpty(Convert.ToString(Session["AppSrvUrl"])) && !string.IsNullOrEmpty(Convert.ToString(Session["EndpointBinding"])))
            {
                PaymentGeneratorUserId = Convert.ToString(Session["UserId"]);
                Password = Convert.ToString(Session["Password"]);
                CompanyId = Convert.ToString(Session["CompanyId"]);
                
                ObjBO.IsSalaryPay = Convert.ToBoolean(Session["IsSalaryPay"]);
                EpicorUserID = Session["UserId"].ToString();
                EpicorUserPassword = Session["Password"].ToString();
                AppSrvUrl = Convert.ToString(Session["AppSrvUrl"]);
                EndpointBinding = Convert.ToString(Session["EndpointBinding"]);

                LogMsg.AppendLine("AppSrvUrl : " + AppSrvUrl + " EndpointBinding : " + EndpointBinding);

                #region Connection Pool
                SessionModImpl SessionModImpl = CreateBusObj<SessionModImpl>(Guid.Empty, SessionModImpl.UriPath);
                
                #region Inside Connection
                try
                {
                    Guid sessionId = SessionModImpl.Login();
                    SessionModImpl.SessionID = sessionId;

                    if (SessionModImpl.SessionID != null)
                    {
                        string CPOBankAcctID = string.Empty;
                        string CurrentSelectedCompany = string.Empty;
                        string Generator = string.Empty;

                        CurrentSelectedCompany = Convert.ToString(lblTxtSourceCompany.Text);
                        CPOBankAcctID = Convert.ToString(lblTxtBankAccountId.Text);
                        Generator = Convert.ToString(lblTxtGenerator.Text);
                                                                     
                        try
                        {
                            string CompanyName, PlantID, PlantName, WorkstationID, WorkstationDesc, EmployeeID, CountryGroupCode, CountryCode, TenantID;
                            SessionModImpl.SetCompany(Convert.ToString(Session["CompanyId"]), out CompanyName, out PlantID, out PlantName, out WorkstationID, out WorkstationDesc, out EmployeeID, out CountryGroupCode, out CountryCode, out TenantID);

                            string Company = string.Empty;
                            string InvoiceNum = string.Empty;
                            string PrimaryBankId = string.Empty;
                            string OrigPrimaryBankId = string.Empty;
                            Int32 VendorNum = 0;
                            decimal InvoiceAmt = 0;
                            string Error = string.Empty;
                            string SrcCompany = string.Empty;
                            string SrcGroupID = string.Empty; //Voucher List Number
                            string GroupId = string.Empty;
                            Int32 PMUID = 0;
                            Int32 HeadNum = 0;
                            string VendorID = string.Empty;
                            string CurrencyCode = string.Empty;
                            bool RequiresUserInput = false;
                            string OcExchResp = string.Empty;
                            string PayRefNo = string.Empty;
                            int RowNum = 0;

                            Company = Session["CompanyId"].ToString();

                            #region Get Default Site ID                           
                            string SiteID = string.Empty;
                            string SqlStr1 = "SELECT DefaultPlant " +
                                                " FROM Erp.XaSyst WITH (NOLOCK) " +
                                                " WHERE Company = '" + Company + "'";
                            SiteID = ObjDO.GetScalarValue(SqlStr1);

                            LogMsg.AppendLine("SiteID : " + SiteID);
                            #endregion

                            if (!string.IsNullOrEmpty(Convert.ToString(Company).Trim()))
                            {
                                DsRecordsForGeneratePay = GetGeneratePayRecordForProceed();

                                if (DsRecordsForGeneratePay.Tables.Count > 0 && DsRecordsForGeneratePay.Tables[0].Rows.Count > 0)
                                {
                                    if (ObjBO.AllowLogAppending == true)
                                    {
                                        LogMsg.AppendLine("Login User : " + Generator);
                                        LogMsg.AppendLine("Total Records For Process" + Convert.ToString(DsRecordsForGeneratePay.Tables[0].Rows.Count));
                                    }

                                    DataTable Dt = new DataTable();
                                    DataRow Dr;
                                    Dt.Columns.Add(new System.Data.DataColumn("SrNo", typeof(int)));
                                    Dt.Columns.Add(new System.Data.DataColumn("BankAcctID", typeof(String)));
                                    Dt.Columns.Add(new System.Data.DataColumn("SrcCompany", typeof(String)));
                                    Dt.Columns.Add(new System.Data.DataColumn("InvoiceNum", typeof(String)));
                                    Dt.Columns.Add(new System.Data.DataColumn("Status", typeof(String)));
                                    Dt.Columns.Add(new System.Data.DataColumn("Time", typeof(String)));

                                    #region ForLoop
                                    for (int Row = 0; Row < DsRecordsForGeneratePay.Tables[0].Rows.Count; Row++)
                                    {
                                        if (ObjBO.AllowLogAppending == true)
                                        {
                                            LogMsg.AppendLine("");
                                            LogMsg.AppendLine("Row No = " + Row + " Start Time = " + Convert.ToString(DateTime.Now));
                                        }

                                        #region IfCondition
                                        if (Convert.ToBoolean(DsRecordsForGeneratePay.Tables[0].Rows[Row]["ChkAction"]) == true)
                                        {
                                            try
                                            {
                                                #region APChkGrpAdapter

                                                InvoiceNum = Convert.ToString(DsRecordsForGeneratePay.Tables[0].Rows[Row]["InvoiceNum"]);
                                                VendorNum = Convert.ToInt32(DsRecordsForGeneratePay.Tables[0].Rows[Row]["VendorNum"]);
                                                InvoiceAmt = Convert.ToDecimal(DsRecordsForGeneratePay.Tables[0].Rows[Row]["InvoiceAmt"]);
                                                SrcCompany = Convert.ToString(Session["SrcCompany"]); // Company;
                                                SrcGroupID = Convert.ToString(DsRecordsForGeneratePay.Tables[0].Rows[Row]["SrcGroupID"]);
                                                PMUID = Convert.ToInt32(DsRecordsForGeneratePay.Tables[0].Rows[Row]["PMUID"]);
                                                VendorID = Convert.ToString(DsRecordsForGeneratePay.Tables[0].Rows[Row]["VendorID"]);
                                                CurrencyCode = Convert.ToString(DsRecordsForGeneratePay.Tables[0].Rows[Row]["CurrencyCode"]).Split(' ')[0]; //changes done to get proper currencyCode. add the split logic bcz enhancement done to show Invoice & base currency. 08-Jun-2016
                                                PayRefNo = Convert.ToString(DsRecordsForGeneratePay.Tables[0].Rows[Row]["PayRefNo"]);
                                                HeadNum = 0;
                                                
                                                APChkGrpImpl APChkGrpImp = CreateBusObj<APChkGrpImpl>(sessionId, APChkGrpImpl.UriPath);
                                                APChkGrpDataSet APChkGrpDS = new APChkGrpDataSet();
                                                VendorImpl VendImpls = CreateBusObj<VendorImpl>(sessionId, VendorImpl.UriPath);
                                                VendorDataSet DsVendor = new VendorDataSet();

                                                DataSet DsValidateData = new DataSet();
                                                if (ObjBO.AllowLogAppending == true)
                                                {
                                                    LogMsg.AppendLine("Current InvoiceNum : " + InvoiceNum + " VendorNum : " + Convert.ToString(VendorNum) + " Source Company : " + SrcCompany + " InvoiceAmt : " + Convert.ToString(InvoiceAmt) + " VoucherListNum : " + SrcGroupID + " PMUID : " + Convert.ToString(PMUID) + " CurrencyCode : " + CurrencyCode + " PayRefNo : " + PayRefNo + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                }

                                                //Validate each record before generate payment
                                                DsValidateData = ObjDO.ValidateAndGetData(1, Company, VendorNum, CPOBankAcctID, SrcCompany, Generator, SrcGroupID, InvoiceNum, InvoiceAmt, ObjBO.IsSalaryPay, PayRefNo);

                                                if (DsValidateData.Tables.Count > 0 && DsValidateData.Tables[0].Rows.Count > 0)
                                                {
                                                    GroupIdExist = true; //Flag set here if GroupId is exist don't delete it, if having any exception during this process.
                                                    GroupId = Convert.ToString(DsValidateData.Tables[0].Rows[0]["GroupID"]).Trim();

                                                    if (ObjBO.AllowLogAppending == true)
                                                    {
                                                        LogMsg.AppendLine("Group ID Exist : GroupID = " + GroupId + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                    }

                                                    if (!string.IsNullOrEmpty(Convert.ToString(DsValidateData.Tables[0].Rows[0]["ErrorMessage"]).Trim()))
                                                    {
                                                        Dr = Dt.NewRow();
                                                        EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));

                                                        Dr[1] = Convert.ToString(CPOBankAcctID);
                                                        Dr[2] = Convert.ToString(SrcCompany);
                                                        Dr[3] = Convert.ToString(InvoiceNum);
                                                        Dr[4] = Convert.ToString(DsValidateData.Tables[0].Rows[0]["ErrorMessage"]).Trim();
                                                        Dr[5] = EndTime;
                                                        Dt.Rows.Add(Dr);

                                                        LogMsg.AppendLine("Group ID Exist : GroupID = " + Convert.ToString(DsValidateData.Tables[0].Rows[0]["ErrorMessage"]).Trim() + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                        //Stop executing further code as error message recorded in database and move to the next iteration.
                                                        continue;
                                                    }
                                                    else if (string.IsNullOrEmpty(GroupId))
                                                    {
                                                        GroupIdExist = false; //Flag set false here if GroupId does not exist delete it if having any exception during this process.

                                                        if (ObjBO.AllowLogAppending == true)
                                                        {
                                                            LogMsg.AppendLine("Group ID Not xist :" + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                            LogMsg.AppendLine("APChkGrp Object declaration started. " + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                        }

                                                        DataSet DsGroupId = new DataSet();
                                                        if (ObjBO.AllowLogAppending == true)
                                                        {
                                                            LogMsg.AppendLine("APChkGrp Object declaration ended. " + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                        }
                                                        //Get new group id                                                      
                                                        DsGroupId = ObjDO.GetPayGroupID(Company, SrcCompany, ObjBO.IsSalaryPay);
                                                        LogMsg.AppendLine("GetPayGroupID Executed: Recorded Time = " + Convert.ToString(DateTime.Now));

                                                        if (DsGroupId.Tables.Count > 0 && DsGroupId.Tables[0].Rows.Count > 0)
                                                        {
                                                            #region APChkGrp Code

                                                            GroupId = Convert.ToString(DsGroupId.Tables[0].Rows[0]["PayGroupID"]).Trim();

                                                            if (ObjBO.AllowLogAppending == true)
                                                            {
                                                                LogMsg.AppendLine("New Group ID : " + GroupId + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                            }

                                                            try
                                                            {
                                                                APChkGrpImp.GetNewAPChkGrp(APChkGrpDS);

                                                                if (ObjBO.AllowLogAppending == true)
                                                                {
                                                                    LogMsg.AppendLine("Called GetNewAPChkGrp BO Method successfully." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                }
                                                            }
                                                            catch (Exception exAPChkGrp)
                                                            {
                                                                Dr = Dt.NewRow();
                                                                EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
                                                                Dr[1] = Convert.ToString(CPOBankAcctID);
                                                                Dr[2] = Convert.ToString(SrcCompany);
                                                                Dr[3] = Convert.ToString(InvoiceNum);
                                                                Dr[4] = Convert.ToString(exAPChkGrp.Message);
                                                                Dr[5] = EndTime;
                                                                Dt.Rows.Add(Dr);

                                                                if (ObjBO.AllowLogAppending == true)
                                                                {
                                                                    LogMsg.AppendLine("Exception on ApChkGrp.OnChangeBankAcctID BO Method call." + " Recorded Time = " + Convert.ToString(DateTime.Now));                                                                    
                                                                    ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentEntry", LogMsg.ToString(), exAPChkGrp);
                                                                }
                                                                //Stop executing further code as error message recorded in database and move to the next iteration.
                                                                continue;
                                                            }

                                                            if (Convert.ToString(APChkGrpDS.APChkGrp[0].RowMod) == "A")
                                                            {
                                                                APChkGrpDS.APChkGrp[0].GroupID = GroupId;
                                                                APChkGrpDS.APChkGrp[0]["SrcCompany_c"] = SrcCompany;
                                                                if (ObjBO.IsSalaryPay == true)
                                                                {
                                                                    APChkGrpDS.APChkGrp.Rows[0]["PayRefNo_c"] = PayRefNo; //Payment reference number stored to identify salary paryment records
                                                                }
                                                                APChkGrpDS.APChkGrp[0].CurrencyCode = CurrencyCode;
                                                                APChkGrpDS.APChkGrp[0].PMUID = PMUID;
                                                            }

                                                            try
                                                            {
                                                                APChkGrpImp.OnChangeBankAcctID(CPOBankAcctID, APChkGrpDS);

                                                                if (ObjBO.AllowLogAppending == true)
                                                                {
                                                                    LogMsg.AppendLine("Called ApChkGrp.OnChangeBankAcctID BO Method successfully." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                }

                                                                APChkGrpImp.GetDisplayTotals(CurrencyCode, APChkGrpDS);
                                                                //above method added by mdd2
                                                                if (ObjBO.AllowLogAppending == true)
                                                                {
                                                                    LogMsg.AppendLine("Called ApChkGrp.GetDisplayTotals BO Method successfully." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                }

                                                            }
                                                            catch (Exception exBankAcctId)
                                                            {
                                                                Dr = Dt.NewRow();
                                                                EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
                                                                Dr[1] = Convert.ToString(CPOBankAcctID);
                                                                Dr[2] = Convert.ToString(SrcCompany);
                                                                Dr[3] = Convert.ToString(InvoiceNum);
                                                                Dr[4] = Convert.ToString(exBankAcctId.Message);
                                                                Dr[5] = EndTime;
                                                                Dt.Rows.Add(Dr);
                                                                if (ObjBO.AllowLogAppending == true)
                                                                {
                                                                    LogMsg.AppendLine("Exception on ApChkGrp.OnChangeBankAcctID BO Method call." + " Recorded Time = " + Convert.ToString(DateTime.Now));                                                                    
                                                                    ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentEntry", LogMsg.ToString(), exBankAcctId);
                                                                }

                                                                //Stop executing further code as error message recorded in database and move to the next iteration.
                                                                continue;
                                                            }

                                                            try
                                                            {
                                                                LogMsg.AppendLine("Group Created By => "+ Convert.ToString(APChkGrpDS.APChkGrp.Rows[0]["CreatedBy"]));

                                                                APChkGrpImp.Update(APChkGrpDS);

                                                                if (ObjBO.AllowLogAppending == true)
                                                                {
                                                                    LogMsg.AppendLine("Called ApChkGrp.Update BO Method 1st time successfully." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                }
                                                            }
                                                            catch (Exception exUpdate)
                                                            {
                                                                Dr = Dt.NewRow();
                                                                EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));

                                                                Dr[1] = Convert.ToString(CPOBankAcctID);
                                                                Dr[2] = Convert.ToString(SrcCompany);
                                                                Dr[3] = Convert.ToString(InvoiceNum);
                                                                if (exUpdate.Message.ToString().ToUpper() == "THE GROUP ALREADY EXISTS.")
                                                                {
                                                                    Dr[4] = Convert.ToString(exUpdate.Message.ToString() + " Delete Group ID : " + GroupId + " from Payment Entry.");
                                                                }
                                                                else
                                                                {
                                                                    Dr[4] = Convert.ToString(exUpdate.Message);
                                                                }
                                                                Dr[5] = EndTime;
                                                                Dt.Rows.Add(Dr);

                                                                if (ObjBO.AllowLogAppending == true)
                                                                {
                                                                    LogMsg.AppendLine("Exception on ApChkGrp.Update BO Method call." + " Recorded Time = " + Convert.ToString(DateTime.Now));                                                                    
                                                                    ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentEntry", LogMsg.ToString(), exUpdate);
                                                                }
                                                                //Stop executing further code as error message recorded in database and move to the next iteration.
                                                                continue;
                                                            }

                                                            GroupId = Convert.ToString(APChkGrpDS.Tables["APChkGrp"].Rows[0]["GroupID"]);
                                                          
                                                            if (Convert.ToString(APChkGrpDS.APChkGrp[0].Company) == Company &&
                                                                Convert.ToString(APChkGrpDS.APChkGrp[0].GroupID) == GroupId)
                                                            {
                                                                APChkGrpDS.APChkGrp[0].PMUID = PMUID;
                                                                APChkGrpDS.APChkGrp[0].ActiveUserID = "";
                                                                APChkGrpDS.APChkGrp[0]["SiteID_c"] = SiteID;
                                                                APChkGrpImp.Update(APChkGrpDS);
                                                                
                                                                if (ObjBO.AllowLogAppending == true)
                                                                {
                                                                    LogMsg.AppendLine("Called ApChkGrp.Update BO Method 2nd time successfully." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                }
                                                            }
                                                            #endregion APChkGrp Code
                                                        }
                                                        else
                                                        {
                                                            //Group Id not get generated
                                                            Dr = Dt.NewRow();
                                                            EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
                                                            Dr[1] = Convert.ToString(CPOBankAcctID);
                                                            Dr[2] = Convert.ToString(SrcCompany);
                                                            Dr[3] = Convert.ToString(InvoiceNum);
                                                            Dr[4] = "Group ID is not get generated. Please contact administrator.";
                                                            Dr[5] = EndTime;
                                                            Dt.Rows.Add(Dr);

                                                            if (ObjBO.AllowLogAppending == true)
                                                            {
                                                                LogMsg.AppendLine("Group ID is not get generated." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                            }

                                                            //Stop executing further code as error message recorded in database and move to the next iteration.
                                                            continue;
                                                        }
                                                    }

                                                    #region "Payment Entry"                                                    
                                                    PaymentEntryImpl PayEntryImp = CreateBusObj<PaymentEntryImpl>(sessionId, PaymentEntryImpl.UriPath);
                                                    PaymentEntryDataSet PayEntryDS = new PaymentEntryDataSet();

                                                    LogMsg.AppendLine("Check Head Creation Started : GroupId =" + Convert.ToString(GroupId) + " ObjBO.VendorNum" + Convert.ToString(VendorNum) + " Recorded Time = " + Convert.ToString(DateTime.Now));


                                                    if (ObjBO.IsSalaryPay == true)
                                                    {
                                                        ObjBO.HeadNumExists = false;
                                                        HeadNum = ObjDO.GetHeadNum(CompanyID, GroupId, VendorNum, ObjBO.IsSalaryPay);

                                                        if (ObjBO.AllowLogAppending == true)
                                                        {
                                                            LogMsg.AppendLine("Fetched HeadNum for Salary Payment Invoice,  HeadNum=" + Convert.ToString(HeadNum) + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                        }
                                                    }
                                                    if (HeadNum == 0)
                                                    {
                                                        PayEntryImp.CreateNewCheckHed(GroupId, PayEntryDS);
                                                        if (ObjBO.AllowLogAppending == true)
                                                        {
                                                            LogMsg.AppendLine("Called PaymentEntry.CreateNewCheckHed BO Method successfully." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                        }

                                                        bool PostAllow = false;
                                                        PayEntryImp.PostAllowed(GroupId, out PostAllow);

                                                        bool ElecInterface = false;
                                                        PayEntryImp.GetElecInterface(GroupId, out ElecInterface);
                                                        //above line added by MDD2 to get Pay entry details

                                                        if ((Convert.ToString(PayEntryDS.CheckHed[0].RowMod) == "A") || (Convert.ToString(PayEntryDS.CheckHed[0].RowMod) == ""))
                                                        {
                                                            HeadNum = Convert.ToInt32(PayEntryDS.Tables["CheckHed"].Rows[0]["HeadNum"]);
                                                            if (ObjBO.AllowLogAppending == true)
                                                            {
                                                                LogMsg.AppendLine("HeadNum : " + HeadNum + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                            }

                                                            PayEntryDS.CheckHed[0].PMUID = PMUID;
                                                            PayEntryDS.CheckHed[0].InvoiceNum = InvoiceNum;
                                                            PayEntryDS.CheckHed[0].VendorBankID = PrimaryBankId;
                                                            PayEntryDS.CheckHed[0]["SiteID_c"] = SiteID;
                                                            DsValidateData = new DataSet();
                                                            DsValidateData = ObjDO.ValidateAndGetData(2, Company, VendorNum, CPOBankAcctID, SrcCompany, Generator, SrcGroupID, InvoiceNum, InvoiceAmt, ObjBO.IsSalaryPay, PayRefNo);

                                                            if (DsValidateData.Tables.Count > 0 && DsValidateData.Tables[0].Rows.Count > 0)
                                                            {
                                                                if (!string.IsNullOrEmpty(Convert.ToString(DsValidateData.Tables[0].Rows[0]["ErrorMessage"])))
                                                                {
                                                                    Dr = Dt.NewRow();
                                                                    EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));

                                                                    Dr[1] = Convert.ToString(CPOBankAcctID);
                                                                    Dr[2] = Convert.ToString(SrcCompany);
                                                                    Dr[3] = Convert.ToString(InvoiceNum);
                                                                    Dr[4] = Convert.ToString(DsValidateData.Tables[0].Rows[0]["ErrorMessage"]).Trim();
                                                                    Dr[5] = EndTime;
                                                                    Dt.Rows.Add(Dr);

                                                                    //======================================================================================================
                                                                    //Delete newly created Check head information from CheckHed table due to exception
                                                                    PayEntryImp.DeleteByID(Convert.ToInt32(PayEntryDS.Tables["CheckHed"].Rows[0]["HeadNum"]));
                                                                    //Delete newly created Group data from ApChkGrp table due to exception                                                                                        
                                                                    if (GroupIdExist == false)
                                                                    {
                                                                        //commented because DeleteByID is not working. Its native issue, already checked with BL-Tester.
                                                                        ////APChkGrpImp.DeleteByID(GroupId);
                                                                        //DeleteApChkGrp(GroupId);
                                                                        APChkGrpDS.APChkGrp[0].RowMod = "D";
                                                                        APChkGrpDS.APChkGrp[0].Delete();
                                                                        APChkGrpImp.Update(APChkGrpDS);
                                                                    }
                                                                    //======================================================================================================
                                                                    if (ObjBO.AllowLogAppending == true)
                                                                    {
                                                                        LogMsg.AppendLine("ObjDo.ValidateAndGetData : Error : -" + Convert.ToString(DsValidateData.Tables[0].Rows[0]["ErrorMessage"]) + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                    }

                                                                    //Stop executing further code as error message recorded in database and move to the next iteration.
                                                                    continue;
                                                                }
                                                                else
                                                                {
                                                                    DsValidateData = new DataSet();
                                                                    DsValidateData = ObjDO.GetCashInfo(Company, CPOBankAcctID, "", InvoiceAmt);
                                                                    if (DsValidateData.Tables.Count > 0 && DsValidateData.Tables[0].Rows.Count > 0)
                                                                    {
                                                                        if (!string.IsNullOrEmpty(Convert.ToString(DsValidateData.Tables[0].Rows[0]["ErrorMessage"])))
                                                                        {
                                                                            Dr = Dt.NewRow();
                                                                            EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));

                                                                            Dr[1] = Convert.ToString(CPOBankAcctID);
                                                                            Dr[2] = Convert.ToString(SrcCompany);
                                                                            Dr[3] = Convert.ToString(InvoiceNum);
                                                                            Dr[4] = Convert.ToString(DsValidateData.Tables[0].Rows[0]["ErrorMessage"]).Trim();
                                                                            Dr[5] = EndTime;
                                                                            Dt.Rows.Add(Dr);

                                                                            //======================================================================================================
                                                                            //Delete newly created Check head information from CheckHed table due to exception
                                                                            PayEntryImp.DeleteByID(Convert.ToInt32(PayEntryDS.Tables["CheckHed"].Rows[0]["HeadNum"]));
                                                                            //Delete newly created Group data from ApChkGrp table due to exception                                                                                               
                                                                            if (GroupIdExist == false)
                                                                            {
                                                                                //commented because DeleteByID is not working. Its native issue, already checked with BL-Tester.
                                                                                ////APChkGrpImp.DeleteByID(GroupId);
                                                                                //DeleteApChkGrp(GroupId);
                                                                                APChkGrpDS.APChkGrp[0].RowMod = "D";
                                                                                APChkGrpDS.APChkGrp[0].Delete();
                                                                                APChkGrpImp.Update(APChkGrpDS);
                                                                            }
                                                                            //======================================================================================================

                                                                            if (ObjBO.AllowLogAppending == true)
                                                                            {
                                                                                LogMsg.AppendLine("ObjDo.GetCashInfo : Error : -" + Convert.ToString(DsValidateData.Tables[0].Rows[0]["ErrorMessage"]) + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                            }
                                                                            //Stop executing further code as error message recorded in database and move to the next iteration.
                                                                            continue;
                                                                        }

                                                                        try
                                                                        {
                                                                            DsVendor = VendImpls.GetByID(VendorNum);
                                                                            OrigPrimaryBankId = ObjDO.GetPrimaryBankOrig(Company, VendorNum);
                                                                            PrimaryBankId = ObjDO.GetPrimaryBank(Company, VendorNum, InvoiceNum);
                                                                            DsVendor.Tables["Vendor"].Rows[0]["PrimaryBankID"] = PrimaryBankId;
                                                                            VendImpls.Update(DsVendor);

                                                                            PayEntryImp.OnChangeVendor(VendorID, PayEntryDS);

                                                                            if (ObjBO.AllowLogAppending == true)
                                                                            {
                                                                                LogMsg.AppendLine("Called PaymentEntry.OnChangeVendor BO Method successfully." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                            }
                                                                        }
                                                                        catch (Exception exChangeVendor)
                                                                        {
                                                                            Dr = Dt.NewRow();
                                                                            EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));

                                                                            Dr[1] = Convert.ToString(CPOBankAcctID);
                                                                            Dr[2] = Convert.ToString(SrcCompany);
                                                                            Dr[3] = Convert.ToString(InvoiceNum);
                                                                            Dr[4] = Convert.ToString(exChangeVendor.Message).Trim();
                                                                            Dr[5] = EndTime;
                                                                            Dt.Rows.Add(Dr);

                                                                            //======================================================================================================
                                                                            //Delete newly created Check head information from CheckHed table due to exception
                                                                            PayEntryImp.DeleteByID(Convert.ToInt32(PayEntryDS.Tables["CheckHed"].Rows[0]["HeadNum"]));
                                                                            //Delete newly created Group data from ApChkGrp table due to exception
                                                                            if (GroupIdExist == false)
                                                                            {
                                                                                //commented because DeleteByID is not working. Its native issue, already checked with BL-Tester.
                                                                                ////APChkGrpImp.DeleteByID(GroupId);
                                                                                //DeleteApChkGrp(GroupId);
                                                                                APChkGrpDS.APChkGrp[0].RowMod = "D";
                                                                                APChkGrpDS.APChkGrp[0].Delete();
                                                                                APChkGrpImp.Update(APChkGrpDS);
                                                                            }
                                                                            //======================================================================================================
                                                                            if (ObjBO.AllowLogAppending == true)
                                                                            {
                                                                                LogMsg.AppendLine("Exception on PaymentEntry.OnChangeVendor BO Method call." + " Recorded Time = " + Convert.ToString(DateTime.Now));                                                                                
                                                                                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentEntry", LogMsg.ToString(), exChangeVendor);
                                                                            }
                                                                            //Stop executing further code as error message recorded in database and move to the next iteration.
                                                                            continue;
                                                                        }

                                                                        try
                                                                        {
                                                                            PayEntryImp.OnChangeCurrency(CurrencyCode, PayEntryDS);

                                                                            if (ObjBO.AllowLogAppending == true)
                                                                            {
                                                                                LogMsg.AppendLine("Called PaymentEntry.OnChangeCurrency BO Method successfully." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                            }

                                                                            bool PostAllowCurr = false;
                                                                            PayEntryImp.PostAllowed(GroupId, out PostAllowCurr);
                                                                        }
                                                                        catch (Exception exChangeCurrency)
                                                                        {
                                                                            Dr = Dt.NewRow();
                                                                            EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));

                                                                            Dr[1] = Convert.ToString(CPOBankAcctID);
                                                                            Dr[2] = Convert.ToString(SrcCompany);
                                                                            Dr[3] = Convert.ToString(InvoiceNum);
                                                                            Dr[4] = Convert.ToString(exChangeCurrency.Message).Trim();
                                                                            Dr[5] = EndTime;
                                                                            Dt.Rows.Add(Dr);

                                                                            //======================================================================================================
                                                                            //Delete newly created Check head information from CheckHed table due to exception
                                                                            PayEntryImp.DeleteByID(Convert.ToInt32(PayEntryDS.Tables["CheckHed"].Rows[0]["HeadNum"]));
                                                                            //Delete newly created Group data from ApChkGrp table due to exception
                                                                            if (GroupIdExist == false)
                                                                            {
                                                                                //commented because DeleteByID is not working. Its native issue, already checked with BL-Tester.
                                                                                ////APChkGrpImp.DeleteByID(GroupId);
                                                                                //DeleteApChkGrp(GroupId);
                                                                                APChkGrpDS.APChkGrp[0].RowMod = "D";
                                                                                APChkGrpDS.APChkGrp[0].Delete();
                                                                                APChkGrpImp.Update(APChkGrpDS);
                                                                            }
                                                                            //======================================================================================================
                                                                            if (ObjBO.AllowLogAppending == true)
                                                                            {
                                                                                LogMsg.AppendLine("Exception on PaymentEntry.OnChangeCurrency BO Method call." + " Recorded Time = " + Convert.ToString(DateTime.Now));                                                                                
                                                                                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentEntry", LogMsg.ToString(), exChangeCurrency);
                                                                            }
                                                                            //Stop executing further code as error message recorded in database and move to the next iteration.
                                                                            continue;
                                                                        }

                                                                        try
                                                                        {
                                                                            PayEntryImp.PreUpdate(PayEntryDS, out RequiresUserInput);

                                                                            if (ObjBO.AllowLogAppending == true)
                                                                            {
                                                                                LogMsg.AppendLine("Called PaymentEntry.PreUpdate BO Method successfully." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                            }
                                                                        }
                                                                        catch (Exception exPreUpdate)
                                                                        {
                                                                            Dr = Dt.NewRow();
                                                                            EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));

                                                                            Dr[1] = Convert.ToString(CPOBankAcctID);
                                                                            Dr[2] = Convert.ToString(SrcCompany);
                                                                            Dr[3] = Convert.ToString(InvoiceNum);
                                                                            Dr[4] = Convert.ToString(exPreUpdate.Message).Trim();
                                                                            Dr[5] = EndTime;
                                                                            Dt.Rows.Add(Dr);

                                                                            //======================================================================================================
                                                                            //Delete newly created Check head information from CheckHed table due to exception
                                                                            PayEntryImp.DeleteByID(Convert.ToInt32(PayEntryDS.Tables["CheckHed"].Rows[0]["HeadNum"]));
                                                                            //Delete newly created Group data from ApChkGrp table due to exception
                                                                            if (GroupIdExist == false)
                                                                            {
                                                                                //commented because DeleteByID is not working. Its native issue, already checked with BL-Tester.
                                                                                ////APChkGrpImp.DeleteByID(GroupId);
                                                                                //DeleteApChkGrp(GroupId);
                                                                                APChkGrpDS.APChkGrp[0].RowMod = "D";
                                                                                APChkGrpDS.APChkGrp[0].Delete();
                                                                                APChkGrpImp.Update(APChkGrpDS);
                                                                            }
                                                                            //======================================================================================================
                                                                            if (ObjBO.AllowLogAppending == true)
                                                                            {
                                                                                LogMsg.AppendLine("Exception on PaymentEntry.PreUpdate BO Method call." + " Recorded Time = " + Convert.ToString(DateTime.Now));                                                                                
                                                                                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentEntry", LogMsg.ToString(), exPreUpdate);
                                                                            }
                                                                            //Stop executing further code as error message recorded in database and move to the next iteration.
                                                                            continue;
                                                                        }

                                                                        try
                                                                        {
                                                                            if (ObjBO.AllowLogAppending == true)
                                                                            {
                                                                                LogMsg.AppendLine("PaymentEntry.Update BO Method Starting." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                            }

                                                                            PayEntryImp.Update(PayEntryDS);

                                                                            if (ObjBO.AllowLogAppending == true)
                                                                            {
                                                                                LogMsg.AppendLine("Called PaymentEntry.Update BO Method successfully." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                            }

                                                                        }
                                                                        catch (Exception exPayUpdate)
                                                                        {
                                                                            Dr = Dt.NewRow();
                                                                            EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));

                                                                            Dr[1] = Convert.ToString(CPOBankAcctID);
                                                                            Dr[2] = Convert.ToString(SrcCompany);
                                                                            Dr[3] = Convert.ToString(InvoiceNum);
                                                                            Dr[4] = Convert.ToString(exPayUpdate.Message).Trim();
                                                                            Dr[5] = EndTime;
                                                                            Dt.Rows.Add(Dr);

                                                                            //======================================================================================================
                                                                            //Delete newly created Check head information from CheckHed table due to exception
                                                                            PayEntryImp.DeleteByID(Convert.ToInt32(PayEntryDS.Tables["CheckHed"].Rows[0]["HeadNum"]));
                                                                            //Delete newly created Group data from ApChkGrp table due to exception
                                                                            if (GroupIdExist == false)
                                                                            {
                                                                                //commented because DeleteByID is not working. Its native issue, already checked with BL-Tester.
                                                                                ////APChkGrpImp.DeleteByID(GroupId);
                                                                                //DeleteApChkGrp(GroupId);
                                                                                APChkGrpDS.APChkGrp[0].RowMod = "D";
                                                                                APChkGrpDS.APChkGrp[0].Delete();
                                                                                APChkGrpImp.Update(APChkGrpDS);
                                                                            }
                                                                            //======================================================================================================
                                                                            if (ObjBO.AllowLogAppending == true)
                                                                            {
                                                                                LogMsg.AppendLine("Exception Message: " + exPayUpdate.Message);
                                                                                LogMsg.AppendLine("Exception on PaymentEntry.Update BO Method call." + " Recorded Time = " + Convert.ToString(DateTime.Now));                                                                                
                                                                                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentEntry", LogMsg.ToString(), exPayUpdate);
                                                                            }
                                                                            //Stop executing further code as error message recorded in database and move to the next iteration.
                                                                            continue;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            ObjBO.HeadNumExists = true;
                                                            PayEntryDS = PayEntryImp.GetByID(HeadNum);

                                                            if (ObjBO.AllowLogAppending == true)
                                                            {
                                                                LogMsg.AppendLine("Called PaymentEntry.GetByID BO Method successfully." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                            }
                                                        }
                                                        catch (Exception exAPTran)
                                                        {
                                                            Dr = Dt.NewRow();
                                                            EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
                                                            Dr[1] = Convert.ToString(CPOBankAcctID);
                                                            Dr[2] = Convert.ToString(SrcCompany);
                                                            Dr[3] = Convert.ToString(InvoiceNum);
                                                            Dr[4] = Convert.ToString(exAPTran.Message).Trim();
                                                            Dr[5] = EndTime;
                                                            Dt.Rows.Add(Dr);

                                                            if (ObjBO.AllowLogAppending == true)
                                                            {
                                                                LogMsg.AppendLine("Exception Message: " + exAPTran.Message);
                                                                LogMsg.AppendLine("Exception on PaymentEntry.GetByID BO Method call." + " Recorded Time = " + Convert.ToString(DateTime.Now));                                                                
                                                                ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentEntry", LogMsg.ToString(), exAPTran);
                                                            }
                                                            //Stop executing further code as error message recorded in database and move to the next iteration.
                                                            continue;
                                                        }
                                                    }

                                                    try
                                                    {
                                                        DsVendor = VendImpls.GetByID(VendorNum);

                                                        DsVendor.Tables["Vendor"].Rows[0]["PrimaryBankID"] = OrigPrimaryBankId;
                                                        VendImpls.Update(DsVendor);
                                                        PayEntryImp.GetNewAPTran(PayEntryDS, HeadNum, 0, InvoiceNum);
                                                    }
                                                    catch (Exception exAPTran)
                                                    {
                                                        Dr = Dt.NewRow();
                                                        EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
                                                        Dr[1] = Convert.ToString(CPOBankAcctID);
                                                        Dr[2] = Convert.ToString(SrcCompany);
                                                        Dr[3] = Convert.ToString(InvoiceNum);
                                                        Dr[4] = Convert.ToString(exAPTran.Message).Trim();
                                                        Dr[5] = EndTime;
                                                        Dt.Rows.Add(Dr);

                                                        //======================================================================================================                                      
                                                        //Delete newly created Group data from ApChkGrp table due to exception
                                                        if (GroupIdExist == false)
                                                        {
                                                            //APChkGrpImp.DeleteByID(ObjBO.GroupId);
                                                            APChkGrpDS.APChkGrp[0].RowMod = "D";
                                                            APChkGrpDS.APChkGrp[0].Delete();
                                                            APChkGrpImp.Update(APChkGrpDS);
                                                        }
                                                        //======================================================================================================
                                                        if (ObjBO.AllowLogAppending == true)
                                                        {
                                                            LogMsg.AppendLine("Exception Message: " + exAPTran.Message);
                                                            LogMsg.AppendLine("Exception on PaymentEntry.GetNewAPTran BO Method call." + " Recorded Time = " + Convert.ToString(DateTime.Now));                                                            
                                                            ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentEntry", LogMsg.ToString(), exAPTran);
                                                        }
                                                        //Stop executing further code as error message recorded in database and move to the next iteration.
                                                        continue;
                                                    }

                                                    RowNum = 0;
                                                    RowNum = PayEntryDS.Tables["APTran"].Rows.Count;

                                                    for (int i = RowNum - 1; i >= 0; i--)  //Loop to get Active row mode record
                                                    {
                                                        if ((Convert.ToString(PayEntryDS.APTran[i].RowMod) == "A") || (Convert.ToString(PayEntryDS.APTran[i].RowMod) == ""))
                                                        {
                                                            try
                                                            {
                                                                PayEntryImp.OnChangeInvoiceNum(InvoiceNum, "Yes", PayEntryDS, out OcExchResp);
                                                                if (ObjBO.AllowLogAppending == true)
                                                                {
                                                                    LogMsg.AppendLine("Called PaymentEntry.OnChangeInvoiceNum BO Method successfully." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                }
                                                            }
                                                            catch (Exception exApTran)
                                                            {
                                                                Dr = Dt.NewRow();
                                                                EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
                                                                Dr[1] = Convert.ToString(CPOBankAcctID);
                                                                Dr[2] = Convert.ToString(SrcCompany);
                                                                Dr[3] = Convert.ToString(InvoiceNum);
                                                                Dr[4] = Convert.ToString(exApTran.Message).Trim();
                                                                Dr[5] = EndTime;
                                                                Dt.Rows.Add(Dr);

                                                                //======================================================================================================                                                                                        
                                                                //Delete newly created Check head information from CheckHed table due to exception                                                                
                                                                PayEntryImp.DeleteByID(Convert.ToInt32(PayEntryDS.Tables["CheckHed"].Rows[0]["HeadNum"]));
                                                                //Delete newly created Group data from ApChkGrp table due to exception
                                                                if (GroupIdExist == false)
                                                                {
                                                                    //commented because DeleteByID is not working. Its native issue, already checked with BL-Tester.
                                                                    ////APChkGrpImp.DeleteByID(GroupId);
                                                                    //DeleteApChkGrp(GroupId);
                                                                    APChkGrpDS.APChkGrp[0].RowMod = "D";
                                                                    APChkGrpDS.APChkGrp[0].Delete();
                                                                    APChkGrpImp.Update(APChkGrpDS);
                                                                }
                                                                //======================================================================================================
                                                                if (ObjBO.AllowLogAppending == true)
                                                                {
                                                                    LogMsg.AppendLine("Exception on PaymentEntry.OnChangeInvoiceNum or PaymentEntry.Update  BO Method call." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                    ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentEntry", LogMsg.ToString(), exApTran);
                                                                }
                                                                continue;
                                                            }

                                                            try
                                                            {
                                                                if (ObjBO.AllowLogAppending == true)
                                                                {
                                                                    LogMsg.AppendLine("PaymentEntry.Update BO Method Starting." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                }

                                                                //below code is added by MDD to resolve object reference error while calling PayEntryImp.Update method
                                                                if (PayEntryDS.Tables.Count > 0)
                                                                {
                                                                    PayEntryDS.APTran[i].LegalNumber = Convert.ToString(DsRecordsForGeneratePay.Tables[0].Rows[Row]["LegalNumber"]);
                                                                    if (string.IsNullOrEmpty(PayEntryDS.CheckHed[0][15].ToString()))
                                                                    {
                                                                        PayEntryDS.CheckHed[0][15] = "";
                                                                    }
                                                                }
                                                                //below code is added by MDD to resolve object reference error while calling PayEntryImp.Update method

                                                                PayEntryImp.Update(PayEntryDS);

                                                                if (ObjBO.AllowLogAppending == true)
                                                                {
                                                                    LogMsg.AppendLine("Called PaymentEntry.Update BO Method successfully." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                }

                                                                if (Convert.ToString(Session["IsAutoSubmitForApproval"]) == "TRUE")
                                                                {
                                                                    //below code is added by MDD to update Submitted4Appr_c = true
                                                                    if (PayEntryDS.Tables.Count > 0)
                                                                    {
                                                                        PayEntryDS.CheckHed[0]["Submitted4Approval_c"] = true;
                                                                    }
                                                                }

                                                                PayEntryImp.Update(PayEntryDS);

                                                                if (ObjBO.AllowLogAppending == true)
                                                                {
                                                                    LogMsg.AppendLine("Called PaymentEntry.Update BO Method successfully." + " Recorded Time = " + Convert.ToString(DateTime.Now));
                                                                }
                                                            }
                                                            catch (Exception exApTran)
                                                            {
                                                                Dr = Dt.NewRow();
                                                                EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
                                                                Dr[1] = Convert.ToString(CPOBankAcctID);
                                                                Dr[2] = Convert.ToString(SrcCompany);
                                                                Dr[3] = Convert.ToString(InvoiceNum);
                                                                Dr[4] = Convert.ToString(exApTran.Message).Trim();
                                                                Dr[5] = EndTime;
                                                                Dt.Rows.Add(Dr);

                                                                //======================================================================================================                                                                                        
                                                                //Delete newly created Check head information from CheckHed table due to exception                                                                
                                                                PayEntryImp.DeleteByID(Convert.ToInt32(PayEntryDS.Tables["CheckHed"].Rows[0]["HeadNum"]));
                                                                //Delete newly created Group data from ApChkGrp table due to exception
                                                                if (GroupIdExist == false)
                                                                {
                                                                    //commented because DeleteByID is not working. Its native issue, already checked with BL-Tester.
                                                                    ////APChkGrpImp.DeleteByID(GroupId);
                                                                    //DeleteApChkGrp(GroupId);
                                                                    APChkGrpDS.APChkGrp[0].RowMod = "D";
                                                                    APChkGrpDS.APChkGrp[0].Delete();
                                                                    APChkGrpImp.Update(APChkGrpDS);
                                                                }
                                                                //======================================================================================================
                                                                if (ObjBO.AllowLogAppending == true)
                                                                {
                                                                    LogMsg.AppendLine("Exception on PaymentEntry.OnChangeInvoiceNum or PaymentEntry.Update  BO Method call." + " Recorded Time = " + Convert.ToString(DateTime.Now));                                                                    
                                                                    ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentEntry", LogMsg.ToString(), exApTran);
                                                                }
                                                                continue;
                                                            }
                                                            Dr = Dt.NewRow();
                                                            EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));

                                                            Dr[1] = Convert.ToString(CPOBankAcctID);
                                                            Dr[2] = Convert.ToString(SrcCompany);
                                                            Dr[3] = Convert.ToString(InvoiceNum);
                                                            Dr[4] = Convert.ToString("Payment Generated Successfully.");
                                                            Dr[5] = EndTime;
                                                            Dt.Rows.Add(Dr);
                                                            break;
                                                        }
                                                    }
                                                    PayEntryImp = null;
                                                    PayEntryDS = null;
                                                    #endregion PaymentEntryAdapter
                                                }
                                                #endregion APChkGrpAdapter
                                            }
                                            catch (Exception exmsg)
                                            {
                                                if (GroupIdExist == false)
                                                {
                                                    //commented because DeleteByID is not working. Its native issue, already checked with BL-Tester.
                                                    ////APChkGrpImp.DeleteByID(GroupId);
                                                    DeleteApChkGrp(GroupId);                                                   
                                                }

                                                Dr = Dt.NewRow();
                                                EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));

                                                Dr[1] = Convert.ToString(CPOBankAcctID);
                                                Dr[2] = Convert.ToString(SrcCompany);
                                                Dr[3] = Convert.ToString(InvoiceNum);
                                                if (exmsg.Message.ToString().ToUpper() == "The Group already exists.".ToUpper())
                                                {
                                                    Dr[4] = Convert.ToString(exmsg.Message.ToString() + " Delete Group ID : " + GroupId + " from Payment Entry.");
                                                }
                                                else
                                                {
                                                    Dr[4] = Convert.ToString(exmsg.Message);
                                                }
                                                Dr[5] = EndTime;
                                                Dt.Rows.Add(Dr);

                                                if (ObjBO.AllowLogAppending == true)
                                                {                                                    
                                                    ObjEvent.ExceptionEvent("GeneratePayment", "GeneratePaymentWebPortal => GeneratePaymentEntry", LogMsg.ToString(), exmsg);
                                                }
                                            }
                                        }
                                        else
                                        {
                                        }
                                        #endregion IfCondition
                                    }
                                    #endregion For Loop End

                                    if (Dt.Rows.Count > 0)
                                    {
                                        string StatusInfo = string.Empty;
                                        StatusInfo = GetStatusInformation(Dt);
                                        //Session["StatusInfo"] = StatusInfo;
                                        tdStatusInfo.InnerHtml = StatusInfo;
                                        modalPopup.Show();
                                        //below code line is commented, If found any issue while testing with more records then will may use below logic
                                        ////ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.showModalDialog('Stcl_StatusPopUp.aspx', '', 'dialogHeight:450px;dialogWidth:900px;status:no');", true);
                                    }
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('Treasury Company Id is missing. Please Contact Administrator.');", true);
                            }
                        }
                        catch (Exception ErrorMsg)
                        {
                            ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('" + Convert.ToString(ErrorMsg.Message) + "');", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('" + Convert.ToString(ex.Message) + "');", true);
                }
                finally
                {
                    SessionModImpl.Logout();
                    SessionModImpl.Dispose();
                }

                #endregion Inside Connection

                if (ObjBO.AllowLogAppending == true)
                {                    
                    ObjEvent.InformationEvent("GeneratePayment", "GeneratePaymentWebPortal", LogMsg.ToString(), "GeneratePayment", WriteEventLog, 1);
                    ObjEvent = null;
                    LogMsg = null;
                }

                ObjDO = null;
                ObjBO = null;

                #endregion Connection Pool
            }
            else
            {
                Session["TransMessage"] = "Invalid session data.";
                Response.Redirect("Stcl_ErrorMessage.aspx");
            }
            #endregion Save Process End
        }
        

        private DataSet GetGeneratePayRecordForProceed()
        {
            DataSet DsGetRecords = new DataSet();

            DataTable Dt = new DataTable();
            DataRow Dr;
            Dt.Columns.Add(new System.Data.DataColumn("ChkAction", typeof(bool)));
            Dt.Columns.Add(new System.Data.DataColumn("LegalNumber", typeof(String)));
            Dt.Columns.Add(new System.Data.DataColumn("VendorName", typeof(String)));
            Dt.Columns.Add(new System.Data.DataColumn("InvoiceNum", typeof(String)));
            Dt.Columns.Add(new System.Data.DataColumn("SrcGroupID", typeof(String)));
            Dt.Columns.Add(new System.Data.DataColumn("GroupID", typeof(String)));
            Dt.Columns.Add(new System.Data.DataColumn("InvoiceAmt", typeof(decimal)));
            Dt.Columns.Add(new System.Data.DataColumn("CurrencyCode", typeof(String)));
            Dt.Columns.Add(new System.Data.DataColumn("ApplyDate", typeof(String)));
            Dt.Columns.Add(new System.Data.DataColumn("VendorNum", typeof(Int32)));
            Dt.Columns.Add(new System.Data.DataColumn("VendorID", typeof(String)));
            Dt.Columns.Add(new System.Data.DataColumn("SubBudgetCls", typeof(String)));
            Dt.Columns.Add(new System.Data.DataColumn("PMUID", typeof(String)));
            Dt.Columns.Add(new System.Data.DataColumn("PayMethodName", typeof(String)));
            Dt.Columns.Add(new System.Data.DataColumn("SrcCompany", typeof(String)));
            Dt.Columns.Add(new System.Data.DataColumn("PayRefNo", typeof(String)));
            //Dt.Columns.Add(new System.Data.DataColumn("Comment", typeof(String))); 
            // Comment not required for now If required that logic we can use it after confirmation

            foreach (GridViewRow row in grvGeneratePayment.Rows)
            {
                CheckBox ChkAction = (CheckBox)row.FindControl("ChkAction");
                //TextBox Comment = (TextBox)row.FindControl("txtComment"); 
                // Comment not required for now If required that logic we can use it after confirmation
                Label InvoiceAmount = (Label)row.FindControl("txtInvoiceAmount");
                Label InvoiceNummber = (Label)row.FindControl("lblInvoiceNum");

                string LegalNumber = (string)grvGeneratePayment.Rows[row.RowIndex].Cells[2].Text;
                string VendorName = (string)grvGeneratePayment.Rows[row.RowIndex].Cells[3].Text;
                string InvoiceNum = Convert.ToString(InvoiceNummber.Text);
                string SrcGroupID = (string)grvGeneratePayment.Rows[row.RowIndex].Cells[5].Text;
                string GroupID = (string)grvGeneratePayment.Rows[row.RowIndex].Cells[6].Text;
                decimal InvoiceAmt = Convert.ToDecimal(InvoiceAmount.Text);

                string CurrencyCode = (string)grvGeneratePayment.Rows[row.RowIndex].Cells[8].Text;
                string ApplyDate = (string)grvGeneratePayment.Rows[row.RowIndex].Cells[9].Text;
                Int32 VendorNum = Convert.ToInt32(grvGeneratePayment.Rows[row.RowIndex].Cells[10].Text);
                string VendorID = (string)grvGeneratePayment.Rows[row.RowIndex].Cells[11].Text;
                string SubBudgetCls = (string)grvGeneratePayment.Rows[row.RowIndex].Cells[12].Text;
                string PMUID = (string)grvGeneratePayment.Rows[row.RowIndex].Cells[13].Text;
                string PayMethodName = (string)grvGeneratePayment.Rows[row.RowIndex].Cells[14].Text;
                string SrcCompany = (string)grvGeneratePayment.Rows[row.RowIndex].Cells[15].Text;
                string PayRefNo = (string)grvGeneratePayment.Rows[row.RowIndex].Cells[16].Text;

                if (ChkAction.Checked == true)
                {
                    Dr = Dt.NewRow();
                    Dr[0] = true;
                    Dr[1] = LegalNumber.ToString();
                    Dr[2] = VendorName.ToString();
                    Dr[3] = InvoiceNum.ToString();
                    Dr[4] = SrcGroupID.ToString();
                    Dr[5] = GroupID.ToString();
                    Dr[6] = InvoiceAmt.ToString();
                    Dr[7] = CurrencyCode.ToString();
                    Dr[8] = ApplyDate.ToString();
                    Dr[9] = VendorNum.ToString();
                    Dr[10] = VendorID.ToString();
                    Dr[11] = SubBudgetCls.ToString();
                    Dr[12] = PMUID.ToString();
                    Dr[13] = PayMethodName.ToString();
                    Dr[14] = SrcCompany.ToString();
                    Dr[15] = PayRefNo.ToString();
                    //Dr[16] = Convert.ToString(Comment.Text);
                    // Comment not required for now If required that logic we can use it after confirmation
                    Dt.Rows.Add(Dr);
                }
            }
            DsGetRecords.Tables.Add(Dt);
            return DsGetRecords;
        }

        protected void btnGetData_Click(object sender, EventArgs e)
        {
            GetData();
        }

        private void GetData()
        {
            try
            {
                CompanyId = Convert.ToString(Session["CompanyId"]);
                PaymentGeneratorUserId = Convert.ToString(Session["UserId"]);
                SessionId = Convert.ToString(Session["SessionId"]);
                if (!String.IsNullOrEmpty(CompanyId) && !String.IsNullOrEmpty(PaymentGeneratorUserId) && !String.IsNullOrEmpty(SessionId))
                {
                    if (!string.IsNullOrEmpty(lblTxtBankAccountId.Text))
                    {
                        DataSet DsGeneratePaymentData = new DataSet();
                        DsGeneratePaymentData = ObjDO.GetGeneratePaymentData(Convert.ToString(lblTxtSourceCompany.Text).Trim(), Convert.ToString(lblTxtGenerator.Text).Trim(), Convert.ToString(lblTxtBankAccountId.Text).Trim(), Convert.ToString(lblTxtSourceGroupId.Text).Trim(), Convert.ToString(txtVendorId.Text).Trim(), Convert.ToString(txtLegalNumber.Text).Trim(), Convert.ToBoolean(Session["IsSalaryPay"]), Convert.ToString(lblTxtSourceGroupId.Text).Trim());

                        if (DsGeneratePaymentData.Tables.Count > 0 && DsGeneratePaymentData.Tables[0].Rows.Count > 0)
                        {
                            grvGeneratePayment.DataSource = DsGeneratePaymentData.Tables[0];
                            grvGeneratePayment.DataBind();
                            for (var i = 0; i < grvGeneratePayment.Rows.Count; i++)
                            {
                                CheckBox chk = grvGeneratePayment.Rows[i].FindControl("chkAction") as CheckBox;
                                chk.Checked = false;
                            }
                            DivGeneratePaymentData.Visible = true;
                            DivAction.Visible = true;
                        }
                        else
                        {
                            grvGeneratePayment.DataSource = null;
                            grvGeneratePayment.DataBind();
                            DivGeneratePaymentData.Visible = false;
                            DivAction.Visible = false;
                            ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('No data for generate payment.');", true);
                        }
                        DsGeneratePaymentData.Dispose();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('Bank Account Id is must to proceed further.');", true);
                    }
                    hfGPValue.Value = "0";
                }
                else
                {
                    Session["TransMessage"] = "Invalid session data.";
                    Response.Redirect("Stcl_ErrorMessage.aspx");
                }

            }
            catch (Exception ex)
            {
                Session["TransMessage"] = "Stcl_GeneratePayment==>GetData=>" + Convert.ToString(ex.Message);
                Response.Redirect("Stcl_ErrorMessage.aspx");
                throw;
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            CompanyId = Convert.ToString(Session["CompanyId"]);
            PaymentGeneratorUserId = Convert.ToString(Session["UserId"]);
            SessionId = Convert.ToString(Session["SessionId"]);

            DataTable Dt = new DataTable();
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

            Response.Redirect("Stcl_ConsolidateGeneratePaymentInfo.aspx", false);
        }             

        private string GetStatusInformation(DataTable dt)
        {
            string StatusInformation = string.Empty;

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        StatusInformation = "<table class='Popup gridtable' style='Left:0px;''><tr><th colspan='6'>Generate Payment Status Report</th></tr><tr><th>SrNo</th><th>Bank Account Id </th><th>Source Company</th><th>Invoice Num </th><th>Status</th><th>Time</th></tr>" +
                                            "<tr><td>" + Convert.ToString(i + 1) + "</td><td>" + Convert.ToString(dt.Rows[i][1]) + "</td><td>" + Convert.ToString(dt.Rows[i][2]) + "</td><td>" + Convert.ToString(dt.Rows[i][3]) + "</td><td>" + Convert.ToString(dt.Rows[i][4]) + "</td><td>" + Convert.ToString(dt.Rows[i][5]) + "</td></tr>";
                    }
                    else if (i > 0)
                    {
                        StatusInformation = StatusInformation + "<tr><td>" + Convert.ToString(i + 1) + "</td><td>" + Convert.ToString(dt.Rows[i][1]) + "</td><td>" + Convert.ToString(dt.Rows[i][2]) + "</td><td>" + Convert.ToString(dt.Rows[i][3]) + "</td><td>" + Convert.ToString(dt.Rows[i][4]) + "</td><td>" + Convert.ToString(dt.Rows[i][5]) + "</td></tr>";
                    }
                }
                StatusInformation = StatusInformation + "</table>";
            }

            return StatusInformation;
        }

        protected void grvGeneratePayment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label LblInvoiceAmount = (Label)e.Row.FindControl("txtInvoiceAmount");
                decimal InvoiceAmount = Convert.ToDecimal(LblInvoiceAmount.Text);
                SumOfInvoiceAmount = SumOfInvoiceAmount + InvoiceAmount;

                LblInvoiceAmount.Text = String.Format("{0:#,0.00}", InvoiceAmount);

                string SetupMissing = Convert.ToString(((Label)e.Row.FindControl("lblSetupMissing")).Text);
                if (!string.IsNullOrEmpty(SetupMissing))
                {
                    //TextBox TxtComment = (TextBox)e.Row.FindControl("txtComment");
                    // Comment not required for now If required that logic we can use it after confirmation
                    CheckBox ChkGeneratePayment = (CheckBox)e.Row.FindControl("chkAction");
                    //TxtComment.Visible = false;
                    // Comment not required for now If required that logic we can use it after confirmation
                    ChkGeneratePayment.Visible = false;
                    ChkGeneratePayment.Checked = false;
                }
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                lblTotalAmountValue.Text = Convert.ToString(String.Format("{0:#,0.00}", SumOfInvoiceAmount));
            }
        }

        protected void btnHidden_Click(object sender, EventArgs e)
        {
            GetData();
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
               
        private static void DeleteApChkGrp(string GrpId)
        {
            var ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["cons"].ConnectionString;
            SqlConnection con = new SqlConnection(ConnectionString);
            try
            {
                con.Open();
                string sql = @"DELETE FROM Erp.APChkGrp WHERE GroupID = '" + GrpId + "'";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                //throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        #endregion
    }
}