using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.NoteControls
{
    public class NoteControls : FormControls
    {
        public readonly RichTextBox Note;
        
        public NoteControls(RichTextBox Note, IFormControllerSet set) : base(set)
        {
            this.Note = Note;
        }
    }
}
