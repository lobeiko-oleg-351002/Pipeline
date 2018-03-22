using Client.Forms;
using Client.Forms.NoteControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainFormTest.FormControllerInit
{
    public class NoteTestControls
    {
        public RichTextBox richTextBox;

        public NoteTestControls(IFormControllerSet set)
        {
            richTextBox = new RichTextBox();
            var noteControls = new NoteControls(richTextBox, set);
            set.noteControlsManager = new NoteControlsManager(noteControls);
        }
    }
}
