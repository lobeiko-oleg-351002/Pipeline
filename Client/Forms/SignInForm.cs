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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                User = server.SignIn(textBox1.Text, SHA1(textBox2.Text));
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            if (User == null)
            {
                MessageBox.Show(Properties.Resources.ResourceManager.GetString("USER_NOT_FOUND"));
            }
            else
            {
                Close();
            }
        }

        private static string SHA1(string input)
        {
            byte[] hash;
            using (var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider())
            {
                hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));

            }
            var sb = new StringBuilder();
            foreach (byte b in hash) sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }
    }
}
