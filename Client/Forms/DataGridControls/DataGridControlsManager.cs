using BllEntities;
using Client.EventClasses;
using Client.Misc.FileService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.DataGridControls
{
    public class DataGridControlsManager
    {
        public readonly DataGridControls dataGridControls;
        public readonly DataGridPopulationManager dataGridPopulationManager;

        public DataGridControlsManager(DataGridControls dataGridControls)
        {
            this.dataGridControls = dataGridControls;
            dataGridControls.DataGrid.CellContentClick += dataGridView_CellContentClick;
            dataGridControls.DataGrid.ColumnHeaderMouseClick += dataGridView_ColumnHeaderMouseClick;
            dataGridControls.DataGrid.RowStateChanged += dataGridView_RowStateChanged;
            dataGridPopulationManager = new DataGridPopulationManager(dataGridControls.DataGrid, dataGridControls.ParentFormControl);
        }

        public int GetSelectedRowIndex()
        {
            return dataGridControls.DataGrid.SelectedRows[0].Index;
        }

        public void ClearSelection()
        {
            dataGridControls.DataGrid.ClearSelection();
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                var lib = dataGridControls.ControllerSet.SelectedEvent.EventData.FilepathLib;
                DownloadAndLaunchFiles(lib);
            }
        }

        private void DownloadAndLaunchFiles(BllFilepathLib lib)
        {
            foreach (var name in lib.Entities)
            {
                try
                {
                    FileDownloader fileDownloader = new FileDownloader();
                    Process.Start(fileDownloader.CheckFileSizeAndDownloadFile(new FilePathMap(name.Path, lib.FolderName)));
                }
                catch
                {
                    MessageBox.Show(Properties.Resources.CANNOT_OPEN_FILE, name.Path);
                }
            }
        }

        public bool IsAnyRowSelected()
        {
            return GetSelectedRowIndex() != -1;
        }

        public void UpdateSelectedEvent(BllEvent source, int rowNum)
        {
            try
            {
                dataGridPopulationManager.ClearApprovedColumn(rowNum);
                if (dataGridControls.DataGrid.SelectedRows.Count > 0)
                {
                    if (GetSelectedRowIndex() == rowNum)
                    {
                        dataGridControls.ControllerSet.statusControlsManager.PopulateStatusDataGridUsingStatusLib(source.StatusLib);
                        dataGridControls.ControllerSet.recieverControlsManager.HandleDisplayingRecievers(rowNum);
                        HandleStatusChanging();
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void SetDisapproveMark(int rowNum)
        {
            dataGridPopulationManager.SetDissaproveMark(dataGridControls.DataGrid.Rows[rowNum]);
        }

        public void SetDisapproveMarkToSelectedRow()
        {
            var row = dataGridControls.DataGrid.SelectedRows[0];
            dataGridPopulationManager.SetDissaproveMark(row);
        }

        public void SetApprovingWaitingMarkToSelectedRow()
        {
            var row = dataGridControls.DataGrid.SelectedRows[0];
            dataGridPopulationManager.SetApprovingWaitingMark(row);
        }

        public void SetApproverToRow(int rowNum)
        {
            try
            {
                var row = dataGridControls.DataGrid.Rows[rowNum];
                dataGridPopulationManager.SetApproverToRow(row, dataGridControls.ControllerSet.eventManager.GetEventByRowNum(rowNum).EventData.Approver.Fullname);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void SetApproverToSelectedRow()
        {
            var row = dataGridControls.DataGrid.SelectedRows[0];
            dataGridPopulationManager.SetApproverToRow(row, dataGridControls.ControllerSet.SelectedEvent.EventData.Approver.Fullname);
        }

        public void HandleStatusChanging()
        {
            var Event = dataGridControls.ControllerSet.SelectedEvent.EventData;
            if (Event.StatusLib.SelectedEntities.Count > 0)
            {
                if (StatusesForOwner.IsStatusForOwner(EventHelper.GetCurrentEventStatus(Event)))
                {
                    if (!EventHelper.AreUsersEqual(dataGridControls.ControllerSet.client.GetUser(), Event.Sender))
                    {
                        dataGridControls.ControllerSet.statusControlsManager.DisableStatusControls();
                        dataGridControls.ControllerSet.recieverControlsManager.HideChecklistAndCheckbox();
                    }
                }
            }

            bool hasCurrentUserAcceptedEvent = EventHelper.IsEventAcceptedByUser(Event, dataGridControls.ControllerSet.client.GetUser());
            bool isEventApproved = Event.IsApproved != null ? Event.IsApproved.Value : false;
            bool isUserSender = EventHelper.AreUsersEqual(dataGridControls.ControllerSet.client.GetUser(), Event.Sender); 
            if (!hasCurrentUserAcceptedEvent || (!isEventApproved && !isUserSender))
            {
                dataGridControls.ControllerSet.statusControlsManager.DisableStatusControls();
                dataGridControls.ControllerSet.mainFormControlsManager.DisableSendOnEventButton();
            }
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridControls.ControllerSet.eventManager.SortEventsUsingHeader(dataGridControls.DataGrid.Columns[e.ColumnIndex].HeaderText);
        }


        private void dataGridView_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            dataGridControls.ControllerSet.mainFormControlsManager.DisableSendOnEventButton();
            if (e.StateChanged != DataGridViewElementStates.Selected) return;
            if (dataGridControls.DataGrid.SelectedRows.Count == 0)
            {
                dataGridControls.ControllerSet.mainFormControlsManager.ClearDataControls();
                return;
            }
            dataGridControls.ControllerSet.mainFormControlsManager.EnableSendOnEventButton();
            UpdateSelectedEventUsingEventManager();
            dataGridControls.ControllerSet.recieverControlsManager.UncheckCheckBox();

            if (StatusesForOwner.IsEventStateRemoved(dataGridControls.ControllerSet.SelectedEvent) )
            {
                dataGridControls.ControllerSet.mainFormControlsManager.EnableDeleteEventButton();
                dataGridControls.ControllerSet.mainFormControlsManager.DisableSendOnEventButton();
            }
            else
            {
                dataGridControls.ControllerSet.mainFormControlsManager.DisableDeleteEventButton();
            }

            SetSelectedEventToControls();

            dataGridControls.ControllerSet.recieverControlsManager.HandleDisplayingRecievers(e.Row.Index);
            HandleStatusChanging();
        }

        private void SetSelectedEventToControls()
        {
            dataGridControls.ControllerSet.staticControlsManager.PopulateTextBoxesUsingEvent(dataGridControls.ControllerSet.SelectedEvent.EventData);
            dataGridControls.ControllerSet.statusControlsManager.PopulateStatusDataGridUsingStatusLib(dataGridControls.ControllerSet.SelectedEvent.EventData.StatusLib);
            dataGridControls.ControllerSet.fileControlsManager.PopulateFileListBoxUsingFilepathLib(dataGridControls.ControllerSet.SelectedEvent.EventData.FilepathLib);
            dataGridControls.ControllerSet.noteControlsManager.SetEventNoteUsingCellValue(dataGridControls.ControllerSet.SelectedEvent.Note);
            dataGridControls.ControllerSet.statusControlsManager.SelectBlankStatus();
            dataGridControls.ControllerSet.statusControlsManager.AddStatusesForSenderAccordingToSender(dataGridControls.ControllerSet.SelectedEvent.EventData.Sender);
        }

        public void UpdateSelectedEventUsingEventManager()
        {
            dataGridControls.ControllerSet.SelectedEvent = dataGridControls.ControllerSet.eventManager.SetSelectedEventUsingNum(dataGridControls.DataGrid.SelectedRows[0].Index);
        }
    }
}
