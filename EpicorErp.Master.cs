/*************************************************************
* Project   : CIFMIS_GeneratePayment Web Portal for E10.1
* Author    : Mahesh D. Deore
* Date      : 10 Dec 2015
* Purpose   : to sign_out & release session for all screens [modified the code which is already developed for ERP 9]
* Version   : 1.0.0.0
*************************************************************/

using System;
using System.Data;

namespace Stcl.EpicorErp.GeneratePayment.WebPages
{
    public partial class EpicorErp : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SessionId"] != null)
            {
            }
            else
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["TransMessage"])))
                {
                    Session.Clear();
                    Session.Abandon();
                    Response.Redirect("Stcl_Login.aspx");
                }
            }
        }

        protected void LinkSignOut_Click(object sender, EventArgs e)
        {
            if (Convert.ToString(Session["IsSSOUser"]).ToUpper() == "TRUE")
            {
                Session["TransMessage"] = "Close browser to logged out the application successfully.";
                Response.Redirect("Stcl_ErrorMessage.aspx");
            }

            DataTable Dt = new DataTable();
            Dt = (DataTable)Application["DtCurrentSession"];
            if (Dt != null)
            {
                DataRow[] drr = Dt.Select("CurrentSession='" + Convert.ToString(Session.SessionID) + "'");
                if (drr.Length > 0)
                {
                    for (int i = 0; i < drr.Length; i++)
                    {
                        drr[i].Delete();
                    }
                    Dt.AcceptChanges();
                }
            }
            Session.Clear();
            Session.Abandon();

            Session["TransMessage"] = "You have logged out the application successfully.";
            Response.Redirect("Stcl_ErrorMessage.aspx");
        }
    }
}