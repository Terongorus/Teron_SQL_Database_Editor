using System.Windows;
using AzureEditor;
using Database;
using Logic;
using static Logic.Globals;

namespace Startup
{
    public partial class Startup : Window
    {
        public Startup()
        {
            InitializeComponent();
            Title = AppInfo.DisplayNameWithVersion;
        }

        private void azure_open_Click(object sender, RoutedEventArgs e)
        {
            app_form = new AppForm();
            startup.Hide();
            app_form.Show();
        }

        private void warcraft_open_Click(object sender, RoutedEventArgs e)
        {
            dbc_editor = new DBCEditor();
            startup.Hide();
            dbc_editor.Show();
        }
    }
}
