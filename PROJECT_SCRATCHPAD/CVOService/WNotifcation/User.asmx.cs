using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;

namespace CVOService
{


    public class ErrorMessage
    {
        public bool flag;
        public List<string> messages;
    }

    public class UserDL
    {
        public int id;
        public string username;
        public string password;
        public string Voornaam;
        public string Familienaam;
    }


    /// <summary>
    /// Summary description for User
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class User : System.Web.Services.WebService
    {

        

        [WebMethod]
        public ErrorMessage RegisterUser(
            string username, 
            string password
            // string password_Repeat,
            // 
        )
        {
            
            UserDL ux = new UserDL();
                ux.id = -1;
            ErrorMessage EM = new ErrorMessage();
                EM.flag = false;
                EM.messages = new List<string>();

            if (  username != null && !Regex.Match(username, @"[A-Za-z0-9]{1,32}").Success){
                EM.flag = true;
                EM.messages.Add("Invalid username.");
            }

           
            if (!Regex.Match(password, @"[A-Za-z0-9]{1,16}").Success)
            {
                EM.flag = true;
                EM.messages.Add("Invalid password.");
            }

            
            if (EM.flag != true) {

                string command =
                    "IF NOT EXISTS("
                    + " SELECT * FROM [User]"
                    + " WHERE username='" + username + "')"
                    + " INSERT INTO [User] (username, password)"
                    + " VALUES ('" + username + "','" + password + "')";
            

                using (SqlConnection con = new SqlConnection(cnc()))
                {
                    using (SqlCommand cmd = new SqlCommand(command, con))
                    {
                        con.Open();

                        int rows_affected = cmd.ExecuteNonQuery();

                        if (rows_affected < 1) 
                        {
                            EM.flag = true;
                            EM.messages.Add("username already exists.");
                        }
                        else
                        {
                            EM.messages.Add("Successfully added new username.");
                        }
                    }
                }        
            }

            return EM;
        }

        [WebMethod]
        public ErrorMessage LoginUser(
            string username, 
            string password
        )
        {
            UserDL ux = new UserDL();
                ux.id = 0;
            ErrorMessage EM = new ErrorMessage();
                EM.flag = false;
                EM.messages = new List<string>();

            string command =
                  " SELECT * FROM [User]"
                + " WHERE username='" + username + "'";

                using (SqlConnection con = new SqlConnection(cnc()))
                {
                    using (SqlCommand cmd = new SqlCommand(command, con))
                    {
                        con.Open();

                        SqlDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            ux.id = Convert.ToInt32(dr["Id"]);
                            ux.username = dr["Username"].ToString();
                            ux.password = dr["Password"].ToString();
                        }
                        dr.Close();
                    }
                }

                if (ux.id == 0)
                {
                    EM.flag = true;
                    EM.messages.Add("Username does not exist.");
                }
                else if (password != ux.password)
                {
                    EM.flag = true;
                    EM.messages.Add("Incorrect password.");
                }
                else
                {
                    EM.messages.Add("Welcome, " + username + ".");

                    // Grant token
                }
                
            return EM;
        }

        [WebMethod]
        public string LogoutUser(
            string token
        )
        {
            // Withdraw token
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
