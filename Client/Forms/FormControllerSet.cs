using Client.EventClasses.Events;
using Client.Forms.DataGridControls;
using Client.Forms.EventControls;
using Client.Forms.FileControls;
using Client.Forms.MainFormControls;
using Client.Forms.NoteControls;
using Client.Forms.RecieverControls;
using Client.Forms.ServerStateControls;
using Client.Forms.StaticControls;
using Client.Forms.StatusControls;
using Client.Misc;
using Client.ServerManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.Forms
{
    public class FormControllerSet : IFormControllerSet
    {
        public ClientLauncher client { get; set; }
        public UiEvent SelectedEvent { get; set; }
        public Indication indication { get; set; }

        public StatusControlsManager statusControlsManager { get; set; }
        public FileControlsManager fileControlsManager { get; set; }
        public RecieverControlsManager recieverControlsManager { get; set; }
        public NoteControlsManager noteControlsManager { get; set; }
        public StaticControlsManager staticControlsManager { get; set; }
        public DataGridControlsManager dataGridControlsManager { get; set; }
        public MainFormControlsManager mainFormControlsManager { get; set; }
        public ServerStateControlsManager serverStateControlsManager { get; set; }
        public EventManager eventManager { get; set; }

    }
}
