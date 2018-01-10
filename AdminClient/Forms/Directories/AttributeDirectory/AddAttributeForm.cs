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
    public partial class AddAttributeForm : AttributeDataForm
    {
        public AddAttributeForm()
        {
            InitializeComponent();
        }

        public AddAttributeForm(DirectoryForm parent, IBusinessService server, BllAttribute Attribute) : base(parent, server, Attribute)
        {
            InitializeComponent();
        }

        protected override void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Введите название", "Оповещение");
            }
            else
            {
                Entity = new BllAttribute
                {
                    Name = textBox1.Text,
                };
                Entity = server.CreateAttribute((BllAttribute)Entity);
                base.button1_Click(sender, e);
            }
        }
    }
}
