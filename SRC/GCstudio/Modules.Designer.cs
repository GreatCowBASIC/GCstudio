namespace GC_Studio
{
    partial class Modules
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
            checkedListBoxModules = new System.Windows.Forms.CheckedListBox();
            buttonOk = new System.Windows.Forms.Button();
            buttonRem = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // checkedListBoxModules
            // 
            checkedListBoxModules.BackColor = System.Drawing.Color.Silver;
            checkedListBoxModules.FormattingEnabled = true;
            checkedListBoxModules.Location = new System.Drawing.Point(17, 20);
            checkedListBoxModules.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            checkedListBoxModules.Name = "checkedListBoxModules";
            checkedListBoxModules.Size = new System.Drawing.Size(533, 592);
            checkedListBoxModules.TabIndex = 0;
            checkedListBoxModules.ThreeDCheckBoxes = true;
            // 
            // buttonOk
            // 
            buttonOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonOk.BackColor = System.Drawing.Color.DimGray;
            buttonOk.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(25, 25, 25);
            buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonOk.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            buttonOk.ForeColor = System.Drawing.Color.White;
            buttonOk.Location = new System.Drawing.Point(417, 634);
            buttonOk.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new System.Drawing.Size(133, 50);
            buttonOk.TabIndex = 50;
            buttonOk.Text = "Ok";
            buttonOk.UseVisualStyleBackColor = false;
            buttonOk.Click += buttonOk_Click;
            // 
            // buttonRem
            // 
            buttonRem.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonRem.BackColor = System.Drawing.Color.DimGray;
            buttonRem.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(25, 25, 25);
            buttonRem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonRem.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            buttonRem.ForeColor = System.Drawing.Color.White;
            buttonRem.Location = new System.Drawing.Point(17, 634);
            buttonRem.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            buttonRem.Name = "buttonRem";
            buttonRem.Size = new System.Drawing.Size(186, 50);
            buttonRem.TabIndex = 51;
            buttonRem.Text = "Remove Selected";
            buttonRem.UseVisualStyleBackColor = false;
            buttonRem.Click += buttonRem_Click;
            // 
            // Modules
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Gray;
            ClientSize = new System.Drawing.Size(569, 700);
            Controls.Add(buttonRem);
            Controls.Add(buttonOk);
            Controls.Add(checkedListBoxModules);
            ForeColor = System.Drawing.Color.Gainsboro;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "Modules";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "GC Studio Module Packs";
            FormClosing += Modules_FormClosing;
            Load += Modules_Load;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBoxModules;
        private System.Windows.Forms.Button buttonabout;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonRem;
    }
}