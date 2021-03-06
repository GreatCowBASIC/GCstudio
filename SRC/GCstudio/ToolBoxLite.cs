using System;
using System.Drawing;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using DBSEngine;
using System.Reflection;
using System.Globalization;

namespace GC_Studio
{
    public partial class ToolBoxLite : Form
    {


        DBS dbs = new DBS();
        string ReleaseChanel = "mainstream";
      
        string IDE = "GCcode";
        string[] arguments;
        string ideargs = "";
        string architecture = "Auto";
        readonly string BugTracking = "https://www.gcbasic.com/bugtracking/bug_report_page.php";
        string[] RecentName = new string[10];
        string[] RecentDir = new string[10];
        int RecentN = 0;
        int sizeW;
        int sizeH;
        bool maximized = false;
        Int32 locx;
        Int32 locy;
        ListViewItem[] RecentItem = new ListViewItem[10];
        NumberStyles Style = NumberStyles.AllowDecimalPoint;
        CultureInfo Provider = new CultureInfo("en-US");

        //set focus function
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        public ToolBoxLite()
        {
            InitializeComponent();
        
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

            //         try
            //         {
            //             dbs.LoadRead("CurrentVersion.nfo");
            //             AppVer = decimal.Parse(dbs.ReadData());
            //             dbs.CloseRead();
            //         }
            //         catch
            //         {
            //            MessageBox.Show("Error reading current version manifest.");
            //             Environment.Exit(0);
            //         }
            //AppVer = decimal.Parse(Assembly.GetEntryAssembly().GetName().Version.Major.ToString() + "." + Assembly.GetEntryAssembly().GetName().Version.Minor.ToString());
            

            ver.Text = Loader.AppVer.ToString();

            LoadConfig();


           
            comboupdate.Text = ReleaseChanel;
            comboide.Text = IDE;
            comboarch.Text = architecture;    


            CompilerArchitecture();
            
            

            //window size
            if (File.Exists("lstsz.dat"))
            {
                try
                {
                    dbs.LoadRead("lstsz.dat");
                    sizeW = int.Parse(dbs.ReadData(),Style, Provider);
                    sizeH = int.Parse(dbs.ReadData(), Style, Provider);
                    try
                    {
                        locx = Int32.Parse(dbs.ReadData(), Style, Provider);
                        locy = Int32.Parse(dbs.ReadData(), Style, Provider);
                        maximized = bool.Parse(dbs.ReadData());
                        this.Location = new Point(locx, locy);
                    }
                    catch { }
                    dbs.CloseRead();                   
                }
                catch
                {

                    MessageBox.Show("Error loading last size");
                }

            }
            else
            {
              
                    sizeW = 1028;
                    sizeH = 681;
                    this.Size = new Size(sizeW, sizeH);
                    this.CenterToScreen();
                    locx = this.Location.X;
                    locy = this.Location.Y;
                    maximized = false;
                    SaveLastSize();               

            }
            this.Size = new Size(sizeW, sizeH);
            if (maximized)
            {
                MaxBounds();
                this.WindowState = FormWindowState.Maximized;
            }


            //CLI Commands
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
                        if (File.Exists("mrf.dat"))
                        {
                            try
                            {
                                File.Delete("mrf.dat");
                            }
                            catch
                            {

                            }
                        }

                        break;

                    case "/resetsize":

                        sizeW = 1028;
                        sizeH = 681;
                        this.Size = new Size(sizeW, sizeH);
                        this.CenterToScreen();
                        locx = this.Location.X;
                        locy = this.Location.Y;
                        maximized = false;
                        SaveLastSize();

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

                        LaunchIDE(ideargs, IDE);
                        break;
                }


            }


            //post updater
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

            //first run
            if (File.Exists("mrf.dat") == false)
            {
                LoadRecent();
                LaunchIDE("\".\\GreatCowBasic\\Demos\\first-start-sample.gcb\" \".\\GreatCowBasic\\Demos\\this_is_useful_list_of_tools_for_the_ide.txt\"", "GCcode");
            }
            else
            {
                LoadRecent();
            }

