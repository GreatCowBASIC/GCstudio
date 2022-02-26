using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using DBSEngine;


namespace GC_Studio
{
    public partial class Loader : Form
    {
        DBS dbs = new DBS();
        readonly string ReleasePath = "https://www.mier.com.mx/rel/GCB/";
        string ReleaseChanel = "mainstream";
        decimal AppVer;
        decimal ManifestVer;
        string ManifestPKG;
        string ManifestChecksum;
        string ManifestTitle;
        string ManifestNotes;
        string UpdateChecksum;
        string[] arguments;
        bool continueflag = false;
        bool downloading = false;
        System.Net.WebClient WebClientCVS = new System.Net.WebClient();
        System.Net.WebClient WebClientPKG = new System.Net.WebClient();
        Thread EnvVarThread = new Thread(new ThreadStart(Loader.SetEnvVar));


        /// <summary>
        /// 1 Initialization of the component
        /// </summary>
        public Loader()
        {

            InitializeComponent();
            this.Shown += new System.EventHandler(this.Form_Shown);
        }


        /// <summary>
        /// 2 Set curved edges and data loading of the assembly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Loader_Load(object sender, EventArgs e)
        {
            RoundCorners(this);
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                if (arguments[1] == "/pkp")
                {
                    this.Close();
                }
            }

                //  ApplicationTitle.Text = Assembly.GetEntryAssembly().GetName().Name;
                //AppVer = decimal.Parse(Assembly.GetEntryAssembly().GetName().Version.Major.ToString() + "." + Assembly.GetEntryAssembly().GetName().Version.Minor.ToString());
                try
            {
                dbs.LoadRead("CurrentVersion.nfo");
                AppVer = decimal.Parse(dbs.ReadData());
                dbs.CloseRead();
            }
            catch
            {
                MessageBox.Show("Error reading current version manifest.");
                Environment.Exit(0);
            }
            Version.Text = "Versión " + AppVer;
            Copyright.Text = ((AssemblyCopyrightAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyCopyrightAttribute))).Copyright;

            EnvVarThread.Start();

        }

        public static void SetEnvVar()
        {
            try
            {
                Environment.SetEnvironmentVariable("GCBASIC_INSTALL_PATH", AppDomain.CurrentDomain.BaseDirectory, EnvironmentVariableTarget.User);
            }
            catch
            {

            }
        }

        /// <summary>
        /// 5 Final code and closure to load below Form 
        /// </summary>
        private void EndForm()
        {
            continueflag = true;
            if (MinSplash.Enabled == false)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Load the configuration of the app 
        /// </summary>
        private void LoadConfig()
        {
            //Load App Config
            if (File.Exists("config.ini"))
            {

                dbs.LoadRead("config.ini");
                ReleaseChanel = dbs.ReadData();
                dbs.CloseRead();
            }
            //else
            //default App Config
            //{
            //    dbs.LoadWrite("config.ini");
            //    dbs.RecordData("mainstream");
            //    dbs.RecordData("END");
            //    dbs.CloseWrite();
            //}

        }



        /// <summary>
        /// 3 Initial tasks, one time shown the Splash Screen (CVS download) 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Shown(object sender, EventArgs e)
        {


            MinSplash.Enabled = true;
            //Clear Update Cache
            try
            {
                System.IO.File.Delete("update.pkg");
            }
            catch { }

            try
            {
                System.IO.File.Delete("cvs.nfo");
            }
            catch { }
            //Configuration load call 
            LoadConfig();

            //Download CVS
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == true)
            {
                WebClientCVS.DownloadFileCompleted += OnCVSDownloadCompleted;
                WebClientCVS.DownloadFileAsync(new Uri(ReleasePath + "cvs" + ReleaseChanel + ".nfo"), "cvs.nfo");
            }
            else
            {
                EndForm();
            }

        }


        /// <summary>
        /// 4 CVS analysis and PKG download if new version exist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCVSDownloadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {

                MessageBox.Show("Update canceled by user.");
                Environment.Exit(0);
            }
            else if (e.Error is object)
            {
                //Exit by disconnection or lack of CVS 
                EndForm();
            }
            else
            {

                dbs.LoadRead("cvs.nfo");
                ManifestVer = decimal.Parse(dbs.ReadData());
                ManifestPKG = dbs.ReadData();
                ManifestChecksum = dbs.ReadData();
                ManifestTitle = dbs.ReadData();
                ManifestNotes = dbs.ReadData();
                dbs.CloseRead();
            //    try
            //    {
            //        System.IO.File.Delete("cvs.nfo");
            //    }
            //    catch
            //    {
            //    }

                if (ManifestVer > AppVer)
                {
                    if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == true)
                    {
                        try
                        {
                            downloading = true;   
                            //BtnExit.Enabled = false;
                            Version.Visible = false;
                            ProgressUpdate.Visible = true;
                            WebClientPKG.DownloadProgressChanged += OnDownloadProgressChanged;
                            WebClientPKG.DownloadFileCompleted += OnFileDownloadCompleted;
                            WebClientPKG.DownloadFileAsync(new Uri(ReleasePath + ManifestPKG), "update.pkg");
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        EndForm();
                    }
                }
                else
                {
                    EndForm();
                }
            }
        }

        /// <summary>
        /// Update of the state of the download of the PKG 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            object totalsize = e.TotalBytesToReceive / 1000000;
            object downloadedbytes = e.BytesReceived / 1000000;
            int percentage = e.ProgressPercentage;
            if (percentage != 100)
            {
                ProgressUpdate.Value = percentage + 1;
            }
            ProgressUpdate.Value = percentage;
            // sizelbl.Text = totalsize & "Mb."
            // downloadedlbl.Text = downloadedbytes & "Mb."
            Copyright.Text = "Downloading Update. " + percentage + " %";
        }

        /// <summary>
        /// 5 PKG Checksum Analysis and Update Installation 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileDownloadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            ProgressUpdate.Value = 100;
            if (e.Cancelled)
            {
                MessageBox.Show("Update canceled by user.");
                //Environment.Exit(0);
            }
            else if (e.Error is object)
            {
                MessageBox.Show("There was a problem downloading the update.");
                Environment.Exit(0);
            }
            else
            {
                try
                {
                    UpdateChecksum = dbs.CreateMD5Sum("update.pkg");
                    if (UpdateChecksum == ManifestChecksum)
                    {


                        try
                        {
                            ProcessStartInfo p = new ProcessStartInfo();
                            p.FileName = "update.exe";
                            // p.Arguments = "";
                            p.WindowStyle = ProcessWindowStyle.Normal;
                            Process x = Process.Start(p);
                            Environment.Exit(0);
                        }
                        catch
                        {
                            MessageBox.Show("Error starting the update.");
                            Environment.Exit(0);
                        }
                    }
                }
                catch
                {
                }

                MessageBox.Show("Error starting the update.");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Code for Timer of Minimum time of Splash Screen 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinSplash_Tick(object sender, EventArgs e)
        {
            if (continueflag == true)
            {
                MinSplash.Enabled = false;
                EndForm();
            }

            MinSplash.Enabled = false;
        }


        private void BtnMini_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            if (downloading == false)
            {
                Environment.Exit(0);
            }
            else
            {
                EndForm();
                WebClientPKG.CancelAsync();
                
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

        

    }
}
