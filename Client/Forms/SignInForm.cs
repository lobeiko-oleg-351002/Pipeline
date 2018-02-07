using BllEntities;
using ServerInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class SignInForm : ParentForm
    {
        public SignInForm()
        {
            InitializeComponent();
        }

        public BllUser User;

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
            throw new UserIsNullException();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((textBox1.Text == "") || (textBox2.Text == ""))
            {
                MessageBox.Show("Введите логин и пароль");
            }
            else
            {
                Close();
            }
        }
    }
}
