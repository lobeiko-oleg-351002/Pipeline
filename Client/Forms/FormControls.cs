using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.Forms
{
    public class FormControls
    {
        public readonly IFormControllerSet ControllerSet;

        public FormControls(IFormControllerSet set)
        {
            ControllerSet = set;
        }
    }
}
