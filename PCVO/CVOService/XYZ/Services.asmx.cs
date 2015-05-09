using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;
using System.Xml.Serialization;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Json;
using System.Text.RegularExpressions;

namespace CVOService
{
    /// <summary>
    /// Summary description for Services
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Services : System.Web.Services.WebService
    {

        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        /////////////////////// HULP CLASSES ////////////////////////         
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////

        public class compound
        {
            public bool flag;
            public List<string> data;

            public compound()
            {
                data = new List<string>();
            }

            public void Add(string element){
                data.Add(element);
            }

        }

        public class package
        {
            // "moodle-cvomobile.rhcloud.com/token.php"
            // "localhost/login/token.php"
            // "localhost/webservice/rest/server.php"

            //String localhost = "localhost";
            //String localhost = "moodle.cvoantwerpen.be";


            String url;
            List<KeyValuePair<String, String>> parameters;

            // Stel Url in
            public package(String url)
            {
                this.url = url;
                parameters = new List<KeyValuePair<String, String>>();
            }

            // Voeg Input Parameters toe
            public void P(String key, String value)
            {
                parameters.Add(new KeyValuePair<string, string>(key, value));
            }


            // Voer [Web Service Request] uit
            public String Execute()
            {
                String output = "";
                String param_string = "";

                foreach (KeyValuePair<String, String> dx in parameters)
                    param_string += dx.Key + "=" + dx.Value + "&";

                byte[] buffer = Encoding.ASCII.GetBytes(param_string);
                String lex = "http://" + url + "?" + param_string;

                //Console.WriteLine(lex);

                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(lex);
                WebReq.Method = WebRequestMethods.Http.Post;
                WebReq.ContentType = "application/x-www-form-urlencoded";

                WebReq.ContentLength = buffer.Length;
                using (Stream PostData = WebReq.GetRequestStream())
                    PostData.Write(buffer, 0, buffer.Length);




                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                using (StreamReader reader = new StreamReader(WebResp.GetResponseStream()))
                    output = reader.ReadToEnd();

                //Console.WriteLine(output);

                return output;

            }
        }


        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        ///////////////// GENERIEKE DAL FUNCTIES ////////////////////         
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////

        public List<compound> SFX(string command){

            List<compound> set = new List<compound>();

            using (SqlConnection con = new SqlConnection(cnc()))
            {
                using (SqlCommand cmd = new SqlCommand(command, con))
                {
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        compound ex = new compound();

                        for (int i = 0; i < dr.FieldCount - 1; i++)
                            ex.Add(dr[dr.GetName(i).ToString()].ToString());

                        set.Add(ex);
                    }

                    dr.Close();
                }
            }

            return set;
        }

        public bool IFX(string command)
        {
            using (SqlConnection con = new SqlConnection(cnc()))
            {
                using (SqlCommand cmd = new SqlCommand(command, con))
                {
                    con.Open();

                    int rows_affected = cmd.ExecuteNonQuery();

                    if (rows_affected < 1) return false;
                    else return true;
                }
            }
        }


        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        ////////////////////// MOODLE FUNCTIES //////////////////////         
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////        

        [WebMethod]
        public string moodle_token(string username, string password)
        {
            string localhost = "moodle.cvoantwerpen.be";

            package
                ex = new package( localhost + "/login/token.php");
                ex.P("username", username);
                ex.P("password", password);
                ex.P("service", "moodle_mobile_app");

            // hier moet nog wat error handling code

            return (string)JsonParser.FromJson(ex.Execute())["token"];

        }

        [WebMethod]
        public string moodule_user_id(string token)
        {
            string localhost = "moodle.cvoantwerpen.be";

            package user =
                new package(localhost + "/webservice/rest/server.php");
            user.P("wstoken", token);
            user.P("wsfunction", "core_webservice_get_site_info");
            user.P("moodlewsrestformat", "json");

            // hier moet nog wat error handling code

            return (string)(JsonParser.FromJson(user.Execute())["userid"]);
            
        }

        [WebMethod]
        public string moodle_user_modules(string token, string user_id)
        {
            string localhost = "moodle.cvoantwerpen.be";

            package course =
                    new package(localhost + "/webservice/rest/server.php");
            course.P("wstoken", token);
            course.P("wsfunction", "core_enrol_get_users_courses");
            course.P("moodlewsrestformat", "xml");
            course.P("userid", user_id);

            return course.Execute(); 
        }

        [WebMethod]
        public string moodle_module_events(string token, string module_id)
        {
            string localhost = "moodle.cvoantwerpen.be";

            package calender =
                new package(localhost + "/webservice/rest/server.php");
            calender.P("wstoken", token);
            calender.P("wsfunction", "core_calendar_get_calendar_events");
            calender.P("moodlewsrestformat", "json");
            calender.P("events[courseids][0]", module_id);

            return calender.Execute();
        }


        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        ///////////////////// CURSIST  FUNCTIES /////////////////////         
        /////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////// 

