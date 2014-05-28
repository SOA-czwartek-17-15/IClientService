using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IClientService.Domain;

namespace ClientService
{
    public interface DB_Interface
    {

        bool AddOperation(ClientOperation operation);

        IEnumerable<ClientOperation> GetAll();

    }
}
