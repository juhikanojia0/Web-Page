using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Data;

namespace Stcl.EpicorErp10.GeneratePaymentWebPortal.WebPages
{
    public class Global : System.Web.HttpApplication
    {        

        protected void Application_Start(object sender, EventArgs e)
        {
            Application["DtCurrentSession"] = null;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Session["init"] = 0;
            Session["CompanyId"] = "000";
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
          
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
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
            //Application["DtCurrentSession"] = null;
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Application["DtCurrentSession"] = null;
        }
    }
}