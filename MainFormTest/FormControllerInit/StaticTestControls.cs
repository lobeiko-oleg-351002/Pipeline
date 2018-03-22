using Client.Forms;
using Client.Forms.StaticControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainFormTest.FormControllerInit
{
    public class StaticTestControls
    {
        public TextBox sender;
        public TextBox date;
        public TextBox time;
        public TextBox title;
        public RichTextBox description;

        public StaticTestControls(IFormControllerSet set)
        {
            sender = new TextBox();
            date = new TextBox();
            time = new TextBox();
            title = new TextBox();
            description = new RichTextBox();

            var staticControls = new StaticControls(sender, date, time, title, description, set);
            set.staticControlsManager = new StaticControlsManager(staticControls);
        }
    }
}
