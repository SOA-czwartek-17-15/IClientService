using System;
using System.Runtime.Serialization;

namespace ClientService.Domain
{
    [DataContract]
    public class Client
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public Guid ClientId { get; set; }
        [DataMember]
        public string AccountNumber { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public double Interest { get; set; }
        [DataMember]
        public int CapitalizationPeriod { get; set; }
        [DataMember]
        public double Money { get; set; }
        [DataMember]
        public string OpeningDate { get; set; }
        [DataMember]
        public string Duration { get; set; }
    }
}