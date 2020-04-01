/*************************************************************
* Project   : CIFMIS_GeneratePayment Web Portal for E10
* Author    : Mahesh D. Deore
* Date      : 10 Dec 2015
* Purpose   : to show catched error
* Version   : 1.0.0.0
*************************************************************/
using System;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;

namespace Stcl.EpicorErp10.GeneratePaymentWebPortal.WebPages
{
    public partial class Stcl_ErrorMessage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //===Access master page control from child page==============================            
                LinkButton lnkLinkSignOut = (LinkButton)Master.FindControl("LinkSignOut");
                lnkLinkSignOut.Visible = false;
                //===========================================================================
                
                if (Request.UrlReferrer != null)
                {
                    string result;
                    result = Path.GetFileName(Path.GetFileName(Request.UrlReferrer.ToString()));
                    string[] PageName = result.Split('?');
                    ViewState["Backpage"] = PageName[0];

                    string Message = Convert.ToString(Session["TransMessage"]);
                    lblMessage.Text = Message.ToString();
                   
                    if (Convert.ToString(Session["TransMessage"]).ToUpper() == "You have logged out the application successfully.".ToUpper())
                    {
                        btnOk.Text = "Click here to Login";
                    }
                }
            }
            catch (Exception ex)
            {
                Session["TransMessage"] = Convert.ToString(ex.Message);
                throw;
            }
            finally
            {               
                Session.Abandon();
            }
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            Session["TransMessage"] = "";
            Response.Redirect("Stcl_Login.aspx");
        }
    }
}