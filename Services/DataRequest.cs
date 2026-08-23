using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using AzureEditor;

namespace Logic
{
    public class DataRequest
    {
        public async Task GetSchemasInfo(AppForm self, string User, string Pass, string ConnString)
        {
            HelperFunctions helper = new HelperFunctions();

            if (!helper.ValidateConnectionString(ConnString))
            {
                self.messages_log_textbox.AppendText("\n[Error] Invalid connection string.");
                return;
            }

            ConnString = ConnString.Replace("{technical_user_username}", User);
            ConnString = ConnString.Replace("{technical_user_password}", Pass);

            string schema_query = Globals.schemas_query;
            self.fetch_status.Text = string.Empty;
            self.messages_log_textbox.Clear();
            if ((!string.IsNullOrEmpty(User) || !string.IsNullOrEmpty(Pass)) && !string.IsNullOrEmpty(ConnString))
            {
                self.messages_log_textbox.AppendText("--------------------------------------------------");
                self.messages_log_textbox.AppendText("\nFetching schemas from DB...");
                self.messages_log_textbox.AppendText($"\nQuery: {schema_query}");
                self.messages_log_textbox.AppendText("\n--------------------------------------------------");
                int schemas_count = 0;
                Globals.internal_schemas_list.Clear();
                DateTime startTime = DateTime.Now;
                try
                {
                    using (SqlConnection schemas_db_connection = new SqlConnection(ConnString))
                    {
                        self.connection_process.Visibility = Visibility.Visible;
                        self.connection_process.Value = 0;
                        self.connection_process.Maximum = 100;

                        var cts = new CancellationTokenSource();
                        var token = cts.Token;

                        // Start fake progress animation
                        var progressTask = Task.Run(async () =>
                        {
                            while (!token.IsCancellationRequested)
                            {
                                await Task.Delay(100);
                                self.Dispatcher.Invoke(() =>
                                {
                                    if (self.connection_process.Value < self.connection_process.Maximum - 1)
                                    {
                                        self.connection_process.Value++;
                                    }
                                });
                            }
                        }, token);

                        try
                        {
                            await Task.Run(() => schemas_db_connection.Open(), token); // open asynchronously

                            cts.Cancel();

                            // Set to 100 safely
                            self.Dispatcher.Invoke(() =>
                            {
                                self.connection_process.Value = self.connection_process.Maximum;
                            });
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Message.Contains("Timeout expired") || ex.Message.Contains("Connection Timeout Expired"))
                            {
                                MessageBox.Show(
                                    "Connection timed out while trying to reach the database.\nPlease check your connection settings or server availability.",
                                    "Connection Timeout",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning
                                );
                            }
                            else if (ex.Message.Contains("Can not connect") || ex.Message.Contains("in its current state"))
                            {
                                MessageBox.Show(
                                    "Access to database is restricted!\nPlease resolve this issue and then initiate a connection with the database.",
                                    "Database Locked or Restricted",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error
                                );
                            }
                            else
                            {
                                MessageBox.Show(
                                    $"Database connection failed:\n{ex.Message}",
                                    "SQL Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error
                                );
                            }

                            self.messages_log_textbox.AppendText($"\n[Error] {ex.Message}");
                            return;
                        }
                        catch (Exception ex)
                        {
                            cts.Cancel();
                            self.Dispatcher.Invoke(() =>
                            {
                                self.connection_process.Value = 0;
                                self.messages_log_textbox.Clear();
                                self.messages_log_textbox.AppendText($"Connection failed: {ex.Message}");
                            });
                            return;
                        }
                        finally
                        {
                            cts.Cancel(); // make sure animation stops
                            self.Dispatcher.Invoke(() => self.connection_process.Visibility = Visibility.Collapsed);
                        }
                        using (SqlCommand schemas_command = new SqlCommand(schema_query, schemas_db_connection))
                        {
                            using (SqlDataReader schemas_reader = schemas_command.ExecuteReader())
                            {
                                self.messages_log_textbox.AppendText("\nSchemas found:");
                                while (schemas_reader.Read())
                                {
                                    self.messages_log_textbox.AppendText("\n" + (schemas_count + 1).ToString() + ". " + schemas_reader["SCHEMA_NAME"]);
                                    Globals.SchemasViewStructure new_schema_entry = new Globals.SchemasViewStructure
                                    {
                                        Name = schemas_reader["SCHEMA_NAME"].ToString()
                                    };
                                    Globals.internal_schemas_list.Add(new_schema_entry);
                                    schemas_count++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    self.messages_log_textbox.Clear();
                    self.messages_log_textbox.AppendText($"\nUnhandled Exception: {ex.Message}");
                    self.messages_log_textbox.AppendText($"\nStack Trace: {ex.StackTrace}");
                    return;
                }
                self.fetch_status.Text = $"Fetched entries: {schemas_count} | Time elapsed: {DateTime.Now - startTime}";
                self.messages_log_textbox.AppendText("\n--------------------------------------------------");
                self.messages_log_textbox.AppendText($"\nTotal schemas fetched: {schemas_count}");
                self.messages_log_textbox.AppendText($"\nTime elapsed: {DateTime.Now - startTime}");
                self.messages_log_textbox.AppendText("\n--------------------------------------------------");

                //helper.SetSchemasSelector(self);
            }
            else
            {
                MessageBox.Show("Username or Password is incorrect or empty! Please provide valid credentials.");
                return;
            }
        }

        public async Task GetTablesInfo(AppForm self, string User, string Pass, string ConnString, string? schema)
        {
            HelperFunctions helper = new HelperFunctions();

            if (!helper.ValidateConnectionString(ConnString))
            {
                self.messages_log_textbox.AppendText("\n[Error] Invalid connection string.");
                return;
            }

            // Guard against null/empty schema to satisfy nullable analysis and avoid runtime errors
            if (string.IsNullOrEmpty(schema))
            {
                self.messages_log_textbox.AppendText("\n[Error] Schema is null or empty.");
                return;
            }

            ConnString = ConnString.Replace("{technical_user_username}", User);
            ConnString = ConnString.Replace("{technical_user_password}", Pass);

            string table_query = Globals.tables_query;
            self.fetch_status.Text = string.Empty;
            self.messages_log_textbox.Clear();
            if ((!string.IsNullOrEmpty(User) || !string.IsNullOrEmpty(Pass)) && !string.IsNullOrEmpty(ConnString))
            {
                table_query = table_query.Replace("%schema%", schema);
                self.messages_log_textbox.AppendText("--------------------------------------------------");
                self.messages_log_textbox.AppendText($"\nFetching tables from DB for {schema} schema...");
                self.messages_log_textbox.AppendText($"\nQuery: {table_query}");
                self.messages_log_textbox.AppendText("\n--------------------------------------------------");
                int tables_count = 0;
                Globals.internal_tables_list.Clear();
                DateTime startTime = DateTime.Now;

                try
                {
                    using (SqlConnection tables_db_connection = new SqlConnection(ConnString))
                    {
                        self.connection_process.Visibility = Visibility.Visible;
                        self.connection_process.Value = 0;
                        self.connection_process.Maximum = 100;

                        var progressTask = Task.Run(async () =>
                        {
                            while (self.connection_process.Value < 90)
                            {
                                await Task.Delay(100);
                                self.Dispatcher.Invoke(() =>
                                {
                                    if (self.connection_process.Value < self.connection_process.Maximum)
                                        self.connection_process.Value++;
                                });
                            }
                        });

                        try
                        {
                            await Task.Run(() => tables_db_connection.Open());
                            self.Dispatcher.Invoke(() =>
                            {
                                self.connection_process.Value = Math.Min(self.connection_process.Maximum, 100);
                            });
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Message.Contains("Timeout expired") || ex.Message.Contains("Connection Timeout Expired"))
                            {
                                MessageBox.Show(
                                    "Connection timed out while trying to reach the database.\nPlease check your connection settings or server availability.",
                                    "Connection Timeout",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning
                                );
                            }
                            else if (ex.Message.Contains("Can not connect") || ex.Message.Contains("in its current state"))
                            {
                                MessageBox.Show(
                                    "Access to database is restricted!\nPlease resolve this issue and then initiate a connection with the database.",
                                    "Database Locked or Restricted",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error
                                );
                            }
                            else
                            {
                                MessageBox.Show(
                                    $"Database connection failed:\n{ex.Message}",
                                    "SQL Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error
                                );
                            }

                            self.messages_log_textbox.AppendText($"\n[Error] {ex.Message}");
                            return;
                        }
                        catch (Exception ex)
                        {
                            self.connection_process.Value = 0;
                            self.messages_log_textbox.Clear();
                            self.messages_log_textbox.AppendText($"Connection failed: {ex.Message}");
                        }
                        finally
                        {
                            self.connection_process.Visibility = Visibility.Collapsed;
                        }
                        using (SqlCommand tables_command = new SqlCommand(table_query, tables_db_connection))
                        {
                            using (SqlDataReader table_reader = tables_command.ExecuteReader())
                            {
                                self.messages_log_textbox.AppendText("\nTables found:");
                                while (table_reader.Read())
                                {
                                    self.messages_log_textbox.AppendText("\n" + (tables_count + 1).ToString() + ". " + table_reader["TABLE_NAME"]);
                                    Globals.TablesViewStructure new_table_entry = new Globals.TablesViewStructure
                                    {
                                        Name = table_reader["TABLE_NAME"].ToString()
                                    };
                                    Globals.internal_tables_list.Add(new_table_entry);
                                    tables_count++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    self.messages_log_textbox.Clear();
                    self.messages_log_textbox.AppendText($"\nUnhandled Exception: {ex.Message}");
                    self.messages_log_textbox.AppendText($"\nStack Trace: {ex.StackTrace}");
                    return;
                }
                self.fetch_status.Text = $"Fetched entries: {tables_count} | Time elapsed: {DateTime.Now - startTime}";
                self.messages_log_textbox.AppendText("\n--------------------------------------------------");
                self.messages_log_textbox.AppendText($"\nTotal tables fetched: {tables_count}");
                self.messages_log_textbox.AppendText($"\nTime elapsed: {DateTime.Now - startTime}");
                self.messages_log_textbox.AppendText("\n--------------------------------------------------");

                //helper.SetTablesSelector(self);
            }
            else
            {
                MessageBox.Show("Username or Password is incorrect or empty! Please provide valid credentials.");
                return;
            }
        }

        public async Task GetColumnsInfo(AppForm self, string User, string Pass, string ConnString, string? schema, string? table)
        {
            HelperFunctions helper = new HelperFunctions();

            if (!helper.ValidateConnectionString(ConnString))
            {
                self.messages_log_textbox.AppendText("\n[Error] Invalid connection string.");
                return;
            }

            // Guard against null/empty schema or table to satisfy nullable analysis and avoid runtime errors
            if (string.IsNullOrEmpty(schema) || string.IsNullOrEmpty(table))
            {
                self.messages_log_textbox.AppendText("\n[Error] Schema or table is null or empty.");
                return;
            }

            ConnString = ConnString.Replace("{technical_user_username}", User);
            ConnString = ConnString.Replace("{technical_user_password}", Pass);

            string column_query = Globals.columns_query;
            self.fetch_status.Text = string.Empty;
            self.messages_log_textbox.Clear();
            if ((!string.IsNullOrEmpty(User) || !string.IsNullOrEmpty(Pass)) && !string.IsNullOrEmpty(ConnString))
            {
                column_query = column_query.Replace("%schema%", schema);
                column_query = column_query.Replace("%table%", table);
                self.messages_log_textbox.AppendText("--------------------------------------------------");
                self.messages_log_textbox.AppendText($"\nFetching columns from DB for {table} table...");
                self.messages_log_textbox.AppendText($"\nQuery: {column_query}");
                self.messages_log_textbox.AppendText("\n--------------------------------------------------");
                int columns_count = 0;
                Globals.internal_columns_list.Clear();
                DateTime startTime = DateTime.Now;

                try
                {
                    using (SqlConnection columns_db_connection = new SqlConnection(ConnString))
                    {
                        self.connection_process.Visibility = Visibility.Visible;
                        self.connection_process.Value = 0;
                        self.connection_process.Maximum = 100;

                        var progressTask = Task.Run(async () =>
                        {
                            while (self.connection_process.Value < 90)
                            {
                                await Task.Delay(100);
                                self.Dispatcher.Invoke(() =>
                                {
                                    if (self.connection_process.Value < self.connection_process.Maximum)
                                        self.connection_process.Value++;
                                });
                            }
                        });

                        try
                        {
                            await Task.Run(() => columns_db_connection.Open());
                            self.Dispatcher.Invoke(() =>
                            {
                                self.connection_process.Value = Math.Min(self.connection_process.Maximum, 100);
                            });
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Message.Contains("Timeout expired") || ex.Message.Contains("Connection Timeout Expired"))
                            {
                                MessageBox.Show(
                                    "Connection timed out while trying to reach the database.\nPlease check your connection settings or server availability.",
                                    "Connection Timeout",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning
                                );
                            }
                            else if (ex.Message.Contains("Can not connect") || ex.Message.Contains("in its current state"))
                            {
                                MessageBox.Show(
                                    "Access to database is restricted!\nPlease resolve this issue and then initiate a connection with the database.",
                                    "Database Locked or Restricted",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error
                                );
                            }
                            else
                            {
                                MessageBox.Show(
                                    $"Database connection failed:\n{ex.Message}",
                                    "SQL Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error
                                );
                            }

                            self.messages_log_textbox.AppendText($"\n[Error] {ex.Message}");
                            return;
                        }
                        catch (Exception ex)
                        {
                            self.connection_process.Value = 0;
                            self.messages_log_textbox.Clear();
                            self.messages_log_textbox.AppendText($"Connection failed: {ex.Message}");
                        }
                        finally
                        {
                            self.connection_process.Visibility = Visibility.Collapsed;
                        }
                        using (SqlCommand columns_command = new SqlCommand(column_query, columns_db_connection))
                        {
                            using (SqlDataReader column_reader = columns_command.ExecuteReader())
                            {
                                self.messages_log_textbox.AppendText("\nColumns found:");
                                while (column_reader.Read())
                                {
                                    self.messages_log_textbox.AppendText("\n" + (columns_count + 1).ToString() + ". " + column_reader["COLUMN_NAME"]);
                                    Globals.ColumnsViewStructure new_column_entry = new Globals.ColumnsViewStructure
                                    {
                                        Name = column_reader["COLUMN_NAME"].ToString()
                                    };
                                    Globals.internal_columns_list.Add(new_column_entry);
                                    columns_count++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    self.messages_log_textbox.Clear();
                    self.messages_log_textbox.AppendText($"\nUnhandled Exception: {ex.Message}");
                    self.messages_log_textbox.AppendText($"\nStack Trace: {ex.StackTrace}");
                    return;
                }
                self.fetch_status.Text = $"Fetched entries: {columns_count} | Time elapsed: {DateTime.Now - startTime}";
                self.messages_log_textbox.AppendText("\n--------------------------------------------------");
                self.messages_log_textbox.AppendText($"\nTotal columns fetched: {columns_count}");
                self.messages_log_textbox.AppendText($"\nTime elapsed: {DateTime.Now - startTime}");
                self.messages_log_textbox.AppendText("\n--------------------------------------------------");

                //helper.SetColumnsSelector(self);
            }
            else
            {
                MessageBox.Show("Username or Password is incorrect or empty! Please provide valid credentials.");
                return;
            }
        }

        public async Task FetchItems(AppForm self, string User, string Pass, string ConnString)
        {
            HelperFunctions helper = new HelperFunctions();

            if (!helper.ValidateConnectionString(ConnString))
            {
                self.messages_log_textbox.AppendText("\n[Error] Invalid connection string.");
                return;
            }

            ConnString = ConnString.Replace("{technical_user_username}", User);
            ConnString = ConnString.Replace("{technical_user_password}", Pass);

            string item_query = Globals.custom_sql_query;
            self.fetch_status.Text = string.Empty;
            self.messages_log_textbox.Clear();

            if ((!string.IsNullOrEmpty(User) || !string.IsNullOrEmpty(Pass)) && !string.IsNullOrEmpty(ConnString))
            {
                bool deepNode = HelperFunctions.GetSelectedNodeTag(self)?.Parent?.Parent?.Parent?.Parent != null;

                if (deepNode)
                {
                    self.messages_log_textbox.AppendText("--------------------------------------------------");
                    self.messages_log_textbox.AppendText($"\nFetching items from DB for [{Globals.schema}].[{Globals.table}]...");
                    self.messages_log_textbox.AppendText($"\nQuery: {item_query}");
                    self.messages_log_textbox.AppendText("\n--------------------------------------------------");
                }
                else
                {
                    self.messages_log_textbox.AppendText("--------------------------------------------------");
                    self.messages_log_textbox.AppendText($"\nFetching items from DB...");
                    self.messages_log_textbox.AppendText($"\nQuery: {item_query}");
                    self.messages_log_textbox.AppendText("\n--------------------------------------------------");
                }

                int item_count = 0;
                Globals.internal_items_list.Clear();
                DateTime startTime = DateTime.Now;

                try
                {
                    using (SqlConnection items_db_connection = new SqlConnection(ConnString))
                    {
                        // Progress bar animation
                        self.connection_process.Visibility = Visibility.Visible;
                        self.connection_process.Value = 0;
                        self.connection_process.Maximum = 100;

                        var progressTask = Task.Run(async () =>
                        {
                            while (self.connection_process.Value < 90)
                            {
                                await Task.Delay(100);
                                self.Dispatcher.Invoke(() =>
                                {
                                    if (self.connection_process.Value < self.connection_process.Maximum)
                                        self.connection_process.Value++;
                                });
                            }
                        });

                        try
                        {
                            await Task.Run(() => items_db_connection.Open());
                            self.Dispatcher.Invoke(() =>
                            {
                                self.connection_process.Value = Math.Min(self.connection_process.Maximum, 100);
                            });
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Message.Contains("Timeout expired") || ex.Message.Contains("Connection Timeout Expired"))
                            {
                                MessageBox.Show(
                                    "Connection timed out while trying to reach the database.\nPlease check your connection settings or server availability.",
                                    "Connection Timeout",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning
                                );
                            }
                            else if (ex.Message.Contains("Can not connect") || ex.Message.Contains("in its current state"))
                            {
                                MessageBox.Show(
                                    "Access to database is restricted!\nPlease resolve this issue and then initiate a connection with the database.",
                                    "Database Locked or Restricted",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error
                                );
                            }
                            else
                            {
                                MessageBox.Show(
                                    $"Database connection failed:\n{ex.Message}",
                                    "SQL Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error
                                );
                            }

                            self.messages_log_textbox.AppendText($"\n[Error] {ex.Message}");
                            return;
                        }
                        catch (Exception ex)
                        {
                            self.connection_process.Value = 0;
                            self.messages_log_textbox.Clear();
                            self.messages_log_textbox.AppendText($"Connection failed: {ex.Message}");
                        }
                        finally
                        {
                            self.connection_process.Visibility = Visibility.Collapsed;
                        }

                        using (SqlCommand items_command = new SqlCommand(item_query, items_db_connection))
                        using (SqlDataReader items_reader = items_command.ExecuteReader())
                        {
                            while (items_reader.Read())
                            {
                                var rowData = new Dictionary<string, object>();
                                for (int i = 0; i < items_reader.FieldCount; i++)
                                {
                                    var columnName = items_reader.GetName(i);
                                    var value = items_reader.IsDBNull(i) ? null : items_reader.GetValue(i);
                                    rowData[columnName] = value ?? DBNull.Value;
                                }
                                Globals.internal_items_list.Add(rowData);
                                item_count++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    self.messages_log_textbox.Clear();
                    self.messages_log_textbox.AppendText($"Unhandled Exception: {ex.Message}");
                    self.messages_log_textbox.AppendText($"Stack Trace: {ex.StackTrace}");
                    return;
                }

                if (Globals.internal_items_list.Count > 0)
                {
                    var headerNames = Globals.internal_items_list[0].Keys;
                    self.messages_log_textbox.AppendText("\n" + string.Join(" | ", headerNames));
                    self.messages_log_textbox.AppendText("\n" + new string('-', 80));

                    foreach (var dict in Globals.internal_items_list)
                    {
                        var rowValues = dict.Values.Select(v => v?.ToString() ?? "");
                        self.messages_log_textbox.AppendText("\n" + string.Join(" | ", rowValues));
                    }
                }

                self.fetch_status.Text = $"Fetched entries: {item_count} | Time elapsed: {DateTime.Now - startTime}";
                self.messages_log_textbox.AppendText("\n--------------------------------------------------");
                self.messages_log_textbox.AppendText($"\nTotal items fetched: {item_count}");
                self.messages_log_textbox.AppendText($"\nTime elapsed: {DateTime.Now - startTime}");
                self.messages_log_textbox.AppendText("\n--------------------------------------------------");

                var new_items_table = helper.ToDataTable(Globals.internal_items_list);
                helper.SetTableDataSource(self, new_items_table);
            }
            else
            {
                MessageBox.Show("Username or Password is incorrect or empty! Please provide valid credentials.");
                return;
            }
        }

        public async void PartialFetchItems(AppForm self, string User, string Pass, string ConnString)
        {
            HelperFunctions helper = new HelperFunctions();

            if (!helper.ValidateConnectionString(ConnString))
            {
                self.messages_log_textbox.AppendText("\n[Error] Invalid connection string.");
                return;
            }

            TextBox? current_textbox = helper.GetActiveQueryTextBox(self);

            ConnString = ConnString.Replace("{technical_user_username}", User);
            ConnString = ConnString.Replace("{technical_user_password}", Pass);

            if (current_textbox != null && !string.IsNullOrEmpty(current_textbox.SelectedText))
            {
                string item_query = current_textbox.SelectedText;

                self.fetch_status.Text = string.Empty;
                self.messages_log_textbox.Clear();

                if ((!string.IsNullOrEmpty(User) || !string.IsNullOrEmpty(Pass)) && !string.IsNullOrEmpty(ConnString))
                {
                    bool deepNode = HelperFunctions.GetSelectedNodeTag(self)?.Parent?.Parent?.Parent?.Parent != null;

                    if (deepNode)
                    {
                        self.messages_log_textbox.AppendText("--------------------------------------------------");
                        self.messages_log_textbox.AppendText($"\nFetching items from DB for [{Globals.schema}].[{Globals.table}]...");
                        self.messages_log_textbox.AppendText($"\nQuery: {item_query}");
                        self.messages_log_textbox.AppendText("\n--------------------------------------------------");
                    }
                    else
                    {
                        self.messages_log_textbox.AppendText("--------------------------------------------------");
                        self.messages_log_textbox.AppendText($"\nFetching items from DB...");
                        self.messages_log_textbox.AppendText($"\nQuery: {item_query}");
                        self.messages_log_textbox.AppendText("\n--------------------------------------------------");
                    }

                    int item_count = 0;
                    Globals.internal_items_list.Clear();
                    DateTime startTime = DateTime.Now;

                    try
                    {
                        using (SqlConnection items_db_connection = new SqlConnection(ConnString))
                        {
                            // Progress bar animation
                            self.connection_process.Visibility = Visibility.Visible;
                            self.connection_process.Value = 0;
                            self.connection_process.Maximum = 100;

                            var progressTask = Task.Run(async () =>
                            {
                                while (self.connection_process.Value < 90)
                                {
                                    await Task.Delay(100);
                                    self.Dispatcher.Invoke(() =>
                                    {
                                        if (self.connection_process.Value < self.connection_process.Maximum)
                                            self.connection_process.Value++;
                                    });
                                }
                            });

                            try
                            {
                                await Task.Run(() => items_db_connection.Open());
                                self.Dispatcher.Invoke(() =>
                                {
                                    self.connection_process.Value = Math.Min(self.connection_process.Maximum, 100);
                                });
                            }
                            catch (SqlException ex)
                            {
                                if (ex.Message.Contains("Timeout expired") || ex.Message.Contains("Connection Timeout Expired"))
                                {
                                    MessageBox.Show(
                                        "Connection timed out while trying to reach the database.\nPlease check your connection settings or server availability.",
                                        "Connection Timeout",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning
                                    );
                                }
                                else if (ex.Message.Contains("Can not connect") || ex.Message.Contains("in its current state"))
                                {
                                    MessageBox.Show(
                                        "Access to database is restricted!\nPlease resolve this issue and then initiate a connection with the database.",
                                        "Database Locked or Restricted",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error
                                    );
                                }
                                else
                                {
                                    MessageBox.Show(
                                        $"Database connection failed:\n{ex.Message}",
                                        "SQL Error",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error
                                    );
                                }

                                self.messages_log_textbox.AppendText($"\n[Error] {ex.Message}");
                                return;
                            }
                            catch (Exception ex)
                            {
                                self.connection_process.Value = 0;
                                self.messages_log_textbox.Clear();
                                self.messages_log_textbox.AppendText($"Connection failed: {ex.Message}");
                            }
                            finally
                            {
                                self.connection_process.Visibility = Visibility.Collapsed;
                            }

                            using (SqlCommand items_command = new SqlCommand(item_query, items_db_connection))
                            {
                                using (SqlDataReader items_reader = items_command.ExecuteReader())
                                {
                                    while (items_reader.Read())
                                    {
                                        var rowData = new Dictionary<string, object>();
                                        for (int i = 0; i < items_reader.FieldCount; i++)
                                        {
                                            var columnName = items_reader.GetName(i);
                                            var value = items_reader.IsDBNull(i) ? null : items_reader.GetValue(i);
                                            rowData[columnName] = value ?? DBNull.Value;
                                        }
                                        Globals.internal_items_list.Add(rowData);
                                        item_count++;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        self.messages_log_textbox.Clear();
                        self.messages_log_textbox.AppendText($"Unhandled Exception: {ex.Message}");
                        self.messages_log_textbox.AppendText($"Stack Trace: {ex.StackTrace}");
                        return;
                    }

                    if (Globals.internal_items_list.Count > 0)
                    {
                        var headerNames = Globals.internal_items_list[0].Keys;
                        self.messages_log_textbox.AppendText("\n" + string.Join(" | ", headerNames));
                        self.messages_log_textbox.AppendText("\n" + new string('-', 80));

                        foreach (var dict in Globals.internal_items_list)
                        {
                            var rowValues = dict.Values.Select(v => v?.ToString() ?? "");
                            self.messages_log_textbox.AppendText("\n" + string.Join(" | ", rowValues));
                        }
                    }

                    self.fetch_status.Text = $"Fetched entries: {item_count} | Time elapsed: {DateTime.Now - startTime}";
                    self.messages_log_textbox.AppendText("\n--------------------------------------------------");
                    self.messages_log_textbox.AppendText($"\nTotal items fetched: {item_count}");
                    self.messages_log_textbox.AppendText($"\nTime elapsed: {DateTime.Now - startTime}");
                    self.messages_log_textbox.AppendText("\n--------------------------------------------------");

                    var new_items_table = helper.ToDataTable(Globals.internal_items_list);
                    helper.SetTableDataSource(self, new_items_table);
                }
                else
                {
                    MessageBox.Show("Username or Password is incorrect or empty! Please provide valid credentials.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Partial fetch failed!");
                return;
            }
        }

        public static void SelectItemsFromSelectedTable(AppForm self)
        {
            var node = HelperFunctions.GetSelectedNodeTag(self);
            if (self == null || node == null) return;

            self.messages_log_textbox.Clear();

            // Default values
            string? column_name = string.Empty;
            string? table_name = string.Empty;
            string? schema_name = string.Empty;
            string? db_nickname = string.Empty;

            string login_user = string.Empty;
            string login_pass = string.Empty;
            string login_connstring = string.Empty;

            // Identify which type of node was selected based on depth
            switch (node.Level)
            {
                case 0: // Connection
                    db_nickname = node.Text;
                    break;

                case 1: // Schema
                    schema_name = node.Text;
                    db_nickname = node.Parent?.Text;
                    break;

                case 2: // Table
                    table_name = node.Text;
                    schema_name = node.Parent?.Text;
                    db_nickname = node.Parent?.Parent?.Text;
                    break;

                case 3: // Column
                    column_name = node.Text;
                    table_name = node.Parent?.Text;
                    schema_name = node.Parent?.Parent?.Text;
                    db_nickname = node.Parent?.Parent?.Parent?.Text;
                    break;
                default:
                    return;
            }

            // Get connection credentials based on the selected connection nickname
            foreach (var conn in Globals.login_list)
            {
                if (conn.Nickname == db_nickname)
                {
                    login_user = conn.Username;
                    login_pass = conn.Password;
                    login_connstring = conn.ConnectionString;
                    break;
                }
            }

            // Mask password for logging(same number of asterisks)
            string masked_pass = string.IsNullOrEmpty(login_pass) ? "(not set)" : new string('*', login_pass.Length);

            // Store connection & selection info globally
            Globals.sql_technical_user_username = login_user;
            Globals.sql_technical_user_password = login_pass;
            Globals.connectionString = login_connstring;
            Globals.table = table_name;
            Globals.schema = schema_name;
            Globals.column = column_name;

            // Append output to messages log — same format you used before
            self.messages_log_textbox.AppendText($"Database Nickname: {db_nickname}\n");
            self.messages_log_textbox.AppendText($"Schema: {schema_name}\n");
            self.messages_log_textbox.AppendText($"Table: {table_name}\n");
            self.messages_log_textbox.AppendText($"Column: {column_name}\n");
            self.messages_log_textbox.AppendText($"Username: {login_user}\n");
            self.messages_log_textbox.AppendText($"Password: {masked_pass}\n");
            self.messages_log_textbox.AppendText($"Connection String: {login_connstring}\n");

            // Finally, call SetQuery() to update the query display
            //HelperFunctions.SetQuery(self);
        }
    }
}
