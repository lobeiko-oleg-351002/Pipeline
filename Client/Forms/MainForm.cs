using BllEntities;
using Client.Forms;
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
    public partial class MainForm : ParentForm, IClientCallBack
    {
        public MainForm()
        {
            InitializeComponent();
            ExecutablePath = Application.ExecutablePath;
        }

        //for tests
        public MainForm(string pathToClient)
        {
            InitializeComponent();
            ExecutablePath = pathToClient;
            
        }

        private class SortableColumn
        {
            public int Number { get; private set; }
            public int Direction { get; set; }
            public Func<bool> Sort { get; private set; }

            public SortableColumn(int Number, int Direction, Func<bool> Sort)
            {
                this.Number = Number;
                this.Direction = Direction;
                this.Sort = Sort;
            }
        }

        const string LOGIN_TAG = "login";
        const string PASSWORD_TAG = "password";
        
        const string STATUS_NOT_CHANGED = "Статус не изменён";
        const string IP_KEY = "hostIP";

        const int SENDER_COL_NUM = 0;
        const int NAME_COL_NUM = 1;
        const int DATE_COL_NUM = 2;
        const int TIME_COL_NUM = 3;
        const int STATUS_COL_NUM = 4;
        const int ATTRIBUTE_COL_NUM = 5;
        const int FILE_COL_NUM = 6;
        const int DESCRIPTION_COL_NUM = 7;
        const int NOTE_COL_NUM = 8;
        const int DIRECTION_MULTIPLIER = -1;

        const int PING_SLEEPTIME_MS = 10000;



        public IServerInstance server { get; private set; }

        BllUser User = null;
        List<BllEvent> EventSequence;
        List<BllStatus> AvailableStatusesForSelectedEvent = new List<BllStatus>();
        BllStatus StatusDeleted = null;
        BllStatus StatusClosed = null;
        SortableColumn CurrentSorting;

        SortableColumn SenderColumn;
        SortableColumn NameColumnSorting;
        SortableColumn DateColumn;
        SortableColumn StatusColumn;
        SortableColumn AttributeColumn;

        List<int> IndeciesOfNewEvents = new List<int>();
        List<int> IndeciesOfNewStatuses = new List<int>();
        List<int> IndeciesOfDeletedEvents = new List<int>();
        List<int> IndeciesOfClosedEvents = new List<int>();
        NotifyIcon notifyIcon = new NotifyIcon();
        
        AddEventForm addEventForm = null;

        int SelectedRowIndex;
        string FormText = Properties.Resources.CLIENT_NAME;

        public bool isServerOnline;
        bool isAppClosed = false;

        private void RunMethodBackground(Action method)
        {
            Invoke(new Action(() =>
            {
                method();
            }));
            
        }

        public static string ExecutablePath { get; set;}
        
        public static class AppConfigManager
        { 
            public static string GetKeyValue(string tag)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ExecutablePath);
                if (config.AppSettings.Settings[tag] != null)
                {
                    return config.AppSettings.Settings[tag].Value;
                }
                return null;
            }

            public static void SetKeyValue(string tag, string value)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ExecutablePath);
                if (config.AppSettings.Settings[tag] != null)
                {
                    config.AppSettings.Settings[tag].Value = value;
                }
                else
                {
                    config.AppSettings.Settings.Add(tag, value);
                }
                config.Save(ConfigurationSaveMode.Minimal);
            }

            public static bool GetBoolKeyValue(string tag)
            {                
                var value = GetKeyValue(tag);
                if (value != null)
                {
                    return bool.Parse(value);
                }
                else
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                    config.AppSettings.Settings.Add(tag, bool.FalseString);
                    return false;
                }
            }

            public static int GetIntKeyValue(string tag)
            {
                var value = GetKeyValue(tag);
                if (value != null)
                {
                    return int.Parse(value);
                }
                else
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                    config.AppSettings.Settings.Add(tag, "0");
                    return 0;
                }
            }


            public static void ClearTagValues(string tag)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                if (config.AppSettings.Settings[tag] != null)
                {
                    config.AppSettings.Settings[tag].Value = "";
                    config.Save(ConfigurationSaveMode.Minimal);
                }
            }

            public static void AddKeyValue(string tag, string value)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                config.AppSettings.Settings.Add(tag, value);
                config.Save(ConfigurationSaveMode.Minimal);
            }
        }

        private void SetControlsAccordingToServerOffline()
        {
            Invoke(new Action(() =>
            {
                label4.Text = Properties.Resources.SERVER_OFFLINE;
                label4.ForeColor = Color.Red;
                создатьСобытиеToolStripMenuItem.Enabled = false;
                button1.Enabled = false;
                DealWithTrayIcon();
            }));
        }

        private bool AreUserStatusesSet()
        {
            return comboBox1.Items.Count == 1;
        }

        private void SetControlsAccordingToServerOnline()
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
                DealWithTrayIcon();
                InitStatusesForSelectedEvent();
            }));
            
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

        private void SetEventNoteUsingCellValue()
        {
            if (dataGridView1.RowCount > 0)
            {
                if (dataGridView1.Rows[SelectedRowIndex].Cells[NOTE_COL_NUM].Value != null)
                {
                    richTextBox2.Text = dataGridView1.Rows[SelectedRowIndex].Cells[NOTE_COL_NUM].Value.ToString();
                }
            }
            richTextBox2.Enabled = true;
        }

        private void AddStatusesForOwnerAccordingToOwner(BllUser sender)
        {
            if (StatusDeleted != null)
            {
                comboBox1.Items.Remove(StatusDeleted.Name);
                comboBox1.Items.Remove(StatusClosed.Name);
                AvailableStatusesForSelectedEvent.Remove(StatusDeleted);
                AvailableStatusesForSelectedEvent.Remove(StatusClosed);

                if (AreUsersEqual(User, sender))
                {
                    comboBox1.Items.Add(StatusDeleted.Name);
                    comboBox1.Items.Add(StatusClosed.Name);
                    AvailableStatusesForSelectedEvent.Add(StatusDeleted);
                    AvailableStatusesForSelectedEvent.Add(StatusClosed);
                }               
            }
        }

        private void SelectBlankStatus()
        {
            comboBox1.SelectedIndex = 0;
            comboBox1.Enabled = true;
            button1.Enabled = false;
        }

        private void SetSelectedEventToControls()
        {
            BllEvent Event = EventSequence[SelectedRowIndex];
            PopulateTextBoxesUsingEvent(Event);
            PopulateStatusDataGridUsingStatusLib(Event.StatusLib);
            PopulateFileListBoxUsingFilepathLib(Event.FilepathLib);
            SetEventDescription(Event.Description);
            SetEventNoteUsingCellValue();
            SelectBlankStatus();
            AddStatusesForOwnerAccordingToOwner(Event.Sender);         
        }

        private void AddStatusToDataGrid(string name, DateTime date)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView2);
            row.Cells[0].Value = name;
            row.Cells[1].Value = date;
            dataGridView2.Rows.Add(row);
        } 

        private bool HasUserLoginAndPassword()
        {
            return User.Login != "";
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

        private void InitTrayIcon()
        {
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.BalloonTipText = "Программа работает в фоновом режиме.";
            notifyIcon.BalloonTipTitle = Properties.Resources.CLIENT_NAME; ;
            notifyIcon.Icon = this.Icon;
            notifyIcon.Text = Properties.Resources.CLIENT_NAME; ;
            notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;
        }

        private void InitializeAppValues()
        {
            EventSequence = new List<BllEvent>(); 
        }

        private void AddVersionToFormName()
        {
            FormText += " " + AppConfigManager.GetKeyValue(Globals.Globals.TAG_VERSION);
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
            AddVersionToFormName();
            AddAppShortcutToStartup();
            InitTrayIcon();
            SetOpenFileLocationCheckBoxAccordingToConfigValue();
            AddBlankStatusToComboBox();
            TurnAppStartupInTrayAccordingToConfigValue();
        }

        private void SetAppControlsAccordingToOfflineServer()
        {
            SetControlsAccordingToServerOffline();
        }

        private void StartPingingServerBackground()
        {
            new Thread(() =>
            {
                while (true)
                {
                    Thread.CurrentThread.IsBackground = true;
                    PingServerAndIndicateHisStateOnControls();
                    Thread.Sleep(PING_SLEEPTIME_MS);
                }
            }).Start();
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

        private void InitializeColumnSortOrder()
        {
            SenderColumn = new SortableColumn(SENDER_COL_NUM, DIRECTION_MULTIPLIER, OrderBySender);
            NameColumnSorting = new SortableColumn(NAME_COL_NUM, DIRECTION_MULTIPLIER, OrderByName);
            DateColumn = new SortableColumn(DATE_COL_NUM, DIRECTION_MULTIPLIER, OrderByDate);
            StatusColumn = new SortableColumn(STATUS_COL_NUM, DIRECTION_MULTIPLIER, OrderByStatus);
            AttributeColumn = new SortableColumn(ATTRIBUTE_COL_NUM, DIRECTION_MULTIPLIER, OrderByAttributes);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeAppValues();
            SetAppControlsAccordingToOfflineServer();
            InitializeAppProperties();
            InitializeColumnSortOrder();
            try
            {
                ConnectToServerAndIndicateServerStateOnControls();
                StartPingingServerBackground();
                GetEventList();
            }
            catch (UserIsNullException) //user has not logged in
            {
                ExitApp();
            }           
        }

        private void GetServerInstance() 
        {
            string ip = AppConfigManager.GetKeyValue(IP_KEY);
            server = new ServerInstance(ip, this);
        }

        private void ConnectToServerAndIndicateServerStateOnControls()
        {
            try
            {
                GetServerInstance();
                Authorize();
                SetUserFullnameOnLabel();
                SetControlsAccordingToServerOnline();
            }
            catch (ConnectionFailedException ex)
            {
                isServerOnline = false;
                SetControlsAccordingToServerOffline();
            }
            catch (UserIsNullException ex)
            {
                throw ex;
            }

        }



        private void PingServerAndIndicateHisStateOnControls()
        {
            bool previousServerState = isServerOnline;
            if (isServerOnline == false)
            {
                ConnectToServerAndIndicateServerStateOnControls();
            }
            try
            {
                server.PingServer();
            }
            catch(Exception)
            {
                if (previousServerState == true)
                {
                    SetControlsAccordingToServerOffline();
                    isServerOnline = false;
                }
            }
        }

        private void GetStatusesForOwnerFromServer()
        {
            bool success = false;
            while (!success)
            {
                try
                {
                    if (StatusDeleted == null)
                    {
                        IStatusGetter sg = (StatusGetter)server;
                        StatusDeleted = sg.GetStatusDELETED();
                    }
                    if (StatusClosed == null)
                    {
                        IStatusGetter sg = (StatusGetter)server;
                        StatusClosed = sg.GetStatusCLOSED();
                    }
                    success = true;
                }
                catch
                {
                    PingServerAndIndicateHisStateOnControls();
                    success = false;
                }
            }

        }

        private void InitStatusesForSelectedEvent()
        {
            if (User.StatusLib != null)
            {
                AvailableStatusesForSelectedEvent.Clear();
                comboBox1.Items.Clear();
                AddBlankStatusToComboBox();
                GetStatusesForOwnerFromServer();
                
                foreach (var item in User.StatusLib.SelectedEntities)
                {
                    AvailableStatusesForSelectedEvent.Add(item.Entity);
                    comboBox1.Items.Add(item.Entity.Name);
                }
            }
        }

        private List<BllEvent> GetEventsFromServerForCurrentUser()
        {
            bool success = false;
            while (!success)
            {
                success = true;
                try
                {
                    IEventCRUD eventCrud = (EventCRUD)server;
                    {
                        return eventCrud.GetEventsForUser(User);
                    }
                }
                catch
                {
                    success = false;
                    PingServerAndIndicateHisStateOnControls();
                }
            }
            return null;
        }

        private bool HasEventListItems(List<BllEvent> list)
        {
            return list != null;
        }

        private bool IsEventAccepted(BllEvent Event)
        {
            foreach(var item in Event.RecieverLib.SelectedEntities)
            {
                if (AreUsersEqual(item.Entity, User) && item.IsEventAccepted)
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateEventUsingCachedEvent(BllEvent Event, BllEvent cachedEvent)
        {
            Event.Note = cachedEvent.Note;
            Event.HasMissedStatus = cachedEvent.HasMissedStatus;
        }

        private void AddLocalCachedEventsAndUpdateEventsFromServerUsingCache(List<BllEvent> serverEvents, List<BllEvent> cachedEvents)
        {
            foreach (var cachedItem in cachedEvents)
            {
                bool isCachedItemMatchsInListFromServer = false;
                foreach (var item in serverEvents)
                {
                    if (item.Id == cachedItem.Id)
                    {
                        isCachedItemMatchsInListFromServer = true;
                        UpdateEventUsingCachedEvent(item, cachedItem);
                        if (IsTargetStatusObsolete(item, cachedItem))
                        {
                            item.HasMissedStatus = true;
                        }
                        break;
                    }
                }
                if (isCachedItemMatchsInListFromServer == false)
                {
                    EventSequence.Add(cachedItem);
                    AddEventToDataGrid(cachedItem);
                }
            }
        }

        private void DownloadEventFilesUsingFilepathLib(BllFilepathLib lib)
        {
            foreach (var name in lib.Entities)
            {
                new Thread(delegate ()
                {
                    DownloadFile(name.Path, lib.FolderName);
                }).Start();
            }
        }

        private void HideClosedEventsAccordingConfigValue()
        {
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_HIDE_CLOSED))
            {
                HideClosedEvents();
            }
        }

        private void AddEventsAndDownloadTheirFiles(List<BllEvent> events)
        {
            foreach (var item in events)
            {
                EventSequence.Add(item);
                AddEventToDataGrid(item);
                DownloadEventFilesUsingFilepathLib(item.FilepathLib);
            }
        }

        private void GetEventList()
        {
            if (isServerOnline == false)
            {
                EventSequence = DeserializeEventsFromCache();
                RefreshDataGrid();
            }
            else
            {
                List<BllEvent> eventsFromServer = GetEventsFromServerForCurrentUser();
                List<BllEvent> cachedEvents = DeserializeEventsFromCache();
                if (HasEventListItems(cachedEvents))
                {
                    AddLocalCachedEventsAndUpdateEventsFromServerUsingCache(eventsFromServer, cachedEvents);
                }
                AddEventsAndDownloadTheirFiles(eventsFromServer);
                IndicateNewEventsOnTaskbar();
                SerializeEventsToCache();
            }
            HideClosedEventsAccordingConfigValue();
            SetSortingFromConfigAndSort();
            SetDefaultSelectedRow();
        }

        private void SetSortingFromConfigAndSort()
        {
            int sortNum = AppConfigManager.GetIntKeyValue(Properties.Resources.TAG_SORT_NUM);
            int sortDir = AppConfigManager.GetIntKeyValue(Properties.Resources.TAG_SORT_DIR);
            SetCurrentSortingAndSort(sortNum, sortDir);        
        }

        private void SetCurrentSortingAndSort(int sortNum, int sortDir)
        {
            switch (sortNum)
            {
                case SENDER_COL_NUM:
                    CurrentSorting = SenderColumn;
                    break;
                case NAME_COL_NUM:
                    CurrentSorting = NameColumnSorting;
                    break;
                case DATE_COL_NUM:
                    CurrentSorting = DateColumn;
                    break;
                case STATUS_COL_NUM:
                    CurrentSorting = StatusColumn;
                    break;
                case ATTRIBUTE_COL_NUM:
                    CurrentSorting = AttributeColumn;
                    break;
            }
            CurrentSorting.Direction = sortDir;
            CurrentSorting.Sort();
        }

       

        private void AddEventToDataGrid(BllEvent Event)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView1);
            row.Cells[SENDER_COL_NUM].Value = Event.Sender.Fullname;
            row.Cells[NAME_COL_NUM].Value = Event.Name;
            row.Cells[DATE_COL_NUM].Value = Event.Date.ToString(DATE_FORMAT);
            row.Cells[TIME_COL_NUM].Value = Event.Date.ToString(TIME_FORMAT);
            if (Event.StatusLib.SelectedEntities.Count > 0)
            {
                var status = Event.StatusLib.SelectedEntities.Last();
                row.Cells[STATUS_COL_NUM].Value = status.Entity.Name + " " + status.Date;
            }
           
            foreach (var attr in Event.AttributeLib.SelectedEntities)
            {
                row.Cells[ATTRIBUTE_COL_NUM].Value += attr.Entity.Name + "; ";
            }

            var cell6 = (DataGridViewButtonCell)row.Cells[FILE_COL_NUM];
            if (Event.FilepathLib.Entities.Count == 0)
            {
                cell6.Value = "-";
                cell6.ReadOnly = true;
            }
            else
            {
                cell6.Value += " " + Event.FilepathLib.Entities.Count + " ф.";
            }

            row.Cells[DESCRIPTION_COL_NUM].Value = Event.Description;

            if (Event.Note != "")
            {
                row.Cells[NOTE_COL_NUM].Value = Event.Note;
            }

            dataGridView1.Rows.Add(row);

            int addedRowNum = dataGridView1.Rows.Count - 1;
            if (!IsEventAccepted(Event))
            {
                HighlightRow(addedRowNum);
                IndeciesOfNewEvents.Add(addedRowNum);
            }

            var selectedEntities = Event.StatusLib.SelectedEntities;
            if (selectedEntities.Count > 0)
            {
                if (selectedEntities.Last().Entity.Name == Globals.Globals.STATUS_CLOSED)
                {
                    IndeciesOfClosedEvents.Add(addedRowNum);
                    MarkEventInDataGridAsClosed(addedRowNum);
                }
                if (selectedEntities.Last().Entity.Name == Globals.Globals.STATUS_DELETED)
                {
                    IndeciesOfDeletedEvents.Add(addedRowNum);
                    MarkEventInDataGridAsDeleted(addedRowNum);
                }
                if (Event.HasMissedStatus)
                {
                    IndeciesOfNewStatuses.Add(addedRowNum);
                    MakeBoldFontToStatusInDataGrid(addedRowNum);
                }
            }
            
        }
        
        private BllUser CreateBlankUserAccordingToLoginAndPasswordFromConfig()
        {
            string login = AppConfigManager.GetKeyValue(LOGIN_TAG);
            string password = AppConfigManager.GetKeyValue(PASSWORD_TAG);
            if ((login != null) && (password != null))
            {
                return new BllUser
                {
                    Login = login,
                    Password = password
                };
            }
            return new BllUser
            {
                Login = "",
                Password = ""
            };
        }

        private void SetLoginAndPasswordUsingSignInForm()
        {
            bool success = false;
            while (success == false)
            {
                try
                {
                    SignInForm signInForm = new SignInForm();
                    signInForm.ShowDialog();
                    User = signInForm.User;
                    success = true;
                }
                catch(UserIsNullException ex)
                {
                    throw ex;
                }
            }
        }


        private void SetUserFullnameOnLabel()
        {
            Invoke(new Action(() =>
            {
                label9.Text = User.Fullname;
            }));
        }

        private void Authorize()
        {
            User = CreateBlankUserAccordingToLoginAndPasswordFromConfig();
            try
            {
                if (!HasUserLoginAndPassword())
                {
                    SetLoginAndPasswordUsingSignInForm();
                }
                IAuthorizationManager am = (AuthorizationManager)server;
                User = am.SignIn(User);
                WriteLoginAndPasswordToConfig();                   
                
            }
            catch(ConnectionFailedException ex)
            {
                throw ex;
            }
            catch(UserIsNullException ex)
            {
                throw ex;
            }
        }

        private void WriteLoginAndPasswordToConfig()
        {
            AppConfigManager.SetKeyValue(LOGIN_TAG, User.Login);
            AppConfigManager.SetKeyValue(PASSWORD_TAG, User.Password);
        }

        private void ExitApp()
        {
            isAppClosed = true;
            Application.Exit();
            Close();
        }

        private BllEvent GetNewEventUsingAddEventForm()
        {
            addEventForm = new AddEventForm(server, User);
            addEventForm.ShowDialog();
            return addEventForm.Event;
        }

        private bool IsAnyRowSelected()
        {
            return SelectedRowIndex != -1;
        }

        private void создатьСобытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PingServerAndIndicateHisStateOnControls();
            if (isServerOnline)
            {
                if (GetNewEventUsingAddEventForm() != null)
                {                 
                    EventSequence.Add(addEventForm.Event);
                    AddEventToDataGrid(addEventForm.Event);
                    CurrentSorting.Sort(); 
                    SerializeEventsToCache();
                    if (IsAnyRowSelected() == false)
                    {
                        dataGridView1.ClearSelection();
                    }
                }
                addEventForm = null;
            }
        }
        private void HighlightRow(int i)
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            foreach (DataGridViewCell cell in dataGridView1.Rows[i].Cells)
            {
                cell.Style.Font = style.Font;
            }

        }

        private void MakeCommonRowBackground(int i)
        {
            dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
        }

        private void RowCommonFont(int i)
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Font = new Font(dataGridView1.Font, FontStyle.Regular);
            foreach (DataGridViewCell cell in dataGridView1.Rows[i].Cells)
            {
                cell.Style.Font = style.Font;
            }
        }

        private void TurnOutWithEventFormIfHidden()
        {
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_TURNOUT_EVENT) && notifyIcon.Visible)
            {
                TurnOutForm();
            }
        }

        private void TurnOutWithStatusFormIfHidden()
        {
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_TURNOUT_STATUS) && notifyIcon.Visible)
            {
                TurnOutForm();
            }
        }

        private void PlaySignalAccordingToEventConfigValue()
        {
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_SOUND_EVENT))
            {
                SystemSounds.Beep.Play();
            }
        }

        private void PlaySignalAccordingToStatusConfigValue()
        {
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_SOUND_STATUS))
            {
                SystemSounds.Beep.Play();
            }
        }

        public void GetEvent(BllEvent Event)
        {
            EventSequence.Add(Event);
            Invoke(new Action(() =>
            {
                AddEventToDataGrid(Event);
                SerializeEventsBackground();
                IndicateNewEventsOnTaskbar();
                TurnOutWithEventFormIfHidden();
                DealWithTrayIcon();
                PlaySignalAccordingToEventConfigValue();
            }));
            CurrentSorting.Sort(); 
        }      

        private void SerializeEventsBackground()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                SerializeEventsToCache();
            }).Start();
        }

        private void DownloadAndLaunchFiles(BllFilepathLib lib)
        {
            foreach (var name in lib.Entities)
            {
                try
                {
                    Process.Start(DownloadFile(name.Path, lib.FolderName));
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
                var lib = EventSequence[e.RowIndex].FilepathLib;
                DownloadAndLaunchFiles(lib);
            }
        }

        private string DownloadFile(string name, string folderName)
        {
            string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Properties.Resources.DOWNLOADS_FOLDER;
            string eventFolderPath = Path.Combine(downloadsPath, folderName);
            string filePath = Path.Combine(downloadsPath, folderName, name);
            if (!Directory.Exists(eventFolderPath))
            {
                Directory.CreateDirectory(eventFolderPath);
            }
            if (!File.Exists(filePath))
            {
                using (FileStream output = new FileStream(filePath, FileMode.Create))
                {
                    Stream downloadStream;
                    using (FileServiceClient client = new FileServiceClient())
                    {
                        downloadStream = client.GetFile(Path.Combine(folderName, name));
                    }
                    downloadStream.CopyTo(output);
                }
            }
            return filePath;
        }

        public void Ping()
        {
            
        }

        private void SetNotesForEvents()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                EventSequence[i].Note = (string)dataGridView1.Rows[i].Cells[NOTE_COL_NUM].Value;
            }
        }

        private void UnsetNotesForEvents()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                EventSequence[i].Note = "";
            }
        }



        private List<BllEvent> DeserializeEventsFromCache()
        {
            try
            {
                string mydoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                using (Stream stream = File.Open(mydoc + Properties.Resources.CACHE_XML_FILE, FileMode.Open))
                {
                    return (List<BllEvent>)serializer.Deserialize(stream);
                }
            }
            catch (IOException)
            {
            }
            return null;

        }

        
        private void IndicateNewEventsOnTaskbar()
        {
            int n = IndeciesOfNewEvents.Count;
            Text = FormText;
            if (n > 0 )
            {
                Text += " (" + n + ")";
            }
            DealWithFlashing();
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

        private bool IsCurrentStatusForOwner()
        {
            var statuses = EventSequence[SelectedRowIndex].StatusLib.SelectedEntities;
            if (statuses.Count > 0)
            {
                string statusName = statuses.Last().Entity.Name;
                if ((statusName == Globals.Globals.STATUS_CLOSED) || (statusName == Globals.Globals.STATUS_DELETED))
                {
                    return true;
                }
            }          
            return false;
        }

        private void AddSelectedStatusToEvent()
        {
            var statuses = EventSequence[SelectedRowIndex].StatusLib.SelectedEntities;
            statuses.Add(new BllSelectedStatus { Entity = AvailableStatusesForSelectedEvent[comboBox1.SelectedIndex - 1] });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PingServerAndIndicateHisStateOnControls();
            
            if (isServerOnline && IsAnyRowSelected())
            {
                if(IsCurrentStatusForOwner())
                {
                    DeleteFromIndexLists(SelectedRowIndex);
                }
                AddSelectedStatusToEvent();
                bool success = false;
                while (!success)
                {
                    try
                    {
                        IEventCRUD crud = (EventCRUD)server;
                        EventSequence[SelectedRowIndex] = crud.UpdateStatusAndSendOutEvent(EventSequence[SelectedRowIndex], User);
                        var newStatus = EventSequence[SelectedRowIndex].StatusLib.SelectedEntities.Last();
                        AddStatusToDataGrid(newStatus.Entity.Name, newStatus.Date);
                        UpdateEventStatusInDataGrid(newStatus, SelectedRowIndex);
                        SelectBlankStatus();
                        SerializeEventsBackground();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        PingServerAndIndicateHisStateOnControls();
                    }
                }
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
                if (isServerOnline)
                {
                    button1.Enabled = true;
                }
            }

        }

        private int FindUpdatedEventInEventSequenceById(int eventId) 
        {
            for (int i = 0; i < EventSequence.Count; i++)
            {
                if (EventSequence[i].Id == eventId)
                {
                    return i;
                }
            }
            return -1;
        }



        private bool IsTargetStatusObsolete(BllEvent source, BllEvent target)
        {
            if (source.StatusLib.SelectedEntities.Count > 0)
            {
                var selectedStatuses = target.StatusLib.SelectedEntities;
                var newstatus = source.StatusLib.SelectedEntities.Last();
                if (selectedStatuses.Count > 0)
                {
                    var oldstatus = selectedStatuses.Last();
                    if (newstatus.Date != oldstatus.Date)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateSelectedEvent(BllEvent source, int rowNum)
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

        public void UpdateEvent(BllEvent Event)
        {
            int i = FindUpdatedEventInEventSequenceById(Event.Id);
            if (i >= 0)
            {
                if (IsTargetStatusObsolete(Event, EventSequence[i]))
                {
                    var newStatus = Event.StatusLib.SelectedEntities.Last();
                    UpdateEventStatusInDataGrid(newStatus, i);
                    MakeBoldFontToStatusInDataGrid(i);
                    IndeciesOfNewStatuses.Add(i);
                    Event.HasMissedStatus = false;
                    DealWithFlashing();
                    DealWithTrayIcon();
                    TurnOutWithStatusFormIfHidden();
                    PlaySignalAccordingToStatusConfigValue();
                }
                else
                {
                    Event.HasMissedStatus = true;
                }
                EventSequence[i] = Event;
                UpdateSelectedEvent(Event, i);
                SerializeEventsBackground();
            }
        }

        private void MarkEventInDataGridAsClosed(int i)
        {
            IndeciesOfClosedEvents.Add(i);
            dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Gray;
            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (SelectedRowIndex == i)
                {
                    EnableDeleteEventButton();
                }
            }
        }

        private void MarkEventInDataGridAsDeleted(int i)
        {
            IndeciesOfDeletedEvents.Add(i);
            dataGridView1.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.DarkRed;
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (SelectedRowIndex == i)
                {
                    EnableDeleteEventButton();
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

        private void PopulateStatusDataGridUsingStatusLib(BllStatusLib lib)
        {
            dataGridView2.Rows.Clear();
            foreach(var item in lib.SelectedEntities)
            {
                AddStatusToDataGrid(item.Entity.Name, item.Date);
            }
        }

        private void MakeBoldFontToStatusInDataGrid(int row)
        {
            var cell = dataGridView1.Rows[row].Cells[STATUS_COL_NUM];
            cell.Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
        }

        private void UpdateEventStatusInDataGrid(BllSelectedStatus status, int index)
        {
            var cell = dataGridView1.Rows[index].Cells[STATUS_COL_NUM];
            cell.Value = status.Entity.Name + " " + status.Date;
            if (status.Entity.Name == Globals.Globals.STATUS_CLOSED)
            {
                MarkEventInDataGridAsClosed(index);
            }
            else
            {
                if (status.Entity.Name == Globals.Globals.STATUS_DELETED)
                {
                    MarkEventInDataGridAsDeleted(index);
                }
                else
                {
                    MakeCommonRowBackground(index);
                }
            }


        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TurnOutForm();
        }

        private void SetTrayNewEventIcon()
        {
            notifyIcon.Icon = new Icon(Properties.Resources.NewEventTray, new Size(32, 32));
        }

        private void SetTrayServerOfflineIcon()
        {
            notifyIcon.Icon = new Icon(Properties.Resources.ServerOfflineTray, new Size(32, 32));
        }

        private void SetTrayCommontIcon()
        {
            notifyIcon.Icon = this.Icon;
        }

        private void DealWithTrayIcon()
        {
            if (!isServerOnline)
            {
                SetTrayServerOfflineIcon();
            }
            else
            {
                if ((IndeciesOfNewEvents.Count != 0) || ((IndeciesOfNewStatuses.Count != 0) && AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_TRAY_INDICATION_STATUS)))
                {
                    SetTrayNewEventIcon();
                }
                else
                {
                    SetTrayCommontIcon();
                }
            }
        }

        private void TurnInForm()
        {
            notifyIcon.Visible = true;
            // notifyIcon.ShowBalloonTip(3000);
            DealWithTrayIcon();
            this.ShowInTaskbar = false;
            this.Hide();
        }

        private void TurnOutForm()
        {
            this.Show();
            this.ShowInTaskbar = true;
            notifyIcon.Visible = false;
            DealWithFlashing();
        }
            

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isAppClosed)
            {
                TurnInForm();
                e.Cancel = true;
            }
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            try
            {
                SerializeEventsToCache();
                notifyIcon.Visible = false;
            }
            catch { }
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

        private void MakeSelectedEventAdmited()
        {
            IndeciesOfNewEvents.Remove(SelectedRowIndex);
            RowCommonFont(SelectedRowIndex);
            IndicateNewEventsOnTaskbar();
            SerializeEventsBackground();
        }

        private bool AreUsersEqual(BllUser user1, BllUser user2)
        {
            if (user1.Login == user2.Login)
            {
                return true;
            }
            return false;
        }

        private void MarkRecieverInLib(BllUserLib RecieverLib)
        {
            foreach (var item in RecieverLib.SelectedEntities)
            {
                if (AreUsersEqual(item.Entity, User))
                {
                    item.IsEventAccepted = true;
                    break;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var currentEvent = EventSequence[SelectedRowIndex];
            if (checkBox1.Checked)
            {
                ShowChecklist();
                if (IndeciesOfNewEvents.Contains(SelectedRowIndex))
                {
                    MakeSelectedEventAdmited();
                }
                MarkRecieverInLib(currentEvent.RecieverLib);
                IEventCRUD crud = (EventCRUD)server;
                currentEvent = crud.UpdateAcceptedUsersAndSendOutEvent(currentEvent, User);
                FillUserChecklist(currentEvent.RecieverLib.SelectedEntities);
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

        private void ShowCheckbox()
        {
            groupBox4.Visible = true;
            groupBox1.Visible = false;
        }

        private void HideChecklistAndCheckbox()
        {
            groupBox4.Visible = false;
            groupBox1.Visible = false;
        }

        private void AddUserToChecklist(BllUser user, bool isAccepted)
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
            groupBox1.Text = "Ознакомились " + acceptedEvents + " из " + users.Count;
        }

        private void FillUserChecklist(List<BllSelectedUser> users)
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
            string filename = EventSequence[SelectedRowIndex].FilepathLib.Entities[listBox2.SelectedIndex].Path;
            string foldername = EventSequence[SelectedRowIndex].FilepathLib.FolderName;
            try
            {
                if (checkBox2.Checked == false)
                {
                    Process.Start(DownloadFile(filename, foldername));
                }
                else
                {
                    string path = DownloadFile(filename, foldername);
                    Process.Start("explorer.exe", "/select, \"" + path + "\"");
                }
                
            }
            catch
            {
                MessageBox.Show(Properties.Resources.CANNOT_OPEN_FILE, filename);
            }
        }

        private void DeleteFromIndexLists(int i)
        {
            if (IndeciesOfDeletedEvents.Contains(i))
            {
                IndeciesOfDeletedEvents.Remove(i);

            }
            if (IndeciesOfClosedEvents.Contains(i))
            {
                IndeciesOfClosedEvents.Remove(i);
            }
            if (IndeciesOfNewEvents.Contains(i))
            {
                IndeciesOfNewEvents.Remove(i);
            }
        }

        private void удалитьСобытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedRowIndex >= 0)
            {
                DeleteFromIndexLists(SelectedRowIndex);
                DealWithTrayIcon();
                RecalculateIndexList(IndeciesOfDeletedEvents, SelectedRowIndex);
                RecalculateIndexList(IndeciesOfClosedEvents, SelectedRowIndex);
                RecalculateIndexList(IndeciesOfNewEvents, SelectedRowIndex);
                EventSequence.RemoveAt(SelectedRowIndex);
                dataGridView1.Rows.RemoveAt(SelectedRowIndex);
                dataGridView1.ClearSelection();
                SerializeEventsBackground();
                if (dataGridView1.DisplayedRowCount(true) == 0)
                {
                    DisableDeleteEventButton();
                    ClearDataControls();
                }
            }
        }

        private void RecalculateIndexList(List<int> list, int selectedRowIndex)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] > selectedRowIndex)
                {
                    list[i]--;
                }
            }
        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            SelectedRowIndex = -1;
            DisableSendOnEventButton();
            if (e.StateChanged != DataGridViewElementStates.Selected) return;
            if (dataGridView1.SelectedRows.Count == 0)
            {
                ClearDataControls();
                return;
            }
            SelectedRowIndex = dataGridView1.SelectedRows[0].Index;
            EnableSendOnEventButton();

            checkBox1.Checked = false;
            if (IndeciesOfClosedEvents.Contains(SelectedRowIndex) || IndeciesOfDeletedEvents.Contains(SelectedRowIndex))
            {
                EnableDeleteEventButton();
            }
            else
            {
                DisableDeleteEventButton();
            }

            if (IndeciesOfNewStatuses.Contains(SelectedRowIndex))
            {
                RemoveNewStatusIndex(SelectedRowIndex);
            }
          
            SetSelectedEventToControls();
            var recievers = EventSequence[SelectedRowIndex].RecieverLib.SelectedEntities;
            if ((IsUserInChecklistByLogin(User, recievers)) || (User.Login == EventSequence[SelectedRowIndex].Sender.Login))
            {
                ShowChecklist();
                FillUserChecklist(recievers);
            }
            else
            {
                if (isServerOnline)
                {
                    ShowCheckbox();
                }
            }

            HandleStatusChanging();
        }

        private void DealWithFlashing()
        {
            if (IndeciesOfNewStatuses.Count == 0 && IndeciesOfNewStatuses.Count == 0)
            {
                FlashWindow.Stop(this);
            }
            else
            {
                if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_TASKBAR_INDICATION_STATUS))
                {
                    FlashWindow.Start(this);
                }
            }
        }

        private void RemoveNewStatusIndex(int selectedRowIndex)
        {
            IndeciesOfNewStatuses.Remove(selectedRowIndex);
            RowCommonFont(selectedRowIndex);
            EventSequence[selectedRowIndex].HasMissedStatus = false;
            DealWithFlashing();
            DealWithTrayIcon();
            SerializeEventsBackground();
        }

        private void HandleStatusChanging()
        {
            if (EventSequence[SelectedRowIndex].StatusLib.SelectedEntities.Count > 0)
            {
                if (IsCurrentStatusForOwner())
                {
                    if (User.Login == EventSequence[SelectedRowIndex].Sender.Login)
                    {
                        EnableStatusControls();
                    }
                    else
                    {
                        DisableStatusControls();
                        HideChecklistAndCheckbox();
                    }
                }
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
            richTextBox2.Text = "";
            richTextBox2.Enabled = false;
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
                    foreach(var i in IndeciesOfClosedEvents)
                    {
                        dataGridView1.Rows[i].Visible = true;
                    }
                }
                else
                {
                    HideClosedEvents();
                }
            }
            else
            {
                if (prevHideClosed && (prevHideAllowance != AppConfigManager.GetIntKeyValue(Properties.Resources.TAG_HIDE_ALLOWANCE)))
                {
                    HideClosedEvents();
                }
            }
        }

        private void HideClosedEvents()
        {
            int days = AppConfigManager.GetIntKeyValue(Properties.Resources.TAG_HIDE_ALLOWANCE);
            DateTime now = DateTime.Now;
            for (int i = 0; i < EventSequence.Count; i++)
            {
                if (IndeciesOfClosedEvents.Contains(i))
                {
                    var currentStatusDate = EventSequence[i].StatusLib.SelectedEntities.Last().Date;
                    if (currentStatusDate.AddDays(days).CompareTo(now) < 0)
                    {
                        dataGridView1.Rows[i].Visible = false;
                    }
                }
                else
                {
                    dataGridView1.Rows[i].Visible = true;
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_OPEN_FILE_LOCATION, checkBox2.Checked.ToString());
        }

        private int GetNewSortDirectionUsingSortNumbers(int previousSortNum, int newSortNum)
        {
            int newDir = CurrentSorting.Direction;
            if (previousSortNum == newSortNum)
            {
                newDir *= DIRECTION_MULTIPLIER;
            }
            return newDir;
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int newDir = GetNewSortDirectionUsingSortNumbers(CurrentSorting.Number, e.ColumnIndex);
            SetCurrentSortingAndSort(e.ColumnIndex, newDir);
        }

        private bool OrderBySender()
        {
            SetNotesForEvents();
            EventSequence.Sort((x, y) => CurrentSorting.Direction * string.Compare(x.Sender.Fullname, y.Sender.Fullname));
            RefreshDataGrid();
            UnsetNotesForEvents();
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_NUM, SENDER_COL_NUM.ToString());
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_DIR, CurrentSorting.Direction.ToString());
            return true;
        }

        private bool OrderByName()
        {
            SetNotesForEvents();
            EventSequence.Sort((x, y) => CurrentSorting.Direction * string.Compare(x.Name, y.Name));
            RefreshDataGrid();
            UnsetNotesForEvents();
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_NUM, NAME_COL_NUM.ToString());
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_DIR, CurrentSorting.Direction.ToString());
            return true;
        }

        private bool OrderByDate()
        {
            SetNotesForEvents();
            EventSequence.Sort((x, y) => CurrentSorting.Direction * DateTime.Compare(x.Date, y.Date));
            RefreshDataGrid();
            UnsetNotesForEvents();
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_NUM, DATE_COL_NUM.ToString());
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_DIR, CurrentSorting.Direction.ToString());
            return true;
        }

        private bool OrderByStatus()
        {
            SetNotesForEvents();
            EventSequence.Sort((x, y) => CurrentSorting.Direction * string.Compare(x.StatusLib.SelectedEntities.Count != 0 ? x.StatusLib.SelectedEntities.Last().Entity.Name : "",
                                                                                   y.StatusLib.SelectedEntities.Count != 0 ? y.StatusLib.SelectedEntities.Last().Entity.Name : ""));
            RefreshDataGrid();
            UnsetNotesForEvents();
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_NUM, STATUS_COL_NUM.ToString());
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_DIR, CurrentSorting.Direction.ToString());
            return true;
        }

        private bool OrderByAttributes()
        {
            SetNotesForEvents();
            EventSequence.Sort((x, y) => CurrentSorting.Direction * (x.AttributeLib.SelectedEntities.Count - y.AttributeLib.SelectedEntities.Count));
            RefreshDataGrid();
            UnsetNotesForEvents();
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_NUM, ATTRIBUTE_COL_NUM.ToString());
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_DIR, CurrentSorting.Direction.ToString());
            return true;
        }

        private void RefreshDataGrid()
        {           
            dataGridView1.Rows.Clear();
            IndeciesOfClosedEvents.Clear();
            IndeciesOfDeletedEvents.Clear();
            IndeciesOfNewEvents.Clear();
            foreach(var item in EventSequence)
            {
                AddEventToDataGrid(item);
            }
            HideClosedEventsAccordingConfigValue();
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (SelectedRowIndex >= 0)
            {
                dataGridView1.Rows[SelectedRowIndex].Cells[NOTE_COL_NUM].Value = richTextBox2.Text;
            }
        }

        private void richTextBox2_Leave(object sender, EventArgs e)
        {
            SerializeEventsBackground();
        }

        private void переслатьСобытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PingServerAndIndicateHisStateOnControls();
            if (isServerOnline)
            {
                SendOnEventForm sendOnEventForm = new SendOnEventForm(server, EventSequence[SelectedRowIndex], User);
                sendOnEventForm.ShowDialog();
            }
        }
    }
}
