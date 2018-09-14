using BllEntities;
using Client.EventClasses;
using Client.EventClasses.Events;
using Client.Forms;
using Client.Forms.ApproveControls;
using Client.Forms.DataGridControls;
using Client.Forms.EventControls;
using Client.Forms.FileControls;
using Client.Forms.MainFormControls;
using Client.Forms.NoteControls;
using Client.Forms.RecieverControls;
using Client.Forms.ReconcileControls;
using Client.Forms.ServerStateControls;
using Client.Forms.StaticControls;
using Client.Forms.StatusControls;
using Client.Misc;
using Client.Misc.FileService;
using Client.Misc.Printing;
using Client.ServerManager;
using Client.ServerManager.Interface;
using Globals;
using Microsoft.Win32;
using ServerInterface;
using ServiceChannelManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Client
{
    public partial class MainForm : ParentForm
    {
        public IFormControllerSet formControllerSet;

        static bool isAppClosed = false;

        //for tests
        public MainForm(string pathToClient)
        {
            InitializeComponent();
            AppConfigManager.SetExecutablePath(pathToClient);
        }

        public MainForm()
        {
            InitializeComponent();
            AppConfigManager.SetExecutablePath(Application.ExecutablePath);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            formControllerSet = new FormControllerSet();
            try
            {
                InitControlManagers();
                formControllerSet.client.Launch();
                StatusesForOwner.Init(formControllerSet.client.GetServerInstance());
                formControllerSet.eventManager.GetEventList();
                formControllerSet.eventManager.HideClosedEventsAccordingToConfigValue();
                formControllerSet.eventManager.StartOtkStatusCheckoutsOnTimer();
            }
            catch (UserIsNullException) //user has not logged in
            {
                ExitApp();
            }
            InitializeAppProperties();
            this.KeyDown += new KeyEventHandler(MainForm_KeyDown);
        }

        private void InitControlManagers()
        {
            formControllerSet.mainForm = this;
            InitStatusControls();
            InitFileControls();
            InitRecieverControls();
            InitNoteControls();
            InitStaticControls();
            InitDataGridControls();
            InitMainFormControls();
            InitServerStateControls();
            InitEventManager();
            InitApproveControls();
            InitReconcileControls();
            formControllerSet.indication = new Indication(this);
            formControllerSet.client = new ClientLauncher(formControllerSet.eventManager.clientCallback, formControllerSet.serverStateControlsManager);
        }

        private void InitStatusControls()
        {
            var statusControls = new StatusControls(dataGridView2, comboBox1, button1, formControllerSet);
            formControllerSet.statusControlsManager = new StatusControlsManager(statusControls);
            formControllerSet.statusControlsManager.AddBlankStatusToComboBox();
        }

        private void InitFileControls()
        {
            var fileControls = new FileControls(checkBox2, listBox2, formControllerSet);
            formControllerSet.fileControlsManager = new FileControlsManager(fileControls);
            formControllerSet.fileControlsManager.SetOpenFileLocationCheckBoxAccordingToConfigValue();
        }

        private void InitRecieverControls()
        {
            var recieverControls = new RecieverControls(groupBox1, groupBox4, checkBox1, button2, listView1, formControllerSet);
            formControllerSet.recieverControlsManager = new RecieverControlsManager(recieverControls);
        }

        private void InitNoteControls()
        {
            var noteControls = new NoteControls(richTextBox2, formControllerSet);
            formControllerSet.noteControlsManager = new NoteControlsManager(noteControls);
        }

        private void InitStaticControls()
        {
            var staticControls = new StaticControls(textBox2, textBox4, textBox5, textBox3, richTextBox1, formControllerSet);
            formControllerSet.staticControlsManager = new StaticControlsManager(staticControls);
        }

        private void InitDataGridControls()
        {
            var dataGridControls = new DataGridControls(dataGridView1, this, formControllerSet);
            formControllerSet.dataGridControlsManager = new DataGridControlsManager(dataGridControls); 
        }

        private void InitMainFormControls()
        {
            var mainFormControls = new MainFormControls(создатьСобытиеToolStripMenuItem1, удалитьСобытиеToolStripMenuItem, переслатьСобытиеToolStripMenuItem,
                выходToolStripMenuItem, настройкиToolStripMenuItem, formControllerSet);
            formControllerSet.mainFormControlsManager = new MainFormControlsManager(mainFormControls);
        }

        private void InitServerStateControls()
        {
            var serverStateControls = new ServerStateControls(label4, label9, this, formControllerSet);
            formControllerSet.serverStateControlsManager = new ServerStateControlsManager(serverStateControls);
        }

        private void InitEventManager()
        {
            var eventControls = new EventControls(formControllerSet);
            formControllerSet.eventManager = new EventManager(eventControls);
        }

        private void InitApproveControls()
        {
            var approveControls = new ApproveControls(radioButton1, radioButton2, button3, groupBox2, formControllerSet);
            formControllerSet.approveControlsManager = new ApproveControlsManager(approveControls);
        }

        private void InitReconcileControls()
        {
            var reconcileControls = new ReconcileControls(radioButton4, radioButton3, button4, groupBox5, formControllerSet);
            formControllerSet.reconcileControlsManager = new ReconcileControlsManager(reconcileControls);
        }

        private void AddAppShortcutToStartup()
        {
            const string launcher = "Launcher.exe";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                string currentLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                key.SetValue(Properties.Resources.CLIENT_NAME, "\"" + currentLocation + "\\" + launcher + "\"");
            }
        }

        private void TurnAppStartupInTrayAccordingToConfigValue()
        {
            if (AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_STARTUP_TRAY))
            {
                Close();
            }
        }

        private void InitializeAppProperties()
        {
            AddAppShortcutToStartup();
            TurnAppStartupInTrayAccordingToConfigValue();
        }

        public void ExitApp()
        {
            Application.Exit();
            Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isAppClosed)
            {
                e.Cancel = true;
                formControllerSet.indication.HideForm();
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isAppClosed = true;
            formControllerSet.eventManager.SerializeEvents();
            Application.Exit();
        }

        public static void SaveAndExit()
        {
            isAppClosed = true;
            Application.Exit();
        }

        private static int WM_QUERYENDSESSION = 0x11;
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == WM_QUERYENDSESSION)
            {
                SaveAndExit();
            }

            base.WndProc(ref m);
        }

        private void печатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintHandler printHandler = new PrintHandler(printDocument1);
            printHandler.PrintDataGrid(dataGridView1);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.P)
            {
                formControllerSet.eventManager.GetAllEventsForSender(formControllerSet.client.GetUser());
            }
        }
    }
}
