namespace Playground.Winforms.Forms.Examples
{
    partial class OracleDb
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
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadDataFromSeraphSchemaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadDataFromHRSchemaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.retrieveAssociativeArraysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.passAssociativeArraysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oracleTimestampToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblMessage = new System.Windows.Forms.Label();
            this.downloadUploadBlobsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvData
            // 
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvData.Location = new System.Drawing.Point(3, 73);
            this.dgvData.Name = "dgvData";
            this.dgvData.RowHeadersVisible = false;
            this.dgvData.Size = new System.Drawing.Size(656, 280);
            this.dgvData.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.menuStrip1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblMessage, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.dgvData, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45.58823F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 54.41177F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 285F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(662, 356);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.actionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(662, 32);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadDataFromSeraphSchemaToolStripMenuItem,
            this.loadDataFromHRSchemaToolStripMenuItem,
            this.retrieveAssociativeArraysToolStripMenuItem,
            this.passAssociativeArraysToolStripMenuItem,
            this.oracleTimestampToolStripMenuItem,
            this.downloadUploadBlobsToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(59, 28);
            this.actionsToolStripMenuItem.Text = "&Actions";
            // 
            // loadDataFromSeraphSchemaToolStripMenuItem
            // 
            this.loadDataFromSeraphSchemaToolStripMenuItem.Name = "loadDataFromSeraphSchemaToolStripMenuItem";
            this.loadDataFromSeraphSchemaToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.loadDataFromSeraphSchemaToolStripMenuItem.Text = "Load Data From Kashif Schema";
            this.loadDataFromSeraphSchemaToolStripMenuItem.Click += new System.EventHandler(this.LoadDataFromSeraphSchemaToolStripMenuItem_Click);
            // 
            // loadDataFromHRSchemaToolStripMenuItem
            // 
            this.loadDataFromHRSchemaToolStripMenuItem.Name = "loadDataFromHRSchemaToolStripMenuItem";
            this.loadDataFromHRSchemaToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.loadDataFromHRSchemaToolStripMenuItem.Text = "Load Data From HR Schema";
            this.loadDataFromHRSchemaToolStripMenuItem.Click += new System.EventHandler(this.LoadDataFromHRSchemaToolStripMenuItem_Click);
            // 
            // retrieveAssociativeArraysToolStripMenuItem
            // 
            this.retrieveAssociativeArraysToolStripMenuItem.Name = "retrieveAssociativeArraysToolStripMenuItem";
            this.retrieveAssociativeArraysToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.retrieveAssociativeArraysToolStripMenuItem.Text = "Retrieve Associative Arrays";
            this.retrieveAssociativeArraysToolStripMenuItem.Click += new System.EventHandler(this.RetrieveAssociativeArraysToolStripMenuItem_Click);
            // 
            // passAssociativeArraysToolStripMenuItem
            // 
            this.passAssociativeArraysToolStripMenuItem.Name = "passAssociativeArraysToolStripMenuItem";
            this.passAssociativeArraysToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.passAssociativeArraysToolStripMenuItem.Text = "Pass Associative Arrays";
            this.passAssociativeArraysToolStripMenuItem.Click += new System.EventHandler(this.PassAssociativeArraysToolStripMenuItem_Click);
            // 
            // oracleTimestampToolStripMenuItem
            // 
            this.oracleTimestampToolStripMenuItem.Name = "oracleTimestampToolStripMenuItem";
            this.oracleTimestampToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.oracleTimestampToolStripMenuItem.Text = "Oracle Timestamp";
            this.oracleTimestampToolStripMenuItem.Click += new System.EventHandler(this.OracleTimestampToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Location = new System.Drawing.Point(3, 32);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(656, 38);
            this.lblMessage.TabIndex = 1;
            this.lblMessage.Text = "lblMessage";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // downloadUploadBlobsToolStripMenuItem
            // 
            this.downloadUploadBlobsToolStripMenuItem.Name = "downloadUploadBlobsToolStripMenuItem";
            this.downloadUploadBlobsToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.downloadUploadBlobsToolStripMenuItem.Text = "Download Upload Blobs";
            this.downloadUploadBlobsToolStripMenuItem.Click += new System.EventHandler(this.DownloadUploadBlobsToolStripMenuItem_Click);
            // 
            // OracleDb
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 356);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximumSize = new System.Drawing.Size(678, 395);
            this.MinimumSize = new System.Drawing.Size(678, 395);
            this.Name = "OracleDb";
            this.Text = "OracleDB";
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadDataFromSeraphSchemaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadDataFromHRSchemaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem passAssociativeArraysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.ToolStripMenuItem retrieveAssociativeArraysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oracleTimestampToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadUploadBlobsToolStripMenuItem;
    }
}