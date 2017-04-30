using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;

namespace Server_Trace_Invader
{
    class Program
    {
        public static string url = "https://www.inaturalist.org/";
        public static string observationParams = "observations.json?per_page=200";
        public static string placesParams = "places.json?per_page=200&place_type=country";

        static void Main(string[] args)
        {
            Server s = new Server();
            s.Start();
            Console.WriteLine("Do you want to download more entries for Taxon's database? [y/N]");
            string command = "";
            while(command!="y" && command!="n")
            {
                // command = Convert.ToString(((command = Console.ReadLine()) == "" ? null : command)[0]).ToLower() ?? "n";
                command = Console.ReadLine();
                if (command == "")
                    command = "n";
                command = Convert.ToString(command[0]).ToLower();
            }
            if(command=="y")
                foo();

        }

        static void foo()
        {
            List<string> countries = new List<string>();
            Console.WriteLine("Insert country: ");
            string country = Console.ReadLine();
            Console.WriteLine("Insert year: ");
            string year = Console.ReadLine();
            Console.WriteLine("Insert month: ");
            string month = Console.ReadLine();
            Console.WriteLine("Insert day: ");
            string day = Console.ReadLine();
            Console.WriteLine("Insert pages: ");
            int pages = Convert.ToInt32(Console.ReadLine());

            string text;
            int page = 1;

            List<Observation> obs = new List<Observation>();
            List<Place> places = new List<Place>();
            Place countrySelected = new Place();
            bool end = true;
            while (end)
            {
                Console.WriteLine("Loading countries");
                var request = WebRequest.Create(url + placesParams + "&page=" + page);
                request.Proxy = null;
                request.ContentType = "application/json; charset=utf-8";

                var response = (HttpWebResponse)request.GetResponse();



                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    text = sr.ReadToEnd();
                    List<Place> tmp = JsonConvert.DeserializeObject<List<Place>>(text);
                    if (tmp.Count == 0)
                    {
                        end = false;
                    }
                    places.AddRange(tmp);

                    page++;
                }
            }
            foreach (Place p in places)
            {
                countries.Add(p.name);
            }
            while (end && country != "")
            {
                Console.WriteLine("Getting countries");
                var request = WebRequest.Create(url + placesParams + "&page=" + page);
                request.Proxy = null;
                request.ContentType = "application/json; charset=utf-8";

                var response = (HttpWebResponse)request.GetResponse();



                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    text = sr.ReadToEnd();
                    List<Place> tmp = JsonConvert.DeserializeObject<List<Place>>(text);
                    if (tmp.Count == 0)
                    {
                        end = false;
                    }
                    places.AddRange(tmp);

                    page++;
                }

            }

            /*
            foreach (Place place in places)
            {
                Console.WriteLine("Filtering countries");
                if (place.name == country)
                {
                    countrySelected = place;
                }
            }*/
            page = 0;
            end = true;

