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
                                string strResponse = "";
                                HttpListenerRequest req = context.Request;
                                HttpListenerResponse res = context.Response;
                                StreamReader reader = new StreamReader(req.InputStream, req.ContentEncoding);
                                StreamWriter writer = new StreamWriter(res.OutputStream);

                                MySqlConnection dbConn = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
                                MySqlCommand cmd = dbConn.CreateCommand();
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
                                    strResponse = "nl";
                                    res.ContentType = "text/plain";
                                    res.ContentEncoding = Encoding.Unicode;
                                    res.ContentLength64 = strResponse.Length;
                                    writer = new StreamWriter(res.OutputStream);
                                    writer.Write(strResponse);
                                    writer.Flush();
                                    writer.Close();
                                    cmd.Dispose();
                                    dbConn.Close();
                                    break;
                                }

                                cmd.Dispose();
                                cmd = dbConn.CreateCommand();
                                double x = Convert.ToDouble(req.QueryString["coordinates"].Split(' ')[0].Replace('.',',')), y = Convert.ToDouble(req.QueryString["coordinates"].Split(' ')[1].Replace('.', ','));
                                if (InsertReport(x, y, req.QueryString["species"], req.QueryString["timestamp"], req.QueryString["email"], req.QueryString["damage"], req.QueryString["solution"], cmd))
                                    strResponse = "ok";
                                else
                                    strResponse = "dbe";
                                res.ContentType = "text/plain";
                                res.ContentEncoding = Encoding.Unicode;
                                res.ContentLength64 = strResponse.Length;
                                writer = new StreamWriter(res.OutputStream);
                                writer.Write(strResponse);
                                writer.Flush();
                                writer.Close();
                                cmd.Dispose();
                                dbConn.Close();
                                break;
                            }
                        default:
                            foreach (string s in context.Request.QueryString.Keys)
                                Console.WriteLine(s + ": " + context.Request.QueryString[s]);
                            break;
                    }
            }
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

        private bool InsertReport(double X, double Y, string species, string timestamp, string email, string damages, string solutions, MySqlCommand cmd)
        {
            cmd.CommandText = "INSERT INTO report(locationX, locationY, species, timestamp, email, damages, solutions) " +
                              "VALUES (@X, @Y, @species, FROM_UNIXTIME(@timestamp), @email, @damages, @solutions);";
            cmd.Parameters.AddWithValue("@X", X); 
            cmd.Parameters.AddWithValue("@Y", Y);
            cmd.Parameters.AddWithValue("@species", species); 
            cmd.Parameters.AddWithValue("@timestamp", timestamp);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@damages", damages);
            cmd.Parameters.AddWithValue("@solutions", solutions);
            if (cmd.ExecuteNonQuery() > 0)
                return true;
            return false;
        }
    }
}