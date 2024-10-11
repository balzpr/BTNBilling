using Guna.UI2.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows.Forms;
using System.Xml;

namespace Client
{
    public partial class Setup : Form
    {
        public Setup()
        {
            InitializeComponent();

            guna2TextBox1.Text = GetLocalIPv4();
            guna2TextBox1.ReadOnly = true;
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            if (m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt32() & 0xFFF0) == SC_MOVE)
            {
                return;
            }

            base.WndProc(ref m);
        }

        private async void guna2Button1_Click(object sender, EventArgs e)
        {
            string client = guna2TextBox1.Text;
            string server = guna2TextBox2.Text;
            string port = guna2TextBox3.Text;
            IPAddress ip;
            int ports;

            if(client.Length == 0 ||  server.Length == 0 || port.Length == 0) 
            {
                Notify.Show("Please fill all data before using BTN Billing.", true);
                return;
            }

            if(!IPAddress.TryParse(server, out ip))
            {
                Notify.Show("Invalid server ip address.", true);
                return;
            }

            if(!int.TryParse(port, out ports))
            {
                Notify.Show("Invalid port.", true);
                return;
            }

            guna2Button1.Enabled = false;

            DialogResult result = MessageBox.Show("Finish setup?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                await TCPClient.Connect(server, ports);
                if (TCPClient.isConnected)
                {
                    var config = new ClientServerConfig
                    {
                        Client = client,
                        Server = server,
                        Port = ports
                    };

                    string json = JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText("configuration.json", json);

                    TCPClient.client.Close();

                    Application.Restart();
                }
            }
            guna2Button1.Enabled = true;
        }
        public string GetLocalIPv4()
        {
            string localIP = string.Empty;

            foreach (IPAddress address in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = address.ToString();
                    break;
                }
            }

            return localIP;
        }
    }
}
