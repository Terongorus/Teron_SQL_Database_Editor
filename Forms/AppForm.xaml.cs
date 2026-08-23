using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Logic;
using static Logic.Globals;

namespace AzureEditor
{
    public partial class AppForm : Window
    {
        public readonly DispatcherTimer update_timer = new DispatcherTimer();

        public AppForm()
        {
            InitializeComponent();
            Title = $"SQL Editor - {AppInfo.DisplayNameWithVersion}";
            update_timer.Tick += update_timer_Tick;

            BuildContextMenus();
        }

        // Replaces the original's WinForms ContextMenuStrip.Show(control, location) call sites
        // (MouseButtonDetect in HelperFunctions.cs, dropped) with WPF's native ContextMenu
        // property, attached directly to the controls that actually used them. The third
        // original context menu (query_tab_control's right-click "Rename") was never reachable
        // in the WinForms version either - its MouseButtonDetect branch compared the mouse
        // event's sender to `query_tab_control.TabPages`, a collection that is never the actual
        // sender of a MouseClick event - so it's not carried over.
        private void BuildContextMenus()
        {
            var exportMenu = new MenuItem { Header = "Export Query Results..." };
            var asCsv = new MenuItem { Header = "As CSV" };
            asCsv.Click += asCSVToolStripMenuItem_Click;
            var asSql = new MenuItem { Header = "As SQL" };
            asSql.Click += asSQLToolStripMenuItem_Click;
            var asXml = new MenuItem { Header = "As XML" };
            asXml.Click += asXMLToolStripMenuItem_Click;
            var asJson = new MenuItem { Header = "As JSON" };
            asJson.Click += asJSONToolStripMenuItem_Click;
            exportMenu.Items.Add(asCsv);
            exportMenu.Items.Add(asSql);
            exportMenu.Items.Add(asXml);
            exportMenu.Items.Add(asJson);

            var refreshQueryEditor = new MenuItem { Header = "Refresh Query Editor" };
            refreshQueryEditor.Click += refreshQueryEditorToolStripMenuItem_Click;

            var resultContextMenu = new ContextMenu();
            resultContextMenu.Items.Add(exportMenu);
            resultContextMenu.Items.Add(refreshQueryEditor);
            data_viewer.ContextMenu = resultContextMenu;

            var refreshTree = new MenuItem { Header = "Refresh" };
            refreshTree.Click += refreshToolStripMenuItem_Click;
            var treeContextMenu = new ContextMenu();
            treeContextMenu.Items.Add(refreshTree);
            connections_tree_view.ContextMenu = treeContextMenu;
        }

        private async void AppForm_Load(object sender, RoutedEventArgs e)
        {
            HelperFunctions helper = new HelperFunctions();
            app_form = this;
            helper.LoginMessage(this);
            helper.SetTableDataSource(this, null);
            helper.SetUpdateTimer(this);
            helper.AddNewTab(this, query_tab_control, helper.NameQueryTabs(this), null);
            helper.NameQueryTabs(this);
            await helper.PopulateTreeViewFromDatabase(this);
            App.LoadConnectionDetails(this);
        }

        internal void query_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            HelperFunctions helper = new HelperFunctions();
            TextBox? current_textbox = helper.GetActiveQueryTextBox(this);
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

        private async void update_timer_Tick(object? sender, EventArgs e)
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
                    if (connections_tree_view == null || connections_tree_view.Items.Count < 1)
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

        private async void run_query_Click(object sender, RoutedEventArgs e)
        {
            DataRequest request = new DataRequest();
            await request.FetchItems(this, Globals.sql_technical_user_username, Globals.sql_technical_user_password, Globals.connectionString);
        }

        private void add_connection_Click(object sender, RoutedEventArgs e)
        {
            AppLogin new_connection = new AppLogin();
            new_connection.Show();
        }

        private async void connections_tree_view_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            HelperFunctions helper = new HelperFunctions();
            TextBox? current_textbox = helper.GetActiveQueryTextBox(this);
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

        private async void refresh_connections_Click(object sender, RoutedEventArgs e)
        {
            App.LoadConnectionDetails(this);
            HelperFunctions helper = new HelperFunctions();
            await helper.PopulateTreeViewFromDatabase(this);
        }

        private void export_query_Click(object sender, RoutedEventArgs e)
        {
            HelperFunctions helper = new HelperFunctions();
            TextBox? current_textbox = helper.GetActiveQueryTextBox(this);
            if (current_textbox != null)
            {
                helper.ExportQuery(this, current_textbox);
            }
        }

        private void delete_connection_Click(object sender, RoutedEventArgs e)
        {
            AppDelete new_deletion_window = new AppDelete();
            new_deletion_window.Show();
        }

        private void asCSVToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExportQueryResults.ExportQueryResultsAsCSV(this);
        }

        private void asSQLToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExportQueryResults.ExportQueryResultsAsSQL(this);
        }

        private void asXMLToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExportQueryResults.ExportQueryResultsAsXML(this);
        }

        private void asJSONToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExportQueryResults.ExportQueryResultsAsJSON(this);
        }

        private void refreshQueryEditorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.data_viewer.Items.Refresh();
        }

        private void toolStripButton1_Click(object sender, RoutedEventArgs e)
        {
            HelperFunctions helper = new HelperFunctions();
            TextBox? current_textbox = helper.GetActiveQueryTextBox(this);
            if (current_textbox != null)
            {
                helper.ImportQuery(current_textbox);
            }
        }

        private void partial_query_run_Click(object sender, RoutedEventArgs e)
        {
            DataRequest request = new DataRequest();
            request.PartialFetchItems(this, Globals.sql_technical_user_username, Globals.sql_technical_user_password, Globals.connectionString);
        }

        private void new_query_button_Click(object sender, RoutedEventArgs e)
        {
            HelperFunctions helper = new HelperFunctions();

            helper.AddNewTab(this, query_tab_control, helper.NameQueryTabs(this), null);
            helper.NameQueryTabs(this);
        }

        private async void refreshToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            HelperFunctions helper = new HelperFunctions();
            await helper.PopulateTreeViewFromDatabase(this);
        }

        private void query_tab_control_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            HelperFunctions helper = new HelperFunctions();
            TextBox? current_textbox = helper.GetActiveQueryTextBox(this);
            if (current_textbox != null)
            {
                Globals.custom_sql_query = current_textbox.Text;
            }
            else
            {
                return;
            }
        }

        private void AppForm_Closed(object? sender, EventArgs e)
        {
            startup.Show();
            startup.Activate();
        }
    }
}
