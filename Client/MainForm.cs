using BllEntities;
using ServerInterface;
using ServiceChannelManager;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace Client
{
    public partial class MainForm : ParentForm
    {
        public MainForm()
        {
            InitializeComponent();
        }

        const string loginTag = "login";
        const string passwordTag = "password";

        private void Form1_Load(object sender, EventArgs e)
        {
            IMethods server = ServiceChannelManagerSingleton.Instance.Methods;

            string login = ConfigurationManager.AppSettings[loginTag];
            string password = ConfigurationManager.AppSettings[passwordTag];
            BllUser user = null;
            if (login == null)
            {
                SignInForm signInForm = new SignInForm(server);
                signInForm.ShowDialog();
                user = signInForm.User;
                if (user == null)
                {
                    ExitApp();
                }
                else
                {
                    WriteLoginAndPasswordToConfig(user.Login, user.Password);
                }
            }
            else
            {
                user = server.SignIn(login, password);
            }
            if (user != null)
            {
                textBox1.Text = user.Login + user.Group.Name + user.ChangeRights;
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
    }
}
