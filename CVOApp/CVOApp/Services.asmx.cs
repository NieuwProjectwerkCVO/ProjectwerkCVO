using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Services;
using System.Configuration;
using System.Xml.Serialization;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;
using System.Web.Script.Services;
using System.Web.Script.Serialization;

namespace CVOApp
{
    /// <summary>
    /// Summary description for User
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Services : System.Web.Services.WebService
    {

        public void export(Object obj)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            Context.Response.Write(js.Serialize(obj));
        }
    
        class Test { public int Id; public string Naam; }

        class atom { public int type; public string data; }

        // Zoekfuncties
        class node
        {
            public int distance;
            public string data;
        }

        static int edit_distance (string lexA, string lexB) {

            // degenerate cases
            if ( lexA.CompareTo(lexB) == 0 )  return 0;
            if ( lexA.Length == 0 || lexB.Length == 0 ) return lexB.Length;

            // 
            int[] v0 = new int[lexB.Length + 1];
            int[] v1 = new int[lexB.Length + 1];

            for (int i = 0; i < v0.Length; i++) v0[i] = i;

            // bereken afstand
            for (int i = 0; i < lexA.Length; i++){
                v1[0] = i + 1;
                for (int j = 0; j < lexB.Length; j++)
                {
                    int cost = ( lexA[i] == lexB[j] ) ? 0 : 1; 

                    bool cDELETE     = (v1[j] + 1) <= (v0[j + 1] + 1) && (v1[j] + 1) <= (v0[j] + cost);
                    bool cINSERT     = (v0[j + 1] + 1) <= (v1[j] + 1) && (v0[j + 1] + 1) <= (v0[j] + cost);
                    bool cSUBSTITUTE = (cDELETE != true && cINSERT != true);

                    if ( cDELETE     ) v1[j + 1] = v1[j] + 1;
                    if ( cINSERT     ) v1[j + 1] = v0[j + 1] + 0;
                    if ( cSUBSTITUTE ) v1[j + 1] = v0[j] + cost;
                }

               Array.Copy(v1, 0, v0, 0, v0.Length);
            }

            return v1[lexB.Length];
        }

        static List<node> fuzzy(string query, List<string> Base)
        {
            // SQL query
            List<node> set = new List<node>();

            // Bereken Edit Distance
            for (int dx = 0; dx < Base.Count; dx++)
            {
                node nx = new node();
                nx.distance = Base[dx].Length - edit_distance(query, Base[dx]);
                nx.data = Base[dx];
                set.Add(nx);
            }

            // Sorteer Elementen
            for (int dx = 0; dx < set.Count - 1; )
            {
                if (set[dx].distance < set[dx + 1].distance)
                {
                    node tmp = set[dx];
                    set[dx] = set[dx + 1];
                    set[dx + 1] = tmp;

                    if (dx != 0) dx--;
                    else dx++;
                }
                else dx++;
            }

            // Houdt geen rekening met of de letters naast elkaar staan


            return set;
        }

        [WebMethod]
        public void test_opleidingen(string query, uint index)
        {
            DBMDataContext db = new DBMDataContext();
            List<node> set = new List<node>();
            
            // SQL query
            List<string> Base = (from opl in db.Opleidings
                                 select opl.Naam)
                                 .ToList<string>();
            
            int lx = 12;
            export(fuzzy(query,Base).GetRange(Convert.ToInt32(((index-1)*lx)), lx));
        }

        [WebMethod]
        public void test_modules(string query, uint index)
        {
            DBMDataContext db = new DBMDataContext();
            List<node> set = new List<node>();

            // SQL query
            List<string> Base = (from mdl in db.Modules
                                 select mdl.Naam)
                                 .ToList<string>();

            int lx = 12;
            export(fuzzy(query, Base).GetRange(Convert.ToInt32(((index - 1) * lx)), lx));
        }

        [WebMethod]
        public void test_events(string query, uint index)
        {
            DBMDataContext db = new DBMDataContext();
            List<node> set = new List<node>();


        }

        // PLAATSEN | Campussen ///////////////////////////////////////////////
        [WebMethod]
        public void campussen()
        {
            DBMDataContext db = new DBMDataContext();

            var query = from centrum in db.Centrums
                        join vestiging in db.Vestigingsplaats
                            on centrum.Id equals vestiging.IdCentrum
                        where vestiging.Geografie == "Antwerpen"
                        select new { 
                            centrum.OfficieleNaam, 
                            vestiging.Geografie 
                        };

            export(query);
        }

