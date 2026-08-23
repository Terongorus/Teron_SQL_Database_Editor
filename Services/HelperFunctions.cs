using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
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
                MessageBox.Show("Connection string is empty or null.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!connString.Contains("{technical_user_username}") || !connString.Contains("{technical_user_password}"))
            {
                MessageBox.Show("Connection string is missing required placeholders: {technical_user_username} or {technical_user_password}.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            self.data_viewer.ItemsSource = null;
            self.data_viewer.Items.Refresh();
            self.data_viewer.ItemsSource = table?.DefaultView;
            self.data_viewer.Items.Refresh();
        }

        // Returns the SqlTreeNodeTag attached to the connections_tree_view's currently selected
        // item, replacing WinForms TreeNode.Level/.Text/.Parent (see Globals.SqlTreeNodeTag).
        public static SqlTreeNodeTag? GetSelectedNodeTag(AppForm self)
        {
            return (self.connections_tree_view.SelectedItem as TreeViewItem)?.Tag as SqlTreeNodeTag;
        }

        public async Task SetQuery(AppForm self, TextBox current_query_textbox)
        {
            DataRequest request = new DataRequest();
            string? custom_query = null;
            var node = GetSelectedNodeTag(self);
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
            if (self.update_timer.IsEnabled == false)
            {
                self.update_timer.IsEnabled = true;
            }
            else
            {
                self.update_timer.Interval = TimeSpan.FromMilliseconds(1000);
                self.update_timer.Start();
            }
        }

        public async Task PopulateTreeViewFromDatabase(AppForm self)
        {
            self.connections_tree_view.Items.Clear();
            DataRequest request = new DataRequest();

            // Iterate through all database connections stored globally
            foreach (var conn in Globals.login_list.ToList())
            {
                var dbTag = new SqlTreeNodeTag { Text = conn.Nickname ?? string.Empty, Level = 0 };
                TreeViewItem dbNode = new TreeViewItem { Header = dbTag.Text, Tag = dbTag };

                // 1️. Fetch schemas using your existing helper

                await request.GetSchemasInfo(self, conn.Username, conn.Password, conn.ConnectionString);
                var local_schema_list = Globals.internal_schemas_list.ToList();
                // Loop through all schemas
                foreach (var schema in local_schema_list)
                {
                    var schemaTag = new SqlTreeNodeTag { Text = schema.Name ?? string.Empty, Level = 1, Parent = dbTag };
                    TreeViewItem schemaNode = new TreeViewItem { Header = schemaTag.Text, Tag = schemaTag };

                    // 2️. Fetch tables for this schema
                    await request.GetTablesInfo(self, conn.Username, conn.Password, conn.ConnectionString, schema.Name);
                    var local_tables_list = Globals.internal_tables_list.ToList();

                    foreach (var table in local_tables_list)
                    {
                        var tableTag = new SqlTreeNodeTag { Text = table.Name ?? string.Empty, Level = 2, Parent = schemaTag };
                        TreeViewItem tableNode = new TreeViewItem { Header = tableTag.Text, Tag = tableTag };

                        // 3️. Fetch columns for this table
                        await request.GetColumnsInfo(self, conn.Username, conn.Password, conn.ConnectionString, schema.Name, table.Name);
                        var local_colmuns_list = Globals.internal_columns_list.ToList();

                        foreach (var column in local_colmuns_list)
                        {
                            var columnTag = new SqlTreeNodeTag { Text = column.Name ?? string.Empty, Level = 3, Parent = tableTag };
                            TreeViewItem columnNode = new TreeViewItem { Header = columnTag.Text, Tag = columnTag };
                            tableNode.Items.Add(columnNode);
                        }

                        schemaNode.Items.Add(tableNode);
                    }

                    dbNode.Items.Add(schemaNode);
                }

                self.connections_tree_view.Items.Add(dbNode);
            }

            // Optional: expand tree
            //foreach (TreeViewItem item in self.connections_tree_view.Items) item.ExpandSubtree();
        }

        public void ExportQuery(AppForm self, TextBox richTextBox)
        {
            try
            {
                var save_query_dialog = new Microsoft.Win32.SaveFileDialog();
                save_query_dialog.Title = "Save SQL Query";
                save_query_dialog.FileName = ((self.query_tab_control.SelectedItem as TabItem)?.Tag?.ToString()?.Replace(" ", "_") ?? "query").ToLowerInvariant();
                save_query_dialog.Filter = "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*";
                save_query_dialog.DefaultExt = "sql";
                save_query_dialog.AddExtension = true;
                save_query_dialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

                if (save_query_dialog.ShowDialog() == true)
                {
                    File.WriteAllText(save_query_dialog.FileName, richTextBox.Text.ToString(), Encoding.UTF8);
                    MessageBox.Show("SQL file exported successfully!", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Export process terminated!", "Export Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting file:\n{ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ImportQuery(TextBox richTextBox)
        {
            try
            {
                // Create an OpenFileDialog to allow user to select a .sql file
                var openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Title = "Import SQL Query";
                openFileDialog.Filter = "SQL Files (*.sql)|*.sql|All Files (*.*)|*.*";
                openFileDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

                // If user selects a file and clicks OK
                if (openFileDialog.ShowDialog() == true)
                {
                    // Read file contents
                    string sqlContent = File.ReadAllText(openFileDialog.FileName);

                    // Load content into the TextBox
                    richTextBox.Text = sqlContent;

                    MessageBox.Show("SQL file imported successfully!", "Import Complete",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing file:\n{ex.Message}", "Import Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public TextBox? GetActiveQueryTextBox(AppForm self)
        {
            if (self == null || self.query_tab_control == null || Globals.box_list == null || self.query_tab_control.SelectedIndex < 0 || self.query_tab_control.SelectedIndex >= Globals.box_list.Count)
            {
                return null;
            }

            return Globals.box_list[self.query_tab_control.SelectedIndex];
        }

        // Reads/writes the plain title string stored in a query TabItem's Tag, and reflects it
        // in the visible TextBlock inside the tab's Header (a StackPanel with a title + close
        // button, built in AddNewTab) - WPF TabItem.Header isn't a plain string here the way
        // WinForms TabPage.Text was, since it also hosts the close button.
        private static string GetTabTitle(TabItem tab) => tab.Tag as string ?? string.Empty;

        private static void SetTabTitle(TabItem tab, string title)
        {
            tab.Tag = title;
            if (tab.Header is StackPanel panel)
            {
                var textBlock = panel.Children.OfType<TextBlock>().FirstOrDefault();
                if (textBlock != null)
                    textBlock.Text = title;
            }
            // WPF's default TabItem automation name falls back to Header.ToString() when Header
            // isn't a plain string (it's a StackPanel here, for the close button) - set the real
            // name explicitly so screen readers/UI Automation see "Query 1", not the object dump.
            AutomationProperties.SetName(tab, title);
        }

        public string NameQueryTabs(AppForm self)
        {
            int i;
            for (i = 0; i < self.query_tab_control.Items.Count; i++)
            {
                var tab = (TabItem)self.query_tab_control.Items[i]; // access tab directly

                if (!GetTabTitle(tab).Contains("Query"))
                {
                    SetTabTitle(tab, $"Query {i + 1}");
                }
            }

            return $"Query {i + 1}";
        }

        public TabItem AddNewTab(AppForm self, TabControl control, string title, UIElement? content = null)
        {
            int cnt_1 = Globals.box_list.Count;
            TextBox query_textbox = new TextBox
            {
                Name = $"query_textbox_{cnt_1}",
                AcceptsReturn = true,
                AcceptsTab = true,
                TextWrapping = TextWrapping.NoWrap,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                FontFamily = new System.Windows.Media.FontFamily("Consolas")
            };
            query_textbox.TextChanged += self.query_textbox_TextChanged;
            Globals.box_list.Add(query_textbox);

            TabItem new_tab = new TabItem { Tag = title };

            var titleBlock = new TextBlock { Text = title, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 6, 0) };
            var closeButton = new Button { Content = "x", Width = 16, Height = 16, Padding = new Thickness(0), FontWeight = System.Windows.FontWeights.Bold };
            closeButton.Click += (s, e) =>
            {
                int idx = control.Items.IndexOf(new_tab);
                if (idx < 0) return;
                if (idx < Globals.box_list.Count)
                    Globals.box_list.RemoveAt(idx);
                control.Items.RemoveAt(idx);
            };
            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal };
            headerPanel.Children.Add(titleBlock);
            headerPanel.Children.Add(closeButton);
            new_tab.Header = headerPanel;
            AutomationProperties.SetName(new_tab, title);

            new_tab.Content = query_textbox;

            control.Items.Add(new_tab);
            control.SelectedItem = new_tab;

            return new_tab;
        }
    }
}
