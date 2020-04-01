using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Configuration;
using System.Net;

//using Epicor.Mfg.Core;
//using Epicor.Mfg.Lib;
//using Epicor.Mfg.IF;
//using Epicor.Mfg.BO;

//using Epicor.Mfg.UI;
//using Epicor.Mfg.UI.Adapters;
//using Epicor.Mfg.UI.FrameWork;
//using Epicor.Mfg.UI.App.PaymentEntryEntry;

//using Epicor.Mfg.UI.Searches;

//using Epicor.Mfg.UI.Customization;
//using Epicor.Mfg.UI.ExtendedProps;
//using Epicor.Mfg.UI.FormFunctions;
//using Epicor.Mfg.Shared;
//using System.Xml;
using Erp;
using Ice;
using Ice.Tables;
using Ice.Tableset;
using Ice.Tablesets;
using System.Threading;
using System.IO;
using Epicor.Hosting;
using Ice.Proxy.Lib;
using Epicor.ServiceModel.Channels;
using System.Transactions;
using Erp.Proxy.BO;
using Erp.Tablesets;
using Erp.BO;
using Ice.Lib.Framework;
using Erp.Adapters;
using Ice.Lib;
using Stcl.Epicor905.GeneratePayment.WebPages;


namespace Stcl.EpicorErp10.GeneratePaymentWebPortal.WebPages
{
    public partial class Stcl_GeneratePaymentUsingSSO : System.Web.UI.Page
    {
        private Erp.UI.App.PaymentEntryEntry.Transaction oTransPay;
        private ILaunch iLaunch;
        static Session MySession;

        string PaymentGeneratorUserId = string.Empty;
        string CompanyId = string.Empty;
        string SessionId = string.Empty;
        string BankAccctId = string.Empty;
        string SourceCompany = string.Empty;
        string Password = string.Empty;
        Decimal SumOfInvoiceAmount = 0;
        string VoucherListNum = string.Empty;

        #region Public variable declaration Added by MDD
        static string EpicorUserID = Convert.ToString(GetSysParam("epicorUserID"));
        static string EpiorUserPassword = Convert.ToString(GetSysParam("epicorUserPassword"));
        static string AppSrvUrl = "net.tcp://" + Convert.ToString(GetSysParam("ServerUrl")); // This should be the url to your appserver
        static string EndpointBinding = "UsernameWindowsChannel"; // This is case sensitive. Valid values are "UsernameWindowsChannel", "Windows" and "UsernameSslChannel"
        static string TreasuryCompany = Convert.ToString(GetSysParam("TreasuryCompany"));
        static string EpicCNString = System.Configuration.ConfigurationManager.ConnectionStrings["cons"].ConnectionString.ToString();
        ////static string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["IsidoreCn"].ConnectionString.ToString();
        ////static SqlConnection Con = new SqlConnection(Convert.ToString(ConnectionString));
        static SqlConnection EpicCon = new SqlConnection(Convert.ToString(EpicCNString));
        string CompanyID = string.Empty;
        static SqlDataAdapter DACOASeg = new SqlDataAdapter();
        static SqlCommand sqlCmd = new SqlCommand();
        static DataSet DsCOASegTMP = new DataSet();
        static string SqlStr = string.Empty;
        SessionModImpl SessionModImpl = CreateBusObj<SessionModImpl>(Guid.Empty, SessionModImpl.UriPath);
        #endregion

        BusinessAccessLayer.GeneratePaymentBusinessObjects objBo = new BusinessAccessLayer.GeneratePaymentBusinessObjects();
        DataAccessLayer.GeneratePaymentDataObjects objDo = new DataAccessLayer.GeneratePaymentDataObjects();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DivGeneratePaymentData.Visible = false;
                DivAction.Visible = false;
                //DivProcess.Visible = false;        

                CompanyId = Convert.ToString(Session["CompanyId"]);
                PaymentGeneratorUserId = Convert.ToString(Session["UserId"]);
                SessionId = Convert.ToString(Session["SessionId"]);
                BankAccctId = Convert.ToString(Session["BankAcctId"]);
                SourceCompany = Convert.ToString(Session["SrcCompany"]);
                Password = Convert.ToString(Session["Password"]);
                VoucherListNum = Convert.ToString(Session["VoucherListNum"]);

