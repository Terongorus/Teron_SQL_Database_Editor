using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using System.Xml;
using AzureEditor;

namespace Logic
{
    public class ExportQueryResults
    {
        public static void ExportQueryResultsAsCSV(AppForm self)
        {
            HelperFunctions helper = new HelperFunctions();

            if (self.data_viewer.DataSource != null)
            {
                SaveFileDialog new_save_file_dialog = new SaveFileDialog();
                new_save_file_dialog.Title = "Export Query Result as CSV";
                new_save_file_dialog.FileName = (Globals.table ?? "table") + "_query_results";
                new_save_file_dialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                new_save_file_dialog.DefaultExt = "csv";
                new_save_file_dialog.AddExtension = true;

                DataTable new_table = helper.ToDataTable(Globals.internal_items_list);

                StringBuilder csv = new StringBuilder();


                if (new_save_file_dialog.ShowDialog() == DialogResult.OK)
                {
                    // Add column headers
                    for (int i = 0; i < new_table.Columns.Count; i++)
                    {
                        csv.Append(new_table.Columns[i].ColumnName);
                        if (i < new_table.Columns.Count - 1)
                            csv.Append(",");
                    }
                    csv.AppendLine();

                    foreach (DataRow row in new_table.Rows)
                    {
                        for (int i = 0; i < new_table.Columns.Count; i++)
                        {
                            string value = row[i]?.ToString() ?? string.Empty;

                            // Escape quotes and commas
                            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                            {
                                value = "\"" + value.Replace("\"", "\"\"") + "\"";
                            }

                            csv.Append(value);
                            if (i < new_table.Columns.Count - 1)
                                csv.Append(",");
                        }
                        csv.AppendLine();
                    }
                    File.WriteAllText(new_save_file_dialog.FileName, csv.ToString(), Encoding.UTF8);
                    MessageBox.Show("CSV file exported successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Export process terminated!", "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("No data in Query Editor! Please run a query to fetch results and then export them!");
            }
        }

        public static void ExportQueryResultsAsSQL(AppForm self)
        {
            HelperFunctions helper = new HelperFunctions();

            if (self.data_viewer.DataSource != null)
            {
                SaveFileDialog dlg = new SaveFileDialog
                {
                    Title = "Export Query Result as SQL",
                    FileName = (Globals.table ?? "table") + "_query_results",
                    Filter = "SQL files (*.sql)|*.sql|All files (*.*)|*.*",
                    DefaultExt = "sql",
                    AddExtension = true
                };

                DataTable table = helper.ToDataTable(Globals.internal_items_list);
                StringBuilder sql = new StringBuilder();

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        sql.Append($"INSERT INTO {Globals.table} (");

                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            sql.Append(table.Columns[i].ColumnName);
                            if (i < table.Columns.Count - 1)
                                sql.Append(", ");
                        }

                        sql.Append(") VALUES (");

                        for (int i = 0; i < table.Columns.Count; i++)
                        {
                            string value = row[i]?.ToString()?.Replace("'", "''") ?? string.Empty;
                            sql.Append($"'{value}'");
                            if (i < table.Columns.Count - 1)
                                sql.Append(", ");
                        }

                        sql.AppendLine(");");
                    }
                    File.WriteAllText(dlg.FileName, sql.ToString(), Encoding.UTF8);
                    MessageBox.Show("SQL file exported successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Export process terminated!", "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("No data in Query Editor! Please run a query to fetch results and then export them!");
            }
        }

        public static void ExportQueryResultsAsXML(AppForm self)
        {
            HelperFunctions helper = new HelperFunctions();

            if (self.data_viewer.DataSource != null)
            {
                SaveFileDialog dlg = new SaveFileDialog
                {
                    Title = "Export Query Result as XML",
                    FileName = (Globals.table ?? "table") + "_query_results",
                    Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                    DefaultExt = "xml",
                    AddExtension = true
                };

                DataTable table = helper.ToDataTable(Globals.internal_items_list);

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    using (XmlWriter writer = XmlWriter.Create(dlg.FileName, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
                    {
                        table.TableName = Globals.table?.ToString() ?? "table";
                        table.WriteXml(writer, XmlWriteMode.WriteSchema, false);
                    }
                    MessageBox.Show("XML file exported successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Export process terminated!", "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("No data in Query Editor! Please run a query to fetch results and then export them!");
            }
        }

        public static void ExportQueryResultsAsJSON(AppForm self)
        {
            HelperFunctions helper = new HelperFunctions();

            if (self.data_viewer.DataSource != null)
            {
                SaveFileDialog dlg = new SaveFileDialog
                {
                    Title = "Export Query Result as JSON",
                    FileName = (Globals.table ?? "table") + "_query_results",
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    DefaultExt = "json",
                    AddExtension = true
                };

                DataTable table = helper.ToDataTable(Globals.internal_items_list);

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    // Convert DataTable → list of dictionaries
                    var rows = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>>();
                    foreach (DataRow row in table.Rows)
                    {
                        var dict = new System.Collections.Generic.Dictionary<string, object>();
                        foreach (DataColumn col in table.Columns)
                            dict[col.ColumnName] = row[col];
                        rows.Add(dict);
                    }

                    string json = JsonSerializer.Serialize(rows, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    File.WriteAllText(dlg.FileName, json, Encoding.UTF8);
                    MessageBox.Show("JSON file exported successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Export process terminated!", "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("No data in Query Editor! Please run a query to fetch results and then export them!");
            }
        }
    }
}
