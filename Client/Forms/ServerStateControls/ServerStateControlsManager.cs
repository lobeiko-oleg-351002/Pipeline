using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.ServerStateControls
{
    public class ServerStateControlsManager
    {
        private readonly ServerStateControls serverStateControls;

        public ServerStateControlsManager(ServerStateControls serverStateControls)
        {
            this.serverStateControls = serverStateControls;
        }

        public void SetControlsAccordingToServerOffline()
        {
            serverStateControls.ControllerSet.mainForm.Invoke(new Action(() =>
            {
                serverStateControls.ServerState.Text = Properties.Resources.SERVER_OFFLINE;
                serverStateControls.ServerState.ForeColor = Color.Red;
                serverStateControls.ControllerSet.mainFormControlsManager.DisableCreateEventButton();
                serverStateControls.ControllerSet.statusControlsManager.DisableStatusControls();
                serverStateControls.ControllerSet.indication.SetTrayServerOfflineIcon();
            }));
        }

        public void SetControlsAccordingToServerOnline()
        {
            serverStateControls.ControllerSet.mainForm.Invoke(new Action(() =>
            {
                serverStateControls.ServerState.Text = Properties.Resources.SERVER_ONLINE;
                serverStateControls.ServerState.ForeColor = Color.Black;
                serverStateControls.ControllerSet.mainFormControlsManager.EnableCreateEventButton();
                serverStateControls.ControllerSet.statusControlsManager.EnableApplyStatusButton();
                serverStateControls.ControllerSet.statusControlsManager.InitStatusesForSelectedEvent();
                serverStateControls.ControllerSet.indication.DealWithTrayIcon();
                SetUserFullnameOnLabel();
                serverStateControls.ControllerSet.eventManager.DeleteUserInRemovedEvents();
                
            }));
        }

        private void SetUserFullnameOnLabel()
        {
            serverStateControls.ControllerSet.mainForm.Invoke(new Action(() =>
            {
                serverStateControls.UserName.Text = serverStateControls.ControllerSet.client.GetUser().Fullname;
            }));
        }
    }
}
