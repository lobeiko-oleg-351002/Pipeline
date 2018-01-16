using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms
{
    public partial class Settings : ParentForm
    {
        Configuration config;

        public Settings()
        {
            InitializeComponent();
            config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            checkedListBox1.Items.Add("Звуковое оповещение для новых событий");
            checkedListBox1.Items.Add("Звуковое оповещение при изменении статуса");
            checkedListBox1.Items.Add("Вызов программы из фонового режима при получении нового события");
            checkedListBox1.Items.Add("Вызов программы из фонового режима при изменении статуса");
            SetCheckListItem(Properties.Resources.TAG_SOUND_EVENT, 0);
            SetCheckListItem(Properties.Resources.TAG_SOUND_STATUS, 1);
            SetCheckListItem(Properties.Resources.TAG_TURNOUT_EVENT, 2);
            SetCheckListItem(Properties.Resources.TAG_TURNOUT_STATUS, 3);
        }

        private void SetCheckListItem(string tag, int i)
        {
            if (ConfigurationManager.AppSettings[tag] != null)
            {
                checkedListBox1.SetItemChecked(i, bool.Parse(config.AppSettings.Settings[tag].Value)); 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetKeyValue(Properties.Resources.TAG_SOUND_EVENT, 0);
            SetKeyValue(Properties.Resources.TAG_SOUND_STATUS, 1);
            SetKeyValue(Properties.Resources.TAG_TURNOUT_EVENT, 2);
            SetKeyValue(Properties.Resources.TAG_TURNOUT_STATUS, 3);
            config.Save(ConfigurationSaveMode.Minimal);
            Close();
        }

        private void SetKeyValue(string tag, int i)
        {
            if (ConfigurationManager.AppSettings[tag] != null)
            {
                config.AppSettings.Settings[tag].Value = checkedListBox1.GetItemChecked(i).ToString();
            }
            else
            {
                config.AppSettings.Settings.Add(tag, checkedListBox1.GetItemChecked(i).ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
