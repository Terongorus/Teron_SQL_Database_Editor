using System;
using System.Windows.Forms;
using AzureEditor;
using Database;
using Logic;
using static Logic.Globals;

namespace Startup
{
    public partial class Startup : Form
    {
        public Startup()
        {
            InitializeComponent();
            Text = AppInfo.DisplayNameWithVersion;
        }

        private void Startup_Load(object sender, EventArgs e)
        {

        }

        private void azure_open_Click(object sender, EventArgs e)
        {
            app_form = new AppForm();
            startup.Hide();
            app_form.Show();
        }

        private void warcraft_open_Click(object sender, EventArgs e)
        {
            dbc_editor = new DBCEditor();
            startup.Hide();
            dbc_editor.Show();
        }
    }
}
