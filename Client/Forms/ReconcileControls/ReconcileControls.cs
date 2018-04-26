using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.ReconcileControls
{
    public class ReconcileControls : FormControls
    {
        public readonly RadioButton agreeSwitch;
        public readonly RadioButton disagreeSwitch;
        public readonly Button reconcileButton;
        public readonly GroupBox groupBox;

        public ReconcileControls(RadioButton agreeSwitch, RadioButton disagreeSwitch, Button reconcileButton, GroupBox groupBox, IFormControllerSet set) : base(set)
        {
            this.reconcileButton = reconcileButton;
            this.agreeSwitch = agreeSwitch;
            this.disagreeSwitch = disagreeSwitch;
            this.groupBox = groupBox;
        }
    }
}
