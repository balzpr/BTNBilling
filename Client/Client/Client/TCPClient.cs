using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class TCPClient
    {
        public static TcpClient client;
        public static NetworkStream stream;
        public static bool isConnected;
        public static async Task Connect(string serverIp, int port)
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(serverIp, port);

                isConnected = true;
                Options._isConnected = true;

                stream= client.GetStream();

                _ = ReceiveData();
            }
            catch(Exception er)
            {
                Billing._instance.timer1.Stop();
                Billing._instance.guna2Button1.Visible = true;
                Billing._instance.label1.Text = "Server Lost";
                Billing._instance.label1.ForeColor = Color.Red;
                Billing._instance.label2.Text = "Disconnected from server, please report this issue to the server owner.";
            }
        }

        public static async Task SendData(string message)
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch(Exception er)
            {
                Notify.Show(er.Message);
            }
        }

        public static async Task ReceiveData()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while((bytesRead = await stream.ReadAsync(buffer,0, buffer.Length)) > 0)
                {
                    string recData = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    Console.WriteLine(recData);

                    if(recData.Contains(","))
                    {
                        string[] array = recData.Split(',');
                        string key = array[0];
                        string value = array[1];

                        switch(key)
                        {
                            case "start":
                                Billing._instance.timer2.Start();
                                Parent._instance.Visible = false;
                                Options._isStart = true;
                                Options.StartTime = DateTime.Now;
                                Options.EndTime = DateTime.Now.AddMinutes(Convert.ToInt32(value));
                                break;
                            case "notification":
                                ShowParent();

                                Billing._instance.label1.Text = "Notification";
                                Billing._instance.label1.ForeColor = Color.Red;
                                Billing._instance.label2.Text = "You got a new notification from the server.";

                                Notify.Show(value);

                                Parent._instance.Visible = false;
                                break;
                        }
                    }
                    else
                    {
                        switch(recData)
                        {
                            case "server_unavailable":
                                Options._isAvailable = false;
                                break;
                            case "server_available":
                                Options._isAvailable = true;
                                break;
                            case "stop":
                                ShowParent();
                                Billing._instance.label1.Text = "Stopped By Server";
                                Billing._instance.label1.ForeColor = Color.Red;
                                Billing._instance.label2.Text = "Your connection has been terminated by the server. If this is an error, please contact the server owner.";
                                break;
                        }
                    }
                }
            }
            catch(Exception er)
            {
                TCPClient.ShowParent();
                Billing._instance.timer1.Stop();
                Billing._instance.guna2Button1.Visible = true;
                Billing._instance.label1.Text = "Server Lost";
                Billing._instance.label1.ForeColor = Color.Red;
                Billing._instance.label2.Text = "Disconnected from server, please report this issue to the server owner.";
                Console.WriteLine(er.Message);
            }
            finally
            {
                TCPClient.ShowParent();
                CloseConnection();
            }
        }
        public static void ShowParent()
        {
            Parent._instance.Visible = true;
        }
        public static void CloseConnection()
        {
            if(client != null)
            {
                stream?.Close();
                client?.Close();
            }
        }
    }
}
