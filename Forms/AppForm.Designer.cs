namespace AzureEditor
{
    partial class AppForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppForm));
            data_viewer = new System.Windows.Forms.DataGridView();
            update_timer = new System.Windows.Forms.Timer(components);
            menus = new System.Windows.Forms.TabControl();
            query_result = new System.Windows.Forms.TabPage();
            messages_log = new System.Windows.Forms.TabPage();
            messages_log_textbox = new System.Windows.Forms.RichTextBox();
            tools_ribbon = new System.Windows.Forms.ToolStrip();
            connections_menu = new System.Windows.Forms.ToolStripDropDownButton();
            add_connection_button = new System.Windows.Forms.ToolStripMenuItem();
            delete_connection_button = new System.Windows.Forms.ToolStripMenuItem();
            refresh_connections = new System.Windows.Forms.ToolStripButton();
            add_connection = new System.Windows.Forms.ToolStripButton();
            delete_connection = new System.Windows.Forms.ToolStripButton();
            run_query = new System.Windows.Forms.ToolStripButton();
            partial_query_run = new System.Windows.Forms.ToolStripButton();
            new_query_button = new System.Windows.Forms.ToolStripButton();
            export_query = new System.Windows.Forms.ToolStripButton();
            import_query_button = new System.Windows.Forms.ToolStripButton();
            split_view_vertical = new System.Windows.Forms.SplitContainer();
            connections_tree_view = new System.Windows.Forms.TreeView();
            query_tab_control = new System.Windows.Forms.TabControl();
            split_view_horizontal = new System.Windows.Forms.SplitContainer();
            status_ribbon = new System.Windows.Forms.StatusStrip();
            connection_process = new System.Windows.Forms.ToolStripProgressBar();
            fetch_status = new System.Windows.Forms.ToolStripStatusLabel();
            schema_table_column = new System.Windows.Forms.ToolStripStatusLabel();
            query_result_context_menu = new System.Windows.Forms.ContextMenuStrip(components);
            exportQueryResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            asCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            asSQLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            asXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            asJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            refreshQueryEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tree_view_context_menu = new System.Windows.Forms.ContextMenuStrip(components);
            refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            query_tab_control_context_menu = new System.Windows.Forms.ContextMenuStrip(components);
            renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)data_viewer).BeginInit();
            menus.SuspendLayout();
            query_result.SuspendLayout();
            messages_log.SuspendLayout();
            tools_ribbon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)split_view_vertical).BeginInit();
            split_view_vertical.Panel1.SuspendLayout();
            split_view_vertical.Panel2.SuspendLayout();
            split_view_vertical.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)split_view_horizontal).BeginInit();
            split_view_horizontal.Panel1.SuspendLayout();
            split_view_horizontal.Panel2.SuspendLayout();
            split_view_horizontal.SuspendLayout();
            status_ribbon.SuspendLayout();
            query_result_context_menu.SuspendLayout();
            tree_view_context_menu.SuspendLayout();
            query_tab_control_context_menu.SuspendLayout();
            SuspendLayout();
            // 
            // data_viewer
            // 
            data_viewer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            data_viewer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            data_viewer.Location = new System.Drawing.Point(0, 0);
            data_viewer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            data_viewer.Name = "data_viewer";
            data_viewer.ReadOnly = true;
            data_viewer.Size = new System.Drawing.Size(896, 177);
            data_viewer.TabIndex = 0;
            data_viewer.CellContentClick += dataGridView1_CellContentClick;
            data_viewer.MouseClick += Context_Menu_LeftMouseClick;
            // 
            // update_timer
            // 
            update_timer.Tick += update_timer_Tick;
            // 
            // menus
            // 
            menus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            menus.Controls.Add(query_result);
            menus.Controls.Add(messages_log);
            menus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            menus.HotTrack = true;
            menus.Location = new System.Drawing.Point(0, 0);
            menus.Margin = new System.Windows.Forms.Padding(0);
            menus.Multiline = true;
            menus.Name = "menus";
            menus.SelectedIndex = 0;
            menus.ShowToolTips = true;
            menus.Size = new System.Drawing.Size(905, 213);
            menus.TabIndex = 4;
            // 
            // query_result
            // 
            query_result.Controls.Add(data_viewer);
            query_result.Location = new System.Drawing.Point(4, 29);
            query_result.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            query_result.Name = "query_result";
            query_result.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            query_result.Size = new System.Drawing.Size(897, 180);
            query_result.TabIndex = 0;
            query_result.Text = "tabPage1";
            query_result.UseVisualStyleBackColor = true;
            // 
            // messages_log
            // 
            messages_log.Controls.Add(messages_log_textbox);
            messages_log.Location = new System.Drawing.Point(4, 29);
            messages_log.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            messages_log.Name = "messages_log";
            messages_log.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            messages_log.Size = new System.Drawing.Size(897, 180);
            messages_log.TabIndex = 1;
            messages_log.Text = "tabPage2";
            messages_log.UseVisualStyleBackColor = true;
            // 
            // messages_log_textbox
            // 
            messages_log_textbox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            messages_log_textbox.BackColor = System.Drawing.SystemColors.Info;
            messages_log_textbox.Location = new System.Drawing.Point(1, 0);
            messages_log_textbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            messages_log_textbox.Name = "messages_log_textbox";
            messages_log_textbox.ReadOnly = true;
            messages_log_textbox.Size = new System.Drawing.Size(893, 176);
            messages_log_textbox.TabIndex = 0;
            messages_log_textbox.Text = "Messages Log";
            messages_log_textbox.TextChanged += messages_log_textbox_TextChanged;
            // 
            // tools_ribbon
            // 
            tools_ribbon.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tools_ribbon.Dock = System.Windows.Forms.DockStyle.None;
            tools_ribbon.ImageScalingSize = new System.Drawing.Size(20, 20);
            tools_ribbon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { connections_menu, refresh_connections, add_connection, delete_connection, run_query, partial_query_run, new_query_button, export_query, import_query_button });
            tools_ribbon.Location = new System.Drawing.Point(10, 10);
            tools_ribbon.Name = "tools_ribbon";
            tools_ribbon.Size = new System.Drawing.Size(404, 25);
            tools_ribbon.TabIndex = 5;
            tools_ribbon.Text = "toolStrip1";
            // 
            // connections_menu
            // 
            connections_menu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { add_connection_button, delete_connection_button });
            connections_menu.ImageTransparentColor = System.Drawing.Color.Magenta;
            connections_menu.Name = "connections_menu";
            connections_menu.Size = new System.Drawing.Size(87, 22);
            connections_menu.Text = "Connections";
            // 
            // add_connection_button
            // 
            add_connection_button.Name = "add_connection_button";
            add_connection_button.Size = new System.Drawing.Size(172, 22);
            add_connection_button.Text = "Add Connection";
            add_connection_button.Click += add_connection_Click;
            // 
            // delete_connection_button
            // 
            delete_connection_button.Name = "delete_connection_button";
            delete_connection_button.Size = new System.Drawing.Size(172, 22);
            delete_connection_button.Text = "Delete Connection";
            delete_connection_button.Click += delete_connection_Click;
            // 
            // refresh_connections
            // 
            refresh_connections.ImageTransparentColor = System.Drawing.Color.Magenta;
            refresh_connections.Name = "refresh_connections";
            refresh_connections.Size = new System.Drawing.Size(115, 22);
            refresh_connections.Text = "Refresh Connection";
            refresh_connections.Click += refresh_connections_Click;
            // 
            // add_connection
            // 
            add_connection.Enabled = false;
            add_connection.ImageTransparentColor = System.Drawing.Color.Magenta;
            add_connection.Name = "add_connection";
            add_connection.Size = new System.Drawing.Size(98, 22);
            add_connection.Text = "Add Connection";
            add_connection.Visible = false;
            add_connection.Click += add_connection_Click;
            // 
            // delete_connection
            // 
            delete_connection.Enabled = false;
            delete_connection.ImageTransparentColor = System.Drawing.Color.Magenta;
            delete_connection.Name = "delete_connection";
            delete_connection.Size = new System.Drawing.Size(109, 22);
            delete_connection.Text = "Delete Connection";
            delete_connection.Visible = false;
            delete_connection.Click += delete_connection_Click;
            // 
            // run_query
            // 
            run_query.ImageTransparentColor = System.Drawing.Color.Magenta;
            run_query.Name = "run_query";
            run_query.Size = new System.Drawing.Size(67, 22);
            run_query.Text = "Run Query";
            run_query.Click += run_query_Click;
            // 
            // partial_query_run
            // 
            partial_query_run.ImageTransparentColor = System.Drawing.Color.Magenta;
            partial_query_run.Name = "partial_query_run";
            partial_query_run.Size = new System.Drawing.Size(103, 22);
            partial_query_run.Text = "Run Selected Text";
            partial_query_run.Click += partial_query_run_Click;
            // 
            // new_query_button
            // 
            new_query_button.ImageTransparentColor = System.Drawing.Color.Magenta;
            new_query_button.Name = "new_query_button";
            new_query_button.Size = new System.Drawing.Size(70, 19);
            new_query_button.Text = "New Query";
            new_query_button.Click += new_query_button_Click;
            // 
            // export_query
            // 
            export_query.ImageTransparentColor = System.Drawing.Color.Magenta;
            export_query.Name = "export_query";
            export_query.Size = new System.Drawing.Size(79, 19);
            export_query.Text = "Export Query";
            export_query.Click += export_query_Click;
            // 
            // import_query_button
            // 
            import_query_button.ImageTransparentColor = System.Drawing.Color.Magenta;
            import_query_button.Name = "import_query_button";
            import_query_button.Size = new System.Drawing.Size(82, 19);
            import_query_button.Text = "Import Query";
            import_query_button.Click += toolStripButton1_Click;
            // 
            // split_view_vertical
            // 
            split_view_vertical.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            split_view_vertical.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            split_view_vertical.Location = new System.Drawing.Point(0, 0);
            split_view_vertical.Margin = new System.Windows.Forms.Padding(0);
            split_view_vertical.Name = "split_view_vertical";
            // 
            // split_view_vertical.Panel1
            // 
            split_view_vertical.Panel1.Controls.Add(connections_tree_view);
            // 
            // split_view_vertical.Panel2
            // 
            split_view_vertical.Panel2.Controls.Add(query_tab_control);
            split_view_vertical.Size = new System.Drawing.Size(906, 213);
            split_view_vertical.SplitterDistance = 186;
            split_view_vertical.SplitterWidth = 5;
            split_view_vertical.TabIndex = 8;
            // 
            // connections_tree_view
            // 
            connections_tree_view.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            connections_tree_view.FullRowSelect = true;
            connections_tree_view.Location = new System.Drawing.Point(-2, -2);
            connections_tree_view.Margin = new System.Windows.Forms.Padding(0);
            connections_tree_view.Name = "connections_tree_view";
            connections_tree_view.Size = new System.Drawing.Size(184, 213);
            connections_tree_view.TabIndex = 6;
            connections_tree_view.AfterSelect += connections_tree_view_AfterSelect;
            connections_tree_view.MouseClick += Context_Menu_LeftMouseClick;
            // 
            // query_tab_control
            // 
            query_tab_control.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            query_tab_control.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            query_tab_control.Location = new System.Drawing.Point(0, 0);
            query_tab_control.Margin = new System.Windows.Forms.Padding(0);
            query_tab_control.Name = "query_tab_control";
            query_tab_control.SelectedIndex = 0;
            query_tab_control.Size = new System.Drawing.Size(709, 209);
            query_tab_control.TabIndex = 0;
            query_tab_control.DrawItem += query_tab_control_DrawItem;
            query_tab_control.SelectedIndexChanged += query_tab_control_SelectedIndexChanged;
            query_tab_control.MouseDown += query_tab_control_MouseDown;
            query_tab_control.MouseLeave += query_tab_control_MouseLeave;
            query_tab_control.MouseMove += query_tab_control_MouseMove;
            // 
            // split_view_horizontal
            // 
            split_view_horizontal.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            split_view_horizontal.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            split_view_horizontal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            split_view_horizontal.Location = new System.Drawing.Point(10, 45);
            split_view_horizontal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            split_view_horizontal.Name = "split_view_horizontal";
            split_view_horizontal.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // split_view_horizontal.Panel1
            // 
            split_view_horizontal.Panel1.Controls.Add(split_view_vertical);
            // 
            // split_view_horizontal.Panel2
            // 
            split_view_horizontal.Panel2.Controls.Add(menus);
            split_view_horizontal.Size = new System.Drawing.Size(909, 445);
            split_view_horizontal.SplitterDistance = 221;
            split_view_horizontal.SplitterWidth = 5;
            split_view_horizontal.TabIndex = 1;
            // 
            // status_ribbon
            // 
            status_ribbon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { connection_process, fetch_status, schema_table_column });
            status_ribbon.Location = new System.Drawing.Point(0, 497);
            status_ribbon.Name = "status_ribbon";
            status_ribbon.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            status_ribbon.Size = new System.Drawing.Size(933, 22);
            status_ribbon.TabIndex = 6;
            status_ribbon.Text = "statusStrip1";
            // 
            // connection_process
            // 
            connection_process.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            connection_process.Name = "connection_process";
            connection_process.Size = new System.Drawing.Size(117, 18);
            connection_process.Visible = false;
            connection_process.Click += toolStripProgressBar1_Click;
            // 
            // fetch_status
            // 
            fetch_status.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            fetch_status.Name = "fetch_status";
            fetch_status.Size = new System.Drawing.Size(0, 17);
            // 
            // schema_table_column
            // 
            schema_table_column.Name = "schema_table_column";
            schema_table_column.Size = new System.Drawing.Size(0, 17);
            schema_table_column.Click += toolStripStatusLabel1_Click;
            // 
            // query_result_context_menu
            // 
            query_result_context_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { exportQueryResultsToolStripMenuItem, refreshQueryEditorToolStripMenuItem });
            query_result_context_menu.Name = "query_result_context_menu";
            query_result_context_menu.Size = new System.Drawing.Size(192, 48);
            // 
            // exportQueryResultsToolStripMenuItem
            // 
            exportQueryResultsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { asCSVToolStripMenuItem, asSQLToolStripMenuItem, asXMLToolStripMenuItem, asJSONToolStripMenuItem });
            exportQueryResultsToolStripMenuItem.Name = "exportQueryResultsToolStripMenuItem";
            exportQueryResultsToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            exportQueryResultsToolStripMenuItem.Text = "Export Query Results...";
            // 
            // asCSVToolStripMenuItem
            // 
            asCSVToolStripMenuItem.Name = "asCSVToolStripMenuItem";
            asCSVToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            asCSVToolStripMenuItem.Text = "As CSV";
            asCSVToolStripMenuItem.Click += asCSVToolStripMenuItem_Click;
            // 
            // asSQLToolStripMenuItem
            // 
            asSQLToolStripMenuItem.Name = "asSQLToolStripMenuItem";
            asSQLToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            asSQLToolStripMenuItem.Text = "As SQL";
            asSQLToolStripMenuItem.Click += asSQLToolStripMenuItem_Click;
            // 
            // asXMLToolStripMenuItem
            // 
            asXMLToolStripMenuItem.Name = "asXMLToolStripMenuItem";
            asXMLToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            asXMLToolStripMenuItem.Text = "As XML";
            asXMLToolStripMenuItem.Click += asXMLToolStripMenuItem_Click;
            // 
            // asJSONToolStripMenuItem
            // 
            asJSONToolStripMenuItem.Name = "asJSONToolStripMenuItem";
            asJSONToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            asJSONToolStripMenuItem.Text = "As JSON";
            asJSONToolStripMenuItem.Click += asJSONToolStripMenuItem_Click;
            // 
            // refreshQueryEditorToolStripMenuItem
            // 
            refreshQueryEditorToolStripMenuItem.Name = "refreshQueryEditorToolStripMenuItem";
            refreshQueryEditorToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            refreshQueryEditorToolStripMenuItem.Text = "Refresh Query Editor";
            refreshQueryEditorToolStripMenuItem.Click += refreshQueryEditorToolStripMenuItem_Click;
            // 
            // tree_view_context_menu
            // 
            tree_view_context_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { refreshToolStripMenuItem });
            tree_view_context_menu.Name = "tree_view_context_menu";
            tree_view_context_menu.Size = new System.Drawing.Size(114, 26);
            // 
            // refreshToolStripMenuItem
            // 
            refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            refreshToolStripMenuItem.Text = "Refresh";
            refreshToolStripMenuItem.Click += refreshToolStripMenuItem_Click;
            // 
            // query_tab_control_context_menu
            // 
            query_tab_control_context_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { renameToolStripMenuItem });
            query_tab_control_context_menu.Name = "query_tab_control_context_menu";
            query_tab_control_context_menu.Size = new System.Drawing.Size(118, 26);
            // 
            // renameToolStripMenuItem
            // 
            renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            renameToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            renameToolStripMenuItem.Text = "Rename";
            // 
            // AppForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(933, 519);
            Controls.Add(split_view_horizontal);
            Controls.Add(tools_ribbon);
            Controls.Add(status_ribbon);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "AppForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Teron SQL Databse Editor";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            FormClosed += AppForm_FormClosed;
            Load += AppForm_Load;
            ((System.ComponentModel.ISupportInitialize)data_viewer).EndInit();
            menus.ResumeLayout(false);
            query_result.ResumeLayout(false);
            messages_log.ResumeLayout(false);
            tools_ribbon.ResumeLayout(false);
            tools_ribbon.PerformLayout();
            split_view_vertical.Panel1.ResumeLayout(false);
            split_view_vertical.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)split_view_vertical).EndInit();
            split_view_vertical.ResumeLayout(false);
            split_view_horizontal.Panel1.ResumeLayout(false);
            split_view_horizontal.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)split_view_horizontal).EndInit();
            split_view_horizontal.ResumeLayout(false);
            status_ribbon.ResumeLayout(false);
            status_ribbon.PerformLayout();
            query_result_context_menu.ResumeLayout(false);
            tree_view_context_menu.ResumeLayout(false);
            query_tab_control_context_menu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        public System.Windows.Forms.DataGridView data_viewer;
        public System.Windows.Forms.Timer update_timer;
        public System.Windows.Forms.TabControl menus;
        public System.Windows.Forms.TabPage query_result;
        public System.Windows.Forms.TabPage messages_log;
        public System.Windows.Forms.RichTextBox messages_log_textbox;
        public System.Windows.Forms.ToolStrip tools_ribbon;
        public System.Windows.Forms.ToolStripButton add_connection;
        public System.Windows.Forms.ToolStripButton delete_connection;
        public System.Windows.Forms.ToolStripButton run_query;
        public System.Windows.Forms.SplitContainer split_view_vertical;
        public System.Windows.Forms.SplitContainer split_view_horizontal;
        public System.Windows.Forms.TreeView connections_tree_view;
        public System.Windows.Forms.ToolStripButton refresh_connections;
        public System.Windows.Forms.ToolStripButton export_query;
        public System.Windows.Forms.StatusStrip status_ribbon;
        public System.Windows.Forms.ToolStripStatusLabel fetch_status;
        public System.Windows.Forms.ContextMenuStrip query_result_context_menu;
        public System.Windows.Forms.ToolStripMenuItem exportQueryResultsToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem asCSVToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem asSQLToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem asXMLToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem asJSONToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem refreshQueryEditorToolStripMenuItem;
        public System.Windows.Forms.ToolStripButton import_query_button;
        public System.Windows.Forms.ToolStripDropDownButton connections_menu;
        public System.Windows.Forms.ToolStripMenuItem add_connection_button;
        public System.Windows.Forms.ToolStripMenuItem delete_connection_button;
        public System.Windows.Forms.ToolStripButton partial_query_run;
        public System.Windows.Forms.ToolStripButton new_query_button;
        public System.Windows.Forms.TabControl query_tab_control;
        public System.Windows.Forms.ToolStripProgressBar connection_process;
        public System.Windows.Forms.ContextMenuStrip tree_view_context_menu;
        public System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        public System.Windows.Forms.ContextMenuStrip query_tab_control_context_menu;
        public System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel schema_table_column;
    }
}