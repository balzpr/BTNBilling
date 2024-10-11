using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        private Database db;
        public Form1()
        {
            InitializeComponent();
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
        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                db = new Database();

                await Task.Delay(500);

                button1_Click(sender, e);
            }
            catch (Exception er)
            {
                Notify.Show(er.Message, true);
                Application.Exit();
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                button1.Enabled = false;

                bool status = db.Login(textBox1.Text, textBox2.Text);

                if(status)
                {
                    Session.ServerIP = GetLocalIPv4();
                    Session.Port = 2002;
                    Session.Username = textBox1.Text;
                    Session.LogDate = DateTime.Now.ToString("dd MMM yyyy, HH:mm tt");

                    new Form2().Show();
                    this.Hide();

                    await TCPServer.Start(GetLocalIPv4(), 2002);
                }

                button1.Enabled = true;
            }
            catch(Exception er)
            {
                Notify.Show(er.Message, true);
            }
        }
    }
}