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

        [WebMethod]
        public void test_functie()
        {
            DBMDataContext db = new DBMDataContext();

           
            var data = new List<Test> {
                new Test { Id=1, Naam="Irrelevant"}
            };
            export(data);
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
