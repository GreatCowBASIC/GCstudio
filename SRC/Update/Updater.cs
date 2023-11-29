using Ngine;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Update
{
    public partial class Updater : Form
    {
        DataFileEngine dfe = new DataFileEngine();
        double ManifestVer = 0;
        string ManifestPKG;
        string ManifestChecksum;
        string ManifestTitle;
        string ManifestNotes;
        string AppExe;

        public Updater()
        {
            InitializeComponent();
        }

        private void Updater_Load(object sender, EventArgs e)
        {
            RoundCorners(this);
            foreach (var process in Process.GetProcessesByName("Code"))
            {
                process.Kill();
            }
            foreach (var process in Process.GetProcessesByName("GCstudio"))
            {
                process.Kill();
            }
            InitialDelay.Enabled = true;
        }

        private void InitialDelay_Tick(object sender, EventArgs e)
        {
            InitialDelay.Enabled = false;
            StartUpdate();
        }

        private void StartUpdate()
        {

            if (File.Exists("cvs.nfo"))
            {
                dfe.LoadRead("cvs.nfo");
                double.TryParse(dfe.ReadData(), out ManifestVer);
                ManifestPKG = dfe.ReadData();
                ManifestChecksum = dfe.ReadData();
                ManifestTitle = dfe.ReadData();
                ManifestNotes = dfe.ReadData();
                AppExe = dfe.ReadData();
                dfe.CloseRead();
            }
            else
            {
                Environment.Exit(0);
            }



            try
            {
                ProcessStartInfo p = new ProcessStartInfo();
                p.FileName = "minidump.exe";
                p.Arguments = "x Update.pkg -y";
                p.WindowStyle = ProcessWindowStyle.Hidden;
                p.CreateNoWindow = true;
                Process x = Process.Start(p);
                x.WaitForExit();
            }
            catch
            {
                MessageBox.Show("Error applying update.");
                Environment.Exit(0);
            }

            try
            {
                System.IO.File.Delete("cvs.nfo");
            }
            catch
            {
            }

            try
            {
                System.IO.File.Delete("update.pkg");
            }
            catch
            {
            }



            try
            {
                ProcessStartInfo p = new ProcessStartInfo();
                p.FileName = AppExe;
                //p.Arguments = "";
                p.WindowStyle = ProcessWindowStyle.Normal;
                Process x = Process.Start(p);
                Environment.Exit(0);
            }
            catch
            {
                MessageBox.Show("Error starting the application.");
                Environment.Exit(0);
            }



        }

        public void RoundCorners(Form form)
        {
            form.FormBorderStyle = FormBorderStyle.None;

            var DGP = new GraphicsPath();
            DGP.StartFigure();
            // top left corner
            DGP.AddArc(new Rectangle(0, 0, 40, 40), 180, 90);
            DGP.AddLine(40, 0, form.Width - 40, 0);

            // top right corner
            DGP.AddArc(new Rectangle(form.Width - 40, 0, 40, 40), -90, 90);
            DGP.AddLine(form.Width, 40, form.Width, form.Height - 40);

            // buttom right corner
            DGP.AddArc(new Rectangle(form.Width - 40, form.Height - 40, 40, 40), 0, 90);
            DGP.AddLine(form.Width - 40, form.Height, 40, form.Height);

            // buttom left corner
            DGP.AddArc(new Rectangle(0, form.Height - 40, 40, 40), 90, 90);
            DGP.CloseFigure();
            form.Region = new Region(DGP);
        }

        private void Updater_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }


}
