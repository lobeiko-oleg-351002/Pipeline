using BllEntities;
using Client.EventClasses;
using Client.EventClasses.Events;
using Client.Forms;
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
using Client.Misc.FileService;
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
    public partial class MainForm : ParentForm, IFormControllerSet
    {
        public ClientLauncher client {get; set; }
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
            try
            {
                InitControlManagers();
                client = new ClientLauncher(eventManager.clientCallback, serverStateControlsManager);
                client.Launch();
                StatusesForOwner.Init(client.GetServerInstance());
                eventManager.GetEventList();
                eventManager.HideClosedEventsAccordingToConfigValue();
                eventManager.StartOtkStatusCheckoutsOnTimer();
            }
            catch (UserIsNullException) //user has not logged in
            {
                ExitApp();
            }
            InitializeAppProperties();
        }

        private void InitControlManagers()
        {
            InitStatusControls();
            InitFileControls();
            InitRecieverControls();
            InitNoteControls();
            InitStaticControls();
            InitDataGridControls();
            InitMainFormControls();
            InitServerStateControls();
            InitEventManager();
            indication = new Indication(this);
        }

        private void InitStatusControls()
        {
            var statusControls = new StatusControls(dataGridView2, comboBox1, button1, this);
            statusControlsManager = new StatusControlsManager(statusControls);
            statusControlsManager.AddBlankStatusToComboBox();
        }

        private void InitFileControls()
        {
            var fileControls = new FileControls(checkBox2, listBox2, this);
            fileControlsManager = new FileControlsManager(fileControls);
            fileControlsManager.SetOpenFileLocationCheckBoxAccordingToConfigValue();
        }

        private void InitRecieverControls()
        {
            var recieverControls = new RecieverControls(groupBox1, groupBox4, checkBox1, button2, listView1, this);
            recieverControlsManager = new RecieverControlsManager(recieverControls);
        }

        private void InitNoteControls()
        {
            var noteControls = new NoteControls(richTextBox2, this);
            noteControlsManager = new NoteControlsManager(noteControls);
        }

        private void InitStaticControls()
        {
            var staticControls = new StaticControls(textBox2, textBox4, textBox5, textBox3, richTextBox1, this);
            staticControlsManager = new StaticControlsManager(staticControls);
        }

        private void InitDataGridControls()
        {
            var dataGridControls = new DataGridControls(dataGridView1, this, this);
            dataGridControlsManager = new DataGridControlsManager(dataGridControls); 
        }

        private void InitMainFormControls()
        {
            var mainFormControls = new MainFormControls(создатьСобытиеToolStripMenuItem1, удалитьСобытиеToolStripMenuItem, переслатьСобытиеToolStripMenuItem,
                выходToolStripMenuItem, настройкиToolStripMenuItem, this);
            mainFormControlsManager = new MainFormControlsManager(mainFormControls);
        }

        private void InitServerStateControls()
        {
            var serverStateControls = new ServerStateControls(label4, label9, this, this);
            serverStateControlsManager = new ServerStateControlsManager(serverStateControls);
        }

        private void InitEventManager()
        {
            var eventControls = new EventControls(this);
            eventManager = new EventManager(eventControls);
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
                indication.HideForm();
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isAppClosed = true;
            eventManager.SerializeEvents();
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
    }
}
