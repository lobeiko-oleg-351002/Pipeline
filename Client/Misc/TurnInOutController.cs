using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Client.MainForm;

namespace Client.Misc
{
    public class TurnInOutController
    {
        Form form;
        NotifyIcon notifyIcon;

        public TurnInOutController(Form form, NotifyIcon notifyIcon)
        {
            this.form = form;
            this.notifyIcon = notifyIcon;
        }

        public void TurnInForm()
        {
            form.ShowInTaskbar = false;
            form.Hide();
            notifyIcon.Visible = true;
        }

        public void TurnOutForm()
        {
            form.Show();
            form.ShowInTaskbar = true;
            notifyIcon.Visible = false;
        }

        public void TurnOutWithEventFormIfHidden()
        {
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_TURNOUT_EVENT) && !form.Visible)
            {
                TurnOutForm();
            }
        }

        public void TurnOutWithStatusFormIfHidden()
        {
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_TURNOUT_STATUS) && !form.Visible)
            {
                TurnOutForm();
            }
        }
    }
}
