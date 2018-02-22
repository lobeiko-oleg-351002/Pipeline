using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Client.MainForm;

namespace Client.Misc
{
    public class Indication
    {
        private int NewEventsCount;
        private int NewStatusesCount;
        private Form form;
        private string formName;
        private NotifyIcon notifyIcon = new NotifyIcon();
        private TurnInOutController turnInOutController = null;

        public Indication(Form form)
        {
            NewEventsCount = 0;
            NewStatusesCount = 0;
            this.form = form;
            formName = Properties.Resources.CLIENT_NAME;
            InitTrayIcon();
            turnInOutController = new TurnInOutController(form, notifyIcon);
            IndicateMissedEventsAndStatuses();
        }

        public void HideNotifyIcon()
        {
            notifyIcon.Visible = false;
        }

        public void IncNewEventsCount()
        {
            NewEventsCount++;
            IndicateMissedEventsAndStatuses();
            turnInOutController.TurnOutWithEventFormIfHidden();
        }

        public void DecNewEventsCount()
        {
            NewEventsCount--;
            IndicateMissedEventsAndStatuses();
        }

        public void IncNewStatusesCount()
        {
            NewStatusesCount++;
            InvokeMissedStatusIndication();
        }

        public void InvokeMissedStatusIndication()
        {
            turnInOutController.TurnOutWithStatusFormIfHidden();
            IndicateMissedEventsAndStatuses();
        }

        public void DecNewStatusesCount()
        {
            NewStatusesCount--;
            IndicateMissedEventsAndStatuses();
        }

        private void IndicateMissedEventsAndStatuses()
        {
            form.Text = AddVersionToFormName(formName);
            bool haveMissedEvents = false;
            if (NewEventsCount > 0)
            {
                form.Text += " (" + NewEventsCount + ")";
                haveMissedEvents = true;
                FlashWindow.Start(form);
                SetTrayNewEventIcon();
            }

            bool isTrayStatusEnabled = AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_TRAY_INDICATION_STATUS);
            bool isTaskbarStatusEnabled = AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_TASKBAR_INDICATION_STATUS);

            if ((NewStatusesCount > 0) )
            {
                haveMissedEvents = true;

                if (isTrayStatusEnabled)
                {
                    SetTrayNewEventIcon();
                }
                if (isTaskbarStatusEnabled)
                {
                    FlashWindow.Start(form);
                }
            }

            if (!haveMissedEvents)
            {
                FlashWindow.Stop(form);
                SetTrayCommontIcon();
            }
        }


        private string AddVersionToFormName(string name)
        {
            name += " " + AppConfigManager.GetKeyValue(Globals.Globals.TAG_VERSION);
            return name;
        }

        public void DealWithTrayIcon()
        {
            if ((NewEventsCount > 0) )
            {
                SetTrayNewEventIcon();
            }
            else
            {
                SetTrayCommontIcon();
            }
        }

        private void SetTrayNewEventIcon()
        {
            notifyIcon.Icon = new Icon(Properties.Resources.NewEventTray, new Size(32, 32));
        }

        public void SetTrayServerOfflineIcon()
        {
            notifyIcon.Icon = new Icon(Properties.Resources.ServerOfflineTray, new Size(32, 32));
        }

        private void SetTrayCommontIcon()
        {
            notifyIcon.Icon = form.Icon;
        }

        private void InitTrayIcon()
        {
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.BalloonTipText = "Программа работает в фоновом режиме.";
            notifyIcon.BalloonTipTitle = Properties.Resources.CLIENT_NAME; ;
            notifyIcon.Icon = form.Icon;
            notifyIcon.Text = Properties.Resources.CLIENT_NAME; ;
            notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            turnInOutController.TurnOutForm();
            IndicateMissedEventsAndStatuses();
        }

        public void HideForm()
        {
            turnInOutController.TurnInForm();
        }
    }
}
