using Client.Forms;
using Client.Forms.DataGridControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainFormTest.FormControllerInit
{
    public class DataGridTestControls
    {
        public DataGridView dataGridView;

        public DataGridTestControls(IFormControllerSet set)
        {
            dataGridView = new DataGridView();
            Form form = new Form();
            form.Show();

            var dataGridControls = new DataGridControls(dataGridView, form, set);
            set.dataGridControlsManager = new DataGridControlsManager(dataGridControls);
        }
    }
}
