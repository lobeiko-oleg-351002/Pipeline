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
        public readonly Control ParentFormControl;

        public DataGridControls(DataGridView dataGrid, Control parentFormControl, IFormControllerSet mainForm) : base(mainForm)
        {
            this.DataGrid = dataGrid;
            this.ParentFormControl = parentFormControl;
        }
    }
}
