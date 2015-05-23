using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace ProjectwerkCursistenportaal.BLL
{
    public class Studiebewijs
    {

        public int Id { get; set; }

        public int Cursist { get; set; }

        public int Studiebewijstype { get; set; }

        public string StudiebewijsNaam { get; set; }
       
    }
}