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

namespace ClientService
{
    public class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        private static IServiceRepository svcRepository;
        private const string selfAddress = "net.tcp://127.0.0.1:55555/IClientService";
        static void Main(string[] args)
        {
            // load log4net configuration

            log4net.Config.XmlConfigurator.Configure();
            //log.Info("this is the first log message");

            // create service host

            var sh = new ServiceHost(typeof(ClientService), new Uri[] { new Uri(selfAddress) });

            try
            {
                // setup service endpoint

                sh.AddServiceEndpoint(typeof(IClientService), new NetTcpBinding(SecurityMode.None), selfAddress);

                // start service and wait for clients

                sh.Open();

                // get ServiceRepository address from App.config

                string svcRepositoryAddr = System.Configuration.ConfigurationManager.AppSettings["svcRepositoryAddress"];
                Console.WriteLine("Łącze z ServiceRepository pod: " + svcRepositoryAddr);

                // create channel to service repository

                var cf = new ChannelFactory<IServiceRepository>(new NetTcpBinding(SecurityMode.None), svcRepositoryAddr);
                svcRepository = cf.CreateChannel();

                // register our service

                svcRepository.RegisterService("IClientService", selfAddress);


                var adres = svcRepository.GetServiceLocation("IClientRepository");
                if (adres != null)
                    Console.WriteLine("Połaczono. IClientRepository dostepny pod: " + adres);
                else
                    Console.WriteLine("Błąd połączenia");
                // create the timer which whill remind service repository that we're alive every 5 secs


                Timer aliveTimer = new Timer();
                aliveTimer.Interval = (1000) * (5);
                aliveTimer.Elapsed += new ElapsedEventHandler(KeepAlive);
                aliveTimer.Enabled = true;
                aliveTimer.Start();

                // press ENTER to finish

                Console.ReadLine();

                // unregister service before closing

                svcRepository.Unregister("IClientService");

                // stope the timer and close our service

                aliveTimer.Stop();
                sh.Close();
            }
            catch (CommunicationException commError)
            {
                Console.WriteLine("Communication error: " + commError.Message);
                sh.Abort();
                Console.Read();
            }
        }

        private static void KeepAlive(object sender, EventArgs e)
        {
            svcRepository.Alive("IClientService");
        }
    }
}
