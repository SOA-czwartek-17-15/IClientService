using System;
using System.Runtime.Serialization;

namespace IClientService.Domain
{
    public class ClientOperation
    {
        protected ClientOperation() { }
        public ClientOperation(Guid _ClientId, string _Type, string _Info)
        {
            Id = Guid.NewGuid();
            ClientId = _ClientId;
            Type = _Type;
            Info = _Info;
            Date = DateTime.Now;
        }

        public virtual Guid Id { set; get; }
        public virtual Guid ClientId { set; get; }
        public virtual string Type { set; get; }
        public virtual string Info { set; get; }
        public virtual DateTime Date { set; get; }
    }
}