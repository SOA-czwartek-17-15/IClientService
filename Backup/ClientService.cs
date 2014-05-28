using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using ClientService.Domain;

namespace ClientService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ClientService : IClientService
    {
        public bool Login(string clientId, string password)
        {
            return true;
        }

        public bool Logout(string clientId)
        {
            return true;
        }

        public Guid CreateAccount(Guid clientId, Client account)
        {
            return new Guid();
        }

        public Client GetAccountInformation(Guid accountId)
        {
            return null;
        }

        public void RemoveAccount(Guid accountId)
        {
            return;
        }
    }
}
