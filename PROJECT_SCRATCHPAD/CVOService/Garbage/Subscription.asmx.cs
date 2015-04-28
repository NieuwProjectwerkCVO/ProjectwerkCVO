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
    /// Summary description for Subscription
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Subscription : System.Web.Services.WebService
    {

        [WebMethod]
        public string RequestAccess()
        {
            return "Hello World";
        }

        [WebMethod]
        public string RelinquishAccess()
        {
            return "Hello World";
        }

        public string RevokeAccess()
        {
            return "";
        }

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
