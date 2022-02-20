
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Loader));
            this.ProgressUpdate = new System.Windows.Forms.ProgressBar();
            this.Version = new System.Windows.Forms.Label();
            this.Copyright = new System.Windows.Forms.Label();
            this.ApplicationTitle = new System.Windows.Forms.Label();
            this.BtnMini = new System.Windows.Forms.Button();
            this.BtnExit = new System.Windows.Forms.Button();
            this.MinSplash = new System.Windows.Forms.Timer(this.components);
            this.PictureBox2 = new System.Windows.Forms.PictureBox();
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // ProgressUpdate
            // 
            this.ProgressUpdate.Location = new System.Drawing.Point(262, 242);
            this.ProgressUpdate.MarqueeAnimationSpeed = 1;
            this.ProgressUpdate.Name = "ProgressUpdate";
            this.ProgressUpdate.Size = new System.Drawing.Size(308, 15);
            this.ProgressUpdate.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressUpdate.TabIndex = 16;
            this.ProgressUpdate.Visible = false;
            // 
            // Version
            // 
            this.Version.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Version.BackColor = System.Drawing.Color.Transparent;
            this.Version.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Version.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.Version.Location = new System.Drawing.Point(274, 245);
            this.Version.Name = "Version";
            this.Version.Size = new System.Drawing.Size(308, 20);
            this.Version.TabIndex = 14;
            this.Version.Text = "Versión {0}.{1:00}";
            // 
            // Copyright
            // 
            this.Copyright.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Copyright.BackColor = System.Drawing.Color.Transparent;
            this.Copyright.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Copyright.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.Copyright.Location = new System.Drawing.Point(272, 265);
            this.Copyright.Name = "Copyright";
            this.Copyright.Size = new System.Drawing.Size(312, 40);
            this.Copyright.TabIndex = 15;
            this.Copyright.Text = "Copyright";
            // 
            // ApplicationTitle
            // 
            this.ApplicationTitle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ApplicationTitle.BackColor = System.Drawing.Color.Transparent;
            this.ApplicationTitle.Font = new System.Drawing.Font("Microsoft YaHei", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ApplicationTitle.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.ApplicationTitle.Location = new System.Drawing.Point(276, 57);
            this.ApplicationTitle.Name = "ApplicationTitle";
            this.ApplicationTitle.Size = new System.Drawing.Size(308, 193);
            this.ApplicationTitle.TabIndex = 13;
            this.ApplicationTitle.Text = "GC Studio";
            this.ApplicationTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BtnMini
            // 
            this.BtnMini.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnMini.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnMini.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BtnMini.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(41)))), ((int)(((byte)(51)))));
            this.BtnMini.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnMini.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold);
            this.BtnMini.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.BtnMini.Location = new System.Drawing.Point(468, 0);
            this.BtnMini.Name = "BtnMini";
            this.BtnMini.Size = new System.Drawing.Size(64, 54);
            this.BtnMini.TabIndex = 10;
            this.BtnMini.Text = "―";
            this.BtnMini.UseVisualStyleBackColor = false;
            this.BtnMini.Click += new System.EventHandler(this.BtnMini_Click);
            // 
            // BtnExit
            // 
            this.BtnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnExit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BtnExit.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BtnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DarkRed;
            this.BtnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnExit.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold);
            this.BtnExit.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.BtnExit.Location = new System.Drawing.Point(532, 0);
            this.BtnExit.Name = "BtnExit";
            this.BtnExit.Size = new System.Drawing.Size(64, 54);
            this.BtnExit.TabIndex = 9;
            this.BtnExit.Text = "X";
            this.BtnExit.UseVisualStyleBackColor = false;
            this.BtnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // MinSplash
            // 
            this.MinSplash.Interval = 2200;
            this.MinSplash.Tick += new System.EventHandler(this.MinSplash_Tick);
            // 
            // PictureBox2
            // 
            this.PictureBox2.Location = new System.Drawing.Point(7, 4);
            this.PictureBox2.Name = "PictureBox2";
            this.PictureBox2.Size = new System.Drawing.Size(60, 61);
            this.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBox2.TabIndex = 12;
            this.PictureBox2.TabStop = false;
            // 
            // PictureBox1
            // 
            this.PictureBox1.Image = global::GC_Studio.Properties.Resources._0_3IFEy_hfoIpgFjBl;
            this.PictureBox1.Location = new System.Drawing.Point(-38, -20);
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.Size = new System.Drawing.Size(368, 354);
            this.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBox1.TabIndex = 11;
            this.PictureBox1.TabStop = false;
            // 
            // Loader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(598, 305);
            this.Controls.Add(this.ProgressUpdate);
            this.Controls.Add(this.Version);
            this.Controls.Add(this.Copyright);
            this.Controls.Add(this.ApplicationTitle);
            this.Controls.Add(this.PictureBox2);
            this.Controls.Add(this.PictureBox1);
            this.Controls.Add(this.BtnMini);
            this.Controls.Add(this.BtnExit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Loader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Loader_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.ProgressBar ProgressUpdate;
        internal System.Windows.Forms.Label Version;
        internal System.Windows.Forms.Label Copyright;
        internal System.Windows.Forms.Label ApplicationTitle;
        internal System.Windows.Forms.PictureBox PictureBox2;
        internal System.Windows.Forms.PictureBox PictureBox1;
        internal System.Windows.Forms.Button BtnMini;
        internal System.Windows.Forms.Button BtnExit;
        internal System.Windows.Forms.Timer MinSplash;
    }
}

