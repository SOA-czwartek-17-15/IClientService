using System.ServiceModel;
using System;

using IClientService.Domain;

namespace ClientService
{
    [ServiceContract]
    public interface IClientService
    {
        [OperationContract]
        bool TransferMoney();

        [OperationContract]
        bool CreateClient();

        [OperationContract]
        bool GetClientInformation();

        [OperationContract]
        bool GetAccountInformation();

        [OperationContract]
        bool GetOperations();

    }
}