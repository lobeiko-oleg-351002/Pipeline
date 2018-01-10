using ServerInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AdminClient
{
    public partial class ParentForm : Form
    {
        protected IBusinessService server;
        public ParentForm()
        {
            InitializeComponent();
        }
    }
}
