namespace OutlastSaveManager
{
    partial class mods
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
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            toolTip1 = new ToolTip(components);
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(12, 41);
            button1.Name = "button1";
            button1.Size = new Size(171, 23);
            button1.TabIndex = 0;
            button1.Text = "Infinite NightVision";
            toolTip1.SetToolTip(button1, "Please notice, that the first button-press enabled this feature, press one more time to enable this cheat");
            button1.UseVisualStyleBackColor = true;
            button1.MouseClick += nightVisionToggle;
            // 
            // button2
            // 
            button2.Location = new Point(12, 70);
            button2.Name = "button2";
            button2.Size = new Size(171, 23);
            button2.TabIndex = 0;
            button2.Text = "Normal Hitbox";
            button2.UseVisualStyleBackColor = true;
            button2.Click += HitBoxNormal;
            // 
            // button3
            // 
            button3.Location = new Point(12, 99);
            button3.Name = "button3";
            button3.Size = new Size(171, 23);
            button3.TabIndex = 0;
            button3.Text = "Vault Hitbox";
            toolTip1.SetToolTip(button3, "Make sure to choose normal hitbox, before/after reloading any checkpoint, to prevent irritation of how the game code works\r\n");
            button3.UseVisualStyleBackColor = true;
            button3.Click += HitBoxVault;
            // 
            // button4
            // 
            button4.Location = new Point(12, 128);
            button4.Name = "button4";
            button4.Size = new Size(171, 23);
            button4.TabIndex = 0;
            button4.Text = "Door Hitbox";
            toolTip1.SetToolTip(button4, "Make sure to choose normal hitbox, before/after reloading any checkpoint, to prevent irritation of how the game code works");
            button4.UseVisualStyleBackColor = true;
            button4.Click += HitBoxDoor;
            // 
            // button5
            // 
            button5.Location = new Point(12, 157);
            button5.Name = "button5";
            button5.Size = new Size(171, 23);
            button5.TabIndex = 0;
            button5.Text = "Shimmy Hitbox";
            toolTip1.SetToolTip(button5, "Make sure to choose normal hitbox, before/after reloading any checkpoint, to prevent irritation of how the game code works\r\n");
            button5.UseVisualStyleBackColor = true;
            button5.Click += HitBoxShimmy;
            // 
            // button6
            // 
            button6.Location = new Point(12, 12);
            button6.Name = "button6";
            button6.Size = new Size(171, 23);
            button6.TabIndex = 1;
            button6.Text = "Freeze Enemy";
            button6.UseVisualStyleBackColor = true;
            button6.Click += toggleEnemy_Click;
            // 
            // mods
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(197, 196);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "mods";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Mods";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private ToolTip toolTip1;
    }
}