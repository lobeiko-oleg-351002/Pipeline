using AdminClient.Forms.Directories.AttributeDirectory;
using AdminClient.Forms.Directories.EventTypeDirectory;
using AdminClient.Forms.Directories.GroupDirectory;
using AdminClient.Forms.Directories.StatusDirectory;
using AdminClient.Forms.Directories.UserDirectory;
using BllEntities;
using ServerInterface;
using ServiceChannelManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AdminClient.Forms
{
    public partial class MainForm : ParentForm, IClientCallBack
    {
        public MainForm()
        {
            InitializeComponent();
        }

        const string DATE_FORMAT = "dd.MM.yyyy";
        const string TIME_FORMAT = "HH:mm";
        const string STATUS_NOT_CHANGED = "Статус не изменён";
        const string IP_KEY = "hostIP";

        const int PING_SLEEPTIME_MS = 10000;

        List<BllUser> UserList;

        bool isAppClosed;

        private bool _isServerOnline;

        bool isServerOnline
        {
            get { return _isServerOnline; }
            set
            {
                if ((value == true) && (_isServerOnline == false))
                {
                    Invoke(new Action(() =>
                    {
                        SetControlsAccordingToServerOnline();
                    }));
                }
                if ((value == false) && (_isServerOnline == true))
                {
                    Invoke(new Action(() =>
                    {
                        SetControlsAccordingToServerOffline();
                    }));


                }
                _isServerOnline = value;
            }
        }

        private void SetControlsAccordingToServerOnline()
        {
            textBox1.Text = GetConstFromResources("SERVER_ONLINE");
            служебныеОбъектыToolStripMenuItem.Enabled = true;
        }

        private void SetControlsAccordingToServerOffline()
        {
            textBox1.Text = GetConstFromResources("SERVER_OFFLINE");
            служебныеОбъектыToolStripMenuItem.Enabled = false;
        }

        private string GetConstFromResources(string name)
        {
            return Properties.Resources.ResourceManager.GetString(name);
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            string ip = ConfigurationManager.AppSettings[IP_KEY];
            server = ServiceChannelManagerSingleton.Instance.GetServerMethods(this, ip);
            UserList = new List<BllUser>();

            SetControlsAccordingToServerOffline();
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

            label2.Text = server.GetClientVersion();
        }

        private void PingServer()
        {
            try
            {
                if (isServerOnline == false)
                {
                    string ip = ConfigurationManager.AppSettings[IP_KEY];
                    server = ServiceChannelManagerSingleton.Instance.GetServerMethods(this, ip);
                }
                server.PingServer();
                isServerOnline = true;
            }
            catch (Exception ex)
            {
                isServerOnline = false;
            }
        }

     
        private void ExitApp()
        {
            Application.Exit();
            isAppClosed = true;
            Close();
        }

        public void Ping()
        {

        }

        public void GetEvent(BllEvent Event)
        {
            throw new NotImplementedException();
        }

        public void UpdateEvent(BllEvent Event)
        {
            throw new NotImplementedException();
        }

        private void статусыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatusDirectoryForm form = new StatusDirectoryForm(server);
            form.ShowDialog();
        }

        private void пользователиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserDirectoryForm form = new UserDirectoryForm(server);
            form.ShowDialog();
        }

        private void аттрибутыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AttributeDirectoryForm form = new AttributeDirectoryForm(server);
            form.ShowDialog();
        }

        private void типыСобытийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EventTypeDirectoryForm form = new EventTypeDirectoryForm(server);
            form.ShowDialog();
        }

        private void группыПользователейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GroupDirectoryForm form = new GroupDirectoryForm(server);
            form.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            server.SetClientVersion((Double.Parse(label2.Text) + 0.01).ToString());
            label2.Text = server.GetClientVersion();
        }
    }
}
