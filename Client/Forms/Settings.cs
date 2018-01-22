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


        public Settings()
        {
            InitializeComponent();
            checkedListBox1.Items.Add("Звуковое оповещение для новых событий");
            checkedListBox1.Items.Add("Звуковое оповещение при изменении статуса");
            checkedListBox1.Items.Add("Вызов программы из фонового режима при получении нового события");
            checkedListBox1.Items.Add("Вызов программы из фонового режима при изменении статуса");
            checkedListBox1.Items.Add("Автозапуск программы в фоновом режиме");
            SetCheckListItem(Properties.Resources.TAG_SOUND_EVENT, 0);
            SetCheckListItem(Properties.Resources.TAG_SOUND_STATUS, 1);
            SetCheckListItem(Properties.Resources.TAG_TURNOUT_EVENT, 2);
            SetCheckListItem(Properties.Resources.TAG_TURNOUT_STATUS, 3);
            SetCheckListItem(Properties.Resources.TAG_STARTUP_TRAY, 4);
        }

        private void SetCheckListItem(string tag, int i)
        {
             checkedListBox1.SetItemChecked(i, MainForm.AppConfigManager.GetBoolKeyValue(tag)); 
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetKeyValue(Properties.Resources.TAG_SOUND_EVENT, 0);
            SetKeyValue(Properties.Resources.TAG_SOUND_STATUS, 1);
            SetKeyValue(Properties.Resources.TAG_TURNOUT_EVENT, 2);
            SetKeyValue(Properties.Resources.TAG_TURNOUT_STATUS, 3);
            SetKeyValue(Properties.Resources.TAG_STARTUP_TRAY, 4);
            Close();
        }

        private void SetKeyValue(string tag, int i)
        {
            MainForm.AppConfigManager.SetKeyValue(tag, checkedListBox1.GetItemChecked(i).ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
