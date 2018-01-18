using BllEntities;
using Client.Forms;
using Globals;
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

        IBusinessService server;

        BllUser User = null;
        List<BllEvent> EventList;
        List<BllStatus> SelectedEventAvailableStatuses = new List<BllStatus>();
        BllStatus StatusDeleted = null;
        BllStatus StatusClosed = null;
        int SelectedRowIndex;
        List<int> NewEventRowIndecies = new List<int>();
        List<int> DeletedEventsIndecies = new List<int>();
        List<int> ClosedEventsIndecies = new List<int>();
        NotifyIcon notifyIcon = new NotifyIcon();
        XmlSerializer serializer = new XmlSerializer(typeof(List<BllEvent>));

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
                        SetControlsServerOnline();
                        if (User == null)
                        {
                            Authorize(server);
                        }
                        if (StatusClosed == null)
                        {
                            StatusClosed = server.GetStatusClosed();
                        }
                        if (StatusDeleted == null)
                        {
                            StatusDeleted = server.GetStatusDeleted();
                        }
                    }));
                }
                if ((value == false) && (_isServerOnline == true))
                {
                    Invoke(new Action(() =>
                    {
                        SetControlsServerOffline();
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
                if (ConfigurationManager.AppSettings[tag] != null)
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

        private void SetControlsServerOffline()
        {
            label4.Text = Properties.Resources.SERVER_OFFLINE;
            label4.ForeColor = Color.Red;
            создатьСобытиеToolStripMenuItem.Enabled = false;
            button1.Enabled = false;
        }

        private void SetControlsServerOnline()
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
                InitStatuses();
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

            comboBox1.SelectedIndex = 0;
            comboBox1.Enabled = true;
            button1.Enabled = false;

            if (StatusDeleted != null)
            {
                comboBox1.Items.Remove(StatusDeleted.Name);
                comboBox1.Items.Remove(StatusClosed.Name);
                SelectedEventAvailableStatuses.Remove(StatusDeleted);
                SelectedEventAvailableStatuses.Remove(StatusClosed);
                if (Event.Sender.Id == User.Id)
                {
                    comboBox1.Items.Add(StatusDeleted.Name);
                    comboBox1.Items.Add(StatusClosed.Name);
                    SelectedEventAvailableStatuses.Add(StatusDeleted);
                    SelectedEventAvailableStatuses.Add(StatusClosed);
                }
            }
        }

        private void AddStatusToDataGrid(string name, DateTime date)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView2);
            row.Cells[0].Value = name;
            row.Cells[1].Value = date;
            dataGridView2.Rows.Add(row);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            string ip = AppConfigManager.GetKeyValue(IP_KEY);
            server = ServiceChannelManagerSingleton.Instance.GetServerMethods(this, ip);
            EventList = new List<BllEvent>();
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.BalloonTipText = "Программа работает в фоновом режиме.";
            notifyIcon.BalloonTipTitle = Properties.Resources.CLIENT_NAME; ;
            notifyIcon.Icon = this.Icon;
            notifyIcon.Text = Properties.Resources.CLIENT_NAME; ;
            notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;

            comboBox1.Items.Add(STATUS_NOT_CHANGED);
            checkBox2.Checked = AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_OPEN_FILE_LOCATION);


             Authorize(server);
            if (!isAppClosed)
            {
                SetControlsServerOffline();
                PingServer();

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


        }

        private void PingServer()
        {
            try
            {
                if (isServerOnline == false)
                {
                    string ip = AppConfigManager.GetKeyValue(IP_KEY);
                    server = ServiceChannelManagerSingleton.Instance.GetServerMethods(this, ip);
                    Authorize(server);

                }
                server.PingServer();

                
                isServerOnline = true;
            }
            catch(Exception ex)
            {
                //MessageBox.Show(ex.Message);
                isServerOnline = false;
            }
        }

        private void InitStatuses()
        {
            if (User.StatusLib != null)
            {
                SelectedEventAvailableStatuses.Clear();
                StatusDeleted = server.GetStatusDeleted();
                StatusClosed =  server.GetStatusClosed();                    
                
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
                foreach(var item in EventList)
                {
                    AddEventToDataGrid(item);
                }
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
                    catch { success = false; }

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
                FlashWindow.Start(this);
                SetEventsCountInPanel();
                SerializeEventsBackground();
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
            if (Event.FilepathLib.Entities.Count == 0)
            {
                ((DataGridViewButtonCell)row.Cells[6]).Value = "-";
                ((DataGridViewButtonCell)row.Cells[6]).ReadOnly = true;
            }
            else
            {
                ((DataGridViewButtonCell)row.Cells[6]).Value += " " + Event.FilepathLib.Entities.Count + " ф.";
            }
            row.Cells[7].Value = Event.Description;

            dataGridView1.Rows.Add(row);
            int i = dataGridView1.Rows.Count - 1;
            if (!Event.IsAdmited && (Event.Sender.Id != User.Id))
            {
                HighlightRow(i);
                NewEventRowIndecies.Add(i);
            }

            if (Event.StatusLib.SelectedEntities.Count > 0)
            {
                if (Event.StatusLib.SelectedEntities.Last().Entity.Name == Globals.Globals.STATUS_CLOSED)
                {
                    ClosedEventsIndecies.Add(i);
                    MarkEventInDataGridAsClosed(i);
                }
                if (Event.StatusLib.SelectedEntities.Last().Entity.Name == Globals.Globals.STATUS_DELETED)
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
                if (login == "" || login == null)
                {
                    SignInForm signInForm = new SignInForm(server);
                    signInForm.ShowDialog();
                    User = signInForm.User;
                    if (User == null)
                    {
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
                if (User != null)
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
                //if (User == null)
                //{
                //    Authorize(server);
                //}
                AddEventForm addEventForm = new AddEventForm(server, User);
                addEventForm.ShowDialog();
                if (addEventForm.Event != null)
                {
                    EventList.Add(addEventForm.Event);
                    AddEventToDataGrid(addEventForm.Event);
                    SerializeEventsBackground();
                }
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

                SetEventsCountInPanel();
                if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_TURNOUT_EVENT) && notifyIcon.Visible)
                {
                    TurnOutForm();
                }
                else
                {
                    SetTrayNewEventIcon();
                }

                FlashWindow.Start(this);
                if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_SOUND_EVENT))
                {
                    SystemSounds.Beep.Play();
                }
            }));
            


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
            
            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                foreach (var name in EventList[e.RowIndex].FilepathLib.Entities)
                {
                    try
                    {
                        Process.Start(DownloadFile(name.Path, EventList[e.RowIndex].FilepathLib.FolderName));
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
                    serializer.Serialize(stream, EventList);
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

        
        private void SetEventsCountInPanel()
        {
            int n = NewEventRowIndecies.Count;
            Text = Properties.Resources.CLIENT_NAME;
            if (n > 0)
            {
                Text += " (" + n + ")";
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
                EventList[SelectedRowIndex].StatusLib.SelectedEntities.Add(new BllSelectedStatus { Entity = SelectedEventAvailableStatuses[comboBox1.SelectedIndex - 1] });
                EventList[SelectedRowIndex] = server.UpdateStatusAndSendOutEvent(EventList[SelectedRowIndex], User);
               // EventList[dataGridView1.SelectedRows[0].Index] = EventList[SelectedRowIndex];
                var newStatus = EventList[SelectedRowIndex].StatusLib.SelectedEntities.Last();
                AddStatusToDataGrid(newStatus.Entity.Name, newStatus.Date);
                UpdateEventStatusInDataGrid(newStatus, SelectedRowIndex, false);
                comboBox1.SelectedIndex = 0;
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
                        var newstatus = Event.StatusLib.SelectedEntities.Last();
                        if (EventList[i].StatusLib.SelectedEntities.Count > 0)
                        {                          
                            var oldstatus = EventList[i].StatusLib.SelectedEntities.Last();
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

        private void TurnInForm()
        {
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(3000);
            if (NewEventRowIndecies.Count != 0)
            {
                SetTrayNewEventIcon();
            }
            else
            {
                SetTrayCommontIcon();
            }
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
                foreach(var item in currentEvent.RecieverLib.SelectedEntities)
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
            if (isAccepted)
            {
                listView1.Items[listView1.Items.Count - 1].ForeColor = Color.Green;
            }
            else
            {
                listView1.Items[listView1.Items.Count - 1].ForeColor = Color.Gray;
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
            try
            {
                if (checkBox2.Checked == false)
                {
                    Process.Start(DownloadFile(EventList[SelectedRowIndex].FilepathLib.Entities[listBox2.SelectedIndex].Path, EventList[SelectedRowIndex].FilepathLib.FolderName));
                }
                else
                {
                    string path = DownloadFile(EventList[SelectedRowIndex].FilepathLib.Entities[listBox2.SelectedIndex].Path, EventList[SelectedRowIndex].FilepathLib.FolderName);
                    Process.Start("explorer.exe", "/select, \"" + path + "\"");
                }
                
            }
            catch
            {
                MessageBox.Show(Properties.Resources.CANNOT_OPEN_FILE, EventList[SelectedRowIndex].FilepathLib.Entities[listBox2.SelectedIndex].Path);
            }
        }

        private void удалитьСобытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EventList.RemoveAt(SelectedRowIndex);
            dataGridView1.Rows.RemoveAt(SelectedRowIndex);

            SerializeEventsBackground();
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
            if (NewEventRowIndecies.Contains(SelectedRowIndex))
            {
                EventList[SelectedRowIndex].IsAdmited = true;
                NewEventRowIndecies.Remove(SelectedRowIndex);
                SetEventsCountInPanel();
                SerializeEventsBackground();
            }
            RowCommonFont(SelectedRowIndex);
            SetSelectedEventToControls(EventList[SelectedRowIndex]);
            if (User != null)
            {
                if ((IsUserInChecklistByLogin(User, EventList[SelectedRowIndex].RecieverLib.SelectedEntities)) || (User.Login == EventList[SelectedRowIndex].Sender.Login))
                {
                    ShowChecklist();
                    FillUserChecklist(EventList[SelectedRowIndex].RecieverLib.SelectedEntities);
                }
                else
                {
                    if (isServerOnline)
                    {
                        ShowCheckbox();
                    }
                }
                if (EventList[SelectedRowIndex].StatusLib.SelectedEntities.Count > 0)
                {
                    if (EventList[SelectedRowIndex].StatusLib.SelectedEntities.Last().Entity.Name == Globals.Globals.STATUS_CLOSED ||
                        EventList[SelectedRowIndex].StatusLib.SelectedEntities.Last().Entity.Name == Globals.Globals.STATUS_DELETED)
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
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_OPEN_FILE_LOCATION, checkBox2.Checked.ToString());
        }
    }
}
