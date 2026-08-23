namespace AzureEditor
{
    partial class AppDelete
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
            this.connection_deletion_selection = new System.Windows.Forms.ListBox();
            this.confirm_button = new System.Windows.Forms.Button();
            this.delete_selected = new System.Windows.Forms.Button();
            this.delete_all = new System.Windows.Forms.Button();
            this.select_all = new System.Windows.Forms.Button();
            this.deselect_all = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // connection_deletion_selection
            // 
            this.connection_deletion_selection.FormattingEnabled = true;
            this.connection_deletion_selection.Location = new System.Drawing.Point(12, 12);
            this.connection_deletion_selection.Name = "connection_deletion_selection";
            this.connection_deletion_selection.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.connection_deletion_selection.Size = new System.Drawing.Size(448, 160);
            this.connection_deletion_selection.TabIndex = 0;
            // 
            // confirm_button
            // 
            this.confirm_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.confirm_button.Location = new System.Drawing.Point(497, 219);
            this.confirm_button.Name = "confirm_button";
            this.confirm_button.Size = new System.Drawing.Size(75, 23);
            this.confirm_button.TabIndex = 1;
            this.confirm_button.Text = "OK";
            this.confirm_button.UseVisualStyleBackColor = true;
            this.confirm_button.Click += new System.EventHandler(this.confirm_button_Click);
            // 
            // delete_selected
            // 
            this.delete_selected.Location = new System.Drawing.Point(466, 120);
            this.delete_selected.Name = "delete_selected";
            this.delete_selected.Size = new System.Drawing.Size(106, 23);
            this.delete_selected.TabIndex = 2;
            this.delete_selected.Text = "Delete Selected";
            this.delete_selected.UseVisualStyleBackColor = true;
            this.delete_selected.Click += new System.EventHandler(this.delete_selected_Click);
            // 
            // delete_all
            // 
            this.delete_all.Location = new System.Drawing.Point(466, 149);
            this.delete_all.Name = "delete_all";
            this.delete_all.Size = new System.Drawing.Size(106, 23);
            this.delete_all.TabIndex = 3;
            this.delete_all.Text = "Delete All";
            this.delete_all.UseVisualStyleBackColor = true;
            this.delete_all.Click += new System.EventHandler(this.delete_all_Click);
            // 
            // select_all
            // 
            this.select_all.Location = new System.Drawing.Point(466, 12);
            this.select_all.Name = "select_all";
            this.select_all.Size = new System.Drawing.Size(106, 23);
            this.select_all.TabIndex = 4;
            this.select_all.Text = "Select All";
            this.select_all.UseVisualStyleBackColor = true;
            this.select_all.Click += new System.EventHandler(this.select_all_Click);
            // 
            // deselect_all
            // 
            this.deselect_all.Location = new System.Drawing.Point(466, 41);
            this.deselect_all.Name = "deselect_all";
            this.deselect_all.Size = new System.Drawing.Size(106, 23);
            this.deselect_all.TabIndex = 5;
            this.deselect_all.Text = "Deselect All";
            this.deselect_all.UseVisualStyleBackColor = true;
            this.deselect_all.Click += new System.EventHandler(this.deselect_all_Click);
            // 
            // AppDelete
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 254);
            this.Controls.Add(this.select_all);
            this.Controls.Add(this.deselect_all);
            this.Controls.Add(this.delete_selected);
            this.Controls.Add(this.delete_all);
            this.Controls.Add(this.confirm_button);
            this.Controls.Add(this.connection_deletion_selection);
            this.Name = "AppDelete";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Delete Connection";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ListBox connection_deletion_selection;
        public System.Windows.Forms.Button confirm_button;
        public System.Windows.Forms.Button delete_selected;
        public System.Windows.Forms.Button delete_all;
        public System.Windows.Forms.Button select_all;
        public System.Windows.Forms.Button deselect_all;
    }
}