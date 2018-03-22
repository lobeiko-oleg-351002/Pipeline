using Client.Forms;
using Client.Forms.ServerStateControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainFormTest.FormControllerInit
{
    public class ServerStateTestControls
    {
        Label label1;
        Label label2;
        
        public ServerStateTestControls(IFormControllerSet set)
        {
            label1 = new Label();
            label2 = new Label();
            Form form = new Form();
            form.Show();

            var serverStateControls = new ServerStateControls(label1, label2, form, set);
            set.serverStateControlsManager = new ServerStateControlsManager(serverStateControls);
        }
    }
}
