using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using Logic;
using static Logic.Globals;

namespace Database
{
    public partial class DBCEditor : Window
    {
        DBCLogic DBCLogic = new DBCLogic();
        public DBCEditor()
        {
            InitializeComponent();
            Title = $"DBC Editor - {AppInfo.DisplayNameWithVersion}";
        }

        private void open_file_Click(object sender, RoutedEventArgs e)
        {
            DBCLogic.SelectFile();
        }

        private void DBCEditor_Closed(object? sender, EventArgs e)
        {
            startup.Show();
            startup.Activate();
        }

        private void save_file_Click(object sender, RoutedEventArgs e)
        {
            DBCLogic.SaveFile();
        }

        private void save_file_as_Click(object sender, RoutedEventArgs e)
        {
            DBCLogic.SaveFileAs();
        }

        private void save_all_Click(object sender, RoutedEventArgs e)
        {
            DBCLogic.SaveAll();
        }

        private void SetMessagesPanelVisible(bool visible)
        {
            messages_control.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            messages_splitter.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void error_box_button_Click(object sender, RoutedEventArgs e)
        {
            bool error_frame_visible = messages_control.SelectedItem == errors_tab && messages_control.Visibility == Visibility.Visible;
            if (!error_frame_visible) { messages_control.SelectedItem = errors_tab; SetMessagesPanelVisible(true); }
            else SetMessagesPanelVisible(false);
        }

        private void warning_box_button_Click(object sender, RoutedEventArgs e)
        {
            bool warning_frame_visible = messages_control.SelectedItem == warnings_tab && messages_control.Visibility == Visibility.Visible;
            if (!warning_frame_visible) { messages_control.SelectedItem = warnings_tab; SetMessagesPanelVisible(true); }
            else SetMessagesPanelVisible(false);
        }

        private void message_box_button_Click(object sender, RoutedEventArgs e)
        {
            bool message_frame_visible = messages_control.SelectedItem == messages_tab && messages_control.Visibility == Visibility.Visible;
            if (!message_frame_visible) { messages_control.SelectedItem = messages_tab; SetMessagesPanelVisible(true); }
            else SetMessagesPanelVisible(false);
        }

        private void filter_enable_Click(object sender, RoutedEventArgs e)
        {
            DBCLogic.CreateNewFilter();
        }

        private void filter_disable_Click(object sender, RoutedEventArgs e)
        {
            DBCLogic.ClearFilter();
        }
    }

    public class DBCLogic
    {
        private DataTable ToDataTable(BindingList<Dictionary<string, object>> list)
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

