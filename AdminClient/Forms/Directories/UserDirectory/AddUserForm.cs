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
            Entity = new BllUser
            {
                Fullname = textBox1.Text,
                Login = textBox2.Text,
                Password = Sha1.Encrypt(textBox3.Text),
                CreateRights = checkedListBox1.CheckedIndices.Contains(0),
                ChangeRights = checkedListBox1.CheckedIndices.Contains(1),
                Group = Groups[comboBox1.SelectedIndex]
            };
            Entity = server.CreateUser((BllUser)Entity);
            base.button1_Click(sender, e);
            
        }
    }
}
