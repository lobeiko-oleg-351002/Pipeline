using BllEntities;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AdminClient.Forms.Directories.UserDirectory
{
    public partial class AddUserForm : UserDataForm
    {
        public AddUserForm()
        {
            InitializeComponent();
        }

        public AddUserForm(DirectoryForm parent, IBusinessService server, BllUser User) : base(parent, server, User)
        {
            InitializeComponent();
        }

        protected override void button1_Click(object sender, EventArgs e)
        {
            BllStatusLib statusLib = new BllStatusLib();
            foreach(int item in checkedListBox3.CheckedIndices)
            {
                statusLib.SelectedEntities.Add(new BllSelectedStatus { Entity = Statuses[item] });
            }
            BllEventTypeLib eventTypeLib = new BllEventTypeLib();
            foreach (int item in checkedListBox2.CheckedIndices)
            {
                eventTypeLib.SelectedEntities.Add(new BllSelectedEntity<BllEventType> { Entity = EventTypes[item] });
            }
            Entity = new BllUser
            {
                Fullname = textBox1.Text,
                Login = textBox2.Text,
                Password = Sha1.Encrypt(textBox3.Text),
                Group = Groups[comboBox1.SelectedIndex],
                StatusLib = statusLib,
                EventTypeLib = eventTypeLib
            };
            Entity = server.CreateUser((BllUser)Entity);
            base.button1_Click(sender, e);
            
        }
    }
}
