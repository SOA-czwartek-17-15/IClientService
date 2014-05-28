using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contracts;
using IClientService.Domain;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace ClientService
{
    class DB_NHibernate : DB_Interface
    {
        private ISession session;

        public DB_NHibernate()
        {
            try
            {
                // Otwarcie sesji NHibernate
                NHibernate.Cfg.Configuration config = new NHibernate.Cfg.Configuration();
                config.Configure();
                config.AddAssembly(typeof(ClientOperation).Assembly);
                new SchemaExport(config).Execute(false, true, false); //Drugi na true gdy chcemy dropTable robić przy każdym uruchomieniu, false gdy mamy już uworzoną tabele
                ISessionFactory factory = config.BuildSessionFactory();
                session = factory.OpenSession();
                Console.WriteLine("Otwarto sesję NHibernate.");
                Program.Log("Otwarto sesję NHibernate");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Program.LogError("Nie udało się otworzyć sesji NHibernate" + e.Message);
            }
        }

        public bool AddOperation(ClientOperation operation)
        {
            try
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(operation);
                    transaction.Commit();
                    Program.Log("Dodano informację o operacji do bazy.");
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Program.LogError("Błąd podczas dodawania operacji do bazy. " + e.Message);
                return false;
            }
        }

        public IEnumerable<ClientOperation> GetAll()
        {
            return session.CreateCriteria<ClientOperation>().List<ClientOperation>();
            //return new ClientOperation[] {new ClientOperation(Guid.NewGuid(), "Zalogowanie do systemu", "Login: krisu"), new ClientOperation(Guid.NewGuid(), "Wykonanie przelewu", "Kwota 200 zł") };
        }
    }
}
