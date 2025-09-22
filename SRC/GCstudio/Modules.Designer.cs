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
            buttonDeploy = new System.Windows.Forms.Button();
            buttonRem = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // checkedListBoxModules
            // 
            checkedListBoxModules.BackColor = System.Drawing.Color.Silver;
            checkedListBoxModules.FormattingEnabled = true;
            checkedListBoxModules.Location = new System.Drawing.Point(12, 12);
            checkedListBoxModules.Name = "checkedListBoxModules";
            checkedListBoxModules.Size = new System.Drawing.Size(374, 346);
            checkedListBoxModules.TabIndex = 0;
            // 
            // buttonDeploy
            // 
            buttonDeploy.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonDeploy.BackColor = System.Drawing.Color.DimGray;
            buttonDeploy.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(25, 25, 25);
            buttonDeploy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonDeploy.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            buttonDeploy.ForeColor = System.Drawing.Color.White;
            buttonDeploy.Location = new System.Drawing.Point(125, 377);
            buttonDeploy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            buttonDeploy.Name = "buttonDeploy";
            buttonDeploy.Size = new System.Drawing.Size(125, 30);
            buttonDeploy.TabIndex = 50;
            buttonDeploy.Text = "Deploy Packages";
            buttonDeploy.UseVisualStyleBackColor = false;
            // 
            // buttonRem
            // 
            buttonRem.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonRem.BackColor = System.Drawing.Color.DimGray;
            buttonRem.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(25, 25, 25);
            buttonRem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonRem.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            buttonRem.ForeColor = System.Drawing.Color.White;
            buttonRem.Location = new System.Drawing.Point(256, 377);
            buttonRem.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            buttonRem.Name = "buttonRem";
            buttonRem.Size = new System.Drawing.Size(130, 30);
            buttonRem.TabIndex = 51;
            buttonRem.Text = "Remove Packages";
            buttonRem.UseVisualStyleBackColor = false;
            // 
            // Modules
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            ClientSize = new System.Drawing.Size(398, 420);
            Controls.Add(buttonRem);
            Controls.Add(buttonDeploy);
            Controls.Add(checkedListBoxModules);
            ForeColor = System.Drawing.Color.Gainsboro;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "Modules";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Modules";
            Load += Modules_Load;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBoxModules;
        private System.Windows.Forms.Button buttonabout;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonDeploy;
        private System.Windows.Forms.Button buttonRem;
    }
}