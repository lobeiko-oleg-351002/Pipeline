using Client.Forms;
using Client.Forms.MainFormControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainFormTest.FormControllerInit
{
    public class MainFormTestControls
    {
        ToolStripMenuItem item1;
        ToolStripMenuItem item2;
        ToolStripMenuItem item3;
        ToolStripMenuItem item4;
        ToolStripMenuItem item5;

        public MainFormTestControls(IFormControllerSet set)
        {
            item1 = new ToolStripMenuItem();
            item2 = new ToolStripMenuItem();
            item3 = new ToolStripMenuItem();
            item4 = new ToolStripMenuItem();
            item5 = new ToolStripMenuItem();

            var mainFormTestControls = new MainFormControls(item1, item2, item3, item4, item5, set);
            set.mainFormControlsManager = new MainFormControlsManager(mainFormTestControls);
        }
    }
}
