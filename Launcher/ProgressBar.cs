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
            InitializeComponent();
        }

        private void ProgressBar_Load(object sender, EventArgs e)
        {
            string currentLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            try
            {
                ExeConfigurationFileMap map = new ExeConfigurationFileMap { ExeConfigFilename = currentLocation + "\\Client.exe.config" };
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

                string ip = ConfigurationManager.AppSettings["hostIP"];
                ILauncherMethods SourceChannel = CreateChannel<ILauncherMethods>("http://" + ip + "/LauncherService/");
                var ver = (config.GetSection("appSettings") as AppSettingsSection).Settings["Version"];
                string currentVersion = SourceChannel.GetClientVersion();
                string appVersion = "";
                if (ver != null)
                {
                    appVersion = ver.Value;
                }
    
                if (appVersion != currentVersion)
                {
                    string updatePath = SourceChannel.GetUpdatePath();
                    var settings = (config.GetSection("appSettings") as AppSettingsSection);
                    string login = "";
                    string password = "";
                    if (settings.Settings["login"] != null)
                    {
                        login = settings.Settings["login"].Value;
                        password = settings.Settings["password"].Value;
                    }
                        
                    CopyDirectory(updatePath, currentLocation);
                    map = new ExeConfigurationFileMap { ExeConfigFilename = currentLocation + "\\Client.exe.config" };
                    config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                    settings = (config.GetSection("appSettings") as AppSettingsSection);
                    settings.Settings.Add("Version", currentVersion);
                    settings.Settings.Add("login", login);
                    settings.Settings.Add("password", password);
                    config.Save(ConfigurationSaveMode.Modified);
                }
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Process client = new Process();
            client.StartInfo.FileName = currentLocation + "\\Client.exe";
            client.Start();
            Application.Exit();
        }
    }
}
