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
    public partial class AmendUserForm : UserDataForm
    {
        public AmendUserForm()
        {
            InitializeComponent();
        }

        public AmendUserForm(DirectoryForm parent, IBusinessService server, BllUser entity) : base(parent, server, entity)
        {
            InitializeComponent();
            textBox1.Text = entity.Fullname;
            textBox2.Text = entity.Login;

            foreach(var entityItem in entity.EventTypeLib.SelectedEntities)
            {
                int i = BllEntityComparer.GetItemIndex(entityItem.Entity, EventTypes.ToList<IBllEntity>());
                if (i >= 0)
                {
                    checkedListBox2.SetItemChecked(i, true);
                    break;
                }
            }

            foreach (var entityItem in entity.StatusLib.SelectedEntities)
            {
                int i = BllEntityComparer.GetItemIndex(entityItem.Entity, Statuses.ToList<IBllEntity>());
                if (i >= 0)
                {
                    checkedListBox3.SetItemChecked(i, true);
                    break;
                }
            }

            comboBox1.SelectedItem = entity.Group.Name;

        }



        protected override void button1_Click(object sender, EventArgs e)
        {
            BllUser User = (BllUser)Entity;

            User.Fullname = textBox1.Text;
            User.Login = textBox2.Text;
            User.Password = Sha1.Encrypt(textBox3.Text);
            User.Group = Groups[comboBox1.SelectedIndex];
            UpdateEventTypeLib(User);
            UpdateStatusLib(User);


            Entity = server.UpdateUser(User);
            base.button1_Click(sender, e);
            
        }

        private void UpdateStatusLib(BllUser User)
        {
            List<BllStatus> checkedStatuses = new List<BllStatus>();
            List<BllStatus> uncheckedStatuses = new List<BllStatus>();
            List<BllStatus> libStatuses = new List<BllStatus>();
            foreach (var item in User.StatusLib.SelectedEntities)
            {
                libStatuses.Add(item.Entity);
            }

            for (int i = 0; i < Statuses.Count; i++)
            {
                if (checkedListBox2.GetItemChecked(i))
                {
                    checkedStatuses.Add(Statuses[i]);
                }
                else
                {
                    uncheckedStatuses.Add(Statuses[i]);
                }
            }
            foreach (var item in checkedStatuses)
            {
                if (BllEntityComparer.GetItemIndex(item, libStatuses.ToList<IBllEntity>()) < 0)
                {
                    User.StatusLib.SelectedEntities.Add(new BllSelectedStatus { Entity = item });
                }
            }
            foreach (var item in User.StatusLib.SelectedEntities)
            {
                if (BllEntityComparer.GetItemIndex(item, uncheckedStatuses.ToList<IBllEntity>()) >= 0)
                {
                    User.StatusLib.SelectedEntities.Remove(item);
                }
            }
        }

        private void UpdateEventTypeLib(BllUser User)
        {
            List<BllEventType> checkedTypes = new List<BllEventType>();
            List<BllEventType> uncheckedTypes = new List<BllEventType>();
            List<BllEventType> libTypes = new List<BllEventType>();
            foreach (var item in User.EventTypeLib.SelectedEntities)
            {
                libTypes.Add(item.Entity);
            }

            for (int i = 0; i < EventTypes.Count; i++)
            {
                if (checkedListBox2.GetItemChecked(i))
                {
                    checkedTypes.Add(EventTypes[i]);
                }
                else
                {
                    uncheckedTypes.Add(EventTypes[i]);
                }
            }
            foreach (var item in checkedTypes)
            {
                if (BllEntityComparer.GetItemIndex(item, libTypes.ToList<IBllEntity>()) < 0)
                {
                    User.EventTypeLib.SelectedEntities.Add(new BllSelectedEntity<BllEventType> { Entity = item });
                }
            }
            foreach (var item in User.EventTypeLib.SelectedEntities)
            {
                if (BllEntityComparer.GetItemIndex(item, uncheckedTypes.ToList<IBllEntity>()) >= 0)
                {
                    User.EventTypeLib.SelectedEntities.Remove(item);
                }
            }
        }

        
    }
}
