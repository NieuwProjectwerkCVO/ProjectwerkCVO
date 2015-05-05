using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ProjectWerk.App_code.DAL
{
    public class Cursist
    {

        //heb hier enkel de belangrijkste toegevoegd die ik nodig heb, 
        //je kan altijd aanvullen
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Id { get; set; }
        public int CursistNr { get; set; }

        public Cursist()
        {

        }

        public List<Cursist> ReadList(SqlCommand command)
        {
            List<Cursist> Cursisten = new List<Cursist>();
            using (SqlDataReader dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    Cursist cursist = new Cursist();
                    cursist = ReadData(cursist, dr);
                    Cursisten.Add(cursist);
                }
            }
            return Cursisten;
        }


        public Cursist ReadData(Cursist cursist, SqlDataReader dr)
        {
            // hier komt de data van de databank
            cursist.Id = Convert.ToInt32(dr["Id"]);
            cursist.Firstname = dr["Voornaam"].ToString();
            cursist.Lastname = dr["Familienaam"].ToString();
            cursist.CursistNr = Convert.ToInt32(dr["cursistNummer"]);
            return cursist;
        }



    }
}