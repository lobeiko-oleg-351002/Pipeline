using Client.Forms;
using Client.Forms.EventControls;
using Client.Forms.StatusControls;
using Client.Misc;
using Client.ServerManager;
using MainFormTest.FormControllerInit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainFormTest
{
    public class TestFormControls
    {
        public StatusTestControls statusTestControls;
        public FileTestControls fileTestControls;
        public RecieverTestControls recieverTestControls;
        public NoteTestControls noteTestControls;
        public StaticTestControls staticTestControls;
        public DataGridTestControls dataGridTestControls;
        public MainFormTestControls mainFormTestControls;
        public ServerStateTestControls serverStateTestControls;

        public void InitFormController(IFormControllerSet formControllerSet)
        {
            InitStatusControls(formControllerSet);
            InitFileControls(formControllerSet);
            InitRecieverControls(formControllerSet);
            InitNoteControls(formControllerSet);
            InitStaticControls(formControllerSet);
            InitDataGridControls(formControllerSet);
            InitMainFormControls(formControllerSet);
            InitServerStateControls(formControllerSet);
            InitEventManager(formControllerSet);
            formControllerSet.indication = new Indication(new System.Windows.Forms.Form());
            formControllerSet.client = new ClientLauncher(formControllerSet.eventManager.clientCallback, formControllerSet.serverStateControlsManager);
        }

        private void InitStatusControls(IFormControllerSet formControllerSet)
        {
            statusTestControls = new StatusTestControls(formControllerSet);
            formControllerSet.statusControlsManager.AddBlankStatusToComboBox();
        }

        private void InitFileControls(IFormControllerSet formControllerSet)
        {
            fileTestControls = new FileTestControls(formControllerSet);
        }

        private void InitRecieverControls(IFormControllerSet formControllerSet)
        {
            recieverTestControls = new RecieverTestControls(formControllerSet);
        }

        private void InitNoteControls(IFormControllerSet formControllerSet)
        {
            noteTestControls = new NoteTestControls(formControllerSet);
        }

        private void InitStaticControls(IFormControllerSet formControllerSet)
        {
            staticTestControls = new StaticTestControls(formControllerSet);
        }

        private void InitDataGridControls(IFormControllerSet formControllerSet)
        {
            dataGridTestControls = new DataGridTestControls(formControllerSet);
        }

        private void InitMainFormControls(IFormControllerSet formControllerSet)
        {
            mainFormTestControls = new MainFormTestControls(formControllerSet);
        }

        private void InitServerStateControls(IFormControllerSet formControllerSet)
        {
            serverStateTestControls = new ServerStateTestControls(formControllerSet);
        }

        private void InitEventManager(IFormControllerSet formControllerSet)
        {
            var eventControls = new EventControls(formControllerSet);
            formControllerSet.eventManager = new EventManager(eventControls);
        }
    }
}
