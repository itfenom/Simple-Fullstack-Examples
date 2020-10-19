namespace Playground.Winforms.Forms.Examples
{
    partial class ReadAndWriteToTxtFile
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
            this.rTxtBox = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblNewValues = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.BtnInsert = new System.Windows.Forms.Button();
            this.BtnShowAll = new System.Windows.Forms.Button();
            this.BtnTotal = new System.Windows.Forms.Button();
            this.BtnExit = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // rTxtBox
            // 
            this.rTxtBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rTxtBox.Font = new System.Drawing.Font("Georgia", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rTxtBox.Location = new System.Drawing.Point(160, 3);
            this.rTxtBox.Name = "rTxtBox";
            this.rTxtBox.ReadOnly = true;
            this.rTxtBox.Size = new System.Drawing.Size(463, 277);
            this.rTxtBox.TabIndex = 13;
            this.rTxtBox.Text = "";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.11628F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 74.88372F));
            this.tableLayoutPanel1.Controls.Add(this.rTxtBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(626, 283);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.lblNewValues, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtValue, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.BtnInsert, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.BtnShowAll, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.BtnTotal, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.BtnExit, 0, 6);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 8;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.203125F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.913794F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.93103F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.10345F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.53448F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.67241F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.59227F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 11F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(151, 277);
            this.tableLayoutPanel2.TabIndex = 14;
            // 
            // lblNewValues
            // 
            this.lblNewValues.AutoSize = true;
            this.lblNewValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNewValues.Location = new System.Drawing.Point(3, 21);
            this.lblNewValues.Name = "lblNewValues";
            this.lblNewValues.Size = new System.Drawing.Size(145, 26);
            this.lblNewValues.TabIndex = 16;
            this.lblNewValues.Text = "Enter Numeric Values Only:";
            this.lblNewValues.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtValue
            // 
            this.txtValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtValue.Location = new System.Drawing.Point(3, 50);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(145, 20);
            this.txtValue.TabIndex = 17;
            // 
            // BtnInsert
            // 
            this.BtnInsert.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnInsert.Location = new System.Drawing.Point(3, 84);
            this.BtnInsert.Name = "BtnInsert";
            this.BtnInsert.Size = new System.Drawing.Size(145, 42);
            this.BtnInsert.TabIndex = 18;
            this.BtnInsert.Text = "Insert into File";
            this.BtnInsert.UseVisualStyleBackColor = true;
            this.BtnInsert.Click += new System.EventHandler(this.BtnInsert_Click);
            // 
            // BtnShowAll
            // 
            this.BtnShowAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnShowAll.Location = new System.Drawing.Point(3, 132);
            this.BtnShowAll.Name = "BtnShowAll";
            this.BtnShowAll.Size = new System.Drawing.Size(145, 43);
            this.BtnShowAll.TabIndex = 19;
            this.BtnShowAll.Text = "Show All Logs";
            this.BtnShowAll.UseVisualStyleBackColor = true;
            this.BtnShowAll.Click += new System.EventHandler(this.BtnShowAll_Click);
            // 
            // BtnTotal
            // 
            this.BtnTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnTotal.Location = new System.Drawing.Point(3, 181);
            this.BtnTotal.Name = "BtnTotal";
            this.BtnTotal.Size = new System.Drawing.Size(145, 41);
            this.BtnTotal.TabIndex = 20;
            this.BtnTotal.Text = "Show Total";
            this.BtnTotal.UseVisualStyleBackColor = true;
            this.BtnTotal.Click += new System.EventHandler(this.BtnTotal_Click);
            // 
            // BtnExit
            // 
            this.BtnExit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BtnExit.Location = new System.Drawing.Point(3, 228);
            this.BtnExit.Name = "BtnExit";
            this.BtnExit.Size = new System.Drawing.Size(145, 32);
            this.BtnExit.TabIndex = 21;
            this.BtnExit.Text = "Exit";
            this.BtnExit.UseVisualStyleBackColor = true;
            this.BtnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // ReadAndWriteToTxtFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 283);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ReadAndWriteToTxtFile";
            this.Text = "Read and Write to Text file";
            this.Load += new System.EventHandler(this.ReadAndWriteToTxtFile_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rTxtBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblNewValues;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Button BtnInsert;
        private System.Windows.Forms.Button BtnShowAll;
        private System.Windows.Forms.Button BtnTotal;
        private System.Windows.Forms.Button BtnExit;
    }
}