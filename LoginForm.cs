using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ZaraCut
{
    public partial class LoginForm : Form
    {
        public string login = "";
        public LoginForm()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;
        }
        private void Login_Click(object sender, EventArgs e)
        {
            if (this.PasswordTB.Text == "qwe123")
            {
                this.login = this.LoginTB.Text;
                base.DialogResult = DialogResult.Yes;
                return;
            }
            MessageBox.Show("Неверный логин или пароль");
        }
    }
}
