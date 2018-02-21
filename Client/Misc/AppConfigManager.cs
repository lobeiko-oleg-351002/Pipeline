using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Client.Misc
{
    public static class AppConfigManager
    {
        private static string ExecutablePath;

        public static void SetExecutablePath(string path)
        {
            ExecutablePath = path;
        }

        public static string GetKeyValue(string tag)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ExecutablePath);
            if (config.AppSettings.Settings[tag] != null)
            {
                return config.AppSettings.Settings[tag].Value;
            }
            return null;
        }

        public static void SetKeyValue(string tag, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ExecutablePath);
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
                Configuration config = ConfigurationManager.OpenExeConfiguration(ExecutablePath);
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
                Configuration config = ConfigurationManager.OpenExeConfiguration(ExecutablePath);
                config.AppSettings.Settings.Add(tag, "0");
                return 0;
            }
        }

        public static void ClearTagValues(string tag)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ExecutablePath);
            if (config.AppSettings.Settings[tag] != null)
            {
                config.AppSettings.Settings[tag].Value = "";
                config.Save(ConfigurationSaveMode.Minimal);
            }
        }

        public static void AddKeyValue(string tag, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ExecutablePath);
            config.AppSettings.Settings.Add(tag, value);
            config.Save(ConfigurationSaveMode.Minimal);
        }
    }
}
