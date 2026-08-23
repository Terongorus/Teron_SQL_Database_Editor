using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using AzureEditor;
using Database;

namespace Logic
{
    // Lightweight replacement for WinForms' TreeNode.Level/.Text/.Parent chain, since WPF
    // TreeViewItem has no equivalent built-in hierarchy accessor. Attached via TreeViewItem.Tag.
    internal class SqlTreeNodeTag
    {
        public string Text = "";
        public int Level;
        public SqlTreeNodeTag? Parent;
    }

    internal class Globals
    {
        //window instances
        public static Window startup = new Startup.Startup(); //initialize a global startup object (Window) which will be a "main menu" for the other Windows, and will be responsible for opening the main AppForm
        public static AppForm? app_form;
        public static DBCEditor? dbc_editor;

        //configurations
        public static string definitons_dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Definitions");
        public static readonly string AppDataRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TeronSQLDatabaseEditor");
        public static string login_config = Path.Combine(AppDataRoot, "loginconfig.xml");
        public static string sql_technical_user_username = "";
        public static string sql_technical_user_password = "";
        public static string connectionString = "";
        public static string select_clause = "";
        public static string from_clause = "";

        //queries
        public static string custom_sql_query = "";
        public static string schemas_query = "SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA;";
        public static string tables_query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '%schema%';";
        public static string columns_query = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '%schema%' AND TABLE_NAME = '%table%';";

        //global objects
        public static string? schema = "";
        public static string? table = "";
        public static string? column = "";

        public static int hoveredCloseButtonIndex = -1;
        public static bool is_loading = false;

        public class LoginTable
        {
            public string? Nickname { get; set; }
            public required string Username { get; set; }
            public required string Password { get; set; }
            public required string ConnectionString { get; set; }
        }

        public static BindingList<LoginTable> login_list = new BindingList<LoginTable>();

        public static BindingList<Dictionary<string, object>> internal_items_list = new BindingList<Dictionary<string, object>>();

        //tables list
        public class TablesViewStructure
        {
            public string? Name { get; set; }
        }

        public static BindingList<TablesViewStructure> internal_tables_list = new BindingList<TablesViewStructure>();

        //schemas list
        public class SchemasViewStructure
        {
            public string? Name { get; set; }
        }

        public static BindingList<SchemasViewStructure> internal_schemas_list = new BindingList<SchemasViewStructure>();

        //columns list
        public class ColumnsViewStructure
        {
            public string? Name { get; set; }
        }

        public static BindingList<ColumnsViewStructure> internal_columns_list = new BindingList<ColumnsViewStructure>();

        public static List<TextBox> box_list = new List<TextBox>();

        public static BindingList<Dictionary<string, object>> dbc_viewer_struct = new BindingList<Dictionary<string, object>>();
    }
}
