using BllEntities;
using Client.EventClasses;
using Client.EventClasses.Events;
using Client.Forms;
using Client.Misc;
using Client.ServerManager;
using Client.ServerManager.Interface;
using Globals;
using Microsoft.Win32;
using ServerInterface;
using ServiceChannelManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Client
{
    public partial class MainForm : ParentForm
    {        
        const string STATUS_NOT_CHANGED = "Статус не изменён";
        const string DATE_FORMAT = "dd.MM.yyyy";
        const string TIME_FORMAT = "HH:mm";

        AddEventForm addEventForm = null;
        UiEventManager eventManager = null;
        public Indication indication = null;
        ClientLauncher client = null;
        UiEvent SelectedEvent = null;
        List<BllStatus> AvailableStatusesForSelectedEvent = new List<BllStatus>();

        int SelectedRowIndex;
        bool noteTextBoxChangesProgramatically = false;
        static bool isAppClosed = false;

        //for tests
        public MainForm(string pathToClient)
        {
            InitializeComponent();
            AppConfigManager.SetExecutablePath(pathToClient);
        }

        public MainForm()
        {
            InitializeComponent();
            AppConfigManager.SetExecutablePath(Application.ExecutablePath);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SystemEvents.SessionEnded += new SessionEndedEventHandler(SaveAndExit);
            eventManager = new UiEventManager(dataGridView1, this);
            indication = new Indication(this);
            InitializeAppProperties();
            try
            {
                client = new ClientLauncher(eventManager, this);
                client.Launch();
                eventManager.client = client;
                StatusesForOwner.Init(client.GetServerInstance());
                //var list = client.GetServerInstance().server.GetEventsForUser(client.GetUser());
                eventManager.GetEventList();
                eventManager.HideClosedEventsAccordingToConfigValue();
                eventManager.StartCheckoutsOnTimer();
            }
            catch (UserIsNullException) //user has not logged in
            {
                ExitApp();
            }
        }

        

        public void SetControlsAccordingToServerOffline()
        {
            Invoke(new Action(() =>
            {
                label4.Text = Properties.Resources.SERVER_OFFLINE;
                label4.ForeColor = Color.Red;
                создатьСобытиеToolStripMenuItem.Enabled = false;
                button1.Enabled = false;
                indication.SetTrayServerOfflineIcon();
            }));
        }

        public void SetControlsAccordingToServerOnline()
        {
            Invoke(new Action(() =>
            {
                label4.Text = Properties.Resources.SERVER_ONLINE;
                label4.ForeColor = Color.Black;
                создатьСобытиеToolStripMenuItem.Enabled = true;
                if (comboBox1.SelectedIndex > 0)
                {
                    button1.Enabled = true;
                }
                indication.DealWithTrayIcon();
                InitStatusesForSelectedEvent();
                SetUserFullnameOnLabel();
                eventManager.DeleteUserInRemovedEvents();
            }));           
        }

        public BllUser GetCurrentUser()
        {
            return client.GetUser();
        }

        private bool AreUserStatusesSet()
        {
            return comboBox1.Items.Count == 1;
        }

        private void PopulateTextBoxesUsingEvent(BllEvent Event)
        {
            textBox2.Text = Event.Sender.Fullname;
            textBox3.Text = Event.Name;
            textBox4.Text = Event.Date.ToString(DATE_FORMAT);
            textBox5.Text = Event.Date.ToString(TIME_FORMAT);
        }

        private void PopulateFileListBoxUsingFilepathLib(BllFilepathLib lib)
        {
            listBox2.Items.Clear();
            foreach (var filename in lib.Entities)
            {
                listBox2.Items.Add(filename.Path);
            }
        }

        private void SetEventDescription(string description)
        {
            richTextBox1.Text = description;
        }

        private void SetEventNoteUsingCellValue(string note)
        {
            richTextBox2.Text = note;
            richTextBox2.Enabled = true;
        }

        private void AddStatusesForSenderAccordingToSender(BllUser sender)
        {
            var statusesForOwner = StatusesForOwner.GetStatusesForOwner();
            foreach(var status in statusesForOwner)
            {
                comboBox1.Items.Remove(status.Name);
                AvailableStatusesForSelectedEvent.Remove(status);
                if (AreUsersEqual(client.GetUser(), sender))
                {
                    comboBox1.Items.Add(status.Name);
                    AvailableStatusesForSelectedEvent.Add(status);
                }
            }
        }

        private bool AreUsersEqual(BllUser user1, BllUser user2)
        {
            if (user1.Login == user2.Login)
            {
                return true;
            }
            return false;
        }

        public void SelectBlankStatus()
        {
            comboBox1.SelectedIndex = 0;
            comboBox1.Enabled = true;
            button1.Enabled = false;
        }

        private void SetSelectedEventToControls()
        {
            PopulateTextBoxesUsingEvent(SelectedEvent.EventData);
            PopulateStatusDataGridUsingStatusLib(SelectedEvent.EventData.StatusLib);
            PopulateFileListBoxUsingFilepathLib(SelectedEvent.EventData.FilepathLib);
            SetEventDescription(SelectedEvent.EventData.Description);
            SetEventNoteUsingCellValue(SelectedEvent.Note);
            SelectBlankStatus();
            AddStatusesForSenderAccordingToSender(SelectedEvent.EventData.Sender);         
        }

        public void AddStatusToDataGrid(string name, DateTime date)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView2);
            row.Cells[0].Value = name;
            row.Cells[1].Value = date;
            dataGridView2.Rows.Add(row);
        } 

        private void AddAppShortcutToStartup()
        {
            const string launcher = "Launcher.exe";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                //if (key.GetValue(Properties.Resources.CLIENT_NAME) == null)
                //{
                string currentLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                key.SetValue(Properties.Resources.CLIENT_NAME, "\"" + currentLocation + "\\" + launcher + "\"");
               // }
            }
        }

        private void AddBlankStatusToComboBox()
        {
            comboBox1.Items.Add(STATUS_NOT_CHANGED);
        }

        private void SetOpenFileLocationCheckBoxAccordingToConfigValue()
        {
            checkBox2.Checked = AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_OPEN_FILE_LOCATION);
        }

        private void TurnAppStartupInTrayAccordingToConfigValue()
        {
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_STARTUP_TRAY))
            {
                Close();
            }
        }

        private void InitializeAppProperties()
        {
            AddAppShortcutToStartup();
            SetOpenFileLocationCheckBoxAccordingToConfigValue();
            AddBlankStatusToComboBox();
            TurnAppStartupInTrayAccordingToConfigValue();
            SetControlsAccordingToServerOffline();
        }

        private void SetAppControlsAccordingToOfflineServer()
        {
            SetControlsAccordingToServerOffline();
        }



        private void SetDefaultSelectedRow()
        {
            if (dataGridView1.Rows.Count > 0)
            {
                if (dataGridView1.CurrentRow != null)
                {
                    dataGridView1.CurrentRow.Selected = false;
                }
            }
        }

        private void InitStatusesForSelectedEvent()
        {
            if (client.GetUser().StatusLib != null)
            {
                AvailableStatusesForSelectedEvent.Clear();
                comboBox1.Items.Clear();
                AddBlankStatusToComboBox();
                
                foreach (var item in client.GetUser().StatusLib.SelectedEntities)
                {
                    AvailableStatusesForSelectedEvent.Add(item.Entity);
                    comboBox1.Items.Add(item.Entity.Name);
                }
            }
        }

        private void SetUserFullnameOnLabel()
        {
            Invoke(new Action(() =>
            {
                label9.Text = client.GetUser().Fullname;
            }));
        }    

        public void ExitApp()
        {
            Application.Exit();
            Close();
        }

        private BllEvent GetNewEventUsingAddEventForm()
        {
            addEventForm = new AddEventForm(client.GetServerInstance(), client.GetUser());
            addEventForm.ShowDialog();
            return addEventForm.Event;
        }

        private bool IsAnyRowSelected()
        {
            return SelectedRowIndex != -1;
        }

        private void создатьСобытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client.PingServerAndIndicateHisStateOnControls();
            if (client.isServerOnline)
            {
                var createdEvent = GetNewEventUsingAddEventForm();
                if (createdEvent != null)
                {
                    eventManager.AddNewEventAndSerialize(createdEvent);
                    if (IsAnyRowSelected() == false)
                    {
                        dataGridView1.ClearSelection();
                    }
                }
                addEventForm = null;
            }
        }

        private void DownloadAndLaunchFiles(BllFilepathLib lib)
        {
            foreach (var name in lib.Entities)
            {
                try
                {
                    Process.Start(FileDownloader.CheckFileSizeAndDownloadFile(name.Path, lib.FolderName));
                }
                catch
                {
                    MessageBox.Show(Properties.Resources.CANNOT_OPEN_FILE, name.Path);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {            
            var senderGrid = (DataGridView)sender;
            
            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                var lib = SelectedEvent.EventData.FilepathLib;
                DownloadAndLaunchFiles(lib);
            }
        }

        private void EnableStatusControls()
        {
            comboBox1.Enabled = true;
            button1.Enabled = true;
        }

        private void DisableStatusControls()
        {
            comboBox1.Enabled = false;
            button1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.PingServerAndIndicateHisStateOnControls();
            
            if (client.isServerOnline && IsAnyRowSelected())
            {
                eventManager.AddStatusToSelectedEvent(AvailableStatusesForSelectedEvent[comboBox1.SelectedIndex - 1]);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                button1.Enabled = false;
            }
            else
            {
                if (client.isServerOnline)
                {
                    button1.Enabled = true;
                }
            }

        }

        public void UpdateSelectedEvent(BllEvent source, int rowNum)  //confirmed
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (SelectedRowIndex == rowNum)
                {
                    Invoke(new Action(() =>
                    {
                        PopulateStatusDataGridUsingStatusLib(source.StatusLib);
                        FillUserChecklist(source.RecieverLib.SelectedEntities);
                        HandleStatusChanging();
                    }));
                }
            }
        }

        private void EnableSendOnEventButton()
        {
            переслатьСобытиеToolStripMenuItem.Enabled = true;
        }

        private void DisableSendOnEventButton()
        {
            переслатьСобытиеToolStripMenuItem.Enabled = false;
        }

        private void EnableDeleteEventButton()
        {
            удалитьСобытиеToolStripMenuItem.Enabled = true;
        }

        private void DisableDeleteEventButton()
        {
            удалитьСобытиеToolStripMenuItem.Enabled = false;
        }

        private void PopulateStatusDataGridUsingStatusLib(BllStatusLib lib) //confirmed
        {
            dataGridView2.Rows.Clear();
            foreach(var item in lib.SelectedEntities)
            {
                AddStatusToDataGrid(item.Entity.Name, item.Date);
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }            

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isAppClosed)
            {
                e.Cancel = true;
                indication.HideForm();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                button2.Enabled = true;
            }
            else
            {
                button2.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                ShowChecklist();
                MarkRecieverInLib(SelectedEvent.EventData.RecieverLib);
                eventManager.AdmitEventAsAcquainted();
                SelectedEvent = eventManager.GetSelectedEvent(SelectedRowIndex);
                FillUserChecklist(SelectedEvent.EventData.RecieverLib.SelectedEntities);
                EnableStatusControls();
            }
        }

        private void MarkRecieverInLib(BllUserLib RecieverLib)
        {
            foreach (var item in RecieverLib.SelectedEntities)
            {
                if (AreUsersEqual(item.Entity, client.GetUser()))
                {
                    item.IsEventAccepted = true;
                    break;
                }
            }
        }

        private bool IsUserInChecklistByLogin(BllUser user, List<BllSelectedUser> list)
        {
            foreach (var item in list)
            {
                if (item.Entity.Login == user.Login)
                {
                    if (item.IsEventAccepted)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        private void ShowChecklist()
        {
            groupBox4.Visible = false;
            groupBox1.Visible = true;
        }

        private void ShowAcquaintedCheckbox()
        {
            groupBox4.Visible = true;
            groupBox1.Visible = false;
        }

        private void HideChecklistAndCheckbox()
        {
            groupBox4.Visible = false;
            groupBox1.Visible = false;
        }

        private void AddUserToChecklist(BllUser user, bool isAccepted) //confirmed
        {
            listView1.Items.Add(user.Fullname);
            var addedItem = listView1.Items[listView1.Items.Count - 1];
            if (isAccepted)
            {
                addedItem.ForeColor = Color.Green;
            }
            else
            {
                addedItem.ForeColor = Color.Gray;
            }
        }

        private void AddUnacquaintedUsersToCheckList(List<BllSelectedUser> users) //confirmed
        {
            foreach (var item in users)
            {
                if (!item.IsEventAccepted)
                {
                    AddUserToChecklist(item.Entity, false);
                }
            }
        }

        private void AddAcquaintedUsersToCheckListAndSetTheirCount(List<BllSelectedUser> users) //confirmed
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
            groupBox1.Text = "Ознакомились " + acceptedEvents + " из " + users.Count;
        }

        private void FillUserChecklist(List<BllSelectedUser> users) //confirmed
        {
            listView1.Items.Clear();
            AddUnacquaintedUsersToCheckList(users);
            AddAcquaintedUsersToCheckListAndSetTheirCount(users);
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void listBox2_DoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox2.SelectedIndex >= 0)
            {
                string filename = SelectedEvent.EventData.FilepathLib.Entities[listBox2.SelectedIndex].Path;
                string foldername = SelectedEvent.EventData.FilepathLib.FolderName;
                try
                {
                    if (checkBox2.Checked == false)
                    {
                        Process.Start(FileDownloader.CheckFileSizeAndDownloadFile(filename, foldername));
                    }
                    else
                    {
                        string path = FileDownloader.CheckFileSizeAndDownloadFile(filename, foldername);
                        Process.Start("explorer.exe", "/select, \"" + path + "\"");
                    }

                }
                catch(Exception ex)
                {
                    MessageBox.Show(Properties.Resources.CANNOT_OPEN_FILE, filename);
                }
            }
        }

        private void удалитьСобытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedRowIndex >= 0)
            {
                eventManager.RemoveEvent(SelectedRowIndex);
                if (dataGridView1.DisplayedRowCount(true) == 0)
                {
                    DisableDeleteEventButton();
                    ClearDataControls();
                }
            }
        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            DisableSendOnEventButton();
            if (e.StateChanged != DataGridViewElementStates.Selected) return;
            if (dataGridView1.SelectedRows.Count == 0)
            {
                ClearDataControls();
                return;
            }
            SelectedRowIndex = dataGridView1.SelectedRows[0].Index;
            EnableSendOnEventButton();
            SelectedEvent = eventManager.GetSelectedEvent(SelectedRowIndex);
            checkBox1.Checked = false;

            if (SelectedEvent.EventState == EventStates.DeletedEvent || SelectedEvent.EventState == EventStates.ClosedEvent)
            {
                EnableDeleteEventButton();
                DisableSendOnEventButton();
            }
            else
            {
                DisableDeleteEventButton();
            }
          
            SetSelectedEventToControls();

            PopulateRecievers();
            HandleStatusChanging();
        }

        private void PopulateRecievers()
        {
            var recievers = SelectedEvent.EventData.RecieverLib.SelectedEntities;
            var User = client.GetUser();
            if ((IsUserInChecklistByLogin(User, recievers)) || (User.Login == SelectedEvent.EventData.Sender.Login))
            {
                ShowChecklist();
                FillUserChecklist(recievers);
            }
            else
            {
                if (client.isServerOnline)
                {
                    ShowAcquaintedCheckbox();
                }
            }
        }


        private void HandleStatusChanging()  //fix
        {
            if (SelectedEvent.EventData.StatusLib.SelectedEntities.Count > 0)
            {
                if (StatusesForOwner.IsStatusForOwner(EventHelper.GetCurrentEventStatus(SelectedEvent.EventData)))
                {
                    if (client.GetUser().Login != SelectedEvent.EventData.Sender.Login)
                    {
                        DisableStatusControls();
                        HideChecklistAndCheckbox();
                    }
                }
            }
            if (!EventHelper.IsEventAcceptedByUser(SelectedEvent.EventData, client.GetUser()))
            {
                DisableStatusControls();
            }
        }

        private void ClearDataControls()
        {
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            richTextBox1.Text = "";
            dataGridView2.Rows.Clear();
            comboBox1.SelectedIndex = 0;
            DisableStatusControls();
            DisableSendOnEventButton();
            listBox2.Items.Clear();
            checkBox1.Checked = false;
            HideChecklistAndCheckbox();
            удалитьСобытиеToolStripMenuItem.Enabled = false;
            ClearNoteTextBox();
            richTextBox2.Enabled = false;
        }

        private void ClearNoteTextBox()
        {
            noteTextBoxChangesProgramatically = true;
            richTextBox2.Text = "";
            noteTextBoxChangesProgramatically = false;
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            bool prevHideClosed = AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_HIDE_CLOSED);
            int prevHideAllowance = AppConfigManager.GetIntKeyValue(Properties.Resources.TAG_HIDE_ALLOWANCE);
            settings.ShowDialog();
            if (prevHideClosed != AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_HIDE_CLOSED))
            {
                if (prevHideClosed)
                {
                    eventManager.ShowClosedEvents();
                }
                else
                {
                    eventManager.HideClosedEventsAccordingToConfigValue();
                }
            }
            else
            {
                if (prevHideClosed && (prevHideAllowance != AppConfigManager.GetIntKeyValue(Properties.Resources.TAG_HIDE_ALLOWANCE)))
                {
                    eventManager.HideClosedEventsAccordingToConfigValue();
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_OPEN_FILE_LOCATION, checkBox2.Checked.ToString());
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            eventManager.SortEventsUsingHeader(dataGridView1.Columns[e.ColumnIndex].HeaderText);
        }

        private void richTextBox2_Leave(object sender, EventArgs e)
        {
            eventManager.SerializeEvents();
        }

        private void переслатьСобытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client.PingServerAndIndicateHisStateOnControls();
            if (client.isServerOnline)
            {
                SendOnEventForm sendOnEventForm = new SendOnEventForm(client.GetServerInstance(), SelectedEvent.EventData, client.GetUser());
                sendOnEventForm.ShowDialog();
            }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (SelectedRowIndex >= 0 && !noteTextBoxChangesProgramatically)
            {
                eventManager.SetEventNote(richTextBox2.Text);
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isAppClosed = true;
            eventManager.SerializeEvents();
            Application.Exit();
        }

        public static void SaveAndExit(object sender, SessionEndedEventArgs e)
        {
            SystemEvents.SessionEnded -= new SessionEndedEventHandler(SaveAndExit);
            isAppClosed = true;
            Application.Exit();
        }
    }
}
