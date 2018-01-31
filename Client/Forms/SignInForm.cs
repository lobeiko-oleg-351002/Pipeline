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

        IBusinessService server;
        public BllUser User;

        public SignInForm(IBusinessService server)
        {
            InitializeComponent();
            this.server = server;
            User = new BllUser { Login = "" };
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
            throw new UserIsNullException();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                User = server.SignIn(textBox1.Text, Sha1.Encrypt(textBox2.Text));
                if (User == null)
                {
                    MessageBox.Show(Properties.Resources.ResourceManager.GetString("USER_NOT_FOUND"));
                }
                else
                {
                    Close();
                }
            }
            catch(Exception)
            {
                MessageBox.Show(Properties.Resources.SERVER_NOT_FOUND);
                Close();
                throw new UserIsNullException();
            }

        }


    }
}
