using BllEntities;
using Client.EventClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.StatusControls
{
    public class StatusControlsManager
    {
        private readonly StatusControls statusControls;

        const string STATUS_NOT_CHANGED = "Статус не изменён";

        List<BllStatus> AvailableStatusesForSelectedEvent = new List<BllStatus>();

        public StatusControlsManager(StatusControls statusControls)
        {
            this.statusControls = statusControls;
            statusControls.ChangeStatusButton.Click += button_Click;
            statusControls.AvailableStatuses.SelectedIndexChanged += comboBox_SelectedIndexChanged;
        }

        public void AddStatusToDataGrid(string name, DateTime date)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(statusControls.StatusTable);
            row.Cells[0].Value = name;
            row.Cells[1].Value = date;
            statusControls.StatusTable.Rows.Add(row);
        }

        public void AddBlankStatusToComboBox()
        {
            statusControls.AvailableStatuses.Items.Add(STATUS_NOT_CHANGED);
        }

        public bool AreUserStatusesSet()
        {
            return statusControls.AvailableStatuses.Items.Count == 1;
        }

        public void AddStatusesForSenderAccordingToSender(BllUser sender)
        {
            var statusesForOwner = StatusesForOwner.GetStatusesForOwner();
            foreach (var status in statusesForOwner)
            {
                statusControls.AvailableStatuses.Items.Remove(status.Name);
                AvailableStatusesForSelectedEvent.Remove(status);
                if (EventHelper.AreUsersEqual(statusControls.ControllerSet.client.GetUser(), sender))
                {
                    statusControls.AvailableStatuses.Items.Add(status.Name);
                    AvailableStatusesForSelectedEvent.Add(status);
                }
            }
        }

        public void SelectBlankStatus()
        {
            statusControls.AvailableStatuses.SelectedIndex = 0;
            statusControls.AvailableStatuses.Enabled = true;
            statusControls.ChangeStatusButton.Enabled = false;
        }

        public void InitStatusesForSelectedEvent()
        {
            if (statusControls.ControllerSet.client.GetUser().StatusLib != null)
            {
                AvailableStatusesForSelectedEvent.Clear();
                statusControls.AvailableStatuses.Items.Clear();
                AddBlankStatusToComboBox();

                foreach (var item in statusControls.ControllerSet.client.GetUser().StatusLib.SelectedEntities)
                {
                    AvailableStatusesForSelectedEvent.Add(item.Entity);
                    statusControls.AvailableStatuses.Items.Add(item.Entity.Name);
                }
            }
        }

        public void EnableStatusControls()
        {
            statusControls.AvailableStatuses.Enabled = true;
            statusControls.ChangeStatusButton.Enabled = true;
        }

        public void DisableStatusControls()
        {
            statusControls.AvailableStatuses.Enabled = false;
            statusControls.AvailableStatuses.SelectedIndex = 0;
            statusControls.ChangeStatusButton.Enabled = false;
        }

        public void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (statusControls.AvailableStatuses.SelectedIndex == 0)
            {
                statusControls.ChangeStatusButton.Enabled = false;
            }
            else
            {
                if (statusControls.ControllerSet.client.isServerOnline)
                {
                    statusControls.ChangeStatusButton.Enabled = true;
                }
            }
        }

        public void ClearControls()
        {
            statusControls.AvailableStatuses.SelectedIndex = 0;
            statusControls.StatusTable.ClearSelection();
        }

        public void EnableApplyStatusButton()
        {
            if (statusControls.AvailableStatuses.SelectedIndex > 0)
            {
                statusControls.ChangeStatusButton.Enabled = true;
            }
        }

        public void PopulateStatusDataGridUsingStatusLib(BllStatusLib lib)
        {
            statusControls.StatusTable.Rows.Clear();
            foreach (var item in lib.SelectedEntities)
            {
                AddStatusToDataGrid(item.Entity.Name, item.Date);
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            statusControls.ControllerSet.client.PingServerAndIndicateHisStateOnControls();

            if (statusControls.ControllerSet.client.isServerOnline)
            {
                var selectedStatus = AvailableStatusesForSelectedEvent[statusControls.AvailableStatuses.SelectedIndex - 1];
                statusControls.ControllerSet.eventManager.AddStatusToSelectedEventAndIndicateHim(selectedStatus);
            }
        }
    }
}
