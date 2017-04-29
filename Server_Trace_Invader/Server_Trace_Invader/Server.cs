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

namespace SerVEr_FInaLe
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
                                int keyNewReport = InsertReport(req.QueryString["coordinates"], req.QueryString["species"], req.QueryString["timestamp"], req.QueryString["email"], req.QueryString["damage"], req.QueryString["solution"], cmd);
                                if (keyNewReport>0)
                                {
                                    cmd = dbConn.CreateCommand();
                                    if (InsertDetail(keyNewReport, req.QueryString["damage"], req.QueryString["solution"], cmd))
                                    {
                                        strResponse = "ok";
                                    }
                                    else
                                    {
                                        strResponse = "Error in data insertion.";
                                    }
                                }
                                else
                                    strResponse = "Error in data insertion.";
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
                            strResponse=GetAllReports(cmd);
                            SendData(res, "application/json", strResponse, Encoding.Unicode);
                            cmd.Dispose();
                            dbConn.Close();
                            break;
                        default:
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
                              "FROM user u " +
                              "WHERE u.email=@email AND u.password=@password;";
            cmd.Parameters.AddWithValue("@email", email); 
            cmd.Parameters.AddWithValue("@password", pswHash); 
            cmd.Prepare();


            MySqlDataReader SQLreader = cmd.ExecuteReader();
            bool res=SQLreader.Read();
            SQLreader.Close();
            return res;
        }

        private int InsertReport(string coordinates, string species, string timestamp, string email, string damages, string solutions, MySqlCommand cmd)
        {
            try
            {
                double x = Convert.ToDouble(coordinates.Split(' ')[0].Replace('.', ',')), y = Convert.ToDouble(coordinates.Split(' ')[1].Replace('.', ','));
                cmd.CommandText = "INSERT INTO reports(locationX, locationY, species, timestamp, email, damages, solutions) " +
                                  "VALUES (@X, @Y, @id_species, FROM_UNIXTIME(@timestamp), @email);";
                cmd.Parameters.AddWithValue("@X", x);
                cmd.Parameters.AddWithValue("@Y", y);
                cmd.Parameters.AddWithValue("@species", species);
                cmd.Parameters.AddWithValue("@timestamp", timestamp);
                cmd.Parameters.AddWithValue("@email", email);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    cmd.CommandText = "Select idReport " +
                                      "from reports " +
                                      "where locationX=@X AND locationY=@Y AND idSpecies=@id_species AND timestamp=FROM_UNIXTIME(@timestamp) AND email=@email);";
                    MySqlDataReader SQLreader = cmd.ExecuteReader();

                    SQLreader.Read();
                    return SQLreader.GetInt32("idReport");
                }
            }
            catch
            {
                
            }
            return -1;
        }

        private bool InsertDetail(int idDetail,string damage, string solution,MySqlCommand cmd)
        {
            cmd.CommandText = "INSERT INTO reports(idDetail, damage, solution) " +
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

        private string GetAllReports(MySqlCommand cmd)
        {
            List<string> res=new List<string>();
            cmd.CommandText = "SELECT id, locationX, locationY " +
                              "FROM report;";
            cmd.Prepare();

            MySqlDataReader SQLreader = cmd.ExecuteReader();
            
            while (SQLreader.Read())
            {
                var obj = new
                {
                    id = SQLreader.GetInt32("id"),
                    locationX = SQLreader.GetDouble("locationX"),
                    locationY = SQLreader.GetDouble("locationY"),
                };
                res.Add(JsonConvert.SerializeObject(obj));
            }
            return JsonConvert.SerializeObject(res);
        }
    }
}