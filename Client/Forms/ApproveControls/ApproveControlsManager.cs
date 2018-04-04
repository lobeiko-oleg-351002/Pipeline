using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.Forms.ApproveControls
{
    public class ApproveControlsManager
    {
        ApproveControls approveControls;

        public ApproveControlsManager(ApproveControls approveControls)
        {
            this.approveControls = approveControls;
            approveControls.approveButton.Click += button_Click;
            approveControls.approveSwitch.Click += EnableButton;
            approveControls.disapproveSwitch.Click += EnableButton;
            approveControls.approveButton.Enabled = false;
        }

        private void button_Click(object sender, EventArgs e)
        {
            HideApproveControls();
            var SelectedEvent = approveControls.ControllerSet.eventManager.SelectedEvent;
            if (approveControls.approveSwitch.Checked)
            {
                SelectedEvent.EventData.IsApproved = true;
                approveControls.ControllerSet.recieverControlsManager.ShowChecklist();
                approveControls.ControllerSet.dataGridControlsManager.SetApproverToSelectedRow();
            }
            if (approveControls.disapproveSwitch.Checked)
            {
                SelectedEvent.EventData.IsApproved = false;
                approveControls.ControllerSet.dataGridControlsManager.SetDisapproveMarkToSelectedRow();
            }
            var user = SelectedEvent.EventData.RecieverLib.SelectedEntities.Single(en => en.Entity.Id == approveControls.ControllerSet.client.GetUser().Id);
            user.IsEventAccepted = true;
            approveControls.ControllerSet.recieverControlsManager.FillUserChecklist(SelectedEvent.EventData.RecieverLib.SelectedEntities);
            approveControls.ControllerSet.eventManager.AdmitEventAsApproved();
            approveControls.ControllerSet.statusControlsManager.EnableStatusControls();
        }

        public void HideApproveControls()
        {
            approveControls.groupBox.Visible = false; 
        }

        public void ShowApproveControls()
        {
            approveControls.groupBox.Visible = true;
        }

        private void EnableButton(object sender, EventArgs e)
        {
            approveControls.approveButton.Enabled = true;
        }
    }
}
