using BllEntities;
using BllEntities.Interface;
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
    public partial class UserDataForm : EntityDataForm
    {
        public UserDataForm()
        {
            InitializeComponent();
        }

        protected List<BllGroup> Groups;
        protected List<BllStatus> Statuses;
        protected List<BllEventType> EventTypes;

        public UserDataForm(DirectoryForm parent, IBusinessService server, IBllEntity entity) : base(parent, server, entity)
        {
            InitializeComponent();

            Groups = server.GetAllGroups();
            foreach(var item in Groups)
            {
                comboBox1.Items.Add(item.Name);
            }
            if (Groups.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }

            Statuses = server.GetAllStatusesExceptDeletedAndClosed();
            foreach(var item in Statuses)
            {
                checkedListBox3.Items.Add(item.Name);
            }

            EventTypes = server.GetAllEventTypes();
            foreach (var item in EventTypes)
            {
                checkedListBox2.Items.Add(item.Name);
            }



        }
    }
}
