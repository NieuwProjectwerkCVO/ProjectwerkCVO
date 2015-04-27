using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace CVOService
{
    public class ModuleDAL
    {
        public static List<string> GetModules()
        {
            // Modulevariant
            // Naam van Module

            // LesDavinci = Rooster
            // ModuleVariantId

            // datum - dag - campus - start - eind - Lokaal - cursus - modulenaam

            string sel = "SELECT * FROM Modulevariant";

            List<string> result = new List<string>();

            using (SqlConnection con = new SqlConnection(cnc()))
            {
                using (SqlCommand cmd = new SqlCommand(sel, con))
                {
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        result.Add(dr["Naam"].ToString());
                    }
                    dr.Close();
                }
            }

            return result;
        }

        public static string cnc()
        {
            return ConfigurationManager
                .ConnectionStrings["ConnectionString"]
                .ConnectionString;
        }
    }

   
}