using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.Forms.MainFormControls
{
    public interface IMessageBox
    {
        void ShowMessage(string text, string title);
    }
}
