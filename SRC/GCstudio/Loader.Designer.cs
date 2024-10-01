
namespace GC_Studio
{
    partial class Loader
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Loader));
            ProgressUpdate = new System.Windows.Forms.ProgressBar();
            Version = new System.Windows.Forms.Label();
            Copyright = new System.Windows.Forms.Label();
            ApplicationTitle = new System.Windows.Forms.Label();
            BtnMini = new System.Windows.Forms.Button();
            BtnExit = new System.Windows.Forms.Button();
            MinSplash = new System.Windows.Forms.Timer(components);
            PictureBox1 = new System.Windows.Forms.PictureBox();
            pictureBox2 = new System.Windows.Forms.PictureBox();
            pictureBox3 = new System.Windows.Forms.PictureBox();
            pictureBox4 = new System.Windows.Forms.PictureBox();
            pictureBox5 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)PictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            SuspendLayout();
            // 
            // ProgressUpdate
            // 
            ProgressUpdate.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            ProgressUpdate.Location = new System.Drawing.Point(565, 551);
            ProgressUpdate.MarqueeAnimationSpeed = 1;
            ProgressUpdate.Name = "ProgressUpdate";
            ProgressUpdate.Size = new System.Drawing.Size(425, 18);
            ProgressUpdate.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            ProgressUpdate.TabIndex = 16;
            ProgressUpdate.Visible = false;
            // 
            // Version
            // 
            Version.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            Version.BackColor = System.Drawing.Color.Transparent;
            Version.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            Version.ForeColor = System.Drawing.Color.WhiteSmoke;
            Version.Location = new System.Drawing.Point(660, 547);
            Version.Name = "Version";
            Version.Size = new System.Drawing.Size(343, 25);
            Version.TabIndex = 14;
            Version.Text = "Version {0}.{1:00}";
            // 
            // Copyright
            // 
            Copyright.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            Copyright.BackColor = System.Drawing.Color.Transparent;
            Copyright.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            Copyright.ForeColor = System.Drawing.Color.WhiteSmoke;
            Copyright.Location = new System.Drawing.Point(659, 570);
            Copyright.Name = "Copyright";
            Copyright.Size = new System.Drawing.Size(347, 60);
            Copyright.TabIndex = 15;
            Copyright.Text = "Copyright";
            // 
            // ApplicationTitle
            // 
            ApplicationTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            ApplicationTitle.BackColor = System.Drawing.Color.Transparent;
            ApplicationTitle.Font = new System.Drawing.Font("Microsoft YaHei", 24F);
            ApplicationTitle.ForeColor = System.Drawing.Color.WhiteSmoke;
            ApplicationTitle.Location = new System.Drawing.Point(710, 226);
            ApplicationTitle.Name = "ApplicationTitle";
            ApplicationTitle.Size = new System.Drawing.Size(272, 55);
            ApplicationTitle.TabIndex = 13;
            ApplicationTitle.Text = "GC Studio";
            ApplicationTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BtnMini
            // 
            BtnMini.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnMini.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            BtnMini.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(10, 14, 43);
            BtnMini.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(34, 41, 51);
            BtnMini.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            BtnMini.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold);
            BtnMini.ForeColor = System.Drawing.Color.WhiteSmoke;
            BtnMini.Location = new System.Drawing.Point(876, 0);
            BtnMini.Name = "BtnMini";
            BtnMini.Size = new System.Drawing.Size(71, 67);
            BtnMini.TabIndex = 10;
            BtnMini.Text = "―";
            BtnMini.UseVisualStyleBackColor = false;
            BtnMini.Click += BtnMini_Click;
            // 
            // BtnExit
            // 
            BtnExit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            BtnExit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            BtnExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(10, 14, 43);
            BtnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkRed;
            BtnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            BtnExit.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold);
            BtnExit.ForeColor = System.Drawing.Color.WhiteSmoke;
            BtnExit.Location = new System.Drawing.Point(947, 0);
            BtnExit.Name = "BtnExit";
            BtnExit.Size = new System.Drawing.Size(71, 67);
            BtnExit.TabIndex = 9;
            BtnExit.Text = "X";
            BtnExit.UseVisualStyleBackColor = false;
            BtnExit.Click += BtnExit_Click;
            // 
            // MinSplash
            // 
            MinSplash.Interval = 1500;
            MinSplash.Tick += MinSplash_Tick;
            // 
            // PictureBox1
            // 
            PictureBox1.Image = Properties.Resources.GCstudio;
            PictureBox1.Location = new System.Drawing.Point(0, 100);
            PictureBox1.Name = "PictureBox1";
            PictureBox1.Size = new System.Drawing.Size(686, 400);
            PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            PictureBox1.TabIndex = 11;
            PictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (System.Drawing.Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new System.Drawing.Point(0, 0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new System.Drawing.Size(636, 124);
            pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 17;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = (System.Drawing.Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new System.Drawing.Point(0, 498);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new System.Drawing.Size(613, 132);
            pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 18;
            pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = (System.Drawing.Image)resources.GetObject("pictureBox4.Image");
            pictureBox4.Location = new System.Drawing.Point(664, 84);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new System.Drawing.Size(356, 450);
            pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 19;
            pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            pictureBox5.Location = new System.Drawing.Point(610, 0);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new System.Drawing.Size(408, 630);
            pictureBox5.TabIndex = 20;
            pictureBox5.TabStop = false;
            // 
            // Loader
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(10, 14, 43);
            ClientSize = new System.Drawing.Size(1020, 630);
            Controls.Add(ProgressUpdate);
            Controls.Add(Version);
            Controls.Add(Copyright);
            Controls.Add(pictureBox3);
            Controls.Add(BtnMini);
            Controls.Add(BtnExit);
            Controls.Add(ApplicationTitle);
            Controls.Add(pictureBox4);
            Controls.Add(pictureBox2);
            Controls.Add(PictureBox1);
            Controls.Add(pictureBox5);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "Loader";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "   ";
            Load += Loader_Load;
            ((System.ComponentModel.ISupportInitialize)PictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ResumeLayout(false);
        }

        #endregion

        internal System.Windows.Forms.ProgressBar ProgressUpdate;
        internal System.Windows.Forms.Label Version;
        internal System.Windows.Forms.Label Copyright;
        internal System.Windows.Forms.Label ApplicationTitle;
        internal System.Windows.Forms.PictureBox PictureBox1;
        internal System.Windows.Forms.Button BtnMini;
        internal System.Windows.Forms.Button BtnExit;
        internal System.Windows.Forms.Timer MinSplash;
        internal System.Windows.Forms.PictureBox pictureBox2;
        internal System.Windows.Forms.PictureBox pictureBox3;
        internal System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox5;
    }
}

