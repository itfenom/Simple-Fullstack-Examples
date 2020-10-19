namespace Playground.Winforms.Forms.Examples
{
    partial class DownloadUploadBlob
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
            this.btnDownloadBlob = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.numUpDownNodeID = new System.Windows.Forms.NumericUpDown();
            this.lblFilePath = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.lblUploadNodeID = new System.Windows.Forms.Label();
            this.btnUploadBlob = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownNodeID)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDownloadBlob
            // 
            this.btnDownloadBlob.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDownloadBlob.Location = new System.Drawing.Point(151, 79);
            this.btnDownloadBlob.Name = "btnDownloadBlob";
            this.btnDownloadBlob.Size = new System.Drawing.Size(263, 39);
            this.btnDownloadBlob.TabIndex = 2;
            this.btnDownloadBlob.Text = "Download Blob";
            this.btnDownloadBlob.UseVisualStyleBackColor = true;
            this.btnDownloadBlob.Click += new System.EventHandler(this.BtnDownloadBlob_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.43874F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 86.56126F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(609, 269);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24.66368F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44.61883F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.71749F));
            this.tableLayoutPanel3.Controls.Add(this.btnBrowse, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.btnDownloadBlob, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.numUpDownNodeID, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblFilePath, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.txtFilePath, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.lblUploadNodeID, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnUploadBlob, 1, 3);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 39);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 5;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.94595F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.67568F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 27.02703F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 26.35135F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(603, 227);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBrowse.Location = new System.Drawing.Point(420, 37);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(180, 36);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.BtnBrowse_Click);
            // 
            // numUpDownNodeID
            // 
            this.numUpDownNodeID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numUpDownNodeID.Location = new System.Drawing.Point(151, 3);
            this.numUpDownNodeID.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numUpDownNodeID.Name = "numUpDownNodeID";
            this.numUpDownNodeID.Size = new System.Drawing.Size(263, 20);
            this.numUpDownNodeID.TabIndex = 1;
            // 
            // lblFilePath
            // 
            this.lblFilePath.AutoSize = true;
            this.lblFilePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFilePath.Location = new System.Drawing.Point(3, 34);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new System.Drawing.Size(142, 42);
            this.lblFilePath.TabIndex = 1;
            this.lblFilePath.Text = "File Path:";
            this.lblFilePath.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtFilePath
            // 
            this.txtFilePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFilePath.Location = new System.Drawing.Point(151, 37);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(263, 20);
            this.txtFilePath.TabIndex = 2;
            // 
            // lblUploadNodeID
            // 
            this.lblUploadNodeID.AutoSize = true;
            this.lblUploadNodeID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUploadNodeID.Location = new System.Drawing.Point(3, 0);
            this.lblUploadNodeID.Name = "lblUploadNodeID";
            this.lblUploadNodeID.Size = new System.Drawing.Size(142, 34);
            this.lblUploadNodeID.TabIndex = 3;
            this.lblUploadNodeID.Text = "Node ID:";
            this.lblUploadNodeID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnUploadBlob
            // 
            this.btnUploadBlob.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnUploadBlob.Location = new System.Drawing.Point(151, 124);
            this.btnUploadBlob.Name = "btnUploadBlob";
            this.btnUploadBlob.Size = new System.Drawing.Size(263, 38);
            this.btnUploadBlob.TabIndex = 4;
            this.btnUploadBlob.Text = "Upload Blob";
            this.btnUploadBlob.UseVisualStyleBackColor = true;
            this.btnUploadBlob.Click += new System.EventHandler(this.BtnUploadBlob_Click);
            // 
            // DownloadUploadBlob
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 269);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DownloadUploadBlob";
            this.Text = "Download Upload Blob";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownNodeID)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDownloadBlob;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.NumericUpDown numUpDownNodeID;
        private System.Windows.Forms.Label lblFilePath;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Label lblUploadNodeID;
        private System.Windows.Forms.Button btnUploadBlob;
    }
}