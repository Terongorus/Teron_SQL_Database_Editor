using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Logic;
using static Logic.Globals;

namespace AzureEditor
{
    public partial class AppForm : Form
    {
        public AppForm()
        {
            InitializeComponent();
            Text = $"SQL Editor - {AppInfo.DisplayNameWithVersion}";
        }

        private async void AppForm_Load(object sender, EventArgs e)
        {
            HelperFunctions helper = new HelperFunctions();
            query_result.Text = "Query Result";
            messages_log.Text = "Messages Log";
            app_form = this;
            helper.LoginMessage(this);
            helper.SetTableDataSource(this, null);
            helper.SetUpdateTimer(this);
            helper.AddNewTab(this, query_tab_control, helper.NameQueryTabs(this), null);
            helper.NameQueryTabs(this);
            await helper.PopulateTreeViewFromDatabase(this);
            App.LoadConnectionDetails(this);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        internal void query_textbox_TextChanged(object? sender, EventArgs e)
        {
            HelperFunctions helper = new HelperFunctions();
            RichTextBox? current_textbox = helper.GetActiveQueryTextBox(this);
            // Ensure a tab is selected
            if (current_textbox != null)
            {
                Globals.custom_sql_query = current_textbox.Text;
            }
            else
            {
                return;
            }
        }

        private async void update_timer_Tick(object sender, EventArgs e)
        {
            // prevent re-entry
            if (Globals.is_loading)
                return;

            Globals.is_loading = true;

            try
            {
                HelperFunctions helper = new HelperFunctions();

                if (File.Exists(Globals.login_config))
                {
                    // force refresh if not loaded yet
                    if (connections_tree_view == null || connections_tree_view.Nodes.Count < 1)
                    {
                        await helper.PopulateTreeViewFromDatabase(this);
                        MessageBox.Show("Loaded all connections!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading connections: {ex.Message}");
            }
            finally
            {
                Globals.is_loading = false;
            }
        }

        private async void run_query_Click(object sender, EventArgs e)
        {
            DataRequest request = new DataRequest();
            await request.FetchItems(this, Globals.sql_technical_user_username, Globals.sql_technical_user_password, Globals.connectionString);
        }

        private void add_connection_Click(object sender, EventArgs e)
        {
            AppLogin new_connection = new AppLogin();
            new_connection.Show();
        }

        private void messages_log_textbox_TextChanged(object sender, EventArgs e)
        {

        }

        private async void connections_tree_view_AfterSelect(object sender, TreeViewEventArgs e)
        {
            HelperFunctions helper = new HelperFunctions();
            RichTextBox? current_textbox = helper.GetActiveQueryTextBox(this);
            if (current_textbox != null)
            {
                await helper.SetQuery(this, current_textbox);
                DataRequest.SelectItemsFromSelectedTable(this);
            }
            else
            {
                return;
            }
        }

        private async void refresh_connections_Click(object sender, EventArgs e)
        {
            App.LoadConnectionDetails(this);
            HelperFunctions helper = new HelperFunctions();
            await helper.PopulateTreeViewFromDatabase(this);
            //this.query_textbox.Clear();
        }

        private void export_query_Click(object sender, EventArgs e)
        {
            HelperFunctions helper = new HelperFunctions();
            RichTextBox? current_textbox = helper.GetActiveQueryTextBox(this);
            if (current_textbox != null)
            {
                helper.ExportQuery(this, current_textbox);
            }
        }

        private void delete_connection_Click(object sender, EventArgs e)
        {
            AppDelete new_deletion_window = new AppDelete();
            new_deletion_window.Show();
        }

        private void Context_Menu_LeftMouseClick(object sender, MouseEventArgs e)
        {
            HelperFunctions helper = new HelperFunctions();
            helper.MouseButtonDetect(sender, e, this);
        }

        private void asCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportQueryResults.ExportQueryResultsAsCSV(this);
        }

        private void asSQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportQueryResults.ExportQueryResultsAsSQL(this);
        }

        private void asXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportQueryResults.ExportQueryResultsAsXML(this);
        }

        private void asJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportQueryResults.ExportQueryResultsAsJSON(this);
        }

