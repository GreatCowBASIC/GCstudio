using Ngine;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Update
{
    public partial class Updater : Form
    {
        DataFileEngine dfe = new DataFileEngine();
        public const double AppVer = 1.0201;
        double ManifestVer = 0;
        string ManifestPKG;
        string ManifestChecksum;
        string ManifestTitle;
        string ManifestNotes;
        string AppExe;

        public Updater()
        {
            debuglog(null);
            debuglog(null);
            debuglog("INFO Updater, starting Updater, initializing...");

            InitializeComponent();
        }

        private void Updater_Load(object sender, EventArgs e)
        {
            debuglog("INFO Updater, setting round corners on splash...");
            RoundCorners(this);

            debuglog("INFO Updater, looking for process Code and GCstudio, trying to kill...");
            foreach (var process in Process.GetProcessesByName("Code"))
            {
                process.Kill();
            }
            foreach (var process in Process.GetProcessesByName("GCstudio"))
            {
                process.Kill();
            }
            InitialDelay.Enabled = true;
            debuglog("DEBUG Updater, initial delay for killing process: Enabled=" + InitialDelay.Enabled + ", Interval=" + InitialDelay.Interval.ToString() + "ms");
        }

        private void InitialDelay_Tick(object sender, EventArgs e)
        {
            InitialDelay.Enabled = false;
            StartUpdate();
        }

        private void StartUpdate()
        {
            debuglog("INFO Updater, starting the update, looking for CVS manifest...");

            if (File.Exists("cvs.nfo"))
            {
                debuglog("INFO Updater, CVS manifest detected, loading it...");
                dfe.LoadRead("cvs.nfo");
                double.TryParse(dfe.ReadData(), out ManifestVer);
                ManifestPKG = dfe.ReadData();
                ManifestChecksum = dfe.ReadData();
                ManifestTitle = dfe.ReadData();
                ManifestNotes = dfe.ReadData();
                AppExe = dfe.ReadData();
                dfe.CloseRead();
                debuglog("DEBUG Updater, ManifestVer= " + ManifestVer);
                debuglog("DEBUG Updater, ManifestChecksumr= " + ManifestChecksum);
                debuglog("DEBUG Updater, ManifestTitle= " + ManifestTitle);
                debuglog("DEBUG Updater, ManifestNotes= " + ManifestNotes);
                debuglog("DEBUG Updater, AppExe= " + AppExe);
            }
            else
            {
                debuglog("WARNING Updater, CVS manifest not found, aborting update...");
                Environment.Exit(0);
            }



            try
            {
                debuglog("INFO Updater, applying update package...");
                ProcessStartInfo p = new ProcessStartInfo();
                p.FileName = "minidump.exe";
                p.Arguments = "x Update.pkg -y";
                p.WindowStyle = ProcessWindowStyle.Hidden;
                p.CreateNoWindow = true;
                Process x = Process.Start(p);
                x.WaitForExit();
            }
            catch (Exception ex) 
            {
                debuglog("ERROR Updater, an error occurred while applying the update, aborting update..." + " > " + ex.Message + " @ " + ex.StackTrace);
                MessageBox.Show("Error applying update.");
                Environment.Exit(0);
            }

            try
            {
                debuglog("INFO Updater, removing CVS manifest...");
                System.IO.File.Delete("cvs.nfo");
            }
            catch(Exception ex) 
            {
                debuglog("ERROR Updater, an error occurred while removing the CVS manifest..." + " > " + ex.Message + " @ " + ex.StackTrace);
            }

            try
            {
                debuglog("INFO Updater, removing update package...");
                System.IO.File.Delete("update.pkg");
            }
            catch (Exception ex)
            {
                debuglog("ERROR Updater, an error occurred while removing the update package..." + " > " + ex.Message + " @ " + ex.StackTrace);
            }



            try
            {
                debuglog("INFO Updater, update finished, starting the application...");
                ProcessStartInfo p = new ProcessStartInfo();
                p.FileName = AppExe;
                //p.Arguments = "";
                p.WindowStyle = ProcessWindowStyle.Normal;
                Process x = Process.Start(p);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                debuglog("ERROR Updater, an error occurred while starting the application..." + " > " + ex.Message + " @ " + ex.StackTrace);
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


        /// <summary>
        /// Debug Logger
        /// </summary>
        /// <param name="logstr"></param>
        public void debuglog(string logstr)
        {
            DataFileEngine dl = new DataFileEngine();
            try
            {
                dl.StreamW = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Log/Updater" + Updater.AppVer.ToString() + ".log", true);
                if (logstr != null)
                {
                    dl.RecordData(DateTime.UtcNow.ToString("[yyyy-MM-ddTHH:mm:ss.fffZ]") + ">>>\t" + logstr);
                }
                else
                {
                    dl.RecordData("");
                }
                dl.CloseWrite();
            }
            catch { }
        }


    }


}
