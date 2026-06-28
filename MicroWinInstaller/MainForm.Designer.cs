namespace MicroWinInstaller
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            WelcomePage = new Panel();
            ButtonPanel = new Panel();
            TableLayoutPanel1 = new TableLayoutPanel();
            Back_Button = new Button();
            Next_Button = new Button();
            Cancel_Button = new Button();
            PageContainerPanel = new Panel();
            DriveSelectionPage = new Panel();
            TableLayoutPanel1.SuspendLayout();
            PageContainerPanel.SuspendLayout();
            WelcomePage.SuspendLayout();
            //
            // WelcomePage
            //
            WelcomePage.Name = "WelcomePage";
            WelcomePage.Dock = DockStyle.Fill;
            WelcomePage.Location = new Point(0, 0);
            WelcomePage.Size = new Size(1008, 521)
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
            Cancel_Button.Click += Cancel_Button_Click;
            //
            // PageContainerPanel
            //
            PageContainerPanel.Controls.Add(WelcomePage);
            PageContainerPanel.Dock = DockStyle.Fill;
            PageContainerPanel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PageContainerPanel.Location = new Point(0, 0);
            PageContainerPanel.Name = "PageContainerPanel";
            PageContainerPanel.Size = new Size(1008, 521);
            PageContainerPanel.TabIndex = 3;
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
            Text = "MicroWin Installer";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            SizeChanged += MainForm_SizeChanged;
            WelcomePage.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        internal System.Windows.Forms.Panel WelcomePage;
        internal System.Windows.Forms.Button Back_Button;
        internal System.Windows.Forms.Button Next_Button;
        internal System.Windows.Forms.Button Cancel_Button;
        internal System.Windows.Forms.Panel PageContainerPanel;
    }
}