        // PERSONEN | Cursisten - Trajectbegeleiders - Lesgevers - Medewerkers ////
        [WebMethod]
        public void personeel()
        {
            DBMDataContext db = new DBMDataContext();

            var query = from persoon in db.Personeels
                        join functie in db.PersoneelFuncties
                            on persoon.Id equals functie.Id
                        select new
                        {
                            persoon.Naam,
                            persoon.Functie,
                            functie.Beschrijving
                        };

            export(query);
        }

        [WebMethod]
        public void cursisten()
        {
            DBMDataContext db = new DBMDataContext();

            var query = from persoon in db.Cursists
                        select new
                        {   persoon.Id,
                            persoon.CursistNummer,
                            persoon.Voornaam,
                            persoon.Familienaam
                        };

            export(query);
        }

        //  GENERIEK OPLEIDING + MODULES ////////////////////////////////////////////
        
        [WebMethod]
        public void all_opleidingen()
        {
            DBMDataContext db = new DBMDataContext();

            var query = from c in db.Opleidingsvariants
                        select new {  
                            c.Id,
                            c.Naam
                        };

            export(query);
        }

        [WebMethod]
        public void all_traject(int id_opleiding)
        {
            DBMDataContext db = new DBMDataContext();

            var query = from c in db.Opleidingsvariants
                        join cnc in db.OpleidingsvariantModulevariants
                            on c.Id equals cnc.IdOpleidingsvariant
                        join m in db.Modulevariants
                            on cnc.IdModulevariant equals m.Id
                        where c.Id == id_opleiding
                        select new
                        {
                            ModuleId = m.Id,
                            Modulenaam = m.Naam
                        };

            export(query);
        }

        [WebMethod]
        public void all_modules()
        {
            DBMDataContext db = new DBMDataContext();

            var query = from c in db.Opleidingsvariants
                        join cnc in db.OpleidingsvariantModulevariants
                            on c.Id equals cnc.IdOpleidingsvariant
                        join m in db.Modulevariants
                            on cnc.IdModulevariant equals m.Id
                        select new
                        {
                            Opleidingnaam = c.Naam,
                            Modulenaam = m.Naam
                        };

            export(query);
        }

        //  SPECIFIEK :: OPLEIDING | MODULES ////////////////////////////////////////////
        [WebMethod]
        public void all_ingerichte_modules()
        {
            DBMDataContext db = new DBMDataContext();

            var query = from c in db.Opleidingsvariants
                        join cnc in db.OpleidingsvariantModulevariants
                            on c.Id equals cnc.IdOpleidingsvariant
                        join m in db.Modulevariants
                            on cnc.IdModulevariant equals m.Id
                        join im in db.IngerichteModulevariants
                            on m.Id equals im.IdModuleVariant
                        select new
                        {
                            Opleiding = c.Naam,
                            im.CursusNummer,
                            im.Id,
                            Modulenaam = m.Naam,
                            im.AanvangsDatum,
                            im.EindDatum
                        };

            export(query);
        }

        [WebMethod]
        public void all_ingerichte_modules_filter_opleiding(int id_opleiding)
        {
            DBMDataContext db = new DBMDataContext();

            var query = from c in db.Opleidingsvariants
                        join cnc in db.OpleidingsvariantModulevariants
                            on c.Id equals cnc.IdOpleidingsvariant
                        join m in db.Modulevariants
                            on cnc.IdModulevariant equals m.Id
                        join im in db.IngerichteModulevariants
                            on m.Id equals im.IdModuleVariant
                        select new
                        {
                            Opleiding = c.Naam,
                            im.CursusNummer,
                            im.Id,
                            Modulenaam = m.Naam
                        };

            export(query);
        }


        // CURSIST ACTIONS //////////////////////////////////////////////

        [WebMethod]
        public void cursist_registreer(string voornaam, string familienaam)
        {
            export("Success, " + voornaam + "!");
        }

        [WebMethod]
        public void cursist_login(string cursistnummer, string wachtwoord)
        {
            export("Success, " + cursistnummer + "!");
        }

        [WebMethod]
        public void cursist_inschrijven_module(int id_cursist, int id_modulevariant)
        {
            DBMDataContext db = new DBMDataContext();

            // export(query);
        }

        [WebMethod]
        public void cursist_stopzetten_module(int id_cursist, int id_modulevariant)
        {
            DBMDataContext db = new DBMDataContext();

            // export(query);
        }

