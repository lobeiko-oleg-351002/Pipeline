using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.ApproveControls
{
    public class ApproveControls : FormControls
    {
        public readonly RadioButton approveSwitch;
        public readonly RadioButton disapproveSwitch;
        public readonly Button approveButton;
        public readonly GroupBox groupBox;

        public ApproveControls(RadioButton approveSwitch, RadioButton disapproveSwitch, Button approveButton, GroupBox groupBox, IFormControllerSet set) : base(set)
        {
            this.approveButton = approveButton;
            this.approveSwitch = approveSwitch;
            this.disapproveSwitch = disapproveSwitch;
            this.groupBox = groupBox;
        }
    }
}
