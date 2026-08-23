using System;

namespace Database
{
    partial class DBCEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DBCEditor));
            top_tool_bar = new System.Windows.Forms.ToolStrip();
            open_file = new System.Windows.Forms.ToolStripButton();
            save_file = new System.Windows.Forms.ToolStripButton();
            save_file_as = new System.Windows.Forms.ToolStripButton();
            save_all = new System.Windows.Forms.ToolStripButton();
            filter_enable = new System.Windows.Forms.ToolStripButton();
            filter_disable = new System.Windows.Forms.ToolStripButton();
            dbc_tab_control = new System.Windows.Forms.TabControl();
            bottom_strip = new System.Windows.Forms.StatusStrip();
            message_text = new System.Windows.Forms.ToolStripStatusLabel();
            error_box_button = new System.Windows.Forms.ToolStripStatusLabel();
            warning_box_button = new System.Windows.Forms.ToolStripStatusLabel();
            message_box_button = new System.Windows.Forms.ToolStripStatusLabel();
            messages_control = new System.Windows.Forms.TabControl();
            errors_tab = new System.Windows.Forms.TabPage();
            errors_text_box = new System.Windows.Forms.RichTextBox();
            warnings_tab = new System.Windows.Forms.TabPage();
            warnings_text_box = new System.Windows.Forms.RichTextBox();
            messages_tab = new System.Windows.Forms.TabPage();
            messages_text_box = new System.Windows.Forms.RichTextBox();
            splitter1 = new System.Windows.Forms.Splitter();
            top_tool_bar.SuspendLayout();
            bottom_strip.SuspendLayout();
            messages_control.SuspendLayout();
            errors_tab.SuspendLayout();
            warnings_tab.SuspendLayout();
            messages_tab.SuspendLayout();
            SuspendLayout();
            // 
            // top_tool_bar
            // 
            top_tool_bar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            top_tool_bar.ImageScalingSize = new System.Drawing.Size(24, 24);
            top_tool_bar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { open_file, save_file, save_file_as, save_all, filter_enable, filter_disable });
            top_tool_bar.Location = new System.Drawing.Point(0, 0);
            top_tool_bar.Name = "top_tool_bar";
            top_tool_bar.Size = new System.Drawing.Size(800, 31);
            top_tool_bar.TabIndex = 0;
            top_tool_bar.Text = "top_tool_bar";
            // 
            // open_file
            // 
            open_file.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            open_file.Image = TeronSQLDatabaseEditor.Properties.Resources.folder_open;
            open_file.ImageTransparentColor = System.Drawing.Color.Magenta;
            open_file.Name = "open_file";
            open_file.Size = new System.Drawing.Size(28, 28);
            open_file.Text = "Open File";
            open_file.Click += toolStripButton1_Click;
            // 
            // save_file
            // 
            save_file.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            save_file.Image = TeronSQLDatabaseEditor.Properties.Resources.floppy_disk_circle_arrow_right;
            save_file.ImageTransparentColor = System.Drawing.Color.Magenta;
            save_file.Name = "save_file";
            save_file.Size = new System.Drawing.Size(28, 28);
            save_file.Text = "Save File";
            save_file.ToolTipText = "Save";
            save_file.Click += save_file_Click;
            // 
            // save_file_as
            // 
            save_file_as.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            save_file_as.Image = TeronSQLDatabaseEditor.Properties.Resources.floppy_disk_pen;
            save_file_as.ImageTransparentColor = System.Drawing.Color.Magenta;
            save_file_as.Name = "save_file_as";
            save_file_as.Size = new System.Drawing.Size(28, 28);
            save_file_as.Text = "Save As";
            save_file_as.Click += save_file_as_Click;
            // 
            // save_all
            // 
            save_all.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            save_all.Image = TeronSQLDatabaseEditor.Properties.Resources.floppy_disks;
            save_all.ImageTransparentColor = System.Drawing.Color.Magenta;
            save_all.Name = "save_all";
            save_all.Size = new System.Drawing.Size(28, 28);
            save_all.Text = "Save All";
            save_all.Click += save_all_Click;
            // 
            // filter_enable
            // 
            filter_enable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            filter_enable.Image = TeronSQLDatabaseEditor.Properties.Resources.filter;
            filter_enable.ImageTransparentColor = System.Drawing.Color.Magenta;
            filter_enable.Name = "filter_enable";
            filter_enable.Size = new System.Drawing.Size(28, 28);
            filter_enable.Text = "Create Filter";
            filter_enable.Click += filter_enable_Click;
            // 
            // filter_disable
            // 
            filter_disable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            filter_disable.Image = TeronSQLDatabaseEditor.Properties.Resources.filter_slash;
            filter_disable.ImageTransparentColor = System.Drawing.Color.Magenta;
            filter_disable.Name = "filter_disable";
            filter_disable.Size = new System.Drawing.Size(28, 28);
            filter_disable.Text = "Remove Filter";
            filter_disable.Click += filter_disable_Click;
            // 
            // dbc_tab_control
            // 
            dbc_tab_control.Dock = System.Windows.Forms.DockStyle.Fill;
            dbc_tab_control.Location = new System.Drawing.Point(0, 31);
            dbc_tab_control.Name = "dbc_tab_control";
            dbc_tab_control.SelectedIndex = 0;
            dbc_tab_control.Size = new System.Drawing.Size(800, 294);
            dbc_tab_control.TabIndex = 1;
            dbc_tab_control.MouseClick += dbc_tab_control_MouseClick;
            // 
            // bottom_strip
            // 
            bottom_strip.ImageScalingSize = new System.Drawing.Size(20, 20);
            bottom_strip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { message_text, error_box_button, warning_box_button, message_box_button });
            bottom_strip.Location = new System.Drawing.Point(0, 425);
            bottom_strip.Name = "bottom_strip";
            bottom_strip.Size = new System.Drawing.Size(800, 25);
            bottom_strip.TabIndex = 2;
            bottom_strip.Text = "statusStrip1";
            // 
            // message_text
            // 
            message_text.Image = TeronSQLDatabaseEditor.Properties.Resources.task_checklist;
            message_text.Name = "message_text";
            message_text.Size = new System.Drawing.Size(20, 20);
            // 
            // error_box_button
            // 
            error_box_button.Image = TeronSQLDatabaseEditor.Properties.Resources.octagon_xmark;
            error_box_button.Margin = new System.Windows.Forms.Padding(50, 3, 5, 2);
            error_box_button.Name = "error_box_button";
            error_box_button.Size = new System.Drawing.Size(20, 20);
            error_box_button.Click += error_box_button_Click;
            // 
            // warning_box_button
            // 
            warning_box_button.Image = TeronSQLDatabaseEditor.Properties.Resources.triangle_warning;
            warning_box_button.Margin = new System.Windows.Forms.Padding(5, 3, 5, 2);
            warning_box_button.Name = "warning_box_button";
            warning_box_button.Size = new System.Drawing.Size(20, 20);
            warning_box_button.Click += warning_box_button_Click;
            // 
            // message_box_button
            // 
            message_box_button.Image = TeronSQLDatabaseEditor.Properties.Resources.info;
            message_box_button.Margin = new System.Windows.Forms.Padding(5, 3, 5, 2);
            message_box_button.Name = "message_box_button";
            message_box_button.Size = new System.Drawing.Size(20, 20);
            message_box_button.Click += message_box_button_Click;
            // 
            // messages_control
            // 
            messages_control.Controls.Add(errors_tab);
            messages_control.Controls.Add(warnings_tab);
            messages_control.Controls.Add(messages_tab);
            messages_control.Dock = System.Windows.Forms.DockStyle.Bottom;
            messages_control.Location = new System.Drawing.Point(0, 325);
            messages_control.Name = "messages_control";
            messages_control.SelectedIndex = 0;
            messages_control.Size = new System.Drawing.Size(800, 100);
            messages_control.TabIndex = 3;
            // 
            // errors_tab
            // 
            errors_tab.Controls.Add(errors_text_box);
            errors_tab.Location = new System.Drawing.Point(4, 24);
            errors_tab.Name = "errors_tab";
            errors_tab.Padding = new System.Windows.Forms.Padding(3);
            errors_tab.Size = new System.Drawing.Size(792, 72);
            errors_tab.TabIndex = 0;
            errors_tab.Text = "Errors";
            errors_tab.UseVisualStyleBackColor = true;
            // 
            // errors_text_box
            // 
            errors_text_box.BorderStyle = System.Windows.Forms.BorderStyle.None;
            errors_text_box.Dock = System.Windows.Forms.DockStyle.Fill;
            errors_text_box.Location = new System.Drawing.Point(3, 3);
            errors_text_box.Name = "errors_text_box";
            errors_text_box.ReadOnly = true;
            errors_text_box.Size = new System.Drawing.Size(786, 66);
            errors_text_box.TabIndex = 0;
            errors_text_box.Text = "";
            errors_text_box.WordWrap = false;
            // 
            // warnings_tab
            // 
            warnings_tab.Controls.Add(warnings_text_box);
            warnings_tab.Location = new System.Drawing.Point(4, 24);
            warnings_tab.Name = "warnings_tab";
            warnings_tab.Padding = new System.Windows.Forms.Padding(3);
            warnings_tab.Size = new System.Drawing.Size(792, 72);
            warnings_tab.TabIndex = 1;
            warnings_tab.Text = "Warnings";
            warnings_tab.UseVisualStyleBackColor = true;
            // 
            // warnings_text_box
            // 
            warnings_text_box.BorderStyle = System.Windows.Forms.BorderStyle.None;
            warnings_text_box.Dock = System.Windows.Forms.DockStyle.Fill;
            warnings_text_box.Location = new System.Drawing.Point(3, 3);
            warnings_text_box.Name = "warnings_text_box";
            warnings_text_box.ReadOnly = true;
            warnings_text_box.Size = new System.Drawing.Size(786, 66);
            warnings_text_box.TabIndex = 0;
            warnings_text_box.Text = "";
            warnings_text_box.WordWrap = false;
            // 
            // messages_tab
            // 
            messages_tab.Controls.Add(messages_text_box);
            messages_tab.Location = new System.Drawing.Point(4, 24);
            messages_tab.Name = "messages_tab";
            messages_tab.Padding = new System.Windows.Forms.Padding(3);
            messages_tab.Size = new System.Drawing.Size(792, 72);
            messages_tab.TabIndex = 2;
            messages_tab.Text = "Messages";
            messages_tab.UseVisualStyleBackColor = true;
            // 
            // messages_text_box
            // 
            messages_text_box.BorderStyle = System.Windows.Forms.BorderStyle.None;
            messages_text_box.Dock = System.Windows.Forms.DockStyle.Fill;
            messages_text_box.Location = new System.Drawing.Point(3, 3);
            messages_text_box.Name = "messages_text_box";
            messages_text_box.ReadOnly = true;
            messages_text_box.Size = new System.Drawing.Size(786, 66);
            messages_text_box.TabIndex = 0;
            messages_text_box.Text = "";
            messages_text_box.WordWrap = false;
            // 
            // splitter1
            // 
            splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            splitter1.Location = new System.Drawing.Point(0, 322);
            splitter1.Name = "splitter1";
            splitter1.Size = new System.Drawing.Size(800, 3);
            splitter1.TabIndex = 4;
            splitter1.TabStop = false;
            // 
            // DBCEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(splitter1);
            Controls.Add(dbc_tab_control);
            Controls.Add(messages_control);
            Controls.Add(bottom_strip);
            Controls.Add(top_tool_bar);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "DBCEditor";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "DBC Editor";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            FormClosed += DBCEditor_FormClosed;
            top_tool_bar.ResumeLayout(false);
            top_tool_bar.PerformLayout();
            bottom_strip.ResumeLayout(false);
            bottom_strip.PerformLayout();
            messages_control.ResumeLayout(false);
            errors_tab.ResumeLayout(false);
            warnings_tab.ResumeLayout(false);
            messages_tab.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip top_tool_bar;
        private System.Windows.Forms.ToolStripButton open_file;
        public System.Windows.Forms.TabControl dbc_tab_control;
        private System.Windows.Forms.ToolStripButton filter_enable;
        private System.Windows.Forms.ToolStripButton filter_disable;
        private System.Windows.Forms.ToolStripButton save_file;
        private System.Windows.Forms.ToolStripButton save_file_as;
        private System.Windows.Forms.ToolStripButton save_all;
        private System.Windows.Forms.StatusStrip bottom_strip;
        public System.Windows.Forms.ToolStripStatusLabel message_text;
        private System.Windows.Forms.ToolStripStatusLabel error_box_button;
        public System.Windows.Forms.TabControl messages;
        private System.Windows.Forms.TabControl messages_control;
        private System.Windows.Forms.TabPage errors_tab;
        private System.Windows.Forms.TabPage warnings_tab;
        private System.Windows.Forms.TabPage messages_tab;
        private System.Windows.Forms.ToolStripStatusLabel warning_box_button;
        private System.Windows.Forms.ToolStripStatusLabel message_box_button;
        private System.Windows.Forms.Splitter splitter1;
        public System.Windows.Forms.RichTextBox errors_text_box;
        public System.Windows.Forms.RichTextBox warnings_text_box;
        public System.Windows.Forms.RichTextBox messages_text_box;
    }
}