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

namespace AdminClient.Forms.Directories
{
    public partial class EntityDataForm : ParentForm
    {
        protected DirectoryForm parent;
        public IBllEntity Entity { get; protected set; }

        public EntityDataForm() : base()
        {
            InitializeComponent();
        }

        public EntityDataForm(DirectoryForm parent, IBusinessService server, IBllEntity Entity) : base()
        {
            InitializeComponent();
            CenterToParent();
            this.parent = parent;
            this.server = server;
            this.Entity = Entity;
        }

        protected virtual void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
