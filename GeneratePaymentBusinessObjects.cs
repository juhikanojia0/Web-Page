using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stcl.EpicorErp10.GeneratePaymentWebPortal.BusinessAccessLayer
{
    public class GeneratePaymentBusinessObjects
    {
        public string ConnectionString { get; set; }

        public string SessionPlantId { get; set; }
        public string ServerUrl { get; set; }
        public string ServerUserId { get; set; }
        public string ServerPassword { get; set; }

        public string SessionUserId { get; set; }
        public string SessionCompany { get; set; }

        public string SourceCompany{get; set;} 
        public string BankAcctId{get; set;}
        public string SessionId { get; set;} 
        public string ApplicationUserValue {get; set;} 
        public string PaymentEntryStatus{get; set;} 
        public bool   StatusInfoFlag{get; set;}
        public string VoucherListNum { get; set; }
        public bool IsSalaryPay { get; set; }
        public Boolean HeadNumExists { get; set; }
        public Boolean AllowLogAppending { get; set; }
    }
}