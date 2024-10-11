using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    internal class Notify
    {
        public static void Show(string text, bool error = false)
        {
            Guna2MessageDialog dialog = new Guna2MessageDialog
            {
                Buttons = MessageDialogButtons.OK,
                Icon = error ? MessageDialogIcon.Error : MessageDialogIcon.Information,
                Style = MessageDialogStyle.Dark,
                Caption = "Notification",
                Text = text,
                Parent = Parent._instance
            };
            dialog.Show();
        }

        public static Guna2MessageDialog Confirmation(string text)
        {
            Guna2MessageDialog dialog = new Guna2MessageDialog
            {
                Buttons = MessageDialogButtons.YesNo,
                Icon = MessageDialogIcon.Question,
                Style = MessageDialogStyle.Dark,
                Caption = "Notification",
                Text = text,
                Parent = Parent._instance
            };
            dialog.Show();

            return dialog;
        }
    }
}
