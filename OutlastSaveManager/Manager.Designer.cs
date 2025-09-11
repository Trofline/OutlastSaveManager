namespace OutlastSaveManager
{
    partial class Manager
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
            treeViewLive = new TreeView();
            treeViewProject = new TreeView();
            panelProject = new Panel();
            panelLive = new Panel();
            label1 = new Label();
            label2 = new Label();
            versionLabel = new Label();
            panelBar = new Panel();
            consoleArea = new RichTextBox();
            btnRemoveChk = new Button();
            panelBtnRemoveChk = new Panel();
            panelBtnSetRO = new Panel();
            btnSetRO = new Button();
            panelBtnAdd = new Panel();
            btnAdd = new Button();
            panelBtnRemoveRO = new Panel();
            btnRemoveRO = new Button();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            help = new LinkLabel();
            label7 = new Label();
            linkLabel1 = new LinkLabel();
            linkLabel2 = new LinkLabel();
            label8 = new Label();
            linkLabel3 = new LinkLabel();
            label9 = new Label();
            label11 = new Label();
            label3 = new Label();
            labelHours = new Label();
            label10 = new Label();
            linkLabel4 = new LinkLabel();
            label12 = new Label();
            linkLabel5 = new LinkLabel();
            info1 = new Label();
            info2 = new Label();
            info7 = new Label();
            info8 = new Label();
            info9 = new Label();
            info10 = new Label();
            info11 = new Label();
            info12 = new Label();
            info13 = new Label();
            info3 = new Label();
            info5 = new Label();
            info6 = new Label();
            info4 = new Label();
            panelProject.SuspendLayout();
            panelLive.SuspendLayout();
            panelBar.SuspendLayout();
            panelBtnRemoveChk.SuspendLayout();
            panelBtnSetRO.SuspendLayout();
            panelBtnAdd.SuspendLayout();
            panelBtnRemoveRO.SuspendLayout();
            SuspendLayout();
            // 
            // treeViewLive
            // 
            treeViewLive.BackColor = Color.FromArgb(10, 10, 10);
            treeViewLive.BorderStyle = BorderStyle.None;
            treeViewLive.CheckBoxes = true;
            treeViewLive.Font = new Font("Segoe UI Variable Display Light", 12.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            treeViewLive.ForeColor = SystemColors.Control;
            treeViewLive.ImeMode = ImeMode.Katakana;
            treeViewLive.LineColor = Color.FromArgb(171, 171, 171);
            treeViewLive.Location = new Point(9, 10);
            treeViewLive.Name = "treeViewLive";
            treeViewLive.Size = new Size(380, 292);
            treeViewLive.TabIndex = 6;
            treeViewLive.AfterCheck += treeViewLive_AfterCheck;
            // 
            // treeViewProject
            // 
            treeViewProject.BackColor = Color.FromArgb(10, 10, 10);
            treeViewProject.BorderStyle = BorderStyle.None;
            treeViewProject.CheckBoxes = true;
            treeViewProject.Font = new Font("Segoe UI Variable Display Light", 12.75F);
            treeViewProject.ForeColor = SystemColors.Control;
            treeViewProject.LineColor = Color.DimGray;
            treeViewProject.Location = new Point(5, 10);
            treeViewProject.Name = "treeViewProject";
            treeViewProject.Size = new Size(383, 292);
            treeViewProject.TabIndex = 5;
            treeViewProject.AfterCheck += treeViewProject_AfterCheck;
            // 
            // panelProject
            // 
            panelProject.Controls.Add(treeViewProject);
            panelProject.Location = new Point(457, 106);
            panelProject.Name = "panelProject";
            panelProject.Size = new Size(407, 311);
            panelProject.TabIndex = 9;
            // 
            // panelLive
            // 
            panelLive.Controls.Add(treeViewLive);
            panelLive.Location = new Point(25, 106);
            panelLive.Name = "panelLive";
            panelLive.Size = new Size(410, 311);
            panelLive.TabIndex = 10;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Historic", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ButtonFace;
            label1.Location = new Point(466, 61);
            label1.Name = "label1";
            label1.Size = new Size(219, 37);
            label1.TabIndex = 11;
            label1.Text = "Checkpoint Pod";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Historic", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = SystemColors.ButtonFace;
            label2.Location = new Point(34, 61);
            label2.Name = "label2";
            label2.Size = new Size(276, 37);
            label2.TabIndex = 12;
            label2.Text = "Loaded Checkpoints";
            // 
            // versionLabel
            // 
            versionLabel.AutoSize = true;
            versionLabel.Font = new Font("Ebrima", 8.25F);
            versionLabel.ForeColor = SystemColors.ButtonFace;
            versionLabel.Location = new Point(236, 593);
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new Size(71, 13);
            versionLabel.TabIndex = 18;
            versionLabel.Text = "Version: X.X.X";
            // 
            // panelBar
            // 
            panelBar.Controls.Add(consoleArea);
            panelBar.Location = new Point(457, 435);
            panelBar.Name = "panelBar";
            panelBar.Size = new Size(407, 135);
            panelBar.TabIndex = 10;
            // 
            // consoleArea
            // 
            consoleArea.BackColor = Color.FromArgb(10, 10, 10);
            consoleArea.BorderStyle = BorderStyle.None;
            consoleArea.Font = new Font("Lucida Console", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            consoleArea.ForeColor = Color.FromArgb(97, 209, 125);
            consoleArea.Location = new Point(10, 11);
            consoleArea.Name = "consoleArea";
            consoleArea.ReadOnly = true;
            consoleArea.Size = new Size(386, 121);
            consoleArea.TabIndex = 0;
            consoleArea.Text = "Welcome to SaveManager.\n\n";
            // 
            // btnRemoveChk
            // 
            btnRemoveChk.BackColor = Color.Transparent;
            btnRemoveChk.FlatAppearance.BorderSize = 0;
            btnRemoveChk.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnRemoveChk.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnRemoveChk.FlatStyle = FlatStyle.Flat;
            btnRemoveChk.Font = new Font("Cascadia Code SemiBold", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnRemoveChk.ForeColor = SystemColors.ButtonFace;
            btnRemoveChk.Location = new Point(0, 4);
            btnRemoveChk.Name = "btnRemoveChk";
            btnRemoveChk.Size = new Size(190, 37);
            btnRemoveChk.TabIndex = 7;
            btnRemoveChk.Text = "Remove Checkpoint";
            btnRemoveChk.UseVisualStyleBackColor = false;
            btnRemoveChk.Click += btnRemoveCheckpoints_Click;
            btnRemoveChk.MouseEnter += panelBtnRemoveChk_MouseEnter;
            btnRemoveChk.MouseLeave += panelBtnRemoveChk_MouseLeave;
            // 
            // panelBtnRemoveChk
            // 
            panelBtnRemoveChk.Controls.Add(btnRemoveChk);
            panelBtnRemoveChk.Location = new Point(244, 435);
            panelBtnRemoveChk.Name = "panelBtnRemoveChk";
            panelBtnRemoveChk.Size = new Size(191, 41);
            panelBtnRemoveChk.TabIndex = 19;
            // 
            // panelBtnSetRO
            // 
            panelBtnSetRO.BackColor = Color.Transparent;
            panelBtnSetRO.BackgroundImage = Properties.Resources.Frame11;
            panelBtnSetRO.Controls.Add(btnSetRO);
            panelBtnSetRO.Location = new Point(25, 482);
            panelBtnSetRO.Name = "panelBtnSetRO";
            panelBtnSetRO.Size = new Size(188, 41);
            panelBtnSetRO.TabIndex = 17;
            // 
            // btnSetRO
            // 
            btnSetRO.BackColor = Color.Transparent;
            btnSetRO.FlatAppearance.BorderSize = 0;
            btnSetRO.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnSetRO.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnSetRO.FlatStyle = FlatStyle.Flat;
            btnSetRO.Font = new Font("Cascadia Code", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSetRO.ForeColor = SystemColors.ActiveCaptionText;
            btnSetRO.Location = new Point(1, 0);
            btnSetRO.Name = "btnSetRO";
            btnSetRO.Size = new Size(188, 40);
            btnSetRO.TabIndex = 7;
            btnSetRO.Text = "Set ReadOnly";
            btnSetRO.UseVisualStyleBackColor = false;
            btnSetRO.Click += btnSetReadOnly_Click;
            btnSetRO.MouseEnter += btnSet_MouseEnter;
            btnSetRO.MouseLeave += btnSet_MouseLeave;
            // 
            // panelBtnAdd
            // 
            panelBtnAdd.BackColor = Color.Transparent;
            panelBtnAdd.BackgroundImage = Properties.Resources.Frame11;
            panelBtnAdd.Controls.Add(btnAdd);
            panelBtnAdd.Location = new Point(25, 435);
            panelBtnAdd.Name = "panelBtnAdd";
            panelBtnAdd.Size = new Size(188, 41);
            panelBtnAdd.TabIndex = 17;
            // 
            // btnAdd
            // 
            btnAdd.BackColor = Color.Transparent;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnAdd.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Font = new Font("Cascadia Code", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAdd.ForeColor = SystemColors.ActiveCaptionText;
            btnAdd.Location = new Point(0, 0);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(188, 40);
            btnAdd.TabIndex = 7;
            btnAdd.Text = "Set Checkpoint";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += btnAddCheckpoints_Click;
            btnAdd.MouseEnter += btnAdd_MouseEnter;
            btnAdd.MouseLeave += btnAdd_MouseLeave;
            // 
            // panelBtnRemoveRO
            // 
            panelBtnRemoveRO.Controls.Add(btnRemoveRO);
            panelBtnRemoveRO.Location = new Point(245, 482);
            panelBtnRemoveRO.Name = "panelBtnRemoveRO";
            panelBtnRemoveRO.Size = new Size(191, 41);
            panelBtnRemoveRO.TabIndex = 20;
            // 
            // btnRemoveRO
            // 
            btnRemoveRO.BackColor = Color.Transparent;
            btnRemoveRO.FlatAppearance.BorderSize = 0;
            btnRemoveRO.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnRemoveRO.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnRemoveRO.FlatStyle = FlatStyle.Flat;
            btnRemoveRO.Font = new Font("Cascadia Code SemiBold", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnRemoveRO.ForeColor = SystemColors.ButtonFace;
            btnRemoveRO.Location = new Point(0, 4);
            btnRemoveRO.Name = "btnRemoveRO";
            btnRemoveRO.Size = new Size(190, 37);
            btnRemoveRO.TabIndex = 7;
            btnRemoveRO.Text = "Remove ReadOnly";
            btnRemoveRO.UseVisualStyleBackColor = false;
            btnRemoveRO.Click += btnRemoveReadOnly_Click;
            btnRemoveRO.MouseEnter += btnRemoveRO_MouseEnter;
            btnRemoveRO.MouseLeave += btnRemoveRO_MouseLeave;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Ebrima", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.ForeColor = SystemColors.ButtonFace;
            label4.Location = new Point(12, 593);
            label4.Name = "label4";
            label4.Size = new Size(203, 13);
            label4.TabIndex = 18;
            label4.Text = "© 2025 TroflineBlack. All rights reserved";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Variable Display Light", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.ForeColor = SystemColors.ButtonFace;
            label5.Location = new Point(221, 590);
            label5.Name = "label5";
            label5.Size = new Size(9, 16);
            label5.TabIndex = 18;
            label5.Text = "|";
            label5.Click += easteregg;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI Variable Display Light", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.ForeColor = SystemColors.ButtonFace;
            label6.Location = new Point(313, 590);
            label6.Name = "label6";
            label6.Size = new Size(9, 16);
            label6.TabIndex = 21;
            label6.Text = "|";
            // 
            // help
            // 
            help.ActiveLinkColor = Color.FromArgb(97, 209, 125);
            help.AutoSize = true;
            help.Font = new Font("Ebrima", 8.25F);
            help.ForeColor = Color.FromArgb(150, 255, 255, 255);
            help.LinkColor = Color.FromArgb(80, 255, 255, 255);
            help.Location = new Point(328, 592);
            help.Name = "help";
            help.Size = new Size(30, 13);
            help.TabIndex = 22;
            help.TabStop = true;
            help.Text = "Help";
            help.LinkClicked += help_LinkClicked;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI Variable Display Light", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label7.ForeColor = SystemColors.ButtonFace;
            label7.Location = new Point(364, 589);
            label7.Name = "label7";
            label7.Size = new Size(9, 16);
            label7.TabIndex = 21;
            label7.Text = "|";
            // 
            // linkLabel1
            // 
            linkLabel1.ActiveLinkColor = Color.FromArgb(97, 209, 125);
            linkLabel1.AutoSize = true;
            linkLabel1.Font = new Font("Ebrima", 8.25F);
            linkLabel1.ForeColor = Color.FromArgb(150, 255, 255, 255);
            linkLabel1.LinkColor = Color.FromArgb(80, 255, 255, 255);
            linkLabel1.Location = new Point(379, 592);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(42, 13);
            linkLabel1.TabIndex = 22;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Credits";
            linkLabel1.LinkClicked += credit_MouseClick;
            // 
            // linkLabel2
            // 
            linkLabel2.ActiveLinkColor = Color.FromArgb(97, 209, 125);
            linkLabel2.AutoSize = true;
            linkLabel2.Font = new Font("Ebrima", 8.25F);
            linkLabel2.ForeColor = Color.FromArgb(150, 255, 255, 255);
            linkLabel2.LinkColor = Color.FromArgb(80, 255, 255, 255);
            linkLabel2.Location = new Point(442, 592);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(83, 13);
            linkLabel2.TabIndex = 22;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "Buy me a coffee";
            linkLabel2.LinkClicked += buymeacoffee;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI Variable Display Light", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label8.ForeColor = SystemColors.ButtonFace;
            label8.Location = new Point(427, 589);
            label8.Name = "label8";
            label8.Size = new Size(9, 16);
            label8.TabIndex = 21;
            label8.Text = "|";
            // 
            // linkLabel3
            // 
            linkLabel3.ActiveLinkColor = Color.FromArgb(97, 209, 125);
            linkLabel3.AutoSize = true;
            linkLabel3.Font = new Font("Ebrima", 8.25F);
            linkLabel3.ForeColor = Color.FromArgb(150, 255, 255, 255);
            linkLabel3.LinkColor = Color.FromArgb(80, 255, 255, 255);
            linkLabel3.Location = new Point(545, 592);
            linkLabel3.Name = "linkLabel3";
            linkLabel3.Size = new Size(47, 13);
            linkLabel3.TabIndex = 22;
            linkLabel3.TabStop = true;
            linkLabel3.Text = "Settings";
            linkLabel3.LinkClicked += settings;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI Variable Display Light", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label9.ForeColor = SystemColors.ButtonFace;
            label9.Location = new Point(530, 589);
            label9.Name = "label9";
            label9.Size = new Size(9, 16);
            label9.TabIndex = 21;
            label9.Text = "|";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI Variable Display Light", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label11.ForeColor = SystemColors.ButtonFace;
            label11.Location = new Point(748, 589);
            label11.Name = "label11";
            label11.Size = new Size(9, 16);
            label11.TabIndex = 18;
            label11.Text = "|";
            label11.Click += easteregg;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Ebrima", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.ButtonFace;
            label3.Location = new Point(763, 592);
            label3.Name = "label3";
            label3.Size = new Size(70, 13);
            label3.TabIndex = 18;
            label3.Text = "Game Hours:";
            label3.Click += labelHours_Click;
            // 
            // labelHours
            // 
            labelHours.AutoSize = true;
            labelHours.Font = new Font("Ebrima", 8.25F);
            labelHours.ForeColor = SystemColors.ButtonFace;
            labelHours.Location = new Point(830, 592);
            labelHours.Name = "labelHours";
            labelHours.Size = new Size(25, 13);
            labelHours.TabIndex = 18;
            labelHours.Text = "XXX";
            labelHours.Click += labelHours_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI Variable Display Light", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label10.ForeColor = SystemColors.ButtonFace;
            label10.Location = new Point(598, 590);
            label10.Name = "label10";
            label10.Size = new Size(9, 16);
            label10.TabIndex = 21;
            label10.Text = "|";
            // 
            // linkLabel4
            // 
            linkLabel4.ActiveLinkColor = Color.FromArgb(97, 209, 125);
            linkLabel4.AutoSize = true;
            linkLabel4.Font = new Font("Ebrima", 8.25F);
            linkLabel4.ForeColor = Color.FromArgb(150, 255, 255, 255);
            linkLabel4.LinkColor = Color.FromArgb(80, 255, 255, 255);
            linkLabel4.Location = new Point(613, 592);
            linkLabel4.Name = "linkLabel4";
            linkLabel4.Size = new Size(74, 13);
            linkLabel4.TabIndex = 22;
            linkLabel4.TabStop = true;
            linkLabel4.Text = "Boot-Shortcut";
            linkLabel4.Click += boot_Clicked;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI Variable Display Light", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label12.ForeColor = SystemColors.ButtonFace;
            label12.Location = new Point(693, 590);
            label12.Name = "label12";
            label12.Size = new Size(9, 16);
            label12.TabIndex = 21;
            label12.Text = "|";
            // 
            // linkLabel5
            // 
            linkLabel5.ActiveLinkColor = Color.FromArgb(97, 209, 125);
            linkLabel5.AutoSize = true;
            linkLabel5.Font = new Font("Ebrima", 8.25F);
            linkLabel5.ForeColor = Color.FromArgb(150, 255, 255, 255);
            linkLabel5.LinkColor = Color.FromArgb(80, 255, 255, 255);
            linkLabel5.Location = new Point(708, 592);
            linkLabel5.Name = "linkLabel5";
            linkLabel5.Size = new Size(34, 13);
            linkLabel5.TabIndex = 22;
            linkLabel5.TabStop = true;
            linkLabel5.Text = "Mods";
            linkLabel5.Click += mods_Clicked;
            // 
            // info1
            // 
            info1.AutoSize = true;
            info1.ForeColor = SystemColors.ButtonFace;
            info1.Location = new Point(305, 70);
            info1.Name = "info1";
            info1.Size = new Size(17, 15);
            info1.TabIndex = 23;
            info1.Text = "🛈";
            info1.Click += info1_Click;
            // 
            // info2
            // 
            info2.AutoSize = true;
            info2.ForeColor = SystemColors.ButtonFace;
            info2.Location = new Point(683, 70);
            info2.Name = "info2";
            info2.Size = new Size(17, 15);
            info2.TabIndex = 23;
            info2.Text = "🛈";
            info2.Click += info2_Click;
            // 
            // info7
            // 
            info7.AutoSize = true;
            info7.BackColor = Color.Transparent;
            info7.ForeColor = SystemColors.ButtonFace;
            info7.Location = new Point(765, 577);
            info7.Name = "info7";
            info7.Size = new Size(17, 15);
            info7.TabIndex = 23;
            info7.Text = "🛈";
            info7.Click += info7_Click;
            // 
            // info8
            // 
            info8.AutoSize = true;
            info8.BackColor = Color.Transparent;
            info8.ForeColor = SystemColors.ButtonFace;
            info8.Location = new Point(711, 577);
            info8.Name = "info8";
            info8.Size = new Size(17, 15);
            info8.TabIndex = 23;
            info8.Text = "🛈";
            info8.Click += info8_Click;
            // 
            // info9
            // 
            info9.AutoSize = true;
            info9.BackColor = Color.Transparent;
            info9.ForeColor = SystemColors.ButtonFace;
            info9.Location = new Point(613, 577);
            info9.Name = "info9";
            info9.Size = new Size(17, 15);
            info9.TabIndex = 23;
            info9.Text = "🛈";
            info9.Click += info9_Click;
            // 
            // info10
            // 
            info10.AutoSize = true;
            info10.BackColor = Color.Transparent;
            info10.ForeColor = SystemColors.ButtonFace;
            info10.Location = new Point(547, 577);
            info10.Name = "info10";
            info10.Size = new Size(17, 15);
            info10.TabIndex = 23;
            info10.Text = "🛈";
            info10.Click += info10_Click;
            // 
            // info11
            // 
            info11.AutoSize = true;
            info11.BackColor = Color.Transparent;
            info11.ForeColor = SystemColors.ButtonFace;
            info11.Location = new Point(444, 577);
            info11.Name = "info11";
            info11.Size = new Size(17, 15);
            info11.TabIndex = 23;
            info11.Text = "🛈";
            info11.Click += info11_Click;
            // 
            // info12
            // 
            info12.AutoSize = true;
            info12.BackColor = Color.Transparent;
            info12.ForeColor = SystemColors.ButtonFace;
            info12.Location = new Point(382, 577);
            info12.Name = "info12";
            info12.Size = new Size(17, 15);
            info12.TabIndex = 23;
            info12.Text = "🛈";
            info12.Click += info12_Click;
            // 
            // info13
            // 
            info13.AutoSize = true;
            info13.BackColor = Color.Transparent;
            info13.ForeColor = SystemColors.ButtonFace;
            info13.Location = new Point(330, 577);
            info13.Name = "info13";
            info13.Size = new Size(17, 15);
            info13.TabIndex = 23;
            info13.Text = "🛈";
            info13.Click += info13_Click;
            // 
            // info3
            // 
            info3.AutoSize = true;
            info3.BackColor = Color.Transparent;
            info3.ForeColor = SystemColors.ButtonFace;
            info3.Location = new Point(213, 435);
            info3.Name = "info3";
            info3.Size = new Size(17, 15);
            info3.TabIndex = 23;
            info3.Text = "🛈";
            info3.Click += info3_Click;
            // 
            // info5
            // 
            info5.AutoSize = true;
            info5.BackColor = Color.Transparent;
            info5.ForeColor = SystemColors.ButtonFace;
            info5.Location = new Point(213, 482);
            info5.Name = "info5";
            info5.Size = new Size(17, 15);
            info5.TabIndex = 23;
            info5.Text = "🛈";
            info5.Click += info5_Click;
            // 
            // info6
            // 
            info6.AutoSize = true;
            info6.BackColor = Color.Transparent;
            info6.ForeColor = SystemColors.ButtonFace;
            info6.Location = new Point(436, 482);
            info6.Name = "info6";
            info6.Size = new Size(17, 15);
            info6.TabIndex = 23;
            info6.Text = "🛈";
            info6.Click += info6_Click;
            // 
            // info4
            // 
            info4.AutoSize = true;
            info4.BackColor = Color.Transparent;
            info4.ForeColor = SystemColors.ButtonFace;
            info4.Location = new Point(436, 435);
            info4.Name = "info4";
            info4.Size = new Size(17, 15);
            info4.TabIndex = 23;
            info4.Text = "🛈";
            info4.Click += info4_Click;
            // 
            // Manager
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(10, 10, 10);
            ClientSize = new Size(900, 615);
            Controls.Add(info6);
            Controls.Add(info4);
            Controls.Add(info5);
            Controls.Add(info3);
            Controls.Add(info13);
            Controls.Add(info12);
            Controls.Add(info11);
            Controls.Add(info10);
            Controls.Add(info9);
            Controls.Add(info8);
            Controls.Add(info7);
            Controls.Add(info2);
            Controls.Add(info1);
            Controls.Add(linkLabel5);
            Controls.Add(linkLabel4);
            Controls.Add(linkLabel3);
            Controls.Add(label12);
            Controls.Add(linkLabel2);
            Controls.Add(label10);
            Controls.Add(linkLabel1);
            Controls.Add(label9);
            Controls.Add(help);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(panelBtnRemoveRO);
            Controls.Add(panelBtnAdd);
            Controls.Add(panelBtnSetRO);
            Controls.Add(panelBtnRemoveChk);
            Controls.Add(panelBar);
            Controls.Add(label4);
            Controls.Add(label11);
            Controls.Add(label5);
            Controls.Add(label3);
            Controls.Add(labelHours);
            Controls.Add(versionLabel);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(panelLive);
            Controls.Add(panelProject);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Manager";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SaveManager";
            TopMost = true;
            Load += Manager_Load;
            panelProject.ResumeLayout(false);
            panelLive.ResumeLayout(false);
            panelBar.ResumeLayout(false);
            panelBtnRemoveChk.ResumeLayout(false);
            panelBtnSetRO.ResumeLayout(false);
            panelBtnAdd.ResumeLayout(false);
            panelBtnRemoveRO.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TreeView treeViewProject;
        private TreeView treeViewLive;
        private Panel panelProject;
        private Panel panelLive;
        private Label label1;
        private Label label2;
        private Label versionLabel;
        private Panel panelBar;
        private RichTextBox consoleArea;
        private Button btnRemoveChk;
        private Panel panelBtnRemoveChk;
        private Panel panelBtnSetRO;
        private Button btnSetRO;
        private Panel panelBtnAdd;
        private Button btnAdd;
        private Panel panelBtnRemoveRO;
        private Button btnRemoveRO;
        private Label label4;
        private Label label5;
        private Label label6;
        private LinkLabel help;
        private Label label7;
        private LinkLabel linkLabel1;
        private LinkLabel linkLabel2;
        private Label label8;
        private LinkLabel linkLabel3;
        private Label label9;
        private Label label11;
        private Label label3;
        private Label labelHours;
        private Label label10;
        private LinkLabel linkLabel4;
        private Label label12;
        private LinkLabel linkLabel5;
        private Label info1;
        private Label info2;
        private Label info7;
        private Label info8;
        private Label info9;
        private Label info4;
        private Label info6;
        private Label info10;
        private Label info11;
        private Label info12;
        private Label info13;
        private Label info3;
        private Label info5;
    }
}