        private void refreshQueryEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.data_viewer.Refresh();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            HelperFunctions helper = new HelperFunctions();
            RichTextBox? current_textbox = helper.GetActiveQueryTextBox(this);
            if (current_textbox != null)
            {
                helper.ImportQuery(current_textbox);
            }
        }

        private void partial_query_run_Click(object sender, EventArgs e)
        {
            DataRequest request = new DataRequest();
            request.PartialFetchItems(this, Globals.sql_technical_user_username, Globals.sql_technical_user_password, Globals.connectionString);
        }

        private void toolStripProgressBar1_Click(object sender, EventArgs e)
        {

        }

        private void new_query_button_Click(object sender, EventArgs e)
        {
            HelperFunctions helper = new HelperFunctions();

            helper.AddNewTab(this, query_tab_control, helper.NameQueryTabs(this), null);
            helper.NameQueryTabs(this);

        }

        private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelperFunctions helper = new HelperFunctions();
            await helper.PopulateTreeViewFromDatabase(this);
        }

        private void query_tab_control_SelectedIndexChanged(object sender, EventArgs e)
        {
            HelperFunctions helper = new HelperFunctions();
            RichTextBox? current_textbox = helper.GetActiveQueryTextBox(this);
            if (current_textbox != null)
            {
                Globals.custom_sql_query = current_textbox.Text;
            }
            else
            {
                return;
            }
        }

        public void query_tab_control_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabControl = sender as TabControl;
            if (tabControl == null)
                return;

            using (var tabPage = tabControl.TabPages[e.Index])
            {
                var tabRect = tabControl.GetTabRect(e.Index);
                tabRect.Inflate(-2, -2);

                // Draw tab title
                e.Graphics.DrawString(
                    tabPage.Text,
                    this.Font,
                    Brushes.Black,
                    tabRect.X + 4,
                    tabRect.Y + 4
                );

                // Define "X" button area (20x20)
                var closeRect = new Rectangle(
                    tabRect.Right - 15,
                    tabRect.Top + (tabRect.Height - 20) / 2,
                    20,
                    20
                );

                // Change color if hovered
                Brush closeBrush = (e.Index == Globals.hoveredCloseButtonIndex) ? Brushes.Red : Brushes.DarkGray;

                using (Font closeFont = new Font(this.Font.FontFamily, this.Font.Size + 2, FontStyle.Bold))
                {
                    e.Graphics.DrawString("x", closeFont, closeBrush, closeRect);
                }
            }

            e.DrawFocusRectangle();
        }

        private void query_tab_control_MouseMove(object sender, MouseEventArgs e)
        {
            var tabControl = sender as TabControl;
            if (tabControl == null)
                return;

            bool hoverFound = false;

            for (int i = 0; i < tabControl.TabPages.Count; i++)
            {
                var tabRect = tabControl.GetTabRect(i);
                var closeRect = new Rectangle(
                    tabRect.Right - 24,
                    tabRect.Top + (tabRect.Height - 20) / 2,
                    20,
                    20
                );

                if (closeRect.Contains(e.Location))
                {
                    Globals.hoveredCloseButtonIndex = i;
                    hoverFound = true;
                    tabControl.Invalidate();
                    break;
                }
            }

            if (!hoverFound && Globals.hoveredCloseButtonIndex != -1)
            {
                Globals.hoveredCloseButtonIndex = -1;
                tabControl.Invalidate();
            }
        }

        private void query_tab_control_MouseLeave(object sender, EventArgs e)
        {
            Globals.hoveredCloseButtonIndex = -1;
            (sender as TabControl)?.Invalidate();
        }

        private void query_tab_control_MouseDown(object sender, MouseEventArgs e)
        {
            // Only respond to left-click
            if (e.Button != MouseButtons.Left)
                return;

            for (int i = 0; i < query_tab_control.TabPages.Count; i++)
            {
                var tabRect = query_tab_control.GetTabRect(i);
                int closeSize = 16;
                var closeRect = new Rectangle(
                    tabRect.Right - closeSize - 6,
                    tabRect.Y + (tabRect.Height - closeSize) / 2,
                    closeSize,
                    closeSize
                );

                if (closeRect.Contains(e.Location))
                {
                    // Safely remove the corresponding RichTextBox from Globals.box_list
                    if (i >= 0 && i < Globals.box_list.Count)
                        Globals.box_list.RemoveAt(i);

                    // Remove the tab page itself
                    query_tab_control.TabPages.RemoveAt(i);

                    break;
                }
            }
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void AppForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            startup.Show();
            startup.Focus();
        }
    }
}
