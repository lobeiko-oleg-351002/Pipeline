using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.ServerStateControls
{
    public class ServerStateControls : FormControls
    {
        public readonly Label ServerState;
        public readonly Label UserName;

        public ServerStateControls(Label serverState, Label userName, Control parentFormControl, IFormControllerSet set) : base(set) 
        {
            this.ServerState = serverState;
            this.UserName = userName;
        }
    }
}
