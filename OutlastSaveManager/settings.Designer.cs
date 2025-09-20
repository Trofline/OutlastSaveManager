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
            shortcutsChk = new CheckBox();
            internalModPackageChk = new CheckBox();
            shCameraChk = new CheckBox();
            fpsChk = new CheckBox();
            lockHitboxChk = new CheckBox();
            disKey = new CheckBox();
            infoChk = new CheckBox();
            shChk = new CheckBox();
            smoothmouseChk = new CheckBox();
            smhkBtn = new Button();
            bootBtn = new Button();
            button1 = new Button();
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
            toolTip1.SetToolTip(bitChk, "Default is 64 Bit; If you have an AMD CPU, use 32 Bit!\r\n");
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
            // shortcutsChk
            // 
            shortcutsChk.AutoSize = true;
            shortcutsChk.Location = new Point(12, 237);
            shortcutsChk.Name = "shortcutsChk";
            shortcutsChk.Size = new Size(166, 19);
            shortcutsChk.TabIndex = 8;
            shortcutsChk.Text = "Shortcuts in Outlast Folder";
            toolTip1.SetToolTip(shortcutsChk, "Creates shortcuts for quick starting in the Outlast root folder\r\n");
            shortcutsChk.UseVisualStyleBackColor = true;
            shortcutsChk.CheckedChanged += shortcuts_CheckedChanged;
            // 
            // internalModPackageChk
            // 
            internalModPackageChk.AutoSize = true;
            internalModPackageChk.Location = new Point(12, 262);
            internalModPackageChk.Name = "internalModPackageChk";
            internalModPackageChk.Size = new Size(176, 19);
            internalModPackageChk.TabIndex = 11;
            internalModPackageChk.Text = "Load External Mod Packages";
            toolTip1.SetToolTip(internalModPackageChk, resources.GetString("internalModPackageChk.ToolTip"));
            internalModPackageChk.UseVisualStyleBackColor = true;
            internalModPackageChk.CheckedChanged += internalModPackageChk_CheckedChanged;
            // 
            // shCameraChk
            // 
            shCameraChk.AutoSize = true;
            shCameraChk.Location = new Point(12, 187);
            shCameraChk.Name = "shCameraChk";
            shCameraChk.Size = new Size(245, 19);
            shCameraChk.TabIndex = 12;
            shCameraChk.Text = "Save Camera Position on SpeedrunHelper";
            toolTip1.SetToolTip(shCameraChk, "Only works for Internal Mod Packages (Load External Mod Packages = OFF)\r\n");
            shCameraChk.UseVisualStyleBackColor = true;
            shCameraChk.CheckedChanged += shCameraChk_CheckedChanged;
            // 
            // fpsChk
            // 
            fpsChk.AutoSize = true;
            fpsChk.Location = new Point(12, 87);
            fpsChk.Name = "fpsChk";
            fpsChk.Size = new Size(170, 19);
            fpsChk.TabIndex = 4;
            fpsChk.Text = "Boot Outlast with 61.81 FPS";
            toolTip1.SetToolTip(fpsChk, "This is ONLY used for bhop categorys. Check the speedrun.com rules");
            fpsChk.UseVisualStyleBackColor = true;
            fpsChk.CheckedChanged += fps_CheckedChanged;
            // 
            // lockHitboxChk
            // 
            lockHitboxChk.AutoSize = true;
            lockHitboxChk.Location = new Point(12, 287);
            lockHitboxChk.Name = "lockHitboxChk";
            lockHitboxChk.Size = new Size(249, 19);
            lockHitboxChk.TabIndex = 13;
            lockHitboxChk.Text = "Lock Changed Hitbock in the Mod section";
            toolTip1.SetToolTip(lockHitboxChk, "In the Mod section, if you choose for example \"VaultHitbox\", it usualy goes back to normal once you jump.\r\nThis option prevents it\r\n");
            lockHitboxChk.UseVisualStyleBackColor = true;
            lockHitboxChk.CheckedChanged += lockHitboxChk_CheckedChanged;
            // 
            // disKey
            // 
            disKey.AutoSize = true;
            disKey.Location = new Point(12, 312);
            disKey.Name = "disKey";
            disKey.Size = new Size(115, 19);
            disKey.TabIndex = 16;
            disKey.Text = "Disable Keybinds";
            toolTip1.SetToolTip(disKey, "Imagine you press while speedrunning the freecam keybind o.o");
            disKey.UseVisualStyleBackColor = true;
            disKey.CheckedChanged += disKey_CheckedChanged;
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
            // shChk
            // 
            shChk.AutoSize = true;
            shChk.Location = new Point(12, 162);
            shChk.Name = "shChk";
            shChk.Size = new Size(111, 19);
            shChk.TabIndex = 6;
            shChk.Text = "SpeedrunHelper";
            shChk.UseVisualStyleBackColor = true;
            shChk.CheckedChanged += speedrunhelper_CheckedChanged;
            // 
            // smoothmouseChk
            // 
            smoothmouseChk.AutoSize = true;
            smoothmouseChk.Location = new Point(12, 212);
            smoothmouseChk.Name = "smoothmouseChk";
            smoothmouseChk.Size = new Size(107, 19);
            smoothmouseChk.TabIndex = 7;
            smoothmouseChk.Text = "Smooth Mouse";
            smoothmouseChk.UseVisualStyleBackColor = true;
            smoothmouseChk.CheckedChanged += smoothmouseChk_CheckedChanged;
            // 
            // smhkBtn
            // 
            smhkBtn.Location = new Point(55, 399);
            smhkBtn.Name = "smhkBtn";
            smhkBtn.Size = new Size(180, 23);
            smhkBtn.TabIndex = 9;
            smhkBtn.Text = "Hotkey Manager";
            smhkBtn.UseVisualStyleBackColor = true;
            smhkBtn.Click += smhkBtn_Click;
            // 
            // bootBtn
            // 
            bootBtn.Location = new Point(55, 370);
            bootBtn.Name = "bootBtn";
            bootBtn.Size = new Size(180, 23);
            bootBtn.TabIndex = 10;
            bootBtn.Text = "Boot Folder";
            bootBtn.UseVisualStyleBackColor = true;
            bootBtn.Click += bootBtn_Click;
            // 
            // button1
            // 
            button1.Location = new Point(55, 341);
            button1.Name = "button1";
            button1.Size = new Size(180, 23);
            button1.TabIndex = 15;
            button1.Text = "Restart Game";
            button1.UseVisualStyleBackColor = true;
            button1.Click += restartgame_click;
            // 
            // settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(286, 434);
            Controls.Add(disKey);
            Controls.Add(button1);
            Controls.Add(lockHitboxChk);
            Controls.Add(shCameraChk);
            Controls.Add(internalModPackageChk);
            Controls.Add(bootBtn);
            Controls.Add(smhkBtn);
            Controls.Add(shortcutsChk);
            Controls.Add(smoothmouseChk);
            Controls.Add(shChk);
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
        private CheckBox shChk;
        private CheckBox smoothmouseChk;
        private CheckBox shortcutsChk;
        private Button smhkBtn;
        private Button bootBtn;
        private CheckBox internalModPackageChk;
        private CheckBox shCameraChk;
        private CheckBox lockHitboxChk;
        private Button button1;
        private CheckBox disKey;
    }
}