using Client.Misc.IndicationService;
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
        TrayIconController trayIconController;

        public TurnInOutController(Form form, TrayIconController trayIconController)
        {
            this.form = form;
            this.trayIconController = trayIconController;
        }

        public void TurnInForm()
        {
            form.ShowInTaskbar = false;
            form.Hide();
            trayIconController.ShowNotifyIcon();
        }

        public void TurnOutForm()
        {
            form.Show();
            form.ShowInTaskbar = true;
            trayIconController.HideNotifyIcon();
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
