using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Contracts;

namespace Mockups
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceRepo = new ServiceHost(typeof(ServiceRepository), new Uri[] { new Uri("net.tcp://127.0.0.1:11111/IServiceRepository") });
            var transferService = new ServiceHost(typeof(CanTransferMoney), new Uri[] { new Uri("net.tcp://127.0.0.1:11111/ICanTransferMoney") });
            var clientRepo = new ServiceHost(typeof(ClientRepository), new Uri[] { new Uri("net.tcp://127.0.0.1:11111/IClientRepository") });
            var accountRepo = new ServiceHost(typeof(AccountRepository), new Uri[] { new Uri("net.tcp://127.0.0.1:11111/IAccountRepository") });

            try
            {
                serviceRepo.AddServiceEndpoint(typeof(IServiceRepository), new NetTcpBinding(SecurityMode.None), "net.tcp://127.0.0.1:11111/IServiceRepository");
                transferService.AddServiceEndpoint(typeof(ICanTransferMoney), new NetTcpBinding(SecurityMode.None), "net.tcp://127.0.0.1:11111/ICanTransferMoney");
                clientRepo.AddServiceEndpoint(typeof(IClientRepository), new NetTcpBinding(SecurityMode.None), "net.tcp://127.0.0.1:11111/IClientRepository");
                accountRepo.AddServiceEndpoint(typeof(IAccountRepository), new NetTcpBinding(SecurityMode.None), "net.tcp://127.0.0.1:11111/IAccountRepository");

                serviceRepo.Open();
                transferService.Open();
                clientRepo.Open();
                accountRepo.Open();

                Console.WriteLine("Działające usługi:\n");
                Console.WriteLine("IServiceRepository");
                Console.WriteLine("ICanTransferMoney");
                Console.WriteLine("IClientRepository");
                Console.WriteLine("IAccountRepository");

                Console.ReadLine();
            }
            catch (CommunicationException commError)
            {
                Console.WriteLine("Communication error: " + commError.Message);
                serviceRepo.Abort();
                transferService.Abort();
                clientRepo.Abort();
                accountRepo.Abort();
                Console.Read();
            }
        }
    }
}
