using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CVOService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICursistSVC" in both code and config file together.
    [ServiceContract]
    public interface CursistIF
    {
        [OperationContract]
        String Login(string gebruikersnaam, string paswoord);
        [OperationContract]
        String Logout();
        [OperationContract]
        String Registreer();
        [OperationContract]
        String Unregistreer();
    }
}