                if (!String.IsNullOrEmpty(CompanyId) && !String.IsNullOrEmpty(PaymentGeneratorUserId) && !String.IsNullOrEmpty(SessionId))
                {
                    lblTxtSourceCompany.Text = SourceCompany;
                    lblTxtBankAccountId.Text = BankAccctId;
                    lblTxtGenerator.Text = PaymentGeneratorUserId;
                    lblTxtSourceGroupId.Text = VoucherListNum;
                    #region commented code
                    /*
                    if (!String.IsNullOrEmpty(Convert.ToString(Session["CompanyId"])) && !String.IsNullOrEmpty(Convert.ToString(Session["UserId"])) && !String.IsNullOrEmpty(Convert.ToString(Session["Password"])))
                    {
                        objBo.ServerUserId = Convert.ToString(Session["UserId"]).Trim();
                        objBo.ServerPassword = Convert.ToString(Session["Password"]).Trim();
                        objBo.ServerUrl = "AppServerDC://e9ap905702:9401";
                        using (var connPool = new BLConnectionPool(objBo.ServerUserId, objBo.ServerPassword, objBo.ServerUrl))
                        {
                            try
                            {
                                using (MySession = new Session(objBo.ServerUserId, objBo.ServerPassword, objBo.ServerUrl,
                                                    Epicor.Mfg.Core.Session.LicenseType.Default))
                                {
                                    string CPOBankAcctID = string.Empty;
                                    string CurrentSelectedCompany = string.Empty;
                                    string Generator = string.Empty;

                                    CurrentSelectedCompany = Convert.ToString(lblTxtSourceCompany.Text);
                                    CPOBankAcctID = Convert.ToString(lblTxtBankAccountId.Text);
                                    Generator = Convert.ToString(lblTxtGenerator.Text);

                                    string connetionstring = Convert.ToString(ConfigurationManager.ConnectionStrings["cons"]).Trim();
                                    SqlConnection conn = new SqlConnection(connetionstring);
                                    SqlCommand cmddata = new SqlCommand("Select CurEmpID from LicUser where  CurEmpID = '" + CurrentSelectedCompany + "-" + CPOBankAcctID + "' ", conn);
                                    conn.Open();
                                    SqlDataReader DataRow = cmddata.ExecuteReader();
                                    if (!DataRow.HasRows)
                                    {
                                        MySession.EmployeeID = CurrentSelectedCompany + "-" + CPOBankAcctID;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('" + Convert.ToString(ex.Message) + "');", true);
                            }
                        }
                    }
                    */
                    #endregion commented code
                }
                else
                {
                    Session["TransMessage"] = "Session Expired!";
                    Response.Redirect("Stcl_ErrorMessage.aspx");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string StartTime = string.Empty;
            string EndTime = string.Empty;

            StartTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));

            #region Save Process Start
            //MySession = (Session)Session["EpicorSession"];            

