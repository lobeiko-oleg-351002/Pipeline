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

        public void HandleDisplayingRecievers(int row)
        {
            recieverControls.ControllerSet.reconcileControlsManager.HideReconcileControls();
            var Event = recieverControls.ControllerSet.SelectedEvent.EventData;
            var recievers = Event.RecieverLib.SelectedEntities;
            var User = recieverControls.ControllerSet.client.GetUser();

            if (Event.ReconcilerLib != null)
            {
                var reconcilers = Event.ReconcilerLib.SelectedEntities;
                if (EventHelper.IsEventReconciled(Event))
                {
                    if (Event.Approver == null)
                    {
                        if (EventHelper.IsEventAcceptedByUser(Event, User))
                        {
                            AddRecieversAndReconcilersToChecklist(Event);
                        }
                        else
                        {
                            ShowAcquaintedCheckbox();
                        }
                    }
                    else
                    {
                        HandleApprovingControls(Event, User, recievers);
                    }
                }
                else
                {
                    recieverControls.ControllerSet.dataGridControlsManager.dataGridPopulationManager.SetReconcilingMark(row);
                    if (!EventHelper.HasUserReconciled(Event, User))
                    {
                        recieverControls.ControllerSet.reconcileControlsManager.ShowReconcileControls();
                    }
                    else
                    {
                        FillReconcilerChecklist(reconcilers);
                        ShowChecklist();
                    }
                 }
            }
            else
            {
                HandleApprovingControls(Event, User, recievers);
                AddRecieversToCheckList(recievers);
                SetAcquaintedCount(recievers, null);
            }

        }

        private void HandleApprovingControls(BllEvent Event, BllUser User, List<BllSelectedUser> recievers)
        {
            if (Event.IsApproved == null)
            {
                ShowApproveAwaiting(Event, User);
            }
            else
            {
                ShowApprovingControls(Event, User, recievers);
            }           
        }

        private void FillReconcilerChecklist(List<BllSelectedUserReconciler> users)
        {
            recieverControls.ControllerSet.mainForm.Invoke(new Action(() =>
            {
                recieverControls.Recievers.Items.Clear();
            }));

            foreach (var item in users)
            {
                if (item.IsEventReconciled == null)
                {
                    AddUserToChecklist(item.Entity, Color.LightGray);
                }
                else
                {
                    if (item.IsEventReconciled.Value)
                    {
                        AddUserToChecklist(item.Entity, Color.Blue);
                    }
                    else
                    {
                        AddUserToChecklist(item.Entity, Color.Red);
                    }
                }
            }
            SetAcquaintedCount(null, users);
        }

        public void AddRecieversAndReconcilersToChecklist(BllEvent Event)
        {
            var recievers = Event.RecieverLib.SelectedEntities;
            List<BllSelectedUserReconciler> reconcilers = null;
            AddRecieversToCheckList(recievers);
            if (Event.ReconcilerLib != null)
            {
                reconcilers = Event.ReconcilerLib.SelectedEntities;
                AddReconcilersToChecklist(reconcilers);
            }

            SetAcquaintedCount(recievers, reconcilers);
            ShowChecklist();
        }

        private void ShowApproveAwaiting(BllEvent Event, BllUser User)
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
                recieverControls.ControllerSet.reconcileControlsManager.HideReconcileControls();
                recieverControls.ControllerSet.dataGridControlsManager.SetApprovingWaitingMarkToSelectedRow();
            }
        }

        private void ShowApprovingControls(BllEvent Event, BllUser User, List<BllSelectedUser> recievers)
        {
            if (Event.IsApproved.Value == true)
            {
                var Sender = recieverControls.ControllerSet.SelectedEvent.EventData.Sender;
                if ((EventHelper.DidUserAcquaintByLogin(User, recievers)) || EventHelper.AreUsersEqual(User, Sender) || EventHelper.IsUserReconciler(User, Event.ReconcilerLib))
                {
                    AddRecieversAndReconcilersToChecklist(Event);
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

        public void ShowChecklist()
        {
            recieverControls.ControllerSet.mainForm.Invoke(new Action(() =>
            {
                recieverControls.GroupBoxForAcquaintButton.Visible = false;
                recieverControls.GroupBoxForRecievers.Visible = true;
            }));

            recieverControls.ControllerSet.approveControlsManager.HideApproveControls();
            recieverControls.ControllerSet.reconcileControlsManager.HideReconcileControls();
        }

        public void ShowAcquaintedCheckbox()
        {
            recieverControls.ControllerSet.mainForm.Invoke(new Action(() =>
            {
                recieverControls.GroupBoxForAcquaintButton.Visible = true;
                recieverControls.GroupBoxForRecievers.Visible = false;
            }));
            recieverControls.ControllerSet.approveControlsManager.HideApproveControls();
            recieverControls.ControllerSet.reconcileControlsManager.HideReconcileControls();
        }

        public void HideChecklistAndCheckbox()
        {
            recieverControls.ControllerSet.mainForm.Invoke(new Action(() =>
            {
                recieverControls.GroupBoxForAcquaintButton.Visible = false;
                recieverControls.GroupBoxForRecievers.Visible = false;
            }));
        }

        public void AddRecieversToCheckList(List<BllSelectedUser> users)
        {
            recieverControls.ControllerSet.mainForm.Invoke(new Action(() =>
            {
                recieverControls.Recievers.Items.Clear();
            }));
            AddUnacquaintedUsersToCheckList(users);
            AddAcquaintedUsersToCheckList(users);
        }

        public void AddReconcilersToChecklist(List<BllSelectedUserReconciler> users)
        {
            foreach (var item in users)
            {
                AddUserToChecklist(item.Entity, Color.Green);
            }
        }

        private void AddAcquaintedUsersToCheckList(List<BllSelectedUser> users)
        {
            foreach (var item in users)
            {
                if (item.IsEventAccepted)
                {
                    AddUserToChecklist(item.Entity, Color.Green);
                }
            }
        }

        private void SetAcquaintedCount(List<BllSelectedUser> recievers, List<BllSelectedUserReconciler> reconcilers)
        {
            int acquaintedCount = 0;
            int reconciledCount = 0;
            int reconcilersCount = 0;
            int recieversCount = 0;
            if (recievers != null)
            {
                recieversCount = recievers.Count;
                foreach (var item in recievers)
                {
                    if (item.IsEventAccepted)
                    {
                        acquaintedCount++;
                    }
                }
            }
            if (reconcilers != null)
            {
                reconcilersCount = reconcilers.Count;
                foreach (var item in reconcilers)
                {
                    if (item.IsEventReconciled != null)
                    {
                        reconciledCount++;
                    }
                }
            }
            recieverControls.ControllerSet.mainForm.Invoke(new Action(() =>
            {
                recieverControls.GroupBoxForRecievers.Text = "Ознакомились " + (acquaintedCount + reconciledCount) + " из " + (reconcilersCount + recieversCount);
            }));
        }

        private void AddUserToChecklist(BllUser user, Color color)
        {
            recieverControls.ControllerSet.mainForm.Invoke(new Action(() =>
            {
                recieverControls.Recievers.Items.Add(user.Fullname);
                var addedItem = recieverControls.Recievers.Items[recieverControls.Recievers.Items.Count - 1];
                addedItem.ForeColor = color;
            }));
        }

        private void AddUnacquaintedUsersToCheckList(List<BllSelectedUser> users)
        {
            foreach (var item in users)
            {
                if (!item.IsEventAccepted)
                {
                    AddUserToChecklist(item.Entity, Color.LightGray);
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
            try
            {
                if (recieverControls.Recieved.Checked)
                {
                    var Event = recieverControls.ControllerSet.SelectedEvent.EventData;
                    EventHelper.MarkRecieverInLib(Event.RecieverLib, recieverControls.ControllerSet.client.GetUser());
                    recieverControls.ControllerSet.eventManager.AdmitEventAsAcquainted();
                    AddRecieversAndReconcilersToChecklist(Event);
                    recieverControls.ControllerSet.statusControlsManager.EnableStatusControls();
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteMessage("buttonEventAcquainted", ex.Message, "");
            }
        }

        public void UncheckCheckBox()
        {
            recieverControls.Recieved.Checked = false;
        }
    }
}
