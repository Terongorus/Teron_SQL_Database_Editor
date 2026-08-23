using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using Logic;
using static Logic.Globals;

namespace Database
{
    public partial class DBCEditor : Form
    {
        DBCLogic DBCLogic = new DBCLogic();
        public DBCEditor()
        {
            InitializeComponent();
            Text = $"DBC Editor - {AppInfo.DisplayNameWithVersion}";
        }

        private void toolStripButton1_Click(object sender, System.EventArgs e)
        {
            DBCLogic.SelectFile();
        }

        private void DBCEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            startup.Show();
            startup.Focus();
        }

        private void save_file_Click(object sender, EventArgs e)
        {
            DBCLogic.SaveFile();
        }

        private void save_file_as_Click(object sender, EventArgs e)
        {
            DBCLogic.SaveFileAs();
        }

        private void save_all_Click(object sender, EventArgs e)
        {
            DBCLogic.SaveAll();
        }

        private void error_box_button_Click(object sender, EventArgs e)
        {
            bool error_frame_visible = messages_control.SelectedTab == errors_tab && messages_control.Visible;
            if (!error_frame_visible) { messages_control.SelectedTab = errors_tab; messages_control.Visible = true; }
            else messages_control.Visible = false;
        }

        private void warning_box_button_Click(object sender, EventArgs e)
        {
            bool warning_frame_visible = messages_control.SelectedTab == warnings_tab && messages_control.Visible;
            if (!warning_frame_visible) { messages_control.SelectedTab = warnings_tab; messages_control.Visible = true; }
            else messages_control.Visible = false;
        }

        private void message_box_button_Click(object sender, EventArgs e)
        {
            bool message_frame_visible = messages_control.SelectedTab == messages_tab && messages_control.Visible;
            if (!message_frame_visible) { messages_control.SelectedTab = messages_tab; messages_control.Visible = true; }
            else messages_control.Visible = false;
        }

