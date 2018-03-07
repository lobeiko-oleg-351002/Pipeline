using BllEntities;
using BllEntities.Interface;
using Client.Misc;
using Client.ServerManager;
using Client.ServerManager.Interface;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Client.Forms
{
    public partial class AddEventForm : ParentForm
    {
        ServerInstance serverInstance;
        public BllEvent Event { get; private set; }
        List<BllAttribute> Attributes;
        List<BllUser> Users = new List<BllUser>();
        List<string> Filepaths = new List<string>();
        BllUser Sender;
        
        public AddEventForm()
        {
            InitializeComponent();
        }

        public AddEventForm(ServerInstance server, BllUser sender)
        {
            InitializeComponent();
            this.serverInstance = server;
            this.Sender = sender;
        }

        private void AddEventForm_Load(object sender, EventArgs e)
        {
            PopulateEventTypeComboBox(Sender.EventTypeLib);
            PopulateAttributeCheckList();
            PopulateRecieverTreeView();

            if (Sender.EventTypeLib.SelectedEntities.Count > 0)
            {
                CheckUserNodesAccordingEventType(Sender.EventTypeLib.SelectedEntities[0].Entity.Id);
            }
        }

        private void PopulateEventTypeComboBox(BllEventTypeLib lib)
        {
            comboBox2.Items.Clear();
            foreach (var EventType in lib.SelectedEntities)
            {
                comboBox2.Items.Add(EventType.Entity.Name);
            }
            if (comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0;
            }
        }

        private void PopulateAttributeCheckList()
        {
            bool success = false;
            while (!success)
            {
                try
                {
                    IAttributeGetter ag = new AttributeGetter(serverInstance.server);
                    Attributes = ag.GetAllAttributes();
                    success = true;
                }
                catch
                {
                    serverInstance.ConnectToServer();
                    success = false;
                }
            }
            foreach (var attribute in Attributes)
            {
                checkedListBox1.Items.Add(attribute.Name);
            }
        }

        private void PopulateRecieverTreeView()
        {
            bool success = false;
            List<BllGroup> groups = null;
            while (!success)
            {
                try
                {
                    IGroupGetter gg = new GroupGetter(serverInstance.server);
                    groups = gg.GetAllGroups();
                    success = true;
                }
                catch
                {
                    serverInstance.ConnectToServer();
                    success = false;
                }
            }

            foreach (var group in groups)
            {
                var node = treeView1.Nodes.Add(group.Name);
                List<BllUser> usersByGroup = new List<BllUser>();
                success = false;
                while (!success)
                {
                    try
                    {
                        IUserGetter ug = new UserGetter(serverInstance.server);
                        usersByGroup = ug.GetUsersByGroupAndSignInDateRange(group, int.Parse(Properties.Resources.PERMISSIBLE_DATE_RANGE_IN_DAYS));
                        success = true;
                    }
                    catch
                    {
                        serverInstance.ConnectToServer();
                        success = false;
                    }
                }
                foreach (var user in usersByGroup)
                {
                    if (user.Id != Sender.Id)
                    {
                        node.Nodes.Add(user.Fullname);
                        Users.Add(user);
                    }
                }
            }
        }

        public void UncheckAllNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.Checked = false;
                CheckChildren(node, false);
            }
        }

        private void CheckChildren(TreeNode rootNode, bool isChecked)
        {
            foreach (TreeNode node in rootNode.Nodes)
            {
                CheckChildren(node, isChecked);
                node.Checked = isChecked;
            }
        }

        private void CheckUserNodesAccordingEventType(int eventId)
        {
            UncheckAllNodes(treeView1.Nodes);
            treeView1.CollapseAll();
            string nodesStr = AppConfigManager.GetKeyValue(eventId.ToString());
            if (nodesStr != null)
            {
                List<string> nodes = nodesStr.Split(',').ToList();

                foreach (TreeNode groupNode in treeView1.Nodes)
                {
                    bool toggle = false;
                    if (nodes.Contains(groupNode.Text))
                    {
                        groupNode.Checked = true;
                    }
                    foreach (TreeNode userNode in groupNode.Nodes)
                    {
                        if (nodes.Contains(userNode.Text))
                        {
                            userNode.Checked = true;
                            toggle = true;
                        }
                    }
                    if (toggle)
                    {
                        groupNode.Toggle();
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Event = null;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Event = new BllEvent();
            Event.Name = textBox1.Text;
            Event.FilepathLib = new BllFilepathLib();
            try
            {
                UploadFiles(Event.FilepathLib);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            Event.Type = Sender.EventTypeLib.SelectedEntities[comboBox2.SelectedIndex].Entity;
            Event.Description = richTextBox1.Text;
            Event.StatusLib = new BllStatusLib();
            Event.AttributeLib = new BllAttributeLib();
            foreach(var item in checkedListBox1.CheckedIndices.Cast<int>().ToArray())
            {
                Event.AttributeLib.SelectedEntities.Add(new BllSelectedEntity<BllAttribute>() { Entity = Attributes[item]});
            }
            Event.RecieverLib = new BllUserLib();
            Event.RecieverLib.SelectedEntities.Add(new BllSelectedUser { Entity = Sender, IsEventAccepted = true });
            int nodeCount = 0;
            AppConfigManager.ClearTagValues(Event.Type.Id.ToString());
            foreach (TreeNode groupNode in treeView1.Nodes)
            {
                if (groupNode.Checked)
                {
                    SaveUserNodeAccordingEventTypeInConfig(groupNode.Text);
                }
                foreach (TreeNode userNode in groupNode.Nodes)
                {
                    if (userNode.Checked)
                    {
                        Event.RecieverLib.SelectedEntities.Add(new BllSelectedUser { Entity = Users[userNode.Index + nodeCount] });
                        SaveUserNodeAccordingEventTypeInConfig(userNode.Text);
                    }
                }
                nodeCount += groupNode.GetNodeCount(false);
            }

            Event.Sender = this.Sender;
            try
            {
                IEventCRUD eventCRUD = new EventCRUD(serverInstance.server);
                Event = eventCRUD.CreateAndSendOutEvent(Event);
                Close();
                AppConfigManager.SetKeyValue(Event.Type.Name, textBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //ConnectToServerUsingParent();
            }
        }

        private bool UploadFiles(BllFilepathLib lib)
        {
            string foldername = Event.Name + " " + DateTime.Now.ToString("dd.MM.yy H-mm-ss");
            Event.FilepathLib.FolderName = foldername;
            foreach (var path in Filepaths)
            {
                string fileName = Path.GetFileName(path);
                
                using (Stream uploadStream = new FileStream(path, FileMode.Open))
                {
                    using (FileServiceClient fileService = new FileServiceClient())
                    {
                        var msg = new FileUploadMessage()
                        {
                            VirtualPath = fileName,
                            DataStream = uploadStream,
                            FolderName = foldername
                        };
                        fileService.PutFile(msg);
                    }
                }
                Event.FilepathLib.Entities.Add(new BllFilepath { Path = fileName});
            }
            //}
            return true;
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var type = Sender.EventTypeLib.SelectedEntities[comboBox2.SelectedIndex].Entity;
            textBox1.Text = AppConfigManager.GetKeyValue(type.Name);
            if (textBox1.Text == "")
            {
                textBox1.Text = comboBox2.Text;
            }
            CheckUserNodesAccordingEventType(type.Id);
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckTreeViewNode(e.Node, e.Node.Checked);
        }

        private void CheckTreeViewNode(TreeNode node, Boolean isChecked)
        {
            foreach (TreeNode item in node.Nodes)
            {
                item.Checked = isChecked;

                if (item.Nodes.Count > 0)
                {
                    CheckTreeViewNode(item, isChecked);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Filepaths.Add(openFileDialog1.FileName);
                listBox1.Items.Add(openFileDialog1.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                int selectedIndex = listBox1.SelectedIndex;
                Filepaths.RemoveAt(selectedIndex);
                listBox1.Items.RemoveAt(selectedIndex);
            }
        }


        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            string pattern = "[\\\\~#%&*{}/:<>?|\"]";

            Regex regEx = new Regex(pattern);
            Match match = regEx.Match(textBox1.Text);
            if (match.Success)
            {
                e.Cancel = true;
                ToolTip tt = new ToolTip();
                tt.Show(Properties.Resources.INVALID_INPUT, textBox1, 0, 0, 4000);
            }
        }

        private void SaveUserNodeAccordingEventTypeInConfig(string nodeName)
        {
            string eventTag = Event.Type.Id.ToString();
            AppConfigManager.AddKeyValue(eventTag, nodeName);
        }


    }
}
