namespace AzureEditor
{
    partial class AppLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppLogin));
            this.username_input = new System.Windows.Forms.TextBox();
            this.password_input = new System.Windows.Forms.TextBox();
            this.connect_button = new System.Windows.Forms.Button();
            this.conn_string_input = new System.Windows.Forms.TextBox();
            this.username_label = new System.Windows.Forms.Label();
            this.password_label = new System.Windows.Forms.Label();
            this.conn_string_label = new System.Windows.Forms.Label();
            this.cancel_button = new System.Windows.Forms.Button();
            this.nickname_textbox = new System.Windows.Forms.TextBox();
            this.nickname_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // username_input
            // 
            this.username_input.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.username_input.Location = new System.Drawing.Point(130, 55);
            this.username_input.Name = "username_input";
            this.username_input.Size = new System.Drawing.Size(200, 20);
            this.username_input.TabIndex = 0;
            // 
            // password_input
            // 
            this.password_input.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.password_input.Location = new System.Drawing.Point(130, 81);
            this.password_input.Name = "password_input";
            this.password_input.Size = new System.Drawing.Size(200, 20);
            this.password_input.TabIndex = 1;
            this.password_input.UseSystemPasswordChar = true;
            // 
            // connect_button
            // 
            this.connect_button.Location = new System.Drawing.Point(240, 160);
            this.connect_button.Name = "connect_button";
            this.connect_button.Size = new System.Drawing.Size(132, 23);
            this.connect_button.TabIndex = 2;
            this.connect_button.Text = "Add Connection";
            this.connect_button.UseVisualStyleBackColor = true;
            this.connect_button.Click += new System.EventHandler(this.connect_button_Click);
            // 
            // conn_string_input
            // 
            this.conn_string_input.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.conn_string_input.Location = new System.Drawing.Point(130, 107);
            this.conn_string_input.Name = "conn_string_input";
            this.conn_string_input.Size = new System.Drawing.Size(200, 20);
            this.conn_string_input.TabIndex = 3;
            // 
            // username_label
            // 
            this.username_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.username_label.AutoSize = true;
            this.username_label.Location = new System.Drawing.Point(66, 58);
            this.username_label.Name = "username_label";
            this.username_label.Size = new System.Drawing.Size(58, 13);
            this.username_label.TabIndex = 4;
            this.username_label.Text = "Username:";
            // 
            // password_label
            // 
            this.password_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.password_label.AutoSize = true;
            this.password_label.Location = new System.Drawing.Point(68, 84);
            this.password_label.Name = "password_label";
            this.password_label.Size = new System.Drawing.Size(56, 13);
            this.password_label.TabIndex = 5;
            this.password_label.Text = "Password:";
            // 
            // conn_string_label
            // 
            this.conn_string_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.conn_string_label.AutoSize = true;
            this.conn_string_label.Location = new System.Drawing.Point(32, 110);
            this.conn_string_label.Name = "conn_string_label";
            this.conn_string_label.Size = new System.Drawing.Size(92, 13);
            this.conn_string_label.TabIndex = 6;
            this.conn_string_label.Text = "Connection string:";
            // 
            // cancel_button
            // 
            this.cancel_button.Location = new System.Drawing.Point(159, 160);
            this.cancel_button.Name = "cancel_button";
            this.cancel_button.Size = new System.Drawing.Size(75, 23);
            this.cancel_button.TabIndex = 7;
            this.cancel_button.Text = "Cancel";
            this.cancel_button.UseVisualStyleBackColor = true;
            this.cancel_button.Click += new System.EventHandler(this.cancel_button_Click);
            // 
            // nickname_textbox
            // 
            this.nickname_textbox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.nickname_textbox.Location = new System.Drawing.Point(130, 29);
            this.nickname_textbox.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
            this.nickname_textbox.Name = "nickname_textbox";
            this.nickname_textbox.Size = new System.Drawing.Size(200, 20);
            this.nickname_textbox.TabIndex = 8;
            // 
            // nickname_label
            // 
            this.nickname_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.nickname_label.AutoSize = true;
            this.nickname_label.Location = new System.Drawing.Point(86, 32);
            this.nickname_label.Name = "nickname_label";
            this.nickname_label.Size = new System.Drawing.Size(38, 13);
            this.nickname_label.TabIndex = 9;
            this.nickname_label.Text = "Name:";
            // 
            // AppLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 195);
            this.Controls.Add(this.conn_string_label);
            this.Controls.Add(this.password_label);
            this.Controls.Add(this.username_label);
            this.Controls.Add(this.nickname_label);
            this.Controls.Add(this.conn_string_input);
            this.Controls.Add(this.password_input);
            this.Controls.Add(this.username_input);
            this.Controls.Add(this.nickname_textbox);
            this.Controls.Add(this.cancel_button);
            this.Controls.Add(this.connect_button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AppLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New Connection Creation";
            this.Load += new System.EventHandler(this.AppLogin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox username_input;
        public System.Windows.Forms.TextBox password_input;
        public System.Windows.Forms.Button connect_button;
        public System.Windows.Forms.TextBox conn_string_input;
        public System.Windows.Forms.Label username_label;
        public System.Windows.Forms.Label password_label;
        public System.Windows.Forms.Label conn_string_label;
        private System.Windows.Forms.Button cancel_button;
        public System.Windows.Forms.TextBox nickname_textbox;
        public System.Windows.Forms.Label nickname_label;
    }
}