            for (int i = 0; i < 10; i++)
            {
                if (RecentDir[i] != "")
                {
                    RecentItem[i] = listViewRecent.Items.Add(RecentName[i]);
                    RecentItem[i].Text = RecentName[i];
                    RecentItem[i].ToolTipText = RecentDir[i];
                }
            }

        }

        /// save last size
        /// 
        private void SaveLastSize()
        {
            try
            {
                dbs.LoadWrite("lstsz.dat");
                dbs.RecordData(sizeW.ToString());
                dbs.RecordData(sizeH.ToString());
                dbs.RecordData(locx.ToString());
                dbs.RecordData(locy.ToString());
                dbs.RecordData(maximized.ToString());
                dbs.CloseWrite();
            }
            catch { MessageBox.Show("Error saving last size"); }
        }

        /// <summary>
        /// Load the configuration of the app 
        /// </summary>
        private void LoadConfig()
        {
            try
            { 
            //Load App Config
            if (File.Exists("config.ini"))
            {
                dbs.LoadRead("config.ini");
                ReleaseChanel = dbs.ReadData();
                IDE = dbs.ReadData();
                architecture = dbs.ReadData();
                dbs.CloseRead();
            }
            else
            //default App Config
            {
                dbs.LoadWrite("config.ini");
                dbs.RecordData("mainstream");
                dbs.RecordData("GCcode");
                dbs.RecordData("Auto");
                dbs.RecordData("END");
                dbs.CloseWrite();
            }
            }
            catch
            {
                MessageBox.Show("Error loading config.");
            }

            //Compat whit older config files
            if (architecture == "END")
            {
                architecture = "Auto";
                SaveConfig();
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
            dbs.LoadWrite("config.ini");
            dbs.RecordData(ReleaseChanel);
            dbs.RecordData(IDE);
            dbs.RecordData(architecture);
            dbs.RecordData("END");
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
            //Load recent list
            if (File.Exists("mrf.dat"))
            {

                dbs.LoadRead("mrf.dat");
                RecentN =int.Parse(dbs.ReadData(), Style, Provider);
                RecentName[0] = dbs.ReadData();
                RecentDir[0] = dbs.ReadData();
                RecentName[1] = dbs.ReadData();
                RecentDir[1] = dbs.ReadData();
                RecentName[2] = dbs.ReadData();
                RecentDir[2] = dbs.ReadData();
                RecentName[3] = dbs.ReadData();
                RecentDir[3] = dbs.ReadData();
                RecentName[4] = dbs.ReadData();
                RecentDir[4] = dbs.ReadData();
                RecentName[5] = dbs.ReadData();
                RecentDir[5] = dbs.ReadData();
                RecentName[6] = dbs.ReadData();
                RecentDir[6] = dbs.ReadData();
                RecentName[7] = dbs.ReadData();
                RecentDir[7] = dbs.ReadData();
                RecentName[8] = dbs.ReadData();
                RecentDir[8] = dbs.ReadData();
                RecentName[9] = dbs.ReadData();
                RecentDir[9] = dbs.ReadData();
                dbs.CloseRead();

            }
            else
            //write empty recent list
            {
                dbs.LoadWrite("mrf.dat");
                dbs.RecordData("0");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("");
                dbs.RecordData("END");
                dbs.CloseWrite();
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
            dbs.LoadWrite("mrf.dat");
            dbs.RecordData(RecentN.ToString());
            dbs.RecordData(RecentName[0]);
            dbs.RecordData(RecentDir[0]);
            dbs.RecordData(RecentName[1]);
            dbs.RecordData(RecentDir[1]);
            dbs.RecordData(RecentName[2]);
            dbs.RecordData(RecentDir[2]);
            dbs.RecordData(RecentName[3]);
            dbs.RecordData(RecentDir[3]);
            dbs.RecordData(RecentName[4]);
            dbs.RecordData(RecentDir[4]);
            dbs.RecordData(RecentName[5]);
            dbs.RecordData(RecentDir[5]);
            dbs.RecordData(RecentName[6]);
            dbs.RecordData(RecentDir[6]);
            dbs.RecordData(RecentName[7]);
            dbs.RecordData(RecentDir[7]);
            dbs.RecordData(RecentName[8]);
            dbs.RecordData(RecentDir[8]);
            dbs.RecordData(RecentName[9]);
            dbs.RecordData(RecentDir[9]);
            dbs.RecordData("END");
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
        private void addrecent(string FileName,string FullPath)
        {
            try
            {
                if (RecentDir[RecentN - 1] != FullPath)
                {

                    RecentName[RecentN] = FileName;
                    RecentDir[RecentN] = FullPath;
                    RecentN++;
                    if (RecentN > 9)
                    { RecentN = 0; }
                    SaveRecent();
                }
            }
            catch
            {
                RecentName[RecentN] = FileName;
                RecentDir[RecentN] = FullPath;
                RecentN++;
                if (RecentN > 9)
                { RecentN = 0; }
                SaveRecent();
            }

        }
    

        private void BtnExit_Click(object sender, EventArgs e)
        {
            
            if (this.WindowState == FormWindowState.Maximized)
            {
                maximized = true;
            }
            else
            {
                maximized = false;
                sizeW = this.Size.Width;
                sizeH = this.Size.Height;
                locx = this.Location.X;
                locy = this.Location.Y;
            }
            SaveLastSize();
            Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BtnMini_Click(object sender, EventArgs e)
        {
            if(this.WindowState == FormWindowState.Maximized)
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

                LaunchIDE("\"" + openFileDialog.FileName + "\"", IDE);
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

                LaunchIDE("\"" + folderBrowserDialog.SelectedPath + "\"", IDE);

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
            if (File.Exists("mrd.dat"))
            {
                dbs.LoadRead("mrd.dat");
                textBox2.Text = dbs.ReadData();
                dbs.CloseRead();
            }

            panelmain.Visible=false;
            panelnewproj.Visible=true;
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
            if (IDE == "GCcode")
            {
                LaunchIDE("-n", IDE);
            }
            else
            {
                LaunchIDE("", IDE);
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
            panelmain.Visible=false;
            panelnewproj.Visible=false;
            panelconfig.Visible=true;

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
                    dbs.LoadWrite("mrd.dat");
                    dbs.RecordData(textBox2.Text);
                    dbs.CloseWrite();
                }
                catch
                {
                    MessageBox.Show("Error saving recent directory");
                }

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
                    switch (IDE)
                    {
                        case "GCcode":
                            addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace");

                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace" + "\"", IDE);
                            break;

                        case "SynWrite":
                            addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\SynWrite Project.synw-proj");

                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\SynWrite Project.synw-proj" + "\"", IDE);
                            break;

                        case "Geany":
                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\"", IDE);
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
                try
            {
                dbs.LoadWrite("mrd.dat");
                dbs.RecordData(textBox2.Text);
                dbs.CloseWrite();
            }
            catch
            {
                MessageBox.Show("Error saving recent directory");
            }

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
                switch (IDE)
                {
                    case "GCcode":
                        addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace");

                        LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace" + "\"", IDE);
                        break;

                    case "SynWrite":
                        addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\SynWrite Project.synw-proj");
                        
                        LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\SynWrite Project.synw-proj" + "\"", IDE);
                        break;

                    case "Geany":
                        LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\"", IDE);
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
                try
            {
                dbs.LoadWrite("mrd.dat");
                dbs.RecordData(textBox2.Text);
                dbs.CloseWrite();
            }
            catch
            {
                MessageBox.Show("Error saving recent directory");
            }

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
                switch (IDE)
                {
                    case "GCcode":
                        addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace");
                        
                        LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace" + "\"", IDE);
                        break;

                    case "SynWrite":
                        addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\SynWrite Project.synw-proj");
                        
                        LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\SynWrite Project.synw-proj" + "\"", IDE);
                        break;

                    case "Geany":
                        LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\"", IDE);
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
                    try
                {
                    dbs.LoadWrite("mrd.dat");
                    dbs.RecordData(textBox2.Text);
                    dbs.CloseWrite();
                }
                catch
                {
                    MessageBox.Show("Error saving recent directory");
                }


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
                    
                        LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + ".gcb\"", IDE);
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
            ReleaseChanel = comboupdate.Text;
            SaveConfig();
        }

        private void comboide_SelectedIndexChanged(object sender, EventArgs e)
        {
           IDE=comboide.Text;
            SaveConfig();
        }

        private void comboarch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((comboarch.Text == "x64") && (Environment.Is64BitOperatingSystem == false))
            {
                comboarch.Text = "x86";
                MessageBox.Show("Your current operating system is a 32bit variant and can't run a 64bit compiler.", "32bit Operating System", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            architecture = comboarch.Text;
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
                            maximized = true;
                        }
                        else
                        {
                            maximized = false;
                            sizeW = this.Size.Width;
                            sizeH = this.Size.Height;
                            locx = this.Location.X;
                            locy = this.Location.Y;
                        }
                        SaveLastSize();
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
                            maximized = true;
                        }
                        else
                        {
                            maximized = false;
                            sizeW = this.Size.Width;
                            sizeH = this.Size.Height;
                            locx = this.Location.X;
                            locy = this.Location.Y;
                        }
                        SaveLastSize();
                        Environment.Exit(0);
                    break;
                    }
                    catch
                    {
                        MessageBox.Show("An error occurred when launching the IDE, expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "SynWrite\\Syn.exe");
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
                            maximized = true;
                        }
                        else
                        {
                            maximized = false;
                            sizeW = this.Size.Width;
                            sizeH = this.Size.Height;
                            locx = this.Location.X;
                            locy = this.Location.Y;
                        }
                        SaveLastSize();
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
            LaunchIDE("\"" +  AppDomain.CurrentDomain.BaseDirectory + "GreatCowBasic\\0pen VS Project.code-workspace" + "\"", "GCcode");
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
            LaunchIDE("\"" + listViewRecent.SelectedItems[0].ToolTipText + "\"", IDE);
        }

        private void linkLabelclear_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            { 
            File.Delete("mrf.dat");
            LoadRecent();
            LoadRecent();
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
            MessageBox.Show("This tool will clone an existing installation of PICKitPlus on your current GC Studio installation.\r\n\r\nPress Ok and select a directory whit a working installation of PICKitPlus to clone.", "PICKitPlus Clone Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            switch (architecture)
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
            if (MessageBox.Show("Do you really want to reset GC Studio to factory settings? This will clear all IDE user configurations, reset programmer preferences, remove all installed extensions and set GC Studio to its default settings.","Reset To Factory Settings.", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
    }


}
