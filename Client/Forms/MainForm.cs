using BllEntities;
using Client.Forms;
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
        }

        const string LOGIN_TAG = "login";
        const string PASSWORD_TAG = "password";
        const string DATE_FORMAT = "dd.MM.yyyy";
        const string TIME_FORMAT = "HH:mm";
        const string STATUS_NOT_CHANGED = "Статус не изменён";
        const string IP_KEY = "hostIP";

        const int PING_SLEEPTIME_MS = 10000;
        const int NOTE_COL_NUM = 8;

        Func<bool> OrderFunc;

        IBusinessService server;

        BllUser User = null;
        List<BllEvent> EventList;
        List<BllStatus> SelectedEventAvailableStatuses = new List<BllStatus>();
        BllStatus StatusDeleted = null;
        BllStatus StatusClosed = null;
        
        List<int> NewEventIndecies = new List<int>();
        List<int> DeletedEventsIndecies = new List<int>();
        List<int> ClosedEventsIndecies = new List<int>();
        NotifyIcon notifyIcon = new NotifyIcon();
        XmlSerializer serializer = new XmlSerializer(typeof(List<BllEvent>));
        AddEventForm addEventForm = null;

        int SelectedRowIndex;

        string FormText = Properties.Resources.CLIENT_NAME;

        bool isAppClosed;
        private bool _isServerOnline;

        bool isServerOnline {
            get { return _isServerOnline; }
            set
            {
                if ((value == true) && (_isServerOnline == false))
                {
                    Invoke(new Action(() =>
                    {
                        SetControlsAccordingServerOnline();
                        if (IsUserEmpty())
                        {
                            Authorize(server);
                        }
                        GetDefaultStatusesFromServer();
                    }));
                }
                if ((value == false) && (_isServerOnline == true))
                {
                    Invoke(new Action(() =>
                    {
                        SetControlsAccordingServerOffline();
                    }));

 
                }
                _isServerOnline = value;
            }
        }
        
        public static class AppConfigManager
        {
            public static string GetKeyValue(string tag)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                if (config.AppSettings.Settings[tag] != null)
                {
                    return config.AppSettings.Settings[tag].Value;
                }
                return null;
            }

            public static void SetKeyValue(string tag, string value)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
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

        private void LogMessage(string message)
        {
            //richTextBox2.Text += "[" + DateTime.Now.ToString(TIME_FORMAT) + "] " + message + "\n";
        }

        private void SetControlsAccordingServerOffline()
        {
            label4.Text = Properties.Resources.SERVER_OFFLINE;
            label4.ForeColor = Color.Red;
            создатьСобытиеToolStripMenuItem.Enabled = false;
            button1.Enabled = false;
        }

        private void SetControlsAccordingServerOnline()
        {
            label4.Text = Properties.Resources.SERVER_ONLINE;
            label4.ForeColor = Color.Black;
            создатьСобытиеToolStripMenuItem.Enabled = true;
            if (comboBox1.SelectedIndex > 0)
            {
                button1.Enabled = true;
            }
            if (comboBox1.Items.Count == 1)
            {
                if (!IsUserEmpty())
                {
                    InitStatusesForSelectedEvent();
                }
            }
        }

        private void SetSelectedEventToControls(BllEvent Event)
        {
            textBox2.Text = Event.Sender.Fullname;
            textBox3.Text = Event.Name;
            textBox4.Text = Event.Date.ToString(DATE_FORMAT);
            textBox5.Text = Event.Date.ToString(TIME_FORMAT);
            dataGridView2.Rows.Clear();
            foreach(var status in Event.StatusLib.SelectedEntities)
            {
                AddStatusToDataGrid(status.Entity.Name, status.Date);
            }
            listBox2.Items.Clear();
            foreach (var filename in Event.FilepathLib.Entities)
            {
                listBox2.Items.Add(filename.Path);
            }
            richTextBox1.Text = Event.Description;
            if (dataGridView1.RowCount > 0)
            {
                if (dataGridView1.Rows[SelectedRowIndex].Cells[NOTE_COL_NUM].Value != null)
                {
                    richTextBox2.Text = dataGridView1.Rows[SelectedRowIndex].Cells[NOTE_COL_NUM].Value.ToString();
                }
            }

            comboBox1.SelectedIndex = 0;
            comboBox1.Enabled = true;
            button1.Enabled = false;

            if (StatusDeleted != null)
            {
                comboBox1.Items.Remove(StatusDeleted.Name);
                comboBox1.Items.Remove(StatusClosed.Name);
                SelectedEventAvailableStatuses.Remove(StatusDeleted);
                SelectedEventAvailableStatuses.Remove(StatusClosed);
                if (User != null)
                {
                    if (Event.Sender.Id == User.Id)
                    {
                        comboBox1.Items.Add(StatusDeleted.Name);
                        comboBox1.Items.Add(StatusClosed.Name);
                        SelectedEventAvailableStatuses.Add(StatusDeleted);
                        SelectedEventAvailableStatuses.Add(StatusClosed);
                    }
                }
            }

            richTextBox2.Enabled = true;
        }

        private void AddStatusToDataGrid(string name, DateTime date)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView2);
            row.Cells[0].Value = name;
            row.Cells[1].Value = date;
            dataGridView2.Rows.Add(row);
        } 

        private bool IsUserEmpty()
        {
            if (User.Login == "")
            {
                return true;
            }
            return false;
        }

        private void SetUserEmpty()
        {
            User = new BllUser { Login = "" };
        }

        private void AppShortcutToStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (key.GetValue(Properties.Resources.CLIENT_NAME) == null)
                {
                    key.SetValue(Properties.Resources.CLIENT_NAME, "\"" + Application.ExecutablePath + "\"");
                }
            }
        }

        private void InitNotifyIcon()
        {
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.BalloonTipText = "Программа работает в фоновом режиме.";
            notifyIcon.BalloonTipTitle = Properties.Resources.CLIENT_NAME; ;
            notifyIcon.Icon = this.Icon;
            notifyIcon.Text = Properties.Resources.CLIENT_NAME; ;
            notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConnectToServer();
            EventList = new List<BllEvent>();
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            FormText += " " + AppConfigManager.GetKeyValue(Globals.Globals.TAG_VERSION);
            AppShortcutToStartup();
            InitNotifyIcon();

            comboBox1.Items.Add(STATUS_NOT_CHANGED);
            checkBox2.Checked = AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_OPEN_FILE_LOCATION);

            SetUserEmpty();
            SetControlsAccordingServerOffline();
            PingServer();

            if (!IsUserEmpty())
            {
                new Thread(() =>
                {
                    while (!isAppClosed)
                    {
                        Thread.CurrentThread.IsBackground = true;
                        PingServer();
                        Thread.Sleep(PING_SLEEPTIME_MS);
                    }
                }).Start();
                GetEventList();
                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.CurrentRow.Selected = false;
                }
            }
            else
            {
                ExitApp();
            }

            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_STARTUP_TRAY))
            {
                Close();
            }
           
        }

        private void ConnectToServer()
        {
            string ip = AppConfigManager.GetKeyValue(IP_KEY);
            server = ServiceChannelManagerSingleton.Instance.GetServerMethods(this, ip);
        }

        private void PingServer()
        {
            try
            {
                if (isServerOnline == false)
                {
                    ConnectToServer();
                    if (addEventForm != null)
                    {
                        addEventForm.server = server;
                    }
                    Authorize(server);

                }
                server.PingServer();

                if (!IsUserEmpty())
                {
                    isServerOnline = true;
                }
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
                isServerOnline = false;
            }
        }

        private void GetDefaultStatusesFromServer()
        {
            bool success = false;
            while (!success)
            {
                try
                {
                    if (StatusDeleted == null)
                    {
                        StatusDeleted = server.GetStatusDeleted();
                    }
                    if (StatusClosed == null)
                    {
                        StatusClosed = server.GetStatusClosed();
                    }
                    success = true;
                }
                catch
                {
                    PingServer();
                    success = false;
                }
            }

        }

        private void InitStatusesForSelectedEvent()
        {
            if (User.StatusLib != null)
            {
                SelectedEventAvailableStatuses.Clear();
                GetDefaultStatusesFromServer();
                
                foreach (var item in User.StatusLib.SelectedEntities)
                {
                    SelectedEventAvailableStatuses.Add(item.Entity);
                    comboBox1.Items.Add(item.Entity.Name);
                }
            }
        }



        private void GetEventList()
        {
            var cachedEvents = DeserializeEvents();
            if (isServerOnline == false)
            {
                EventList = cachedEvents;
                RefreshDataGrid();
            }
            else
            {
                bool success = false;
                List<BllEvent> serverEvents = null;
                while (!success)
                {
                    success = true;
                    try
                    {
                        serverEvents = server.GetEventsForUser(User);
                    }
                    catch
                    {
                        success = false;
                        PingServer();
                    }

                }
                if (cachedEvents != null)
                {
                    foreach (var cachedItem in cachedEvents)
                    {
                        bool isExists = false;
                        foreach (var item in serverEvents)
                        {
                            if (item.Id == cachedItem.Id)
                            {
                                isExists = true;
                                if (cachedItem.IsAdmited)
                                {
                                    item.IsAdmited = true;                                    
                                }
                                item.Note = cachedItem.Note;
                                break;
                            }
                        }
                        if (!isExists)
                        {
                            EventList.Add(cachedItem);
                            AddEventToDataGrid(cachedItem);
                        }
                    }
                }
                int i = EventList.Count;
                foreach(var item in serverEvents)
                {
                    EventList.Add(item);
                    AddEventToDataGrid(item);
                    foreach (var name in item.FilepathLib.Entities)
                    {
                        new Thread(delegate ()
                        {
                            DownloadFile(name.Path, item.FilepathLib.FolderName);
                        }).Start();

                    }
                    i++;
                }
                
                DealWithMissedEventsOnTaskbar();
                SerializeEvents();
            }
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_HIDE_CLOSED))
            {
                HideClosedEvents();
            }

            SetSortingFromConfigAndSort();
        }

        private void SetSortingFromConfigAndSort()
        {
            string sortNum = AppConfigManager.GetKeyValue(Properties.Resources.TAG_SORT_NUM);
            int sortDir = AppConfigManager.GetIntKeyValue(Properties.Resources.TAG_SORT_DIR);
            if (sortNum != null)
            {
                switch (sortNum)
                {
                    case "0":
                        OrderFunc = OrderBySender;
                        dir0col = decimal.ToInt32(sortDir);
                        break;
                    case "1":
                        OrderFunc = OrderByName;
                        dir1col = decimal.ToInt32(sortDir);
                        break;
                    case "2":
                        OrderFunc = OrderByDate;
                        dir2col = decimal.ToInt32(sortDir);
                        break;
                    case "3":
                        OrderFunc = OrderByStatus;
                        dir4col = decimal.ToInt32(sortDir);
                        break;
                    case "4":
                        OrderFunc = OrderByAttributes;
                        dir5col = decimal.ToInt32(sortDir);
                        break;
                }
                OrderFunc();
            }
            else
            {
                OrderByDate();
            }
        }



        private void AddEventToDataGrid(BllEvent Event)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView1);
            row.Cells[0].Value = Event.Sender.Fullname;
            row.Cells[1].Value = Event.Name;
            row.Cells[2].Value = Event.Date.Date.ToString(DATE_FORMAT);
            row.Cells[3].Value = Event.Date.ToString(TIME_FORMAT);
            if (Event.StatusLib.SelectedEntities.Count > 0)
            {
                var status = Event.StatusLib.SelectedEntities.Last();
                row.Cells[4].Value = status.Entity.Name + " " + status.Date;
            }
           
            foreach (var attr in Event.AttributeLib.SelectedEntities)
            {
                row.Cells[5].Value += attr.Entity.Name + "; ";
            }
            var cell6 = (DataGridViewButtonCell)row.Cells[6];
            if (Event.FilepathLib.Entities.Count == 0)
            {
                cell6.Value = "-";
                cell6.ReadOnly = true;
            }
            else
            {
                cell6.Value += " " + Event.FilepathLib.Entities.Count + " ф.";
            }
            row.Cells[7].Value = Event.Description;

            if (Event.Note != "")
            {
                row.Cells[NOTE_COL_NUM].Value = Event.Note;
            }

            dataGridView1.Rows.Add(row);

            int i = dataGridView1.Rows.Count - 1;
            if (!IsUserEmpty() && isServerOnline)
            {
                if (!Event.IsAdmited && (Event.Sender.Id != User.Id))
                {
                    HighlightRow(i);
                    NewEventIndecies.Add(i);
                }
            }
            else
            {
                if (!Event.IsAdmited)
                {
                    HighlightRow(i);
                    NewEventIndecies.Add(i);
                }
            }

            var selectedEntities = Event.StatusLib.SelectedEntities;
            if (selectedEntities.Count > 0)
            {
                if (selectedEntities.Last().Entity.Name == Globals.Globals.STATUS_CLOSED)
                {
                    ClosedEventsIndecies.Add(i);
                    MarkEventInDataGridAsClosed(i);
                }
                if (selectedEntities.Last().Entity.Name == Globals.Globals.STATUS_DELETED)
                {
                    DeletedEventsIndecies.Add(i);
                    MarkEventInDataGridAsDeleted(i);
                }
            }
            
        }

        private void Authorize(IBusinessService server)
        {
            string login = AppConfigManager.GetKeyValue(LOGIN_TAG);
            string password = AppConfigManager.GetKeyValue(PASSWORD_TAG);
            User = new BllUser { Login = login, Password = password };
            try
            {
                if (IsUserEmpty())
                {
                    SignInForm signInForm = new SignInForm(server);
                    signInForm.ShowDialog();
                    User = signInForm.User;
                    if (User == null)
                    {
                        SetUserEmpty();
                        ExitApp();

                    }
                    else
                    {
                        WriteLoginAndPasswordToConfig(User.Login, User.Password);
                    }
                }
                else
                {
                    User = server.SignIn(login, password);
                }
                if (!IsUserEmpty())
                {
                    Invoke(new Action(() =>
                    {
                        label9.Text = User.Fullname;
                    }));
                }
            }
            catch(Exception ex)
            {
                if ((login == null) && (password == null))
                {
                    MessageBox.Show(Properties.Resources.SERVER_NOT_FOUND + ex.Message);
                    SetUserEmpty();
                    ExitApp();
                }
            }
        }

        private void WriteLoginAndPasswordToConfig(string login, string password)
        {
            AppConfigManager.SetKeyValue(LOGIN_TAG, login);
            AppConfigManager.SetKeyValue(PASSWORD_TAG, password);
        }

        private void ExitApp()
        {
            Application.Exit();
            isAppClosed = true;
            Close();
        }

        private void создатьСобытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PingServer();
            if (isServerOnline)
            {
                addEventForm = new AddEventForm(server, User);
                addEventForm.ShowDialog();
                if (addEventForm.Event != null)
                {                 
                    EventList.Add(addEventForm.Event);
                    AddEventToDataGrid(addEventForm.Event);
                    OrderFunc();
                    SerializeEvents();
                    if (SelectedRowIndex == -1)
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

        public void GetEvent(BllEvent Event)
        {
            Event.IsAdmited = false;
            EventList.Add(Event);
            Invoke(new Action(() =>
            {
                AddEventToDataGrid(Event);
                SerializeEventsBackground();

                DealWithMissedEventsOnTaskbar();
                if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_TURNOUT_EVENT) && notifyIcon.Visible)
                {
                    TurnOutForm();
                }
                else
                {
                    SetTrayNewEventIcon();
                }

                if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_SOUND_EVENT))
                {
                    SystemSounds.Beep.Play();
                }
            }));
            OrderFunc();
        }      

        private void SerializeEventsBackground()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                SerializeEvents();
            }).Start();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {            
            var senderGrid = (DataGridView)sender;
            
            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                var lib = EventList[e.RowIndex].FilepathLib;
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
                EventList[i].Note = (string)dataGridView1.Rows[i].Cells[NOTE_COL_NUM].Value;
            }
        }

        private void UnsetNotesForEvents()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                EventList[i].Note = "";
            }
        }

        private void SerializeEvents()
        {
            try
            {
                string mydoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (!Directory.Exists(mydoc + Properties.Resources.DOWNLOADS_FOLDER))
                {
                    Directory.CreateDirectory(mydoc + Properties.Resources.DOWNLOADS_FOLDER);
                }

                using (FileStream stream = new FileStream(mydoc + Properties.Resources.CACHE_XML_FILE, FileMode.Create))
                {
                    SetNotesForEvents();
                    serializer.Serialize(stream, EventList);
                    UnsetNotesForEvents();
                }
            }
            catch (IOException)
            {

            }
        }

        private List<BllEvent> DeserializeEvents()
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

        
        private void DealWithMissedEventsOnTaskbar()
        {
            int n = NewEventIndecies.Count;
            Text = FormText;
            if (n > 0)
            {
                Text += " (" + n + ")";
                FlashWindow.Start(this);
            }
            else
            {
                FlashWindow.Stop(this);
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
            PingServer();
            
            if (isServerOnline)
            {
                var statuses = EventList[SelectedRowIndex].StatusLib.SelectedEntities;
                if (statuses.Count > 0)
                {
                    string statusName = statuses.Last().Entity.Name;
                    if ((statusName == Globals.Globals.STATUS_CLOSED) || (statusName == Globals.Globals.STATUS_DELETED))
                    {
                        DeleteFromIndexLists(SelectedRowIndex);
                    }
                }
                statuses.Add(new BllSelectedStatus { Entity = SelectedEventAvailableStatuses[comboBox1.SelectedIndex - 1] });
                try
                {
                    EventList[SelectedRowIndex] = server.UpdateStatusAndSendOutEvent(EventList[SelectedRowIndex], User);
                    var newStatus = EventList[SelectedRowIndex].StatusLib.SelectedEntities.Last();
                    AddStatusToDataGrid(newStatus.Entity.Name, newStatus.Date);
                    UpdateEventStatusInDataGrid(newStatus, SelectedRowIndex, false);
                    comboBox1.SelectedIndex = 0;
                    SerializeEventsBackground();
                }
                catch(Exception ex)
                {
                    statuses.RemoveAt(statuses.Count - 1);
                    MessageBox.Show(ex.Message);
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

        public void UpdateEvent(BllEvent Event)
        {
            int i = 0;
            for (i = 0; i < EventList.Count; i++)
            {
                if (EventList[i].Id == Event.Id)
                {
                    if (Event.StatusLib.SelectedEntities.Count > 0)
                    {
                        var selectedEntities = EventList[i].StatusLib.SelectedEntities;
                        var newstatus = Event.StatusLib.SelectedEntities.Last();
                        if (selectedEntities.Count > 0)
                        {                          
                            var oldstatus = selectedEntities.Last();
                            if (newstatus.Date != oldstatus.Date)
                            {
                                UpdateEventStatusInDataGrid(newstatus, i, true);
                            }
                        }
                        else
                        {
                            UpdateEventStatusInDataGrid(newstatus, i, true);
                        }

                    }
                    EventList[i] = Event;
                    break;
                }
            }

            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (SelectedRowIndex == i)
                {
                    Invoke(new Action(() =>
                    {
                        FillStatusDataGrid(Event.StatusLib);
                        FillUserChecklist(Event.RecieverLib.SelectedEntities);
                        HandleStatusChanging();
                    }));
                }
            }
            SerializeEventsBackground();

            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_TURNOUT_STATUS) && notifyIcon.Visible)
            {
                TurnOutForm();
            }
            //FlashWindow.Start(this);
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_SOUND_STATUS))
            {
                SystemSounds.Beep.Play();
            }
        }

        private void MarkEventInDataGridAsClosed(int i)
        {
            ClosedEventsIndecies.Add(i);
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
            DeletedEventsIndecies.Add(i);
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

        private void EnableDeleteEventButton()
        {
            удалитьСобытиеToolStripMenuItem.Enabled = true;
        }

        private void DisableDeleteEventButton()
        {
            удалитьСобытиеToolStripMenuItem.Enabled = false;
        }

        private void FillStatusDataGrid(BllStatusLib lib)
        {
            dataGridView2.Rows.Clear();
            foreach(var item in lib.SelectedEntities)
            {
                AddStatusToDataGrid(item.Entity.Name, item.Date);
            }
        }

        private void UpdateEventStatusInDataGrid(BllSelectedStatus status, int index, bool isBold)
        {
            var cell = dataGridView1.Rows[index].Cells[4];
            cell.Value = status.Entity.Name + " " + status.Date;
            if (isBold)
            {
                cell.Style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            }
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

        private void SetTrayCommontIcon()
        {
            notifyIcon.Icon = this.Icon;
        }

        private void DealWithTrayIcon()
        {
            if (NewEventIndecies.Count != 0)
            {
                SetTrayNewEventIcon();
            }
            else
            {
                SetTrayCommontIcon();
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
        }
            

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            TurnInForm();
            e.Cancel = true;
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            try
            {
                SerializeEvents();
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

        private void button2_Click(object sender, EventArgs e)
        {
            var currentEvent = EventList[SelectedRowIndex];
            if (checkBox1.Checked)
            {
                ShowChecklist();
                if (NewEventIndecies.Contains(SelectedRowIndex))
                {
                    EventList[SelectedRowIndex].IsAdmited = true;
                    NewEventIndecies.Remove(SelectedRowIndex);
                    RowCommonFont(SelectedRowIndex);
                    DealWithMissedEventsOnTaskbar();
                    SerializeEventsBackground();
                }
                foreach (var item in currentEvent.RecieverLib.SelectedEntities)
                {
                    if (item.Entity.Id == User.Id)
                    {
                        item.IsEventAccepted = true;
                    }
                }
                currentEvent = server.UpdateAcceptedUsersAndSendOutEvent(currentEvent, User);
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

        private void FillUserChecklist(List<BllSelectedUser> users)
        {
            listView1.Items.Clear();
            foreach (var item in users)
            {
                if (!item.IsEventAccepted)
                {
                    AddUserToChecklist(item.Entity, false);
                }
            }
            int acceptedEvents = 0;
            foreach(var item in users)
            {
                if (item.IsEventAccepted)
                {
                    AddUserToChecklist(item.Entity, true);
                    acceptedEvents++;
                }
            }
            groupBox1.Text = "Ознакомились " + acceptedEvents + " из " + users.Count;
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void listBox2_DoubleClick(object sender, MouseEventArgs e)
        {
            string filename = EventList[SelectedRowIndex].FilepathLib.Entities[listBox2.SelectedIndex].Path;
            string foldername = EventList[SelectedRowIndex].FilepathLib.FolderName;
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
            if (DeletedEventsIndecies.Contains(i))
            {
                DeletedEventsIndecies.Remove(i);

            }
            if (ClosedEventsIndecies.Contains(i))
            {
                ClosedEventsIndecies.Remove(i);
            }
            if (NewEventIndecies.Contains(i))
            {
                NewEventIndecies.Remove(i);
            }
        }

        private void удалитьСобытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedRowIndex >= 0)
            {
                DeleteFromIndexLists(SelectedRowIndex);
                DealWithTrayIcon();
                RecalculateIndexList(DeletedEventsIndecies, SelectedRowIndex);
                RecalculateIndexList(ClosedEventsIndecies, SelectedRowIndex);
                RecalculateIndexList(NewEventIndecies, SelectedRowIndex);
                EventList.RemoveAt(SelectedRowIndex);
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
            if (e.StateChanged != DataGridViewElementStates.Selected) return;
            if (dataGridView1.SelectedRows.Count == 0)
            {
                ClearDataControls();
                return;
            }
            SelectedRowIndex = dataGridView1.SelectedRows[0].Index;
            
            checkBox1.Checked = false;
            if (ClosedEventsIndecies.Contains(SelectedRowIndex) || DeletedEventsIndecies.Contains(SelectedRowIndex))
            {
                EnableDeleteEventButton();
            }
            else
            {
                DisableDeleteEventButton();
            }

            
            SetSelectedEventToControls(EventList[SelectedRowIndex]);
            if (User != null)
            {
                var recievers = EventList[SelectedRowIndex].RecieverLib.SelectedEntities;
                if ((IsUserInChecklistByLogin(User, recievers)) || (User.Login == EventList[SelectedRowIndex].Sender.Login))
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
        }

        private void HandleStatusChanging()
        {
            if (EventList[SelectedRowIndex].StatusLib.SelectedEntities.Count > 0)
            {
                string currentStatusName = EventList[SelectedRowIndex].StatusLib.SelectedEntities.Last().Entity.Name;
                if (currentStatusName == Globals.Globals.STATUS_CLOSED ||
                    currentStatusName == Globals.Globals.STATUS_DELETED)
                {
                    if (User.Login == EventList[SelectedRowIndex].Sender.Login)
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
                    foreach(var i in ClosedEventsIndecies)
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
            for (int i = 0; i < EventList.Count; i++)
            {

                if (ClosedEventsIndecies.Contains(i))
                {
                    var currentStatusDate = EventList[i].StatusLib.SelectedEntities.Last().Date;
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


        int dir0col = 1;
        int dir1col = 1;
        int dir2col = -1;
        int dir4col = 1;
        int dir5col = 1;
        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                dir0col *= -1;
                OrderBySender();                
                SerializeEventsBackground();
            }
            if (e.ColumnIndex == 1)
            {
                dir1col *= -1;
                OrderByName();               
                SerializeEventsBackground();
            }
            if (e.ColumnIndex == 2)
            {
                dir2col *= -1;
                OrderByDate();               
                SerializeEventsBackground();
            }
            if (e.ColumnIndex == 4)
            {
                dir4col *= -1;
                OrderByStatus();               
                SerializeEventsBackground();
            }
            if (e.ColumnIndex == 5)
            {
                dir5col *= -1;
                OrderByAttributes();               
                SerializeEventsBackground();
            }

        }

        

        private bool OrderBySender()
        {
            SetNotesForEvents();
            EventList.Sort((x, y) => dir0col * string.Compare(x.Sender.Fullname, y.Sender.Fullname));
            RefreshDataGrid();
            OrderFunc = OrderBySender;
            UnsetNotesForEvents();
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_NUM, "0");
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_DIR, dir0col.ToString());
            return true;
        }

        private bool OrderByName()
        {
            SetNotesForEvents();
            EventList.Sort((x, y) => dir1col * string.Compare(x.Name, y.Name));
            RefreshDataGrid();
            OrderFunc = OrderByName;
            UnsetNotesForEvents();
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_NUM, "1");
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_DIR, dir1col.ToString());
            return true;
        }

        private bool OrderByDate()
        {
            SetNotesForEvents();
            EventList.Sort((x, y) => dir2col * DateTime.Compare(x.Date, y.Date));
            RefreshDataGrid();
            OrderFunc = OrderByDate;
            UnsetNotesForEvents();
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_NUM, "2");
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_DIR, dir2col.ToString());
            return true;
        }

        private bool OrderByStatus()
        {
            SetNotesForEvents();
            EventList.Sort((x, y) => dir4col * string.Compare(x.StatusLib.SelectedEntities.Count != 0 ? x.StatusLib.SelectedEntities.Last().Entity.Name : "",
                                                        y.StatusLib.SelectedEntities.Count != 0 ? y.StatusLib.SelectedEntities.Last().Entity.Name : ""));
            RefreshDataGrid();
            UnsetNotesForEvents();
            OrderFunc = OrderByStatus;
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_NUM, "3");
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_DIR, dir4col.ToString());
            return true;
        }

        private bool OrderByAttributes()
        {
            SetNotesForEvents();
            EventList.Sort((x, y) => dir5col * (x.AttributeLib.SelectedEntities.Count - y.AttributeLib.SelectedEntities.Count));
            RefreshDataGrid();
            UnsetNotesForEvents();
            OrderFunc = OrderByAttributes;
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_NUM, "4");
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_SORT_DIR, dir5col.ToString());
            return true;
        }

        private void RefreshDataGrid()
        {           
            dataGridView1.Rows.Clear();
            ClosedEventsIndecies.Clear();
            DeletedEventsIndecies.Clear();
            NewEventIndecies.Clear();
            foreach(var item in EventList)
            {
                AddEventToDataGrid(item);
            }
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_HIDE_CLOSED))
            {
                HideClosedEvents();
            }
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
    }
}
