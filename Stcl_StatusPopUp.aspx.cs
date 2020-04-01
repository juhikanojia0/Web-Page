using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Stcl.Epicor905.GeneratePayment.WebPages
{
    public partial class Stcl_StatusPopUp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {               
                Literal Lit = new Literal();
                Lit.Text = "<table><tr><td><center><h3>Generate Payment Status<h3></center><td></tr><tr>";
                Lit.Text = Lit.Text + Convert.ToString(Session["StatusInfo"]);
                Lit.Text = Lit.Text + "</tr></table>";
                Session["StatusInfo"] = "";
                LocalPlaceHolder.Controls.Add(Lit);
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            //ScriptManager.RegisterStartupScript(Page, typeof(Page), "Close", "window.close()", true);
        }
    }
}