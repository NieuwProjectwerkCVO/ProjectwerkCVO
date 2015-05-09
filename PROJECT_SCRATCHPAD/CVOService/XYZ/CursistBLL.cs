using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CVOService
{
    [DataContract]
    public class CursistBLL
    {
        [DataMember]
        public int StudentId { get; set; }
        [DataMember]
        public int CursistNummer { get; set; }
        [DataMember]
        public string Paswoord { get; set; }
        [DataMember]
        public string Voornaam { get; set; }
        [DataMember]
        public string Familienaam { get; set; }
        [DataMember]
        public int EventId { get; set; }
    }
}