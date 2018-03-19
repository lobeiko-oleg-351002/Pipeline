using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.DataGridControls
{
    public class DataGridControls : FormControls
    {
        public readonly DataGridView DataGrid;

        public DataGridControls(DataGridView dataGrid, IFormControllerSet mainForm) : base(mainForm)
        {
            this.DataGrid = dataGrid;
        }
    }
}