        private void dbc_tab_control_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && sender is TabControl tabControl)
            {
                // Find which tab was right-clicked
                int clickedTabIndex = -1;
                for (int i = 0; i < tabControl.TabPages.Count; i++)
                {
                    if (tabControl.GetTabRect(i).Contains(e.Location))
                    {
                        clickedTabIndex = i;
                        break;
                    }
                }

                if (clickedTabIndex >= 0)
                {
                    // Select the clicked tab
                    tabControl.SelectedIndex = clickedTabIndex;

                    ContextMenuStrip menu = new ContextMenuStrip();

                    menu.Items.Add("Close Current Tab").Click += (s, ev) =>
                    {
                        if (tabControl.SelectedTab != null)
                        {
                            tabControl.TabPages.Remove(tabControl.SelectedTab);
                        }
                    };

                    menu.Items.Add("Close All Tabs").Click += (s, ev) =>
                    {
                        tabControl.TabPages.Clear();
                    };

                    menu.Show(tabControl, e.Location);
                }
            }
        }

        private void filter_enable_Click(object sender, EventArgs e)
        {
            DBCLogic.CreateNewFilter();
        }

        private void filter_disable_Click(object sender, EventArgs e)
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
        public DialogResult SelectFile()
        {
            dbc_viewer_struct.Clear();

            var ofd = new OpenFileDialog();
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
                            MessageBox.Show("Invalid DBC file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    var new_tab = new TabPage(Path.GetFileName(ofd.FileName));
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
                    MessageBox.Show($"Error loading DBC file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public void SaveFile()
        {
            try
            {
                // Get the currently active tab
                if (dbc_editor?.dbc_tab_control?.SelectedTab == null)
                {
                    MessageBox.Show("No DBC file is currently loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dbc_editor?.errors_text_box.AppendText("Save failed: No DBC file is currently loaded.\n");
                    return;
                }

                // Get the DataGridView from the active tab
                DataGridView? dataGridView = dbc_editor.dbc_tab_control.SelectedTab.Controls[0] as DataGridView;
                if (dataGridView == null)
                {
                    MessageBox.Show("DataGridView not found in the active tab.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dbc_editor?.errors_text_box.AppendText("Save failed: DataGridView not found in the active tab.\n");
                    return;
                }

                // Verify we have data to save
                if (dataGridView.Rows.Count == 0)
                {
                    MessageBox.Show("No data to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dbc_editor?.errors_text_box.AppendText("Save failed: No data to save.\n");
                    return;
                }

                try
                {
                    string? tag = dbc_editor.dbc_tab_control.SelectedTab.Tag?.ToString();
                    // Get the original file path from the tab's Tag property
                    string filePath = tag ?? string.Empty;
                    if (string.IsNullOrEmpty(filePath))
                    {
                        MessageBox.Show("File path not found. Please use 'Save As' instead.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dbc_editor?.errors_text_box.AppendText("Save failed: File path not found. Please use 'Save As' instead.\n");
                        return;
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    using (var writer = new BinaryWriter(fileStream))
                    {
                        // Write WDBC header
                        writer.Write(Encoding.ASCII.GetBytes("WDBC"));

                        // Prepare record data
                        int recordCount = dataGridView.Rows.Count;
                        int fieldCount = dataGridView.Columns.Count;
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
                            DataGridViewRow row = dataGridView.Rows[i];
                            for (int j = 0; j < fieldCount; j++)
                            {
                                object? cellValue = row.Cells[j].Value;
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

                        dbc_editor?.messages_text_box.AppendText($"File saved successfully: {filePath}\n");
                        dbc_editor?.message_text.Text = $"File saved successfully: {filePath}";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dbc_editor?.errors_text_box.AppendText($"Error saving file: {ex.Message}\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dbc_editor?.errors_text_box.AppendText($"Error: {ex.Message}\n");
            }
        }

        public DialogResult SaveFileAs()
        {
            try
            {
                // Get the currently active tab
                if (dbc_editor?.dbc_tab_control?.SelectedTab == null)
                {
                    MessageBox.Show("No DBC file is currently loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dbc_editor?.errors_text_box.AppendText("Save As failed: No DBC file is currently loaded.\n");
                    return DialogResult.Cancel;
                }

                // Get the DataGridView from the active tab
                DataGridView? dataGridView = dbc_editor.dbc_tab_control.SelectedTab.Controls[0] as DataGridView;
                if (dataGridView == null)
                {
                    MessageBox.Show("DataGridView not found in the active tab.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dbc_editor?.errors_text_box.AppendText("Save As failed: DataGridView not found in the active tab.\n");
                    return DialogResult.Cancel;
                }

                // Verify we have data to save
                if (dataGridView.Rows.Count == 0)
                {
                    MessageBox.Show("No data to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dbc_editor?.errors_text_box.AppendText("Save As failed: No data to save.\n");
                    return DialogResult.Cancel;
                }

                var svf = new SaveFileDialog();
                svf.Filter = "DBC files (*.dbc)|*.dbc|All files (*.*)|*.*";
                svf.FileName = dbc_editor.dbc_tab_control.SelectedTab.Text; // Default to current tab name
                svf.Title = "Save DBC file";
                svf.FileOk += (sender, e) =>
                {
                    try
                    {
                        string filePath = svf.FileName;

                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        using (var writer = new BinaryWriter(fileStream))
                        {
                            // Write WDBC header
                            writer.Write(Encoding.ASCII.GetBytes("WDBC"));

                            // Prepare record data
                            int recordCount = dataGridView.Rows.Count;
                            int fieldCount = dataGridView.Columns.Count;
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
                                DataGridViewRow row = dataGridView.Rows[i];
                                for (int j = 0; j < fieldCount; j++)
                                {
                                    object? cellValue = row.Cells[j].Value;
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
                            dbc_editor?.messages_text_box.AppendText($"File saved successfully: {filePath}\n");
                            dbc_editor?.message_text.Text = $"File saved successfully: {filePath}";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        dbc_editor?.errors_text_box.AppendText($"Error saving file: {ex.Message}\n");
                    }
                };

                return svf.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing save dialog: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dbc_editor?.errors_text_box.AppendText($"Error initializing save dialog: {ex.Message}\n");
                return DialogResult.Cancel;
            }
        }

        public void SaveAll()
        {
            try
            {
                // Check if there are any tabs open
                if (dbc_editor?.dbc_tab_control?.TabPages.Count == 0)
                {
                    MessageBox.Show("No DBC files are open.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dbc_editor?.errors_text_box.AppendText("No DBC files are open.\n");
                    return;
                }

                int savedCount = 0;
                int failedCount = 0;
                var failedFiles = new List<string>();
                var tab_pages = dbc_editor?.dbc_tab_control.TabPages.Cast<TabPage>().ToList();

                // Iterate through all tabs
                foreach (TabPage tab in tab_pages ?? Enumerable.Empty<TabPage>())
                {
                    try
                    {
                        // Get the DataGridView from the tab
                        DataGridView? dataGridView = tab.Controls[0] as DataGridView;
                        if (dataGridView == null)
                        {
                            failedCount++;
                            failedFiles.Add($"{tab.Text} (DataGridView not found)");
                            continue;
                        }

                        // Skip if no data
                        if (dataGridView.Rows.Count == 0)
                        {
                            failedCount++;
                            failedFiles.Add($"{tab.Text} (no data)");
                            continue;
                        }

                        // Get the file path from the tab's Tag property
                        string? filePath = tab.Tag?.ToString();
                        if (string.IsNullOrEmpty(filePath))
                        {
                            failedCount++;
                            failedFiles.Add($"{tab.Text} (file path not found)");
                            continue;
                        }

                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        using (var writer = new BinaryWriter(fileStream))
                        {
                            // Write WDBC header
                            writer.Write(Encoding.ASCII.GetBytes("WDBC"));

                            // Prepare record data
                            int recordCount = dataGridView.Rows.Count;
                            int fieldCount = dataGridView.Columns.Count;
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
                                DataGridViewRow row = dataGridView.Rows[i];
                                for (int j = 0; j < fieldCount; j++)
                                {
                                    object? cellValue = row.Cells[j].Value;
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

                            savedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        failedCount++;
                        failedFiles.Add($"{tab.Text} ({ex.Message})");
                    }
                }

                // Show summary
                var message = $"Saved {savedCount} file(s)";
                if (failedCount > 0)
                {
                    message += $", failed to save {failedCount} file(s):\n";
                    message += string.Join("\n", failedFiles);
                    dbc_editor?.messages_text_box.AppendText($"Save All completed with errors: {failedCount} file(s) failed to save.\n");
                    dbc_editor?.message_text.Text = $"Save All completed with errors: {failedCount} file(s) failed to save.";
                }
                else
                {
                    dbc_editor?.messages_text_box.AppendText($"Save All completed successfully: {savedCount} file(s) saved.\n");
                    dbc_editor?.message_text.Text = $"Save All completed successfully: {savedCount} file(s) saved.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in Save All: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dbc_editor?.errors_text_box.AppendText($"Error in Save All: {ex.Message}\n");
            }
        }

        private string? PromptForClientVersion()
        {
            try
            {
                if (!Directory.Exists(definitons_dir))
                {
                    MessageBox.Show("Definitions folder not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("No definition files found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dbc_editor?.errors_text_box.AppendText($"No definition files found in: {definitons_dir}\n");
                    return null;
                }

                // Let user select the client version
                var form = new Form
                {
                    Text = "Select Client Version",
                    Width = 400,
                    Height = 200,
                    StartPosition = FormStartPosition.CenterScreen,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false
                };

                var label = new Label { Text = "Select the client version for this DBC file:", Left = 10, Top = 10, Width = 370 };
                var comboBox = new ComboBox
                {
                    Left = 10,
                    Top = 35,
                    Width = 370,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    DataSource = xmlFiles,
                    DisplayMember = "Display",
                    ValueMember = "FullName"
                };

                var okButton = new Button { Text = "OK", Left = 210, Top = 100, Width = 80, Height = 30, DialogResult = DialogResult.OK };
                var cancelButton = new Button { Text = "Cancel", Left = 300, Top = 100, Width = 80, Height = 30, DialogResult = DialogResult.Cancel };

                form.Controls.Add(label);
                form.Controls.Add(comboBox);
                form.Controls.Add(okButton);
                form.Controls.Add(cancelButton);
                form.AcceptButton = okButton;
                form.CancelButton = cancelButton;

                if (form.ShowDialog() != DialogResult.OK)
                    return null;

                return comboBox.SelectedValue?.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting client version: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dbc_editor?.errors_text_box.AppendText($"Error selecting client version: {ex.Message}\n");
                return null;
            }
        }

        private void CreateNewDataGrid(TabControl parent, TabPage tabPage, DataTable dbc_data, DBCEditor editor, string dbcFileName, string clientVersion)
        {
            DataGridView dataGridView = new DataGridView();
            dataGridView.AllowUserToAddRows = true;
            dataGridView.AllowUserToDeleteRows = true;
            dataGridView.AutoResizeColumnHeadersHeight();
            dataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
            dataGridView.Parent = tabPage;
            dataGridView.Dock = DockStyle.Fill;

            // Handle scroll with keyboard modifiers
            // SHIFT + scroll = horizontal scroll
            // CTRL + scroll = vertical scroll
            dataGridView.MouseWheel += (s, e) =>
            {
                HandledMouseEventArgs? hme = e as HandledMouseEventArgs;
                if (hme != null)
                {
                    if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                    {
                        // SHIFT pressed: horizontal scroll
                        int scrollAmount = 50;
                        int newOffset = dataGridView.HorizontalScrollingOffset + (e.Delta > 0 ? -scrollAmount : scrollAmount);
                        dataGridView.HorizontalScrollingOffset = Math.Max(0, newOffset);
                        hme.Handled = true;
                    }
                    else if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        // CTRL pressed: vertical scroll
                        int scrollRows = 3;
                        int newRowIndex = dataGridView.FirstDisplayedScrollingRowIndex + (e.Delta > 0 ? -scrollRows : scrollRows);
                        dataGridView.FirstDisplayedScrollingRowIndex = Math.Max(0, newRowIndex);
                        hme.Handled = true;
                    }
                }
            };

            // Load and apply field definitions BEFORE binding data
            LoadDataDefinitions(dbcFileName, clientVersion, dataGridView, dbc_data);

            // If definitions failed to load, use default column names
            if (dataGridView.Columns.Count == 0)
            {
                dataGridView.DataSource = dbc_data;
            }

            tabPage.Controls.Add(dataGridView);
            parent.TabPages.Add(tabPage);
            parent.SelectedTab = tabPage;

            dbc_editor?.message_text.Text = $"Loaded {dbcFileName} with client version {clientVersion.Replace(".xml", string.Empty)}.";
        }

        private void LoadDataDefinitions(string dbcFileName, string clientVersion, DataGridView dataGridView, DataTable dbc_data)
        {
            try
            {
                string xmlFilePath = Path.Combine(definitons_dir, clientVersion);

                if (!File.Exists(xmlFilePath))
                {
                    MessageBox.Show($"Definition file not found: {clientVersion}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dbc_editor?.warnings_text_box.AppendText($"Definition file not found: {clientVersion}\n");
                    dataGridView.DataSource = dbc_data;
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
                    MessageBox.Show($"Table definition for '{tableName}' not found in {clientVersion}.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dbc_editor?.warnings_text_box.AppendText($"Table definition for '{tableName}' not found in {clientVersion}.\n");
                    dataGridView.DataSource = dbc_data;
                    return;
                }

                // Extract all Field elements from the XML table definition
                var fields = table.Descendants("Field").ToList();

                if (fields.Count == 0)
                {
                    MessageBox.Show($"No fields found for table '{tableName}'.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dbc_editor?.warnings_text_box.AppendText($"No fields found for table '{tableName}' in {clientVersion}.\n");
                    dataGridView.DataSource = dbc_data;
                    return;
                }

                // Create DataGridViewTextBoxColumns with proper headers
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

                    // Check IsIndex attribute for potential future use
                    bool isIndex = false;
                    var isIndexAttr = fieldElement.Attribute("IsIndex");
                    if (isIndexAttr != null && bool.TryParse(isIndexAttr.Value, out bool idx))
                    {
                        isIndex = idx;
                    }

                    // Create columns for this field (1 if no array, or N if array)
                    for (int i = 1; i <= arraySize; i++)
                    {
                        // Create display name with array index if ArraySize > 1
                        string displayName = arraySize > 1 ? $"{fieldName}_{i}" : fieldName;

                        var column = new DataGridViewTextBoxColumn
                        {
                            Name = displayName,
                            HeaderText = displayName,
                            DataPropertyName = $"Field{fieldIndex}",
                            Tag = isIndex ? "Index" : null
                        };
                        dataGridView.Columns.Add(column);
                        fieldIndex++;
                    }
                }

                // Now bind the data - columns are already set up
                dataGridView.DataSource = dbc_data;
                dataGridView.AutoResizeColumns();
                dataGridView.AutoResizeRows();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading definitions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dbc_editor?.errors_text_box.AppendText($"Error loading definitions for {clientVersion}: {ex.Message}\n");
                dataGridView.DataSource = dbc_data;
            }
        }

        private void ApplyFilterToTable(TabPage tabPage, DataGridView dataGridView, string filter, string dataColumnName = "")
        {
            if (tabPage == null || dataGridView == null || string.IsNullOrEmpty(filter))
                return;

            try
            {
                // Store the current column definitions before changing the data source
                var currentColumns = new List<(string Name, string HeaderText, Type DataType)>();
                foreach (DataGridViewColumn col in dataGridView.Columns)
                {
                    currentColumns.Add((col.Name, col.HeaderText, col.ValueType ?? typeof(object)));
                }

                // Get the underlying DataTable (handle both DataTable and BindingSource sources)
                DataTable? sourceTable = null;
                if (dataGridView.DataSource is DataTable dt)
                {
                    sourceTable = dt;
                }
                else if (dataGridView.DataSource is BindingSource bs && bs.DataSource is DataTable bsdt)
                {
                    sourceTable = bsdt;
                }

                if (sourceTable == null)
                {
                    MessageBox.Show("Unable to access the underlying data source.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Replace the display column name with the actual DataTable column name in the filter
                string actualFilter = filter;
                if (!string.IsNullOrEmpty(dataColumnName) && !filter.Contains(dataColumnName))
                {
                    // Find the first word (column name) in the filter and replace it with dataColumnName
                    // This handles cases like "TabID LIKE '%value%'" -> "Field0 LIKE '%value%'"
                    foreach (DataGridViewColumn col in dataGridView.Columns)
                    {
                        if (col.HeaderText == dataColumnName && col.DataPropertyName != col.HeaderText)
                        {
                            actualFilter = filter.Replace(dataColumnName, col.DataPropertyName);
                            break;
                        }
                    }
                }

                // Apply the filter to the DataGridView
                BindingSource bindingSource = new BindingSource();
                bindingSource.DataSource = sourceTable;
                bindingSource.Filter = actualFilter;

                // Save current columns before changing data source
                var savedColumns = currentColumns.ToList();

                dataGridView.DataSource = bindingSource;

                // Restore the column definitions
                if (savedColumns.Count > 0 && savedColumns.Count <= dataGridView.Columns.Count)
                {
                    for (int i = 0; i < savedColumns.Count; i++)
                    {
                        if (i < dataGridView.Columns.Count)
                        {
                            dataGridView.Columns[i].HeaderText = savedColumns[i].HeaderText;
                            dataGridView.Columns[i].Name = savedColumns[i].Name;
                        }
                    }
                }

                dbc_editor?.messages_text_box.AppendText($"Filter applied: {filter}\n");
                dbc_editor?.message_text.Text = $"Filter applied successfully. ({bindingSource.Count} row(s) match)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filter: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dbc_editor?.errors_text_box.AppendText($"Error applying filter: {ex.Message}\n");
            }
        }

        public void ClearFilter()
        {
            try
            {
                // Get the currently active tab
                if (dbc_editor?.dbc_tab_control?.SelectedTab == null)
                {
                    MessageBox.Show("No DBC file is currently loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get the DataGridView from the active tab
                DataGridView? dataGridView = dbc_editor.dbc_tab_control.SelectedTab.Controls[0] as DataGridView;
                if (dataGridView == null)
                {
                    MessageBox.Show("DataGridView not found in the active tab.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get the underlying data source before the filter was applied
                if (dataGridView.DataSource is BindingSource bindingSource)
                {
                    // Restore the original data source
                    dataGridView.DataSource = bindingSource.DataSource;
                    dbc_editor?.messages_text_box.AppendText("Filter cleared. Showing all rows.\n");
                    dbc_editor?.message_text.Text = "Filter cleared.";
                }
                else
                {
                    MessageBox.Show("No filter is currently applied.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing filter: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dbc_editor?.errors_text_box.AppendText($"Error clearing filter: {ex.Message}\n");
            }
        }

        public void CreateNewFilter()
        {
            // Get the currently active tab
            if (dbc_editor?.dbc_tab_control?.SelectedTab == null)
            {
                MessageBox.Show("No DBC file is currently loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dbc_editor?.errors_text_box.AppendText("Create Filter failed: No DBC file is currently loaded.\n");
                return;
            }

            // Get the DataGridView from the active tab
            DataGridView? dataGridView = dbc_editor.dbc_tab_control.SelectedTab.Controls[0] as DataGridView;
            if (dataGridView == null)
            {
                MessageBox.Show("DataGridView not found in the active tab.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dbc_editor?.errors_text_box.AppendText("Create Filter failed: DataGridView not found in the active tab.\n");
                return;
            }

            // Extract column names and their data property names from the DataGridView columns
            var columnMappings = new List<(string DisplayName, string DataPropertyName)>();
            foreach (DataGridViewColumn col in dataGridView.Columns)
            {
                string dataPropertyName = col.DataPropertyName;
                if (string.IsNullOrEmpty(dataPropertyName))
                {
                    dataPropertyName = col.Name;
                }
                columnMappings.Add((col.HeaderText, dataPropertyName));
            }

            if (columnMappings.Count == 0)
            {
                MessageBox.Show("No columns found in the current table.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dbc_editor?.errors_text_box.AppendText("Create Filter failed: No columns found in the current table.\n");
                return;
            }

            // Pass the column mappings to FilterForm so it builds the filter using actual column names
            var filterForm = new FilterForm(columnMappings);
            if (filterForm.ShowDialog() == DialogResult.OK)
            {
                ApplyFilterToTable(dbc_editor.dbc_tab_control.SelectedTab, dataGridView, filterForm.FilterExpression);
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

    public class FilterForm : Form
    {
        private ComboBox? columnComboBox;
        private ComboBox? operatorComboBox;
        private TextBox? valueTextBox;
        private Button? okButton;
        private Button? cancelButton;
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
            this.Text = "Create Filter";
            this.Width = 450;
            this.Height = 200;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Column Label and ComboBox
            var columnLabel = new Label
            {
                Text = "Column:",
                Left = 10,
                Top = 10,
                Width = 60
            };

            columnComboBox = new ComboBox
            {
                Left = 80,
                Top = 10,
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource = displayNames
            };

            // Operator Label and ComboBox
            var operatorLabel = new Label
            {
                Text = "Operator:",
                Left = 10,
                Top = 45,
                Width = 60
            };

            operatorComboBox = new ComboBox
            {
                Left = 80,
                Top = 45,
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            operatorComboBox.Items.AddRange(new object[]
            {
                "Equals",
                "Does Not Equal",
                "Contains",
                "Does Not Contain",
                "Starts With",
                "Ends With",
                "Greater Than",
                "Less Than",
                "Greater Than Or Equal",
                "Less Than Or Equal",
                "Is Null",
                "Is Not Null"
            });

            operatorComboBox.SelectedIndex = 0;

            // Value Label and TextBox
            var valueLabel = new Label
            {
                Text = "Value:",
                Left = 10,
                Top = 80,
                Width = 60
            };

            valueTextBox = new TextBox
            {
                Left = 80,
                Top = 80,
                Width = 350
            };

            // Buttons
            okButton = new Button
            {
                Text = "OK",
                Left = 250,
                Top = 120,
                Width = 80,
                Height = 30,
                DialogResult = DialogResult.OK
            };

            cancelButton = new Button
            {
                Text = "Cancel",
                Left = 340,
                Top = 120,
                Width = 80,
                Height = 30,
                DialogResult = DialogResult.Cancel
            };

            okButton.Click += OkButton_Click;

            // Add controls to form
            this.Controls.Add(columnLabel);
            this.Controls.Add(columnComboBox);
            this.Controls.Add(operatorLabel);
            this.Controls.Add(operatorComboBox);
            this.Controls.Add(valueLabel);
            this.Controls.Add(valueTextBox);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            if (columnComboBox?.SelectedItem == null)
            {
                MessageBox.Show("Please select a column.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("Unable to build filter expression.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            this.DialogResult = DialogResult.OK;
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
