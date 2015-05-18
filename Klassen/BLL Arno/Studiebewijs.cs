using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace resultatenTonen.BLL
{
    public class Studiebewijs
    {
        public int Id { get; set; }
        public int IdModuleVariant { get; set; }
        public string ModuleNaam { get; set; }
        public double Percentage { get; set; }
    }
}