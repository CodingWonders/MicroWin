namespace MicroWin
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            ButtonPanel = new Panel();
            TableLayoutPanel1 = new TableLayoutPanel();
            Back_Button = new Button();
            Next_Button = new Button();
            Cancel_Button = new Button();
            PageContainerPanel = new Panel();
            FinishPage = new Panel();
            lnkViewCreationLogs = new LinkLabel();
            lnkOpenIsoLoc = new LinkLabel();
            panel4 = new Panel();
            lnkUseNtLite = new LinkLabel();
            lnkUseDT = new LinkLabel();
            label18 = new Label();
            label16 = new Label();
            label17 = new Label();
            IsoCreationPage = new Panel();
            pnlProgress = new Panel();
            pbOverall = new ProgressBar();
            pbCurrent = new ProgressBar();
            lblOverallStatus = new Label();
            lblCurrentStatus = new Label();
            logTB = new TextBox();
            label14 = new Label();
            label15 = new Label();
            IsoSettingsPage = new Panel();
            DriverExportCombo = new ComboBox();
            label13 = new Label();
            UnattendCopyCB = new CheckBox();
            ReportToolCB = new CheckBox();
            label11 = new Label();
            label12 = new Label();
            UserAccountsPage = new Panel();
            panel1 = new Panel();
            tableLayoutPanel3 = new TableLayoutPanel();
            panel3 = new Panel();
            label10 = new Label();
            lnkLusrMgr = new LinkLabel();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            panel2 = new Panel();
            label9 = new Label();
            lnkImmersiveAccounts = new LinkLabel();
            label8 = new Label();
            b64CB = new CheckBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            label6 = new Label();
            label7 = new Label();
            usrNameTB = new TextBox();
            usrPasswordTB = new TextBox();
            usrNameCurrentSysNameBtn = new Button();
            usrPasswordRevealCB = new CheckBox();
            label5 = new Label();
            label4 = new Label();
            ImageChooserPage = new Panel();
            label2 = new Label();
            lvVersions = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            label3 = new Label();
            IsoChooserPage = new Panel();
            isoExtractionPB = new ProgressBar();
            isoPickerBtn = new Button();
            isoPathTB = new TextBox();
            lblExtractionStatus = new Label();
            label1 = new Label();
            SysCheckPage_Description = new Label();
            SysCheckPage_Header = new Label();
            WelcomePage = new Panel();
            lblDisclaimer = new Label();
            WelcomePage_Description = new Label();
            WelcomePage_Header = new Label();
            isoPickerOFD = new OpenFileDialog();
            isoSaverSFD = new SaveFileDialog();
            ButtonPanel.SuspendLayout();
            TableLayoutPanel1.SuspendLayout();
            PageContainerPanel.SuspendLayout();
            FinishPage.SuspendLayout();
            panel4.SuspendLayout();
            IsoCreationPage.SuspendLayout();
            pnlProgress.SuspendLayout();
            IsoSettingsPage.SuspendLayout();
            UserAccountsPage.SuspendLayout();
            panel1.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            panel2.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ImageChooserPage.SuspendLayout();
            IsoChooserPage.SuspendLayout();
            WelcomePage.SuspendLayout();
            SuspendLayout();
            // 
            // ButtonPanel
            // 
            ButtonPanel.Controls.Add(TableLayoutPanel1);
            ButtonPanel.Dock = DockStyle.Bottom;
            ButtonPanel.Location = new Point(0, 521);
            ButtonPanel.Name = "ButtonPanel";
            ButtonPanel.Size = new Size(1008, 40);
            ButtonPanel.TabIndex = 1;
            // 
            // TableLayoutPanel1
            // 
            TableLayoutPanel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            TableLayoutPanel1.ColumnCount = 3;
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            TableLayoutPanel1.Controls.Add(Back_Button, 0, 0);
            TableLayoutPanel1.Controls.Add(Next_Button, 1, 0);
            TableLayoutPanel1.Controls.Add(Cancel_Button, 2, 0);
            TableLayoutPanel1.Location = new Point(777, 6);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.RowCount = 1;
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            TableLayoutPanel1.Size = new Size(219, 29);
            TableLayoutPanel1.TabIndex = 1;
            // 
            // Back_Button
            // 
            Back_Button.Anchor = AnchorStyles.None;
            Back_Button.Enabled = false;
            Back_Button.FlatStyle = FlatStyle.System;
            Back_Button.Location = new Point(4, 3);
            Back_Button.Name = "Back_Button";
            Back_Button.Size = new Size(64, 23);
            Back_Button.TabIndex = 0;
            Back_Button.Text = "Back";
            Back_Button.Click += Back_Button_Click;
            // 
            // Next_Button
            // 
            Next_Button.Anchor = AnchorStyles.None;
            Next_Button.DialogResult = DialogResult.Cancel;
            Next_Button.Enabled = false;
            Next_Button.FlatStyle = FlatStyle.System;
            Next_Button.Location = new Point(77, 3);
            Next_Button.Name = "Next_Button";
            Next_Button.Size = new Size(64, 23);
            Next_Button.TabIndex = 1;
            Next_Button.Text = "Next";
            Next_Button.Click += Next_Button_Click;
            // 
            // Cancel_Button
            // 
            Cancel_Button.Anchor = AnchorStyles.None;
            Cancel_Button.DialogResult = DialogResult.Cancel;
            Cancel_Button.FlatStyle = FlatStyle.System;
            Cancel_Button.Location = new Point(150, 3);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.Size = new Size(64, 23);
            Cancel_Button.TabIndex = 1;
            Cancel_Button.Text = "Cancel";
            // 
            // PageContainerPanel
            // 
            PageContainerPanel.Controls.Add(FinishPage);
            PageContainerPanel.Controls.Add(IsoCreationPage);
            PageContainerPanel.Controls.Add(IsoSettingsPage);
            PageContainerPanel.Controls.Add(UserAccountsPage);
            PageContainerPanel.Controls.Add(ImageChooserPage);
            PageContainerPanel.Controls.Add(IsoChooserPage);
            PageContainerPanel.Controls.Add(WelcomePage);
            PageContainerPanel.Dock = DockStyle.Fill;
            PageContainerPanel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PageContainerPanel.Location = new Point(0, 0);
            PageContainerPanel.Name = "PageContainerPanel";
            PageContainerPanel.Size = new Size(1008, 521);
            PageContainerPanel.TabIndex = 3;
            // 
            // FinishPage
            // 
            FinishPage.Controls.Add(lnkViewCreationLogs);
            FinishPage.Controls.Add(lnkOpenIsoLoc);
            FinishPage.Controls.Add(panel4);
            FinishPage.Controls.Add(label16);
            FinishPage.Controls.Add(label17);
            FinishPage.Dock = DockStyle.Fill;
            FinishPage.Location = new Point(0, 0);
            FinishPage.Name = "FinishPage";
            FinishPage.Size = new Size(1008, 521);
            FinishPage.TabIndex = 8;
            // 
            // lnkViewCreationLogs
            // 
            lnkViewCreationLogs.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lnkViewCreationLogs.AutoSize = true;
            lnkViewCreationLogs.LinkBehavior = LinkBehavior.NeverUnderline;
            lnkViewCreationLogs.LinkColor = Color.DodgerBlue;
            lnkViewCreationLogs.Location = new Point(99, 441);
            lnkViewCreationLogs.Name = "lnkViewCreationLogs";
            lnkViewCreationLogs.Size = new Size(124, 15);
            lnkViewCreationLogs.TabIndex = 12;
            lnkViewCreationLogs.TabStop = true;
            lnkViewCreationLogs.Text = "View ISO creation logs";
            // 
            // lnkOpenIsoLoc
            // 
            lnkOpenIsoLoc.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lnkOpenIsoLoc.AutoSize = true;
            lnkOpenIsoLoc.LinkBehavior = LinkBehavior.NeverUnderline;
            lnkOpenIsoLoc.LinkColor = Color.DodgerBlue;
            lnkOpenIsoLoc.Location = new Point(99, 416);
            lnkOpenIsoLoc.Name = "lnkOpenIsoLoc";
            lnkOpenIsoLoc.Size = new Size(103, 15);
            lnkOpenIsoLoc.TabIndex = 12;
            lnkOpenIsoLoc.TabStop = true;
            lnkOpenIsoLoc.Text = "Open ISO location";
            lnkOpenIsoLoc.LinkClicked += lnkOpenIsoLoc_LinkClicked;
            // 
            // panel4
            // 
            panel4.Anchor = AnchorStyles.None;
            panel4.Controls.Add(lnkUseNtLite);
            panel4.Controls.Add(lnkUseDT);
            panel4.Controls.Add(label18);
            panel4.Location = new Point(101, 123);
            panel4.Name = "panel4";
            panel4.Size = new Size(806, 276);
            panel4.TabIndex = 11;
            // 
            // lnkUseNtLite
            // 
            lnkUseNtLite.AutoSize = true;
            lnkUseNtLite.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lnkUseNtLite.LinkBehavior = LinkBehavior.NeverUnderline;
            lnkUseNtLite.LinkColor = Color.DodgerBlue;
            lnkUseNtLite.Location = new Point(101, 193);
            lnkUseNtLite.Name = "lnkUseNtLite";
            lnkUseNtLite.Size = new Size(55, 21);
            lnkUseNtLite.TabIndex = 11;
            lnkUseNtLite.TabStop = true;
            lnkUseNtLite.Text = "NTLite";
            lnkUseNtLite.LinkClicked += lnkUseNtLite_LinkClicked;
            // 
            // lnkUseDT
            // 
            lnkUseDT.AutoSize = true;
            lnkUseDT.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lnkUseDT.LinkBehavior = LinkBehavior.NeverUnderline;
            lnkUseDT.LinkColor = Color.DodgerBlue;
            lnkUseDT.Location = new Point(101, 160);
            lnkUseDT.Name = "lnkUseDT";
            lnkUseDT.Size = new Size(83, 21);
            lnkUseDT.TabIndex = 11;
            lnkUseDT.TabStop = true;
            lnkUseDT.Text = "DISMTools";
            lnkUseDT.LinkClicked += lnkUseDT_LinkClicked;
            // 
            // label18
            // 
            label18.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label18.AutoEllipsis = true;
            label18.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label18.Location = new Point(51, 31);
            label18.Name = "label18";
            label18.Size = new Size(704, 95);
            label18.TabIndex = 10;
            label18.Text = resources.GetString("label18.Text");
            label18.TextAlign = ContentAlignment.TopCenter;
            // 
            // label16
            // 
            label16.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label16.AutoEllipsis = true;
            label16.Location = new Point(17, 64);
            label16.Name = "label16";
            label16.Size = new Size(977, 52);
            label16.TabIndex = 10;
            label16.Text = "Your ISO file is now ready for operating system installation.";
            // 
            // label17
            // 
            label17.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label17.AutoEllipsis = true;
            label17.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label17.Location = new Point(14, 12);
            label17.Name = "label17";
            label17.Size = new Size(980, 45);
            label17.TabIndex = 9;
            label17.Text = "Customizations complete";
            // 
            // IsoCreationPage
            // 
            IsoCreationPage.Controls.Add(pnlProgress);
            IsoCreationPage.Controls.Add(logTB);
            IsoCreationPage.Controls.Add(label14);
            IsoCreationPage.Controls.Add(label15);
            IsoCreationPage.Dock = DockStyle.Fill;
            IsoCreationPage.Location = new Point(0, 0);
            IsoCreationPage.Name = "IsoCreationPage";
            IsoCreationPage.Size = new Size(1008, 521);
            IsoCreationPage.TabIndex = 7;
            // 
            // pnlProgress
            // 
            pnlProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlProgress.Controls.Add(pbOverall);
            pnlProgress.Controls.Add(pbCurrent);
            pnlProgress.Controls.Add(lblOverallStatus);
            pnlProgress.Controls.Add(lblCurrentStatus);
            pnlProgress.Location = new Point(19, 405);
            pnlProgress.Name = "pnlProgress";
            pnlProgress.Size = new Size(971, 110);
            pnlProgress.TabIndex = 10;
            // 
            // pbOverall
            // 
            pbOverall.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbOverall.Location = new Point(14, 77);
            pbOverall.Name = "pbOverall";
            pbOverall.Size = new Size(941, 23);
            pbOverall.TabIndex = 1;
            // 
            // pbCurrent
            // 
            pbCurrent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pbCurrent.Location = new Point(14, 30);
            pbCurrent.Name = "pbCurrent";
            pbCurrent.Size = new Size(941, 23);
            pbCurrent.TabIndex = 1;
            // 
            // lblOverallStatus
            // 
            lblOverallStatus.AutoSize = true;
            lblOverallStatus.Location = new Point(11, 58);
            lblOverallStatus.Name = "lblOverallStatus";
            lblOverallStatus.Size = new Size(95, 15);
            lblOverallStatus.TabIndex = 0;
            lblOverallStatus.Text = "Overall Progress:";
            // 
            // lblCurrentStatus
            // 
            lblCurrentStatus.AutoSize = true;
            lblCurrentStatus.Location = new Point(11, 11);
            lblCurrentStatus.Name = "lblCurrentStatus";
            lblCurrentStatus.Size = new Size(98, 15);
            lblCurrentStatus.TabIndex = 0;
            lblCurrentStatus.Text = "Current Progress:";
            // 
            // logTB
            // 
            logTB.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            logTB.BorderStyle = BorderStyle.None;
            logTB.Font = new Font("Courier New", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            logTB.Location = new Point(99, 128);
            logTB.Multiline = true;
            logTB.Name = "logTB";
            logTB.ReadOnly = true;
            logTB.ScrollBars = ScrollBars.Vertical;
            logTB.Size = new Size(790, 248);
            logTB.TabIndex = 9;
            // 
            // label14
            // 
            label14.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label14.AutoEllipsis = true;
            label14.Location = new Point(17, 64);
            label14.Name = "label14";
            label14.Size = new Size(977, 52);
            label14.TabIndex = 8;
            label14.Text = "This process will take several minutes; please be patient.";
            // 
            // label15
            // 
            label15.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label15.AutoEllipsis = true;
            label15.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label15.Location = new Point(14, 12);
            label15.Name = "label15";
            label15.Size = new Size(980, 45);
            label15.TabIndex = 7;
            label15.Text = "Customizations in progress";
            // 
            // IsoSettingsPage
            // 
            IsoSettingsPage.Controls.Add(DriverExportCombo);
            IsoSettingsPage.Controls.Add(label13);
            IsoSettingsPage.Controls.Add(UnattendCopyCB);
            IsoSettingsPage.Controls.Add(ReportToolCB);
            IsoSettingsPage.Controls.Add(label11);
            IsoSettingsPage.Controls.Add(label12);
            IsoSettingsPage.Dock = DockStyle.Fill;
            IsoSettingsPage.Location = new Point(0, 0);
            IsoSettingsPage.Name = "IsoSettingsPage";
            IsoSettingsPage.Size = new Size(1008, 521);
            IsoSettingsPage.TabIndex = 6;
            // 
            // DriverExportCombo
            // 
            DriverExportCombo.FormattingEnabled = true;
            DriverExportCombo.Items.AddRange(new object[] { "Don't export drivers", "Export essential drivers (SCSI Adapters/Storage Controllers)", "Export all drivers" });
            DriverExportCombo.Location = new Point(83, 206);
            DriverExportCombo.Name = "DriverExportCombo";
            DriverExportCombo.Size = new Size(374, 23);
            DriverExportCombo.TabIndex = 9;
            DriverExportCombo.SelectedIndexChanged += DriverExportCombo_SelectedIndexChanged;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(80, 185);
            label13.Name = "label13";
            label13.Size = new Size(111, 15);
            label13.TabIndex = 8;
            label13.Text = "Driver export mode:";
            // 
            // UnattendCopyCB
            // 
            UnattendCopyCB.AutoSize = true;
            UnattendCopyCB.Location = new Point(83, 158);
            UnattendCopyCB.Name = "UnattendCopyCB";
            UnattendCopyCB.Size = new Size(412, 19);
            UnattendCopyCB.TabIndex = 7;
            UnattendCopyCB.Text = "Make a copy of the unattended answer file that I can use on other images";
            UnattendCopyCB.UseVisualStyleBackColor = true;
            UnattendCopyCB.CheckedChanged += UnattendCopyCB_CheckedChanged;
            // 
            // ReportToolCB
            // 
            ReportToolCB.AutoSize = true;
            ReportToolCB.Checked = true;
            ReportToolCB.CheckState = CheckState.Checked;
            ReportToolCB.Location = new Point(83, 133);
            ReportToolCB.Name = "ReportToolCB";
            ReportToolCB.Size = new Size(218, 19);
            ReportToolCB.TabIndex = 7;
            ReportToolCB.Text = "Add a shortcut for the reporting tool";
            ReportToolCB.UseVisualStyleBackColor = true;
            ReportToolCB.CheckedChanged += ReportToolCB_CheckedChanged;
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label11.AutoEllipsis = true;
            label11.Location = new Point(17, 64);
            label11.Name = "label11";
            label11.Size = new Size(977, 52);
            label11.TabIndex = 6;
            label11.Text = "Configure additional settings for your customized image.";
            // 
            // label12
            // 
            label12.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label12.AutoEllipsis = true;
            label12.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label12.Location = new Point(14, 12);
            label12.Name = "label12";
            label12.Size = new Size(980, 45);
            label12.TabIndex = 5;
            label12.Text = "Specify additional settings for the image";
            // 
            // UserAccountsPage
            // 
            UserAccountsPage.Controls.Add(panel1);
            UserAccountsPage.Controls.Add(b64CB);
            UserAccountsPage.Controls.Add(tableLayoutPanel2);
            UserAccountsPage.Controls.Add(label5);
            UserAccountsPage.Controls.Add(label4);
            UserAccountsPage.Dock = DockStyle.Fill;
            UserAccountsPage.Location = new Point(0, 0);
            UserAccountsPage.Name = "UserAccountsPage";
            UserAccountsPage.Size = new Size(1008, 521);
            UserAccountsPage.TabIndex = 5;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(tableLayoutPanel3);
            panel1.Controls.Add(label8);
            panel1.Location = new Point(85, 254);
            panel1.Name = "panel1";
            panel1.Size = new Size(838, 236);
            panel1.TabIndex = 7;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 47.61337F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52.38663F));
            tableLayoutPanel3.Controls.Add(panel3, 0, 1);
            tableLayoutPanel3.Controls.Add(pictureBox1, 1, 0);
            tableLayoutPanel3.Controls.Add(pictureBox2, 1, 1);
            tableLayoutPanel3.Controls.Add(panel2, 0, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(0, 29);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 41.54589F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 58.45411F));
            tableLayoutPanel3.Size = new Size(838, 207);
            tableLayoutPanel3.TabIndex = 2;
            // 
            // panel3
            // 
            panel3.Controls.Add(label10);
            panel3.Controls.Add(lnkLusrMgr);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(3, 88);
            panel3.Name = "panel3";
            panel3.Size = new Size(393, 116);
            panel3.TabIndex = 3;
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label10.AutoEllipsis = true;
            label10.Location = new Point(8, 8);
            label10.Name = "label10";
            label10.Size = new Size(375, 62);
            label10.TabIndex = 4;
            label10.Text = "- Open Local Users and Groups, then go to Users";
            // 
            // lnkLusrMgr
            // 
            lnkLusrMgr.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lnkLusrMgr.AutoSize = true;
            lnkLusrMgr.LinkBehavior = LinkBehavior.NeverUnderline;
            lnkLusrMgr.LinkColor = Color.DodgerBlue;
            lnkLusrMgr.Location = new Point(302, 91);
            lnkLusrMgr.Name = "lnkLusrMgr";
            lnkLusrMgr.Size = new Size(81, 15);
            lnkLusrMgr.TabIndex = 0;
            lnkLusrMgr.TabStop = true;
            lnkLusrMgr.Text = "Take me there";
            lnkLusrMgr.LinkClicked += lnkLusrMgr_LinkClicked;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Image = Properties.Resources.user_creation_settings;
            pictureBox1.Location = new Point(402, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(433, 79);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Dock = DockStyle.Fill;
            pictureBox2.Image = Properties.Resources.user_creation_lusrmgr;
            pictureBox2.Location = new Point(402, 88);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(433, 116);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // panel2
            // 
            panel2.Controls.Add(label9);
            panel2.Controls.Add(lnkImmersiveAccounts);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(3, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(393, 79);
            panel2.TabIndex = 2;
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label9.AutoEllipsis = true;
            label9.Location = new Point(8, 8);
            label9.Name = "label9";
            label9.Size = new Size(293, 43);
            label9.TabIndex = 4;
            label9.Text = "- Head over to Settings > Accounts > Other Users";
            // 
            // lnkImmersiveAccounts
            // 
            lnkImmersiveAccounts.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lnkImmersiveAccounts.AutoSize = true;
            lnkImmersiveAccounts.LinkBehavior = LinkBehavior.NeverUnderline;
            lnkImmersiveAccounts.LinkColor = Color.DodgerBlue;
            lnkImmersiveAccounts.Location = new Point(302, 54);
            lnkImmersiveAccounts.Name = "lnkImmersiveAccounts";
            lnkImmersiveAccounts.Size = new Size(81, 15);
            lnkImmersiveAccounts.TabIndex = 0;
            lnkImmersiveAccounts.TabStop = true;
            lnkImmersiveAccounts.Text = "Take me there";
            lnkImmersiveAccounts.LinkClicked += lnkImmersiveAccounts_LinkClicked;
            // 
            // label8
            // 
            label8.Dock = DockStyle.Top;
            label8.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label8.Location = new Point(0, 0);
            label8.Name = "label8";
            label8.Size = new Size(838, 29);
            label8.TabIndex = 0;
            label8.Text = "To set up new accounts:";
            // 
            // b64CB
            // 
            b64CB.AutoSize = true;
            b64CB.Checked = true;
            b64CB.CheckState = CheckState.Checked;
            b64CB.Location = new Point(85, 200);
            b64CB.Name = "b64CB";
            b64CB.Size = new Size(259, 19);
            b64CB.TabIndex = 6;
            b64CB.Text = "Encode password in Base64 (recommended)";
            b64CB.UseVisualStyleBackColor = true;
            b64CB.CheckedChanged += b64CB_CheckedChanged;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 19.12799F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60.85919F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20.04773F));
            tableLayoutPanel2.Controls.Add(label6, 0, 0);
            tableLayoutPanel2.Controls.Add(label7, 0, 1);
            tableLayoutPanel2.Controls.Add(usrNameTB, 1, 0);
            tableLayoutPanel2.Controls.Add(usrPasswordTB, 1, 1);
            tableLayoutPanel2.Controls.Add(usrNameCurrentSysNameBtn, 2, 0);
            tableLayoutPanel2.Controls.Add(usrPasswordRevealCB, 2, 1);
            tableLayoutPanel2.Location = new Point(85, 133);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(838, 59);
            tableLayoutPanel2.TabIndex = 5;
            // 
            // label6
            // 
            label6.AutoEllipsis = true;
            label6.Dock = DockStyle.Fill;
            label6.Location = new Point(3, 0);
            label6.Name = "label6";
            label6.Size = new Size(154, 29);
            label6.TabIndex = 4;
            label6.Text = "User Name:";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            label7.AutoEllipsis = true;
            label7.Dock = DockStyle.Fill;
            label7.Location = new Point(3, 29);
            label7.Name = "label7";
            label7.Size = new Size(154, 30);
            label7.TabIndex = 4;
            label7.Text = "Password:";
            label7.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // usrNameTB
            // 
            usrNameTB.BorderStyle = BorderStyle.FixedSingle;
            usrNameTB.Dock = DockStyle.Fill;
            usrNameTB.Location = new Point(163, 3);
            usrNameTB.MaxLength = 20;
            usrNameTB.Name = "usrNameTB";
            usrNameTB.Size = new Size(503, 23);
            usrNameTB.TabIndex = 5;
            usrNameTB.TextChanged += usrNameTB_TextChanged;
            // 
            // usrPasswordTB
            // 
            usrPasswordTB.BorderStyle = BorderStyle.FixedSingle;
            usrPasswordTB.Dock = DockStyle.Fill;
            usrPasswordTB.Location = new Point(163, 32);
            usrPasswordTB.Name = "usrPasswordTB";
            usrPasswordTB.PasswordChar = '*';
            usrPasswordTB.Size = new Size(503, 23);
            usrPasswordTB.TabIndex = 5;
            usrPasswordTB.TextChanged += usrPasswordTB_TextChanged;
            // 
            // usrNameCurrentSysNameBtn
            // 
            usrNameCurrentSysNameBtn.Dock = DockStyle.Fill;
            usrNameCurrentSysNameBtn.FlatStyle = FlatStyle.System;
            usrNameCurrentSysNameBtn.Location = new Point(672, 3);
            usrNameCurrentSysNameBtn.Name = "usrNameCurrentSysNameBtn";
            usrNameCurrentSysNameBtn.Size = new Size(163, 23);
            usrNameCurrentSysNameBtn.TabIndex = 6;
            usrNameCurrentSysNameBtn.Text = "Use current user name";
            usrNameCurrentSysNameBtn.UseVisualStyleBackColor = true;
            usrNameCurrentSysNameBtn.Click += usrNameCurrentSysNameBtn_Click;
            // 
            // usrPasswordRevealCB
            // 
            usrPasswordRevealCB.Appearance = Appearance.Button;
            usrPasswordRevealCB.AutoSize = true;
            usrPasswordRevealCB.Dock = DockStyle.Fill;
            usrPasswordRevealCB.FlatStyle = FlatStyle.System;
            usrPasswordRevealCB.Location = new Point(672, 32);
            usrPasswordRevealCB.Name = "usrPasswordRevealCB";
            usrPasswordRevealCB.Size = new Size(163, 24);
            usrPasswordRevealCB.TabIndex = 7;
            usrPasswordRevealCB.Text = "Reveal password";
            usrPasswordRevealCB.TextAlign = ContentAlignment.MiddleCenter;
            usrPasswordRevealCB.UseVisualStyleBackColor = true;
            usrPasswordRevealCB.CheckedChanged += usrPasswordRevealCB_CheckedChanged;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label5.AutoEllipsis = true;
            label5.Location = new Point(17, 64);
            label5.Name = "label5";
            label5.Size = new Size(977, 52);
            label5.TabIndex = 4;
            label5.Text = resources.GetString("label5.Text");
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label4.AutoEllipsis = true;
            label4.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(14, 12);
            label4.Name = "label4";
            label4.Size = new Size(980, 45);
            label4.TabIndex = 3;
            label4.Text = "Who will use the computer?";
            // 
            // ImageChooserPage
            // 
            ImageChooserPage.Controls.Add(label2);
            ImageChooserPage.Controls.Add(lvVersions);
            ImageChooserPage.Controls.Add(label3);
            ImageChooserPage.Dock = DockStyle.Fill;
            ImageChooserPage.Location = new Point(0, 0);
            ImageChooserPage.Name = "ImageChooserPage";
            ImageChooserPage.Size = new Size(1008, 521);
            ImageChooserPage.TabIndex = 4;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new Point(80, 448);
            label2.Name = "label2";
            label2.Size = new Size(529, 15);
            label2.TabIndex = 4;
            label2.Text = "We have automatically picked the Pro edition for you. However, you can still select another edition.";
            // 
            // lvVersions
            // 
            lvVersions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvVersions.BorderStyle = BorderStyle.FixedSingle;
            lvVersions.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4, columnHeader5 });
            lvVersions.FullRowSelect = true;
            lvVersions.Location = new Point(80, 89);
            lvVersions.MultiSelect = false;
            lvVersions.Name = "lvVersions";
            lvVersions.Size = new Size(849, 352);
            lvVersions.TabIndex = 3;
            lvVersions.UseCompatibleStateImageBehavior = false;
            lvVersions.View = View.Details;
            lvVersions.SelectedIndexChanged += lvVersions_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "#";
            columnHeader1.TextAlign = HorizontalAlignment.Center;
            columnHeader1.Width = 32;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Name";
            columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Description";
            columnHeader3.Width = 256;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Architecture";
            columnHeader4.Width = 84;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "Last Modified";
            columnHeader5.Width = 160;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label3.AutoEllipsis = true;
            label3.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(14, 12);
            label3.Name = "label3";
            label3.Size = new Size(980, 45);
            label3.TabIndex = 2;
            label3.Text = "Choose the image to modify";
            // 
            // IsoChooserPage
            // 
            IsoChooserPage.Controls.Add(isoExtractionPB);
            IsoChooserPage.Controls.Add(isoPickerBtn);
            IsoChooserPage.Controls.Add(isoPathTB);
            IsoChooserPage.Controls.Add(lblExtractionStatus);
            IsoChooserPage.Controls.Add(label1);
            IsoChooserPage.Controls.Add(SysCheckPage_Description);
            IsoChooserPage.Controls.Add(SysCheckPage_Header);
            IsoChooserPage.Dock = DockStyle.Fill;
            IsoChooserPage.Location = new Point(0, 0);
            IsoChooserPage.Name = "IsoChooserPage";
            IsoChooserPage.Size = new Size(1008, 521);
            IsoChooserPage.TabIndex = 1;
            IsoChooserPage.Visible = false;
            // 
            // isoExtractionPB
            // 
            isoExtractionPB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            isoExtractionPB.Location = new Point(125, 219);
            isoExtractionPB.Name = "isoExtractionPB";
            isoExtractionPB.Size = new Size(719, 23);
            isoExtractionPB.TabIndex = 4;
            // 
            // isoPickerBtn
            // 
            isoPickerBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            isoPickerBtn.FlatStyle = FlatStyle.System;
            isoPickerBtn.Location = new Point(769, 146);
            isoPickerBtn.Name = "isoPickerBtn";
            isoPickerBtn.Size = new Size(75, 23);
            isoPickerBtn.TabIndex = 3;
            isoPickerBtn.Text = "Browse...";
            isoPickerBtn.UseVisualStyleBackColor = true;
            isoPickerBtn.Click += isoPickerBtn_Click;
            // 
            // isoPathTB
            // 
            isoPathTB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            isoPathTB.BorderStyle = BorderStyle.FixedSingle;
            isoPathTB.Location = new Point(125, 147);
            isoPathTB.Name = "isoPathTB";
            isoPathTB.ReadOnly = true;
            isoPathTB.Size = new Size(638, 23);
            isoPathTB.TabIndex = 2;
            isoPathTB.TextChanged += isoPathTB_TextChanged;
            // 
            // lblExtractionStatus
            // 
            lblExtractionStatus.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblExtractionStatus.AutoEllipsis = true;
            lblExtractionStatus.AutoSize = true;
            lblExtractionStatus.Location = new Point(122, 200);
            lblExtractionStatus.Name = "lblExtractionStatus";
            lblExtractionStatus.Size = new Size(243, 15);
            lblExtractionStatus.TabIndex = 1;
            lblExtractionStatus.Text = "Disc image extraction status will appear here.";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.AutoEllipsis = true;
            label1.AutoSize = true;
            label1.Location = new Point(122, 128);
            label1.Name = "label1";
            label1.Size = new Size(68, 15);
            label1.TabIndex = 1;
            label1.Text = "Disc image:";
            // 
            // SysCheckPage_Description
            // 
            SysCheckPage_Description.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SysCheckPage_Description.AutoEllipsis = true;
            SysCheckPage_Description.Location = new Point(17, 64);
            SysCheckPage_Description.Name = "SysCheckPage_Description";
            SysCheckPage_Description.Size = new Size(977, 52);
            SysCheckPage_Description.TabIndex = 1;
            SysCheckPage_Description.Text = "Please specify the ISO that you want to use with this wizard. Supported operating systems are Windows 10 and Windows 11.";
            // 
            // SysCheckPage_Header
            // 
            SysCheckPage_Header.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            SysCheckPage_Header.AutoEllipsis = true;
            SysCheckPage_Header.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SysCheckPage_Header.Location = new Point(14, 12);
            SysCheckPage_Header.Name = "SysCheckPage_Header";
            SysCheckPage_Header.Size = new Size(980, 45);
            SysCheckPage_Header.TabIndex = 0;
            SysCheckPage_Header.Text = "Choose a disc image";
            // 
            // WelcomePage
            // 
            WelcomePage.Controls.Add(lblDisclaimer);
            WelcomePage.Controls.Add(WelcomePage_Description);
            WelcomePage.Controls.Add(WelcomePage_Header);
            WelcomePage.Dock = DockStyle.Fill;
            WelcomePage.Location = new Point(0, 0);
            WelcomePage.Name = "WelcomePage";
            WelcomePage.Size = new Size(1008, 521);
            WelcomePage.TabIndex = 0;
            // 
            // lblDisclaimer
            // 
            lblDisclaimer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblDisclaimer.AutoEllipsis = true;
            lblDisclaimer.Location = new Point(119, 128);
            lblDisclaimer.Name = "lblDisclaimer";
            lblDisclaimer.Size = new Size(770, 313);
            lblDisclaimer.TabIndex = 1;
            // 
            // WelcomePage_Description
            // 
            WelcomePage_Description.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            WelcomePage_Description.AutoEllipsis = true;
            WelcomePage_Description.Location = new Point(17, 64);
            WelcomePage_Description.Name = "WelcomePage_Description";
            WelcomePage_Description.Size = new Size(977, 52);
            WelcomePage_Description.TabIndex = 1;
            WelcomePage_Description.Text = "This wizard will help you configure your Windows image. To begin, click Next.";
            // 
            // WelcomePage_Header
            // 
            WelcomePage_Header.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            WelcomePage_Header.AutoEllipsis = true;
            WelcomePage_Header.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            WelcomePage_Header.Location = new Point(14, 12);
            WelcomePage_Header.Name = "WelcomePage_Header";
            WelcomePage_Header.Size = new Size(980, 45);
            WelcomePage_Header.TabIndex = 0;
            WelcomePage_Header.Text = "Welcome";
            // 
            // isoPickerOFD
            // 
            isoPickerOFD.Filter = "ISO Files|*.iso";
            isoPickerOFD.FileOk += isoPickerOFD_FileOk;
            // 
            // isoSaverSFD
            // 
            isoSaverSFD.Filter = "ISO Files|*.iso";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1008, 561);
            Controls.Add(PageContainerPanel);
            Controls.Add(ButtonPanel);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(1024, 600);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MicroWin .NET";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            SizeChanged += MainForm_SizeChanged;
            ButtonPanel.ResumeLayout(false);
            TableLayoutPanel1.ResumeLayout(false);
            PageContainerPanel.ResumeLayout(false);
            FinishPage.ResumeLayout(false);
            FinishPage.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            IsoCreationPage.ResumeLayout(false);
            IsoCreationPage.PerformLayout();
            pnlProgress.ResumeLayout(false);
            pnlProgress.PerformLayout();
            IsoSettingsPage.ResumeLayout(false);
            IsoSettingsPage.PerformLayout();
            UserAccountsPage.ResumeLayout(false);
            UserAccountsPage.PerformLayout();
            panel1.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ImageChooserPage.ResumeLayout(false);
            ImageChooserPage.PerformLayout();
            IsoChooserPage.ResumeLayout(false);
            IsoChooserPage.PerformLayout();
            WelcomePage.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Panel ButtonPanel;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        internal System.Windows.Forms.Button Back_Button;
        internal System.Windows.Forms.Button Next_Button;
        internal System.Windows.Forms.Button Cancel_Button;
        internal System.Windows.Forms.Panel PageContainerPanel;
        internal System.Windows.Forms.Panel WelcomePage;
        internal System.Windows.Forms.Label WelcomePage_Description;
        internal System.Windows.Forms.Label WelcomePage_Header;
        internal System.Windows.Forms.Panel IsoChooserPage;
        internal System.Windows.Forms.Label SysCheckPage_Description;
        internal System.Windows.Forms.Label SysCheckPage_Header;
        internal System.Windows.Forms.Label lblDisclaimer;
        internal System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button isoPickerBtn;
        private System.Windows.Forms.TextBox isoPathTB;
        private System.Windows.Forms.ProgressBar isoExtractionPB;
        internal System.Windows.Forms.Label lblExtractionStatus;
        private System.Windows.Forms.OpenFileDialog isoPickerOFD;
        private System.Windows.Forms.Panel ImageChooserPage;
        private System.Windows.Forms.ListView lvVersions;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        internal System.Windows.Forms.Label label3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel UserAccountsPage;
        internal System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        internal System.Windows.Forms.Label label6;
        internal System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox usrNameTB;
        private System.Windows.Forms.TextBox usrPasswordTB;
        internal System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox b64CB;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Panel panel2;
        internal System.Windows.Forms.Label label9;
        private System.Windows.Forms.LinkLabel lnkImmersiveAccounts;
        private System.Windows.Forms.Panel panel3;
        internal System.Windows.Forms.Label label10;
        private System.Windows.Forms.LinkLabel lnkLusrMgr;
        private System.Windows.Forms.Button usrNameCurrentSysNameBtn;
        private System.Windows.Forms.CheckBox usrPasswordRevealCB;
        private System.Windows.Forms.Panel IsoSettingsPage;
        internal System.Windows.Forms.Label label11;
        internal System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox ReportToolCB;
        private System.Windows.Forms.ComboBox DriverExportCombo;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox UnattendCopyCB;
        private System.Windows.Forms.Panel IsoCreationPage;
        internal System.Windows.Forms.Label label14;
        internal System.Windows.Forms.Label label15;
        private System.Windows.Forms.Panel pnlProgress;
        private System.Windows.Forms.ProgressBar pbOverall;
        private System.Windows.Forms.ProgressBar pbCurrent;
        private System.Windows.Forms.Label lblOverallStatus;
        private System.Windows.Forms.Label lblCurrentStatus;
        private System.Windows.Forms.TextBox logTB;
        private System.Windows.Forms.SaveFileDialog isoSaverSFD;
        private System.Windows.Forms.Panel FinishPage;
        internal System.Windows.Forms.Label label16;
        internal System.Windows.Forms.Label label17;
        internal System.Windows.Forms.Label label18;
        private System.Windows.Forms.LinkLabel lnkViewCreationLogs;
        private System.Windows.Forms.LinkLabel lnkOpenIsoLoc;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.LinkLabel lnkUseNtLite;
        private System.Windows.Forms.LinkLabel lnkUseDT;
    }
}