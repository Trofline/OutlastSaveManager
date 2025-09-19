namespace OutlastSaveManager
{
    partial class help
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
            toolTip1 = new ToolTip(components);
            btnPermission = new Button();
            btnFurtherHelp = new Button();
            button5 = new Button();
            button3 = new Button();
            SuspendLayout();
            // 
            // btnPermission
            // 
            btnPermission.Location = new Point(12, 12);
            btnPermission.Name = "btnPermission";
            btnPermission.Size = new Size(138, 23);
            btnPermission.TabIndex = 0;
            btnPermission.Text = "Permission Problems";
            toolTip1.SetToolTip(btnPermission, "If you got permission errors, this should fix it");
            btnPermission.UseVisualStyleBackColor = true;
            btnPermission.Click += permission_Click;
            // 
            // btnFurtherHelp
            // 
            btnFurtherHelp.Location = new Point(12, 41);
            btnFurtherHelp.Name = "btnFurtherHelp";
            btnFurtherHelp.Size = new Size(138, 23);
            btnFurtherHelp.TabIndex = 1;
            btnFurtherHelp.Text = "I need further help";
            toolTip1.SetToolTip(btnFurtherHelp, "Add me on discord,\r\nI will try to help you :)");
            btnFurtherHelp.UseVisualStyleBackColor = true;
            btnFurtherHelp.Click += btnFurtherHelp_Click;
            // 
            // button5
            // 
            button5.Location = new Point(156, 41);
            button5.Name = "button5";
            button5.Size = new Size(138, 23);
            button5.TabIndex = 4;
            button5.Text = "Report a bug";
            toolTip1.SetToolTip(button5, "If you found a bug, I would appreciate\r\nit if you would like to share it with me.\r\nYou can add me on my discord");
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button3
            // 
            button3.Location = new Point(156, 12);
            button3.Name = "button3";
            button3.Size = new Size(138, 23);
            button3.TabIndex = 2;
            button3.Text = "Check for update";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // help
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(306, 75);
            Controls.Add(button5);
            Controls.Add(button3);
            Controls.Add(btnFurtherHelp);
            Controls.Add(btnPermission);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "help";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Help";
            TopMost = true;
            Load += help_Load;
            ResumeLayout(false);
        }

        #endregion

        private ToolTip toolTip1;
        private Button btnPermission;
        private Button btnFurtherHelp;
        private Button button3;
        private Button button5;
    }
}