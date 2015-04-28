using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace CVOService
{
    public class DAL
    {
        /*
            [ Person ---------------- ]
            [ DATA COLLECTION -----
                [ DATA CLC ACCESS RIGHTS ID ]
                [ DATA TYPE | DATA ELEMENT  ]
            --- ] 
         * 
         *  [ ACCESS RIGHTS TABLE ]
         *      [ DCLCId | PersonId | access rights ]
         *      [ DCLCId | PersonId | access rights ]
         *      [ DCLCId | PersonId | access rights ]
         *      
         * 
         *  Request Access to CLC
         *  Take Access to CLC
         * 
         *  [ ARRAY
         *      [ ARRAY ID | DATA TYPE: X | ELEMENT INDEX | TABLE INDEX ]
         *      
            [ DATA TYPE: MESSAGE ]
         *      [ INDEX | ---------------- ]
         *      
         *  [ DATA TYPE: PICTURE ]
         *      [ INDEX | ---------------- ]
         *      
         *  [ DATA TYPE: EVENT ]
         *      [ INDEX | status --------- ]
         *      
         *  [ DATA TYPE: COURSES ]
         *      [ INDEX | ---------------- ]
         *      
         * 
         *  [ DATA TYPE ACCES RIGHT ]
                [ INDEX | ----------------- ]
         * 
         * 
         *  Types of Messages
         *      assign/remove rights
                deadline 
                available 
                happening
                generic message
         * 
         * 
         * [ STATIC FEED/ARRAY/QUERY  ]
         *      [ ELEMENTS | -------- ]
         *      
         * [ DYNAMIC FEED/ARRAY/QUERY ]
         *      [ ELEMENTS | -------- ]
         */

        public static string GetUser()
        {
            string sel = "SELECT * FROM Collection";

            string result = "";

            using (SqlConnection con = new SqlConnection(cnc()))
            {
                using (SqlCommand cmd = new SqlCommand(sel, con))
                {
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        result += dr["CollectionId"].ToString();
                        result += dr["DataType"].ToString();
                        result += dr["Index"].ToString();
                        result += dr["OrderIndex"].ToString();
                    }
                    dr.Close();
                }
            }

            return result;
        }

        public static string cnc()
        {
            return ConfigurationManager
                .ConnectionStrings["Instant.mdf"]
                .ConnectionString;
        }
    }
}