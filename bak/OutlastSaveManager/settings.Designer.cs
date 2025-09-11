namespace OutlastSaveManager
{
    partial class settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(settings));
            minimizeChk = new CheckBox();
            duplicationChk = new CheckBox();
            toolTip1 = new ToolTip(components);
            bitChk = new CheckBox();
            borderlessChk = new CheckBox();
            infoChk = new CheckBox();
            fpsChk = new CheckBox();
            manageBindingsBtn = new Button();
            SuspendLayout();
            // 
            // minimizeChk
            // 
            minimizeChk.AutoSize = true;
            minimizeChk.Location = new Point(12, 12);
            minimizeChk.Name = "minimizeChk";
            minimizeChk.Size = new Size(109, 19);
            minimizeChk.TabIndex = 0;
            minimizeChk.Text = "Start minimized";
            toolTip1.SetToolTip(minimizeChk, "SaveManager boots minimized");
            minimizeChk.UseVisualStyleBackColor = true;
            minimizeChk.CheckedChanged += minimizeChk_CheckedChanged;
            // 
            // duplicationChk
            // 
            duplicationChk.AutoSize = true;
            duplicationChk.Location = new Point(12, 37);
            duplicationChk.Name = "duplicationChk";
            duplicationChk.Size = new Size(261, 19);
            duplicationChk.TabIndex = 0;
            duplicationChk.Text = "Prevent Outlast from checkpoint duplication";
            toolTip1.SetToolTip(duplicationChk, resources.GetString("duplicationChk.ToolTip"));
            duplicationChk.UseVisualStyleBackColor = true;
            duplicationChk.CheckedChanged += duplicationChk_CheckedChanged;
            // 
            // bitChk
            // 
            bitChk.AutoSize = true;
            bitChk.Location = new Point(12, 62);
            bitChk.Name = "bitChk";
            bitChk.Size = new Size(147, 19);
            bitChk.TabIndex = 1;
            bitChk.Text = "Boot Outlast with 32Bit";
            toolTip1.SetToolTip(bitChk, "Default is 64 Bit");
            bitChk.UseVisualStyleBackColor = true;
            bitChk.CheckedChanged += Bit_CheckedChanged;
            // 
            // borderlessChk
            // 
            borderlessChk.AutoSize = true;
            borderlessChk.Location = new Point(12, 112);
            borderlessChk.Name = "borderlessChk";
            borderlessChk.Size = new Size(127, 19);
            borderlessChk.TabIndex = 2;
            borderlessChk.Text = "Borderless Window";
            toolTip1.SetToolTip(borderlessChk, "You may get perfomance issues with this option enabled");
            borderlessChk.UseVisualStyleBackColor = true;
            borderlessChk.CheckedChanged += borderless_CheckedChanged;
            // 
            // infoChk
            // 
            infoChk.AutoSize = true;
            infoChk.Location = new Point(12, 137);
            infoChk.Name = "infoChk";
            infoChk.Size = new Size(119, 19);
            infoChk.TabIndex = 3;
            infoChk.Text = "Display Info icons";
            infoChk.UseVisualStyleBackColor = true;
            infoChk.CheckedChanged += infoChk_CheckedChanged;
            // 
            // fpsChk
            // 
            fpsChk.AutoSize = true;
            fpsChk.Location = new Point(12, 87);
            fpsChk.Name = "fpsChk";
            fpsChk.Size = new Size(75, 19);
            fpsChk.TabIndex = 4;
            fpsChk.Text = "61.81 FPS";
            fpsChk.UseVisualStyleBackColor = true;
            fpsChk.CheckedChanged += fps_CheckedChanged;
            // 
            // manageBindingsBtn
            // 
            manageBindingsBtn.Location = new Point(54, 256);
            manageBindingsBtn.Name = "manageBindingsBtn";
            manageBindingsBtn.Size = new Size(180, 23);
            manageBindingsBtn.TabIndex = 5;
            manageBindingsBtn.Text = "Manage Bindings";
            manageBindingsBtn.UseVisualStyleBackColor = true;
            manageBindingsBtn.Click += manageBindingsBtn_Click;
            // 
            // settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(286, 291);
            Controls.Add(manageBindingsBtn);
            Controls.Add(fpsChk);
            Controls.Add(infoChk);
            Controls.Add(borderlessChk);
            Controls.Add(bitChk);
            Controls.Add(duplicationChk);
            Controls.Add(minimizeChk);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "settings";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Settings";
            TopMost = true;
            Load += settings_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox minimizeChk;
        private CheckBox duplicationChk;
        private ToolTip toolTip1;
        private CheckBox bitChk;
        private CheckBox borderlessChk;
        private CheckBox infoChk;
        private CheckBox fpsChk;
        private Button manageBindingsBtn;
    }
}