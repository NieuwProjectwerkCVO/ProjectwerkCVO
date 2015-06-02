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
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
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

        class validator
        {
            public int id_cursist;
            public string cursistnummer;
            public string wachtwoord;
            public bool is_valid;

            public validator(string access_token)
            {
                DBMDataContext db = new DBMDataContext();

                if (access_token.Length > 4) { 
                    cursistnummer = access_token.Substring(0,5);
                    wachtwoord = access_token.Substring(5);
                    is_valid = true;
                }
                // cursistnummer ophalen
                var query = from cs in db.Cursists
                            where cs.CursistNummer == cursistnummer
                            select new
                            {
                                cs.Id,
                                cs.CursistNummer,
                                cs.Wachtwoord
                            };


                // bestaat cursistnummer?
                int count = 0;
                foreach (var c in query) count++;


                if (count == 0) is_valid = false;
                
                else
                {
                    // correcte wachtwoord
                    bool test = false;
                    foreach (var cs in query)
                    {
                        if (cs.Wachtwoord == wachtwoord) id_cursist = cs.Id;
                        else is_valid = false;
                    }
                    
                }
            }
        }
        
       
    
        class Test { public int Id; public string Naam; }

   
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
        public void search_engine(string query, int lvl)
        {
            /*
                :type
             *      opleiding
             *      module
             *      event
             *      campus
             *      personeel
             *      cursist
             */

            var Directory = new[]{ 
                new { lvl = 0, slvl = 1, type = "link", label="login" },
                new { lvl = 0, slvl = 2, type = "link", label="registreer" },
                new { lvl = 0, slvl = 3, type = "link", label="opleidingen" },
                new { lvl = 0, slvl = 4, type = "link", label="modules" },
                new { lvl = 0, slvl = 5, type = "link", label="campussen" },
                new { lvl = 0, slvl = 6, type = "link", label="contact" },

                new { lvl = 1, slvl = 1, type = "login", label="login" },
                new { lvl = 2, slvl = 1, type = "registreer", label = "registreer" },
                new { lvl = 3, slvl = 1, type = "opleidingen", label = "registreer" },
                new { lvl = 4, slvl = 1, type = "modules", label = "registreer" },
                new { lvl = 5, slvl = 1, type = "campussen", label = "registreer" },
                new { lvl = 6, slvl = 1, type = "contact", label = "registreer" }
            };


            rss_fetch("http://podcasts.engadget.com/rss.xml");
            export(Directory.Where(x => x.lvl == lvl));
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
            
            int lx = 10;
            export(fuzzy(query,Base).GetRange(Convert.ToInt32(((index-1)*lx)), lx));
        }

        [WebMethod]
        public void test_modules(string query, uint index )
        {
            DBMDataContext db = new DBMDataContext();
            List<node> set = new List<node>();

            // SQL query
            List<string> Base = (from mdl in db.Modules
                                 select mdl.Naam)
                                 .ToList<string>();

            int lx = 10;
            export(fuzzy(query, Base).GetRange(Convert.ToInt32(((index - 1) * lx)), lx));
        }

        [WebMethod]
        public void test_events(string query, uint index)
        {
            DBMDataContext db = new DBMDataContext();
            List<node> set = new List<node>();

        }

        ///////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////
        /// GENERIEKE DATA | Gegevens - Resultaten - Modules  - Events
        ///////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////
      
        [WebMethod]
        public void select_campussen()
        {
            DBMDataContext db = new DBMDataContext();

            var Campus = new[] {
                new {
                    Naam = "Campus Hoboken (Hoofdzetel)",
                    Straat = "Distelvinklaan 22",
                    Postcode = "2660 Hoboken",
                    Telefoon = "03 830 41 05",
                    Fax = "03 830 33 79",
                    Openingsuren = "Maandag t.e.m. donderdag: 09 - 13 u., 13.30 - 17.30 u. en 18  - 21 u. Vrijdag: 09 - 13 u. "
                },
                new
                {
                    Naam = "Campus Breughel",
                    Straat = "Breughelstraat 35",
                    Postcode = "2018 Antwerpen",
                    Telefoon = "03 230 10 21",
                    Fax = "03 830 33 79",
                    Openingsuren = "Maandag t.e.m. vrijdag: 09 - 13 u. en 13.30 - 15 u."
                },
                new
                {
                    Naam = "Campus Craeybeckx",
                    Straat = "Frank Craeybeckxlaan 22",
                    Postcode = "2100 Deurne",
                    Telefoon = "03 360 80 40",
                    Fax = "03 830 33 79",
                    Openingsuren = "Maandag t.e.m. donderdag: 09 - 13 u. , 13.30 - 17.30 u. en 18 - 21 u. Vrijdag: 09 - 13 u."
                },
                new
                {
                    Naam = "Campus Roosevelt",
                    Straat = "Frank Rooseveltplaats 11",
                    Postcode = "2060 Antwerpen",
                    Telefoon = "0493 598 175",
                    Fax = "03 830 33 79",
                    Openingsuren = "Maandag t.e.m. donderdag: 18.30 - 21 u."
                },
                new
                {
                    Naam = "Campus Ruggeveld",
                    Straat = "Ruggeveldlaan 496",
                    Postcode = "2100 Deurne",
                    Telefoon = "03 328 05 30",
                    Fax = "03 830 33 79",
                    Openingsuren = "Maandag t.e.m. donderdag: 17.30 - 21 u."
                }
            };

            export(Campus);
        }

        [WebMethod]
        public void select_personeel()
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
        public void select_cursisten()
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

        // OPLEIDINGEN + MODULES ////////////////////////////////////////////
        
        [WebMethod]
        public void select_opleidingen()
        {
            DBMDataContext db = new DBMDataContext();

            // IngerichteOpleidingsvariant is praktisch leeg 
            // Dit is dus een andere manier om alsnog de opleidingen
            // te krijgen die ingericht zijn.
            // Deze lijst komt overeen met de lijst op de website.

            var query = (from c in db.IngerichteModulevariants
                         join opl in db.Opleidingsvariants
                            on c.IdOpleidingsVariant equals opl.Id
                         select new
                         {
                             c.IdOpleidingsVariant,
                             opl.IdOpleiding,
                             opl.Naam
                         })
                         .GroupBy(x => x.IdOpleidingsVariant)
                         .Select(grp => grp.First());

            export(query);
        }
 
        ///////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////
        /// CURSIST DATA | Gegevens - Resultaten - Modules  - Events
        ///////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////

        [WebMethod]
        public void cursist_gegevens(string access_token)
        {
            DBMDataContext db = new DBMDataContext();

            validator vx = new validator(access_token);

            if (vx.is_valid == false)
            {
                export(new { Message = "Incorrecte wachtwoord en/of cursistnummer"});
                return;
            }

            var query = from crs in db.Cursists
                        where crs.CursistNummer == vx.cursistnummer
                        select new
                        {
                            crs.Voornaam,
                            crs.Familienaam,
                            crs.Email,
                            crs.GSM,
                            crs.Straat,
                            crs.Tel1,
                            crs.GeboorteDatum,
                            crs.GeboortePlaats
                        };

            export(query);
  
        }

        
        [WebMethod]
        public void cursist_resultaten(string access_token)
        {
            DBMDataContext db = new DBMDataContext();

            validator vx = new validator(access_token);

            if (vx.is_valid == false) return;

            var query = from crs in db.Cursists
                        join plts in db.Plaatsings
                            on crs.Id equals plts.IdCursist
                        join mdl in db.IngerichteModulevariants
                            on plts.IdIngerichteModulevariant equals mdl.Id
                        join res in db.PlaatsingResultaats
                            on plts.IdPlaatsingResultaat equals res.Id
                        where crs.CursistNummer == vx.cursistnummer
                        select new
                        {
                            mdl.CursusNummer,
                            mdl.Naam,
                            res.PuntenTotaal
                        };

            export(query);
            
        }

        class Event
        {
            // module, les, afspraak, taak, globaal, examen, herexamen, deliberatie
            // feestdagen, ...
            public string type;
            public int index;
            public string source;
            public string description;
            public DateTime t1;
            public DateTime t2;
            public string location;
            public bool cancelled;
            public bool subscribed;
        }

     
        [WebMethod]
        public void imv_data(int id_module)
        {
            DBMDataContext db = new DBMDataContext();

            // lesmomenten
            var query = from mdl in db.IngerichteModulevariants
                        join evt in db.LesDavincis
                            on mdl.Id equals evt.IdIngerichteModulevariant
                        join lkl in db.Lokaals
                            on evt.IdLokaal equals lkl.Id
                        where mdl.IdModuleVariant == id_module
                        orderby evt.Aanvangsdatum
                        select new
                        {
                            mdl.Naam,
                            Lokaal = lkl.Naam,
                            evt.Afgelast,
                            Aanvangsdatum = evt.Aanvangsdatum.ToString(),
                            Einddatum = evt.Einddatum.ToString()
                        };

            // taken

            // notificatie

            // deliberatie moment

            // examen

            // herexamen

            // docent

            // lestijden

            // campus
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////
        ///// CURSIST ////////////////////////////////////////////////////////////////////////

        [WebMethod]
        public void cursist_registreer(
            string Voornaam,
            string Familienaam,
            string Wachtwoord
            /* string Straat,
               string Email,
               string GSM,
             
             */
        )
        {
            DBMDataContext db = new DBMDataContext();

            // Wachtwoord correcte formaat?
            if (Regex.Match("[A-Za-z0-9]{8,}", Wachtwoord).Success)
            {
                export(new { Message = "Incorrecte wachtwoord!" });
                return;
            }

            // Ophalen Cursistnummer
            var query = (from crs in db.Cursists
                         select new { crs.CursistNummer })
                        .OrderByDescending(item => item.CursistNummer).First();

            // Inschrijven
            Cursist cx = new Cursist
            {
                CursistNummer = (Convert.ToInt32(query.CursistNummer) + 1).ToString(),
                Voornaam = Voornaam,
                Familienaam = Familienaam,
                Wachtwoord = Wachtwoord,
                Geslacht = "M",
                Straat = "Straat",
                HuisNr = "1",
                IdPostcode = 2328,
                IdGeboorteLand = 19,
                IdDomicilieLand = 19,
                IdNationaliteitType = 43,

            };

            db.Cursists.InsertOnSubmit(cx);
            db.SubmitChanges();
            export(new { Message = "Success, " + cx.CursistNummer + "!"});
        }

        [WebMethod]
        public void cursist_login(string access_token)
        {
            export(new validator(access_token));
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////
        ///// NOTIFICATIES ///////////////////////////////////////////////////////////////////

        public void cursist_notifications(string access_token, uint start_index, uint count)
        {
            DBMDataContext db = new DBMDataContext();

            validator vx = new validator(access_token);

            if (vx.is_valid == false) return;

            // notificatie
            var notificatie_query = (from crs in db.Cursists
                                     join plts in db.Plaatsings
                                         on crs.Id equals plts.IdCursist
                                     join mdl in db.IngerichteModulevariants
                                         on plts.IdIngerichteModulevariant equals mdl.Id
                                     join ntf in db.Notificaties
                                         on plts.IdIngerichteModulevariant equals ntf.IdIngerichteModulevariant
                                     where crs.CursistNummer == vx.cursistnummer
                                     orderby ntf.Datum
                                     select new Event
                                     {
                                         type = "notificatie",
                                         source = (mdl.CursusNummer + " " + mdl.Naam),
                                         description = ntf.Bericht,
                                         location = "N/A",
                                         cancelled = false,
                                         t1 = Convert.ToDateTime(ntf.Datum),
                                         t2 = Convert.ToDateTime(ntf.Datum)
                                     })
                                     .ToList()
                                     .Skip((int)start_index)
                                     .Take((int)count);

            export(notificatie_query);
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////
        ///// EVENTS /////////////////////////////////////////////////////////////////////////
        
        [WebMethod]
        public void cursist_all_events(string access_token, DateTime t1, DateTime t2)
        {
            DBMDataContext db = new DBMDataContext();

            validator vx = new validator(access_token);

            if (vx.is_valid == false) return;

            // lesmomenten
            var les_query = (from crs in db.Cursists
                             join plts in db.Plaatsings
                                 on crs.Id equals plts.IdCursist
                             join mdl in db.IngerichteModulevariants
                                 on plts.IdIngerichteModulevariant equals mdl.Id
                             join evt in db.LesDavincis
                                 on mdl.Id equals evt.IdIngerichteModulevariant
                             join lkl in db.Lokaals
                                 on evt.IdLokaal equals lkl.Id
                             where crs.CursistNummer == vx.cursistnummer
                             && (evt.Aanvangsdatum > t1 || evt.Aanvangsdatum < t2)
                             && (evt.Einddatum > t1 || evt.Einddatum < t2)
                             select new Event
                             {
                                 type = "lesmoment",
                                 source = (mdl.CursusNummer + " " + mdl.Naam),
                                 index = evt.Id,
                                 description = mdl.Naam,
                                 location = lkl.Naam,
                                 cancelled = evt.Afgelast,
                                 t1 = evt.Aanvangsdatum,
                                 t2 = evt.Einddatum
                             }).ToList();

            // examen
            var examen_query = (from crs in db.Cursists
                                join plts in db.Plaatsings
                                    on crs.Id equals plts.IdCursist
                                join mdl in db.IngerichteModulevariants
                                    on plts.IdIngerichteModulevariant equals mdl.Id
                                where crs.CursistNummer == vx.cursistnummer
                                && (mdl.ExamenDatum > t1 || mdl.ExamenDatum < t2)
                                select new Event
                                {
                                    type = "examen",
                                    source = (mdl.CursusNummer + " " + mdl.Naam),
                                    description = mdl.Naam,
                                    location = mdl.InfoLokaal,
                                    cancelled = false,
                                    t1 = Convert.ToDateTime(mdl.ExamenDatum),
                                    t2 = Convert.ToDateTime(mdl.ExamenDatum)
                                }).ToList();

            // herexamen
            var herexamen_query = (from crs in db.Cursists
                                   join plts in db.Plaatsings
                                       on crs.Id equals plts.IdCursist
                                   join mdl in db.IngerichteModulevariants
                                       on plts.IdIngerichteModulevariant equals mdl.Id
                                   where crs.CursistNummer == vx.cursistnummer
                                   && (mdl.DatumTweedeZit > t1 || mdl.DatumTweedeZit < t2)
                                   select new Event
                                   {
                                       type = "herexamen",
                                       source = (mdl.CursusNummer + " " + mdl.Naam),
                                       description = mdl.Naam,
                                       location = mdl.InfoLokaal,
                                       cancelled = false,
                                       t1 = Convert.ToDateTime(mdl.DatumTweedeZit),
                                       t2 = Convert.ToDateTime(mdl.DatumTweedeZit)
                                   }).ToList();

            // deliberatie
            var deliberatie_query = (from crs in db.Cursists
                                     join plts in db.Plaatsings
                                         on crs.Id equals plts.IdCursist
                                     join mdl in db.IngerichteModulevariants
                                         on plts.IdIngerichteModulevariant equals mdl.Id
                                     where crs.CursistNummer == vx.cursistnummer
                                     && (mdl.DeliberatieDatum > t1 || mdl.DeliberatieDatum < t2)
                                     select new Event
                                     {
                                         type = "deliberatie",
                                         source = (mdl.CursusNummer + " " + mdl.Naam),
                                         description = mdl.Naam,
                                         location = mdl.InfoLokaal,
                                         cancelled = false,
                                         t1 = Convert.ToDateTime(mdl.DeliberatieDatum),
                                         t2 = Convert.ToDateTime(mdl.DeliberatieDatum)
                                     }).ToList();


            // evenementen
            var evenement_query = (from evn in db.Evenements
                                   select new Event
                                   {
                                       type = "evenement",
                                       source = "CVO",
                                       description = evn.Naam,
                                       location = evn.Locatie,
                                       cancelled = false,
                                       t1 = Convert.ToDateTime(evn.StartUur),
                                       t2 = Convert.ToDateTime(evn.EindUur)
                                   }).ToList();

            // feestdagen
            var feestdag_query = (from evn in db.Kalenders
                                  select new Event
                                  {
                                      type = "feestdag",
                                      source = "CVO",
                                      description = evn.Omschrijving,
                                      location = "N/A",
                                      cancelled = false,
                                      t1 = Convert.ToDateTime(evn.Datum),
                                      t2 = Convert.ToDateTime(evn.Datum)
                                  }).ToList();

            // taken
            var taken_query = (from crs in db.Cursists
                               join plts in db.Plaatsings
                                   on crs.Id equals plts.IdCursist
                               join mdl in db.IngerichteModulevariants
                                   on plts.IdIngerichteModulevariant equals mdl.Id
                               join tk in db.Taaks
                                   on plts.IdIngerichteModulevariant equals tk.IdIngerichteModulevariant
                               where crs.CursistNummer == vx.cursistnummer
                               && (tk.Deadline > t1 || tk.Deadline < t2)
                               select new Event
                               {
                                   type = "taak",
                                   source = (mdl.CursusNummer + " " + mdl.Naam),
                                   description = tk.Naam,
                                   location = "N/A",
                                   cancelled = false,
                                   t1 = Convert.ToDateTime(tk.Deadline),
                                   t2 = Convert.ToDateTime(tk.Deadline)
                               }).ToList();

            // afspraken



            List<Event> query = new List<Event>();
            query.AddRange(les_query);
            query.AddRange(examen_query);
            query.AddRange(herexamen_query);
            query.AddRange(deliberatie_query);

            export(query);
        }

        [WebMethod]
        public void cursist_events_filter_imv(string access_token, string t1, string t2)
        {
            DBMDataContext db = new DBMDataContext();

            validator vx = new validator(access_token);

            if (vx.is_valid == false) return;

            var query = from crs in db.Cursists
                        join plts in db.Plaatsings
                            on crs.Id equals plts.IdCursist
                        join mdl in db.IngerichteModulevariants
                            on plts.IdIngerichteModulevariant equals mdl.Id
                        join evt in db.LesDavincis
                            on mdl.Id equals evt.IdIngerichteModulevariant
                        join lkl in db.Lokaals
                            on evt.IdLokaal equals lkl.Id
                        where crs.CursistNummer == vx.cursistnummer
                        orderby evt.Aanvangsdatum
                        select new
                        {
                            mdl.Naam,
                            Lokaal = lkl.Naam,
                            evt.Afgelast,
                            Aanvangsdatum = evt.Aanvangsdatum.ToString(),
                            Einddatum = evt.Einddatum.ToString()
                        };

            // deadlines
            /* query = from c in db.LesDavincis // db.Deadlines
                     select new
                     {
                         c.Afgelast,
                         c.Aanvangsdatum,
                         c.Einddatum
                     };

             // deliberatie
             query = from c in db.IngerichteModulevariants
                     where c.Id == id_modulevariant
                     select new
                     {
                         c.DeliberatieDatum
                     };
             // examen
             query = from c in db.IngerichteModulevariants
                         where c.Id == id_modulevariant
                         select new
                         {
                             c.DatumTweedeZit
                         };

             // herexamen
             query = from c in db.IngerichteModulevariants
                         where c.Id == id_modulevariant
                         select new
                         {
                             c.DatumTweedeZit
                         };
             */
            // afspraken

            // feestdagen

            export(query);

        }

      
        //////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////
        ///// MODULES ////////////////////////////////////////////////////////////////////////

        [WebMethod]
        public void select_modules_filter_opleiding(int id_opleidingsvariant)
        {
            DBMDataContext db = new DBMDataContext();

            var query = (from c in db.Opleidingsvariants
                         join cnc in db.OpleidingsvariantModulevariants
                             on c.Id equals cnc.IdOpleidingsvariant
                         join m in db.Modulevariants
                             on cnc.IdModulevariant equals m.Id
                         where c.Id == id_opleidingsvariant
                         select new
                         {
                             ModuleId = m.Id,
                             m.Code,
                             Modulenaam = m.Naam
                         })
                        .OrderBy(x => x.ModuleId);

            export(query);
        }

        [WebMethod]
        public void select_ingerichte_modules_filter_opleiding(int id_opleidingsvariant)
        {
            DBMDataContext db = new DBMDataContext();

            var query = from c in db.Opleidingsvariants
                        join cnc in db.OpleidingsvariantModulevariants
                            on c.Id equals cnc.IdOpleidingsvariant
                        join m in db.Modulevariants
                            on cnc.IdModulevariant equals m.Id
                        join im in db.IngerichteModulevariants
                            on m.Id equals im.IdModuleVariant
                        where c.Id == id_opleidingsvariant
                        select new
                        {
                            Opleiding = c.Naam,
                            im.CursusNummer,
                            im.Id,
                            Modulenaam = m.Naam
                        };

            export(query);
        }

        [WebMethod]
        public void cursist_modules(string access_token)
        {
            DBMDataContext db = new DBMDataContext();

            validator vx = new validator(access_token);

            if (vx.is_valid == false) return;

            var query = from crs in db.Cursists
                        join plts in db.Plaatsings
                            on crs.Id equals plts.IdCursist
                        join mdl in db.IngerichteModulevariants
                            on plts.IdIngerichteModulevariant equals mdl.Id
                        where crs.CursistNummer == vx.cursistnummer
                        select new
                        {
                            mdl.CursusNummer,
                            mdl.Id,
                            mdl.Naam
                        };

            export(query);

        }



        [WebMethod]
        public void cursist_module_plannen(string access_token, int id_ingerichte_modulevariant)
        {
            DBMDataContext db = new DBMDataContext();

            // juiste login?
            validator vx = new validator(access_token);
            if (vx.is_valid == false) 
                return;

            // bestaat module?
            var imv_query = (from igm in db.IngerichteModulevariants
                             where igm.Id == id_ingerichte_modulevariant
                             select new
                             {
                                 igm.IsInschrijfbaar,
                                 igm.AanvangsDatum,
                                 igm.RegistratieMoment,
                                 igm.MaximumCapaciteit,
                                 igm.PlaatsenVoorDerden
                             }).ToList();

            if (imv_query.Count == 0)
            {
                export(new { Message = "Module bestaat niet." });
                return;
            }

            var imv = imv_query.First();

            // al ingeschreven?
            var query = (from crs in db.Cursists
                         join plts in db.Plaatsings
                             on crs.Id equals plts.IdCursist
                         join mdl in db.IngerichteModulevariants
                             on plts.IdIngerichteModulevariant equals mdl.Id

                         where crs.CursistNummer == vx.cursistnummer
                         && plts.IdIngerichteModulevariant == id_ingerichte_modulevariant
                         select plts.Id).ToList();

            if (query.Count > 0 )
            {
                export(new { Message = "Al ingeschreven." });
                return;
            }

            // inschrijfbaar?
            if ( !imv.IsInschrijfbaar ){
                export(new { Message = "Niet inschrijfbaar."});
                return;
            }

            // binnen inschrijvingsdatum?
            if ( DateTime.Now > imv.RegistratieMoment ) {
                export(new { Message = "Registratie [" + imv.RegistratieMoment + "] is voorbij. Je kunt je niet langer inschrijven."});
               // return;
            }

            // is er nog plaats?
            if ( imv.PlaatsenVoorDerden == 0 ){
                export(new { Message = "De module is volzet."});
               // return;
            }

            // bezit cursist nodige voorkennis?
            var kennis = from vk in db.ModulevariantTrajectVoorkennis
                         join igm in db.IngerichteModulevariants
                            on id_ingerichte_modulevariant equals igm.Id
                          join crs in db.Cursists
                             on vx.id_cursist equals crs.Id
                          join plts in db.PlaatsingHistorieks
                                   on crs.Id equals plts.IdCursist
                         join ev in db.EvaluatieresultaattypeCVOs
                             on plts.IdEvaluatieresultaat equals ev.IdEvaluatieresultaattype  
                         where vk.VoorkennisModulevariantId == igm.IdModuleVariant
                         select new
                         {
                             plts.IdModulevariant,
                             vk.VoorkennisModulevariantId,
                             ev.Code
                         };

            bool condition = true;
            foreach(var kns in kennis){
                if (kns.Code != "GESLAAGD")
                    condition = false;
            }

            if (condition == false)
            {
                export(new { Message = "Je bezit niet de nodige voorkennis." });
                return;
            }


            // inschrijving
            var plaatsing = from plts in db.Plaatsings
                            select plts;

            Plaatsing px = new Plaatsing 
            {
                IdCursist = vx.id_cursist,
                IdIngerichteModulevariant = id_ingerichte_modulevariant,
                IdIngerichteOpleidingsvariant = 1,
                Reservatiedatum = DateTime.Now,
                IdPlaatsingsstatus = 1
            };

            db.Plaatsings.InsertOnSubmit(px);
            db.SubmitChanges();

            export(new { Message = "Inschrijving successvol." });
        }

        [WebMethod]
        public void cursist_module_cancellen(string access_token, int id_ingerichte_modulevariant)
        {
            DBMDataContext db = new DBMDataContext();

            validator vx = new validator(access_token);

            if (vx.is_valid == false) return;

            
            int cancelled = 6;

            var query = (from plts in db.Plaatsings
                         join crs in db.Cursists
                             on plts.IdCursist equals crs.Id
                         where crs.Id == vx.id_cursist &&
                         plts.IdIngerichteModulevariant == id_ingerichte_modulevariant
                         select  plts ).ToList();
            
            // niet ingeschreven?
            if (query.Count == 0 ) {
                export(new { Message = "Niet ingeschreven." });
                return;
            }

            // al uitgeschreven?
            if (query.First().IdPlaatsingsstatus == cancelled){
                export(new { Message = "Al uitgeschreven." });
                return;
            }

            query.First().IdPlaatsingsstatus = cancelled;
            db.SubmitChanges();
            export(new { Message = "Module stopgezet.", Status = query.First().IdPlaatsingsstatus });
        }
        
        //////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////
        ///// AFSPRAKEN //////////////////////////////////////////////////////////////////////

        [WebMethod]
        public void cursist_all_afspraken(string access_token, DateTime t1, DateTime t2)
        {
            
            DBMDataContext db = new DBMDataContext();

            // access_code correct?
            validator vx = new validator(access_token);
            if (vx.is_valid == false)
                return;

            // ingeschreven?
            var query = from afk in db.Afspraaks
                        where afk.Startdatum < t1 && t1 < afk.Einddatum
                        && afk.Startdatum < t2 && t2 < afk.Einddatum
                        select new Event
                        {
                            type = "afspraak",
                            t1 = afk.Startdatum,
                            t2 = afk.Einddatum,
                            cancelled = afk.Cancelled,
                            location = ""
                        };
        }


        [WebMethod]
        public void cursist_afspraak_plannen(string access_token, int id_afspraak)
        {
            DBMDataContext db = new DBMDataContext();

            // access_code correct?
            validator vx = new validator(access_token);
            if (vx.is_valid == false)
                return;

            var query = (from afk in db.Afspraaks
                        select new Afspraak()).ToList();


            // afspraak bestaat?
            if (query.Count() == 0) {
                export(new { Message ="Afspraak bestaat niet."});
                return;
            }

            // al ingeschreven?
            var traject = (from tjc in db.AfspraakTrajectbegleiders
                           join afk in db.Afspraaks
                            on tjc.IdAfspraak equals afk.Id
                          where tjc.IdCursist == vx.id_cursist
                          && afk.IdPersoneel == query.First().IdPersoneel
                          select new AfspraakTrajectbegleider()).ToList();


            if (traject.Count > 0)
            {
                export(new { Message = "Je kan alleen enkele afspraak met dezelfde begeleider maken." });
                return;
            }


            // afspraak al volzet?
            if (query.First().Capacity == query.First().Count)
            {
                export(new { Message = "Afspraak is volzet." });
                return;
            }


            // afspraak inschrijven
            AfspraakTrajectbegleider ax = new AfspraakTrajectbegleider();
                ax.IdCursist = vx.id_cursist;
                ax.IdAfspraak = query.First().Id;
                ax.RegistratieDatum = DateTime.Now;
                ax.Vastgelegd = true;
                ax.Afspraak.Count += 1;

            db.AfspraakTrajectbegleiders.InsertOnSubmit(ax);
            db.SubmitChanges();

            export(new { Message = "Succesvol."});
        }

        [WebMethod]
        public void cursist_afspraak_cancellen(string access_token, int id_afspraak_trajectbegeleider)
        {
            DBMDataContext db = new DBMDataContext();

            // access_code correct?
            validator vx = new validator(access_token);
            if (vx.is_valid == false)
                return;

            // afspraak bestaat?
            var query = (from afk in db.AfspraakTrajectbegleiders
                         where afk.Id == id_afspraak_trajectbegeleider
                         select new AfspraakTrajectbegleider()).ToList();

            
            if (query.Count() == 0)
            {
                export(new { Message = "Afspraak bestaat niet." });
                return;
            }


            // afspraak cancellen
            db.AfspraakTrajectbegleiders.DeleteOnSubmit(query.First());

            export(new { Message = "Afspraak is gecancelled."});
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////
        ///// HEREXAMEN //////////////////////////////////////////////////////////////////////

        [WebMethod]
        public void cursist_herexamen_plannen(string access_token, int id_ingerichte_modulevariant)
        {
            DBMDataContext db = new DBMDataContext();

            // access_code correct?
            validator vx = new validator(access_token);
            if (vx.is_valid == false)
                return;

            // bestaat module?
            var imv_query = (from imv in db.IngerichteModulevariants
                             where imv.Id == id_ingerichte_modulevariant
                             select imv).ToList();

            if (imv_query.Count == 0)
            {
                export(new { Message = "Module bestaat niet." });
                return;
            }


            // ingeschreven?
            var query = (from crs in db.Cursists
                         join plts in db.Plaatsings
                             on crs.Id equals plts.IdCursist
                         join mdl in db.IngerichteModulevariants
                             on plts.IdIngerichteModulevariant equals mdl.Id

                         where crs.CursistNummer == vx.cursistnummer
                         && plts.IdIngerichteModulevariant == id_ingerichte_modulevariant
                         select plts.Id).ToList();

            if (query.Count > 0)
            {
                export(new { Message = "Al ingeschreven." });
                return;
            }

            // gebuisd?


            // code { inschrijven }

            // export(query);
        }

        [WebMethod]
        public void cursist_herexamen_cancellen(string access_token, int id_ingerichte_modulevariant)
        {
            DBMDataContext db = new DBMDataContext();

            // access_code correct?
            validator vx = new validator(access_token);
            if (vx.is_valid == false)
                return;

            // bestaat module?
            var imv_query = (from imv in db.IngerichteModulevariants
                             where imv.Id == id_ingerichte_modulevariant
                             select imv).ToList();

            if (imv_query.Count == 0)
            {
                export(new { Message = "Module bestaat niet." });
                return;
            }


            // ingeschreven?
            var query = (from crs in db.Cursists
                         join plts in db.Plaatsings
                             on crs.Id equals plts.IdCursist
                         join mdl in db.IngerichteModulevariants
                             on plts.IdIngerichteModulevariant equals mdl.Id

                         where crs.CursistNummer == vx.cursistnummer
                         && plts.IdIngerichteModulevariant == id_ingerichte_modulevariant
                         select plts.Id).ToList();

            if (query.Count > 0)
            {
                export(new { Message = "Al ingeschreven." });
                return;
            }

            // gebuisd?


            // code { inschrijven }

            // export(query);
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////
        ///// TAKEN //////////////////////////////////////////////////////////////////////////

        [WebMethod]
        public void cursist_taak_indienen(string access_token, int id_taak)
        {
            DBMDataContext db = new DBMDataContext();

            // access_code correct?
            validator vx = new validator(access_token);
            if (vx.is_valid == false)
                return;
 
        }

        [WebMethod]
        public void cursist_taak_delete(string access_token, int id_taak)
        {
            DBMDataContext db = new DBMDataContext();

            // access_code correct?
            validator vx = new validator(access_token);
            if (vx.is_valid == false)
                return;
        }

        [WebMethod]
        public void cursist_event_inschrijven(string access_token, int id_modulevariant)
        {
            DBMDataContext db = new DBMDataContext();

            // export(query);
        }
      

        /*
         *  
         */

    }
}
