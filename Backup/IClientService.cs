using System.ServiceModel;
using System;

using ClientService.Domain;

namespace ClientService
{
    [ServiceContract]
    public interface IClientService
    {
        [OperationContract]
        bool Login(string clientId, string password);

        [OperationContract]
        bool Logout(string clientId);

        [OperationContract]
        Guid CreateAccount(Guid clientId, Client account);

        [OperationContract]
        Client GetAccountInformation(Guid accountId);

        [OperationContract]
        void RemoveAccount(Guid accountId);
    }
}