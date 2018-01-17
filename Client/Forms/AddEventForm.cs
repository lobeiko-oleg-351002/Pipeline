using BllEntities;
using BllEntities.Interface;
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
        IBusinessService server;
        public BllEvent Event { get; private set; }
        List<BllAttribute> Attributes;
        List<BllUser> Users = new List<BllUser>();
        List<string> Filepaths = new List<string>();
        BllUser Sender;

        public AddEventForm()
        {
            InitializeComponent();
        }

        public AddEventForm(IBusinessService server, BllUser sender)
        {
            InitializeComponent();
            this.server = server;
            this.Sender = sender;


            foreach (var EventType in sender.EventTypeLib.SelectedEntities)
            {
                 comboBox2.Items.Add(EventType.Entity.Name);            
            }
            if (comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0;
                
            }

            Attributes = server.GetAllAttributes();
            foreach(var attribute in Attributes)
            {
                checkedListBox1.Items.Add(attribute.Name);
            }

            List<BllGroup> groups = server.GetAllGroups();

            foreach(var group in groups)
            {
                var node = treeView1.Nodes.Add(group.Name);
                var users = server.GetUsersByGroup(group);
                foreach(var user in users)
                {
                    if (user.Id != Sender.Id)
                    {
                        node.Nodes.Add(user.Fullname);
                        Users.Add(user);
                    }
                }
            }
            CheckUserNodesAccordingEventType(sender.EventTypeLib.SelectedEntities[0].Entity.Id);



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
            string nodesStr = MainForm.AppConfigManager.GetKeyValue(eventId.ToString());
            if (nodesStr != null)
            {
                List<string> nodes = nodesStr.Split(',').ToList();

                foreach (TreeNode groupNode in treeView1.Nodes)
                {
                    if (nodes.Contains(groupNode.Text))
                    {
                        groupNode.Checked = true;
                    }
                    foreach (TreeNode userNode in groupNode.Nodes)
                    {
                        if (nodes.Contains(userNode.Text))
                        {
                            userNode.Checked = true;
                        }
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
            Event.IsAdmited = true;
            Event.FilepathLib = new BllFilepathLib();
            bool success = UploadFiles(Event.FilepathLib);
            if (!success)
            {
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
            MainForm.AppConfigManager.ClearTagValues(Event.Type.Id.ToString());
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
            Event = server.CreateAndSendOutEvent(Event);
            Close();
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
            textBox1.Text = comboBox2.Text;
            CheckUserNodesAccordingEventType(Sender.EventTypeLib.SelectedEntities[comboBox2.SelectedIndex].Entity.Id);
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
            int selectedIndex = listBox1.SelectedIndex;
            Filepaths.RemoveAt(selectedIndex);
            listBox1.Items.RemoveAt(selectedIndex);
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
            MainForm.AppConfigManager.AddKeyValue(eventTag, nodeName);
                

        }
    }
}
