using Ngine;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;


namespace GC_Studio
{
    public partial class ToolBoxLite : Form
    {


        DataFileEngine dfe = new DataFileEngine();
        DataFileEngine debuglog = new DataFileEngine();
        JsonConvert json = new JsonConvert();
        RecentFile RecentFiles = new RecentFile();
        ConfigSchema Config = new ConfigSchema();
        string[] arguments;
        string ideargs = "";
        readonly string BugTracking = "https://www.gcbasic.com/bugtracking/bug_report_page.php";
        readonly string DonateLink = "https://paypal.me/gcbasic";
        ListViewItem[] RecentItem = new ListViewItem[10];
        NumberStyles Style = NumberStyles.AllowDecimalPoint;
        CultureInfo Provider = new CultureInfo("en-US");

        /// <summary>
        /// set focus function
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        public ToolBoxLite()
        {
            try
            {
                debuglog.LoadWrite("/Log/GCstudio" + Loader.AppVer.ToString() + ".log");
            }
            catch { }

            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, initializing main pannel...");
            }
            catch { }

            InitializeComponent();
            Application.EnableVisualStyles();

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            DragMouseDown(pictureBox1, e);
            MaxBounds();
        }




        public void DragMouseDown(Control sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Release the mouse capture started by the mouse down.

                sender.Capture = false; //select control

                // Create and send a WM_NCLBUTTONDOWN message.
                const int WM_NCLBUTTONDOWN = 0x00A1;
                const int HTCAPTION = 2;
                Message msg =
                    Message.Create(this.Handle, WM_NCLBUTTONDOWN,
                        new IntPtr(HTCAPTION), IntPtr.Zero);
                this.DefWndProc(ref msg);
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            MaxBounds();

            ver.Text = Loader.AppVer.ToString();

            LoadConfig();
            LoadRecent();

            if (Environment.OSVersion.Version.Major == 6 & Environment.OSVersion.Version.Minor < 2)
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, Windows 7 detected, enabling mainstream win7 channel...");
                }
                catch { }


