using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Contracts;

namespace Mockups
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class ClientRepository : IClientRepository
    {
        public Guid CreateClient(ClientInformation clientInfo)
        {
            return Guid.NewGuid();
        }

        public ClientInformation GetClientInformation(Guid clientID)
        {
            var client = new ClientInformation();
            client.FirstName = "Jan";
            client.LastName = "Kowalski";
            client.Street = "Wymyślona 15";
            client.PostCode = "12-345";
            client.City = "Pcim Dolny";
            client.Country = "Polska";
            client.BirthDate = new System.DateTime(1992, 01, 23);

            return client;
        }

        public IEnumerable<Guid> SearchForClientsBy(ClientInformation someClientInfo)
        {
            return null;
        }
    }
}