        [WebMethod]
        public compound registeer_user(string username, string password)
        {

            compound ux = new compound();

            if (username != null && !Regex.Match(username, @"[A-Za-z0-9]{1,32}").Success)
            {
                ux.flag = true;
                ux.Add("Invalid username.");
            }


            if (!Regex.Match(password, @"[A-Za-z0-9]{1,16}").Success)
            {
                ux.flag = true;
                ux.Add("Invalid password.");
            }


            if (ux.flag != true)
            {
                ux.flag = IFX("IF NOT EXISTS("
                    + " SELECT * FROM [User]"
                    + " WHERE username='" + username + "')"
                    + " INSERT INTO [User] (username, password)"
                    + " VALUES ('" + username + "','" + password + "')"
                );

            }

            return ux;
        }

        [WebMethod]
        public compound login_user(string username, string password)
        {
            /*UserDL ux = new UserDL();
            ux.id = 0;
            ErrorMessage EM = new ErrorMessage();
            EM.flag = false;
            EM.messages = new List<string>();
            */
            compound ux = new compound();

            string command =
                  " SELECT * FROM [User]"
                + " WHERE username='" + username + "'";

            /*using (SqlConnection con = new SqlConnection(cnc()))
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
                ux.flag = true;
                ux.Add("Username does not exist.");
            }
            else if (password != ux.password)
            {
                ux.flag = true;
                ux.Add("Incorrect password.");
            }
            else
            {
                ux.Add("Welcome, " + username + ".");

                // Grant token
            }*/

            return ux;
        }

        [WebMethod]
        public compound logout_user(string token)
        {
            // 
            return new compound();
        }

        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        ///////////////////// EVENT DAL FUNCTIES ////////////////////         
        /////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////// 

        [WebMethod]
        public List<compound> verlof_events(string t1, string t2)
        {

            // regex saneren t1, t2

            return SFX(
                        " SELECT * FROM [Kalender]"
                      + " WHERE Datum >= '1/5/2015'"
                   );
        }

        [WebMethod]
        public List<compound> module_events(string t1, string t2)
        {
            // regex saneren t1, t2

            return SFX(
                        " SELECT CursusNummer, Id, Naam, AanvangsDatum, EindDatum, PlaatsenVoorDerden, MaximumCapaciteit, EindDatum FROM [IngerichteModulevariant] "
                      + " WHERE AanvangsDatum >= '1/1/2015'");
        }

        [WebMethod] 
        public List<compound> module_events_filter_opleiding(int opleidingvariant)
        {
            return SFX(
                        " SELECT CursusNummer, Id, Naam, AanvangsDatum, EindDatum, PlaatsenVoorDerden, MaximumCapaciteit, EindDatum FROM [IngerichteModulevariant] "
                      + " WHERE IdOpleidingsVariant='" + opleidingvariant + "'");
        }

        [WebMethod]
        public List<compound> module_events_filter_cursist(int cursist_id)
        {
            return
                SFX(" SELECT IdCursist, IdIngerichteModulevariant FROM [Plaatsing] "
                  + " WHERE IdCursist='" + cursist_id + "'"
                );
        }

        [WebMethod]
        public List<compound> les_events(string module_id)
        {
            // voorbeeld moduleid \\ 3514

            return SFX(
                        " SELECT IdIngerichteModulevariant, Aanvangsdatum, Eindatum FROM [LesDavinci]"
                      + " WHERE IdIngerichteModulevariant='" + module_id + "'"
                       );
        }

        [WebMethod]
        public List<compound> opleidingen()
        {
            return SFX(" SELECT * FROM [Opleidingsvariant] ");
        }


        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        ////////////////////// EVENT FUNCTIES ///////////////////////         
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////

        // module 
        // traject begeleider afspraak
        // notificaties
        // globaal nieuws

        [WebMethod]
        public bool inschrijving_event(int event_id)
        {   
            return  
                IFX(
                "IF ( SELECT Count, Capacity FROM [Event] )"
                + "INSERT INTO [Attendee] ()"
                + "VALUES (" + ")"
                );
        }

        [WebMethod]
        public bool uitschrijving_event(int event_id)
        {
            return
                IFX(
                "IF ( SELECT Count, Capacity FROM [Event] )"
                + "DELETE FROM [Attendee] ()"
                + "VALUES (" + ")"
                );
        }

        [WebMethod]
        public bool new_event(int timeline_id, int event_type, string[] data)
        {
            return
                IFX("INSERT INTO [Attendee] ()"
                + "VALUES (" + ")");
        }

        [WebMethod]
        public bool delete_event(int timeline_id, int event_id)
        {
            return
                IFX("DELETE FROM [Attendee] ()"
                + "VALUES (" + ")");
        }

        
        public static string cnc()
        {
            return ConfigurationManager
                .ConnectionStrings["DavinciDB"]
                .ConnectionString;
        }
    }
}
