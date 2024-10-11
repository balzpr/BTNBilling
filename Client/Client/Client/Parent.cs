using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Parent : Form
    {

        public static Parent _instance;
        public Parent()
        {
            InitializeComponent();

            _instance = this;
            this.IsMdiContainer = true;
            this.BackColor = Color.Black;
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80; 
                return cp;
            }
        }
        private async void Parent_Load(object sender, EventArgs e)
        {
            if (!File.Exists("configuration.json"))
            {
                Setup setup = new Setup();

                setup.MdiParent = this;

                setup.Show();
            }
            else
            {
                Billing billing = new Billing();
                billing.MdiParent = this;
                billing.Show();


                string json = File.ReadAllText("configuration.json");
                ClientServerConfig config = JsonConvert.DeserializeObject<ClientServerConfig>(json);
                await TCPClient.Connect(config.Server, config.Port);

                if(Options._isConnected)
                {
                    FirstConnect();
                }
                else
                {
                    Billing._instance.guna2Button1.Visible = true;
                    Billing._instance.label1.Text = "Server Lost";
                    Billing._instance.label1.ForeColor = Color.Red;
                    Billing._instance.label2.Text = "Disconnected from server, please report this issue to the server owner.";
                }
            }
        }

        private static async void FirstConnect()
        {
            await TCPClient.SendData($"request_connect,{Environment.UserDomainName}");
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
    }
}
