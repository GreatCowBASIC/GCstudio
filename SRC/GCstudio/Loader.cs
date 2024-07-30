using Ngine;
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
        DataFileEngine debuglog = new DataFileEngine();
        DataFileEngine dfe = new DataFileEngine();
        JsonConvert json = new JsonConvert();
        ConfigSchema Config = new ConfigSchema();
        readonly string ReleasePath = "https://gcbasic.com/reps/stagebuild/updates/";
        public const double AppVer = 1.01142;
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
            try
            {
                debuglog.LoadWrite("/Log/GCstudio" + AppVer.ToString() + ".log");
            }
            catch { }

            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio started, initializing...");
            }
            catch { }

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
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, setting round corners on splash screen...");
            }
            catch { }

            RoundCorners(this);
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);


            arguments = Environment.GetCommandLineArgs();

            if (!this.IsElevated)
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, testing if can write on install path...");
                }
                catch { }

                try
                {
                    dfe.LoadWrite("access.dat");
                    dfe.RecordData("access");
                    dfe.CloseWrite();
                    File.Delete("access.dat");

                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, Access granted");
                    }
                    catch { }


                }
                catch
                {

                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, Access denied, trying to restart as Elevated...");
                        debuglog.CloseWrite();
                    }
                    catch { }


                    this.Visible = false;
                    try
                    { dfe.CloseWrite(); }
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


            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, looking for arguments...");
            }
            catch { }


            if (arguments.Length > 1)
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, arguments found, first argument:" + arguments[1]);
                }
                catch { }


                switch (arguments[1])
                {
                    case "/pkp" or "-p" or "--pkp":
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, aborting loader process...");
                            debuglog.CloseWrite();
                        }
                        catch { }

                        this.Close();
                        break;

                    case "/settings" or "-s" or "--settings":
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, aborting loader process...");
                            debuglog.CloseWrite();
                        }
                        catch { }

                        this.Close();
                        break;

                    case "/about" or "-a" or "--about":
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, aborting loader process...");
                            debuglog.CloseWrite();
                        }
                        catch { }

                        this.Close();
                        break;

                    case "/forceupdate" or "-f" or "--forceupdate":
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, getting ready for a Force Update...");
                        }
                        catch { }


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

            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, setting up version number on splash...");
            }
            catch { }

            Version.Text = "Version " + AppVer;
            Copyright.Text = ((AssemblyCopyrightAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyCopyrightAttribute))).Copyright;

            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, Starting thread to update GCBASIC_INSTALL_PATH Environment variable...");
            }
            catch { }


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
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, Closing flag for loader reached continuing to main panel...");
                    debuglog.CloseWrite();
                }
                catch { }


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
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, loading configuration started...");
            }
            catch { }


    
                //Load old App Config
                if (File.Exists("config.ini"))
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  Old config.ini detected, reading ReleaseChanel from it...");
                    }
                    catch { }

                    try
                    {
                        dfe.LoadRead("config.ini");
                        Config.GCstudio.ReleaseChanel = dfe.ReadData();
                        dfe.CloseRead();
                    }
                    catch
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio Loader, Failed to read config.ini, trying to fix...");
                        }
                        catch { }
                        try
                        {
                            File.Delete("config.ini");
                        }
                        catch
                        {
                            try
                            {
                                debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio Loader, Could not delete config.ini, giving up...");
                            }
                            catch { }
                        }

                }
                }
                else
                //load app config
                {
                    if (File.Exists("GCstudio.config.json"))
                    {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, GCstudio.config.json detected, reading and deserializing it...");
                    }
                    catch { }

                    try
                    {
                        dfe.LoadRead("GCstudio.config.json");
                        Config = json.DeserializeObject<ConfigSchema>(dfe.ReadAll());
                        dfe.CloseRead();
                    }
                    catch
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio Loader, failed to read and deserialize GCstudio.config.json, trying to fix...");
                        }
                        catch { }
                        try
                        {
                            File.Delete("GCstudio.config.json");
                        }
                        catch
                        {
                            try
                            {
                                debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio Loader, Could not delete GCstudio.config.json, giving up...");
                            }
                            catch { }
                        }

                    }
                }
                }
            

        }



        /// <summary>
        /// 3 Initial tasks, one time shown the Splash Screen (CVS download) 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Shown(object sender, EventArgs e)
        {

            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, Clearing update cache...");
            }
            catch { }


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
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, Detecting Network...");
            }
            catch { }


            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == true)
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, Network detected, downloading CVS manifest...");
                }
                catch { }


                WebClientCVS.DownloadFileCompleted += OnCVSDownloadCompleted;
                WebClientCVS.DownloadFileAsync(new Uri(ReleasePath + "cvs" + Config.GCstudio.ReleaseChanel + ".nfo"), "cvs.nfo");
            }
            else
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, No network detected, exiting updater...");
                }
                catch { }


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
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, cvs manifest download canceled by user, exiting updater...");
                }
                catch { }

                EndForm();
            }
            else if (e.Error is object)
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio Loader, an error ocurred while downloading cvs manifest, exiting updater...");
                }
                catch { }


                //Exit by disconnection or lack of CVS 
                EndForm();
            }
            else
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, cvs manifest downloaded, appending it...");
                }
                catch { }

                try
                {
                    dfe.LoadRead("cvs.nfo");
                    double.TryParse(dfe.ReadData(), Style, Provider, out ManifestVer);
                    ManifestPKG = dfe.ReadData();
                    ManifestChecksum = dfe.ReadData();
                    ManifestTitle = dfe.ReadData();
                    ManifestNotes = dfe.ReadData();
                    dfe.ReadData();
                    double.TryParse(dfe.ReadData(), Style, Provider, out ManifestMinVer);
                    dfe.CloseRead();
                }
                catch
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio Loader, an error ocurred while appending the cvs manifest...");
                    }
                    catch { }


                }

                if (AppVer >= ManifestMinVer)
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, There is no update available...");
                    }
                    catch { }


                    if (ManifestVer > AppVer || forceupdate)
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, Starting the Update...");
                        }
                        catch { }


                        if (File.Exists("post.dat"))
                        {
                            try
                            {
                                debuglog.RecordData(DateTime.Now.ToString() + ">>>  WARNING GCstudio Loader, Previous update error detected, warning user...");
                            }
                            catch { }


                            MessageBox.Show("There was an error while applying the update. Please check that all GC Studio instances are closed, or restart your PC. The update will retry on next launch.", "Oops! something didn’t go as planned.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            EndForm();
                        }
                        else
                        {
                            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == true)
                            {
                                try
                                {
                                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, Downloading the update package...");
                                }
                                catch { }


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
                                    try
                                    {
                                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio Loader, an error ocurred while downloading the update pakage.");
                                    }
                                    catch { }


                                }
                            }
                            else
                            {
                                try
                                {
                                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, Exiting updater...");
                                }
                                catch { }


                                EndForm();
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, Exiting updater...");
                        }
                        catch { }


                        EndForm();
                    }
                }
                else
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  WARNING GCstudio Loader, unsupported version of GCstudio detected, warning user and exiting updater...");
                    }
                    catch { }


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
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, Update canceled by user, exiting updater...");
                }
                catch { }


                MessageBox.Show("Update canceled by user.");
                EndForm();
            }
            else if (e.Error is object)
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio Loader, an error ocurred while downloading the update package, exiting updater...");
                }
                catch { }


                MessageBox.Show("There was a problem downloading the update.");
                EndForm();
            }
            else
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, update package download complete, verifying...");
                }
                catch { }


                try
                {
                    UpdateChecksum = dfe.CreateMD5Sum("update.pkg");
                    if (UpdateChecksum == ManifestChecksum)
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, update package checksum match, starting the update process...");
                        }
                        catch { }



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
                            try
                            {
                                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, an error ocurred while launching the update process, exiting updater...");
                            }
                            catch { }


                            MessageBox.Show("Error starting the update.");
                            EndForm();
                        }
                    }
                    else
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio Loader, update package checksum missmatch, exiting updater...");
                        }
                        catch { }

                        EndForm();

                    }
                }
                catch
                {
                }

                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio Loader, an error ocurred while verifying the update package, exiting updater...");
                }
                catch { }


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
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, minimizing the splash by user request...");
            }
            catch { }


            this.WindowState = FormWindowState.Minimized;
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio Loader, Aborting the updater by user request...");
            }
            catch { }


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
