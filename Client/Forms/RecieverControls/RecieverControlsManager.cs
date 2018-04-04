using BllEntities;
using Client.EventClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Client.Forms.RecieverControls
{
    public class RecieverControlsManager
    {
        private readonly RecieverControls recieverControls;

        public RecieverControlsManager(RecieverControls recieverControls)
        {
            this.recieverControls = recieverControls;
            recieverControls.Recieved.CheckedChanged += checkBox_CheckedChanged;
            recieverControls.Acquaint.Click += button_Click;
        }

        public void HandleDisplayingRecievers()
        {
            var Event = recieverControls.ControllerSet.SelectedEvent.EventData;
            var recievers = Event.RecieverLib.SelectedEntities;
            var User = recieverControls.ControllerSet.client.GetUser();

            if (Event.IsApproved == null)
            {
                if (EventHelper.AreUsersEqual(Event.Approver, User))
                {
                    if (recieverControls.ControllerSet.client.isServerOnline)
                    {
                        recieverControls.ControllerSet.approveControlsManager.ShowApproveControls();
                    }
                }
                else
                {
                    recieverControls.ControllerSet.dataGridControlsManager.SetApprovingWaitingMarkToSelectedRow();
                }
            }
            else
            {
                if (Event.IsApproved.Value == true)
                {
                    var Sender = recieverControls.ControllerSet.SelectedEvent.EventData.Sender;
                    if ((EventHelper.IsUserInChecklistByLogin(User, recievers)) || EventHelper.AreUsersEqual(User, Sender))
                    {
                        ShowChecklist();
                    }
                    else
                    {
                        if (recieverControls.ControllerSet.client.isServerOnline)
                        {
                            ShowAcquaintedCheckbox();
                        }
                    }
                }
                else
                {
                    recieverControls.ControllerSet.dataGridControlsManager.SetDisapproveMarkToSelectedRow();
                    recieverControls.ControllerSet.approveControlsManager.HideApproveControls();
                }
            }
            FillUserChecklist(recievers);
        }

        public void ShowChecklist()
        {
            recieverControls.GroupBoxForAcquaintButton.Visible = false;
            recieverControls.GroupBoxForRecievers.Visible = true;
            recieverControls.ControllerSet.approveControlsManager.HideApproveControls();
        }

        public void ShowAcquaintedCheckbox()
        {
            recieverControls.GroupBoxForAcquaintButton.Visible = true;
            recieverControls.GroupBoxForRecievers.Visible = false;
            recieverControls.ControllerSet.approveControlsManager.HideApproveControls();
        }

        public void HideChecklistAndCheckbox()
        {
            recieverControls.GroupBoxForAcquaintButton.Visible = false;
            recieverControls.GroupBoxForRecievers.Visible = false;
        }

        public void FillUserChecklist(List<BllSelectedUser> users)
        {
            recieverControls.Recievers.Items.Clear();
            AddUnacquaintedUsersToCheckList(users);
            AddAcquaintedUsersToCheckListAndSetTheirCount(users);
        }

        private void AddAcquaintedUsersToCheckListAndSetTheirCount(List<BllSelectedUser> users)
        {
            int acceptedEvents = 0;
            foreach (var item in users)
            {
                if (item.IsEventAccepted)
                {
                    AddUserToChecklist(item.Entity, true);
                    acceptedEvents++;
                }
            }
            recieverControls.GroupBoxForRecievers.Text = "Ознакомились " + acceptedEvents + " из " + users.Count;
        }

        private void AddUserToChecklist(BllUser user, bool isAccepted)
        {
            recieverControls.Recievers.Items.Add(user.Fullname);
            var addedItem = recieverControls.Recievers.Items[recieverControls.Recievers.Items.Count - 1];
            if (isAccepted)
            {
                addedItem.ForeColor = Color.Green;
            }
            else
            {
                addedItem.ForeColor = Color.Gray;
            }
        }

        private void AddUnacquaintedUsersToCheckList(List<BllSelectedUser> users)
        {
            foreach (var item in users)
            {
                if (!item.IsEventAccepted)
                {
                    AddUserToChecklist(item.Entity, false);
                }
            }
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (recieverControls.Recieved.Checked)
            {
                recieverControls.Acquaint.Enabled = true;
            }
            else
            {
                recieverControls.Acquaint.Enabled = false;
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            if (recieverControls.Recieved.Checked)
            {
                ShowChecklist();
                EventHelper.MarkRecieverInLib(recieverControls.ControllerSet.SelectedEvent.EventData.RecieverLib, recieverControls.ControllerSet.client.GetUser());
                recieverControls.ControllerSet.eventManager.AdmitEventAsAcquainted();
                FillUserChecklist(recieverControls.ControllerSet.SelectedEvent.EventData.RecieverLib.SelectedEntities);
                recieverControls.ControllerSet.statusControlsManager.EnableStatusControls();                
            }
        }

        public void UncheckCheckBox()
        {
            recieverControls.Recieved.Checked = false;
        }
    }
}
