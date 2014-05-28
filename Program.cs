﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Configuration;
using System.Timers;

using Contracts;
using log4net;
using IClientService.Domain;

namespace ClientService
{
    public class Program
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Program));
        private static Timer aliveTimer;
        private static ClientService service;

        private const string selfAddress = "net.tcp://127.0.0.1:55555/IClientService";
       // private const string selfAddress = "net.tcp://87.206.208.149:55555/IClientService";
        private const string selfAddressLan = "net.tcp://0.0.0.0:55555/IClientService";

        private static IServiceRepository serviceRepo;


        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            // utworzenie service host
            var sh = new ServiceHost(typeof(ClientService), new Uri[] { new Uri(selfAddressLan) });
            try
            {

                // uruchomienie usługi i czekanie na połaczenia
                sh.AddServiceEndpoint(typeof(IClientService), new NetTcpBinding(SecurityMode.None), selfAddressLan);
                ServiceMetadataBehavior metadata = sh.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (metadata == null)
                {
                    metadata = new ServiceMetadataBehavior();
                    sh.Description.Behaviors.Add(metadata);
                }
                metadata.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                sh.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexTcpBinding(), "mex");
                sh.Open();

                Log("Uruchomiono usługe ClientService");
            }
            catch (CommunicationException commError)
            {
                Console.WriteLine("Communication error: " + commError.Message);
                LogError(commError.Message);
                sh.Abort();
                Console.Read();
            }

            // utworzenie timera
            aliveTimer = new Timer();
            aliveTimer.Interval = (1000) * (5);
            aliveTimer.Elapsed += new ElapsedEventHandler(KeepAlive);
            aliveTimer.Enabled = true;


            // rejestracja usługi
            while (!Register(false)) ;


            service = new ClientService(serviceRepo, logger);
            ShowMainMenu(service);
           

            try
            {
                // wyrejestrowanie usługi i zatrzymanie timera
                serviceRepo.Unregister("IClientService");
                aliveTimer.Stop();
                sh.Close();
                Log("Wyrejestrowano usługę ClientService");
            }
            catch (Exception e)
            {
                LogError(e.Message);
            }
        }

        private static bool Register(bool reRegister)
        {
            // pobranie adresu ServiceRepository z App.config
            string serviceRepoAddress = System.Configuration.ConfigurationManager.AppSettings["serviceRepoAddress"];
            Console.WriteLine("Łączę z ServiceRepository pod: " + serviceRepoAddress);

            // połączenie z ServiceRepository
            var cf = new ChannelFactory<IServiceRepository>(new NetTcpBinding(SecurityMode.None), serviceRepoAddress);
            serviceRepo = cf.CreateChannel();

            // rejestracja naszej usługi w ServiceRepository
            try
            {
                serviceRepo.RegisterService("IClientService", selfAddress);
                Console.WriteLine("Zarejestrowano usługę ClientService.");
                Log("Zarejestrowano usługe ClientService");
                aliveTimer.Start();
                if (reRegister)
                    ShowMainMenu(service);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Nie udało się zarejestrować. Próbuję ponownie...");
                LogError(e.Message);
                System.Threading.Thread.Sleep(5000);
                Console.Clear();
            }
            return false;
        }

        private static void KeepAlive(object sender, EventArgs e)
        {
            try
            {
                serviceRepo.Alive("IClientService");
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine("Utracono połączenie z serwerem. Próbuję ponownie...");
                LogError(ex.Message);
                aliveTimer.Stop();
                while (!Register(true)) ;
            }
        }

        public static void Log(String log)
        {
            DateTime czas = DateTime.Now;
            logger.Info(log);
        }
        public static void LogError(String log)
        {
            DateTime czas = DateTime.Now;
            logger.Error(log);
        }


        private static void ShowMainMenu(ClientService service)
        {
            ConsoleKeyInfo key;
            do
            {
                Console.Clear();
                Console.WriteLine("Menu główne");
                Console.WriteLine();
                Console.WriteLine("1. Wykonaj przelew");
                Console.WriteLine("2. Utwórz klienta");
                Console.WriteLine("3. Informacje o kliencie");
                Console.WriteLine("4. Informacje o koncie");
                Console.WriteLine("5. Historia operacji");
                Console.WriteLine("\nESC. Wyjście");
                key = Console.ReadKey(true);
                switch (key.KeyChar.ToString())
                {
                    case "1":
                        Console.Clear();
                        if (service.TransferMoney())
                        {
                            Console.WriteLine("\nPrzelew został zlecony.");
                            Log("Przelew został zlecony.");
                        }
                        else
                            Console.WriteLine("\nWystąpił błąd. Spróbuj ponownie.");                        
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Clear();
                        if (service.CreateClient())
                        {
                            Console.WriteLine("\nUtworzono nowego klienta.");
                            Log("Utworzono nowego klienta.");
                        }
                        else
                            Console.WriteLine("\nWystąpił błąd. Spróbuj ponownie.");
                        Console.ReadKey();
                        break;

                    case "3":
                        Console.Clear();
                        if (service.GetClientInformation())
                            Log("Pobrano informacje o kliencie.");
                        else
                            Console.WriteLine("\nWystąpił błąd. Spróbuj ponownie.");
                        Console.ReadKey();
                        break;

                    case "4":
                        Console.Clear();
                        if (service.GetAccountInformation())
                            Log("Pobrano informacje o koncie.");
                        else
                            Console.WriteLine("\nWystąpił błąd. Spróbuj ponownie.");
                        Console.ReadKey();
                        break;

                    case "5":
                        Console.Clear();
                        if (service.GetOperations())
                            Log("Pobrano historię operacji.");
                        else
                            Console.WriteLine("\nWystąpił błąd. Spróbuj ponownie.");
                        Console.ReadKey();
                        break;
                }
            } while (key.Key != ConsoleKey.Escape);
        }
        

      

    }
}
