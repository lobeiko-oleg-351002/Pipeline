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

namespace AdminClient.Forms.Directories.AttributeDirectory
{
    public partial class AmendAttributeForm : AttributeDataForm
    {
        public AmendAttributeForm()
        {
            InitializeComponent();
        }

        public AmendAttributeForm(DirectoryForm parent, IBusinessService server, BllAttribute entity) : base(parent, server, entity)
        {
            InitializeComponent();
            textBox1.Text = entity.Name;

        }

        protected override void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Введите название", "Оповещение");
            }
            else
            {
                BllAttribute Attribute = (BllAttribute)Entity;
                Attribute.Name = textBox1.Text;
                Entity = server.UpdateAttribute(Attribute);
                base.button1_Click(sender, e);
            }
        }
    }
}