                comboupdate.Items.Add("mainstream win7");
                if (Config.GCstudio.ReleaseChanel == "mainstream")
                {
                    Config.GCstudio.ReleaseChanel = "mainstream win7";
                    SaveConfig();
                    button12_Click(this, null);
                }

            }

            /// Code for name change on compiler directory
            /// 


            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "GreatCowBasic"))
            {

                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, old GreatCowBasic directory detected, started automatic migration...");
                }
                catch { }


                if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GreatCowBASIC"))
                {
                    try
                    {
                        Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\GreatCowBASIC", true);
                    }
                    catch
                    {
                    }
                }
                try
                {
                    FileSystem.MoveDirectory(AppDomain.CurrentDomain.BaseDirectory + "GreatCowBasic", AppDomain.CurrentDomain.BaseDirectory + "gcbasic", false);
                }
                catch
                {
                }
            }

            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "GreatCowBasic"))
            {
                try
                {
                    Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "GreatCowBasic", true);
                }
                catch
                {
                }
            }






            ///

            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, applying configuration...");
            }
            catch { }



            comboupdate.Text = Config.GCstudio.ReleaseChanel;
            comboide.Text = Config.GCstudio.IDE;
            if (Config.GCstudio.Legacymode)
            {
                comboide.Enabled = false;
            }
            comboarch.Text = Config.GCstudio.Architecture;
            if (Config.GCstudio.Legacymode)
            {
                combomode.Text = "Legacy";
            }
            else
            {
                combomode.Text = "Modern";
            }
            comboHide.Text = Config.GCstudio.HideDonate;


            CompilerArchitecture();


            this.Size = new Size(Config.Window.sizeW, Config.Window.sizeH);

            this.Location = new Point(Config.Window.locx, Config.Window.locy);

            if (Config.Window.maximized)
            {
                MaxBounds();
                this.WindowState = FormWindowState.Maximized;
            }

            ///post updater
            if (File.Exists("post.dat"))
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, postupdate flag detected, launching process...");
                }
                catch { }


                try
                {
                    ProcessStartInfo p = new ProcessStartInfo();
                    p.FileName = "postupdate.exe";
                    p.Arguments = "/S";
                    p.WindowStyle = ProcessWindowStyle.Normal;
                    Process x = Process.Start(p);
                    try
                    {
                        File.Delete("post.dat");
                    }
                    catch 
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio, could not clear post update flag...");
                        }
                        catch { }


                    }
                }
                catch
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio, an error ocurred while launching postupdate process, exiting GCstudio...");
                        debuglog.CloseWrite();
                    }
                    catch { }


                    MessageBox.Show("Error starting the post updater.");
                    Environment.Exit(0);
                }
            }


            ///CLI Commands
            ///
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, looking for arguments...");
            }
            catch { }


            arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, arguments detected, executing commands...");
                }
                catch { }


                switch (arguments[1])
                {
                    case "/syn" or "-syn" or "--syn":
                        this.Visible = false;
                        if (arguments.Length > 2)
                        {
                            for (int i = 2; i < arguments.Length; i++)
                            {
                                if (i == arguments.Length - 1)
                                {
                                    ideargs += "\"" + arguments[i] + "\"";
                                }
                                else
                                {
                                    ideargs += "\"" + arguments[i] + "\" ";
                                }
                            }

                            addrecent(Path.GetFileName(arguments[2]), arguments[2]);
                        }

                        LaunchIDE(ideargs, "SynWrite");
                        break;

                    case "/pkp" or "-p" or "--pkp":
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, running pkp tool and exiting...");
                            debuglog.CloseWrite();
                        }
                        catch { }


                        this.Visible = false;
                        pkptool();
                        Environment.Exit(0);
                        break;

                    case "/firststart" or "-fs" or "--firststart":
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, overriding first start");
                        }
                        catch { }


                        Config.GCstudio.Firstrun = true;

                        break;

                    case "/resetsize" or "-rs" or "--resetsize":

                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, overriding a reset size...");
                        }
                        catch { }


                        ResetSize();

                        break;

                    case "/settings" or "-s" or "--settings":

                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, overriding config panel view only...");
                        }
                        catch { }



                        panelmain.Visible = false;
                        panelnewproj.Visible = false;
                        panelconfig.Visible = true;
                        button9.Visible = false;
                        button12.Visible = false;

                        break;

                    case "/about" or "-a" or "--about":

                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, overriding about box view only...");
                        }
                        catch { }



                        Form about = new GC_Studio.AboutBox();
                        about.ShowDialog();
                        this.Close();

                        break;


                    default:

                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, parsing arguments to IDE and stay hidden...");
                        }
                        catch { }



                        this.Visible = false;
                        for (int i = 1; i < arguments.Length; i++)
                        {
                            if (i == arguments.Length - 1)
                            {
                                ideargs += "\"" + arguments[i] + "\"";
                            }
                            else
                            {
                                ideargs += "\"" + arguments[i] + "\" ";
                            }
                        }

                        addrecent(Path.GetFileName(arguments[1]), arguments[1]);

                        LaunchIDE(ideargs, Config.GCstudio.IDE);
                        break;
                }


            }
            else
            {
                if (Config.GCstudio.Legacymode && !Config.GCstudio.Firstrun)
                {
                    LaunchIDE(ideargs, "SynWrite");
                }
            }





            ///first run
            if (Config.GCstudio.Firstrun)

                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, first run flag detected, and launching accordingly...");
                }
                catch { }


            {
                ResetSize();
                Config.GCstudio.Firstrun = false;
                if (Config.GCstudio.Legacymode)
                {
                    Config.GCstudio.IDE = "SynWrite";
                    Config.GCstudio.ReleaseChanel = "compiler only";
                    Config.GCstudio.IDE = "SynWrite";
                    SaveConfig();
                    LaunchIDE("\"" + AppDomain.CurrentDomain.BaseDirectory + "gcbasic\\demos\\first-start-sample.gcb\"", "SynWrite");
                }
                else
                if (Environment.OSVersion.Version.Major == 6 & Environment.OSVersion.Version.Minor < 2)
                {
                    Config.GCstudio.IDE = "SynWrite";
                    SaveConfig();
                    LaunchIDE("\"" + AppDomain.CurrentDomain.BaseDirectory + "gcbasic\\demos\\first-start-sample.gcb\"", "SynWrite");
                }
                else
                {
                    SaveConfig();
                    LaunchIDE("\".\\gcbasic\\demos\\first-start-sample.gcb\" \".\\gcbasic\\demos\\this_is_useful_list_of_tools_for_the_ide.txt\"", "GCcode");
                }
            }

            if (Config.GCstudio.HideDonate == "Hide")
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, hidden donate button flag detected, hidding...");
                }
                catch { }


                buttondonate.Visible = false;
            }

            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, populating recent files...");
            }
            catch { }


            for (int i = 0; i < 10; i++)
            {
                if (RecentFiles.RecentDir[i] != "")
                {
                    RecentItem[i] = listViewRecent.Items.Add(RecentFiles.RecentName[i]);
                    RecentItem[i].Text = RecentFiles.RecentName[i];
                    RecentItem[i].ToolTipText = RecentFiles.RecentDir[i];
                }
            }


        }

        /// <summary>
        /// Reset size
        /// </summary>
        private void ResetSize()
        {

            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, resseting window size...");
            }
            catch { }


            Config.Window.sizeW = 1028;
            Config.Window.sizeH = 681;
            this.Size = new Size(Config.Window.sizeW, Config.Window.sizeH);
            this.CenterToScreen();
            Config.Window.locx = this.Location.X;
            Config.Window.locy = this.Location.Y;
            Config.Window.maximized = false;
            SaveConfig();
        }

        /// <summary>
        /// Load the configuration of the app 
        /// </summary>
        private void LoadConfig()
        {

            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, Load configuration started...");
            }
            catch { }


            try
            {
                if (File.Exists("GCstudio.config.json"))
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, GCstudio.config.json detected, loading and deserializing...");
                    }
                    catch { }


                    dfe.LoadRead("GCstudio.config.json");
                    Config = json.DeserializeObject<ConfigSchema>(dfe.ReadAll());
                    dfe.CloseRead();
                }
            }
            catch
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio, an error ocurred while loading configuration.");
                }
                catch { }


                MessageBox.Show("Error loading config.");
            }

        }


        /// <summary>
        /// Save configuration of the app 
        /// </summary>
        private void SaveConfig()
        {
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, save configuration started...");
            }
            catch { }


            try
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, serializing configuration and saving GCstudio.config.json...");
                }
                catch { }


                //save current config
                dfe.LoadWrite("GCstudio.config.json");
                dfe.RecordData(json.SerializeObject(Config));
                dfe.CloseWrite();
            }
            catch
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio, an error ocurred while saving configuration.");
                }
                catch { }


                MessageBox.Show("Error saving config.");
            }


        }



        /// <summary>
        /// Load the Recent List 
        /// </summary>
        private void LoadRecent()
        {
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, Loading recent file list started...");
            }
            catch { }


            try
            {
                //Load recent list
                if (File.Exists("GCstudio.mrf.json"))
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, GCstudio.mrf.json detected, loading and deserializing...");
                    }
                    catch { }


                    dfe.LoadRead("GCstudio.mrf.json");
                    RecentFiles = json.DeserializeObject<RecentFile>(dfe.ReadAll());
                    dfe.CloseRead();

                }
            }
            catch
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, an error ocurred while loading recent file list.");
                }
                catch { }


                MessageBox.Show("Error loading recent files.");
            }
        }


        /// <summary>
        /// Save recent list
        /// </summary>
        private void SaveRecent()
        {
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, save recent file list started...");
            }
            catch { }


            try
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, serializing and saving GCstudio.mrf.json...");
                }
                catch { }


                dfe.LoadWrite("GCstudio.mrf.json");
                dfe.RecordData(json.SerializeObject(RecentFiles));
                dfe.CloseWrite();
            }
            catch
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, an error ocurred while saving recent file list.");
                }
                catch { }


                MessageBox.Show("Error saving recent files.");
            }

        }

        /// <summary>
        /// Add file to recent list 
        /// </summary>
        private void addrecent(string FileName, string FullPath)
        {
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, adding file/directory to recent list...");
            }
            catch { }


            try
            {
                if (RecentFiles.RecentDir[RecentFiles.RecentN - 1] != FullPath)
                {

                    RecentFiles.RecentName[RecentFiles.RecentN] = FileName;
                    RecentFiles.RecentDir[RecentFiles.RecentN] = FullPath;
                    RecentFiles.RecentN++;
                    if (RecentFiles.RecentN > 9)
                    { RecentFiles.RecentN = 0; }
                    SaveRecent();
                }
            }
            catch
            {
                RecentFiles.RecentName[RecentFiles.RecentN] = FileName;
                RecentFiles.RecentDir[RecentFiles.RecentN] = FullPath;
                RecentFiles.RecentN++;
                if (RecentFiles.RecentN > 9)
                { RecentFiles.RecentN = 0; }
                SaveRecent();
            }

        }


        private void BtnExit_Click(object sender, EventArgs e)
        {
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, Exiting GCstudio by user request...");
            }
            catch { }


            if (this.WindowState == FormWindowState.Maximized)
            {
                Config.Window.maximized = true;
            }
            else
            {
                Config.Window.maximized = false;
                Config.Window.sizeW = this.Size.Width;
                Config.Window.sizeH = this.Size.Height;
                Config.Window.locx = this.Location.X;
                Config.Window.locy = this.Location.Y;
            }
            SaveConfig();
            debuglog.CloseWrite();
            Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, minimizing window by user request...");
            }
            catch { }


            this.WindowState = FormWindowState.Minimized;
        }

        private void BtnMini_Click(object sender, EventArgs e)
        {
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, executing maximize funtion by user request...");
            }
            catch { }


            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                MaxBounds();
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void MaxBounds()
        {
            var workingArea = System.Windows.Forms.Screen.FromHandle(Handle).WorkingArea;
            MaximizedBounds = new Rectangle(-8, 0, workingArea.Width + 16, workingArea.Height + 8);
        }

        private void buttonclone_MouseHover(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.FromArgb(34, 41, 51);
            buttonclone.BackColor = Color.FromArgb(34, 41, 51);
            label5.BackColor = Color.FromArgb(34, 41, 51);
            label6.BackColor = Color.FromArgb(34, 41, 51);
        }

        private void buttonclone_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.FromArgb(24, 31, 41);
            buttonclone.BackColor = Color.FromArgb(24, 31, 41);
            label5.BackColor = Color.FromArgb(24, 31, 41);
            label6.BackColor = Color.FromArgb(24, 31, 41);
        }


        private void buttonopensol_MouseHover(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.FromArgb(34, 41, 51);
            buttonopensol.BackColor = Color.FromArgb(34, 41, 51);
            label14.BackColor = Color.FromArgb(34, 41, 51);
            label13.BackColor = Color.FromArgb(34, 41, 51);
        }

        private void buttonopensol_MouseLeave(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.FromArgb(24, 31, 41);
            buttonopensol.BackColor = Color.FromArgb(24, 31, 41);
            label14.BackColor = Color.FromArgb(24, 31, 41);
            label13.BackColor = Color.FromArgb(24, 31, 41);
        }

        private void buttonopensol_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "")
            {
                addrecent(openFileDialog.SafeFileName, openFileDialog.FileName);

                LaunchIDE("\"" + openFileDialog.FileName + "\"", Config.GCstudio.IDE);
            }
        }

        private void buttonopenfol_MouseHover(object sender, EventArgs e)
        {
            pictureBox4.BackColor = Color.FromArgb(34, 41, 51);
            buttonopenfol.BackColor = Color.FromArgb(34, 41, 51);
            label16.BackColor = Color.FromArgb(34, 41, 51);
            label15.BackColor = Color.FromArgb(34, 41, 51);
        }

        private void buttonopenfol_MouseLeave(object sender, EventArgs e)
        {
            pictureBox4.BackColor = Color.FromArgb(24, 31, 41);
            buttonopenfol.BackColor = Color.FromArgb(24, 31, 41);
            label16.BackColor = Color.FromArgb(24, 31, 41);
            label15.BackColor = Color.FromArgb(24, 31, 41);
        }

        private void buttonopenfol_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();

            if (folderBrowserDialog.SelectedPath != "")
            {
                addrecent(folderBrowserDialog.SelectedPath.Split(Path.DirectorySeparatorChar).Last(), folderBrowserDialog.SelectedPath);

                LaunchIDE("\"" + folderBrowserDialog.SelectedPath + "\"", Config.GCstudio.IDE);

            }


        }

        private void buttonnew_MouseHover(object sender, EventArgs e)
        {
            pictureBox5.BackColor = Color.FromArgb(34, 41, 51);
            buttonnew.BackColor = Color.FromArgb(34, 41, 51);
            label18.BackColor = Color.FromArgb(34, 41, 51);
            label17.BackColor = Color.FromArgb(34, 41, 51);
        }

        private void buttonnew_MouseLeave(object sender, EventArgs e)
        {
            pictureBox5.BackColor = Color.FromArgb(24, 31, 41);
            buttonnew.BackColor = Color.FromArgb(24, 31, 41);
            label18.BackColor = Color.FromArgb(24, 31, 41);
            label17.BackColor = Color.FromArgb(24, 31, 41);
        }

        private void buttonnew_Click(object sender, EventArgs e)
        {
            textBox2.Text = Config.GCstudio.LastDirectory;
            panelmain.Visible = false;
            panelnewproj.Visible = true;
        }

        private void buttonwitout_MouseHover(object sender, EventArgs e)
        {
            buttonwitout.ForeColor = Color.White;
        }

        private void buttonwitout_MouseLeave(object sender, EventArgs e)
        {
            buttonwitout.ForeColor = Color.CornflowerBlue;
        }

        private void buttonwitout_Click(object sender, EventArgs e)
        {
            if (Config.GCstudio.IDE == "GCcode")
            {
                LaunchIDE("-n", Config.GCstudio.IDE);
            }
            else
            {
                LaunchIDE("", Config.GCstudio.IDE);
            }
        }

        private void button7_MouseHover(object sender, EventArgs e)
        {
            buttonbug.ForeColor = Color.White;
        }

        private void button7_MouseLeave(object sender, EventArgs e)
        {
            buttonbug.ForeColor = Color.CornflowerBlue;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", BugTracking);

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            panelmain.Visible = false;
            panelnewproj.Visible = false;
            panelconfig.Visible = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();
            textBox2.Text = folderBrowserDialog.SelectedPath;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            panelnewproj.Visible = false;
            panelmain.Visible = true;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox2.Text + "\\" + textBox1.Text) == false)
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, creating new project...");
                }
                catch { }



                Config.GCstudio.LastDirectory = textBox2.Text;
                SaveConfig();

                try
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, expanding project data on new process...");
                    }
                    catch { }


                    ProcessStartInfo p = new ProcessStartInfo();
                    p.FileName = "minidump.exe";
                    p.Arguments = "x \"GCBprog.tpl\" -o\"" + textBox2.Text + "\\" + textBox1.Text + "\" * -r -y";
                    p.WindowStyle = ProcessWindowStyle.Hidden;
                    p.CreateNoWindow = true;
                    Process x = Process.Start(p);
                    x.WaitForExit();
                }
                catch
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, an errror ocurred while expanding project data...");
                    }
                    catch { }


                    MessageBox.Show("Error creating project.");
                }

                if (File.Exists(textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace") == true)
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, IDE launch requested...");
                    }
                    catch { }


                    switch (Config.GCstudio.IDE)
                    {
                        case "GCcode":
                            addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace");

                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace" + "\"", Config.GCstudio.IDE);
                            break;

                        case "SynWrite":
                            addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\SynWrite Project.synw-proj");

                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\SynWrite Project.synw-proj" + "\"", Config.GCstudio.IDE);
                            break;

                        case "GCgraphical":
                            addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\main.gcb");

                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\main.gcb" + "\"", Config.GCstudio.IDE);
                            break;


                        case "Geany":
                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\"", Config.GCstudio.IDE);
                            break;
                    }
                }
            }
            else
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  WARING GCstudio, a directory with the same name of the project already exists, warning user...");
                }
                catch { }


                MessageBox.Show("A folder of the same name already exists on the location, please use a valid project name.", "Folder already exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox2.Text + "\\" + textBox1.Text) == false)
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, Creating new project...");
                }
                catch { }


                Config.GCstudio.LastDirectory = textBox2.Text;
                SaveConfig();

                try
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, expanding project data on new process...");
                    }
                    catch { }


                    ProcessStartInfo p = new ProcessStartInfo();
                    p.FileName = "minidump.exe";
                    p.Arguments = "x \"GCBLib.tpl\" -o\"" + textBox2.Text + "\\" + textBox1.Text + "\" * -r -y";
                    p.WindowStyle = ProcessWindowStyle.Hidden;
                    p.CreateNoWindow = true;
                    Process x = Process.Start(p);
                    x.WaitForExit();
                }
                catch
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio, an error ocurred while expanding project data.");
                    }
                    catch { }


                    MessageBox.Show("Error creating project.");
                }

                if (File.Exists(textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace") == true)
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, IDE launch requested...");
                    }
                    catch { }


                    switch (Config.GCstudio.IDE)
                    {
                        case "GCcode":
                            addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace");

                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace" + "\"", Config.GCstudio.IDE);
                            break;

                        case "SynWrite":
                            addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\SynWrite Project.synw-proj");

                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\SynWrite Project.synw-proj" + "\"", Config.GCstudio.IDE);
                            break;

                        case "GCgraphical":
                            addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\main.gcb");

                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\main.gcb" + "\"", Config.GCstudio.IDE);
                            break;


                        case "Geany":
                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\"", Config.GCstudio.IDE);
                            break;
                    }
                }
            }
            else
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  WARING GCstudio, a directory with the same name of the project already exists, warning user...");
                }
                catch { }


                MessageBox.Show("A folder of the same name already exists on the location, please use a valid project name.", "Folder already exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox2.Text + "\\" + textBox1.Text) == false)
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, creating new project...");
                }
                catch { }


                Config.GCstudio.LastDirectory = textBox2.Text;
                SaveConfig();

                try
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, expanding project data on new process...");
                    }
                    catch { }


                    ProcessStartInfo p = new ProcessStartInfo();
                    p.FileName = "minidump.exe";
                    p.Arguments = "x \"FBasicProg.tpl\" -o\"" + textBox2.Text + "\\" + textBox1.Text + "\" * -r -y";
                    p.WindowStyle = ProcessWindowStyle.Hidden;
                    p.CreateNoWindow = true;
                    Process x = Process.Start(p);
                    x.WaitForExit();
                }
                catch
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio, an error ocurred while expanding project data.");
                    }
                    catch { }


                    MessageBox.Show("Error creating project.");
                }


                if (File.Exists(textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace") == true)
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, IDE launch requested...");
                    }
                    catch { }


                    switch (Config.GCstudio.IDE)
                    {
                        case "GCcode":
                            addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace");

                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace" + "\"", Config.GCstudio.IDE);
                            break;

                        case "SynWrite":
                            addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\SynWrite Project.synw-proj");

                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\SynWrite Project.synw-proj" + "\"", Config.GCstudio.IDE);
                            break;

                        case "GCgraphical":
                            addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\main.gcb");

                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\main.gcb" + "\"", Config.GCstudio.IDE);
                            break;


                        case "Geany":
                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\"", Config.GCstudio.IDE);
                            break;
                    }
                }
            }
            else
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  WARING GCstudio, a directory with the same name of the project already exists, warning user...");
                }
                catch { }


                MessageBox.Show("A folder of the same name already exists on the location, please use a valid project name.", "Folder already exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox2.Text + "\\" + textBox1.Text + ".gcb") == false)
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, creating new project...");
                }
                catch { }


                Config.GCstudio.LastDirectory = textBox2.Text;
                SaveConfig();


                try
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, expanding project data...");
                    }
                    catch { }


                    File.Copy("GCBFile.tpl", textBox2.Text + "\\" + textBox1.Text + ".gcb");
                }

                catch
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio, an error ocurred while expanding project data...");
                    }
                    catch { }


                    MessageBox.Show("Error creating project.");
                }


                if (File.Exists(textBox2.Text + "\\" + textBox1.Text + ".gcb") == true)
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, IDE launch requested...");
                    }
                    catch { }


                    addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + ".gcb");

                    LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + ".gcb\"", Config.GCstudio.IDE);
                }
            }
            else
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  WARNING GCstudio, a file of the same name as the project already exist, warning user...");
                }
                catch { }


                MessageBox.Show("A file of the same name already exists on the location, please use a valid project name.", "File already exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void button9_Click(object sender, EventArgs e)
        {

            //panelclone.Visible=false;
            panelnewproj.Visible = false;
            panelconfig.Visible = false;
            panelmain.Visible = true;
        }

        private void comboupdate_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.GCstudio.ReleaseChanel = comboupdate.Text;
            SaveConfig();
        }

        private void comboide_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.GCstudio.IDE = comboide.Text;
            SaveConfig();
        }

        private void comboarch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((comboarch.Text == "x64") && (Environment.Is64BitOperatingSystem == false))
            {
                comboarch.Text = "x86";
                MessageBox.Show("Your current operating system is a 32bit variant and can't run a 64bit compiler.", "32bit Operating System", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Config.GCstudio.Architecture = comboarch.Text;
            SaveConfig();
            CompilerArchitecture();
        }


        public void LaunchIDE(string Args, string IDE)
        {
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, IDE Launch started, selecting the right one...");
            }
            catch { }


            ProcessStartInfo p = new ProcessStartInfo();
            Process x;
            switch (IDE)
            {
                case "GCcode":
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, launching GCcode...");
                    }
                    catch { }




                    try
                    {
                        p.FileName = AppDomain.CurrentDomain.BaseDirectory + "vscode\\code.exe";
                        p.Arguments = Args;
                        p.WindowStyle = ProcessWindowStyle.Normal;
                        x = Process.Start(p);
                        SetForegroundWindow(x.MainWindowHandle);
                        if (this.WindowState == FormWindowState.Maximized)
                        {
                            Config.Window.maximized = true;
                        }
                        else
                        {
                            Config.Window.maximized = false;
                            Config.Window.sizeW = this.Size.Width;
                            Config.Window.sizeH = this.Size.Height;
                            Config.Window.locx = this.Location.X;
                            Config.Window.locy = this.Location.Y;
                        }
                        SaveConfig();
                        Environment.Exit(0);
                        break;
                    }
                    catch
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, an error ocurred while launching GCcode; expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "vscode\\code.exe");
                        }
                        catch { }


                        MessageBox.Show("An error occurred when launching the IDE, expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "vscode\\code.exe");
                        break;
                    }

                case "SynWrite":
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, launching SynWrite...");
                    }
                    catch { }



                    try
                    {
                        p.FileName = AppDomain.CurrentDomain.BaseDirectory + "SynWrite\\Syn.exe";
                        p.Arguments = Args;
                        p.WindowStyle = ProcessWindowStyle.Normal;
                        x = Process.Start(p);
                        SetForegroundWindow(x.MainWindowHandle);
                        if (this.WindowState == FormWindowState.Maximized)
                        {
                            Config.Window.maximized = true;
                        }
                        else
                        {
                            Config.Window.maximized = false;
                            Config.Window.sizeW = this.Size.Width;
                            Config.Window.sizeH = this.Size.Height;
                            Config.Window.locx = this.Location.X;
                            Config.Window.locy = this.Location.Y;
                        }
                        SaveConfig();
                        Environment.Exit(0);
                        break;
                    }
                    catch
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, an error ocurred while launching SynWrite; expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "SynWrite\\Syn.exe");
                        }
                        catch { }


                        MessageBox.Show("An error occurred when launching the IDE, expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "SynWrite\\Syn.exe");
                        break;
                    }

                case "GCgraphical":
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, launching GCgraphical...");
                    }
                    catch { }


                    try
                    {
                        p.FileName = AppDomain.CurrentDomain.BaseDirectory + "gcbasic\\great cow graphical basic.exe";
                        p.Arguments = Args;
                        p.WindowStyle = ProcessWindowStyle.Normal;
                        x = Process.Start(p);
                        SetForegroundWindow(x.MainWindowHandle);
                        if (this.WindowState == FormWindowState.Maximized)
                        {
                            Config.Window.maximized = true;
                        }
                        else
                        {
                            Config.Window.maximized = false;
                            Config.Window.sizeW = this.Size.Width;
                            Config.Window.sizeH = this.Size.Height;
                            Config.Window.locx = this.Location.X;
                            Config.Window.locy = this.Location.Y;
                        }
                        SaveConfig();
                        Environment.Exit(0);
                        break;
                    }
                    catch
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, an error ocurred while launching GCgraphical; expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "gcbasic\\great cow graphical basic.exe");
                        }
                        catch { }


                        MessageBox.Show("An error occurred when launching the IDE, expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "gcbasic\\great cow graphical basic.exe");
                        break;
                    }



                case "Geany":
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, launching Geany...");
                    }
                    catch { }


                    try
                    {
                        p.FileName = AppDomain.CurrentDomain.BaseDirectory + "Geany\\bin\\Geany.exe";
                        p.Arguments = Args;
                        p.WindowStyle = ProcessWindowStyle.Normal;
                        x = Process.Start(p);
                        SetForegroundWindow(x.MainWindowHandle);
                        if (this.WindowState == FormWindowState.Maximized)
                        {
                            Config.Window.maximized = true;
                        }
                        else
                        {
                            Config.Window.maximized = false;
                            Config.Window.sizeW = this.Size.Width;
                            Config.Window.sizeH = this.Size.Height;
                            Config.Window.locx = this.Location.X;
                            Config.Window.locy = this.Location.Y;
                        }
                        SaveConfig();
                        Environment.Exit(0);
                        break;
                    }
                    catch
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, an error ocurred while launching Geany; expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "Geany\\bin\\Geany.exe");
                        }
                        catch { }


                        MessageBox.Show("An error occurred when launching the IDE, expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "Geany\\bin\\Geany.exe");
                        break;
                    }

                default:

                    break;
            }
        }


        private void buttoncompiler_MouseLeave(object sender, EventArgs e)
        {
            buttoncompiler.ForeColor = Color.CornflowerBlue;
        }

        private void buttoncompiler_MouseHover(object sender, EventArgs e)
        {
            buttoncompiler.ForeColor = Color.White;
        }

        private void buttongstools_MouseLeave(object sender, EventArgs e)
        {
            buttongstools.ForeColor = Color.CornflowerBlue;
        }

        private void buttongstools_MouseHover(object sender, EventArgs e)
        {
            buttongstools.ForeColor = Color.White;
        }

        private void buttongccode_MouseLeave(object sender, EventArgs e)
        {
            buttongccode.ForeColor = Color.CornflowerBlue;
        }

        private void buttongccode_MouseHover(object sender, EventArgs e)
        {
            buttongccode.ForeColor = Color.White;
        }

        private void buttongcbext_MouseLeave(object sender, EventArgs e)
        {
            buttongcbext.ForeColor = Color.CornflowerBlue;
        }

        private void buttongcbext_MouseHover(object sender, EventArgs e)
        {
            buttongcbext.ForeColor = Color.White;
        }

        private void buttoncompiler_Click(object sender, EventArgs e)
        {
            LaunchIDE("\"" + AppDomain.CurrentDomain.BaseDirectory + "gcbasic\\0pen VS Project.code-workspace" + "\"", "GCcode");
        }

        private void buttongstools_Click(object sender, EventArgs e)
        {
            LaunchIDE("\"" + AppDomain.CurrentDomain.BaseDirectory + "G+Stools\\Visual Studio Project.code-workspace" + "\"", "GCcode");
        }

        private void buttongccode_Click(object sender, EventArgs e)
        {
            LaunchIDE("\"" + AppDomain.CurrentDomain.BaseDirectory + "vscode\\0pen VS Project.code-workspace" + "\"", "GCcode");
        }

        private void buttongcbext_Click(object sender, EventArgs e)
        {
            LaunchIDE("\"" + AppDomain.CurrentDomain.BaseDirectory + "vscode\\data\\extensions\\MierEngineering.GreatCowBasic-1.0.0\\Visual Studio Project.code-workspace" + "\"", "GCcode");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Form about = new GC_Studio.AboutBox();
            about.ShowDialog();
        }

        private void listViewRecent_SelectedIndexChanged(object sender, EventArgs e)
        {
            LaunchIDE("\"" + listViewRecent.SelectedItems[0].ToolTipText + "\"", Config.GCstudio.IDE);
        }

        private void linkLabelclear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, clearing recent file list...");
            }
            catch { }


            try
            {
                if (File.Exists("GCstudio.mrf.json"))
                {
                    File.Delete("GCstudio.mrf.json");
                }
                listViewRecent.Items.Clear();
            }
            catch
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, an error ocurred while clearing recent file list.");
                }
                catch { }


                MessageBox.Show("Error clearing recent list.");
            }

        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            pkptool();
        }

        private void pkptool()
        {
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, starting pkp tool...");
            }
            catch { }


            MessageBox.Show("This tool will clone an existing installation of PICKitPlus on your current GC Studio installation.\r\n\r\nPress Ok and select a directory with a working installation of PICKitPlus to clone.", "PICKitPlus Clone Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            folderBrowserDialog.ShowDialog();
            if (File.Exists(folderBrowserDialog.SelectedPath + "\\PICkit2Plus.exe"))
            {
                try
                {
                    FileSystem.CopyDirectory(folderBrowserDialog.SelectedPath, AppDomain.CurrentDomain.BaseDirectory + "PICKitPlus", UIOption.AllDialogs);
                    MessageBox.Show("PICKitPlus Cloned successfully!", "PICKitPlus Clone Tool", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, pkp tool clone success.");
                    }
                    catch { }


                }
                catch
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio, pkp tool an error ocurred while cloning.");
                    }
                    catch { }


                    MessageBox.Show("An error occurred while cloning.", "PICKitPlus Clone Tool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  WARNING GCstudio, pickitplus wasn´t found, clone aborted, warning user...");
                }
                catch { }


                MessageBox.Show("PICKitPlus wasn’t found on selected directory, clone aborted.", "PICKitPlus Clone Tool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            folderBrowserDialog.SelectedPath = "";
        }

        private void CompilerArchitecture()
        {
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, identifying OS architecture...");
            }
            catch { }


            if (Environment.Is64BitOperatingSystem)
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, x64 OS detected.");
                }
                catch { }


                labelarch.Text = "x64";
            }
            else
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, x86 OS detected.");
                }
                catch { }


                labelarch.Text = "x86";
            }

            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, selecting compiler...");
            }
            catch { }


            switch (Config.GCstudio.Architecture)
            {
                case "Auto":
                    try
                    {
                        if (Environment.Is64BitOperatingSystem)
                        {
                            try
                            {
                                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, automatic selection enabled, choosing 64bit...");
                            }
                            catch { }


                            File.Copy("gcbasic\\gcbasic64.exe", "gcbasic\\gcbasic.exe", true);
                        }
                        else
                        {
                            try
                            {
                                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, automatic selection enabled, choosing 32bit...");
                            }
                            catch { }


                            File.Copy("gcbasic\\gcbasic32.exe", "gcbasic\\gcbasic.exe", true);
                        }
                    }
                    catch
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, an error ocurred while selecting compiler architecture...");
                        }
                        catch { }


                    }
                    break;

                case "x86":
                    try
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, manual architecture enabled, selecting 32bit...");
                        }
                        catch { }


                        File.Copy("gcbasic\\gcbasic32.exe", "gcbasic\\gcbasic.exe", true);
                    }
                    catch
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, an error ocurred while selecting compiler architecture...");
                        }
                        catch { }


                    }

                    break;

                case "x64":
                    try
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, manual architecture enabled, selecting 64bit...");
                        }
                        catch { }


                        File.Copy("gcbasic\\gcbasic64.exe", "gcbasic\\gcbasic.exe", true);
                    }
                    catch
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, an error ocurred while selecting compiler architecture...");
                        }
                        catch { }


                    }

                    break;

                case "Developer":
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, developer override detected, leaving current...");
                    }
                    catch { }


                    break;

                default:
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, no option detected, forcing auto...");
                    }
                    catch { }


                    try
                    {
                        if (Environment.Is64BitOperatingSystem)
                        {
                            try
                            {
                                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, selecting 64bit...");
                            }
                            catch { }


                            File.Copy("gcbasic\\gcbasic64.exe", "gcbasic\\gcbasic.exe", true);
                        }
                        else
                        {
                            try
                            {
                                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, selecting 32bit...");
                            }
                            catch { }


                            File.Copy("gcbasic\\gcbasic32.exe", "gcbasic\\gcbasic.exe", true);
                        }
                    }
                    catch
                    {
                        try
                        {
                            debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, an error ocurred while selecting compiler architecture...");
                        }
                        catch { }


                    }

                    break;
            }
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, starting file association tool...");
            }
            catch { }


            ProcessStartInfo p = new ProcessStartInfo();
            Process x;
            try
            {
                p.FileName = "fassoc.exe";
                p.Arguments = "";
                p.WindowStyle = ProcessWindowStyle.Maximized;
                p.Verb = "runas";
                p.UseShellExecute = true;
                x = Process.Start(p);
            }
            catch
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio, an error ocurred when launching file association tool.");
                }
                catch { }


                MessageBox.Show("An error occurred when launching the File Association Tool");
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, resseting programmer preferences...");
                }
                catch { }


                File.Copy("use_in_master\\use.ini", "gcbasic\\use.ini", true);
                MessageBox.Show("The Programmer Preferences has been reset successfully.", "Reset Programmer Preferences", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio, an error ocurred while reseting programmer preferences.");
                }
                catch { }


                MessageBox.Show("Error while reseting programmer preferences.");
            }
        }



        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, launching force update...");
                debuglog.CloseWrite();
            }
            catch { }


            ProcessStartInfo p = new ProcessStartInfo();
            Process x;
            try
            {
                p.FileName = "gcstudio.exe";
                p.Arguments = "/forceupdate";
                p.WindowStyle = ProcessWindowStyle.Normal;
                x = Process.Start(p);
                Environment.Exit(0);
            }
            catch
            {
                MessageBox.Show("An error occurred when launching the Force Update");
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to reset GC Studio to factory settings? This will clear all IDE user configurations, reset programmer preferences, remove all installed extensions and set GC Studio to its default settings.", "Reset To Factory Settings.", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, restore GCstudio factory settings started...");
                }
                catch { }


                try
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, reseting programmer preferences...");
                    }
                    catch { }


                    File.Copy("use_in_master\\use.ini", "gcbasic\\use.ini", true);

                }
                catch
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio, an error ocurred while reseting programmer preferences.");
                    }
                    catch { }


                    MessageBox.Show("Error while reseting programmer preferences.");
                }

                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, starting reset to factory tool...");
                }
                catch { }


                ProcessStartInfo p = new ProcessStartInfo();
                Process x;
                try
                {
                    p.FileName = "ResetToFactory.exe";
                    p.Arguments = "";
                    p.WindowStyle = ProcessWindowStyle.Maximized;
                    x = Process.Start(p);
                    Environment.Exit(0);
                }
                catch
                {
                    try
                    {
                        debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio, an error ocurred when launching the reset to factory tool.");
                    }
                    catch { }


                    MessageBox.Show("An error occurred when launching the Reset To Factory Tool");
                }
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, starting the prefseditor tool...");
            }
            catch { }


            ProcessStartInfo p = new ProcessStartInfo();
            Process x;
            try
            {
                p.FileName = AppDomain.CurrentDomain.BaseDirectory + "gcbasic\\Programmer Editor.exe";
                p.Arguments = AppDomain.CurrentDomain.BaseDirectory + "gcbasic\\use.INI";
                p.WindowStyle = ProcessWindowStyle.Maximized;
                x = Process.Start(p);
            }
            catch
            {
                try
                {
                    debuglog.RecordData(DateTime.Now.ToString() + ">>>  ERROR GCstudio, an error ocurred when launching the prefseditor tool.");
                }
                catch { }


                MessageBox.Show("An error occurred when launching the PrefsEditor Tool");
            }
        }

        private void panelmain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void combomode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combomode.Text == "Modern")
            {
                if (Config.GCstudio.Legacymode == true)
                {
                    comboide.Enabled = true;
                    Config.GCstudio.Legacymode = false;
                    Config.GCstudio.IDE = "GCcode";
                    Config.GCstudio.ReleaseChanel = "mainstream";
                    comboupdate.Text = Config.GCstudio.ReleaseChanel;
                    comboide.Text = Config.GCstudio.IDE;
                }
            }
            else
            {
                if (Config.GCstudio.Legacymode == false)
                {
                    DialogResult dialogResult = MessageBox.Show("Are you sure that you want to change to legacy mode? This will change the behavior of GC studio to open SynWrite directly at launch and you may need to access these settings from the SynWrite User Interface.", "Change to Legacy Mode", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        comboide.Enabled = false;
                        Config.GCstudio.Legacymode = true;
                        Config.GCstudio.IDE = "SynWrite";
                        Config.GCstudio.ReleaseChanel = "compiler only";
                        comboupdate.Text = Config.GCstudio.ReleaseChanel;
                        comboide.Text = Config.GCstudio.IDE;
                    }
                }

            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            try
            {
                debuglog.RecordData(DateTime.Now.ToString() + ">>>  GCstudio, opening donate link...");
            }
            catch { }


            Process.Start("explorer", DonateLink);
        }

        private void comboHide_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.GCstudio.HideDonate = comboHide.Text;
            SaveConfig();
            if (Config.GCstudio.HideDonate == "Hide")
            {
                buttondonate.Visible = false;
            }
            else
            {
                buttondonate.Visible = true;
            }
        }
    }
}