            //if (MySession != null)
            //{
            try
            {
                string CPOBankAcctID = string.Empty;
                string CurrentSelectedCompany = string.Empty;
                string Generator = string.Empty;

                CurrentSelectedCompany = Convert.ToString(lblTxtSourceCompany.Text);
                CPOBankAcctID = Convert.ToString(lblTxtBankAccountId.Text);
                Generator = Convert.ToString(lblTxtGenerator.Text);

                //string connetionstring = Convert.ToString(ConfigurationManager.ConnectionStrings["cons"]).Trim();
                //SqlConnection conn = new SqlConnection(connetionstring);
                //SqlCommand cmddata = new SqlCommand("Select CurEmpID from LicUser where  CurEmpID = '" + CurrentSelectedCompany + "-" + CPOBankAcctID + "' ", conn);
                //conn.Open();
                //SqlDataReader DataRow = cmddata.ExecuteReader();
                //if (!DataRow.HasRows)
                //{
                try
                {
                    objBo.ServerUrl = System.Configuration.ConfigurationManager.AppSettings["ServerUrl"];
                    //MySession = LogOn.RunDialog(Generator, objBo.ServerUrl, Epicor.Mfg.Core.Session.LicenseType.Default);

                    ////MySession = LogOn.RunDialog();
                    ////MySession.CompanyID = "000";
                    string CompanyName, PlantID, PlantName, WorkstationID, WorkstationDesc, EmployeeID, CountryGroupCode, CountryCode, TenantID;
                    SessionModImpl.SetCompany(Convert.ToString(lblTxtSourceCompany.Text), out CompanyName, out PlantID, out PlantName, out WorkstationID, out WorkstationDesc, out EmployeeID, out CountryGroupCode, out CountryCode, out TenantID);

                    iLaunch = new ILauncher(MySession);
                    oTransPay = new Erp.UI.App.PaymentEntryEntry.Transaction(iLaunch);

                    DataSet DsRecordsForGeneratePay = new DataSet();
                    string Company = string.Empty;
                    string InvoiceNum = string.Empty;
                    Int32 VendorNum;
                    decimal InvoiceAmt = 0;
                    string Error = string.Empty;
                    string SrcCompany = string.Empty;
                    string SrcGroupID = string.Empty; //Voucher List Number
                    string GroupId = string.Empty;
                    Int32 PMUID;
                    Int16 HeadNum;
                    string VendorID = string.Empty;
                    string CurrencyCode = string.Empty;
                    bool RequiresUserInput;
                    string OcExchResp = string.Empty;

                    string Code = "TreasuryCompany";
                    Company = GetTreasuryCompany(Code);
                    if (!string.IsNullOrEmpty(Convert.ToString(Company).Trim()))
                    {
                        //Get selected records to generate payment
                        DsRecordsForGeneratePay = GetGeneratePayRecordForProceed();

                        if (DsRecordsForGeneratePay.Tables.Count > 0 && DsRecordsForGeneratePay.Tables[0].Rows.Count > 0)
                        {
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
                                #region IfCondition
                                if (Convert.ToBoolean(DsRecordsForGeneratePay.Tables[0].Rows[Row]["ChkAction"]) == true)
                                {

                                    try
                                    {
                                        #region TransScopePay
                                        using (var TransScopePay = TransactionUtils.CreateTransactionScope())
                                        {
                                            InvoiceNum = Convert.ToString(DsRecordsForGeneratePay.Tables[0].Rows[Row]["InvoiceNum"]);
                                            VendorNum = Convert.ToInt32(DsRecordsForGeneratePay.Tables[0].Rows[Row]["VendorNum"]);
                                            InvoiceAmt = Convert.ToDecimal(DsRecordsForGeneratePay.Tables[0].Rows[Row]["InvoiceAmt"]);
                                            SrcCompany = Convert.ToString(DsRecordsForGeneratePay.Tables[0].Rows[Row]["SrcCompany"]);
                                            SrcGroupID = Convert.ToString(DsRecordsForGeneratePay.Tables[0].Rows[Row]["SrcGroupID"]);
                                            PMUID = Convert.ToInt32(DsRecordsForGeneratePay.Tables[0].Rows[Row]["PMUID"]);
                                            VendorID = Convert.ToString(DsRecordsForGeneratePay.Tables[0].Rows[Row]["VendorID"]);
                                            CurrencyCode = Convert.ToString(DsRecordsForGeneratePay.Tables[0].Rows[Row]["CurrencyCode"]);

                                            DataSet DsValidateData = new DataSet();

                                            //Validate each record before generate payment
                                            DsValidateData = objDo.ValidateAndGetData(1, Company, VendorNum, CPOBankAcctID, SrcCompany, Generator, SrcGroupID, InvoiceNum, InvoiceAmt);

                                            if (DsValidateData.Tables.Count > 0 && DsValidateData.Tables[0].Rows.Count > 0)
                                            {
                                                GroupId = Convert.ToString(DsValidateData.Tables[0].Rows[0]["GroupID"]).Trim();

                                                //if (GroupId.Trim().Length > 8)
                                                if (!string.IsNullOrEmpty(Convert.ToString(DsValidateData.Tables[0].Rows[0]["ErrorMessage"]).Trim()))
                                                {
                                                    Dr = Dt.NewRow();
                                                    EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
                                                    if (!string.IsNullOrEmpty(Convert.ToString(Dr[0])))
                                                    {
                                                        Dr[0] = Convert.ToInt16(Dr[0]) + 1;
                                                    }
                                                    else
                                                    {
                                                        Dr[0] = 1;
                                                    }
                                                    Dr[1] = Convert.ToString(CPOBankAcctID);
                                                    Dr[2] = Convert.ToString(SrcCompany);
                                                    Dr[3] = Convert.ToString(InvoiceNum);
                                                    Dr[4] = Convert.ToString(DsValidateData.Tables[0].Rows[0]["ErrorMessage"]).Trim();
                                                    Dr[5] = EndTime;
                                                    Dt.Rows.Add(Dr);
                                                    TransScopePay.Dispose();
                                                    //Stop executing further code as error message recorded in database and move to the next iteration.
                                                    continue;
                                                }
                                                else if (string.IsNullOrEmpty(GroupId))
                                                {
                                                    DataSet DsGroupId = new DataSet();

                                                    //Get new group id
                                                    DsGroupId = objDo.GetPayGroupID(Company, SrcCompany);

                                                    if (DsGroupId.Tables.Count > 0 && DsGroupId.Tables[0].Rows.Count > 0)
                                                    {
                                                        GroupId = Convert.ToString(DsGroupId.Tables[0].Rows[0]["PayGroupID"]);

                                                        #region APChkGrp Code
                                                        //added using erp.adapters for below line error by mdd
                                                        APChkGrpAdapter AdApChkGrpAdapter = new APChkGrpAdapter(this.oTransPay);
                                                        AdApChkGrpAdapter.BOConnect();

                                                        AdApChkGrpAdapter.GetNewAPChkGrp();

                                                        var APChkGrpDataSet = AdApChkGrpAdapter.APChkGrpData;

                                                        if (Convert.ToString(APChkGrpDataSet.Tables["APChkGrp"].Rows[0]["RowMod"]) == "A")
                                                        {
                                                            APChkGrpDataSet.Tables["APChkGrp"].Rows[0]["GroupID"] = GroupId;
                                                            APChkGrpDataSet.Tables["APChkGrp"].Rows[0]["ShortChar01"] = SrcCompany;
                                                        }

                                                        try
                                                        {
                                                            AdApChkGrpAdapter.OnChangeBankAcctID(CPOBankAcctID);
                                                        }
                                                        catch (Exception exBankAcctId)
                                                        {
                                                            ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('" + Convert.ToString(exBankAcctId.Message) + "');", true);
                                                        }
                                                        try
                                                        {
                                                            AdApChkGrpAdapter.Update();
                                                        }
                                                        catch (Exception exUpdate)
                                                        {
                                                            ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('" + Convert.ToString(exUpdate.Message) + "');", true);
                                                        }

                                                        if (Convert.ToString(APChkGrpDataSet.Tables["APChkGrp"].Rows[0]["Company"]) == Company &&
                                                            Convert.ToString(APChkGrpDataSet.Tables["APChkGrp"].Rows[0]["GroupID"]) == GroupId)
                                                        {
                                                            APChkGrpDataSet.Tables["APChkGrp"].Rows[0]["PMUID"] = PMUID;
                                                            AdApChkGrpAdapter.Update();
                                                        }

                                                        AdApChkGrpAdapter = null;

                                                        #endregion APChkGrp Code
                                                    }
                                                }

                                                PaymentEntryAdapter AdPaymentEntryAdapter = new PaymentEntryAdapter(this.oTransPay);

                                                AdPaymentEntryAdapter.BOConnect();

                                                AdPaymentEntryAdapter.CreateNewCheckHed(GroupId);

                                                var PaymentEntryDataSet = AdPaymentEntryAdapter.PaymentEntryData;

                                                if (Convert.ToString(PaymentEntryDataSet.Tables["CheckHed"].Rows[0]["RowMod"]) == "A")
                                                {
                                                    HeadNum = Convert.ToInt16(PaymentEntryDataSet.Tables["CheckHed"].Rows[0]["HeadNum"]);

                                                    DsValidateData = new DataSet();
                                                    DsValidateData = objDo.ValidateAndGetData(2, Company, VendorNum, CPOBankAcctID, SrcCompany, Generator, SrcGroupID, InvoiceNum, InvoiceAmt);

                                                    if (DsValidateData.Tables.Count > 0 && DsValidateData.Tables[0].Rows.Count > 0)
                                                    {
                                                        if (!string.IsNullOrEmpty(Convert.ToString(DsValidateData.Tables[0].Rows[0]["ErrorMessage"])))
                                                        {
                                                            Dr = Dt.NewRow();
                                                            EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
                                                            if (!string.IsNullOrEmpty(Convert.ToString(Dr[0])))
                                                            {
                                                                Dr[0] = Convert.ToInt16(Dr[0]) + 1;
                                                            }
                                                            else
                                                            {
                                                                Dr[0] = 1;
                                                            }
                                                            Dr[1] = Convert.ToString(CPOBankAcctID);
                                                            Dr[2] = Convert.ToString(SrcCompany);
                                                            Dr[3] = Convert.ToString(InvoiceNum);
                                                            Dr[4] = Convert.ToString(DsValidateData.Tables[0].Rows[0]["ErrorMessage"]).Trim();
                                                            Dr[5] = EndTime;
                                                            Dt.Rows.Add(Dr);

                                                            TransScopePay.Dispose();
                                                            //Stop executing further code as error message recorded in database and move to the next iteration.
                                                            continue;
                                                        }
                                                        else
                                                        {
                                                            DsValidateData = new DataSet();
                                                            DsValidateData = objDo.GetCashInfo(Company, CPOBankAcctID, "", InvoiceAmt);
                                                            if (DsValidateData.Tables.Count > 0 && DsValidateData.Tables[0].Rows.Count > 0)
                                                            {
                                                                if (!string.IsNullOrEmpty(Convert.ToString(DsValidateData.Tables[0].Rows[0]["ErrorMessage"])))
                                                                {
                                                                    Dr = Dt.NewRow();
                                                                    EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
                                                                    if (!string.IsNullOrEmpty(Convert.ToString(Dr[0])))
                                                                    {
                                                                        Dr[0] = Convert.ToInt16(Dr[0]) + 1;
                                                                    }
                                                                    else
                                                                    {
                                                                        Dr[0] = 1;
                                                                    }
                                                                    Dr[1] = Convert.ToString(CPOBankAcctID);
                                                                    Dr[2] = Convert.ToString(SrcCompany);
                                                                    Dr[3] = Convert.ToString(InvoiceNum);
                                                                    Dr[4] = Convert.ToString(DsValidateData.Tables[0].Rows[0]["ErrorMessage"]).Trim();
                                                                    Dr[5] = EndTime;
                                                                    Dt.Rows.Add(Dr);

                                                                    TransScopePay.Dispose();
                                                                    //Stop executing further code as error message recorded in database and move to the next iteration.
                                                                    continue;
                                                                }
                                                                else
                                                                {

                                                                    /**  Seting VendorPrimary Bank As selected in ApInvoice entry **/
                                                                    //FIND FIRST Vendor EXCLUSIVE-LOCK
                                                                    //WHERE Vendor.Company = CUR-COMP
                                                                    //AND Vendor.VendorNum = piVendorNum NO-ERROR.

                                                                    //IF AVAILABLE (Vendor) THEN
                                                                    //DO:
                                                                    //    ASSIGN cPrimaryBankID = Vendor.PrimaryBankID
                                                                    //        Vendor.PrimaryBankID = (IF ApInvHed.{&AH-PymtBank} = "" 
                                                                    //                                    Then Vendor.PrimaryBankID
                                                                    //                                    Else ApInvHed.{&AH-PymtBank}).
                                                                    //END.
                                                                }

                                                                try
                                                                {
                                                                    AdPaymentEntryAdapter.OnChangeVendor(VendorID);
                                                                }
                                                                catch (Exception exChangeVendor)
                                                                {
                                                                    //ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('" + exChangeVendor.Message + "')", true);
                                                                }

                                                                try
                                                                {
                                                                    AdPaymentEntryAdapter.OnChangeCurrency(CurrencyCode);
                                                                }
                                                                catch (Exception exChangeCurrency)
                                                                {
                                                                    //ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('" + exChangeCurrency.Message + "')", true);
                                                                }

                                                                try
                                                                {
                                                                    AdPaymentEntryAdapter.PreUpdate(out RequiresUserInput);
                                                                }
                                                                catch (Exception exPreUpdate)
                                                                {
                                                                    //ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('" + exPreUpdate.Message + "')", true);
                                                                }

                                                                try
                                                                {
                                                                    AdPaymentEntryAdapter.Update();
                                                                }
                                                                catch (Exception exPayUpdate)
                                                                {
                                                                    //ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('" + exPayUpdate.Message + "')", true);
                                                                }

                                                                /* ** Reset the original Primary Bank for a Particular Vendor **/
                                                                //FIND FIRST Vendor
                                                                //      WHERE Vendor.Company = CUR-COMP
                                                                //      AND Vendor.VendorNum = piVendorNum EXCLUSIVE-LOCK NO-ERROR.

                                                                //  IF AVAILABLE (Vendor) THEN
                                                                //  DO:
                                                                //      ASSIGN Vendor.PrimaryBankID = cPrimaryBankID.
                                                                //  END.
                                                            }
                                                        }
                                                    }

                                                    AdPaymentEntryAdapter.GetNewAPTran(HeadNum, 0, InvoiceNum);

                                                    var ApTranDataSet = AdPaymentEntryAdapter.PaymentEntryData;

                                                    if (Convert.ToString(ApTranDataSet.Tables["APTran"].Rows[0]["RowMod"]) == "A")
                                                    {
                                                        AdPaymentEntryAdapter.OnChangeInvoiceNum(InvoiceNum, "Yes", out OcExchResp);

                                                        AdPaymentEntryAdapter.Update();

                                                        Dr = Dt.NewRow();

                                                        EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));

                                                        if (!string.IsNullOrEmpty(Convert.ToString(Dr[0])))
                                                        {
                                                            Dr[0] = Convert.ToInt16(Dr[0]) + 1;
                                                        }
                                                        else
                                                        {
                                                            Dr[0] = 1;
                                                        }
                                                        Dr[1] = Convert.ToString(CPOBankAcctID);
                                                        Dr[2] = Convert.ToString(SrcCompany);
                                                        Dr[3] = Convert.ToString(InvoiceNum);
                                                        Dr[4] = Convert.ToString("Payment Generated Successfully.");
                                                        Dr[5] = EndTime;
                                                        Dt.Rows.Add(Dr);
                                                    }
                                                    AdPaymentEntryAdapter = null;
                                                    ApTranDataSet = null;
                                                }
                                                else
                                                {
                                                    //Group Id not get generated
                                                }
                                            }
                                            TransScopePay.Complete();

                                            //ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('Process Completed.');", true);
                                        }
                                        #endregion  TransScopePay
                                    }
                                    catch (Exception exmsg)
                                    {
                                        //ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('" + exmsg.Message + "');", true);
                                        Dr = Dt.NewRow();
                                        EndTime = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff"));
                                        if (!string.IsNullOrEmpty(Convert.ToString(Dr[0])))
                                        {
                                            Dr[0] = Convert.ToInt16(Dr[0]) + 1;
                                        }
                                        else
                                        {
                                            Dr[0] = 1;
                                        }
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
                                Session["StatusInfo"] = StatusInfo;

                                ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.showModalDialog('Stcl_StatusPopUp.aspx', '', 'dialogHeight:450px;dialogWidth:900px;status:no');", true);
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
                finally
                {
                    MySession.Dispose();
                }
                //}
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('" + Convert.ToString(ex.Message) + "');", true);
            }
            //}
            //else
            //{
            //    Session["TransMessage"] = "Invalid session data.";
            //    Response.Redirect("Stcl_ErrorMessage.aspx");
            //}
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
            //Dt.Columns.Add(new System.Data.DataColumn("Comment", typeof(String))); // Comment not required for MOFTZ envoinment

            foreach (GridViewRow row in grvGeneratePayment.Rows)
            {
                CheckBox ChkAction = (CheckBox)row.FindControl("ChkAction");
                //TextBox Comment = (TextBox)row.FindControl("txtComment");
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
                    //Dr[15] = Convert.ToString(Comment.Text);
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
            CompanyId = Convert.ToString(Session["CompanyId"]);
            PaymentGeneratorUserId = Convert.ToString(Session["UserId"]);
            SessionId = Convert.ToString(Session["SessionId"]);

            if (!String.IsNullOrEmpty(CompanyId) && !String.IsNullOrEmpty(PaymentGeneratorUserId) && !String.IsNullOrEmpty(SessionId))
            {
                if (!string.IsNullOrEmpty(lblTxtBankAccountId.Text))
                {
                    string connetionstring = Convert.ToString(ConfigurationManager.ConnectionStrings["cons"]).Trim();
                    SqlConnection conn = new SqlConnection(connetionstring);
                    SqlCommand cmddata = new SqlCommand("Select CurEmpID from LicUser where  CurEmpID = '" + Convert.ToString(lblTxtSourceCompany.Text).Trim() + "-" + Convert.ToString(lblTxtBankAccountId.Text).Trim() + "' ", conn);
                    conn.Open();
                    SqlDataReader DataRow = cmddata.ExecuteReader();
                    if (!DataRow.HasRows)
                    {
                        DataSet DsGeneratePaymentData = new DataSet();
                        DsGeneratePaymentData = objDo.GetGeneratePaymentData(Convert.ToString(lblTxtSourceCompany.Text).Trim(), Convert.ToString(lblTxtGenerator.Text).Trim(), Convert.ToString(lblTxtBankAccountId.Text).Trim(), Convert.ToString(lblTxtSourceGroupId.Text).Trim(), Convert.ToString(txtVendorId.Text).Trim(), Convert.ToString(txtLegalNumber.Text).Trim());
                        if (DsGeneratePaymentData.Tables.Count > 0 && DsGeneratePaymentData.Tables[0].Rows.Count > 0)
                        {
                            grvGeneratePayment.DataSource = DsGeneratePaymentData.Tables[0];
                            grvGeneratePayment.DataBind();
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
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('Other user processing same Vote and Bank payment');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('Bank Account Id is must to proceed further.');", true);
                }
            }
            else
            {
                Session["TransMessage"] = "Invalid session data.";
                Response.Redirect("Stcl_ErrorMessage.aspx");
            }
        }

        //protected void btnHidden_Click(object sender, EventArgs e)
        //{
        //    GetData();
        //}

        protected void btnBack_Click(object sender, EventArgs e)
        {
            CompanyId = Convert.ToString(Session["CompanyId"]);
            PaymentGeneratorUserId = Convert.ToString(Session["UserId"]);
            SessionId = Convert.ToString(Session["SessionId"]);
            Response.Redirect("Stcl_ConsolidateGeneratePaymentInfoUsingSSO.aspx");
        }

        private string GetTreasuryCompany(string code)
        {
            string TreasuryCompany = string.Empty;
            DataSet DsTreasuryCompanyData = new DataSet();
            DsTreasuryCompanyData = objDo.GetGlobalData(code);
            if (DsTreasuryCompanyData.Tables.Count > 0 && DsTreasuryCompanyData.Tables[0].Rows.Count > 0)
            {
                TreasuryCompany = Convert.ToString(DsTreasuryCompanyData.Tables[0].Rows[0]["Value"]);
                return TreasuryCompany;
            }
            else
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('Treasury Company Id is missing. Please Contact Administrator.');", true);
                return "";
            }
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
                        StatusInformation = "<table><tr><th>SrNo</th><th>Bank Account Id </th><th>Source Company</th><th>Invoice Num </th><th>Status</th><th>Time</th></tr>" +
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
                //decimal InvoiceAmount = Convert.ToDecimal(((Label)e.Row.FindControl("txtInvoiceAmount")).Text);
                Label LblInvoiceAmount = (Label)e.Row.FindControl("txtInvoiceAmount");
                decimal InvoiceAmount = Convert.ToDecimal(LblInvoiceAmount.Text);
                SumOfInvoiceAmount = SumOfInvoiceAmount + InvoiceAmount;

                LblInvoiceAmount.Text = String.Format("{0:#,0.00}", InvoiceAmount);

                string SetupMissing = Convert.ToString(((Label)e.Row.FindControl("lblSetupMissing")).Text);
                if (!string.IsNullOrEmpty(SetupMissing))
                {
                    //TextBox TxtComment = (TextBox)e.Row.FindControl("txtComment");
                    CheckBox ChkGeneratePayment = (CheckBox)e.Row.FindControl("chkAction");
                    //TxtComment.Visible = false;
                    ChkGeneratePayment.Visible = false;
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
                    password: EpiorUserPassword,
                    operationTimeout: OperationTimeout,
                    validateWcfCertificate: ValidateWcfCertificate,
                    dnsEndpointIdentity: DnsEndpointIdentity,
                    licenseTypeId: Ice.License.LicensableUserCounts.Default);

            return BO;
        }


        private static string GetSysParam(string code)
        {
            var ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["cons"].ConnectionString;
            SqlConnection Con = new SqlConnection(ConnectionString.ToString());
            SqlCommand Cmd = new SqlCommand();
            Con.Open();
            Cmd.CommandText = "Stcl_CIFMIS_Global_GetGlobalData";
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Connection = Con;
            Cmd.Parameters.AddWithValue("@code", code);
            SqlDataAdapter Da = new SqlDataAdapter(Cmd);
            DataSet Ds = new DataSet();
            Da.Fill(Ds);
            if (Ds != null)
            {
                if (Ds.Tables.Count > 0)
                {
                    if (Ds.Tables[0].Rows.Count > 0)
                    {
                        return Ds.Tables[0].Rows[0][1].ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        #endregion

    }

}