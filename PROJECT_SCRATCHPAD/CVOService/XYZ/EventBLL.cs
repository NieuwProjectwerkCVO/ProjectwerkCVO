using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CVOService
{
    [DataContract]
    public class EventBLL
    {
        [DataMember]
        public int AccessId  { get; set; }
        /*
         AccessId kan exclusief zijn voor een enkele gebruiker of meerdere
         */
        [DataMember]
        public int EventId  { get; set; }
        [DataMember]
        public string Name  { get; set; }
        [DataMember]
        public string Description  { get; set; }
        [DataMember]
        public DateTime T1  { get; set; }
        [DataMember]
        public DateTime T2  { get; set; }
        [DataMember]
        public _Participation Participation  { get; set; }
        [DataMember]
        public _Status Status  { get; set; }
        [DataMember]
        public _Location Location  { get; set; }
        [DataMember]
        public float Price  { get; set; }
        /*  
         *  Price  < 0      Non_Applicable 
         *  Price == 0      Free
         *  Price  > 0      Paid
         */
        [DataMember]
        public int Capacity  { get; set; }

        /*  
         *  Capacity  < 0   Non_Applicable vb Vakantie
         *  Capacity  > 0   Limiet
         */
        [DataMember]
        public int Count  { get; set; }


        // er moeten enkele nog toegevoegd worden
    }


    public enum _Location
    {
        Non_Applicable,
        Applicable
    }

    public enum _Participation
    {
        Mandatory,
        Optional,
        Conditional,
        Non_Applicable // vb. vakantie
    }

    public enum _Status
    {
        Non_Applicable,
        Cancelled,
        Delayed,
        Rescheduled,
        As_Planned
    }
    class Calender
    {
        int CursistId;
        int EventId;
    }


    class Notificatie
    {
        int EventId;
        int NotificationId;
        string Description;
    }

    class Location
    {
        int EventId;
        int LocationId;
        string Description;
    }
}