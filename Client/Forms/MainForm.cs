using BllEntities;
using Client.Forms;
using ServerInterface;
using ServiceChannelManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;

namespace Client
{
    public partial class MainForm : ParentForm, IServerCallBack
    {
        public MainForm()
        {
            InitializeComponent();
        }

        const string loginTag = "login";
        const string passwordTag = "password";
        const string dateFormat = "dd.MM.yyyy";
        const string timeFormat = "HH:mm";

        IMethods server;

        BllUser User = null;
        List<BllEvent> EventList;



        private void Form1_Load(object sender, EventArgs e)
        {
            server = ServiceChannelManagerSingleton.Instance.GetServerMethods(this);
            EventList = new List<BllEvent>();

            Authorize(server);
            GetEventList();

        }

        private void GetEventList()
        {
            EventList = server.GetEventsForUser(User);
            foreach(var item in EventList)
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
            row.Cells[2].Value = Event.Date.Date.ToString(dateFormat);
            row.Cells[3].Value = Event.Date.ToString(timeFormat);
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

        private void Authorize(IMethods server)
        {
            string login = ConfigurationManager.AppSettings[loginTag];
            string password = ConfigurationManager.AppSettings[passwordTag];

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
                textBox1.Text = User.Login + User.Group.Name + User.ChangeRights;
                server.RegisterClient(User.Login);
            }
        }

        private void WriteLoginAndPasswordToConfig(string login, string password)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            config.AppSettings.Settings.Add(loginTag, login);
            config.AppSettings.Settings.Add(passwordTag, password);
            config.Save(ConfigurationSaveMode.Minimal);
        }

        private void ExitApp()
        {
            Application.Exit();
        }

        private void создатьСобытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddEventForm addEventForm = new AddEventForm(server, User);
            addEventForm.ShowDialog();
        }

        public void GetEvent(BllEvent Event)
        {
            AddEventToDataGrid(Event);
            MessageBox.Show("event");
        }

        public void Ping()
        {
            //MessageBox.Show("Вас пингонули!");
        }
    }
}
