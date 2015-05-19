using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Loket.BLL
{
    public class Document
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public int TypeId { get; set; }
        public int OpleidingVariantId { get; set; }
        public int OpleidingId { get; set; }
        public string Specificatie { get; set; }


    }
}