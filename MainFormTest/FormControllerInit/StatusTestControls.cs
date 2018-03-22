using Client.Forms;
using Client.Forms.StatusControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainFormTest.FormControllerInit
{
    public class StatusTestControls
    {
        public DataGridView dataGridView;
        public ComboBox comboBox;
        public Button button;

        public StatusTestControls(IFormControllerSet set)
        {
            dataGridView = new DataGridView();
            dataGridView.Columns.Add("Column1", "Статус");
            dataGridView.Columns.Add("Column2", "Дата");

            comboBox = new ComboBox();

            button = new Button();

            var statusControls = new StatusControls(dataGridView, comboBox, button, set);
            set.statusControlsManager = new StatusControlsManager(statusControls);
        }
    }
}
