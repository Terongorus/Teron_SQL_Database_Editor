using System.Windows;
using Logic;

namespace AzureEditor
{
    public partial class AppLogin : Window
    {
        public AppLogin()
        {
            InitializeComponent();
            username_input.Text = string.Empty;
            password_input.Password = string.Empty;
            conn_string_input.Text = string.Empty;
        }

        private void connect_button_Click(object sender, RoutedEventArgs e)
        {
            AppForm new_AppForm = new AppForm();
            App.AuthenticateUser(new_AppForm, this);
        }

        private void cancel_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
