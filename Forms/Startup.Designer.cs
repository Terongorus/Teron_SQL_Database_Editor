namespace Startup
{
    partial class Startup
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
            azure_open = new System.Windows.Forms.Button();
            warcraft_open = new System.Windows.Forms.Button();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // azure_open
            // 
            azure_open.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            azure_open.Location = new System.Drawing.Point(3, 3);
            azure_open.Name = "azure_open";
            azure_open.Size = new System.Drawing.Size(150, 50);
            azure_open.TabIndex = 0;
            azure_open.Text = "Azure SQL Editor";
            azure_open.UseVisualStyleBackColor = true;
            azure_open.Click += azure_open_Click;
            // 
            // warcraft_open
            // 
            warcraft_open.Anchor = System.Windows.Forms.AnchorStyles.None;
            warcraft_open.Location = new System.Drawing.Point(3, 59);
            warcraft_open.Name = "warcraft_open";
            warcraft_open.Size = new System.Drawing.Size(150, 50);
            warcraft_open.TabIndex = 1;
            warcraft_open.Text = "Warcraft DBC Editor";
            warcraft_open.UseVisualStyleBackColor = true;
            warcraft_open.Click += warcraft_open_Click;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(azure_open);
            flowLayoutPanel1.Controls.Add(warcraft_open);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(800, 450);
            flowLayoutPanel1.TabIndex = 2;
            // 
            // Startup
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(flowLayoutPanel1);
            Name = "Startup";
            Text = "Form1";
            Load += Startup_Load;
            flowLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button azure_open;
        private System.Windows.Forms.Button warcraft_open;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}