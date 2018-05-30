using BllEntities;
using BllEntities.Interface;
using Client.EventClasses;
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
        List<BllUser> Approvers = new List<BllUser>();
        List<BllUser> Reconcilers = new List<BllUser>();
        BllUser Sender;

        const string APPROVER_IN_RECONCILERS = "Утверждающий не должен участвовать в согласовании!";
        const string EMPTY_RECONCILERS = "Не выбрано ни одного пользователя для согласования!";

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
            PopulateReconcilers();
            PopulateApproversComboBox();
            if (Sender.EventTypeLib.SelectedEntities.Count > 0)
            {
                CheckUserNodesAccordingToEventType(Sender.EventTypeLib.SelectedEntities[0].Entity.Id);
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
            AttributeService attributeService = new AttributeService(serverInstance);
            Attributes = attributeService.GetAttributes();
            foreach (var attribute in Attributes)
            {
                checkedListBox1.Items.Add(attribute.Name);
            }
        }

        private void PopulateRecieverTreeView()
        {
            GroupService groupService = new GroupService(serverInstance);
            var groups = groupService.GetGroups();
            UserService userService = new UserService(serverInstance);
            foreach (var group in groups)
            {
                var node = treeView1.Nodes.Add(group.Name);
                var usersByGroup = userService.GetUsersByGroup(group);
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

        private void PopulateApproversComboBox()
        {
            UserService userService = new UserService(serverInstance);
            Approvers = userService.GetApprovers();
            RemoveSenderFromApproverList();
            foreach (var user in Approvers)
            {
                comboBox1.Items.Add(user.Fullname);
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
                checkBox1.Checked = AppConfigManager.GetBoolKeyValue(Properties.Resources.TAG_ADDEVENT_APPROVE_BOX);
            }
            else
            {
                checkBox1.Enabled = false;
                comboBox1.Enabled = false;
            }
        }

        private void RemoveSenderFromApproverList()
        {
            try
            {
                Approvers.Remove(Approvers.Single(e => e.Id == Sender.Id));
            }
            catch
            {

            }
        }

        public void PopulateReconcilers()
        {
            checkedListBox2.Enabled = false;
            UserService userService = new UserService(serverInstance);
            Reconcilers = userService.GetReconcilers();
            RemoveSenderFromReconcilerList();
            foreach (var user in Reconcilers)
            {
                checkedListBox2.Items.Add(user.Fullname);
            }
            if (checkedListBox2.Items.Count == 0)
            {
                checkBox2.Enabled = false;
            }
        }

        public void RemoveSenderFromReconcilerList()
        {
            try
            {
                Reconcilers.Remove(Reconcilers.Single(e => e.Id == Sender.Id));
            }
            catch
            {

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

        private void CheckUserNodesAccordingToEventType(int eventId)
        {
            UncheckAllNodes(treeView1.Nodes);
            treeView1.CollapseAll();
            string nodesStr = AppConfigManager.GetKeyValue(eventId.ToString());
            if (nodesStr != null)
            {
                List<string> nodes = nodesStr.Split(',').ToList();
                foreach (TreeNode groupNode in treeView1.Nodes)
                {
                    ToggleNodesWithCheckedUsers(groupNode, nodes);
                }
            }
        }

        private void ToggleNodesWithCheckedUsers(TreeNode groupNode, List<string> checkedNodes)
        {
            bool toggle = false;
            if (checkedNodes.Contains(groupNode.Text))
            {
                groupNode.Checked = true;
            }
            foreach (TreeNode userNode in groupNode.Nodes)
            {
                if (checkedNodes.Contains(userNode.Text))
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

        private void button4_Click(object sender, EventArgs e)
        {
            Event = null;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Event = new BllEvent();
            Event.Name = textBox1.Text;
            Event.Sender = this.Sender;
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

            SetRecieversFromTreeView(Event);

            CreateEventAccordingToApproversAndReconcilers();

            AppConfigManager.SetKeyValue(Properties.Resources.TAG_ADDEVENT_APPROVE_BOX, checkBox1.Checked.ToString());
        }

        private void CreateEventAccordingToApproversAndReconcilers()
        {
            try
            {
                IEventCRUD eventCRUD = new EventCRUD(serverInstance.server);
                if (!checkBox1.Checked && !checkBox2.Checked)
                {
                    Event.IsApproved = true;
                    CallCreateMethod(eventCRUD.CreateAndSendOutEvent, Event);
                }
                if (!checkBox1.Checked && checkBox2.Checked)
                {
                    if (IsReconcilerListEmpty())
                    {
                        MessageBox.Show(EMPTY_RECONCILERS);
                        return;
                    }

                    Event.IsApproved = true;
                    Event.ReconcilerLib = new BllReconcilerLib();

                    foreach (var item in checkedListBox2.CheckedIndices.Cast<int>().ToArray())
                    {
                        Event.ReconcilerLib.SelectedEntities.Add(new BllSelectedUserReconciler { Entity = Reconcilers[item] });
                    }
                    RemoveReconcilersFromRecievers();

                    CallCreateMethod(eventCRUD.CreateEventAndSendToReconcilers, Event);
                }
                if (checkBox1.Checked && !checkBox2.Checked)
                {
                    Event.Approver = Approvers[comboBox1.SelectedIndex];
                    Event.RecieverLib.SelectedEntities.Add(new BllSelectedUser { Entity = Event.Approver, IsEventAccepted = false });
                    CallCreateMethod(eventCRUD.CreateEventAndSendToApprover, Event);
                }
                if (checkBox1.Checked && checkBox2.Checked)
                {
                    CheckConflicstWithReconcilersAndApprover();
                    Event.Approver = Approvers[comboBox1.SelectedIndex];
                    Event.ReconcilerLib = new BllReconcilerLib();
                    Event.RecieverLib.SelectedEntities.Add(new BllSelectedUser { Entity = Event.Approver, IsEventAccepted = false });

                    foreach (var item in checkedListBox2.CheckedIndices.Cast<int>().ToArray())
                    {
                        Event.ReconcilerLib.SelectedEntities.Add(new BllSelectedUserReconciler { Entity = Reconcilers[item] });
                    }
                    RemoveReconcilersFromRecievers();

                    CallCreateMethod(eventCRUD.CreateEventAndSendToReconcilers, Event);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void CallCreateMethod(Func<BllEvent, BllEvent> method, BllEvent arg)
        {
            try
            {               
                Event = method(Event);
                AppConfigManager.SetKeyValue(Event.Type.Name, textBox1.Text);
                Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetRecieversFromTreeView(BllEvent Event)
        {
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
                        SaveUserNodeAccordingEventTypeInConfig(userNode.Text);

                        if (checkBox1.Checked && (userNode.Text == comboBox1.Text))
                        {
                            continue;
                        }

                        Event.RecieverLib.SelectedEntities.Add(new BllSelectedUser { Entity = Users[userNode.Index + nodeCount] });
                    }
                }
                nodeCount += groupNode.GetNodeCount(false);
            }
        }

        private void RemoveReconcilersFromRecievers()
        {
            for (int i = 0; i < Event.RecieverLib.SelectedEntities.Count;)
            {
                var reciever = Event.RecieverLib.SelectedEntities[i].Entity;
                i++;
                foreach (var item in Event.ReconcilerLib.SelectedEntities)
                {
                    if (reciever.Id == item.Entity.Id)
                    {
                        i--;
                        Event.RecieverLib.SelectedEntities.RemoveAt(i);
                        break;
                    }
                }
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
            CheckUserNodesAccordingToEventType(type.Id);
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                comboBox1.Enabled = true;
            }
            else
            {
                comboBox1.Enabled = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkedListBox2.Enabled = true;
            }
            else
            {
                checkedListBox2.Enabled = false;
            }
        }

        private void CheckConflicstWithReconcilersAndApprover()
        {
            if (IsApproverInReconcilers())
            {
                throw new Exception(APPROVER_IN_RECONCILERS);
            }
            if (IsReconcilerListEmpty())
            {
                throw new Exception(EMPTY_RECONCILERS);
            }
        }

        private bool IsApproverInReconcilers()
        {
            for(int i = 0; i < checkedListBox2.CheckedIndices.Count; i++)
            {
                if (EventHelper.AreUsersEqual(Approvers[comboBox1.SelectedIndex], Reconcilers[checkedListBox2.CheckedIndices[i]]))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsReconcilerListEmpty()
        {
            if (checkedListBox2.CheckedItems.Count == 0)
            {
                return true;
            }
            return false;
        }
    }
}
