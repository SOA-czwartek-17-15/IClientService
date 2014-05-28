using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using IClientService.Domain;

using Contracts;
using log4net;
using NHibernate;

namespace ClientService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ClientService : IClientService
    {
        private readonly ILog logger;

        // baza danych
        DB_Interface database = new DB_NHibernate();
        //DB_Interface database = new DB_Mockup();

        // repozytoria i usługi
        private IServiceRepository serviceRepo;
        private IClientRepository clientRepo;
        private IAccountRepository accountRepo;
        private ICanTransferMoney transferService;

        public ClientService() { }
        public ClientService(IServiceRepository _serviceRepo, ILog _logger)
        {
            serviceRepo = _serviceRepo;
            logger = _logger;
        }

        public bool TransferMoney()
        {
            try
            {
                var serviceAddress = serviceRepo.GetServiceLocation("ICanTransferMoney");
                if (serviceAddress == null)
                {
                    Console.WriteLine("Usługa jest w tej chwili niedostępna.");
                    Program.LogError("Usługa ICanTransferMoney jest niedostępna.");
                    return false;
                }

                var cf = new ChannelFactory<ICanTransferMoney>(new NetTcpBinding(SecurityMode.None), serviceAddress);
                transferService = cf.CreateChannel();

                Console.Clear();
                Console.WriteLine("Podaj kwotę przelewu:");
                long amount = Convert.ToInt64(Console.ReadLine());

                if (transferService.TransferMoney(new Guid(), new Guid(), amount))
                {
                    // Zapis operacji do bazy
                    ClientOperation operation = new ClientOperation(Guid.NewGuid(), "TransferMoney", "Kwota: " + amount + " zł");
                    database.AddOperation(operation);

                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                Program.LogError(e.Message);
                return false;
            }
        }


        public bool CreateClient()
        {
            try
            {
                var serviceAddress = serviceRepo.GetServiceLocation("IClientRepository");
                if (serviceAddress == null)
                {
                    Console.WriteLine("Usługa jest w tej chwili niedostępna.");
                    Program.LogError("Usługa IClientRepository jest niedostępna.");
                    return false;
                }

                var cf = new ChannelFactory<IClientRepository>(new NetTcpBinding(SecurityMode.None), serviceAddress);
                clientRepo = cf.CreateChannel();


                var client = new ClientInformation();
                Console.Clear();
                Console.Write("Imię:    \t");
                client.FirstName = Console.ReadLine();
                Console.Write("Nazwisko:\t");
                client.LastName = Console.ReadLine();
                Console.Write("Miasto:  \t");
                client.City = Console.ReadLine();

                if (clientRepo.CreateClient(client) == Guid.Empty)
                    return false;
                else
                {
                    // Zapis operacji do bazy
                    ClientOperation operation = new ClientOperation(Guid.NewGuid(), "CreateClient", client.FirstName + " " + client.LastName + ", " + client.City);
                    database.AddOperation(operation);

                    return true;
                }

            }
            catch (Exception e)
            {
                Program.LogError(e.Message);
                return false;
            }
        }


        public bool GetClientInformation()
        {
            try
            {
                var serviceAddress = serviceRepo.GetServiceLocation("IClientRepository");
                if (serviceAddress == null)
                {
                    Console.WriteLine("Usługa jest w tej chwili niedostępna.");
                    Program.LogError("Usługa IClientRepository jest niedostępna.");
                    return false;
                }

                var cf = new ChannelFactory<IClientRepository>(new NetTcpBinding(SecurityMode.None), serviceAddress);
                clientRepo = cf.CreateChannel();
                
                Console.Clear();
                ClientInformation client = clientRepo.GetClientInformation(new Guid());
                Console.WriteLine("Imię:\t\t" + client.FirstName);
                Console.WriteLine("Nazwisko:\t" + client.LastName);
                Console.WriteLine("Ulica:\t\t" + client.Street);
                Console.WriteLine("Kod:\t\t" + client.PostCode);
                Console.WriteLine("Miasto:\t\t" + client.City);
                Console.WriteLine("Kraj:\t\t" + client.Country);
                Console.WriteLine("Data urodzenia:\t" + client.BirthDate);


                // Zapis operacji do bazy
                ClientOperation operation = new ClientOperation(Guid.NewGuid(), "GetClientInformation", client.FirstName + " " + client.LastName);
                database.AddOperation(operation);

                return true;

            }
            catch (Exception e)
            {
                Program.LogError(e.Message);
                return false;
            }
        }


        public bool GetAccountInformation()
        {
            try
            {
                var serviceAddress = serviceRepo.GetServiceLocation("IAccountRepository");
                if (serviceAddress == null)
                {
                    Console.WriteLine("Usługa jest w tej chwili niedostępna.");
                    Program.LogError("Usługa IAccountRepository jest niedostępna.");
                    return false;
                }

                var cf = new ChannelFactory<IAccountRepository>(new NetTcpBinding(SecurityMode.None), serviceAddress);
                accountRepo = cf.CreateChannel();
                
                Console.Clear();
                Account account = accountRepo.GetAccountInformation("");
                //Console.WriteLine("Nr konta:\t" + account.AccountNumber);
                Console.WriteLine("Kasa:\t\t" + account.Money + " zł");
                Console.WriteLine("Id klienta:\t" + account.ClientId);
                Console.WriteLine("Typ:\t\t" + account.Type);
                Console.WriteLine("Oprocentowanie:\t" + account.Percentage + "%");
                Console.WriteLine("Otwarcie:\t" + account.StartDate);
                Console.WriteLine("Zamkniecie:\t" + account.EndDate);
                

                // Zapis operacji do bazy
                ClientOperation operation = new ClientOperation(Guid.NewGuid(), "GetAccountInformation", account.Money + " zł, typ: " + account.Type + ", oproc. " + account.Percentage + "%");
                database.AddOperation(operation);

                return true;

            }
            catch (Exception e)
            {
                Program.LogError(e.Message);
                return false;
            }
        }

        
        public bool GetOperations()
        {
            try
            {
                IEnumerable<ClientOperation> operations = database.GetAll();
                if (!operations.Any())
                {
                    Console.WriteLine("Historia operacji jest pusta.");
                }
                else
                {
                    foreach (ClientOperation op in operations)
                    {
                        //Console.WriteLine("ID:\t\t" + op.Id);
                        Console.WriteLine("Typ:\t\t" + op.Type);
                        Console.WriteLine("Informacje:\t" + op.Info);
                        Console.WriteLine("Data:\t\t" + op.Date);
                        Console.WriteLine();
                    }
                }

                return true;

            }
            catch (Exception e)
            {
                Program.LogError(e.Message);
                return false;
            }
        }
    }
}
