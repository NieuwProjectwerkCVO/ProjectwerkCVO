using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;

namespace CVOService.WNotifcation
{
    /// <summary>
    /// Summary description for Messaging
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Messaging : System.Web.Services.WebService
    {

        [WebMethod]
        public string NewMessage(string message)
        {
            string sel = "INSERT INTO Type_Message (Time, Message, UserId) "
                        + "OUTPUT Inserted.ID "
                        + "VALUES ( "
                        + "'00:00:00',"
                        + "'" + message + "'," 
                        + 1 +
                        ")";

            string result = "";

            using (SqlConnection con = new SqlConnection(cnc()))
            {
                using (SqlCommand cmd = new SqlCommand(sel, con))
                {
                    con.Open();

                    int rows_affected = cmd.ExecuteNonQuery();

                    if (rows_affected > 0) result = "true";
                    else result = "false";

                }
            }

            return result;
        }

        [WebMethod ]
        public string GetMessage(int CollectionId)
        {
            string sel = "SELECT * FROM Type_Message "
                        + "WHERE CollectionId='" + CollectionId + "'";

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
        [WebMethod]
        public string DeleteMessage(int message_index)
        {
            string sel = "DELETE FROM Collection "
                        + "WHERE Id='" + message_index + "'";

            string result = "";

            using (SqlConnection con = new SqlConnection(cnc()))
            {
                using (SqlCommand cmd = new SqlCommand(sel, con))
                {
                    con.Open();

                    int rows_affected = cmd.ExecuteNonQuery();

                    if (rows_affected > 0) result = "true";
                    else result = "false";

                }
            }

            return result;
        }


        public static string cnc()
        {
            return ConfigurationManager
                .ConnectionStrings["InstantConnection"]
                .ConnectionString;
        }
    }
}
