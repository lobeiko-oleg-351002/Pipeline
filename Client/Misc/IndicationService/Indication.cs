using Client.Misc.IndicationService;
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
        TrayIconController trayIconController;
        private TurnInOutController turnInOutController = null;

        public Indication(Form form)
        {
            NewEventsCount = 0;
            NewStatusesCount = 0;
            this.form = form;
            formName = Properties.Resources.CLIENT_NAME;

            IconSet iconSet = new IconSet(
                form.Icon,
                new Icon(Properties.Resources.NewEventTray, new Size(32, 32)),
                new Icon(Properties.Resources.ServerOfflineTray, new Size(32, 32)));

            trayIconController = new TrayIconController(iconSet, notifyIcon_MouseDoubleClick);
            turnInOutController = new TurnInOutController(form, trayIconController);
            IndicateMissedEventsAndStatuses();
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
                trayIconController.SetTrayNewEventIcon();
            }

            bool isTrayStatusEnabled = AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_TRAY_INDICATION_STATUS);
            bool isTaskbarStatusEnabled = AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_TASKBAR_INDICATION_STATUS);

            if ((NewStatusesCount > 0) )
            {
                haveMissedEvents = true;

                if (isTrayStatusEnabled)
                {
                    trayIconController.SetTrayNewEventIcon();
                }
                if (isTaskbarStatusEnabled)
                {
                    FlashWindow.Start(form);
                }
            }

            if (!haveMissedEvents)
            {
                FlashWindow.Stop(form);
                trayIconController.SetTrayCommontIcon();
            }
        }


        private string AddVersionToFormName(string name)
        {
            name += " " + AppConfigManager.GetKeyValue(Globals.Globals.TAG_VERSION);
            return name;
        }

        public void HideForm()
        {
            turnInOutController.TurnInForm();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            turnInOutController.TurnOutForm();
            IndicateMissedEventsAndStatuses();
        }

        public void DealWithTrayIcon()
        {
            trayIconController.DealWithTrayIcon(NewEventsCount);
        }

        public void SetTrayServerOfflineIcon()
        {
            trayIconController.SetTrayServerOfflineIcon();
        }
    }
}
