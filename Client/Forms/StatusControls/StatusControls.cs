using BllEntities;
using Client.EventClasses;
using Client.ServerManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.StatusControls
{
    public class StatusControls : FormControls
    {
        public readonly DataGridView StatusTable;
        public readonly ComboBox AvailableStatuses;
        public readonly Button ChangeStatusButton;

        public StatusControls(DataGridView StatusTable, ComboBox AvailableStatuses, Button ChangeStatusButton, IFormControllerSet set) : base(set)
        {
            this.StatusTable = StatusTable;
            this.AvailableStatuses = AvailableStatuses;
            this.ChangeStatusButton = ChangeStatusButton;
        }
    }
}
