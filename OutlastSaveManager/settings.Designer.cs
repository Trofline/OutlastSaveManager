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
            toolTip1 = new ToolTip(components);
            panel1 = new Panel();
            materialButton3 = new MaterialSkin.Controls.MaterialButton();
            minimizeChk = new MaterialSkin.Controls.MaterialCheckbox();
            materialButton2 = new MaterialSkin.Controls.MaterialButton();
            duplicationChk = new MaterialSkin.Controls.MaterialCheckbox();
            materialButton1 = new MaterialSkin.Controls.MaterialButton();
            bitChk = new MaterialSkin.Controls.MaterialCheckbox();
            pauselossChk = new MaterialSkin.Controls.MaterialCheckbox();
            fpsChk = new MaterialSkin.Controls.MaterialCheckbox();
            disKey = new MaterialSkin.Controls.MaterialCheckbox();
            borderlessChk = new MaterialSkin.Controls.MaterialCheckbox();
            lockHitboxChk = new MaterialSkin.Controls.MaterialCheckbox();
            infoChk = new MaterialSkin.Controls.MaterialCheckbox();
            internalModPackageChk = new MaterialSkin.Controls.MaterialCheckbox();
            shChk = new MaterialSkin.Controls.MaterialCheckbox();
            shortcutsChk = new MaterialSkin.Controls.MaterialCheckbox();
            shCameraChk = new MaterialSkin.Controls.MaterialCheckbox();
            smoothmouseChk = new MaterialSkin.Controls.MaterialCheckbox();
            label1 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Control;
            panel1.Controls.Add(materialButton3);
            panel1.Controls.Add(minimizeChk);
            panel1.Controls.Add(materialButton2);
            panel1.Controls.Add(duplicationChk);
            panel1.Controls.Add(materialButton1);
            panel1.Controls.Add(bitChk);
            panel1.Controls.Add(pauselossChk);
            panel1.Controls.Add(fpsChk);
            panel1.Controls.Add(disKey);
            panel1.Controls.Add(borderlessChk);
            panel1.Controls.Add(lockHitboxChk);
            panel1.Controls.Add(infoChk);
            panel1.Controls.Add(internalModPackageChk);
            panel1.Controls.Add(shChk);
            panel1.Controls.Add(shortcutsChk);
            panel1.Controls.Add(shCameraChk);
            panel1.Controls.Add(smoothmouseChk);
            panel1.Location = new Point(-2, 24);
            panel1.Name = "panel1";
            panel1.Size = new Size(357, 692);
            panel1.TabIndex = 0;
            panel1.Paint += panel1_Paint;
            // 
            // materialButton3
            // 
            materialButton3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            materialButton3.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            materialButton3.Depth = 0;
            materialButton3.HighEmphasis = true;
            materialButton3.Icon = null;
            materialButton3.Location = new Point(97, 541);
            materialButton3.Margin = new Padding(4, 6, 4, 6);
            materialButton3.MouseState = MaterialSkin.MouseState.HOVER;
            materialButton3.Name = "materialButton3";
            materialButton3.NoAccentTextColor = Color.Empty;
            materialButton3.Size = new Size(152, 36);
            materialButton3.TabIndex = 3;
            materialButton3.Text = "Hotkey Manager";
            materialButton3.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            materialButton3.UseAccentColor = false;
            materialButton3.UseVisualStyleBackColor = true;
            materialButton3.Click += smhkBtn_Click;
            // 
            // minimizeChk
            // 
            minimizeChk.AutoSize = true;
            minimizeChk.BackColor = Color.Transparent;
            minimizeChk.Depth = 0;
            minimizeChk.Location = new Point(11, 10);
            minimizeChk.Margin = new Padding(0);
            minimizeChk.MouseLocation = new Point(-1, -1);
            minimizeChk.MouseState = MaterialSkin.MouseState.HOVER;
            minimizeChk.Name = "minimizeChk";
            minimizeChk.ReadOnly = false;
            minimizeChk.Ripple = true;
            minimizeChk.Size = new Size(147, 37);
            minimizeChk.TabIndex = 2;
            minimizeChk.Text = "Start minimized";
            minimizeChk.UseVisualStyleBackColor = false;
            // 
            // materialButton2
            // 
            materialButton2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            materialButton2.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            materialButton2.Depth = 0;
            materialButton2.HighEmphasis = true;
            materialButton2.Icon = null;
            materialButton2.Location = new Point(97, 637);
            materialButton2.Margin = new Padding(4, 6, 4, 6);
            materialButton2.MouseState = MaterialSkin.MouseState.HOVER;
            materialButton2.Name = "materialButton2";
            materialButton2.NoAccentTextColor = Color.Empty;
            materialButton2.Size = new Size(117, 36);
            materialButton2.TabIndex = 3;
            materialButton2.Text = "Boot Folder";
            materialButton2.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            materialButton2.UseAccentColor = false;
            materialButton2.UseVisualStyleBackColor = true;
            materialButton2.Click += bootBtn_Click;
            // 
            // duplicationChk
            // 
            duplicationChk.AutoSize = true;
            duplicationChk.BackColor = Color.Transparent;
            duplicationChk.Depth = 0;
            duplicationChk.Location = new Point(11, 47);
            duplicationChk.Margin = new Padding(0);
            duplicationChk.MouseLocation = new Point(-1, -1);
            duplicationChk.MouseState = MaterialSkin.MouseState.HOVER;
            duplicationChk.Name = "duplicationChk";
            duplicationChk.ReadOnly = false;
            duplicationChk.Ripple = true;
            duplicationChk.Size = new Size(256, 37);
            duplicationChk.TabIndex = 2;
            duplicationChk.Text = "Prevent Checkpoint Duplication";
            duplicationChk.UseVisualStyleBackColor = false;
            // 
            // materialButton1
            // 
            materialButton1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            materialButton1.Depth = 0;
            materialButton1.HighEmphasis = true;
            materialButton1.Icon = null;
            materialButton1.Location = new Point(97, 589);
            materialButton1.Margin = new Padding(4, 6, 4, 6);
            materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
            materialButton1.Name = "materialButton1";
            materialButton1.NoAccentTextColor = Color.Empty;
            materialButton1.Size = new Size(129, 36);
            materialButton1.TabIndex = 3;
            materialButton1.Text = "Restart Game";
            materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            materialButton1.UseAccentColor = false;
            materialButton1.UseVisualStyleBackColor = true;
            materialButton1.Click += restartgame_click;
            // 
            // bitChk
            // 
            bitChk.AutoSize = true;
            bitChk.BackColor = Color.Transparent;
            bitChk.Depth = 0;
            bitChk.Location = new Point(11, 84);
            bitChk.Margin = new Padding(0);
            bitChk.MouseLocation = new Point(-1, -1);
            bitChk.MouseState = MaterialSkin.MouseState.HOVER;
            bitChk.Name = "bitChk";
            bitChk.ReadOnly = false;
            bitChk.Ripple = true;
            bitChk.Size = new Size(189, 37);
            bitChk.TabIndex = 2;
            bitChk.Text = "Boot Outlst with 32Bit";
            bitChk.UseVisualStyleBackColor = false;
            // 
            // pauselossChk
            // 
            pauselossChk.AutoSize = true;
            pauselossChk.BackColor = Color.Transparent;
            pauselossChk.Depth = 0;
            pauselossChk.Location = new Point(11, 491);
            pauselossChk.Margin = new Padding(0);
            pauselossChk.MouseLocation = new Point(-1, -1);
            pauselossChk.MouseState = MaterialSkin.MouseState.HOVER;
            pauselossChk.Name = "pauselossChk";
            pauselossChk.ReadOnly = false;
            pauselossChk.Ripple = true;
            pauselossChk.Size = new Size(294, 37);
            pauselossChk.TabIndex = 2;
            pauselossChk.Text = "Pause on Loss Focus - Coming Soon";
            pauselossChk.UseVisualStyleBackColor = false;
            pauselossChk.CheckedChanged += pauselossChk_CheckedChanged;
            // 
            // fpsChk
            // 
            fpsChk.AutoSize = true;
            fpsChk.BackColor = Color.Transparent;
            fpsChk.Depth = 0;
            fpsChk.Location = new Point(11, 121);
            fpsChk.Margin = new Padding(0);
            fpsChk.MouseLocation = new Point(-1, -1);
            fpsChk.MouseState = MaterialSkin.MouseState.HOVER;
            fpsChk.Name = "fpsChk";
            fpsChk.ReadOnly = false;
            fpsChk.Ripple = true;
            fpsChk.Size = new Size(234, 37);
            fpsChk.TabIndex = 2;
            fpsChk.Text = "Boot Outlast with 61.81 FPS";
            fpsChk.UseVisualStyleBackColor = false;
            // 
            // disKey
            // 
            disKey.AutoSize = true;
            disKey.BackColor = Color.Transparent;
            disKey.Depth = 0;
            disKey.Location = new Point(11, 454);
            disKey.Margin = new Padding(0);
            disKey.MouseLocation = new Point(-1, -1);
            disKey.MouseState = MaterialSkin.MouseState.HOVER;
            disKey.Name = "disKey";
            disKey.ReadOnly = false;
            disKey.Ripple = true;
            disKey.Size = new Size(157, 37);
            disKey.TabIndex = 2;
            disKey.Text = "Disable Keybinds";
            disKey.UseVisualStyleBackColor = false;
            // 
            // borderlessChk
            // 
            borderlessChk.AutoSize = true;
            borderlessChk.BackColor = Color.Transparent;
            borderlessChk.Depth = 0;
            borderlessChk.Location = new Point(11, 158);
            borderlessChk.Margin = new Padding(0);
            borderlessChk.MouseLocation = new Point(-1, -1);
            borderlessChk.MouseState = MaterialSkin.MouseState.HOVER;
            borderlessChk.Name = "borderlessChk";
            borderlessChk.ReadOnly = false;
            borderlessChk.Ripple = true;
            borderlessChk.Size = new Size(170, 37);
            borderlessChk.TabIndex = 2;
            borderlessChk.Text = "Borderless Window";
            borderlessChk.UseVisualStyleBackColor = false;
            // 
            // lockHitboxChk
            // 
            lockHitboxChk.AutoSize = true;
            lockHitboxChk.BackColor = Color.Transparent;
            lockHitboxChk.Depth = 0;
            lockHitboxChk.Location = new Point(11, 417);
            lockHitboxChk.Margin = new Padding(0);
            lockHitboxChk.MouseLocation = new Point(-1, -1);
            lockHitboxChk.MouseState = MaterialSkin.MouseState.HOVER;
            lockHitboxChk.Name = "lockHitboxChk";
            lockHitboxChk.ReadOnly = false;
            lockHitboxChk.Ripple = true;
            lockHitboxChk.Size = new Size(186, 37);
            lockHitboxChk.TabIndex = 2;
            lockHitboxChk.Text = "Lock Changed Hitbox";
            lockHitboxChk.UseVisualStyleBackColor = false;
            // 
            // infoChk
            // 
            infoChk.AutoSize = true;
            infoChk.BackColor = Color.Transparent;
            infoChk.Depth = 0;
            infoChk.Location = new Point(11, 195);
            infoChk.Margin = new Padding(0);
            infoChk.MouseLocation = new Point(-1, -1);
            infoChk.MouseState = MaterialSkin.MouseState.HOVER;
            infoChk.Name = "infoChk";
            infoChk.ReadOnly = false;
            infoChk.Ripple = true;
            infoChk.Size = new Size(162, 37);
            infoChk.TabIndex = 2;
            infoChk.Text = "Display Info icons";
            infoChk.UseVisualStyleBackColor = false;
            // 
            // internalModPackageChk
            // 
            internalModPackageChk.AutoSize = true;
            internalModPackageChk.BackColor = Color.Transparent;
            internalModPackageChk.Depth = 0;
            internalModPackageChk.Location = new Point(11, 380);
            internalModPackageChk.Margin = new Padding(0);
            internalModPackageChk.MouseLocation = new Point(-1, -1);
            internalModPackageChk.MouseState = MaterialSkin.MouseState.HOVER;
            internalModPackageChk.Name = "internalModPackageChk";
            internalModPackageChk.ReadOnly = false;
            internalModPackageChk.Ripple = true;
            internalModPackageChk.Size = new Size(241, 37);
            internalModPackageChk.TabIndex = 2;
            internalModPackageChk.Text = "Load External Mod Packages";
            internalModPackageChk.UseVisualStyleBackColor = false;
            // 
            // shChk
            // 
            shChk.AutoSize = true;
            shChk.BackColor = Color.Transparent;
            shChk.Depth = 0;
            shChk.Location = new Point(11, 232);
            shChk.Margin = new Padding(0);
            shChk.MouseLocation = new Point(-1, -1);
            shChk.MouseState = MaterialSkin.MouseState.HOVER;
            shChk.Name = "shChk";
            shChk.ReadOnly = false;
            shChk.Ripple = true;
            shChk.Size = new Size(147, 37);
            shChk.TabIndex = 2;
            shChk.Text = "SpeedrunHelper";
            shChk.UseVisualStyleBackColor = false;
            // 
            // shortcutsChk
            // 
            shortcutsChk.AutoSize = true;
            shortcutsChk.BackColor = Color.Transparent;
            shortcutsChk.Depth = 0;
            shortcutsChk.Location = new Point(11, 343);
            shortcutsChk.Margin = new Padding(0);
            shortcutsChk.MouseLocation = new Point(-1, -1);
            shortcutsChk.MouseState = MaterialSkin.MouseState.HOVER;
            shortcutsChk.Name = "shortcutsChk";
            shortcutsChk.ReadOnly = false;
            shortcutsChk.Ripple = true;
            shortcutsChk.Size = new Size(215, 37);
            shortcutsChk.TabIndex = 2;
            shortcutsChk.Text = "Shortcut in Outlast Folder";
            shortcutsChk.UseVisualStyleBackColor = false;
            // 
            // shCameraChk
            // 
            shCameraChk.AutoSize = true;
            shCameraChk.BackColor = Color.Transparent;
            shCameraChk.Depth = 0;
            shCameraChk.Location = new Point(11, 269);
            shCameraChk.Margin = new Padding(0);
            shCameraChk.MouseLocation = new Point(-1, -1);
            shCameraChk.MouseState = MaterialSkin.MouseState.HOVER;
            shCameraChk.Name = "shCameraChk";
            shCameraChk.ReadOnly = false;
            shCameraChk.Ripple = true;
            shCameraChk.Size = new Size(299, 37);
            shCameraChk.TabIndex = 2;
            shCameraChk.Text = "Load Camera-Pos on SpeedrunHelper";
            shCameraChk.UseVisualStyleBackColor = false;
            // 
            // smoothmouseChk
            // 
            smoothmouseChk.AutoSize = true;
            smoothmouseChk.BackColor = Color.Transparent;
            smoothmouseChk.Depth = 0;
            smoothmouseChk.Location = new Point(11, 306);
            smoothmouseChk.Margin = new Padding(0);
            smoothmouseChk.MouseLocation = new Point(-1, -1);
            smoothmouseChk.MouseState = MaterialSkin.MouseState.HOVER;
            smoothmouseChk.Name = "smoothmouseChk";
            smoothmouseChk.ReadOnly = false;
            smoothmouseChk.Ripple = true;
            smoothmouseChk.Size = new Size(143, 37);
            smoothmouseChk.TabIndex = 2;
            smoothmouseChk.Text = "Smooth Mouse";
            smoothmouseChk.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Location = new Point(20, 6);
            label1.Name = "label1";
            label1.Size = new Size(49, 15);
            label1.TabIndex = 1;
            label1.Text = "Settings";
            // 
            // settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(467, 991);
            Controls.Add(label1);
            Controls.Add(panel1);
            ForeColor = SystemColors.ControlLightLight;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "settings";
            Sizable = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Settings";
            TopMost = true;
            Load += settings_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ToolTip toolTip1;
        private Panel panel1;
        private Label label1;
        private MaterialSkin.Controls.MaterialCheckbox minimizeChk;
        private MaterialSkin.Controls.MaterialCheckbox duplicationChk;
        private MaterialSkin.Controls.MaterialCheckbox bitChk;
        private MaterialSkin.Controls.MaterialCheckbox fpsChk;
        private MaterialSkin.Controls.MaterialCheckbox borderlessChk;
        private MaterialSkin.Controls.MaterialCheckbox infoChk;
        private MaterialSkin.Controls.MaterialCheckbox shChk;
        private MaterialSkin.Controls.MaterialCheckbox smoothmouseChk;
        private MaterialSkin.Controls.MaterialCheckbox shortcutsChk;
        private MaterialSkin.Controls.MaterialCheckbox internalModPackageChk;
        private MaterialSkin.Controls.MaterialCheckbox lockHitboxChk;
        private MaterialSkin.Controls.MaterialCheckbox disKey;
        private MaterialSkin.Controls.MaterialCheckbox pauselossChk;
        private MaterialSkin.Controls.MaterialButton materialButton1;
        private MaterialSkin.Controls.MaterialButton materialButton2;
        private MaterialSkin.Controls.MaterialButton materialButton3;
        private MaterialSkin.Controls.MaterialCheckbox shCameraChk;
    }
}