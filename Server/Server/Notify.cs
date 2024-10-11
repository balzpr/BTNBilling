using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    internal class Notify
    {
        public static void Show(string text, bool error = false)
        {
            MessageBox.Show(text, Application.ProductName, MessageBoxButtons.OK, error ? MessageBoxIcon.Stop : MessageBoxIcon.Information);
        }
        public static void Logs(string text)
        {
            Form2._instance.textBox1.Invoke(new Action(() =>
            {
                Form2._instance.textBox1.AppendText($"[{DateTime.Now.ToString("dd MMM yyyy, HH:mm tt")}]: {text}" + Environment.NewLine);
            }));
        }
    }
}
