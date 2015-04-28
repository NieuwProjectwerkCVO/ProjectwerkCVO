using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace CVOService.WNotifcation
{
    /// <summary>
    /// Summary description for Notification
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Notification : System.Web.Services.WebService
    {


        class Notification
        {

        }

        [WebMethod]
        public string InsertNotification()
        {

            /*
                Stuur een [notificatie] 
             *          naar List[user] 
             *          voor List[Events]
             */

            return "Hello World";
        }

        [WebMethod]
        public string DeleteNotification()
        {
            return "Hello World";
        }

        [WebMethod]
        public string SelectNotifications()
        {
            return "Hello World";
        }
    }
}
