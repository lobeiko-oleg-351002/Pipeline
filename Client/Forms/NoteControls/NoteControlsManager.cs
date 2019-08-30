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
            noteControls.ControllerSet.eventManager.SerializeEventsBackground();
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!noteTextBoxChangesProgramatically)
                {
                    noteControls.ControllerSet.eventManager.SetEventNote(noteControls.Note.Text);
                }
                if (noteControls.ControllerSet.client.GetUser().Group.Name == Globals.Globals.VED_GROUP)
                {

                    noteControls.ControllerSet.SelectedEvent.EventData.CustomerNote = noteControls.Note.Text;
                    noteControls.ControllerSet.eventManager.UpdateEvent(noteControls.ControllerSet.SelectedEvent);
                }
            }
            catch { }
        }

        public void ClearNoteTextBox()
        {
            noteTextBoxChangesProgramatically = true;
            ClearControls();
            noteTextBoxChangesProgramatically = false;
        }
    }
}
