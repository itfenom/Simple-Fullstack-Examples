namespace Playground.Winforms.Forms
{
    partial class Main
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
            this.lblLoginMessage = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewWelcomeMessageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.examplesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oracleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridExToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridToExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridComboBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridColumnFiltersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileOperationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rowsInFIleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitFilesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readWriteToTextFileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadExcelFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listViewExampleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.worldWithTreeViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.encryptDecryptToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.numericTextBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topMostMessageBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exceptionHandlingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeDuplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.encodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitLargeStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redTextMessageBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.basicAsyncAwaitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.adminToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageUsersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageOnlineAccountsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewAccountHistoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedAsyncAwaitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblLoginMessage
            // 
            this.lblLoginMessage.AutoSize = true;
            this.lblLoginMessage.Location = new System.Drawing.Point(182, 132);
            this.lblLoginMessage.Name = "lblLoginMessage";
            this.lblLoginMessage.Size = new System.Drawing.Size(76, 13);
            this.lblLoginMessage.TabIndex = 4;
            this.lblLoginMessage.Text = "LoginMessage";
            this.lblLoginMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.examplesToolStripMenuItem,
            this.utilitiesToolStripMenuItem,
            this.adminToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(520, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loginToolStripMenuItem,
            this.logoutToolStripMenuItem,
            this.viewWelcomeMessageToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.fileToolStripMenuItem.Text = "&Connection";
            // 
            // loginToolStripMenuItem
            // 
            this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            this.loginToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.loginToolStripMenuItem.Text = "Login";
            this.loginToolStripMenuItem.Click += new System.EventHandler(this.LoginToolStripMenuItem_Click);
            // 
            // logoutToolStripMenuItem
            // 
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.logoutToolStripMenuItem.Text = "Logout";
            this.logoutToolStripMenuItem.Click += new System.EventHandler(this.LogoutToolStripMenuItem_Click);
            // 
            // viewWelcomeMessageToolStripMenuItem
            // 
            this.viewWelcomeMessageToolStripMenuItem.Name = "viewWelcomeMessageToolStripMenuItem";
            this.viewWelcomeMessageToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.viewWelcomeMessageToolStripMenuItem.Text = "View Welcome Message";
            this.viewWelcomeMessageToolStripMenuItem.Click += new System.EventHandler(this.ViewWelcomeMessageToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // examplesToolStripMenuItem
            // 
            this.examplesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.oracleToolStripMenuItem,
            this.dataGridExToolStripMenuItem,
            this.fileOperationToolStripMenuItem,
            this.readWriteToTextFileToolStripMenuItem1,
            this.loadExcelFileToolStripMenuItem,
            this.listViewExampleToolStripMenuItem,
            this.worldWithTreeViewToolStripMenuItem});
            this.examplesToolStripMenuItem.Name = "examplesToolStripMenuItem";
            this.examplesToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.examplesToolStripMenuItem.Text = "Examples";
            // 
            // oracleToolStripMenuItem
            // 
            this.oracleToolStripMenuItem.Name = "oracleToolStripMenuItem";
            this.oracleToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.oracleToolStripMenuItem.Text = "Oracle";
            this.oracleToolStripMenuItem.Click += new System.EventHandler(this.OracleToolStripMenuItem_Click);
            // 
            // dataGridExToolStripMenuItem
            // 
            this.dataGridExToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataGridToExcelToolStripMenuItem,
            this.dataGridComboBoxToolStripMenuItem,
            this.dataGridColumnFiltersToolStripMenuItem});
            this.dataGridExToolStripMenuItem.Name = "dataGridExToolStripMenuItem";
            this.dataGridExToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.dataGridExToolStripMenuItem.Text = "DataGridEx";
            // 
            // dataGridToExcelToolStripMenuItem
            // 
            this.dataGridToExcelToolStripMenuItem.Name = "dataGridToExcelToolStripMenuItem";
            this.dataGridToExcelToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.dataGridToExcelToolStripMenuItem.Text = "DataGrid to Excel";
            this.dataGridToExcelToolStripMenuItem.Click += new System.EventHandler(this.DataGridToExcelToolStripMenuItem_Click);
            // 
            // dataGridComboBoxToolStripMenuItem
            // 
            this.dataGridComboBoxToolStripMenuItem.Name = "dataGridComboBoxToolStripMenuItem";
            this.dataGridComboBoxToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.dataGridComboBoxToolStripMenuItem.Text = "DataGrid ComboBox";
            this.dataGridComboBoxToolStripMenuItem.Click += new System.EventHandler(this.DataGridComboBoxToolStripMenuItem_Click);
            // 
            // dataGridColumnFiltersToolStripMenuItem
            // 
            this.dataGridColumnFiltersToolStripMenuItem.Name = "dataGridColumnFiltersToolStripMenuItem";
            this.dataGridColumnFiltersToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.dataGridColumnFiltersToolStripMenuItem.Text = "DataGrid Column Filters";
            this.dataGridColumnFiltersToolStripMenuItem.Click += new System.EventHandler(this.DataGridColumnFiltersToolStripMenuItem_Click);
            // 
            // fileOperationToolStripMenuItem
            // 
            this.fileOperationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createFilesToolStripMenuItem,
            this.rowsInFIleToolStripMenuItem,
            this.splitFilesToolStripMenuItem1,
            this.deleteFilesToolStripMenuItem});
            this.fileOperationToolStripMenuItem.Name = "fileOperationToolStripMenuItem";
            this.fileOperationToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.fileOperationToolStripMenuItem.Text = "File Operation";
            // 
            // createFilesToolStripMenuItem
            // 
            this.createFilesToolStripMenuItem.Name = "createFilesToolStripMenuItem";
            this.createFilesToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.createFilesToolStripMenuItem.Text = "Create Files";
            this.createFilesToolStripMenuItem.Click += new System.EventHandler(this.CreateFilesToolStripMenuItem_Click);
            // 
            // rowsInFIleToolStripMenuItem
            // 
            this.rowsInFIleToolStripMenuItem.Name = "rowsInFIleToolStripMenuItem";
            this.rowsInFIleToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.rowsInFIleToolStripMenuItem.Text = "Rows in File";
            this.rowsInFIleToolStripMenuItem.Click += new System.EventHandler(this.RowInFileToolStripMenuItem_Click);
            // 
            // splitFilesToolStripMenuItem1
            // 
            this.splitFilesToolStripMenuItem1.Name = "splitFilesToolStripMenuItem1";
            this.splitFilesToolStripMenuItem1.Size = new System.Drawing.Size(136, 22);
            this.splitFilesToolStripMenuItem1.Text = "Split Files";
            this.splitFilesToolStripMenuItem1.Click += new System.EventHandler(this.SplitFilesToolStripMenuItem_Click);
            // 
            // deleteFilesToolStripMenuItem
            // 
            this.deleteFilesToolStripMenuItem.Name = "deleteFilesToolStripMenuItem";
            this.deleteFilesToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.deleteFilesToolStripMenuItem.Text = "Delete Files";
            this.deleteFilesToolStripMenuItem.Click += new System.EventHandler(this.DeleteSplitFilesToolStripMenuItem_Click);
            // 
            // readWriteToTextFileToolStripMenuItem1
            // 
            this.readWriteToTextFileToolStripMenuItem1.Name = "readWriteToTextFileToolStripMenuItem1";
            this.readWriteToTextFileToolStripMenuItem1.Size = new System.Drawing.Size(187, 22);
            this.readWriteToTextFileToolStripMenuItem1.Text = "Read Write to TextFile";
            this.readWriteToTextFileToolStripMenuItem1.Click += new System.EventHandler(this.ReadWriteToTextFileToolStripMenuItem_Click);
            // 
            // loadExcelFileToolStripMenuItem
            // 
            this.loadExcelFileToolStripMenuItem.Name = "loadExcelFileToolStripMenuItem";
            this.loadExcelFileToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.loadExcelFileToolStripMenuItem.Text = "Load Excel File";
            this.loadExcelFileToolStripMenuItem.Click += new System.EventHandler(this.LoadExcelFileToolStripMenuItem_Click);
            // 
            // listViewExampleToolStripMenuItem
            // 
            this.listViewExampleToolStripMenuItem.Name = "listViewExampleToolStripMenuItem";
            this.listViewExampleToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.listViewExampleToolStripMenuItem.Text = "ListView example";
            this.listViewExampleToolStripMenuItem.Click += new System.EventHandler(this.ListViewExampleToolStripMenuItem_Click);
            // 
            // worldWithTreeViewToolStripMenuItem
            // 
            this.worldWithTreeViewToolStripMenuItem.Name = "worldWithTreeViewToolStripMenuItem";
            this.worldWithTreeViewToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.worldWithTreeViewToolStripMenuItem.Text = "World with TreeView";
            this.worldWithTreeViewToolStripMenuItem.Click += new System.EventHandler(this.WorldWithTreeViewToolStripMenuItem_Click);
            // 
            // utilitiesToolStripMenuItem
            // 
            this.utilitiesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.encryptDecryptToolStripMenuItem1,
            this.numericTextBoxToolStripMenuItem,
            this.topMostMessageBoxToolStripMenuItem,
            this.exceptionHandlingToolStripMenuItem,
            this.removeDuplicateToolStripMenuItem,
            this.encodeToolStripMenuItem,
            this.decodeToolStripMenuItem,
            this.splitLargeStringToolStripMenuItem,
            this.redTextMessageBoxToolStripMenuItem,
            this.basicAsyncAwaitToolStripMenuItem1,
            this.advancedAsyncAwaitToolStripMenuItem});
            this.utilitiesToolStripMenuItem.Name = "utilitiesToolStripMenuItem";
            this.utilitiesToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.utilitiesToolStripMenuItem.Text = "&Misc";
            // 
            // encryptDecryptToolStripMenuItem1
            // 
            this.encryptDecryptToolStripMenuItem1.Name = "encryptDecryptToolStripMenuItem1";
            this.encryptDecryptToolStripMenuItem1.Size = new System.Drawing.Size(195, 22);
            this.encryptDecryptToolStripMenuItem1.Text = "Encrypt Decrypt";
            this.encryptDecryptToolStripMenuItem1.Click += new System.EventHandler(this.EncryptDecryptToolStripMenuItem_Click);
            // 
            // numericTextBoxToolStripMenuItem
            // 
            this.numericTextBoxToolStripMenuItem.Name = "numericTextBoxToolStripMenuItem";
            this.numericTextBoxToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.numericTextBoxToolStripMenuItem.Text = "Numeric TextBox";
            this.numericTextBoxToolStripMenuItem.Click += new System.EventHandler(this.NumericTextBoxToolStripMenuItem_Click);
            // 
            // topMostMessageBoxToolStripMenuItem
            // 
            this.topMostMessageBoxToolStripMenuItem.Name = "topMostMessageBoxToolStripMenuItem";
            this.topMostMessageBoxToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.topMostMessageBoxToolStripMenuItem.Text = "TopMost MessageBox";
            this.topMostMessageBoxToolStripMenuItem.Click += new System.EventHandler(this.TopMostMessageBoxToolStripMenuItem_Click);
            // 
            // exceptionHandlingToolStripMenuItem
            // 
            this.exceptionHandlingToolStripMenuItem.Name = "exceptionHandlingToolStripMenuItem";
            this.exceptionHandlingToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.exceptionHandlingToolStripMenuItem.Text = "ExceptionHandling";
            this.exceptionHandlingToolStripMenuItem.Click += new System.EventHandler(this.ExceptionHandlingToolStripMenuItem_Click);
            // 
            // removeDuplicateToolStripMenuItem
            // 
            this.removeDuplicateToolStripMenuItem.Name = "removeDuplicateToolStripMenuItem";
            this.removeDuplicateToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.removeDuplicateToolStripMenuItem.Text = "RemoveDuplicates";
            this.removeDuplicateToolStripMenuItem.Click += new System.EventHandler(this.RemoveDuplicateToolStripMenuItem_Click);
            // 
            // encodeToolStripMenuItem
            // 
            this.encodeToolStripMenuItem.Name = "encodeToolStripMenuItem";
            this.encodeToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.encodeToolStripMenuItem.Text = "Encode";
            this.encodeToolStripMenuItem.Click += new System.EventHandler(this.EncodeToolStripMenuItem_Click);
            // 
            // decodeToolStripMenuItem
            // 
            this.decodeToolStripMenuItem.Name = "decodeToolStripMenuItem";
            this.decodeToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.decodeToolStripMenuItem.Text = "Decode";
            this.decodeToolStripMenuItem.Click += new System.EventHandler(this.DecodeToolStripMenuItem_Click);
            // 
            // splitLargeStringToolStripMenuItem
            // 
            this.splitLargeStringToolStripMenuItem.Name = "splitLargeStringToolStripMenuItem";
            this.splitLargeStringToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.splitLargeStringToolStripMenuItem.Text = "Split Large String";
            this.splitLargeStringToolStripMenuItem.Click += new System.EventHandler(this.SplitLargeStringToolStripMenuItem_Click);
            // 
            // redTextMessageBoxToolStripMenuItem
            // 
            this.redTextMessageBoxToolStripMenuItem.Name = "redTextMessageBoxToolStripMenuItem";
            this.redTextMessageBoxToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.redTextMessageBoxToolStripMenuItem.Text = "Red Text MessageBox";
            this.redTextMessageBoxToolStripMenuItem.Click += new System.EventHandler(this.RedTextMessageBoxToolStripMenuItem_Click);
            // 
            // basicAsyncAwaitToolStripMenuItem1
            // 
            this.basicAsyncAwaitToolStripMenuItem1.Name = "basicAsyncAwaitToolStripMenuItem1";
            this.basicAsyncAwaitToolStripMenuItem1.Size = new System.Drawing.Size(195, 22);
            this.basicAsyncAwaitToolStripMenuItem1.Text = "Basic Async Await";
            this.basicAsyncAwaitToolStripMenuItem1.Click += new System.EventHandler(this.basicAsyncAwaitToolStripMenuItem1_Click);
            // 
            // adminToolStripMenuItem
            // 
            this.adminToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manageUsersToolStripMenuItem,
            this.manageOnlineAccountsToolStripMenuItem,
            this.viewAccountHistoryToolStripMenuItem});
            this.adminToolStripMenuItem.Name = "adminToolStripMenuItem";
            this.adminToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.adminToolStripMenuItem.Text = "&Admin";
            // 
            // manageUsersToolStripMenuItem
            // 
            this.manageUsersToolStripMenuItem.Name = "manageUsersToolStripMenuItem";
            this.manageUsersToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.manageUsersToolStripMenuItem.Text = "Manage Users";
            this.manageUsersToolStripMenuItem.Click += new System.EventHandler(this.ManageUsersToolStripMenuItem_Click);
            // 
            // manageOnlineAccountsToolStripMenuItem
            // 
            this.manageOnlineAccountsToolStripMenuItem.Name = "manageOnlineAccountsToolStripMenuItem";
            this.manageOnlineAccountsToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.manageOnlineAccountsToolStripMenuItem.Text = "Manage Online Accounts";
            this.manageOnlineAccountsToolStripMenuItem.Click += new System.EventHandler(this.ManageOnlineAccountsToolStripMenuItem_Click);
            // 
            // viewAccountHistoryToolStripMenuItem
            // 
            this.viewAccountHistoryToolStripMenuItem.Name = "viewAccountHistoryToolStripMenuItem";
            this.viewAccountHistoryToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.viewAccountHistoryToolStripMenuItem.Text = "View Account History";
            this.viewAccountHistoryToolStripMenuItem.Click += new System.EventHandler(this.ViewAccountHistoryToolStripMenuItem_Click);
            // 
            // advancedAsyncAwaitToolStripMenuItem
            // 
            this.advancedAsyncAwaitToolStripMenuItem.Name = "advancedAsyncAwaitToolStripMenuItem";
            this.advancedAsyncAwaitToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.advancedAsyncAwaitToolStripMenuItem.Text = "Advanced Async Await";
            this.advancedAsyncAwaitToolStripMenuItem.Click += new System.EventHandler(this.advancedAsyncAwaitToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 290);
            this.Controls.Add(this.lblLoginMessage);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Main";
            this.Text = "Main";
            this.Load += new System.EventHandler(this.Main_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.Label lblLoginMessage;
        private System.Windows.Forms.ToolStripMenuItem viewWelcomeMessageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utilitiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem numericTextBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topMostMessageBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exceptionHandlingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeDuplicateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem encodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem splitLargeStringToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redTextMessageBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem examplesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oracleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataGridExToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataGridToExcelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataGridComboBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataGridColumnFiltersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileOperationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rowsInFIleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem splitFilesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readWriteToTextFileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem encryptDecryptToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem loadExcelFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listViewExampleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem worldWithTreeViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem adminToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageUsersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageOnlineAccountsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewAccountHistoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem basicAsyncAwaitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem advancedAsyncAwaitToolStripMenuItem;
    }
}