using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.FileControls
{
    public class FileControls : FormControls
    {
        public readonly CheckBox ShowLocation;
        public readonly ListBox FileList;

        public FileControls(CheckBox showLocation, ListBox fileList, IFormControllerSet set) : base(set)
        {
            this.ShowLocation = showLocation;
            this.FileList = fileList;
        }

    }
}
