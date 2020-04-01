using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;

namespace Stcl.EpicorErp10.GeneratePaymentWebPortal.WebPages
{
    public static class UserExtensions
    {
        public static string GetDomain(this IIdentity identity)
        {
            string s = identity.Name;
            int stop = s.IndexOf("\\");
            return (stop > -1) ? s.Substring(0, stop) : string.Empty;
        }

        public static string GetLogin(this IIdentity identity)
        {
            string s = identity.Name;
            int stop = s.IndexOf("\\");
            return (stop > -1) ? s.Substring(stop + 1, s.Length - stop - 1) : string.Empty;
        }

        //public static string GetDomain(this IIdentity identity)
        //{
        //    return Regex.Match(identity.Name, ".*\\\\").ToString();
        //}

        //public static string GetLogin(this IIdentity identity)
        //{
        //    return Regex.Replace(identity.Name, ".*\\\\", "");
        //}
    }
}