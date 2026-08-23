using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AzureEditor;

namespace Logic
{
    internal class HelperFunctions
    {
        public void LoginMessage(AppForm self)
        {
            //Initial Console Info
            self.messages_log_textbox.Text = string.Empty;
            self.messages_log_textbox.AppendText("--------------------------------------------------");
            self.messages_log_textbox.AppendText("\nTeron SQL Database Editor (TSDE)");
            self.messages_log_textbox.AppendText("\nby Terongorus");
            self.messages_log_textbox.AppendText("\nv0.1.5-beta");
            self.messages_log_textbox.AppendText("\nChangelog:");
            self.messages_log_textbox.AppendText("\n - Made major changes to the UI and all the functionalities.");
            self.messages_log_textbox.AppendText("\n--------------------------------------------------");
        }

        public bool ValidateConnectionString(string connString)
        {
            if (string.IsNullOrEmpty(connString))
            {
                MessageBox.Show("Connection string is empty or null.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!connString.Contains("{technical_user_username}") || !connString.Contains("{technical_user_password}"))
            {
                MessageBox.Show("Connection string is missing required placeholders: {technical_user_username} or {technical_user_password}.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public DataTable ToDataTable(BindingList<Dictionary<string, object>> list)
        {
            DataTable table = new DataTable();

            if (list == null || list.Count == 0)
                return table;

            // Create columns from the keys of the first dictionary
            foreach (var key in list[0].Keys)
            {
                table.Columns.Add(key);
            }

            // Fill rows
            foreach (var dict in list)
            {
                var row = table.NewRow();
                foreach (var kvp in dict)
                {
                    row[kvp.Key] = kvp.Value ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }

            return table;
        }

        public void SetTableDataSource(AppForm self, DataTable? table)
        {
            self.data_viewer.DataSource = null;
            self.data_viewer.Refresh();
            self.data_viewer.DataSource = table;
            self.data_viewer.Refresh();
        }

        public async Task SetQuery(AppForm self, RichTextBox current_query_textbox)
        {
            DataRequest request = new DataRequest();
            string? custom_query = null;
            var node = self.connections_tree_view.SelectedNode;
            if (node == null)
                return;

            // Reset default values
            Globals.select_clause = "SELECT *";
            Globals.from_clause = "";
            Globals.custom_sql_query = "";

            string? connection = null;
            string? schema = null;
            string? table = null;
            string? column = null;

            // Determine node type and extract info from hierarchy
            switch (node.Level)
            {
                case 0: // Connection
                    connection = node.Text;
                    break;

                case 1: // Schema
                    schema = node.Text;
                    break;

                case 2: // Table
                    table = node.Text;
                    schema = node.Parent?.Text;
                    break;

                case 3: // Column
                    column = node.Text;
                    table = node.Parent?.Text;
                    schema = node.Parent?.Parent?.Text;
                    break;
            }

            // Build SQL query based on what was selected
            if (!string.IsNullOrEmpty(column) && !string.IsNullOrEmpty(table) && !string.IsNullOrEmpty(schema))
            {
                // Load columns if needed (optional)
                await request.GetColumnsInfo(
                    self,
                    Globals.sql_technical_user_username,
                    Globals.sql_technical_user_password,
                    Globals.connectionString,
                    schema,
                    table
                );

                Globals.schema = schema;
                Globals.table = table;
                Globals.column = column;

                Globals.select_clause = $"SELECT [{column}]";
                Globals.from_clause = $"FROM [{schema}].[{table}]";
            }
            else if (!string.IsNullOrEmpty(table) && !string.IsNullOrEmpty(schema))
            {

                Globals.schema = schema;
                Globals.table = table;

                Globals.select_clause = "SELECT *";
                Globals.from_clause = $"FROM [{schema}].[{table}]";
            }
            else if (!string.IsNullOrEmpty(schema))
            {
                Globals.schema = schema;
                //Globals.select_clause = "SELECT *";
                //Globals.from_clause = $"FROM [{schema}]";
                return;
            }

            // Only build query if FROM clause is valid
            if (!string.IsNullOrEmpty(Globals.from_clause))
            {
                custom_query = $"{Globals.select_clause} {Globals.from_clause};";
                current_query_textbox.Text = custom_query;
            }
            else
            {
                return;
            }
        }

        public void SetUpdateTimer(AppForm self)
        {
            if (self.update_timer.Enabled == false)
            {
                self.update_timer.Enabled = true;
            }
            else
            {
                self.update_timer.Interval = 1000;
                self.update_timer.Start();
            }
        }

        public async Task PopulateTreeViewFromDatabase(AppForm self)
        {
            self.connections_tree_view.Nodes.Clear();
            DataRequest request = new DataRequest();

            // Iterate through all database connections stored globally
            foreach (var conn in Globals.login_list.ToList())
            {
                TreeNode dbNode = new TreeNode(conn.Nickname);

                // 1️. Fetch schemas using your existing helper

                await request.GetSchemasInfo(self, conn.Username, conn.Password, conn.ConnectionString);
                var local_schema_list = Globals.internal_schemas_list.ToList();
                // Loop through all schemas
                foreach (var schema in local_schema_list)
                {
                    TreeNode schemaNode = new TreeNode(schema.Name);

                    // 2️. Fetch tables for this schema
                    await request.GetTablesInfo(self, conn.Username, conn.Password, conn.ConnectionString, schema.Name);
                    var local_tables_list = Globals.internal_tables_list.ToList();

                    foreach (var table in local_tables_list)
                    {
                        TreeNode tableNode = new TreeNode(text: table.Name);

                        // 3️. Fetch columns for this table
                        await request.GetColumnsInfo(self, conn.Username, conn.Password, conn.ConnectionString, schema.Name, table.Name);
                        var local_colmuns_list = Globals.internal_columns_list.ToList();

                        foreach (var column in local_colmuns_list)
                        {
                            TreeNode columnNode = new TreeNode(column.Name);
                            tableNode.Nodes.Add(columnNode);
                        }

                        schemaNode.Nodes.Add(tableNode);
                    }

                    dbNode.Nodes.Add(schemaNode);
                }

                self.connections_tree_view.Nodes.Add(dbNode);
            }

            // Optional: expand tree
            //self.connections_tree_view.ExpandAll();
        }

        public void ExportQuery(AppForm self,RichTextBox richTextBox)
        {
            try
            {
                using (SaveFileDialog save_query_dialog = new SaveFileDialog())
                {
                    save_query_dialog.Title = "Save SQL Query";
                    save_query_dialog.FileName = (self.query_tab_control.SelectedTab?.Text.Replace(" ", "_") ?? "query").ToLowerInvariant();
                    save_query_dialog.Filter = "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*";
                    save_query_dialog.DefaultExt = "sql";
                    save_query_dialog.AddExtension = true;
                    save_query_dialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

                    if (save_query_dialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(save_query_dialog.FileName, richTextBox.Text.ToString(), Encoding.UTF8);
                        MessageBox.Show("SQL file exported successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Export process terminated!", "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting file:\n{ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ImportQuery(RichTextBox richTextBox)
        {
            try
            {
                // Create an OpenFileDialog to allow user to select a .sql file
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Title = "Import SQL Query";
                    openFileDialog.Filter = "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*";
                    openFileDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

                    // If user selects a file and clicks OK
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Read file contents
                        string sqlContent = File.ReadAllText(openFileDialog.FileName);

                        // Load content into the RichTextBox
                        richTextBox.Text = sqlContent;

                        MessageBox.Show("SQL file imported successfully!", "Import Complete",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing file:\n{ex.Message}", "Import Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void MouseButtonDetect(object sender, MouseEventArgs e, AppForm self)
        {
            if (sender == self.data_viewer)
            {
                // Check which button was pressed
                if (e.Button == MouseButtons.Left)
                {
                    return;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (sender is Control source_control)
                    {
                        self.query_result_context_menu.Show(source_control, e.Location);
                    }
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    return;
                }
                else
                {
                    return;
                }
            }
            else if (sender == self.connections_tree_view)
            {
                // Check which button was pressed
                if (e.Button == MouseButtons.Left)
                {
                    return;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (sender is Control source_control)
                    {
                        self.tree_view_context_menu.Show(source_control, e.Location);
                    }
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    return;
                }
                else
                {
                    return;
                }
            }
            else if (sender == self.query_tab_control.TabPages)
            {
                // Check which button was pressed
                if (e.Button == MouseButtons.Left)
                {
                    return;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    //rename tabs
                    if (sender is Control source_control)
                    {
                        self.query_tab_control_context_menu.Show(source_control, e.Location);
                    }
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    return;
                }
                else
                {
                    return;
                }
            }
        }

        public RichTextBox? GetActiveQueryTextBox(AppForm self)
        {
            if (self == null || self.query_tab_control == null || Globals.box_list == null || self.query_tab_control.SelectedIndex < 0 || self.query_tab_control.SelectedIndex >= Globals.box_list.Count)
            {
                return null;
            }

            return Globals.box_list[self.query_tab_control.SelectedIndex];
        }

        public string NameQueryTabs(AppForm self)
        {
            int i;
            for (i = 0; i < self.query_tab_control.TabCount; i++)
            {
                var tab = self.query_tab_control.TabPages[i]; // access tab directly

                if (!tab.Text.Contains("Query"))
                {
                    tab.Text = $"Query {i + 1}";
                }
            }

            return $"Query {i + 1}";
        }

        public TabPage AddNewTab(AppForm self, TabControl control, string title, Control? content = null)
        {
            int cnt_1 = Globals.box_list.Count;
            RichTextBox query_textbox = new RichTextBox();
            query_textbox.Name = $"query_textbox_{cnt_1}";
            query_textbox.TextChanged += self.query_textbox_TextChanged;
            Globals.box_list.Add(query_textbox);

            content = query_textbox;

            TabPage new_tab = new TabPage(title);
            if (content != null)
            {
                content.Dock = DockStyle.Fill;
                new_tab.Controls.Add(content);
            }
            control.TabPages.Add(new_tab);
            control.SelectedTab = new_tab;

            return new_tab;
        }
    }
}
