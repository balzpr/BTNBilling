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
    public partial class Form4 : Form
    {
        private TcpClient _client;
        
        public Form4(string client, string ip, TcpClient clients)
        {
            InitializeComponent();

            textBox1.Text = client;
            textBox2.Text = ip;
            _client = clients;

            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox3.Text.Length == 0)
            {
                return;
            }

            TCPServer.SendPackets(_client.GetStream(), "notification," + textBox3.Text);
            this.Close();
        }
    }
}
