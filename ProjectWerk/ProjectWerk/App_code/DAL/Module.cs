using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ProjectWerk.App_code.DAL
{
    public class Module : Base
    {

        public int Id { get; set; }
        public string ModuleName { get; set; }



        public Module()
        {

        }

        public List<Module> ReadList(SqlCommand command)
        {
            List<Module> Modules = new List<Module>();
            using (SqlDataReader dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    Module module = new Module();
                    module = ReadData(module, dr);
                    Modules.Add(module);
                }
            }
            return Modules;
        }


        public Module ReadData(Module module, SqlDataReader dr)
        {
            // hier komt de data van de databank
            return module;
        }




    }
}