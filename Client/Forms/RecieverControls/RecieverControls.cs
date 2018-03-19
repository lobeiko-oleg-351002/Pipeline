using Client.EventClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.RecieverControls
{
    public class RecieverControls : FormControls
    {
        public readonly GroupBox GroupBoxForRecievers;
        public readonly GroupBox GroupBoxForAcquaintButton;
        public readonly CheckBox Recieved;
        public readonly Button Acquaint;
        public readonly ListView Recievers;

        public RecieverControls(GroupBox groupBoxForRecievers, GroupBox groupBoxForAcquaintButton, CheckBox checkBox, Button button, ListView listView,
            IFormControllerSet mainForm) : base(mainForm)
        {
            this.GroupBoxForAcquaintButton = groupBoxForAcquaintButton;
            this.GroupBoxForRecievers = groupBoxForRecievers;
            this.Recieved = checkBox;
            this.Acquaint = button;
            this.Recievers = listView;
        }

    }
}
