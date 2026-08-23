using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using AzureEditor;

namespace Logic
{
    internal class App
    {
        // DPAPI-protects the password before it's written to disk. CurrentUser scope means
        // only the Windows account that saved it can read it back.
        private static string ProtectPassword(string plainText)
        {
            byte[] encrypted = ProtectedData.Protect(Encoding.UTF8.GetBytes(plainText), null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        private static string UnprotectPassword(string storedValue)
        {
            try
            {
                byte[] decrypted = ProtectedData.Unprotect(Convert.FromBase64String(storedValue), null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (Exception ex) when (ex is FormatException or CryptographicException)
            {
                // Not a value this app protected (e.g. a plaintext loginconfig.xml carried over
                // from an older version) - treat it as already-plaintext rather than failing.
                return storedValue;
            }
        }

        public static void AuthenticateUser(AppForm self, AppLogin login)
        {
            Globals.login_list.Clear();

            string temp_name = string.Empty;
            string temp_user = string.Empty;
            string temp_pass = string.Empty;
            string temp_conn_string = string.Empty;

            self.messages_log_textbox.Text = string.Empty;
            self.messages_log_textbox.AppendText("Login to DB with Technical User:");
            self.messages_log_textbox.AppendText("\n--------------------------------------------------");
            temp_name = login.nickname_textbox.Text.ToString();
            if (string.IsNullOrEmpty(temp_name))
            {
                MessageBox.Show("Please name your connection in the 'Name' field!");
                return;
            }
            //username authentication
            temp_user = login.username_input.Text.ToString();
            if (string.IsNullOrEmpty(temp_user))
            {
                MessageBox.Show("Enter a correct AzureSQL username!");
                return;
            }
            //password authentication
            temp_pass = login.password_input.Text.ToString();
            if (string.IsNullOrEmpty(temp_pass))
            {
                MessageBox.Show("Enter a correct AzureSQL password!");
                return;
            }
            //connection string authentication
            temp_conn_string = login.conn_string_input.Text.ToString();
            if (string.IsNullOrEmpty(temp_conn_string))
            {
                MessageBox.Show("Enter a correct AzureSQL connection string!");
                return;
            }

            login.Hide();

            Globals.LoginTable new_login = new Globals.LoginTable
            {
                Nickname = temp_name,
                Username = temp_user,
                Password = temp_pass,
                ConnectionString = temp_conn_string,
            };
            Globals.login_list.Add(new_login);

            Directory.CreateDirectory(Globals.AppDataRoot);

            // Ensure document root is present before dereferencing
            XDocument doc = File.Exists(Globals.login_config)
                ? XDocument.Load(Globals.login_config)
                : new XDocument(new XElement("Connections"));
            XElement? root = doc.Root;
            if (root == null)
            {
                root = new XElement("Connections");
                doc.Add(root);
            }

            int new_ID = root.Elements("Connection").Any()
                ? root.Elements("Connection").Max(c => int.Parse(c.Attribute("ID")?.Value ?? "0")) + 1
                : 1;

            foreach (Globals.LoginTable login_entry in Globals.login_list)
            {
                XElement new_connection = new XElement("Connection",
                    new XAttribute("ID", new_ID),
                    new XElement("Nickname", login_entry.Nickname),
                    new XElement("Username", login_entry.Username),
                    new XElement("Password", ProtectPassword(login_entry.Password)),
                    new XElement("ConnString", login_entry.ConnectionString)
                );
                root.Add(new_connection);
                doc.Save(Globals.login_config);
            }

            self.messages_log_textbox.Text = string.Empty;
            self.messages_log_textbox.AppendText("\nSuccessfully added new connection!");

            LoadConnectionDetails(self);
            //DataRequest.GetSchemasInfo(self);
        }

        public static void LoadConnectionDetails(AppForm self)
        {
            if (File.Exists(Globals.login_config))
            {
                XDocument doc = XDocument.Load(Globals.login_config);
                var connections = doc.Root?.Elements("Connection").ToList() ?? new List<XElement>();

                if (!connections.Any())
                {
                    self.update_timer.Enabled = false;
                    MessageBox.Show("No connections found in configuration file!");
                    return;
                }

                Globals.login_list.Clear();
                foreach (var element in connections)
                {
                    Globals.LoginTable load_login = new Globals.LoginTable
                    {
                        Nickname = element.Element("Nickname")?.Value,
                        Username = element.Element("Username")?.Value ?? "",
                        Password = UnprotectPassword(element.Element("Password")?.Value ?? ""),
                        ConnectionString = element.Element("ConnString")?.Value ?? ""
                    };
                    Globals.login_list.Add(load_login);
                }
                self.update_timer.Enabled = true;
            }
            else
            {
                self.update_timer.Enabled = false;
                return;
            }
        }

        public static void DeleteConnection(AppForm self)
        {

        }

        public static void ExportQuery(AppForm self)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

        }
    }
}
