using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Json;

namespace CVOService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EventSVC" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select EventSVC.svc or EventSVC.svc.cs at the Solution Explorer and start debugging.
    public class EventSVC : EventIF
    {
        List<EventBLL> data;


        /*
            Events gesorteerd op datum dat ze toegevoegd werden  
         */
        public List<String> FilterLatestEvents(int index, int count)
        {            
            return EventDAL.GetEvents();
        }

        /*
             Events gesorteerd op datum
                per maand
                per week
                per dag       
         */

        public List<EventBLL> GetEvents()
        {
            return data;
        }
        public List<String> FilterOrderedEvents(int index, int count)
        {
            return EventDAL.GetEvents();
        }

        /*
            
         */
        public List<String> FilterModuleEvents(int index, int count)
        {
            return EventDAL.GetEvents();
        }

        public bool InsertEvent(EventBLL ex)
        {
            return true;
        }

        public bool DeleteEvent(EventBLL ex)
        {
            return true;
        }

        // Deze moeten nog geintegreerd worden
        class Package
        {

            // "localhost/login/token.php"
            // "localhost/webservice/rest/server.php"

            //String localhost = "localhost";
            //String localhost = "moodle.cvoantwerpen.be";


            String url;
            List<KeyValuePair<String, String>> parameters;

            // Stel Url in
            public Package(String url)
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

        class MoodleDAL
        {
            string localhost = "localhost";
            string token;
            string userid;

            void GetToken()
            {

                Package px =
                   new Package(localhost + "/login/token.php");
                px.P("username", "testuser");
                px.P("password", "Fa_123456");
                px.P("service", "myservice");

                token = (String)JsonParser.FromJson(px.Execute())["token"];
                Console.WriteLine(token);

            }

            void GetUser()
            {
                Package user =
                    new Package(localhost + "/webservice/rest/server.php");
                user.P("wstoken", token);
                user.P("wsfunction", "core_webservice_get_site_info");
                user.P("moodlewsrestformat", "json");

                userid = Convert.ToString(JsonParser.FromJson(user.Execute())["userid"]);
                Console.WriteLine(userid);
            }


            void GetCourses()
            {
                Package course =
                    new Package(localhost + "/webservice/rest/server.php");
                course.P("wstoken", token);
                course.P("wsfunction", "core_enrol_get_users_courses");
                course.P("moodlewsrestformat", "json");
                course.P("userid", userid);

                Console.WriteLine(course.Execute());
            }

            void GetCalendar()
            {
                Package calender =
                    new Package(localhost + "/webservice/rest/server.php");
                calender.P("wstoken", token);
                calender.P("wsfunction", "core_calendar_get_calendar_events");
                calender.P("moodlewsrestformat", "json");
                calender.P("events[courseids][0]", "2");
                //calender.P("options[userevents]", "1");
                //calender.P("options[userevents]", "1");
                //calender.P("options[siteevents]", "1");
                Console.WriteLine(calender.Execute());
            }
        }
    
    }

}
