using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Stcl.EpicorERP10.GeneratePaymentWebPortal
{
    public partial class Default : System.Web.UI.Page
    {
        public override void ProcessRequest(HttpContext context)
        {
            context.Response.Redirect("WebPages/Stcl_Login.aspx", true);
        }
    }
}