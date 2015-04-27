using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CVOService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Services" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Services.svc or Services.svc.cs at the Solution Explorer and start debugging.
    public class ModuleSVC : ModuleIF
    {
        List<ModuleBLL> data;
        public List<ModuleBLL> GetModules(string token)
        {
            data = new List<ModuleBLL>();

            return data;
        }

        public bool AddModule(int index)
        {
            // betalen
            return true;
        }

        public bool RemoveModule(int index)
        {
            return true;
        }
    }
}
