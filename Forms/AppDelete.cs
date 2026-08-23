using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using static Logic.Globals;

namespace AzureEditor
{
    public partial class AppDelete : Form
    {
        public AppDelete()
        {
            InitializeComponent();
            foreach (LoginTable login_cred in login_list)
            {
                if (!string.IsNullOrEmpty(login_cred.Nickname))
                {
                    connection_deletion_selection.Items.Add(login_cred.Nickname);
                }
            }
        }

        private void select_all_Click(object sender, EventArgs e)
        {
            if (connection_deletion_selection.Items.Count > 0)
            {
                foreach (LoginTable login_cred in login_list)
                {
                    if (!string.IsNullOrEmpty(login_cred.Nickname))
                    {
                        int index = connection_deletion_selection.Items.IndexOf(login_cred.Nickname);
                        if (index >= 0)
                            connection_deletion_selection.SetSelected(index, true);
                    }
                }
            }
            else 
            {
                MessageBox.Show("No connections in the list to be selected!");
            }
        }

        private void deselect_all_Click(object sender, EventArgs e)
        {
            if (connection_deletion_selection.Items.Count > 0)
            {
                foreach (LoginTable login_cred in login_list)
                {
                    if (!string.IsNullOrEmpty(login_cred.Nickname))
                    {
                        int index = connection_deletion_selection.Items.IndexOf(login_cred.Nickname);
                        if (index >= 0)
                            connection_deletion_selection.SetSelected(index, false);
                    }
                }
            }
            else
            {
                return;
            }
        }

        private void delete_selected_Click(object sender, EventArgs e)
        {
            int remove_counter = 0;

            if (!File.Exists(login_config))
            {
                MessageBox.Show("Connection configuration file not found!");
                return;
            }

            // Load the XML document
            XDocument doc = XDocument.Load(login_config);
            var root = doc.Root;

            // Work on a copy of the login list to avoid modification errors
            if (root != null)
            {
                foreach (LoginTable login_cred in login_list.ToList())
                {
                    // Ensure SelectedItems is valid and contains this nickname
                    if (!string.IsNullOrEmpty(login_cred.Nickname) && connection_deletion_selection.SelectedItems.Contains(login_cred.Nickname))
                    {
                        // Remove from ListBox
                        int index = connection_deletion_selection.Items.IndexOf(login_cred.Nickname);
                        if (index >= 0)
                            connection_deletion_selection.Items.RemoveAt(index);

                        // Remove from the global list
                        login_list.Remove(login_cred);

                        // Remove from the XML file
                        var nodeToRemove = root.Elements("Connection")
                            .FirstOrDefault(x => (string?)x.Element("Nickname") == login_cred.Nickname);

                        if (nodeToRemove != null)
                            nodeToRemove.Remove();

                        remove_counter++;
                    }
                }
            }

            // Save the modified XML file back to disk
            doc.Save(login_config);

            MessageBox.Show($"Removed {remove_counter} connection(s) successfully!");
        }

        private void delete_all_Click(object sender, EventArgs e)
        {
            if (login_list.Count == 0)
            {
                MessageBox.Show("No connections to delete!");
                return;
            }

            if (!File.Exists(login_config))
            {
                MessageBox.Show("Connection configuration file not found!");
                return;
            }

            // Count how many items we’re deleting
            int remove_counter = login_list.Count;

            // Clear the ListBox safely
            connection_deletion_selection.Items.Clear();

            // Clear the in-memory connection list
            login_list.Clear();

            // Update the XML file (remove all connections)
            try
            {
                XDocument doc = XDocument.Load(login_config);
                var root = doc.Root;
                if (root != null)
                {
                    root.Elements("Connection").Remove(); // removes all <Connection> nodes
                    doc.Save(login_config);
                }

                MessageBox.Show($"Removed {remove_counter} connection(s) from the list!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating configuration file:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void confirm_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
