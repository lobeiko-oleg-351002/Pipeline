using Client.Forms;
using Client.Forms.FileControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainFormTest.FormControllerInit
{
    public class FileTestControls
    {
        public ListBox listBox;
        public CheckBox checkBox;

        public FileTestControls(IFormControllerSet set)
        {
            listBox = new ListBox();
            checkBox = new CheckBox();

            var fileControls = new FileControls(checkBox, listBox, set);
            set.fileControlsManager = new FileControlsManager(fileControls);
        }
    }
}
