using DBSEngine;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace GC_Studio
{
    public partial class ToolBoxLite : Form
    {


        DBS dbs = new DBS();
        RecentFile RecentFiles = new RecentFile();
        ConfigSchema Config = new ConfigSchema();
        string[] arguments;
        string ideargs = "";
        readonly string BugTracking = "https://www.gcbasic.com/bugtracking/bug_report_page.php";
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

            comboupdate.Text = Config.GCstudio.ReleaseChanel;
            comboide.Text = Config.GCstudio.IDE;
            comboarch.Text = Config.GCstudio.Architecture;

            CompilerArchitecture();



            //window size old file
            if (File.Exists("lstsz.dat"))
            {
                try
                {
                    dbs.LoadRead("lstsz.dat");
                    Config.Window.sizeW = int.Parse(dbs.ReadData(), Style, Provider);
                    Config.Window.sizeH = int.Parse(dbs.ReadData(), Style, Provider);
                    try
                    {
                        Config.Window.locx = Int32.Parse(dbs.ReadData(), Style, Provider);
                        Config.Window.locy = Int32.Parse(dbs.ReadData(), Style, Provider);
                        Config.Window.maximized = bool.Parse(dbs.ReadData());
                        this.Location = new Point(Config.Window.locx, Config.Window.locy);
                    }
                    catch { }
                    dbs.CloseRead();
                    File.Delete("lstsz.dat");
                }
                catch
                {

                    MessageBox.Show("Error loading last size");
                }

            }

            this.Size = new Size(Config.Window.sizeW, Config.Window.sizeH);

            this.Location = new Point(Config.Window.locx, Config.Window.locy);

            if (Config.Window.maximized)
            {
                MaxBounds();
                this.WindowState = FormWindowState.Maximized;
            }


            ///CLI Commands
            arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                switch (arguments[1])
                {
                    case "/syn":
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

                    case "/pkp":
                        this.Visible = false;
                        pkptool();
                        Environment.Exit(0);
                        break;

                    case "/firststart":
                        Config.GCstudio.Firstrun = true;

                        break;

                    case "/resetsize":

                        ResetSize();

                        break;


                    default:
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


            ///post updater
            if (File.Exists("post.dat"))
            {
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
                    catch { }
                }
                catch
                {
                    MessageBox.Show("Error starting the post updater.");
                    Environment.Exit(0);
                }
            }

            ///first run
            if (Config.GCstudio.Firstrun)
            {
                ResetSize();
                Config.GCstudio.Firstrun = false;
                
                if (Environment.OSVersion.Version.Major == 6 & Environment.OSVersion.Version.Minor < 2)
                {
                    Config.GCstudio.IDE = "SynWrite";
                    SaveConfig();
                    LaunchIDE("\"" + AppDomain.CurrentDomain.BaseDirectory + "GreatCowBasic\\first_sample\\first-start-sample.gcb\"", "SynWrite");
                }
                else
                {
                    SaveConfig();
                    LaunchIDE("\".\\GreatCowBasic\\first_sample\\first-start-sample.gcb\" \".\\GreatCowBasic\\first_sample\\this_is_useful_list_of_tools_for_the_ide.txt\"", "GCcode");
                }
            }


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
                //Load old App Config and save new
                if (File.Exists("config.ini"))
                {
                    dbs.LoadRead("config.ini");
                    Config.GCstudio.ReleaseChanel = dbs.ReadData();
                    Config.GCstudio.IDE = dbs.ReadData();
                    Config.GCstudio.Architecture = dbs.ReadData();
                    Config.GCstudio.Firstrun = false;
                    dbs.CloseRead();
                    SaveConfig();
                    File.Delete("config.ini");
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
        /// Save configuration of the app 
        /// </summary>
        private void SaveConfig()
        {
            try
            {
                //save current config
                dbs.LoadWrite("GCstudio.config.json");
                dbs.RecordData(JsonConvert.SerializeObject(Config, Formatting.Indented));
                dbs.CloseWrite();
            }
            catch
            {
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
                if (File.Exists("mrf.dat"))
                {
                    File.Delete("mrf.dat");
                }
                //Load recent list
                if (File.Exists("GCstudio.mrf.json"))
                {

                    dbs.LoadRead("GCstudio.mrf.json");
                    RecentFiles = JsonConvert.DeserializeObject<RecentFile>(dbs.ReadAll());
                    dbs.CloseRead();

                }
            }
            catch
            {
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
                dbs.LoadWrite("GCstudio.mrf.json");
                dbs.RecordData(JsonConvert.SerializeObject(RecentFiles, Formatting.Indented));
                dbs.CloseWrite();
            }
            catch
            {
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BtnMini_Click(object sender, EventArgs e)
        {
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


                Config.GCstudio.LastDirectory = textBox2.Text;
                SaveConfig();

                try
                {
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
                    MessageBox.Show("Error creating project.");
                }

                if (File.Exists(textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace") == true)
                {
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
                MessageBox.Show("A folder of the same name already exists on the location, please use a valid project name.", "Folder already exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox2.Text + "\\" + textBox1.Text) == false)
            {
                Config.GCstudio.LastDirectory = textBox2.Text;
                SaveConfig();

                try
                {
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
                    MessageBox.Show("Error creating project.");
                }

                if (File.Exists(textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace") == true)
                {
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
                MessageBox.Show("A folder of the same name already exists on the location, please use a valid project name.", "Folder already exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox2.Text + "\\" + textBox1.Text) == false)
            {
                Config.GCstudio.LastDirectory = textBox2.Text;
                SaveConfig();

                try
                {
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
                    MessageBox.Show("Error creating project.");
                }


                if (File.Exists(textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace") == true)
                {
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
                MessageBox.Show("A folder of the same name already exists on the location, please use a valid project name.", "Folder already exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox2.Text + "\\" + textBox1.Text + ".gcb") == false)
            {
                Config.GCstudio.LastDirectory = textBox2.Text;
                SaveConfig();


                try
                {
                    File.Copy("GCBFile.tpl", textBox2.Text + "\\" + textBox1.Text + ".gcb");
                }

                catch
                {
                    MessageBox.Show("Error creating project.");
                }


                if (File.Exists(textBox2.Text + "\\" + textBox1.Text + ".gcb") == true)
                {
                    addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + ".gcb");

                    LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + ".gcb\"", Config.GCstudio.IDE);
                }
            }
            else
            {
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
            ProcessStartInfo p = new ProcessStartInfo();
            Process x;
            switch (IDE)
            {
                case "GCcode":



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
                        MessageBox.Show("An error occurred when launching the IDE, expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "vscode\\code.exe");
                        break;
                    }

                case "SynWrite":


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
                        MessageBox.Show("An error occurred when launching the IDE, expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "SynWrite\\Syn.exe");
                        break;
                    }

                case "GCgraphical":

                    try
                    {
                        p.FileName = AppDomain.CurrentDomain.BaseDirectory + "GreatCowBASIC\\great cow graphical basic.exe";
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
                        MessageBox.Show("An error occurred when launching the IDE, expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "GreatCowBASIC\\great cow graphical basic.exe");
                        break;
                    }



                case "Geany":
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
            LaunchIDE("\"" + AppDomain.CurrentDomain.BaseDirectory + "GreatCowBasic\\0pen VS Project.code-workspace" + "\"", "GCcode");
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
                if (File.Exists("GCstudio.mrf.json"))
                {
                    File.Delete("GCstudio.mrf.json");
                }
                listViewRecent.Items.Clear();
            }
            catch
            {
                MessageBox.Show("Error clearing recent list.");
            }

        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            pkptool();
        }

        private void pkptool()
        {
            MessageBox.Show("This tool will clone an existing installation of PICKitPlus on your current GC Studio installation.\r\n\r\nPress Ok and select a directory with a working installation of PICKitPlus to clone.", "PICKitPlus Clone Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            folderBrowserDialog.ShowDialog();
            if (File.Exists(folderBrowserDialog.SelectedPath + "\\PICkit2Plus.exe"))
            {
                try
                {
                    FileSystem.CopyDirectory(folderBrowserDialog.SelectedPath, AppDomain.CurrentDomain.BaseDirectory + "PICKitPlus", UIOption.AllDialogs);
                    MessageBox.Show("PICKitPlus Cloned successfully!", "PICKitPlus Clone Tool", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                catch
                {
                    MessageBox.Show("An error occurred while cloning.", "PICKitPlus Clone Tool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("PICKitPlus wasn’t found on selected directory, clone aborted.", "PICKitPlus Clone Tool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            folderBrowserDialog.SelectedPath = "";
        }

        private void CompilerArchitecture()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                labelarch.Text = "x64";
            }
            else
            {
                labelarch.Text = "x86";
            }

            switch (Config.GCstudio.Architecture)
            {
                case "Auto":
                    try
                    {
                        if (Environment.Is64BitOperatingSystem)
                        {
                            File.Copy("GreatCowBasic\\gcbasic64.exe", "GreatCowBasic\\gcbasic.exe", true);
                        }
                        else
                        {
                            File.Copy("GreatCowBasic\\gcbasic32.exe", "GreatCowBasic\\gcbasic.exe", true);
                        }
                    }
                    catch
                    {

                    }
                    break;

                case "x86":
                    try
                    {
                        File.Copy("GreatCowBasic\\gcbasic32.exe", "GreatCowBasic\\gcbasic.exe", true);
                    }
                    catch
                    {

                    }

                    break;

                case "x64":
                    try
                    {
                        File.Copy("GreatCowBasic\\gcbasic64.exe", "GreatCowBasic\\gcbasic.exe", true);
                    }
                    catch
                    {

                    }

                    break;

                case "Developer":

                    break;

                default:
                    try
                    {
                        if (Environment.Is64BitOperatingSystem)
                        {
                            File.Copy("GreatCowBasic\\gcbasic64.exe", "GreatCowBasic\\gcbasic.exe", true);
                        }
                        else
                        {
                            File.Copy("GreatCowBasic\\gcbasic32.exe", "GreatCowBasic\\gcbasic.exe", true);
                        }
                    }
                    catch
                    {

                    }

                    break;
            }
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
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
                MessageBox.Show("An error occurred when launching the File Association Tool");
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                File.Copy("use_in_master\\use.ini", "GreatCowBasic\\use.ini", true);
                MessageBox.Show("The Programmer Preferences has been reset successfully.", "Reset Programmer Preferences", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch
            {
                MessageBox.Show("Error while reseting programmer preferences.");
            }
        }



        private void button12_Click(object sender, EventArgs e)
        {
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
                    File.Copy("use_in_master\\use.ini", "GreatCowBasic\\use.ini", true);

                }
                catch
                {
                    MessageBox.Show("Error while reseting programmer preferences.");
                }

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
                    MessageBox.Show("An error occurred when launching the Reset To Factory Tool");
                }
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            ProcessStartInfo p = new ProcessStartInfo();
            Process x;
            try
            {
                p.FileName = AppDomain.CurrentDomain.BaseDirectory + "greatcowbasic\\Programmer Editor.exe";
                p.Arguments = AppDomain.CurrentDomain.BaseDirectory + "greatcowbasic\\use.INI";
                p.WindowStyle = ProcessWindowStyle.Maximized;
                x = Process.Start(p);
            }
            catch
            {
                MessageBox.Show("An error occurred when launching the PrefsEditor Tool");
            }
        }

        private void panelmain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {
            ProcessStartInfo p = new ProcessStartInfo();
            Process x;
            try
            {
                p.FileName = AppDomain.CurrentDomain.BaseDirectory + "greatcowbasic\\Download_Demos.bat";
                p.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + "greatcowbasic\\";
                p.Arguments = "";
                p.WindowStyle = ProcessWindowStyle.Normal;
                x = Process.Start(p);
            }
            catch
            {
                MessageBox.Show("An error occurred when launching the Script");
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            ProcessStartInfo p = new ProcessStartInfo();
            Process x;
            try
            {
                p.FileName = AppDomain.CurrentDomain.BaseDirectory + "greatcowbasic\\Update_Demos.bat";
                p.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory + "greatcowbasic\\";
                p.Arguments = "";
                p.WindowStyle = ProcessWindowStyle.Normal;
                x = Process.Start(p);
            }
            catch
            {
                MessageBox.Show("An error occurred when launching the Script");
            }
        }
    }
    }



