namespace InvAddIn
{
    partial class WhatWhereWhy
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
            this.SaveMe = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SaveMe
            // 
            this.SaveMe.Location = new System.Drawing.Point(12, 218);
            this.SaveMe.Name = "SaveMe";
            this.SaveMe.Size = new System.Drawing.Size(75, 23);
            this.SaveMe.TabIndex = 0;
            this.SaveMe.Text = "Save";
            this.SaveMe.UseVisualStyleBackColor = true;
            this.SaveMe.Click += new System.EventHandler(this.SaveMe_Click);
            // 
            // WhatWhereWhy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Controls.Add(this.SaveMe);
            this.Name = "WhatWhereWhy";
            this.Text = "WhatWhereWhy";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button SaveMe;
    }
}