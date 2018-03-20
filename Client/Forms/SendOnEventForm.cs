using BllEntities;
using Client.ServerManager;
using Client.ServerManager.Interface;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms
{
    public partial class SendOnEventForm : ParentForm
    {
        public SendOnEventForm()
        {
            InitializeComponent();
        }

        List<BllUser> Users = new List<BllUser>();
        BllEvent Event;
        ServerInstance serverInstance;

        public SendOnEventForm(ServerInstance serverInstance, BllEvent Event, BllUser sender)
        {
            InitializeComponent();
            this.Event = Event;
            this.serverInstance = serverInstance;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
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

        private void button2_Click(object sender, EventArgs e)
        {
            List<BllUser> newRecievers = GetNewRecieversAccordingToCheckedNodes();
            try
            {
                IEventCRUD eventCRUD = new EventCRUD(serverInstance.server);
                eventCRUD.UpdateRecieversAndSendOnEvent(Event, newRecievers);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private List<BllUser> GetNewRecieversAccordingToCheckedNodes()
        {
            List<BllUser> newRecievers = new List<BllUser>();
            int nodeCount = 0;
            foreach (TreeNode groupNode in treeView1.Nodes)
            {
                foreach (TreeNode userNode in groupNode.Nodes)
                {
                    if (userNode.Checked)
                    {
                        newRecievers.Add(Users[userNode.Index + nodeCount]);
                    }
                }
                nodeCount += groupNode.GetNodeCount(false);
            }
            return newRecievers;
        }

        private void SendOnEventForm_Load(object sender, EventArgs e)
        {
            PopulateRecieverTreeView();
        }

        private void PopulateRecieverTreeView()
        {
            GroupService groupService = new GroupService(serverInstance);
            List<BllGroup> groups = groupService.GetGroups();

            SetTreeViewNodes(groups);
        }

        private void AddNewRecieversToTreeViewNode(IEnumerable<BllUser> users, TreeNode node)
        {
            foreach (var user in users)
            {
                bool isUserMatch = false;
                foreach (var item in Event.RecieverLib.SelectedEntities)
                {
                    if (item.Entity.Id == user.Id)
                    {
                        isUserMatch = true;
                        break;
                    }
                }
                if (isUserMatch == false)
                {
                    node.Nodes.Add(user.Fullname);
                    Users.Add(user);
                }
            }
        }

        private void SetTreeViewNodes(List<BllGroup> groups)
        {
            UserService userService = new UserService(serverInstance);
            foreach (var group in groups)
            {
                var node = treeView1.Nodes.Add(group.Name);
                List<BllUser> users = userService.GetUsersByGroup(group);
                AddNewRecieversToTreeViewNode(users, node);
                if (node.Nodes.Count == 0)
                {
                    treeView1.Nodes.Remove(node);
                }
            }
        }
    }
}
