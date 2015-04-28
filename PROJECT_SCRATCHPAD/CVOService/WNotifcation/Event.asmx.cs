using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;

namespace CVOService
{
    /// <summary>
    /// Summary description for Event
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Event : System.Web.Services.WebService
    {

        public class Event
        {
            public int id;
            public string title;
            public string description;
            public int count;
            public int capacity;
            public DateTime t1;
            public DateTime t2;
            public int statusid;
        }

        [WebMethod]
        public string SelectEvents(string token){
            return "";
        }

        [WebMethod]
        public string ScheduleEvent(
            //string token,
            string title,
            string description,
            int capacity,
            DateTime t1,
            DateTime t2,
            int statusid
            // public/private
            // notification -> link to [events, grade, document]
            // event -> doesnt point at anything
        )
        {
      
            ErrorMessage em = new ErrorMessage();

            string command =
                  " INSERT INTO Event (Title, Description, Count, Capacity, T1, T2, StatusId)"
                + " VALUES (";

            using (SqlConnection con = new SqlConnection(cnc()))
            {
                using (SqlCommand cmd = new SqlCommand(command, con))
                {
                    con.Open();

                    int rows_affected = cmd.ExecuteNonQuery();

                    if (rows_affected > 0)
                    {

                    }
                }
            }

            return 
        }

        [WebMethod]
        public string DeleteEvent(string token)
        {
            return "Hello World";
        }

        [WebMethod]
        public string SignEvent(string token)
        {
            return "Hello World";
        }

        [WebMethod]
        public string UnsignEvent(string token)
        {
            return "Hello World";
        }

        public static string cnc()
        {
            return ConfigurationManager
                .ConnectionStrings["InstantConnection"]
                .ConnectionString;
        }
    }
}
