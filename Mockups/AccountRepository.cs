using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Contracts;

namespace Mockups
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class AccountRepository : IAccountRepository
    {
        public bool CreateAccount(Account details)
        {
            return true;
        }

        public Account GetAccountInformation(string accountNumber)
        {
            var account = new Account();

            account.AccountNumber = "50 1020 5558 1111 1360 6520 0097";
            account.ClientId = Guid.NewGuid();
            account.Money = 555000;
            account.Type = AccountType.Normal;
            account.Percentage = 4;
            account.StartDate = new System.DateTime(2010, 01, 01);
            account.EndDate = new System.DateTime(2015, 12, 31);

            return account;
        }

        public Account GetAccountById(Guid accountId)
        {
            return null;
        }

        public bool ChangeAccountBalance(Guid accountId, long amount)
        {
            return true;
        }
    }
}
