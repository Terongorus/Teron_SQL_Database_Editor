using System;
using System.Windows.Forms;
using Logic;

namespace AzureEditor
{
    public partial class AppLogin : Form
    {
        public AppLogin()
        {
            InitializeComponent();
        }

        private void AppLogin_Load(object sender, EventArgs e)
        {
            username_input.Text = string.Empty;
            password_input.Text = string.Empty;
            conn_string_input.Text = string.Empty;
        }

        private void connect_button_Click(object sender, EventArgs e)
        {
            AppForm new_AppForm = new AppForm();
            App.AuthenticateUser(new_AppForm, this);
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
