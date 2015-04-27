using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace CVOService
{
    public class EventDAL
    {
        public static List<string> GetEvents()
        {

            List<string> result = new List<string>();

            using (SqlConnection con = new SqlConnection(cnc()))
            {
                string S1 = "SELECT * FROM LesDavinci";
                using (SqlCommand cmd = new SqlCommand(S1, con))
                {
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        result.Add(dr["Lesdatum"].ToString());
                        result.Add(dr["Afgelast"].ToString());
                        result.Add(dr["IdLesplaats"].ToString());
                    }
                    dr.Close();
                }
            }

            using (SqlConnection con = new SqlConnection(cnc()))
            {
                string S2 = "SELECT * FROM Lesplaats";
                using (SqlCommand cmd = new SqlCommand(S2, con))
                {
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        result.Add(dr["Naam"].ToString());
                        result.Add(dr["Aanvangsdatum"].ToString());
                        result.Add(dr["Einddatum"].ToString());
                    }
                    dr.Close();
                }
            }

            result.Add("token");
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