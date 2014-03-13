namespace MSBNpcCreator
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
            this.fieldPath = new System.Windows.Forms.TextBox();
            this.btBrowse = new System.Windows.Forms.Button();
            this.btStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // fieldPath
            // 
            this.fieldPath.Enabled = false;
            this.fieldPath.Location = new System.Drawing.Point(12, 12);
            this.fieldPath.Name = "fieldPath";
            this.fieldPath.Size = new System.Drawing.Size(268, 20);
            this.fieldPath.TabIndex = 0;
            // 
            // btBrowse
            // 
            this.btBrowse.Location = new System.Drawing.Point(286, 10);
            this.btBrowse.Name = "btBrowse";
            this.btBrowse.Size = new System.Drawing.Size(24, 23);
            this.btBrowse.TabIndex = 1;
            this.btBrowse.Text = "...";
            this.btBrowse.UseVisualStyleBackColor = true;
            this.btBrowse.Click += new System.EventHandler(this.btBrowse_OnClick);
            // 
            // btStart
            // 
            this.btStart.Location = new System.Drawing.Point(12, 38);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(298, 36);
            this.btStart.TabIndex = 2;
            this.btStart.Text = "Create NPC(s)";
            this.btStart.UseVisualStyleBackColor = true;
            this.btStart.Click += new System.EventHandler(this.btStart_OnClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 86);
            this.Controls.Add(this.btStart);
            this.Controls.Add(this.btBrowse);
            this.Controls.Add(this.fieldPath);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox fieldPath;
        private System.Windows.Forms.Button btBrowse;
        private System.Windows.Forms.Button btStart;

    }
}

