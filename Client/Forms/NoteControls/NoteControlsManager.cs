using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.Forms.NoteControls
{
    public class NoteControlsManager
    {
        private readonly NoteControls noteControls;

        bool noteTextBoxChangesProgramatically;

        public NoteControlsManager(NoteControls noteControls)
        {
            this.noteControls = noteControls;
            noteControls.Note.Leave += richTextBox_Leave;
            noteControls.Note.TextChanged += richTextBox_TextChanged;
        }

        public void SetEventNoteUsingCellValue(string note)
        {
            noteControls.Note.Text = note;
            noteControls.Note.Enabled = true;
        }

        public void DisableTextBox()
        {
            noteControls.Note.Enabled = false;
        }

        public void EnableTextBox()
        {
            noteControls.Note.Enabled = true;
        }

        public void ClearControls()
        {
            noteControls.Note.Clear();
        }

        private void richTextBox_Leave(object sender, EventArgs e)
        {
            noteControls.ControllerSet.eventManager.SerializeEvents();
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!noteTextBoxChangesProgramatically)
            {
                noteControls.ControllerSet.eventManager.SetEventNote(noteControls.Note.Text);
            }
        }

        public void ClearNoteTextBox()
        {
            noteTextBoxChangesProgramatically = true;
            ClearControls();
            noteTextBoxChangesProgramatically = false;
        }
    }
}
