using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class TCPServer
    {
        public static TcpListener server;
        public static bool isRunning;
        public static Dictionary<TcpClient, string> _userList = new Dictionary<TcpClient, string>();
        public static Dictionary<TcpClient, ClientInfo> _userInfo = new Dictionary<TcpClient, ClientInfo>();
        public static async Task Start(string ipaddress, int port)
        {
            server = new TcpListener(IPAddress.Parse(ipaddress), port);
            server.Start();
            isRunning = true;

            while(isRunning)
            {
                TcpClient client = await server.AcceptTcpClientAsync();

                Notify.Logs($"{client.Client.RemoteEndPoint} has connected.");

                _ = HandleClient(client);
            }
        }

        public static async Task HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while((bytesRead = await stream.ReadAsync(buffer,0,buffer.Length)) > 0)
                {
                    string recData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    if(recData.Contains(","))
                    {
                        string[] array = recData.Split(',');
                        string key = array[0];
                        string value = array[1];

                        switch(key)
                        {
                            case "request_connect":
                                RegisterClient(client, value);
                                if (!Options._isAvailable)
                                {
                                    SendPackets(stream, "server_unavailable");
                                }
                                else
                                {
                                    SendPackets(stream, "server_available");
                                }
                                break;
                        }
                    }
                }
                RemoveClient(client);
                Notify.Logs($"{client.Client.RemoteEndPoint} has disconnected gracefully.");
            }
            catch (Exception er)
            {
                RemoveClient(client);
                Notify.Logs("Client disconnected: " + er.Message);
            }
            finally
            {
                client.Close();
            }
        }
        private static void RegisterClient(TcpClient client, string value)
        {
            if (!_userList.ContainsKey(client))
            {
                _userList.Add(client, value);
                ClientInfo.Add(client, value);
                Notify.Logs($"Registering new session {value}");
                ParsingDataToListBox();
                return;
            }

            Notify.Logs($"Failed to register client ip {client.Client.RemoteEndPoint}. An existing connection is already exists.");
            client.Close();
        }
        private static void RemoveClient(TcpClient client)
        {
            if(_userInfo.ContainsKey(client))
                _userInfo.Remove(client);

            if(_userList.ContainsKey(client))
                _userList.Remove(client);

            client.Close();

            ParsingDataToListBox();
        }
        private static void ParsingDataToListBox()
        {
            Form2._instance.listBox1.Items.Clear();
            foreach (var item in _userList)
            {
                Form2._instance.listBox1.Items.Add(item.Value);
            }
        }
        public static async void SendPackets(NetworkStream stream, string message)
        {
            byte[] response = Encoding.ASCII.GetBytes(message);
            await stream.WriteAsync(response, 0, response.Length);
        }

        public class ClientInfo
        {
            public string Username { get; set; }
            public string IP { get; set; }
            public int Timer { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public static void Add(TcpClient client, string username)
            {
                ClientInfo info = new ClientInfo
                {
                    Username = username,
                    IP = client.Client.RemoteEndPoint.ToString(),
                    Timer = 0,
                    StartTime = DateTime.Now
                };

                _userInfo.Add(client, info);
            }
            public static ClientInfo Get(TcpClient client)
            {
                if(_userInfo.ContainsKey(client))
                {
                    return _userInfo[client];
                }

                return null;
            }
        }
    }
}
