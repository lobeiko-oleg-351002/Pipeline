using BllEntities;
using Client.EventClasses.Events;
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
            HandleApproveCheck(SelectedEvent);
            HandleDisapproveCheck(SelectedEvent);
            AcquaintSelectedUserInSelectedEvent(SelectedEvent);
            approveControls.ControllerSet.eventManager.AdmitEventAsApproved();
            approveControls.ControllerSet.statusControlsManager.EnableStatusControls();
        }

        private void HandleApproveCheck(UiEvent SelectedEvent)
        {
            if (approveControls.approveSwitch.Checked)
            {
                SelectedEvent.EventData.IsApproved = true;
                approveControls.ControllerSet.recieverControlsManager.AddRecieversAndReconcilersToChecklist(SelectedEvent.EventData);
                approveControls.ControllerSet.dataGridControlsManager.SetApproverToSelectedRow();
            }
        }

        private void HandleDisapproveCheck(UiEvent SelectedEvent)
        {
            if (approveControls.disapproveSwitch.Checked)
            {
                SelectedEvent.EventData.IsApproved = false;
                approveControls.ControllerSet.dataGridControlsManager.SetDisapproveMarkToSelectedRow();
            }
        }

        private void AcquaintSelectedUserInSelectedEvent(UiEvent SelectedEvent)
        {
            var user = SelectedEvent.EventData.RecieverLib.SelectedEntities.Single(en => en.Entity.Id == approveControls.ControllerSet.client.GetUser().Id);
            user.IsEventAccepted = true;
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