            while (page <= pages)
            {
                Console.WriteLine("Getting observations at page {0}", page);
                var request = WebRequest.Create(url + observationParams + "&swlat=" + countrySelected.swlat + "&swlng=" + countrySelected.swlng + "&nelat=" + countrySelected.nelat + "&nelng=" + countrySelected.nelng + "&page=" + page + "&year=" + year + "&month=" + month + "&day=" + day + "&has[geo]");
                request.Proxy = null;
                request.ContentType = "application/json; charset=utf-8";
                var response = (HttpWebResponse)request.GetResponse();


                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    text = sr.ReadToEnd();
                    List<Observation> tmp = JsonConvert.DeserializeObject<List<Observation>>(text);
                    if (tmp.Count == 0)
                    {
                        end = false;
                    }
                    obs.AddRange(tmp);

                }
                page++;
            }
            //List<Observation> filteredObservation = new List<Observation>();
           /* foreach (Observation o in obs)
            {
                Console.WriteLine("Filtering observations by countries");
                if (countries.Contains(o.place_guess))
                {
                    filteredObservation.Add(o);
                }
            }*/

            List<ObservationDetail> obsDetails = new List<ObservationDetail>();
            foreach (Observation observation in obs)
            {
                Console.WriteLine("Getting observation details");
                var request = WebRequest.Create(url + "observations/" + observation.id + ".json");

                request.Proxy = null;
                request.ContentType = "application/json; charset=utf-8";
                var response = (HttpWebResponse)request.GetResponse();


                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    text = sr.ReadToEnd();
                    ObservationDetail tmp = JsonConvert.DeserializeObject<ObservationDetail>(text);
                    obsDetails.Add(tmp);
                }
            }

            MySqlConnection dbConn;
            MySqlCommand cmd;
            string DBuser = "root", DBname = "spaceapp", DBpwd = "";
            string ConnectionString = "server=localhost;database=" + DBname + ";uid=" + DBuser + ";password=" + DBpwd;
            dbConn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
            try
            {
                dbConn.Open();
            }
            catch (Exception erro)
            {
                Console.WriteLine("Erro" + erro);

            }
            foreach (ObservationDetail d in obsDetails)
            {
                if (d.time_observed_at_utc != null)
                {
                    try
                    {
                        cmd = dbConn.CreateCommand();
                        cmd.CommandText = "INSERT INTO taxons(idTaxon, name, observationCount, wikipedia_summary) " +
                                          "VALUES (@id, @name, @observationCount, @wikipedia_summary);";
                        cmd.Parameters.AddWithValue("@id", d.taxon.id);
                        cmd.Parameters.AddWithValue("@name", d.taxon.name);
                        cmd.Parameters.AddWithValue("@observationCount", d.taxon.observations_count);
                        cmd.Parameters.AddWithValue("@wikipedia_summary", d.taxon.wikipedia_summary);
                    
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            Console.WriteLine("Inserito {0}", d.taxon.name);
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Errore inserimento {0}", e);
                    }
                    cmd = dbConn.CreateCommand();

                    try
                    {
                        string timestamp = d.time_observed_at_utc.Replace("T", " ");
                        timestamp = timestamp.Substring(0, timestamp.Length - 5);
                        cmd.CommandText = "INSERT INTO reports(idReport, locationX, locationY, timestamp,trust, email, idTaxon) " +
                                          "VALUES (@idReport, @X, @Y, @timestamp, @trust, @email, @idTaxon);";
                        cmd.Parameters.AddWithValue("@idReport", Convert.ToInt32(d.id));
                        cmd.Parameters.AddWithValue("@X", Convert.ToDouble(d.latitude.Replace('.', ',')));
                        cmd.Parameters.AddWithValue("@Y", Convert.ToDouble(d.longitude.Replace('.', ',')));
                        cmd.Parameters.AddWithValue("@timestamp", timestamp);
                        cmd.Parameters.AddWithValue("@email", "prova@prova.com");
                        cmd.Parameters.AddWithValue("@trust", 5);
                        cmd.Parameters.AddWithValue("@idTaxon", Convert.ToInt32(d.taxon.id));
                    
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            Console.WriteLine("Inserito {0}", d.id);
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Errore inserimento {0}", e);
                    }
                    cmd = dbConn.CreateCommand();

                    try
                    {
                        cmd.CommandText = "INSERT INTO details(idDetail, quality_grade, identifications_count) " +
                                      "VALUES (@idDetail, @quality_grade, @identifications_count);";
                        cmd.Parameters.AddWithValue("@idDetail", Convert.ToInt32(d.id));
                        cmd.Parameters.AddWithValue("@quality_grade", d.quality_grade);
                        cmd.Parameters.AddWithValue("@identifications_count", Convert.ToInt32(d.identifications_count));
                    
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            Console.WriteLine("Inserito {0}", d.id);
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Errore inserimento {0}", e);
                    }
                }

            }
        }
    }
}
