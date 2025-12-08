namespace GC_Studio
{
    partial class AboutBox
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            pictureBox1 = new System.Windows.Forms.PictureBox();
            textBoxDescription = new System.Windows.Forms.TextBox();
            okButton = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            linkLabel1 = new System.Windows.Forms.LinkLabel();
            linkLabel2 = new System.Windows.Forms.LinkLabel();
            linkLabel3 = new System.Windows.Forms.LinkLabel();
            label2 = new System.Windows.Forms.Label();
            labelcomp = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            labelassembly = new System.Windows.Forms.Label();
            labelfbas = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            linkLabel4 = new System.Windows.Forms.LinkLabel();
            linkLabel5 = new System.Windows.Forms.LinkLabel();
            linkLabel6 = new System.Windows.Forms.LinkLabel();
            labelver = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            labelgccode = new System.Windows.Forms.Label();
            linkLabel7 = new System.Windows.Forms.LinkLabel();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label12 = new System.Windows.Forms.Label();
            labelinstall = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            labeltoolchain = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            button1 = new System.Windows.Forms.Button();
            label13 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.GCstudio;
            pictureBox1.Location = new System.Drawing.Point(0, 33);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(686, 400);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // textBoxDescription
            // 
            textBoxDescription.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            textBoxDescription.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F);
            textBoxDescription.ForeColor = System.Drawing.Color.White;
            textBoxDescription.Location = new System.Drawing.Point(33, 103);
            textBoxDescription.Margin = new System.Windows.Forms.Padding(10, 7, 4, 7);
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.ReadOnly = true;
            textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            textBoxDescription.Size = new System.Drawing.Size(1188, 759);
            textBoxDescription.TabIndex = 29;
            textBoxDescription.TabStop = false;
            textBoxDescription.Text = "Description";
            textBoxDescription.Visible = false;
            // 
            // okButton
            // 
            okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            okButton.BackColor = System.Drawing.Color.DimGray;
            okButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            okButton.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            okButton.ForeColor = System.Drawing.Color.White;
            okButton.Location = new System.Drawing.Point(1097, 877);
            okButton.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(124, 57);
            okButton.TabIndex = 30;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = false;
            okButton.Click += okButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft YaHei", 22F);
            label1.ForeColor = System.Drawing.Color.White;
            label1.Location = new System.Drawing.Point(239, 13);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(236, 57);
            label1.TabIndex = 31;
            label1.Text = "GC Studio";
            // 
            // linkLabel1
            // 
            linkLabel1.ActiveLinkColor = System.Drawing.Color.White;
            linkLabel1.AutoSize = true;
            linkLabel1.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            linkLabel1.LinkColor = System.Drawing.Color.CornflowerBlue;
            linkLabel1.Location = new System.Drawing.Point(43, 542);
            linkLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new System.Drawing.Size(240, 31);
            linkLabel1.TabIndex = 32;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Acknowledgements";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // linkLabel2
            // 
            linkLabel2.ActiveLinkColor = System.Drawing.Color.White;
            linkLabel2.AutoSize = true;
            linkLabel2.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            linkLabel2.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            linkLabel2.LinkColor = System.Drawing.Color.CornflowerBlue;
            linkLabel2.Location = new System.Drawing.Point(43, 598);
            linkLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new System.Drawing.Size(98, 31);
            linkLabel2.TabIndex = 33;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "License";
            linkLabel2.LinkClicked += linkLabel2_LinkClicked;
            // 
            // linkLabel3
            // 
            linkLabel3.ActiveLinkColor = System.Drawing.Color.White;
            linkLabel3.AutoSize = true;
            linkLabel3.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            linkLabel3.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            linkLabel3.LinkColor = System.Drawing.Color.CornflowerBlue;
            linkLabel3.Location = new System.Drawing.Point(43, 657);
            linkLabel3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            linkLabel3.Name = "linkLabel3";
            linkLabel3.Size = new System.Drawing.Size(169, 31);
            linkLabel3.TabIndex = 34;
            linkLabel3.TabStop = true;
            linkLabel3.Text = "Privacy Policy";
            linkLabel3.LinkClicked += linkLabel3_LinkClicked;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            label2.ForeColor = System.Drawing.Color.White;
            label2.Location = new System.Drawing.Point(27, 48);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(179, 24);
            label2.TabIndex = 35;
            label2.Text = "GC BASIC Compiler:";
            // 
            // labelcomp
            // 
            labelcomp.AutoSize = true;
            labelcomp.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            labelcomp.ForeColor = System.Drawing.Color.White;
            labelcomp.Location = new System.Drawing.Point(246, 50);
            labelcomp.Name = "labelcomp";
            labelcomp.Size = new System.Drawing.Size(34, 24);
            labelcomp.TabIndex = 36;
            labelcomp.Text = "---";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            label3.ForeColor = System.Drawing.Color.White;
            label3.Location = new System.Drawing.Point(191, 452);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(300, 31);
            label3.TabIndex = 37;
            label3.Text = "Copyright © 2007 - 2025";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            label4.ForeColor = System.Drawing.Color.White;
            label4.Location = new System.Drawing.Point(27, 83);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(188, 24);
            label4.TabIndex = 38;
            label4.Text = "GC Studio Assembly:";
            // 
            // labelassembly
            // 
            labelassembly.AutoSize = true;
            labelassembly.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            labelassembly.ForeColor = System.Drawing.Color.White;
            labelassembly.Location = new System.Drawing.Point(246, 83);
            labelassembly.Name = "labelassembly";
            labelassembly.Size = new System.Drawing.Size(34, 24);
            labelassembly.TabIndex = 39;
            labelassembly.Text = "---";
            // 
            // labelfbas
            // 
            labelfbas.AutoSize = true;
            labelfbas.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            labelfbas.ForeColor = System.Drawing.Color.White;
            labelfbas.Location = new System.Drawing.Point(246, 157);
            labelfbas.Name = "labelfbas";
            labelfbas.Size = new System.Drawing.Size(34, 24);
            labelfbas.TabIndex = 41;
            labelfbas.Text = "---";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            label6.ForeColor = System.Drawing.Color.White;
            label6.Location = new System.Drawing.Point(27, 157);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(151, 24);
            label6.TabIndex = 40;
            label6.Text = "FBasic Compiler:";
            // 
            // linkLabel4
            // 
            linkLabel4.ActiveLinkColor = System.Drawing.Color.White;
            linkLabel4.AutoSize = true;
            linkLabel4.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            linkLabel4.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            linkLabel4.LinkColor = System.Drawing.Color.CornflowerBlue;
            linkLabel4.Location = new System.Drawing.Point(43, 713);
            linkLabel4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            linkLabel4.Name = "linkLabel4";
            linkLabel4.Size = new System.Drawing.Size(198, 31);
            linkLabel4.TabIndex = 42;
            linkLabel4.TabStop = true;
            linkLabel4.Text = "Full Change Log";
            linkLabel4.LinkClicked += linkLabel4_LinkClicked;
            // 
            // linkLabel5
            // 
            linkLabel5.ActiveLinkColor = System.Drawing.Color.White;
            linkLabel5.AutoSize = true;
            linkLabel5.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            linkLabel5.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            linkLabel5.LinkColor = System.Drawing.Color.CornflowerBlue;
            linkLabel5.Location = new System.Drawing.Point(43, 772);
            linkLabel5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            linkLabel5.Name = "linkLabel5";
            linkLabel5.Size = new System.Drawing.Size(131, 31);
            linkLabel5.TabIndex = 43;
            linkLabel5.TabStop = true;
            linkLabel5.Text = "Road Map";
            linkLabel5.LinkClicked += linkLabel5_LinkClicked;
            // 
            // linkLabel6
            // 
            linkLabel6.ActiveLinkColor = System.Drawing.Color.White;
            linkLabel6.AutoSize = true;
            linkLabel6.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            linkLabel6.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            linkLabel6.LinkColor = System.Drawing.Color.CornflowerBlue;
            linkLabel6.Location = new System.Drawing.Point(43, 828);
            linkLabel6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            linkLabel6.Name = "linkLabel6";
            linkLabel6.Size = new System.Drawing.Size(164, 31);
            linkLabel6.TabIndex = 44;
            linkLabel6.TabStop = true;
            linkLabel6.Text = "Report a Bug";
            linkLabel6.LinkClicked += linkLabel6_LinkClicked;
            // 
            // labelver
            // 
            labelver.AutoSize = true;
            labelver.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            labelver.ForeColor = System.Drawing.Color.White;
            labelver.Location = new System.Drawing.Point(339, 420);
            labelver.Name = "labelver";
            labelver.Size = new System.Drawing.Size(44, 31);
            labelver.TabIndex = 46;
            labelver.Text = "---";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            label7.ForeColor = System.Drawing.Color.White;
            label7.Location = new System.Drawing.Point(239, 420);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(100, 31);
            label7.TabIndex = 45;
            label7.Text = "Version";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            label8.ForeColor = System.Drawing.Color.White;
            label8.Location = new System.Drawing.Point(27, 122);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(123, 24);
            label8.TabIndex = 47;
            label8.Text = "GC Code IDE:";
            // 
            // labelgccode
            // 
            labelgccode.AutoSize = true;
            labelgccode.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            labelgccode.ForeColor = System.Drawing.Color.White;
            labelgccode.Location = new System.Drawing.Point(246, 122);
            labelgccode.Name = "labelgccode";
            labelgccode.Size = new System.Drawing.Size(34, 24);
            labelgccode.TabIndex = 48;
            labelgccode.Text = "---";
            // 
            // linkLabel7
            // 
            linkLabel7.ActiveLinkColor = System.Drawing.Color.White;
            linkLabel7.AutoSize = true;
            linkLabel7.Font = new System.Drawing.Font("Microsoft YaHei", 12F);
            linkLabel7.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            linkLabel7.LinkColor = System.Drawing.Color.CornflowerBlue;
            linkLabel7.Location = new System.Drawing.Point(979, 35);
            linkLabel7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            linkLabel7.Name = "linkLabel7";
            linkLabel7.Size = new System.Drawing.Size(223, 31);
            linkLabel7.TabIndex = 50;
            linkLabel7.TabStop = true;
            linkLabel7.Text = "www.GCBasic.com";
            linkLabel7.LinkClicked += linkLabel7_LinkClicked;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label12);
            groupBox1.Controls.Add(labelinstall);
            groupBox1.Controls.Add(label11);
            groupBox1.Controls.Add(labeltoolchain);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(labelcomp);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(labelgccode);
            groupBox1.Controls.Add(labelassembly);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(labelfbas);
            groupBox1.ForeColor = System.Drawing.Color.White;
            groupBox1.Location = new System.Drawing.Point(733, 132);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(464, 283);
            groupBox1.TabIndex = 51;
            groupBox1.TabStop = false;
            groupBox1.Text = "Modules:";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            label12.ForeColor = System.Drawing.Color.White;
            label12.Location = new System.Drawing.Point(27, 227);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(194, 24);
            label12.TabIndex = 51;
            label12.Text = "Installation Directory:";
            // 
            // labelinstall
            // 
            labelinstall.AutoSize = true;
            labelinstall.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            labelinstall.ForeColor = System.Drawing.Color.White;
            labelinstall.Location = new System.Drawing.Point(246, 227);
            labelinstall.Name = "labelinstall";
            labelinstall.Size = new System.Drawing.Size(34, 24);
            labelinstall.TabIndex = 52;
            labelinstall.Text = "---";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            label11.ForeColor = System.Drawing.Color.White;
            label11.Location = new System.Drawing.Point(27, 192);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(154, 24);
            label11.TabIndex = 49;
            label11.Text = "Tool Chain Build:";
            // 
            // labeltoolchain
            // 
            labeltoolchain.AutoSize = true;
            labeltoolchain.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            labeltoolchain.ForeColor = System.Drawing.Color.White;
            labeltoolchain.Location = new System.Drawing.Point(246, 192);
            labeltoolchain.Name = "labeltoolchain";
            labeltoolchain.Size = new System.Drawing.Size(34, 24);
            labeltoolchain.TabIndex = 50;
            labeltoolchain.Text = "---";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Microsoft YaHei", 18F);
            label5.ForeColor = System.Drawing.Color.Silver;
            label5.Location = new System.Drawing.Point(621, 587);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(425, 46);
            label5.TabIndex = 52;
            label5.Text = "GC Studio for Windows";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            label9.ForeColor = System.Drawing.Color.DarkGray;
            label9.Location = new System.Drawing.Point(607, 657);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(460, 24);
            label9.TabIndex = 49;
            label9.Text = "Our mission is to create the best programming tools";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            label10.ForeColor = System.Drawing.Color.DarkGray;
            label10.Location = new System.Drawing.Point(546, 692);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(592, 24);
            label10.TabIndex = 53;
            label10.Text = "For Microchip PIC, Atmel AVR and LogicGreen LGT microcontrollers.";
            // 
            // button1
            // 
            button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            button1.BackColor = System.Drawing.Color.DimGray;
            button1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button1.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            button1.ForeColor = System.Drawing.Color.White;
            button1.Location = new System.Drawing.Point(1007, 430);
            button1.Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(190, 82);
            button1.TabIndex = 54;
            button1.Text = "Create a Debug Dump File";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold);
            label13.ForeColor = System.Drawing.Color.DarkGray;
            label13.Location = new System.Drawing.Point(709, 772);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(241, 25);
            label13.TabIndex = 55;
            label13.Text = "+ In Memory of Bill Roth.";
            // 
            // AboutBox
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(10, 14, 43);
            ClientSize = new System.Drawing.Size(1257, 972);
            Controls.Add(textBoxDescription);
            Controls.Add(label13);
            Controls.Add(button1);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(label5);
            Controls.Add(linkLabel7);
            Controls.Add(labelver);
            Controls.Add(label7);
            Controls.Add(linkLabel6);
            Controls.Add(linkLabel5);
            Controls.Add(linkLabel4);
            Controls.Add(label3);
            Controls.Add(linkLabel3);
            Controls.Add(linkLabel2);
            Controls.Add(linkLabel1);
            Controls.Add(label1);
            Controls.Add(okButton);
            Controls.Add(pictureBox1);
            Controls.Add(groupBox1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 7, 4, 7);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(1259, 988);
            Name = "AboutBox";
            Padding = new System.Windows.Forms.Padding(16, 17, 16, 17);
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "About GC Studio";
            Load += AboutBox_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelcomp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelassembly;
        private System.Windows.Forms.Label labelfbas;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.LinkLabel linkLabel5;
        private System.Windows.Forms.LinkLabel linkLabel6;
        private System.Windows.Forms.Label labelver;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelgccode;
        private System.Windows.Forms.LinkLabel linkLabel7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label labeltoolchain;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label labelinstall;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label13;
    }
}
