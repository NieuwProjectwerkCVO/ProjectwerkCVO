using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CVOService
{
    [DataContract]
    public class ModuleBLL
    {
        [DataMember]
        string id { get; set; }
        [DataMember]
        string shortname { get; set; }
        [DataMember]
        string fullname { get; set; }
        [DataMember]
        string enrolledusercount { get; set; }
        [DataMember]
        string idnumber { get; set; }
        [DataMember]
        string visible { get; set; }
    }
    
}