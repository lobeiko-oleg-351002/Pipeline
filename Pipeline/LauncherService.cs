using ServerInterfaceForLauncher;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Server
{
    public class LauncherService : ILauncherMethods
    {


        private const string TAG_VERSION = "CLIENT_VERSION";
        private const string UPDATE_PATH = "UPDATE_PATH";

        public string GetClientVersion()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[TAG_VERSION] != null)
            {
                return config.AppSettings.Settings[TAG_VERSION].Value;
            }
            return null;
        }

        public string GetUpdatePath()
        {
            return ConfigurationManager.AppSettings[UPDATE_PATH];
        }
    }
}
