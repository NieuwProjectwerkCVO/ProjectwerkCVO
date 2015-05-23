using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectwerkCursistenportaal.BLL
{
    public class StudiebewijsGraadType
    {
        public int Id { get; set; }
        public string Omschrijving { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
    }
}