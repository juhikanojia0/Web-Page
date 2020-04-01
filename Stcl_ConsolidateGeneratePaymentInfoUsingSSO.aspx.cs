using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;

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


namespace Stcl.EpicorErp10.GeneratePaymentWebPortal.WebPages
{
    public partial class Stcl_ConsolidateGeneratePaymentInfoUsingSSO : System.Web.UI.Page
    {
        string PaymentGeneratorUserId = string.Empty;
        string CompanyId = string.Empty;
        string Password = string.Empty;
        string SessionId = string.Empty;

        private Erp.UI.App.PaymentEntryEntry.Transaction oTransPay;
        private ILaunch iLaunch;
        private Session MyEpicorSession;

        BusinessAccessLayer.GeneratePaymentBusinessObjects objBo = new BusinessAccessLayer.GeneratePaymentBusinessObjects();
        DataAccessLayer.GeneratePaymentDataObjects objDo = new DataAccessLayer.GeneratePaymentDataObjects();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PaymentGeneratorUserId = Convert.ToString(Session["UserId"]);
                Password = Convert.ToString(Session["Password"]);
                ////CompanyId = "000";
                CompanyId = ddlCompany.SelectedValue.ToString();
                Session["ValidationAppInfo"] = System.Configuration.ConfigurationManager.AppSettings["ValidationAppInfo"];

                objBo.ServerUserId = PaymentGeneratorUserId;
                objBo.ServerPassword = Password;
                objBo.ServerUrl = System.Configuration.ConfigurationManager.AppSettings["ServerUrl"];

                try
                {
                    //MyEpicorSession = (Session)Session["EpicorSession"];
                    //if (MyEpicorSession != null)
                    //{
                    SessionId = "12345";// MyEpicorSession.SessionID;                       
                        //var connPool = MyEpicorSession.ConnectionPool;

                        if (!String.IsNullOrEmpty(CompanyId.Trim()) && !String.IsNullOrEmpty(PaymentGeneratorUserId.Trim()) && !string.IsNullOrEmpty(Convert.ToString(SessionId.Trim())))
                        {
                            DataSet DsGeneratePaymentConsolidatedData = new DataSet();
                            DsGeneratePaymentConsolidatedData = objDo.GetConsolidatedGeneratePaymentData(Convert.ToString(PaymentGeneratorUserId).Trim(), CompanyId, txtBankAccId.Text.ToString());
                            if (DsGeneratePaymentConsolidatedData.Tables.Count > 0 && DsGeneratePaymentConsolidatedData.Tables[0].Rows.Count > 0)
                            {
                                grvGeneratePaymentConsolidatedData.DataSource = DsGeneratePaymentConsolidatedData.Tables[0];
                                grvGeneratePaymentConsolidatedData.DataBind();
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(Page, GetType(), "Error", "alert('No data for generate payment.');", true);
                            }
                        }
                        //MyEpicorSession.Dispose();
                    //}
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
            }
        }

        protected void grvGeneratePaymentConsolidatedData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Proceed")
            {
                SessionId = Convert.ToString(Session["SessionId"]);

                int index = Convert.ToInt32(e.CommandArgument.ToString());
                Label lblBankAcctID = (Label)grvGeneratePaymentConsolidatedData.Rows[index].FindControl("lblBankAcctID");
                Label lblSrcCompany = (Label)grvGeneratePaymentConsolidatedData.Rows[index].FindControl("lblSrcCompany");
                Label lblTotalRecord = (Label)grvGeneratePaymentConsolidatedData.Rows[index].FindControl("lblTotalRecord");
                Label lblVoucherListNum = (Label)grvGeneratePaymentConsolidatedData.Rows[index].FindControl("lblVoucherListNum");

                ////Session["CompanyId"] = "000";
                Session["CompanyId"] = ddlCompany.SelectedValue.ToString();
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
                Response.Redirect("Stcl_GeneratePaymentUsingSSO.aspx");
            }
        }

        protected void grvGeneratePaymentConsolidatedData_RowDataBound(object sender, GridViewRowEventArgs e)
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

                    DataRow[] DataRow = Dt.Select(" BankAcctID='" + Convert.ToString(LblBankAcctID.Text).Trim() + "' AND SrcCompany='" + Convert.ToString(LblSrcCompany.Text).Trim() + "' AND VoucherListNum = '" + Convert.ToString(LblVoucherListNum.Text).Trim() + "'");//AND CurrentSession <> '" + Convert.ToString(Session.SessionID) + "'
                    if ((DataRow.Length > 0) || string.IsNullOrEmpty(LblVoucherListNum.Text))
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

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("Stcl_Login.aspx");
        }
    }   
}