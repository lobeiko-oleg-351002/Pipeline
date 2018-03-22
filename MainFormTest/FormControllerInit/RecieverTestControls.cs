using Client.Forms;
using Client.Forms.RecieverControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainFormTest.FormControllerInit
{
    public class RecieverTestControls
    {
        public GroupBox groupBox1;
        public GroupBox groupBox2;
        public CheckBox checkBox;
        public ListView listView;
        public Button button;

        public RecieverTestControls(IFormControllerSet set)
        {
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            checkBox = new CheckBox();
            listView = new ListView();
            button = new Button();

            var recieverControls = new RecieverControls(groupBox1, groupBox2, checkBox, button, listView, set);
            set.recieverControlsManager = new RecieverControlsManager(recieverControls);
        }
    }
}
