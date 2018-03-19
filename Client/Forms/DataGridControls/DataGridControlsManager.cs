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
            dataGridPopulationManager = new DataGridPopulationManager(dataGridControls.DataGrid);
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
            if (dataGridControls.DataGrid.SelectedRows.Count > 0)
            {
                if (GetSelectedRowIndex() == rowNum)
                {
                  //  Invoke(new Action(() =>
                  //  {
                    dataGridControls.ControllerSet.statusControlsManager.PopulateStatusDataGridUsingStatusLib(source.StatusLib);
                    dataGridControls.ControllerSet.recieverControlsManager.FillUserChecklist(source.RecieverLib.SelectedEntities);
                    HandleStatusChanging();
                   // }));
                }
            }
        }

        public void HandleStatusChanging()
        {
            if (dataGridControls.ControllerSet.SelectedEvent.EventData.StatusLib.SelectedEntities.Count > 0)
            {
                if (StatusesForOwner.IsStatusForOwner(EventHelper.GetCurrentEventStatus(dataGridControls.ControllerSet.SelectedEvent.EventData)))
                {
                    if (!EventHelper.AreUsersEqual(dataGridControls.ControllerSet.client.GetUser(), dataGridControls.ControllerSet.SelectedEvent.EventData.Sender))
                    {
                        dataGridControls.ControllerSet.statusControlsManager.DisableStatusControls();
                        dataGridControls.ControllerSet.recieverControlsManager.HideChecklistAndCheckbox();
                    }
                }
            }
            if (!EventHelper.IsEventAcceptedByUser(dataGridControls.ControllerSet.SelectedEvent.EventData, dataGridControls.ControllerSet.client.GetUser()))
            {
                dataGridControls.ControllerSet.statusControlsManager.DisableStatusControls();
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
            //SelectedRowIndex = dataGridView1.SelectedRows[0].Index
            dataGridControls.ControllerSet.mainFormControlsManager.EnableSendOnEventButton();
            UpdateSelectedEventUsingEventManager();
            dataGridControls.ControllerSet.recieverControlsManager.UncheckCheckBox();

            if (StatusesForOwner.IsEventStateRemoved(dataGridControls.ControllerSet.SelectedEvent))
            {
                dataGridControls.ControllerSet.mainFormControlsManager.EnableDeleteEventButton();
                dataGridControls.ControllerSet.mainFormControlsManager.DisableSendOnEventButton();
            }
            else
            {
                dataGridControls.ControllerSet.mainFormControlsManager.DisableDeleteEventButton();
            }

            SetSelectedEventToControls();

            dataGridControls.ControllerSet.recieverControlsManager.PopulateRecievers();
            dataGridControls.ControllerSet.dataGridControlsManager.HandleStatusChanging();
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
