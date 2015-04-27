using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CVOService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServices" in both code and config file together.
    [ServiceContract]
    public interface ModuleIF
    {
        [OperationContract]
        List<ModuleBLL> GetModules(string token);
        [OperationContract]
        bool AddModule(int index);
        [OperationContract]
        bool RemoveModule(int index);
    }

}
