using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Misc.IndicationService
{
    public class TrayIconController
    {
        private NotifyIcon notifyIcon = new NotifyIcon();
        IconSet iconSet;

        public void HideNotifyIcon()
        {
            notifyIcon.Visible = false;
        }

        public void ShowNotifyIcon()
        {
            notifyIcon.Visible = true;
        }

        public void DealWithTrayIcon(int newEventsCount)
        {
            if ((newEventsCount > 0))
            {
                SetTrayNewEventIcon();
            }
            else
            {
                SetTrayCommontIcon();
            }
        }

        public void SetTrayNewEventIcon()
        {
            notifyIcon.Icon = iconSet.NewEventIcon;
        }

        public void SetTrayServerOfflineIcon()
        {
            notifyIcon.Icon = iconSet.ServerOfflineIcon;
        }

        public void SetTrayCommontIcon()
        {
            notifyIcon.Icon = iconSet.BaseIcon;
        }

        public TrayIconController(IconSet iconSet, MouseEventHandler doubleClickHandler)
        {
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.BalloonTipText = "Программа работает в фоновом режиме.";
            notifyIcon.BalloonTipTitle = Properties.Resources.CLIENT_NAME; 
            notifyIcon.Icon = iconSet.BaseIcon;
            this.iconSet = iconSet;
            notifyIcon.Text = Properties.Resources.CLIENT_NAME; 
            notifyIcon.MouseDoubleClick += doubleClickHandler;
        }
    }
}
