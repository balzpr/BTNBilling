using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Form3 : Form
    {
        private TcpClient client;
        public Form3(string client, string ip, TcpClient clients)
        {
            InitializeComponent();

            textBox1.Text = client;
            textBox2.Text = ip;

            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;

            this.client = clients;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if(textBox3.Text.Length == 0)
            {
                Notify.Show("Please fill the timer.", true);
                return;
            }

            if(int.TryParse(textBox3.Text, out int value))
            {

                DialogResult result = MessageBox.Show("Procced?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(result == DialogResult.Yes)
                {
                    var _userInfo = TCPServer.ClientInfo.Get(client);

                    if (_userInfo != null && client.Connected)
                    {
                        _userInfo.Timer = 60 * value;
                        DateTime endTime = _userInfo.StartTime.AddSeconds(_userInfo.Timer);
                        _userInfo.EndTime = endTime;

                        TCPServer.SendPackets(client.GetStream(), $"start,{textBox3.Text}");

                        Form2._instance.button2.Enabled = true;
                        Form2._instance.button1.Enabled = false;
                        Form2._instance.button3.Enabled = true;

                        Notify.Logs($"Start timer for {client.Client.RemoteEndPoint} - user {_userInfo.Username} for {_userInfo.Timer / 60} minutes.");

                        this.Close();
                    }
                    else
                    {
                        Notify.Show("Error communication with the client.", true);
                    }
                }
            }
            else
            {
                Notify.Show("Invalid timer.", true);
            }
        }
    }
}
