using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IClientService.Domain;

namespace ClientService
{
    public class DB_Mockup : DB_Interface
    {
        public bool AddOperation(ClientOperation operation)
        {
            return true;
        }

        public IEnumerable<ClientOperation> GetAll()
        {
            return null;
            //return new ClientOperation[] { new ClientOperation(Guid.NewGuid(), "Zalogowanie do systemu", "Login: krisu"), new ClientOperation(Guid.NewGuid(), "Wykonanie przelewu", "Kwota 200 zł") };
        }
    }
}
