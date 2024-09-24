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
        
        DataFileEngine dfe = new DataFileEngine();
        JsonConvert json = new JsonConvert();
        ConfigSchema Config = new ConfigSchema();
        UpdateManifest CVS = new UpdateManifest();
        readonly string ReleasePath = "https://gcbasic.com/reps/stagebuild/updates/";
        public const double AppVer = 1.0116;
        string[] arguments;
        string UpdateChecksum = null;
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
            debuglog(null);
            debuglog(null);
            debuglog("INFO GCstudio Loader, starting GCstudio, initializing loader...");

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

            debuglog("INFO GCstudio Loader, setting round corners on splash screen...");
       

            RoundCorners(this);


            debuglog("DEBUG GCstudio Loader, setting current directory=" + AppDomain.CurrentDomain.BaseDirectory);



            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);


            arguments = Environment.GetCommandLineArgs();

            if (!this.IsElevated)
            {

                debuglog("INFO GCstudio Loader, testing if can write on install path...");


                try
                {
                    dfe.LoadWrite("access.dat");
                    dfe.RecordData("access");
                    dfe.CloseWrite();
                    File.Delete("access.dat");

                    debuglog("DEBUG GCstudio Loader, Access granted");

                }
                catch
                {

                    debuglog("DEBUG GCstudio Loader, Access denied, trying to restart as Elevated...");

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
                    catch (Exception ex)
                    {
                        debuglog("CRITICAL GCstudio Loader, launching GCstudio elevated failed, exiting." + " > " + ex.Message + " @ " + ex.StackTrace);
                        MessageBox.Show("The current Install Path doesn't have write rights, this requires GC Studio to run Elevated.");
                        Environment.Exit(0);
                    }

                }
            }


                debuglog("INFO GCstudio Loader, looking for arguments...");

            if (arguments.Length > 1)
            {

                    debuglog("DEBUG GCstudio Loader, arguments found, first argument=" + arguments[1]);


                switch (arguments[1])
                {
                    case "/pkp" or "-p" or "--pkp":

                        debuglog("INFO GCstudio Loader, aborting loader process...");

                        this.Close();
                        break;

                    case "/settings" or "-s" or "--settings":

                        debuglog("INFO GCstudio Loader, aborting loader process...");

                        this.Close();
                        break;

                    case "/about" or "-a" or "--about":

                        debuglog("INFO GCstudio Loader, aborting loader process...");

                        this.Close();
                        break;

                    case "/forceupdate" or "-f" or "--forceupdate":

                        debuglog("INFO GCstudio Loader, getting ready for a Force Update...");

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

            debuglog("INFO GCstudio Loader, setting up version number on splash...");

            Version.Text = "Version " + AppVer;
            Copyright.Text = ((AssemblyCopyrightAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyCopyrightAttribute))).Copyright;

            debuglog("INFO GCstudio Loader, Starting thread to update GCBASIC_INSTALL_PATH Environment variable...");


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

                debuglog("INFO GCstudio Loader, Closing flag for loader reached, continuing to main panel...");

                this.Close();
            }
        }

        /// <summary>
        /// Load the configuration of the app 
        /// </summary>
        private void LoadConfig()
        {

            debuglog("INFO GCstudio Loader, loading configuration started...");
    

                //load app config
                {
                if (File.Exists("GCstudio.config.json"))
                {

                    debuglog("INFO GCstudio Loader, GCstudio.config.json detected, reading and deserializing it...");


                    try
                    {
                        dfe.LoadRead("GCstudio.config.json");
                        Config = json.DeserializeObject<ConfigSchema>(dfe.ReadAll());
                        dfe.CloseRead();
                    }
                    catch (Exception ex1)
                    {

                        debuglog("ERROR GCstudio Loader, failed to read and deserialize GCstudio.config.json." + " > " + ex1.Message + ex1.StackTrace);

                        try
                        {
                            debuglog("INFO GCstudio Loader, trying to fix config file...");
                            dfe.CloseRead();
                            File.Delete("GCstudio.config.json");
                            debuglog("INFO GCstudio Loader, fix applied.");
                        }
                        catch (Exception ex2)
                        {
                            debuglog("ERROR GCstudio Loader, Could not delete GCstudio.config.json, giving up..." + " > " + ex2.Message + " @ " + ex2.StackTrace);

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

            debuglog("INFO GCstudio Loader, Clearing update cache...");

            MinSplash.Enabled = true;
            //Clear Update Cache
            try
            {
                System.IO.File.Delete("update.pkg");
            }
            catch { }

            try
            {
                System.IO.File.Delete("cvs.json");
            }
            catch { }
            //Configuration load call 
            LoadConfig();

            //Download CVS

            debuglog("INFO GCstudio Loader, Detecting Network...");

            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == true)
            {
                debuglog("INFO GCstudio Loader, Network detected, downloading CVS manifest...");
                debuglog("DEBUG GCstudio Loader, Config.GCstudio.ReleaseChanel=" + Config.GCstudio.ReleaseChanel);

                WebClientCVS.DownloadFileCompleted += OnCVSDownloadCompleted;
                WebClientCVS.DownloadFileAsync(new Uri(ReleasePath + "cvs" + Config.GCstudio.ReleaseChanel + ".json"), "cvs.json");
            }
            else
            {
                debuglog("INFO GCstudio Loader, No network detected, exiting updater...");

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
                debuglog("INFO GCstudio Loader, cvs manifest download canceled by user, exiting updater...");

                EndForm();
            }
            else if (e.Error is object)
            {

                debuglog("ERROR GCstudio Loader, an error ocurred while downloading cvs manifest, exiting updater..." + " > " + e.Error.ToString());

                //Exit by disconnection or lack of CVS 
                EndForm();
            }
            else
            {

                debuglog("INFO GCstudio Loader, cvs manifest downloaded, loading and deserializing it...");

                try
                {
                    dfe.LoadRead("cvs.json");
                    CVS = json.DeserializeObject<UpdateManifest>(dfe.ReadAll());
                    dfe.CloseRead();
                }
                catch (Exception ex)
                {

                    debuglog("ERROR GCstudio Loader, an error occurred while loading and deserializing the cvs manifest..." + " > " + ex.Message + " @ " + ex.StackTrace);

                }

                if (AppVer >= CVS.UpdateInfo.ManifestMinVer)
                {


                    if (CVS.UpdateInfo.ManifestVer > AppVer)
                    {
                        debuglog("INFO GCstudio Loader, update available, starting the update...");
                        debuglog("DEBUG GCstudio Loader, CVS.UpdateInfo.ManifestVer="+ CVS.UpdateInfo.ManifestVer);
                        StartUpdate();
                    }
                    else
                    {
                        if (forceupdate)
                        {
                            debuglog("INFO GCstudio Loader, Force Update requested...");
                            if (CVS.UpdateInfo.ManifestVer >= AppVer)
                            {
                                debuglog("INFO GCstudio Loader, update available, starting the update...");
                                debuglog("DEBUG GCstudio Loader, CVS.UpdateInfo.ManifestVer=" + CVS.UpdateInfo.ManifestVer);
                                StartUpdate();
                            }
                            else
                            {
                                debuglog("CRITICAL GCstudio Loader, Downgrade detected, informing user and Exiting...");
                                MessageBox.Show("A force update was requested, but a Downgrade scenario was detected, wait for a new update available or change channel. Aborting update.", "Version Downgrade Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Environment.Exit(0);
                            }
                        }
                        else
                        {
                            debuglog("INFO GCstudio Loader, There is no new update available...");
                            debuglog("INFO GCstudio Loader, Exiting updater...");

                            EndForm();
                        }
                    }
                }
                else
                {
                    debuglog("WARNING GCstudio Loader, unsupported version of GCstudio detected, warning user and exiting updater...");
                    debuglog("DEBUG GCstudio Loader, CVS.UpdateInfo.ManifestMinVer=" + CVS.UpdateInfo.ManifestMinVer);

                    MessageBox.Show("A new update is available, but the current installed version is too old to update, please download and install the current version of GC Studio.", "Unsupported GC Studio Version", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    EndForm();
                }

            }
        }

        private void StartUpdate ()
        {
            if (File.Exists("post.dat"))
            {
                debuglog("WARNING GCstudio Loader, Previous update error detected, warning user...");

                MessageBox.Show("There was an error while applying the update. Please check that all GC Studio instances are closed, or restart your PC. The update will retry on next launch.", "Oops! something didn’t go as planned.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                EndForm();
            }
            else
            {
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == true)
                {

                    debuglog("INFO GCstudio Loader, Downloading the update package...");

                    try
                    {
                        downloading = true;
                        //BtnExit.Enabled = false;
                        Version.Visible = false;
                        ProgressUpdate.Visible = true;
                        WebClientPKG.DownloadProgressChanged += OnDownloadProgressChanged;
                        WebClientPKG.DownloadFileCompleted += OnFileDownloadCompleted;
                        WebClientPKG.DownloadFileAsync(new Uri(ReleasePath + CVS.UpdateInfo.ManifestPKG), "update.pkg");
                    }
                    catch (Exception ex)
                    {

                        debuglog("ERROR GCstudio Loader, an error occurred while downloading the update pakage." + " > " + ex.Message + " @ " + ex.StackTrace);

                    }
                }
                else
                {
                    debuglog("INFO GCstudio Loader, Exiting updater...");

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
                debuglog("INFO GCstudio Loader, Update canceled by user, exiting updater...");

                MessageBox.Show("Update canceled by user.");
                EndForm();
            }
            else if (e.Error is object)
            {
                debuglog("ERROR GCstudio Loader, an error occurred while downloading the update package, exiting updater...");

                MessageBox.Show("There was a problem downloading the update.");
                EndForm();
            }
            else
            {
                debuglog("INFO GCstudio Loader, update package download complete, verifying...");

                try
                {
                    UpdateChecksum = dfe.CreateMD5Sum("update.pkg");
                    if (UpdateChecksum == CVS.UpdateInfo.ManifestChecksum)
                    {
                        debuglog("INFO GCstudio Loader, update package checksum match, starting the update process...");

                        try
                        {
                            ProcessStartInfo p = new ProcessStartInfo();
                            p.FileName = "update.exe";
                            p.Arguments = "-a";
                            p.WindowStyle = ProcessWindowStyle.Normal;
                            Process x = Process.Start(p);
                            Environment.Exit(0);
                        }
                        catch
                        {
                            debuglog("INFO GCstudio Loader, an error occurred while launching the update process, exiting updater...");

                            MessageBox.Show("Error starting the update.");
                            EndForm();
                        }
                    }
                    else
                    {
                        debuglog("ERROR GCstudio Loader, update package checksum mismatch, exiting updater...");

                        EndForm();

                    }
                }
                catch
                {
                }
                debuglog("ERROR GCstudio Loader, an error occurred while verifying the update package, exiting updater...");

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
            debuglog("INFO GCstudio Loader, minimizing the splash by user request...");

            this.WindowState = FormWindowState.Minimized;
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            debuglog("INFO GCstudio Loader, Aborting the updater by user request...");

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

        /// <summary>
        /// Debug Logger
        /// </summary>
        /// <param name="logstr"></param>
        public void debuglog(string logstr)
        {
            DataFileEngine dl = new DataFileEngine();
            try
            {
                dl.StreamW = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Log/GCstudio" + Loader.AppVer.ToString() + ".log", true);
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
