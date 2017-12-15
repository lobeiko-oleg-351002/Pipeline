using ServerInterface;
using ServiceChannelManager;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            //config.AppSettings.Settings.Remove("Version");
            //config.AppSettings.Settings.Add("Version", "0.11");
            //config.Save(ConfigurationSaveMode.Minimal);

            IMethods server = ServiceChannelManagerSingleton.Instance.Methods;
            textBox1.Text = "asd";

        }
    }
}
