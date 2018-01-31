namespace SamleHuffman
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSourceArray = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblEncodeArray = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbChars = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(195, 40);
            this.button1.TabIndex = 0;
            this.button1.Text = "Run encode-decode";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Source array lenght:";
            // 
            // lblSourceArray
            // 
            this.lblSourceArray.AutoSize = true;
            this.lblSourceArray.Location = new System.Drawing.Point(117, 113);
            this.lblSourceArray.Name = "lblSourceArray";
            this.lblSourceArray.Size = new System.Drawing.Size(35, 13);
            this.lblSourceArray.TabIndex = 2;
            this.lblSourceArray.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Encode array lenght:";
            // 
            // lblEncodeArray
            // 
            this.lblEncodeArray.AutoSize = true;
            this.lblEncodeArray.Location = new System.Drawing.Point(117, 143);
            this.lblEncodeArray.Name = "lblEncodeArray";
            this.lblEncodeArray.Size = new System.Drawing.Size(35, 13);
            this.lblEncodeArray.TabIndex = 4;
            this.lblEncodeArray.Text = "label4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(149, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Numbers of various character:";
            // 
            // cmbChars
            // 
            this.cmbChars.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbChars.FormattingEnabled = true;
            this.cmbChars.Location = new System.Drawing.Point(167, 71);
            this.cmbChars.Name = "cmbChars";
            this.cmbChars.Size = new System.Drawing.Size(69, 21);
            this.cmbChars.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 249);
            this.Controls.Add(this.cmbChars);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblEncodeArray);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblSourceArray);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Sample Huffman Library";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSourceArray;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblEncodeArray;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbChars;
    }
}

