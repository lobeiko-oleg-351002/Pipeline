using BllEntities;
using Client.EventClasses;
using Client.EventClasses.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.Forms.ReconcileControls
{
    public class ReconcileControlsManager
    {
        ReconcileControls reconcileControls;

        public ReconcileControlsManager(ReconcileControls reconcileControls)
        {
            this.reconcileControls = reconcileControls;
            reconcileControls.reconcileButton.Click += button_Click;
            reconcileControls.agreeSwitch.Click += EnableButton;
            reconcileControls.disagreeSwitch.Click += EnableButton;
            reconcileControls.reconcileButton.Enabled = false;
        }

        private void button_Click(object sender, EventArgs e)
        {
            var SelectedEvent = reconcileControls.ControllerSet.eventManager.SelectedEvent;
            HandleAgreeCheck(SelectedEvent);
            HandleDisagreeCheck(SelectedEvent);

            reconcileControls.ControllerSet.eventManager.AdmitAgreementInReconcilers();
            if (SelectedEvent.EventData.Approver == null)
            {
                reconcileControls.ControllerSet.statusControlsManager.EnableStatusControls();
            }
        }

        private void HandleAgreeCheck(UiEvent SelectedEvent)
        {
            if (reconcileControls.agreeSwitch.Checked)
            {
                EventHelper.AdmitReconciledUserInEvent(SelectedEvent.EventData, reconcileControls.ControllerSet.client.GetUser(), true);
            }
        }

        private void HandleDisagreeCheck(UiEvent SelectedEvent)
        {
            if (reconcileControls.disagreeSwitch.Checked)
            {
                EventHelper.AdmitReconciledUserInEvent(SelectedEvent.EventData, reconcileControls.ControllerSet.client.GetUser(), false);
            }
        }

        public void HideReconcileControls()
        {
            reconcileControls.ControllerSet.mainForm.Invoke(new Action(() =>
            {
                reconcileControls.groupBox.Visible = false;
            }));
        }

        public void ShowReconcileControls()
        {
            reconcileControls.ControllerSet.mainForm.Invoke(new Action(() =>
            {
                reconcileControls.groupBox.Visible = true;
            }));
        }

        private void EnableButton(object sender, EventArgs e)
        {
            reconcileControls.ControllerSet.mainForm.Invoke(new Action(() =>
            {
                reconcileControls.reconcileButton.Enabled = true;
            }));
        }
    }
}
