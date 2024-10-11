using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Client
{
    public partial class Billing : Form
    {
        public static Billing _instance;
        public Billing()
        {
            InitializeComponent();
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(!Options._isAvailable && !Options._isStart)
            {
                TCPClient.ShowParent();
                label1.Text = "Server Maintenance";
                label1.ForeColor = Color.Red;
                label2.Text = "The server is up but the server is currently under maintenance. Please come back later or contact the server owner.";
            }
            else
            {

                if(Options._isAvailable && !Options._isStart)
                {
                    TCPClient.ShowParent();
                    label1.Text = "Waiting Approval...";
                    label1.ForeColor = Color.Green;
                    label2.Text = "Waiting for confirmation from the server owner, please wait, after confirmation, you can play the computer.";
                }
            }
        }

        private void Billing_Load(object sender, EventArgs e)
        {
            label3.Text = Environment.UserDomainName;
            _instance = this;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Delete current configuration?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(result == DialogResult.Yes)
            {
                if(File.Exists(Application.StartupPath + "\\configuration.json"))
                {
                    File.Delete(Application.StartupPath + "\\configuration.json");
                }

                Application.Restart();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now >= Options.EndTime)
            {
                timer2.Stop();
                Console.Beep(1000, 3000);
                TCPClient.ShowParent();
                label1.Text = "Timeout";
                label1.ForeColor = Color.Red;
                label2.Text = "Your time is up, please come back again.";
                guna2Button1.Visible = true;
            }
        }
    }
}
