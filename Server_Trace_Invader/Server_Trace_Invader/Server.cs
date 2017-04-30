using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace Server_Trace_Invader
{
    class Server
    {
        private HttpListener Listener;
        private const string Prefixes = "http://10.0.3.17:80/";
        private bool Stop = false;
        Thread ListenerThread;

        private string DBuser="root", DBname="spaceapp", DBpwd="";
        private string ConnectionString = "";
        public Server()
        {
            Listener = new HttpListener();
            ConnectionString = "server=localhost;database=" + DBname + ";uid=" + DBuser + ";password=" + DBpwd;
        }

        public void Start()
        {
            ListenerThread = new Thread(new ThreadStart(BeginListening));
            ListenerThread.Start();
        }

        public void StopServer()
        {
            this.Stop = true;
        }

        private void BeginListening()
        {
            string strResponse = "";
            HttpListenerRequest req;
            HttpListenerResponse res;
            StreamWriter writer;

            MySqlConnection dbConn;
            MySqlCommand cmd;

            Listener.Prefixes.Add(Prefixes);
            Listener.Start();
            while (!Stop)
            {
                HttpListenerContext context = Listener.GetContext();
                if (context.Request.QueryString.Count > 0)
                    switch (context.Request.QueryString["r"])
                    {
                        case "new_report":
                            {
                                strResponse = "";
                                req = context.Request;
                                res = context.Response;
                                writer = new StreamWriter(res.OutputStream);

                                dbConn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
                                cmd = dbConn.CreateCommand();
                                try
                                {
                                    dbConn.Open();
                                }
                                catch (Exception erro)
                                {
                                    Console.WriteLine("Erro" + erro);
                                    break;
                                }
                                if (!AuthUser(req.QueryString["email"], req.QueryString["password"], cmd))
                                {
                                    strResponse = "Wrong email or password.";
                                    SendData(res, "text/plain", strResponse, Encoding.Unicode);
                                    cmd.Dispose();
                                    dbConn.Close();
                                    break;
                                }

                                cmd.Dispose();
                                cmd = dbConn.CreateCommand();
                                int keyNewReport = InsertReport(req.QueryString["coordinates"], req.QueryString["id_species"], req.QueryString["timestamp"], req.QueryString["email"], req.QueryString["damage"], req.QueryString["solution"], cmd);
                                if (keyNewReport > 0)
                                {
                                    cmd.Dispose();
                                    cmd = dbConn.CreateCommand();
                                    if (InsertDetail(keyNewReport, req.QueryString["damage"], req.QueryString["solution"], cmd))
                                    {
                                        if (req.QueryString["image_url"] != "")
                                        {
                                            string str = req.QueryString["image_url"];
                                            cmd.Dispose();
                                            cmd = dbConn.CreateCommand();
                                            if (InsertImage(req.QueryString["image_url"], keyNewReport, cmd))
                                                strResponse = "ok";
                                            else
                                                strResponse = "Error in data insertion.";
                                            cmd.Dispose();
                                            dbConn.Close();
                                        }
                                        else
                                        {
                                            strResponse = "ok";
                                        }
                                    }
                                    else
                                    {
                                        strResponse = "Error in data insertion.";
                                    }
                                }
                                else
                                {
                                    strResponse = "Error in data insertion.";
                                }
                                SendData(res, "text/plain", strResponse, Encoding.Unicode);
                                cmd.Dispose();
                                dbConn.Close();
                                break;
                            }
                        case "login":
                            strResponse = "";
                            req = context.Request;
                            res = context.Response;
                            writer = new StreamWriter(res.OutputStream);

                            dbConn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
                            cmd = dbConn.CreateCommand();
                            try
                            {
                                dbConn.Open();
                            }
                            catch (Exception erro)
                            {
                                Console.WriteLine("Erro" + erro);
                                break;
                            }
                            if (AuthUser(req.QueryString["email"], req.QueryString["password"], cmd))
                                strResponse = "ok";
                            else
                                strResponse = "Wrong email or password.";
                            SendData(res, "text/plain", strResponse, Encoding.Unicode);
                            cmd.Dispose();
                            dbConn.Close();
                            break;
                        case "all_reports":
                            strResponse = "";
                            req = context.Request;
                            res = context.Response;
                            writer = new StreamWriter(res.OutputStream);

                            dbConn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
                            cmd = dbConn.CreateCommand();
                            try
                            {
                                dbConn.Open();
                            }
                            catch (Exception erro)
                            {
                                Console.WriteLine("Erro" + erro);
                                break;
                            }
                            strResponse= GetAllReportsCoordinates(cmd);
                            SendData(res, "application/json", strResponse, Encoding.Unicode);
                            cmd.Dispose();
                            dbConn.Close();
                            break;
                        case "all_species":
                            strResponse = "";
                            req = context.Request;
                            res = context.Response;
                            writer = new StreamWriter(res.OutputStream);

                            dbConn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
                            cmd = dbConn.CreateCommand();
                            try
                            {
                                dbConn.Open();
                            }
                            catch (Exception erro)
                            {
                                Console.WriteLine("Erro" + erro);
                                break;
                            }
                            strResponse = GetAllTaxon(cmd);
                            SendData(res, "application/json", strResponse, Encoding.Unicode);
                            cmd.Dispose();
                            dbConn.Close();
                            break;
                        case "show_report":
                            strResponse = "";
                            req = context.Request;
                            res = context.Response;
                            writer = new StreamWriter(res.OutputStream);

                            dbConn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
                            cmd = dbConn.CreateCommand();
                            try
                            {
                                dbConn.Open();
                            }
                            catch (Exception erro)
                            {
                                Console.WriteLine("Erro" + erro);
                                break;
                            }
                            strResponse = GetReport(Convert.ToInt32(req.QueryString["report_id"]),cmd);
                            SendData(res, "application/json", strResponse, Encoding.Unicode);
                            cmd.Dispose();
                            dbConn.Close();
                            break;
                        
                        default:
                            req = context.Request;
                            res = context.Response;
                            writer = new StreamWriter(res.OutputStream);
                            SendData(res, "text/plain", "Error: r contains -> " + context.Request.QueryString["r"], Encoding.Unicode);
                            foreach (string s in context.Request.QueryString.Keys)
                                Console.WriteLine(s + ": " + context.Request.QueryString[s]);
                            break;
                    }
            }
        }

        private void SendData(HttpListenerResponse response, string contentType, string message, Encoding encode)
        {
            response.ContentType = contentType;
            response.ContentEncoding = encode;
            response.ContentLength64 = message.Length;
            StreamWriter writer = new StreamWriter(response.OutputStream);
            writer.Write(message);
            writer.Flush();
            writer.Close();
        }

        private bool AuthUser(string email, string pswHash, MySqlCommand cmd)
        {
            cmd.CommandText = "SELECT * " +
                              "FROM users u " +
                              "WHERE u.email=@email AND u.password=@password;";
            cmd.Parameters.AddWithValue("@email", email); 
            cmd.Parameters.AddWithValue("@password", pswHash); 
            cmd.Prepare();


            MySqlDataReader SQLreader = cmd.ExecuteReader();
            bool res=SQLreader.Read();
            SQLreader.Close();
            return res;
        }

        private int InsertReport(string coordinates, string Taxon, string timestamp, string email, string damages, string solutions, MySqlCommand cmd)
        {
            try
            {
                float x = (float)Convert.ToDouble(coordinates.Split(' ')[0].Replace('.', ',')), y = (float)Convert.ToDouble(coordinates.Split(' ')[1].Replace('.', ','));
                cmd.CommandText = "INSERT INTO reports(locationX, locationY, idTaxon, timestamp, email) " +
                                  "VALUES (@X, @Y, @idTaxon, FROM_UNIXTIME(@timestamp), @email);";
                cmd.Parameters.AddWithValue("@X", x);
                cmd.Parameters.AddWithValue("@Y", y);
                cmd.Parameters.AddWithValue("@idTaxon", Taxon);
                cmd.Parameters.AddWithValue("@timestamp", timestamp);
                cmd.Parameters.AddWithValue("@email", email);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    cmd.CommandText = "Select idReport " +
                                      "from reports " +
                                      "where idTaxon=@idTaxon AND timestamp=FROM_UNIXTIME(@timestamp) AND email=@email;";

                    MySqlDataReader SQLreader = cmd.ExecuteReader();

                    bool tmp= SQLreader.Read();
                    int res= SQLreader.GetInt32("idReport");
                    SQLreader.Close();
                    return res;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return -1;
        }

        private bool InsertDetail(int idDetail,string damage, string solution,MySqlCommand cmd)
        {
            cmd.CommandText = "INSERT INTO details(idDetail, damage, solution) " +
                               "VALUES (@idDetail, @damage, @solution);";
            cmd.Parameters.AddWithValue("@idDetail", idDetail);
            cmd.Parameters.AddWithValue("@damage", damage);
            cmd.Parameters.AddWithValue("@solution", solution);
            if (cmd.ExecuteNonQuery() > 0)
            {
                return true;
            }
            return false;
        }

        private string GetAllReportsCoordinates(MySqlCommand cmd)
        {
            List<string> res = new List<string>();
            cmd.CommandText = "SELECT idReport, locationX, locationY " +
                              "FROM reports;";
            cmd.Prepare();

            MySqlDataReader SQLreader = cmd.ExecuteReader();

            while (SQLreader.Read())
            {
                var obj = new
                {
                    id = SQLreader.GetInt32("idReport"),
                    locationX = SQLreader.GetDouble("locationX"),
                    locationY = SQLreader.GetDouble("locationY"),
                };
                res.Add(JsonConvert.SerializeObject(obj));
            }
            return JsonConvert.SerializeObject(res);
        }

        private string GetAllTaxon(MySqlCommand cmd)
        {
            List<string> res = new List<string>();
            cmd.CommandText = "SELECT idTaxon, name " +
                              "FROM Taxons;";
            cmd.Prepare();

            MySqlDataReader SQLreader = cmd.ExecuteReader();

            while (SQLreader.Read())
            {
                var obj = new
                {
                    idTaxon = SQLreader.GetInt32("idTaxon"),
                    name = SQLreader.GetString("name"),
                };
                res.Add(JsonConvert.SerializeObject(obj));
            }
            return JsonConvert.SerializeObject(res);
        }

        private string GetReport(int idReport, MySqlCommand cmd)
        {
            List<string> res =new List<string>();
            cmd.CommandText = "SELECT * " +
                              "FROM Reports r inner join Taxons t ON (r.idTaxon=t.idTaxon) inner join Details d ON (r.idReport=d.idDetail) left join Images i ON (r.idReport=i.idReport) " +
                              "WHERE r.idReport =@idReport";
            cmd.Parameters.AddWithValue("@idReport", idReport);
            cmd.Prepare();

            MySqlDataReader SQLreader = cmd.ExecuteReader();
            string damage = "", solution = "", image_url = "";
            while (SQLreader.Read())
            {
                if (!SQLreader.IsDBNull(SQLreader.GetOrdinal("damage")))
                {
                    damage = SQLreader.GetString("damage");
                }
                if (!SQLreader.IsDBNull(SQLreader.GetOrdinal("solution")))
                {
                    solution = SQLreader.GetString("solution");
                }
                if (!SQLreader.IsDBNull(SQLreader.GetOrdinal("url")))
                {
                    image_url = SQLreader.GetString("url");
                }
                var obj = new
                {
                    idReport = SQLreader.GetInt32("idReport"),
                    locationX = SQLreader.GetFloat("locationX"),
                    locationY = SQLreader.GetFloat("locationY"),
                    timespan = SQLreader.GetDateTime("timestamp"),
                    trust = SQLreader.GetInt32("trust"),
                    email = SQLreader.GetString("email"),
                    idTaxon = SQLreader.GetInt32("idTaxon"),
                    name = SQLreader.GetString("name"),
                    idDetail = SQLreader.GetInt32("idDetail"),
                    damage = damage,
                    solution = solution,
                    image_url = image_url,
                };
                res.Add(JsonConvert.SerializeObject(obj));
            }
            return JsonConvert.SerializeObject(res);
        }
        
        private bool InsertImage(string url,int idReport, MySqlCommand cmd)
        {
            cmd.CommandText = "INSERT INTO Images(url, idReport) " +
                                    "VALUES (@url, @idReport);";
            cmd.Parameters.AddWithValue("@url", url);
            cmd.Parameters.AddWithValue("@idReport", idReport);
            if (cmd.ExecuteNonQuery() > 0)
            {
                return true;
            }
            return false;
        }
    }
}