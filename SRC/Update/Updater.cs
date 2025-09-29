using Ngine;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Update
{
    public partial class Updater : Form
    {
        DataFileEngine dfe = new DataFileEngine();
        JsonConvert json = new JsonConvert();
        UpdateManifest CVS = new UpdateManifest();
        public const double AppVer = 1.0204;
        string[] arguments;

        public Updater()
        {
            debuglog(null);
            debuglog(null);
            debuglog("INFO Updater, starting Updater, initializing...");

            InitializeComponent();
            this.Visible = false;
            this.Shown += Updater_Shown;
        }

        private async void Updater_Shown(object sender, EventArgs e)
        {
            // Ensure all pending UI events are processed
            Application.DoEvents();

            // Smart delay: Wait for 200ms (adjust as needed)
            await Task.Delay(200);

            // Now start the update process
            StartUpdate();
        }

        private void Updater_Load(object sender, EventArgs e)
        {
            debuglog("INFO Updater, setting round corners on splash...");
            RoundCorners(this);

            arguments = Environment.GetCommandLineArgs();

            debuglog("INFO Updater, looking for arguments...");

            if (arguments.Length > 1)
            {
                debuglog("INFO Updater, arguments found...");

                switch (arguments[1])
                {
                    case "/apply" or "-a" or "--apply":
                        this.Visible = true;

                        try
                        {
                            debuglog("INFO Updater, looking for process Code and GCstudio, trying to kill...");
                            foreach (var process in Process.GetProcessesByName("Code"))
                            {
                                process.Kill();
                            }
                            foreach (var process in Process.GetProcessesByName("GCstudio"))
                            {
                                process.Kill();
                            }
                            // InitialDelay removed; update will start in Updater_Shown
                        }
                        catch (Exception ex)
                        {
                            debuglog("CRITICAL Updater, an error ocurred while killing process, exiting." + " > " + ex.Message + " @ " + ex.StackTrace);
                            MessageBox.Show("An error ocurred while updating, reboot your system and try again.","Error while updating",MessageBoxButtons.OK,MessageBoxIcon.Error);
                            Environment.Exit(0);
                        }

                        break;

                    default:
                        debuglog("ERROR Updater, Unknown argument, Exiting, argument=" + arguments[1]);
                        Environment.Exit(0);
                        break;
                }
            }
            else
            {
                debuglog("ERROR Updater, no argument detected, exiting");
                Environment.Exit(0);
            }
        }

        private void StartUpdate()
        {
            debuglog("INFO Updater, starting the update, looking for CVS manifest...");

            try
            { 
                if (File.Exists("cvs.json"))
                {
                    debuglog("INFO Updater, CVS manifest detected, loading and deserializing it...");
                    dfe.LoadRead("cvs.json");
                    CVS = json.DeserializeObject<UpdateManifest>(dfe.ReadAll());
                    dfe.CloseRead();
                    debuglog("DEBUG Updater, CVS.UpdateInfo.ManifestVer=" + CVS.UpdateInfo.ManifestVer);
                    debuglog("DEBUG Updater, CVS.UpdateInfo.ManifestMinVer=" + CVS.UpdateInfo.ManifestMinVer);
                    debuglog("DEBUG Updater, CVS.UpdateInfo.ManifestChecksumr=" + CVS.UpdateInfo.ManifestChecksum);
                    debuglog("DEBUG Updater, CVS.UpdateInfo.ManifestPKG=" + CVS.UpdateInfo.ManifestPKG);
                    debuglog("DEBUG Updater, CVS.UpdateInfo.ManifestTitle=" + CVS.UpdateInfo.ManifestTitle);
                    debuglog("DEBUG Updater, CVS.UpdateInfo.ManifestNotes=" + CVS.UpdateInfo.ManifestNotes);
                    debuglog("DEBUG Updater, CVS.UpdateInfo.AppExe=" + CVS.UpdateInfo.AppExe);
                }
                else
                {
                    debuglog("WARNING Updater, CVS manifest not found, aborting update...");
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                debuglog("CRITICAL Updater, an error ocurred while loading the CBS manifest, exiting." + " > " + ex.Message + " @ " + ex.StackTrace);
                MessageBox.Show("An error ocurred while updating, try again later.", "Error while updating", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                System.IO.File.Delete("cvs.json");
            }
            catch(Exception ex) 
            {
                debuglog("ERROR Updater, an error occurred while removing the CVS manifest..." + " > " + ex.Message + " @ " + ex.StackTrace);
            }

            try
            {
                debuglog("INFO Updater, update finished, starting the application...");
                ProcessStartInfo p = new ProcessStartInfo();
                p.FileName = CVS.UpdateInfo.AppExe;
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
