﻿using BllEntities;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.Forms
{
    public partial class AddEventForm : ParentForm
    {
        IBusinessService server;
        public BllEvent Event { get; private set; }
        List<BllStatus> Statuses;
        List<BllEventType> EventTypes;
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
            Event = new BllEvent();

            EventTypes = server.GetAllEventTypes().ToList();
            foreach (var EventType in EventTypes)
            {
                comboBox2.Items.Add(EventType.Name);
            }
            if (comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0;
            }

            Statuses = server.GetAllStatuses().ToList();
            foreach(var status in Statuses)
            {
                comboBox1.Items.Add(status.Name);
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }

            Attributes = server.GetAllAttributes().ToList();
            foreach(var attribute in Attributes)
            {
                checkedListBox1.Items.Add(attribute.Name);
            }

            IEnumerable<BllGroup> groups = server.GetAllGroups();

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



        }

        private void button4_Click(object sender, EventArgs e)
        {
            Event = null;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Event.Name = textBox1.Text;
            Event.Type = EventTypes[comboBox2.SelectedIndex];
            Event.Description = richTextBox1.Text;
            Event.StatusLib = new BllStatusLib();
            Event.StatusLib.SelectedEntities.Add(new BllSelectedStatus { Entity = Statuses[comboBox1.SelectedIndex] });
            Event.AttributeLib = new BllAttributeLib();
            foreach(var item in checkedListBox1.CheckedIndices.Cast<int>().ToArray())
            {
                Event.AttributeLib.SelectedEntities.Add(new BllSelectedEntity<BllAttribute>() { Entity = Attributes[item]});
            }
            Event.RecieverLib = new BllUserLib();
            Event.RecieverLib.SelectedEntities.Add(new BllSelectedEntity<BllUser>() { Entity = Sender });
            int nodeCount = 0;
            foreach (TreeNode groupNode in treeView1.Nodes)
            {
                foreach (TreeNode userNode in groupNode.Nodes)
                {
                    if (userNode.Checked)
                    {
                        Event.RecieverLib.SelectedEntities.Add(new BllSelectedEntity<BllUser>() { Entity = Users[userNode.Index + nodeCount] });
                    }
                }
                nodeCount += groupNode.GetNodeCount(false);
            }

            Event.FilepathLib = new BllFilepathLib();
            UploadFiles(Event.FilepathLib);

            Event.Sender = this.Sender;
            Event = server.CreateAndSendOutEvent(Event);
            Close();
        }

        private void UploadFiles(BllFilepathLib lib)
        {           
            foreach (var path in Filepaths)
            {
                string virtualPath = Path.GetFileName(path);

                using (Stream uploadStream = new FileStream(path, FileMode.Open))
                {
                    using (FileServiceClient fileService = new FileServiceClient())
                    {
                        fileService.PutFile(new FileUploadMessage()
                        {
                            VirtualPath = virtualPath,
                            DataStream = uploadStream
                        });
                    }
                }
                Event.FilepathLib.Entities.Add(new BllFilepath { Path = virtualPath });
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = comboBox2.Text;
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
    }
}