        [WebMethod]
        public void cursist_afspraak_plannen(int id_cursist, int id_modulevariant)
        {
            DBMDataContext db = new DBMDataContext();

            // export(query);
        }

        [WebMethod]
        public void cursist_afspraak_cancellen(int id_cursist, int id_modulevariant)
        {
            DBMDataContext db = new DBMDataContext();

            // export(query);
        }


        // Cursist Data | Gegevens - Resultaten - Modules 

        [WebMethod]
        public void cursist_gegevens(int id_cursist)
        {
            DBMDataContext db = new DBMDataContext();

            var query = from cursist in db.Cursists
                        where cursist.Id == id_cursist
                        select new
                        {
                            cursist.CursistNummer,
                            cursist.Voornaam,
                            cursist.Familienaam
                        };

            export(query);
        }

        [WebMethod]
        public void cursist_modules(string cursistnummer)
        {
            DBMDataContext db = new DBMDataContext();

            var query = from cursist in db.Cursists
                        join mdl in db.Plaatsings
                            on cursist.Id equals mdl.IdCursist
                        join m in db.IngerichteModulevariants
                            on mdl.IdIngerichteModulevariant equals m.Id
                        where cursist.CursistNummer == cursistnummer
                        select new
                        {
                            
                            m.CursusNummer,
                            m.Id,
                            m.Naam
                        };

            export(query);
        }

        [WebMethod]
        public void cursist_resultaten(int id_cursist)
        {
            DBMDataContext db = new DBMDataContext();

            var query = from rs in db.Studiebewijs
                        join mdl in db.Modulevariants
                            on rs.IdModuleVariant equals mdl.Id
                        //where rs.IdCursist == id_cursist
                        select new
                        {
                            mdl.Naam,
                            rs.Percentage
                        };

            export(query);
        }


        // EVENTS //////////////////////////////////////////////////////
        // Specifiek | Lesmomenten - Deadlines - Examen - Herexamen - Deliberatie

        [WebMethod]
        public void cursist_lesmomenten(int cursist_nummer)
        {
            DBMDataContext db = new DBMDataContext();

            var query = from c in db.LesDavincis
                        where c.IdIngerichteModulevariant == cursist_nummer
                        select new
                        {
                            c.Afgelast,
                            c.Aanvangsdatum,
                            c.Einddatum
                        };

            export(query);
        }

        [WebMethod]
        public void all_lesmomenten(int id_modulevariant)
        {
            DBMDataContext db = new DBMDataContext();

            var query = from c in db.LesDavincis
                        where c.IdIngerichteModulevariant == id_modulevariant
                        select new
                        {
                            c.Afgelast,
                            c.Aanvangsdatum,
                            c.Einddatum
                        };

            export(query);
        }

        [WebMethod]
        public void all_deadlines(int id_modulevariant)
        {
            DBMDataContext db = new DBMDataContext();

            var query = from c in  db.LesDavincis // db.Deadlines
                        select new
                        {
                            c.Afgelast,
                            c.Aanvangsdatum,
                            c.Einddatum
                        };

            export(query);
        }

        [WebMethod]
        public void deliberatie_moment(int id_modulevariant)
        {
            DBMDataContext db = new DBMDataContext();

            var query = from c in db.IngerichteModulevariants
                        where c.Id == id_modulevariant
                        select new
                        {
                            c.DeliberatieDatum
                        };

            export(query);
        }

        [WebMethod]
        public void herexamen_moment(int id_modulevariant)
        {
            DBMDataContext db = new DBMDataContext();

            var query = from c in db.IngerichteModulevariants
                        where c.Id == id_modulevariant
                        select new
                        {
                            c.DatumTweedeZit
                        };

            export(query);
        }

        // Global | Feestdagen
        [WebMethod]
        public void all_feestdagen()
        {
            DBMDataContext db = new DBMDataContext();

            var query = from c in db.Kalenders
                        select new
                        {
                            c.Datum,
                            c.Omschrijving
                        };

            export(query);
        }

        [WebMethod]
        public void all_afspraken()
        {
            DBMDataContext db = new DBMDataContext();

            var query = from c in db.Kalenders
                        select new
                        {
                            c.Datum,
                            c.Omschrijving
                        };

            export(query);
        }

        // NOTIFICATIES //////////////////////////////////////////////////
        [WebMethod]
        public void all_notificatie()
        {
            DBMDataContext db = new DBMDataContext();

            var query = from c in db.Kalenders
                        select new
                        {
                            c.Datum,
                            c.Omschrijving
                        };

            export(query);
        }

    }
}
