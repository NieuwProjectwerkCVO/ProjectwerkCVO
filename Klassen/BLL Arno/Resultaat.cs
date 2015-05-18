using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace resultatenTonen.BLL
{
    public class Resultaat
    {
        public int Id { get; set; }
        public int IdCursist { get; set; }
        public string Module { get; set; }
        public double Behaald { get; set; }
    }
}