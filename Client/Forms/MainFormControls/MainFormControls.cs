using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms.MainFormControls
{
    public class MainFormControls : FormControls
    {
        public readonly ToolStripMenuItem CreateEventButton;
        public readonly ToolStripMenuItem DeleteEventButton;
        public readonly ToolStripMenuItem SendOnEventButton;
        public readonly ToolStripMenuItem ExitButton;
        public readonly ToolStripMenuItem SettingsButton;
        public readonly ToolStripMenuItem RefreshButton;
        public readonly IMessageBox MessageBox;

        public MainFormControls(ToolStripMenuItem refreshButton, ToolStripMenuItem createEventButton, ToolStripMenuItem deleteEventButton, 
            ToolStripMenuItem sendOnEventButton, ToolStripMenuItem exitButton, ToolStripMenuItem settingsButton, IFormControllerSet set, IMessageBox messageBox) : base(set)
        {
            this.CreateEventButton = createEventButton;
            this.DeleteEventButton = deleteEventButton;
            this.ExitButton = exitButton;
            this.SendOnEventButton = sendOnEventButton;
            this.SettingsButton = settingsButton;
            this.MessageBox = messageBox;
            this.RefreshButton = refreshButton;
        }
    }
}
