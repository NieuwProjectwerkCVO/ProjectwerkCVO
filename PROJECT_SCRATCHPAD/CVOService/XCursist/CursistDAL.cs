using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace CVOService
{
    public class CursistDAL
    {
        public static string GetUser()
        {
            string sel = "SELECT * FROM Centrum";

            string result = "";

            using (SqlConnection con = new SqlConnection(cnc()))
            {
                using (SqlCommand cmd = new SqlCommand(sel, con))
                {
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        result += dr["Id"].ToString();
                    }
                    dr.Close();
                }
            }

            return result;
        }
  
        public static string cnc(){
            return ConfigurationManager
                .ConnectionStrings["ConnectionString"]
                .ConnectionString;
        }
    
    }
}