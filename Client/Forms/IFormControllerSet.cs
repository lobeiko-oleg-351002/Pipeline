using Client.EventClasses;
using Client.EventClasses.Events;
using Client.Forms.ApproveControls;
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
    public interface IFormControllerSet
    {
        UiEvent SelectedEvent { get; set; }
        ClientLauncher client { get; set; }
        EventManager eventManager { get; set; }
        Indication indication { get; set; }

        FileControlsManager fileControlsManager { get; set; }
        NoteControlsManager noteControlsManager { get; set; }
        RecieverControlsManager recieverControlsManager { get; set; }
        StaticControlsManager staticControlsManager { get; set; }
        StatusControlsManager statusControlsManager { get; set; }
        DataGridControlsManager dataGridControlsManager { get; set; }
        MainFormControlsManager mainFormControlsManager { get; set; }
        ServerStateControlsManager serverStateControlsManager { get; set; }
        ApproveControlsManager approveControlsManager { get; set; }
    }
}
