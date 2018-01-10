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
    public partial class DirectoryForm : ParentForm
    {
        public DirectoryForm() : base()
        {
            InitializeComponent();
        }

        public DirectoryForm(IBusinessService server) : base()
        {
            InitializeComponent();
            this.server = server;
            CenterToScreen();
        }

        protected virtual void button1_Click(object sender, EventArgs e)
        {

        }

        protected virtual void button2_Click(object sender, EventArgs e)
        {

        }

        protected virtual void button3_Click(object sender, EventArgs e)
        {

        }

        protected virtual void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        public virtual void RefreshData()
        {
            throw new NotImplementedException();
        }
        
        protected virtual void AddNewRow(IBllEntity entity)
        {
            throw new NotImplementedException();
        }

        protected virtual void UpdateRow(IBllEntity entity, int row) { throw new NotImplementedException(); }

        protected virtual DataGridViewRow FillRow(IBllEntity entity, DataGridViewRow row) { throw new NotImplementedException(); }
    }
}
