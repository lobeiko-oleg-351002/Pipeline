using Globals;
using ServerInterfaceForLauncher;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;

namespace Launcher
{
    public partial class ProgressBar : Form
    {
        private static T CreateChannel<T>(string serviceAddress)
        {
            EndpointIdentity spn = EndpointIdentity.CreateSpnIdentity("Server"); // dns
            Uri uri = new Uri(serviceAddress);
            var address = new EndpointAddress(uri, spn);
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 2147483647;
            ChannelFactory<T> factory = new ChannelFactory<T>(binding, address);
            return factory.CreateChannel();
        }



        private void CopyDirectory(string sourceLocation, string destinationLocation)
        {
            int fileCount = Directory.GetFiles(sourceLocation, "*.*", SearchOption.AllDirectories).Count();
            progressBar1.Step = (int)(progressBar1.Maximum / (float)fileCount);
            foreach (string newPath in Directory.GetFiles(sourceLocation, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourceLocation, destinationLocation), true);
                progressBar1.PerformStep();
            }
        }

        public ProgressBar()
        {
            MessageBox.Show("Init comp");
            InitializeComponent();
            MessageBox.Show("Form has started");
        }

        private void ProgressBar_Load(object sender, EventArgs e)
        {
            string currentLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            MessageBox.Show("Launcher has started. Current location is " + currentLocation);
            try
            {
                ExeConfigurationFileMap map = new ExeConfigurationFileMap { ExeConfigFilename = currentLocation + "\\Client.exe.config" };
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                const string TAG_APPSETTINGS = "appSettings";
                const string TAG_VERSION = "Version";
                MessageBox.Show("Config is opened");

                string ip = ConfigurationManager.AppSettings["hostIP"];
                ILauncherMethods SourceChannel = CreateChannel<ILauncherMethods>("http://" + ip + "/LauncherService/");
                var ver = (config.GetSection(TAG_APPSETTINGS) as AppSettingsSection).Settings[TAG_VERSION];
                string currentVersion = SourceChannel.GetClientVersion();
                string appVersion = "";
                MessageBox.Show("Last client version is " + currentVersion);
                if (ver != null)
                {
                    appVersion = ver.Value;
                }
    
                if (appVersion != currentVersion)
                {
                    string updatePath = SourceChannel.GetUpdatePath();
                    var clientSettings = (config.GetSection(TAG_APPSETTINGS) as AppSettingsSection);
                    string login = "";
                    string password = "";
                    if (clientSettings.Settings["login"] != null)
                    {
                        login = clientSettings.Settings["login"].Value;
                        password = clientSettings.Settings["password"].Value;
                    }
                        
                    CopyDirectory(updatePath, currentLocation);
                    map = new ExeConfigurationFileMap { ExeConfigFilename = currentLocation + "\\Client.exe.config" };
                    config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                    var settings = (config.GetSection(TAG_APPSETTINGS) as AppSettingsSection);
                    settings.Settings.Add(TAG_VERSION, currentVersion);
                    foreach (var key in clientSettings.Settings.AllKeys)
                    {
                        if (key != TAG_VERSION)
                        {
                            settings.Settings.Add(key, clientSettings.Settings[key].Value);
                        }
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                }
                MessageBox.Show("Client is ready to start ");
                Process client = new Process();
                client.StartInfo.FileName = currentLocation + "\\Client.exe";
                client.Start();
                MessageBox.Show("Client has started.");
                Application.Exit();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
    }
}