        public bool? SelectFile()
        {
            dbc_viewer_struct.Clear();

            var ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "DBC files (*.dbc)|*.dbc|All files (*.*)|*.*";
            ofd.Multiselect = false;
            ofd.Title = "Select DBC file(s)";
            ofd.FileOk += (sender, e) =>
            {
                try
                {
                    // Prompt for client version BEFORE reading the file
                    var selectedVersion = PromptForClientVersion();
                    if (selectedVersion == null)
                    {
                        e.Cancel = true;
                        return;
                    }

                    using (var raw_data = ofd.OpenFile())
                    using (var data_reader = new BinaryReader(raw_data))
                    {
                        // Header validation
                        byte[] header = data_reader.ReadBytes(4);
                        if (Encoding.ASCII.GetString(header) != "WDBC")
                        {
                            MessageBox.Show("Invalid DBC file format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            dbc_editor?.errors_text_box.AppendText($"File '{ofd.FileName}' is not a valid DBC file.\n");
                            e.Cancel = true;
                            return;
                        }

                        // Metadata
                        int recordCount = data_reader.ReadInt32();
                        int fieldCount = data_reader.ReadInt32();
                        int recordSize = data_reader.ReadInt32();
                        int stringBlockSize = data_reader.ReadInt32();

                        // Load field definitions from XML to determine field types
                        string dbcFileName = Path.GetFileNameWithoutExtension(ofd.FileName);
                        var fieldDefinitions = LoadFieldDefinitionsFromXml(dbcFileName, selectedVersion);

                        // If definitions couldn't be loaded, use default uint32 for all fields
                        if (fieldDefinitions.Count == 0)
                        {
                            for (int i = 0; i < fieldCount; i++)
                            {
                                fieldDefinitions.Add((Name: $"Field{i}", Type: "uint", ArraySize: 1));
                            }
                        }

                        // Read records with type awareness
                        var records = new BindingList<Dictionary<string, object>>();
                        var stringOffsets = new List<(int RecordIndex, int FieldIndex, uint Offset)>();

                        for (int i = 0; i < recordCount; i++)
                        {
                            var record = new Dictionary<string, object>();
                            int fieldIndex = 0;

                            foreach (var fieldDef in fieldDefinitions)
                            {
                                for (int arrayIdx = 0; arrayIdx < fieldDef.ArraySize; arrayIdx++)
                                {
                                    uint rawValue = data_reader.ReadUInt32();
                                    object fieldValue = rawValue;

                                    // Convert based on field type
                                    switch (fieldDef.Type.ToLower())
                                    {
                                        case "int":
                                            fieldValue = unchecked((int)rawValue);
                                            break;
                                        case "uint":
                                        case "uint32":
                                            fieldValue = rawValue;
                                            break;
                                        case "byte":
                                        case "uint8":
                                            fieldValue = (byte)rawValue;
                                            break;
                                        case "float":
                                            fieldValue = BitConverter.ToSingle(BitConverter.GetBytes(rawValue), 0);
                                            break;
                                        case "string":
                                        case "loc":
                                            // Store offset for later resolution
                                            stringOffsets.Add((i, fieldIndex, rawValue));
                                            fieldValue = rawValue; // Placeholder, will be resolved after
                                            break;
                                    }

                                    record.Add($"Field{fieldIndex}", fieldValue);
                                    fieldIndex++;
                                }
                            }

                            records.Add(record);
                        }

                        // Read string block
                        byte[] stringBlock = data_reader.ReadBytes(stringBlockSize);

                        // Resolve string offsets
                        if (stringOffsets.Count > 0)
                        {
                            dbc_editor?.messages_text_box.AppendText($"Resolving {stringOffsets.Count} string offset(s) from string block size: {stringBlock.Length}\n");
                        }
                        foreach (var (recordIdx, fieldIdx, offset) in stringOffsets)
                        {
                            string resolvedString = ResolveStringOffset(offset, stringBlock);
                            records[recordIdx][$"Field{fieldIdx}"] = resolvedString;

                            // Log first few resolutions for debugging
                            if (recordIdx < 2)
                            {
                                dbc_editor?.messages_text_box.AppendText($"  Record {recordIdx}, Field {fieldIdx}: offset={offset} -> \"{resolvedString}\"\n");
                            }
                        }

                        dbc_viewer_struct = records;
                    }

                    var new_dbc_data = ToDataTable(dbc_viewer_struct);
                    var new_tab = new TabItem { Header = Path.GetFileName(ofd.FileName) };
                    new_tab.Tag = ofd.FileName; // Store the full file path
                    if (dbc_editor is not null)
                    {
                        CreateNewDataGrid(dbc_editor.dbc_tab_control, new_tab, new_dbc_data, dbc_editor, Path.GetFileName(ofd.FileName), selectedVersion);
                    }
                    else
                    {
                        throw new Exception("DBC Editor form is not initialized.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading DBC file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    dbc_editor?.errors_text_box.AppendText($"Error loading file '{ofd.FileName}': {ex.Message}\n");
                    e.Cancel = true;
                }
            };

            return ofd.ShowDialog();
        }

        private List<(string Name, string Type, int ArraySize)> LoadFieldDefinitionsFromXml(string dbcFileName, string clientVersion)
        {
            var fieldDefinitions = new List<(string Name, string Type, int ArraySize)>();

            try
            {
                string xmlFilePath = Path.Combine(definitons_dir, clientVersion);

                if (!File.Exists(xmlFilePath))
                {
                    dbc_editor?.warnings_text_box.AppendText($"Definition file not found: {clientVersion}\n");
                    return fieldDefinitions;
                }

                XDocument doc = XDocument.Load(xmlFilePath);
                var table = doc.Descendants("Table").FirstOrDefault(t => t.Attribute("Name")?.Value == dbcFileName);

                if (table == null)
                {
                    dbc_editor?.warnings_text_box.AppendText($"Table definition for '{dbcFileName}' not found in {clientVersion}.\n");
                    return fieldDefinitions;
                }

                var fields = table.Descendants("Field").ToList();
                dbc_editor?.messages_text_box.AppendText($"Loaded {fields.Count} field definitions from {dbcFileName}\n");

                foreach (var fieldElement in fields)
                {
                    string? fieldName = fieldElement.Attribute("Name")?.Value;
                    if (string.IsNullOrEmpty(fieldName))
                        continue;

                    string? fieldType = fieldElement.Attribute("Type")?.Value ?? "uint";

                    int arraySize = 1;
                    var arraySizeAttr = fieldElement.Attribute("ArraySize");
                    if (arraySizeAttr != null && int.TryParse(arraySizeAttr.Value, out int size))
                    {
                        arraySize = size;
                    }

                    // Log string/loc field types for debugging
                    if (fieldType.Equals("string", StringComparison.OrdinalIgnoreCase) || fieldType.Equals("loc", StringComparison.OrdinalIgnoreCase))
                    {
                        dbc_editor?.messages_text_box.AppendText($"  Field: {fieldName} Type: {fieldType} ArraySize: {arraySize}\n");
                    }

                    for (int i = 0; i < arraySize; i++)
                    {
                        fieldDefinitions.Add((fieldName, fieldType, 1));
                    }
                }
            }
            catch (Exception ex)
            {
                dbc_editor?.warnings_text_box.AppendText($"Error loading field definitions: {ex.Message}\n");
            }

            return fieldDefinitions;
        }

        private string ResolveStringOffset(uint offset, byte[] stringBlock)
        {
            if (offset >= stringBlock.Length)
                return string.Empty;

            int startIdx = (int)offset;
            int endIdx = startIdx;

            // Find null terminator
            while (endIdx < stringBlock.Length && stringBlock[endIdx] != 0)
            {
                endIdx++;
            }

            if (endIdx <= startIdx)
                return string.Empty;

            return Encoding.UTF8.GetString(stringBlock, startIdx, endIdx - startIdx);
        }

        // Returns the rows currently visible in the grid (i.e. respecting any active RowFilter),
        // matching the original WinForms behavior where a filtered BindingSource meant Save/SaveAs/
        // SaveAll only wrote back the currently-displayed subset of rows, not the whole table.
        private static DataView? GetBackingView(DataGrid dataGridView)
        {
            return dataGridView.ItemsSource as DataView;
        }

        public void SaveFile()
        {
            try
            {
                // Get the currently active tab
                if (dbc_editor?.dbc_tab_control?.SelectedItem is not TabItem selectedTab)
                {
                    MessageBox.Show("No DBC file is currently loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    dbc_editor?.errors_text_box.AppendText("Save failed: No DBC file is currently loaded.\n");
                    return;
                }

                // Get the DataGrid from the active tab
                DataGrid? dataGridView = selectedTab.Content as DataGrid;
                DataView? view = dataGridView != null ? GetBackingView(dataGridView) : null;
                if (dataGridView == null || view == null)
                {
                    MessageBox.Show("DataGridView not found in the active tab.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    dbc_editor?.errors_text_box.AppendText("Save failed: DataGridView not found in the active tab.\n");
                    return;
                }

                // Verify we have data to save
                if (view.Count == 0)
                {
                    MessageBox.Show("No data to save.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    dbc_editor?.errors_text_box.AppendText("Save failed: No data to save.\n");
                    return;
                }

                try
                {
                    string? tag = selectedTab.Tag?.ToString();
                    // Get the original file path from the tab's Tag property
                    string filePath = tag ?? string.Empty;
                    if (string.IsNullOrEmpty(filePath))
                    {
                        MessageBox.Show("File path not found. Please use 'Save As' instead.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        dbc_editor?.errors_text_box.AppendText("Save failed: File path not found. Please use 'Save As' instead.\n");
                        return;
                    }

                    WriteDbcFile(filePath, view);
                    dbc_editor?.messages_text_box.AppendText($"File saved successfully: {filePath}\n");
                    if (dbc_editor != null) dbc_editor.message_text.Text = $"File saved successfully: {filePath}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    dbc_editor?.errors_text_box.AppendText($"Error saving file: {ex.Message}\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                dbc_editor?.errors_text_box.AppendText($"Error: {ex.Message}\n");
            }
        }

        public bool? SaveFileAs()
        {
            try
            {
                // Get the currently active tab
                if (dbc_editor?.dbc_tab_control?.SelectedItem is not TabItem selectedTab)
                {
                    MessageBox.Show("No DBC file is currently loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    dbc_editor?.errors_text_box.AppendText("Save As failed: No DBC file is currently loaded.\n");
                    return false;
                }

                // Get the DataGrid from the active tab
                DataGrid? dataGridView = selectedTab.Content as DataGrid;
                DataView? view = dataGridView != null ? GetBackingView(dataGridView) : null;
                if (dataGridView == null || view == null)
                {
                    MessageBox.Show("DataGridView not found in the active tab.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    dbc_editor?.errors_text_box.AppendText("Save As failed: DataGridView not found in the active tab.\n");
                    return false;
                }

                // Verify we have data to save
                if (view.Count == 0)
                {
                    MessageBox.Show("No data to save.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    dbc_editor?.errors_text_box.AppendText("Save As failed: No data to save.\n");
                    return false;
                }

                var svf = new Microsoft.Win32.SaveFileDialog();
                svf.Filter = "DBC files (*.dbc)|*.dbc|All files (*.*)|*.*";
                svf.FileName = selectedTab.Header?.ToString() ?? "file.dbc"; // Default to current tab name
                svf.Title = "Save DBC file";
                svf.FileOk += (sender, e) =>
                {
                    try
                    {
                        string filePath = svf.FileName;
                        WriteDbcFile(filePath, view);
                        dbc_editor?.messages_text_box.AppendText($"File saved successfully: {filePath}\n");
                        if (dbc_editor != null) dbc_editor.message_text.Text = $"File saved successfully: {filePath}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        dbc_editor?.errors_text_box.AppendText($"Error saving file: {ex.Message}\n");
                    }
                };

                return svf.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing save dialog: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                dbc_editor?.errors_text_box.AppendText($"Error initializing save dialog: {ex.Message}\n");
                return false;
            }
        }

        public void SaveAll()
        {
            try
            {
                // Check if there are any tabs open
                if (dbc_editor?.dbc_tab_control?.Items.Count == 0)
                {
                    MessageBox.Show("No DBC files are open.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    dbc_editor?.errors_text_box.AppendText("No DBC files are open.\n");
                    return;
                }

                int savedCount = 0;
                int failedCount = 0;
                var failedFiles = new List<string>();
                var tab_pages = dbc_editor?.dbc_tab_control.Items.Cast<TabItem>().ToList();

                // Iterate through all tabs
                foreach (TabItem tab in tab_pages ?? Enumerable.Empty<TabItem>())
                {
                    string tabName = tab.Header?.ToString() ?? "file.dbc";
                    try
                    {
                        // Get the DataGrid from the tab
                        DataGrid? dataGridView = tab.Content as DataGrid;
                        DataView? view = dataGridView != null ? GetBackingView(dataGridView) : null;
                        if (dataGridView == null || view == null)
                        {
                            failedCount++;
                            failedFiles.Add($"{tabName} (DataGridView not found)");
                            continue;
                        }

                        // Skip if no data
                        if (view.Count == 0)
                        {
                            failedCount++;
                            failedFiles.Add($"{tabName} (no data)");
                            continue;
                        }

                        // Get the file path from the tab's Tag property
                        string? filePath = tab.Tag?.ToString();
                        if (string.IsNullOrEmpty(filePath))
                        {
                            failedCount++;
                            failedFiles.Add($"{tabName} (file path not found)");
                            continue;
                        }

                        WriteDbcFile(filePath, view);
                        savedCount++;
                    }
                    catch (Exception ex)
                    {
                        failedCount++;
                        failedFiles.Add($"{tabName} ({ex.Message})");
                    }
                }

                // Show summary
                var message = $"Saved {savedCount} file(s)";
                if (failedCount > 0)
                {
                    message += $", failed to save {failedCount} file(s):\n";
                    message += string.Join("\n", failedFiles);
                    dbc_editor?.messages_text_box.AppendText($"Save All completed with errors: {failedCount} file(s) failed to save.\n");
                    if (dbc_editor != null) dbc_editor.message_text.Text = $"Save All completed with errors: {failedCount} file(s) failed to save.";
                }
                else
                {
                    dbc_editor?.messages_text_box.AppendText($"Save All completed successfully: {savedCount} file(s) saved.\n");
                    if (dbc_editor != null) dbc_editor.message_text.Text = $"Save All completed successfully: {savedCount} file(s) saved.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in Save All: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                dbc_editor?.errors_text_box.AppendText($"Error in Save All: {ex.Message}\n");
            }
        }

        // Shared binary writer used by SaveFile/SaveFileAs/SaveAll - writes exactly the rows
        // currently visible in `view` (i.e. respecting any active RowFilter), matching the
        // original DataGridView-bound-to-a-filtered-BindingSource behavior.
        private static void WriteDbcFile(string filePath, DataView view)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(fileStream))
            {
                // Write WDBC header
                writer.Write(Encoding.ASCII.GetBytes("WDBC"));

                // Prepare record data
                int recordCount = view.Count;
                int fieldCount = view.Table?.Columns.Count ?? 0;
                int recordSize = fieldCount * 4; // Each field is uint32 (4 bytes)
                int stringBlockSize = 0; // TODO: Implement string block handling

                // Write metadata
                writer.Write(recordCount);
                writer.Write(fieldCount);
                writer.Write(recordSize);
                writer.Write(stringBlockSize);

                // Write records - convert each cell value to uint32
                for (int i = 0; i < recordCount; i++)
                {
                    for (int j = 0; j < fieldCount; j++)
                    {
                        object? cellValue = view[i][j];
                        uint value = 0;

                        // Try to convert cell value to uint32
                        if (cellValue != null && cellValue != DBNull.Value)
                        {
                            if (cellValue is uint)
                            {
                                value = (uint)cellValue;
                            }
                            else if (uint.TryParse(cellValue.ToString(), out uint parsedValue))
                            {
                                value = parsedValue;
                            }
                        }

                        writer.Write(value);
                    }
                }

                // Write string block (currently empty)
                // TODO: Implement string offset resolution for string fields
            }
        }

        private string? PromptForClientVersion()
        {
            try
            {
                if (!Directory.Exists(definitons_dir))
                {
                    MessageBox.Show("Definitions folder not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    dbc_editor?.errors_text_box.AppendText($"Definitions folder not found: {definitons_dir}\n");
                    return null;
                }

                // Get all available XML definition files with display name and full filename
                var xmlFiles = Directory.GetFiles(definitons_dir, "*.xml")
                    .Where(f => !f.EndsWith("Offsets.json", StringComparison.OrdinalIgnoreCase) && !f.EndsWith("WDB.xml", StringComparison.OrdinalIgnoreCase))
                    .Select(f => new { Display = Path.GetFileNameWithoutExtension(f), FullName = Path.GetFileName(f) })
                    .OrderBy(f => f.FullName, new VersionComparer())
                    .ToArray();

                if (xmlFiles.Length == 0)
                {
                    MessageBox.Show("No definition files found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    dbc_editor?.errors_text_box.AppendText($"No definition files found in: {definitons_dir}\n");
                    return null;
                }

                // Let user select the client version
                var window = new Window
                {
                    Title = "Select Client Version",
                    Width = 400,
                    Height = 200,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    ResizeMode = ResizeMode.NoResize
                };

                var label = new Label { Content = "Select the client version for this DBC file:", Margin = new Thickness(10, 10, 10, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
                var comboBox = new ComboBox
                {
                    Margin = new Thickness(10, 35, 10, 0),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top,
                    ItemsSource = xmlFiles,
                    DisplayMemberPath = "Display",
                    SelectedValuePath = "FullName"
                };

                bool dialogResult = false;
                var okButton = new Button { Content = "OK", Width = 80, Height = 30, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(210, 100, 0, 0), IsDefault = true };
                var cancelButton = new Button { Content = "Cancel", Width = 80, Height = 30, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(300, 100, 0, 0), IsCancel = true };

                okButton.Click += (s, e) => { dialogResult = true; window.Close(); };
                cancelButton.Click += (s, e) => { dialogResult = false; window.Close(); };

                var grid = new Grid();
                grid.Children.Add(label);
                grid.Children.Add(comboBox);
                grid.Children.Add(okButton);
                grid.Children.Add(cancelButton);
                window.Content = grid;

                window.ShowDialog();

                if (!dialogResult)
                    return null;

                return comboBox.SelectedValue?.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting client version: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                dbc_editor?.errors_text_box.AppendText($"Error selecting client version: {ex.Message}\n");
                return null;
            }
        }

        private void CreateNewDataGrid(TabControl parent, TabItem tabPage, DataTable dbc_data, DBCEditor editor, string dbcFileName, string clientVersion)
        {
            DataGrid dataGridView = new DataGrid();
            dataGridView.AutoGenerateColumns = false;
            dataGridView.CanUserAddRows = true;
            dataGridView.CanUserDeleteRows = true;

            // Right-click a tab to close it, replacing the WinForms owner-drawn tab
            // hit-testing hack with a plain WPF context menu wired directly on the tab.
            tabPage.MouseRightButtonDown += (s, e) =>
            {
                parent.SelectedItem = tabPage;
                var menu = new ContextMenu();
                var closeCurrent = new MenuItem { Header = "Close Current Tab" };
                closeCurrent.Click += (s2, e2) => parent.Items.Remove(tabPage);
                var closeAll = new MenuItem { Header = "Close All Tabs" };
                closeAll.Click += (s2, e2) => parent.Items.Clear();
                menu.Items.Add(closeCurrent);
                menu.Items.Add(closeAll);
                menu.IsOpen = true;
            };

            // Load and apply field definitions BEFORE binding data
            LoadDataDefinitions(dbcFileName, clientVersion, dataGridView, dbc_data);

            // If definitions failed to load, use default column names
            if (dataGridView.Columns.Count == 0)
            {
                dataGridView.AutoGenerateColumns = true;
                dataGridView.ItemsSource = dbc_data.DefaultView;
            }

            tabPage.Content = dataGridView;
            parent.Items.Add(tabPage);
            parent.SelectedItem = tabPage;

            if (dbc_editor != null)
                dbc_editor.message_text.Text = $"Loaded {dbcFileName} with client version {clientVersion.Replace(".xml", string.Empty)}.";
        }

        private void LoadDataDefinitions(string dbcFileName, string clientVersion, DataGrid dataGridView, DataTable dbc_data)
        {
            try
            {
                string xmlFilePath = Path.Combine(definitons_dir, clientVersion);

                if (!File.Exists(xmlFilePath))
                {
                    MessageBox.Show($"Definition file not found: {clientVersion}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    dbc_editor?.warnings_text_box.AppendText($"Definition file not found: {clientVersion}\n");
                    dataGridView.ItemsSource = dbc_data.DefaultView;
                    return;
                }

                // Parse the XML file
                XDocument doc = XDocument.Load(xmlFilePath);

                // Extract table name from DBC filename (e.g., "Achievement.dbc" -> "Achievement")
                string tableName = Path.GetFileNameWithoutExtension(dbcFileName);

                // Find the matching table in the XML
                var table = doc.Descendants("Table").FirstOrDefault(t => t.Attribute("Name")?.Value == tableName);

                if (table == null)
                {
                    MessageBox.Show($"Table definition for '{tableName}' not found in {clientVersion}.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    dbc_editor?.warnings_text_box.AppendText($"Table definition for '{tableName}' not found in {clientVersion}.\n");
                    dataGridView.ItemsSource = dbc_data.DefaultView;
                    return;
                }

                // Extract all Field elements from the XML table definition
                var fields = table.Descendants("Field").ToList();

                if (fields.Count == 0)
                {
                    MessageBox.Show($"No fields found for table '{tableName}'.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    dbc_editor?.warnings_text_box.AppendText($"No fields found for table '{tableName}' in {clientVersion}.\n");
                    dataGridView.ItemsSource = dbc_data.DefaultView;
                    return;
                }

                // Create DataGridTextColumns with proper headers
                // Process each field and expand array fields into indexed columns
                int fieldIndex = 0;
                foreach (var fieldElement in fields)
                {
                    string? fieldName = fieldElement.Attribute("Name")?.Value;
                    if (string.IsNullOrEmpty(fieldName))
                        continue;

                    // Extract ArraySize attribute (default to 1 if not specified)
                    int arraySize = 1;
                    var arraySizeAttr = fieldElement.Attribute("ArraySize");
                    if (arraySizeAttr != null && int.TryParse(arraySizeAttr.Value, out int size))
                    {
                        arraySize = size;
                    }

                    // Create columns for this field (1 if no array, or N if array)
                    for (int i = 1; i <= arraySize; i++)
                    {
                        // Create display name with array index if ArraySize > 1
                        string displayName = arraySize > 1 ? $"{fieldName}_{i}" : fieldName;

                        var column = new DataGridTextColumn
                        {
                            Header = displayName,
                            Binding = new System.Windows.Data.Binding($"[Field{fieldIndex}]")
                        };
                        dataGridView.Columns.Add(column);
                        fieldIndex++;
                    }
                }

                // Now bind the data - columns are already set up
                dataGridView.ItemsSource = dbc_data.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading definitions: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                dbc_editor?.errors_text_box.AppendText($"Error loading definitions for {clientVersion}: {ex.Message}\n");
                dataGridView.ItemsSource = dbc_data.DefaultView;
            }
        }

        private void ApplyFilterToTable(TabItem tabPage, DataGrid dataGridView, string filter, string dataColumnName = "")
        {
            if (tabPage == null || dataGridView == null || string.IsNullOrEmpty(filter))
                return;

            try
            {
                DataView? view = GetBackingView(dataGridView);
                if (view?.Table == null)
                {
                    MessageBox.Show("Unable to access the underlying data source.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Replace the display column name with the actual DataTable column name in the filter
                string actualFilter = filter;
                if (!string.IsNullOrEmpty(dataColumnName) && !filter.Contains(dataColumnName))
                {
                    // Find the first word (column name) in the filter and replace it with dataColumnName
                    // This handles cases like "TabID LIKE '%value%'" -> "Field0 LIKE '%value%'"
                    foreach (DataGridColumn col in dataGridView.Columns)
                    {
                        string? header = col.Header?.ToString();
                        string? boundName = (col as DataGridTextColumn)?.Binding is System.Windows.Data.Binding b ? b.Path.Path.Trim('[', ']') : null;
                        if (header == dataColumnName && boundName != null && boundName != header)
                        {
                            actualFilter = filter.Replace(dataColumnName, boundName);
                            break;
                        }
                    }
                }

                // Apply the filter directly on the DataView backing the grid - WPF's DataGrid
                // re-renders live off the same DataView, so no data-source swap/restore is needed
                // the way a WinForms BindingSource required.
                view.RowFilter = actualFilter;

                dbc_editor?.messages_text_box.AppendText($"Filter applied: {filter}\n");
                if (dbc_editor != null) dbc_editor.message_text.Text = $"Filter applied successfully. ({view.Count} row(s) match)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filter: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                dbc_editor?.errors_text_box.AppendText($"Error applying filter: {ex.Message}\n");
            }
        }

        public void ClearFilter()
        {
            try
            {
                // Get the currently active tab
                if (dbc_editor?.dbc_tab_control?.SelectedItem is not TabItem selectedTab)
                {
                    MessageBox.Show("No DBC file is currently loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Get the DataGrid from the active tab
                DataGrid? dataGridView = selectedTab.Content as DataGrid;
                DataView? view = dataGridView != null ? GetBackingView(dataGridView) : null;
                if (dataGridView == null || view == null)
                {
                    MessageBox.Show("DataGridView not found in the active tab.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!string.IsNullOrEmpty(view.RowFilter))
                {
                    view.RowFilter = string.Empty;
                    dbc_editor?.messages_text_box.AppendText("Filter cleared. Showing all rows.\n");
                    if (dbc_editor != null) dbc_editor.message_text.Text = "Filter cleared.";
                }
                else
                {
                    MessageBox.Show("No filter is currently applied.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing filter: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                dbc_editor?.errors_text_box.AppendText($"Error clearing filter: {ex.Message}\n");
            }
        }

        public void CreateNewFilter()
        {
            // Get the currently active tab
            if (dbc_editor?.dbc_tab_control?.SelectedItem is not TabItem selectedTab)
            {
                MessageBox.Show("No DBC file is currently loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                dbc_editor?.errors_text_box.AppendText("Create Filter failed: No DBC file is currently loaded.\n");
                return;
            }

            // Get the DataGrid from the active tab
            DataGrid? dataGridView = selectedTab.Content as DataGrid;
            if (dataGridView == null)
            {
                MessageBox.Show("DataGridView not found in the active tab.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                dbc_editor?.errors_text_box.AppendText("Create Filter failed: DataGridView not found in the active tab.\n");
                return;
            }

            // Extract column names and their data property names from the DataGrid columns
            var columnMappings = new List<(string DisplayName, string DataPropertyName)>();
            foreach (DataGridColumn col in dataGridView.Columns)
            {
                string? header = col.Header?.ToString() ?? string.Empty;
                string dataPropertyName = (col as DataGridTextColumn)?.Binding is System.Windows.Data.Binding b ? b.Path.Path.Trim('[', ']') : header ?? string.Empty;
                columnMappings.Add((header ?? string.Empty, dataPropertyName));
            }

            if (columnMappings.Count == 0)
            {
                MessageBox.Show("No columns found in the current table.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                dbc_editor?.errors_text_box.AppendText("Create Filter failed: No columns found in the current table.\n");
                return;
            }

            // Pass the column mappings to FilterForm so it builds the filter using actual column names
            var filterForm = new FilterForm(columnMappings);
            if (filterForm.ShowDialog() == true)
            {
                ApplyFilterToTable(selectedTab, dataGridView, filterForm.FilterExpression);
            }
        }
    }

    public class VersionComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == y)
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            // Parse format: "ExpansionName Version (BuildNumber)"
            // Example: "Legion 7.2.0 (23835)"
            var xVersion = ExtractVersion(x);
            var yVersion = ExtractVersion(y);

            // Compare versions first
            int versionComparison = CompareVersionStrings(xVersion.version, yVersion.version);
            if (versionComparison != 0)
                return versionComparison;

            // If versions are equal, compare build numbers
            return xVersion.buildNumber.CompareTo(yVersion.buildNumber);
        }

        private (string version, int buildNumber) ExtractVersion(string input)
        {
            // Extract build number from parentheses at the end
            int buildNumber = 0;
            string version = input;

            int lastOpenParen = input.LastIndexOf('(');
            int lastCloseParen = input.LastIndexOf(')');
            if (lastOpenParen >= 0 && lastCloseParen > lastOpenParen)
            {
                string buildStr = input.Substring(lastOpenParen + 1, lastCloseParen - lastOpenParen - 1);
                int.TryParse(buildStr, out buildNumber);
                version = input.Substring(0, lastOpenParen).Trim();
            }

            // Extract version number (after the expansion name)
            // Find the first digit to locate where the version starts
            int versionStart = 0;
            for (int i = 0; i < version.Length; i++)
            {
                if (char.IsDigit(version[i]))
                {
                    versionStart = i;
                    break;
                }
            }

            version = version.Substring(versionStart).Trim();
            return (version, buildNumber);
        }

        private int CompareVersionStrings(string x, string y)
        {
            var xParts = x.Split('.');
            var yParts = y.Split('.');

            int minLength = Math.Min(xParts.Length, yParts.Length);
            for (int i = 0; i < minLength; i++)
            {
                if (int.TryParse(xParts[i], out int xNum) && int.TryParse(yParts[i], out int yNum))
                {
                    if (xNum != yNum)
                        return xNum.CompareTo(yNum);
                }
                else
                {
                    int stringComparison = xParts[i].CompareTo(yParts[i]);
                    if (stringComparison != 0)
                        return stringComparison;
                }
            }

            return xParts.Length.CompareTo(yParts.Length);
        }
    }

    public class FilterForm : Window
    {
        private ComboBox? columnComboBox;
        private ComboBox? operatorComboBox;
        private TextBox? valueTextBox;
        private List<(string DisplayName, string DataPropertyName)> columnMappings;
        private string[] displayNames;

        public string FilterExpression { get; private set; } = string.Empty;
        public string SelectedColumn { get; private set; } = string.Empty;

        public FilterForm(List<(string DisplayName, string DataPropertyName)> mappings)
        {
            columnMappings = mappings;
            displayNames = mappings.Select(m => m.DisplayName).ToArray();
            InitializeFormComponents();
        }

        private void InitializeFormComponents()
        {
            this.Title = "Create Filter";
            this.Width = 450;
            this.Height = 200;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ResizeMode = ResizeMode.NoResize;

            var grid = new Grid();

            // Column Label and ComboBox
            var columnLabel = new Label { Content = "Column:", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(10, 10, 0, 0), Width = 60 };

            columnComboBox = new ComboBox
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(80, 10, 0, 0),
                Width = 350,
                ItemsSource = displayNames
            };

            // Operator Label and ComboBox
            var operatorLabel = new Label { Content = "Operator:", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(10, 45, 0, 0), Width = 60 };

            operatorComboBox = new ComboBox
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(80, 45, 0, 0),
                Width = 350
            };

            operatorComboBox.Items.Add("Equals");
            operatorComboBox.Items.Add("Does Not Equal");
            operatorComboBox.Items.Add("Contains");
            operatorComboBox.Items.Add("Does Not Contain");
            operatorComboBox.Items.Add("Starts With");
            operatorComboBox.Items.Add("Ends With");
            operatorComboBox.Items.Add("Greater Than");
            operatorComboBox.Items.Add("Less Than");
            operatorComboBox.Items.Add("Greater Than Or Equal");
            operatorComboBox.Items.Add("Less Than Or Equal");
            operatorComboBox.Items.Add("Is Null");
            operatorComboBox.Items.Add("Is Not Null");

            operatorComboBox.SelectedIndex = 0;

            // Value Label and TextBox
            var valueLabel = new Label { Content = "Value:", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(10, 80, 0, 0), Width = 60 };

            valueTextBox = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(80, 80, 0, 0),
                Width = 350
            };

            // Buttons
            var okButton = new Button { Content = "OK", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(250, 120, 0, 0), Width = 80, Height = 30 };
            var cancelButton = new Button { Content = "Cancel", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(340, 120, 0, 0), Width = 80, Height = 30, IsCancel = true };

            okButton.Click += OkButton_Click;
            cancelButton.Click += (s, e) => { this.DialogResult = false; this.Close(); };

            grid.Children.Add(columnLabel);
            grid.Children.Add(columnComboBox);
            grid.Children.Add(operatorLabel);
            grid.Children.Add(operatorComboBox);
            grid.Children.Add(valueLabel);
            grid.Children.Add(valueTextBox);
            grid.Children.Add(okButton);
            grid.Children.Add(cancelButton);

            this.Content = grid;
        }

        private void OkButton_Click(object? sender, RoutedEventArgs e)
        {
            if (columnComboBox?.SelectedItem == null)
            {
                MessageBox.Show("Please select a column.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get the selected display name
            string selectedDisplayName = columnComboBox.SelectedItem.ToString() ?? string.Empty;
            SelectedColumn = selectedDisplayName;

            // Find the corresponding DataPropertyName (actual column name in DataTable)
            string actualColumnName = selectedDisplayName;
            int selectedIndex = Array.IndexOf(displayNames, selectedDisplayName);
            if (selectedIndex >= 0 && selectedIndex < columnMappings.Count)
            {
                actualColumnName = columnMappings[selectedIndex].DataPropertyName;
            }

            string operatorText = operatorComboBox?.SelectedItem?.ToString() ?? "Equals";
            string value = valueTextBox?.Text.Trim() ?? string.Empty;

            // Build filter expression using actual column name (Field0, Field1, etc.)
            FilterExpression = BuildFilterExpression(actualColumnName, operatorText, value);

            if (string.IsNullOrEmpty(FilterExpression))
            {
                MessageBox.Show("Unable to build filter expression.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.DialogResult = null;
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        private string BuildFilterExpression(string column, string operatorText, string value)
        {
            return operatorText switch
            {
                "Equals" => BuildEqualsExpression(column, value),
                "Does Not Equal" => $"NOT ({column} = {FormatValue(value)})",
                "Contains" => $"{column} LIKE '%{EscapeValue(value)}%'",
                "Does Not Contain" => $"NOT {column} LIKE '%{EscapeValue(value)}%'",
                "Starts With" => $"{column} LIKE '{EscapeValue(value)}%'",
                "Ends With" => $"{column} LIKE '%{EscapeValue(value)}'",
                "Greater Than" => $"{column} > {value}",
                "Less Than" => $"{column} < {value}",
                "Greater Than Or Equal" => $"{column} >= {value}",
                "Less Than Or Equal" => $"{column} <= {value}",
                "Is Null" => $"{column} IS NULL",
                "Is Not Null" => $"{column} IS NOT NULL",
                _ => string.Empty
            };
        }

        private string BuildEqualsExpression(string column, string value)
        {
            // Try to parse as number, if successful use numeric comparison
            if (int.TryParse(value, out int intValue))
            {
                return $"{column} = {intValue}";
            }
            else if (decimal.TryParse(value, out decimal decimalValue))
            {
                return $"{column} = {decimalValue}";
            }
            else
            {
                // String comparison
                return $"{column} = '{EscapeValue(value)}'";
            }
        }

        private string FormatValue(string value)
        {
            // Try to parse as number
            if (int.TryParse(value, out int intValue))
            {
                return intValue.ToString();
            }
            else if (decimal.TryParse(value, out decimal decimalValue))
            {
                return decimalValue.ToString();
            }
            else
            {
                // String value
                return $"'{EscapeValue(value)}'";
            }
        }

        private string EscapeValue(string value)
        {
            // Escape single quotes for SQL-like syntax
            return value.Replace("'", "''");
        }
    }
}
