using BllEntities;
using Client.Forms;
using ServerInterface;
using ServiceChannelManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Client
{
    public partial class MainForm : ParentForm, IServerCallBack
    {
        public MainForm()
        {
            InitializeComponent();
        }

        const string LOGIN_TAG = "login";
        const string PASSWORD_TAG = "password";
        const string DATE_FORMAT = "dd.MM.yyyy";
        const string TIME_FORMAT = "HH:mm";

        const int PING_SLEEPTIME_MS = 10000;

        IBusinessService server;

        BllUser User = null;
        List<BllEvent> EventList;

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
                        textBox1.Text = Properties.Resources.ResourceManager.GetString("SERVER_ONLINE");
                        создатьСобытиеToolStripMenuItem.Enabled = true;
                    }));
                }
                if ((value == false) && (_isServerOnline == true))
                {
                    Invoke(new Action(() =>
                    {
                        textBox1.Text = Properties.Resources.ResourceManager.GetString("SERVER_OFFLINE");
                        создатьСобытиеToolStripMenuItem.Enabled = false;
                    }));

 
                }
                _isServerOnline = value;
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            server = ServiceChannelManagerSingleton.Instance.GetServerMethods(this);
            EventList = new List<BllEvent>();

            Authorize(server);
            if (!isAppClosed)
            {
                GetEventList();
                new Thread(() =>
                {
                    while (!isAppClosed)
                    {
                        Thread.CurrentThread.IsBackground = true;
                        PingServer();
                        Thread.Sleep(PING_SLEEPTIME_MS);
                    }
                }).Start();
            }
        }

        private void PingServer()
        {
            try
            {
                if (isServerOnline == false)
                {
                    server = ServiceChannelManagerSingleton.Instance.GetServerMethods(this);
                }
                server.PingServer();
                isServerOnline = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                isServerOnline = false;
            }
        }

        private void GetEventList()
        {
            EventList = server.GetEventsForUser(User);
            foreach (var item in EventList)
            {
                AddEventToDataGrid(item);
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
            var statusCell = ((DataGridViewComboBoxCell)row.Cells[4]);
            foreach (var status in Event.StatusLib.SelectedEntities)
            {                
                statusCell.Items.Add(status.Entity.Name + " " +  status.Date);
            }
            if (statusCell.Items.Count != 0)
            {
                statusCell.Value = statusCell.Items[statusCell.Items.Count - 1];
            }
            foreach (var attr in Event.AttributeLib.SelectedEntities)
            {
                row.Cells[5].Value += attr.Entity.Name + "; ";
            }
            ((DataGridViewButtonCell)row.Cells[6]).Value += " " + Event.FilepathLib.Entities.Count + " ф.";
            row.Cells[7].Value = Event.Description;

            dataGridView1.Rows.Add(row);
        }

        private void Authorize(IBusinessService server)
        {
            string login = ConfigurationManager.AppSettings[LOGIN_TAG];
            string password = ConfigurationManager.AppSettings[PASSWORD_TAG];
            try
            {
                if (login == null)
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
                    server.RegisterClient(User.Login);
                }
            }
            catch
            {
                MessageBox.Show(Properties.Resources.ResourceManager.GetString("SERVER_NOT_FOUND"));
                ExitApp();
            }
        }

        private void WriteLoginAndPasswordToConfig(string login, string password)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            config.AppSettings.Settings.Add(LOGIN_TAG, login);
            config.AppSettings.Settings.Add(PASSWORD_TAG, password);
            config.Save(ConfigurationSaveMode.Minimal);
        }

        private void ExitApp()
        {
            Application.Exit();
            isAppClosed = true;
            Close();
        }

        private void создатьСобытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddEventForm addEventForm = new AddEventForm(server, User);
            addEventForm.ShowDialog();
            if (addEventForm.Event != null)
            {
                EventList.Add(addEventForm.Event);
                AddEventToDataGrid(addEventForm.Event);
            }
        }

        public void GetEvent(BllEvent Event)
        {
            EventList.Add(Event);
            AddEventToDataGrid(Event);
            MessageBox.Show("event");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            string downloadPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Properties.Resources.ResourceManager.GetString("DOWNLOADS_FOLDER");
            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                foreach (var name in EventList[e.RowIndex].FilepathLib.Entities)
                {
                    string fullPath = downloadPath + "\\" + name.Path;
                    if (!File.Exists(fullPath))
                    {
                        using (FileStream output = new FileStream(fullPath, FileMode.Create))
                        {
                            Stream downloadStream;

                            using (FileServiceClient client = new FileServiceClient())
                            {
                                downloadStream = client.GetFile(name.Path);
                            }

                            downloadStream.CopyTo(output);
                        }
                    }
                    Process.Start(fullPath);
                }
            }
        }

        public void Ping()
        {
            
        }
    }
}
