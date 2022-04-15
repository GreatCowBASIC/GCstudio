using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using DBSEngine;

namespace Update
{
    public partial class Updater : Form
    {
        DBS dbs = new DBS();
        decimal ManifestVer;
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
                dbs.LoadRead("cvs.nfo");
                ManifestVer = decimal.Parse(dbs.ReadData());
                ManifestPKG = dbs.ReadData();
                ManifestChecksum = dbs.ReadData();
                ManifestTitle = dbs.ReadData();
                ManifestNotes = dbs.ReadData();
                AppExe = dbs.ReadData();
                dbs.CloseRead();
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
                // p.Arguments = "";
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

    }
}
