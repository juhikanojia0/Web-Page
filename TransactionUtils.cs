using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;

namespace Stcl.EpicorErp10.GeneratePaymentWebPortal.WebPages
{
    public class TransactionUtils
    {
        public static TransactionScope CreateTransactionScope()
        {
            var transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            transactionOptions.Timeout = TransactionManager.MaximumTimeout;
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }
    }
}