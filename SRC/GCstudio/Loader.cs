using DBSEngine;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;


namespace GC_Studio
{
    public partial class Loader : Form
    {
        DBS dbs = new DBS();
        ConfigSchema Config = new ConfigSchema();
        readonly string ReleasePath = "https://gcbasic.com/reps/stagebuild/updates/";
        public const double AppVer = 1.0014;
        double ManifestVer = 0;
        double ManifestMinVer = 0;
        string ManifestPKG;
        string ManifestChecksum;
        string ManifestTitle;
        string ManifestNotes;
        string UpdateChecksum;
        string[] arguments;
        NumberStyles Style = NumberStyles.AllowDecimalPoint;
        CultureInfo Provider = new CultureInfo("en-US");

        bool continueflag = false;
        bool downloading = false;
        bool forceupdate = false;
        System.Net.WebClient WebClientCVS = new();
        System.Net.WebClient WebClientPKG = new();
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

            if (!this.IsElevated)
            {
                try
                {
                    dbs.LoadWrite("access.dat");
                    dbs.RecordData("access");
                    dbs.CloseWrite();
                    File.Delete("access.dat");
                }
                catch
                {
                    this.Visible = false;
                    try
                    { dbs.CloseWrite(); }
                    catch { }

                    ProcessStartInfo p = new ProcessStartInfo();
                    Process x;
                    try
                    {
                        string args;
                        if (arguments.Length > 1)
                        {
                            args = "";
                            for (int i = 1; i < arguments.Length; i++)
                            {
                                args += "\"" + arguments[i] + "\"";
                            }
                        }
                        else
                        {
                            args = "";
                        }
                        p.FileName = "gcstudio.exe";
                        p.Arguments = args;
                        p.WindowStyle = ProcessWindowStyle.Normal;
                        p.Verb = "runas";
                        p.UseShellExecute = true;
                        x = Process.Start(p);
                        Environment.Exit(0);
                    }
                    catch
                    {
                        MessageBox.Show("The current Install Path doesnt have write rights, this requires GC Studio to run Elevated.");
                        Environment.Exit(0);
                    }

                }
            }

            if (arguments.Length > 1)
            {

                switch (arguments[1])
                {
                    case "/pkp" or "-p" or "--pkp":
                        this.Close();
                        break;

                    case "/settings" or "-s" or "--settings":
                        this.Close();
                        break;

                    case "/about" or "-a" or "--about":
                        this.Close();
                        break;

                    case "/forceupdate" or "-f" or "--forceupdate":
                        try
                        {
                            File.Delete("post.dat");
                        }
                        catch { }

                        forceupdate = true;
                        break;


                    default:

                        break;
                }

            }

            Version.Text = "Version " + AppVer;
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

            try
            {
                //Load old App Config
                if (File.Exists("config.ini"))
                {
                    dbs.LoadRead("config.ini");
                    Config.GCstudio.ReleaseChanel = dbs.ReadData();
                    dbs.CloseRead();
                }
                else
                //load app config
                {
                    if (File.Exists("GCstudio.config.json"))
                    {
                        dbs.LoadRead("GCstudio.config.json");
                        Config = JsonConvert.DeserializeObject<ConfigSchema>(dbs.ReadAll());
                        dbs.CloseRead();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error loading config.");
            }

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
                WebClientCVS.DownloadFileAsync(new Uri(ReleasePath + "cvs" + Config.GCstudio.ReleaseChanel + ".nfo"), "cvs.nfo");
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
                EndForm();
            }
            else if (e.Error is object)
            {
                //Exit by disconnection or lack of CVS 
                EndForm();
            }
            else
            {

                dbs.LoadRead("cvs.nfo");
                double.TryParse(dbs.ReadData(), Style, Provider, out ManifestVer);
                ManifestPKG = dbs.ReadData();
                ManifestChecksum = dbs.ReadData();
                ManifestTitle = dbs.ReadData();
                ManifestNotes = dbs.ReadData();
                dbs.ReadData();
                double.TryParse(dbs.ReadData(), Style, Provider, out ManifestMinVer);
                dbs.CloseRead();

                if (AppVer >= ManifestMinVer)
                {
                    if (ManifestVer > AppVer || forceupdate)
                    {
                        if (File.Exists("post.dat"))
                        {
                            MessageBox.Show("There was an error while applying the update. Please check that all GC Studio instances are closed, or restart your PC. The update will retry on next launch.", "Oops! something didn’t go as planned.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            EndForm();
                        }
                        else
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
                    }
                    else
                    {
                        EndForm();
                    }
                }
                else
                {
                    MessageBox.Show("A new update is available, but the current installed version is too old to update, please download and install the current version of GC Studio.", "Unsupported GC Studio Version", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                EndForm();
            }
            else if (e.Error is object)
            {
                MessageBox.Show("There was a problem downloading the update.");
                EndForm();
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
                            EndForm();
                        }
                    }
                }
                catch
                {
                }

                MessageBox.Show("Error starting the update.");
                EndForm();
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
                EndForm();
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

        public bool IsElevated
        {
            get
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }


    }


}
