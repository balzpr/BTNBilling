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
    public partial class Form2 : Form
    {
        public static Form2 _instance;
        public Form2()
        {
            InitializeComponent();
            _instance = this;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Server IP: " + Session.ServerIP;
            toolStripStatusLabel2.Text = "Port: " + Session.Port;
            toolStripStatusLabel3.Text = "Username: " + Session.Username;
            toolStripStatusLabel4.Text = "Log Date: " + Session.LogDate;

            Notify.Logs("Login Successfully, log date " + Session.LogDate);
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex != -1)
            {
                string selected = listBox1.Items[listBox1.SelectedIndex].ToString();
                TcpClient client = null;

                foreach(var item in TCPServer._userList)
                {
                    if(item.Value == selected)
                    {
                        client = item.Key;
                        var data = TCPServer.ClientInfo.Get(client);

                        DateTime endTime = data.StartTime.AddSeconds(data.Timer);

                        label2.Text = data.Username;
                        label3.Text = data.IP;
                        label5.Text = data.Timer / 60 + " minutes.";
                        label7.Text = data.StartTime.ToString();
                        label9.Text = endTime.ToString();

                        if(DateTime.Now >= endTime)
                        {
                            label5.Text = "expired.";
                            button2.Enabled = false;
                            button1.Enabled = true;
                            button3.Enabled = false;
                        }
                        else
                        {
                            button2.Enabled = true;
                            button1.Enabled = false;
                            button3.Enabled = true;
                        }

                        break;
                    }
                }
            }
        }
        public string GetRemainingTime(int Timer, DateTime StartTime)
        {
            TimeSpan elapsedTime = DateTime.Now - StartTime;

            int remainingSeconds = Timer - (int)elapsedTime.TotalSeconds;

            if (remainingSeconds <= 0)
            {
                return "0";
            }

            TimeSpan remainingTime = TimeSpan.FromSeconds(remainingSeconds);

            if (remainingTime.TotalHours >= 1)
            {
                return $"{(int)remainingTime.TotalHours} hour{((int)remainingTime.TotalHours > 1 ? "s" : "")} {remainingTime.Minutes} minute{(remainingTime.Minutes > 1 ? "s" : "")}";
            }
            else if (remainingTime.TotalMinutes >= 1)
            {
                return $"{(int)remainingTime.TotalMinutes} minute{((int)remainingTime.TotalMinutes > 1 ? "s" : "")}";
            }
            else
            {
                return $"{remainingTime.Seconds} second{(remainingTime.Seconds > 1 ? "s" : "")}";
            }
        }
        public string FormatTime(int seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);

            if (time.TotalHours >= 1)
            {
                return $"{(int)time.TotalHours} hour{((int)time.TotalHours > 1 ? "s" : "")} {time.Minutes} minute{(time.Minutes > 1 ? "s" : "")}";
            }
            else if (time.TotalMinutes >= 1)
            {
                return $"{(int)time.TotalMinutes} minute{((int)time.TotalMinutes > 1 ? "s" : "")}";
            }
            else
            {
                return $"{time.Seconds} second{(time.Seconds > 1 ? "s" : "")}";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox4.Enabled = checkBox1.Checked;
            if(checkBox1.Checked)
            {
                Options._isAvailable = true;
                foreach(var item in TCPServer._userList)
                {
                    if(item.Key.Connected)
                    {
                        TCPServer.SendPackets(item.Key.GetStream(), "server_available");

                        Notify.Logs($"Send Packets server_available to {item.Value}");
                    }
                }
            }
            else
            {
                Options._isAvailable = false;
                foreach (var item in TCPServer._userList)
                {
                    if (item.Key.Connected)
                    {
                        TCPServer.SendPackets(item.Key.GetStream(), "server_unavailable");

                        Notify.Logs($"Send Packets server_unavailable to {item.Value}");
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex != -1)
            {
                string selected = listBox1.Items[listBox1.SelectedIndex].ToString();
                TcpClient client = null;

                foreach (var item in TCPServer._userList)
                {
                    if(item.Value == selected)
                    {
                        client = item.Key;
                        var data = TCPServer.ClientInfo.Get(client);
                        if(DateTime.Now >= data.EndTime)
                        {
                            new Form3(item.Value, client.Client.RemoteEndPoint.ToString(), client).Show();
                        }
                        break;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string selected = listBox1.Items[listBox1.SelectedIndex].ToString();
                TcpClient client = null;

                foreach (var item in TCPServer._userList)
                {
                    if (item.Value == selected)
                    {
                        client = item.Key;
                        var data = TCPServer.ClientInfo.Get(client);
                        DialogResult result = MessageBox.Show("Stop this client?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        
                        if(result == DialogResult.Yes)
                        {
                            data.Timer = 0;
                            button2.Enabled = false;
                            button1.Enabled = true;
                            button3.Enabled = false;

                            TCPServer.SendPackets(client.GetStream(), "stop");
                        }
                        break;
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                string selected = listBox1.Items[listBox1.SelectedIndex].ToString();
                TcpClient client = null;

                foreach (var item in TCPServer._userList)
                {
                    if (item.Value == selected)
                    {
                        client = item.Key;
                        var data = TCPServer.ClientInfo.Get(client);

                        new Form4(item.Value, client.Client.RemoteEndPoint.ToString(), client).Show();
                        break;
                    }
                }
            }
        }
    }
}
