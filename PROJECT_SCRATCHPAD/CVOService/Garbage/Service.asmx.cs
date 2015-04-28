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
    /// Summary description for Service
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service : System.Web.Services.WebService
    {

       
        [WebMethod]
        public string NewCollection(string alias)
        {

            // regex check

            string sel = "INSERT INTO Collection (Alias, TypeComputeData) "
                         + "VALUES ('" + alias + "'," + "0)";

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

        [WebMethod]
        public string DeleteCollection(int index)
        {
            string sel = "DELETE FROM Collection "
                        + "WHERE CollectionId='" + index + "'";

            // delete all elements as well

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

        [WebMethod]
        public List<object> SelectElements(int collection_index)
        {
            string sel = "SELECT * FROM Element "
                         + "WHERE CollectionId='" + collection_index + "' ";

            List<object> result = new List<object>();
            
            using (SqlConnection con = new SqlConnection(cnc()))
            {
                using (SqlCommand cmd = new SqlCommand(sel, con))
                {
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        result.Add(dr["CollectionId"].ToString());
                        result.Add(dr["ElementId"].ToString());
                        result.Add(dr["DataType"].ToString());
                        result.Add(dr["Index"].ToString());
                    }
                    dr.Close();
                }
            }

            return result;
        }

        [WebMethod]
        public string NewElement(int collection_index, int datatype)
        {
            string sel = "INSERT INTO Collection (Alias, TypeComputeData) "
                        + "VALUES ('" + datatype + "'," + "0)";

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

        [WebMethod]
        public string DeleteElement(int element_index)
        {
            string sel = "INSERT INTO Collection (Alias, TypeComputeData) "
                        + "VALUES ('" + element_index + "'," + "0)";

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



        

        [WebMethod]
        public string NewUser()
        {
            return "";
        }

        [WebMethod]
        public string DeleteUser()
        {
            return "";
        }

        [WebMethod]
        public string RequestAccessTo(string token, string collection)
        {
            return "";
        }

        [WebMethod]
        public string RelinquishAccessTo(string token, string collection)
        {
            return "token";
        }

        [WebMethod]
        public string Login(string username, string password)
        {
            return "token";
        }

        [WebMethod]
        public string Logout(string token)
        {
            return "token";
        }

        [WebMethod]
        public string SeverAccess(string token, string collection)
        {
            return "";
        }

        [WebMethod]
        public string RevokeAccess()
        {
            return "";
        }

        [WebMethod]
        public string GrantAccess()
        {
            return "";
        }


        public static string cnc()
        {
            return ConfigurationManager
                .ConnectionStrings["InstantConnection"]
                .ConnectionString;
        }
    }
}
