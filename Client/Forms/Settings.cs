using Client.Misc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms
{
    public partial class Settings : ParentForm
    {

        bool hideClosedEvents = false;

        public Settings()
        {
            InitializeComponent();

            checkedListBox1.Items.Add("Звуковое оповещение для новых событий");
            checkedListBox1.Items.Add("Звуковое оповещение при изменении статуса");
            checkedListBox1.Items.Add("Вызов программы из фонового режима при получении нового события");
            checkedListBox1.Items.Add("Вызов программы из фонового режима при изменении статуса");
            checkedListBox1.Items.Add("Автозапуск программы в фоновом режиме");
            checkedListBox1.Items.Add("Индикация на панели задач при изменении статуса");
            checkedListBox1.Items.Add("Индикация в трэе при изменении статуса");
            checkedListBox1.Items.Add("Отметка статусов для ОТК");
            checkedListBox1.Items.Add("Индикация только статуса СКЛАД");

            SetCheckListItem(Properties.Resources.TAG_SOUND_EVENT, 0);
            SetCheckListItem(Properties.Resources.TAG_SOUND_STATUS, 1);
            SetCheckListItem(Properties.Resources.TAG_TURNOUT_EVENT, 2);
            SetCheckListItem(Properties.Resources.TAG_TURNOUT_STATUS, 3);
            SetCheckListItem(Properties.Resources.TAG_STARTUP_TRAY, 4);
            SetCheckListItem(Properties.Resources.TAG_TASKBAR_INDICATION_STATUS, 5);
            SetCheckListItem(Properties.Resources.TAG_TRAY_INDICATION_STATUS, 6);
            SetCheckListItem(Properties.Resources.TAG_OTK_STATUS, 7);
            SetCheckListItem(Properties.Resources.TAG_STOCK_STATUS, 8);

            checkBox1.Checked = AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_HIDE_CLOSED);
            numericUpDown1.Value = AppConfigManager.GetIntKeyValue(Properties.Resources.TAG_HIDE_ALLOWANCE);
        }

        private void SetCheckListItem(string tag, int i)
        {
             checkedListBox1.SetItemChecked(i, AppConfigManager.GetBoolKeyValue(tag)); 
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetKeyValue(Properties.Resources.TAG_SOUND_EVENT, 0);
            SetKeyValue(Properties.Resources.TAG_SOUND_STATUS, 1);
            SetKeyValue(Properties.Resources.TAG_TURNOUT_EVENT, 2);
            SetKeyValue(Properties.Resources.TAG_TURNOUT_STATUS, 3);
            SetKeyValue(Properties.Resources.TAG_STARTUP_TRAY, 4);
            SetKeyValue(Properties.Resources.TAG_TASKBAR_INDICATION_STATUS, 5);
            SetKeyValue(Properties.Resources.TAG_TRAY_INDICATION_STATUS, 6);
            SetKeyValue(Properties.Resources.TAG_OTK_STATUS, 7);
            SetKeyValue(Properties.Resources.TAG_STOCK_STATUS, 8);

            AppConfigManager.SetKeyValue(Properties.Resources.TAG_HIDE_CLOSED, hideClosedEvents.ToString());
            AppConfigManager.SetKeyValue(Properties.Resources.TAG_HIDE_ALLOWANCE, numericUpDown1.Value.ToString());
            Close();
        }

        private void SetKeyValue(string tag, int i)
        {
            AppConfigManager.SetKeyValue(tag, checkedListBox1.GetItemChecked(i).ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                numericUpDown1.Enabled = true;
                hideClosedEvents = true;
            }
            else
            {
                numericUpDown1.Enabled = false;
                hideClosedEvents = false;
            }
        }
    }
}
