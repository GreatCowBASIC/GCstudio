using Microsoft.VisualBasic.FileIO;
using Ngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows.Forms;


namespace GC_Studio
{
    public partial class ToolBoxLite : Form
    {


        DataFileEngine dfe = new DataFileEngine();
        JsonConvert json = new JsonConvert();
        RecentFile RecentFiles = new RecentFile();
        ConfigSchema Config = new ConfigSchema();
        string[] arguments;
        string ideargs = "";
        string GCcodeVer;
        readonly string BugTracking = "https://www.gcbasic.com/bugtracking/bug_report_page.php";
        readonly string Demonstrations = "https://sourceforge.net/projects/gcbasic/files/GCStudio%20-%20Complete%20IDE%20and%20Toolchain%20for%20Windows/GCBdemonstrationsPack.exe/download";
        readonly string DonateLink = "https://paypal.me/gcbasic";
        ListViewItem[] RecentItem = new ListViewItem[10];
        CultureInfo Provider = new CultureInfo("en-US");
        List<Module> modulesList = new List<Module>();

        /// <summary>
        /// set focus function
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        public ToolBoxLite()
        {
            debuglog("INFO GCstudio, initializing main panel...");

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
                Message msg = Message.Create(this.Handle, WM_NCLBUTTONDOWN, new IntPtr(HTCAPTION), IntPtr.Zero);
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
                debuglog("INFO GCstudio, Windows 7 OS detected, enabling mainstream win7 channel...");

                comboupdate.Items.Add("mainstream win7");
                if (Config.GCstudio.ReleaseChanel == "mainstream")
                {
                    Config.GCstudio.ReleaseChanel = "mainstream win7";
                    SaveConfig();
                    button12_Click(this, null);
                }

            }


            debuglog("INFO GCstudio, applying configuration...");

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


            ///Task.json compilation
            ///
            try
            {
                debuglog("INFO GCstudio, compiling task.json for GCcode...");

                string targetFile = AppDomain.CurrentDomain.BaseDirectory + "vscode/data/user-data/user/tasks.json";
                string[] sourceFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "tasks", "*.json");

                var allTasks = new List<JsonElement>();
                string version = "2.0.0";

                foreach (string file in sourceFiles)
                {
                    string content = File.ReadAllText(file);
                    try
                    {
                        using (JsonDocument doc = JsonDocument.Parse(content))
                        {
                            if (version == "2.0.0" && doc.RootElement.TryGetProperty("version", out var verProp))
                            {
                                version = verProp.GetString() ?? "2.0.0";
                            }

                            if (doc.RootElement.TryGetProperty("tasks", out var tasksProp) && tasksProp.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var task in tasksProp.EnumerateArray())
                                {
                                    allTasks.Add(task.Clone());
                                }
                            }
                        }

                        // VScode tasks.json deployment
                        if (Config.GCstudio.IDE == "VScode")
                        {
                            try
                            {
                                string appDataCodeUserPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Code", "User", "tasks.json");
                                debuglog($"INFO GCstudio, Deploying Tasks.json to VScode: {appDataCodeUserPath}");
                                File.Copy(targetFile, appDataCodeUserPath, true);
                                debuglog("INFO GCstudio, Tasks.json deployed successfully.");
                            }
                            catch (Exception ex)
                            {
                                debuglog($"ERROR GCstudio, failed to deploy Tasks.json to VScode: {ex.Message} @ {ex.StackTrace}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        debuglog($"ERROR GCstudio, failed to parse {file}: {ex.Message}");
                    }
                }

                var output = new
                {
                    version,
                    tasks = allTasks
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string combinedJson = JsonSerializer.Serialize(output, options);
                File.WriteAllText(targetFile, combinedJson, Encoding.UTF8);

     
            }
            catch (Exception ex)
            {
                debuglog("ERROR GCstudio, an error occurred while compiling task.json" + " > " + ex.Message + " @ " + ex.StackTrace);
            }


            ///IntelliSenseGCB.json compilation
            ///
            try
            {
                debuglog("INFO GCstudio, compiling IntelliSenseGCB.json for GCcode...");

                string targetFile = AppDomain.CurrentDomain.BaseDirectory + "vscode/data/extensions/MierEngineering.GreatCowBasic-1.0.0/dist/IntelliSenseGCB.json";
                string[] sourceFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "IntelliSense", "*.json");


                // Dictionary to hold merged properties
                var merged = new Dictionary<string, object>();

                foreach (string file in sourceFiles)
                {
                    string content = File.ReadAllText(file);
                    using (JsonDocument doc = JsonDocument.Parse(content))
                    {
                        foreach (var prop in doc.RootElement.EnumerateObject())
                        {
                            if (prop.Value.ValueKind == JsonValueKind.Array)
                            {
                                // Merge arrays: concatenate items
                                if (!merged.ContainsKey(prop.Name))
                                    merged[prop.Name] = new List<JsonElement>();

                                var arr = (List<JsonElement>)merged[prop.Name];
                                foreach (var item in prop.Value.EnumerateArray())
                                    arr.Add(item.Clone());
                            }
                            else if (prop.Value.ValueKind == JsonValueKind.Object)
                            {
                                // Merge objects: merge by key
                                if (!merged.ContainsKey(prop.Name))
                                    merged[prop.Name] = new Dictionary<string, JsonElement>();

                                var dict = (Dictionary<string, JsonElement>)merged[prop.Name];
                                foreach (var objProp in prop.Value.EnumerateObject())
                                {
                                    dict[objProp.Name] = objProp.Value.Clone();
                                }
                            }
                            else
                            {
                                // For primitive values, keep the first encountered
                                if (!merged.ContainsKey(prop.Name))
                                    merged[prop.Name] = prop.Value.Clone();
                            }
                        }
                    }
                }

                // Build output object for serialization
                using var memoryStream = new MemoryStream();
                using (var docBuilder = new Utf8JsonWriter(memoryStream, new JsonWriterOptions { Indented = true }))
                {
                    docBuilder.WriteStartObject();
                    foreach (var kvp in merged)
                    {
                        if (kvp.Value is List<JsonElement> arr)
                        {
                            docBuilder.WritePropertyName(kvp.Key);
                            docBuilder.WriteStartArray();
                            foreach (var item in arr)
                                item.WriteTo(docBuilder);
                            docBuilder.WriteEndArray();
                        }
                        else if (kvp.Value is Dictionary<string, JsonElement> dict)
                        {
                            docBuilder.WritePropertyName(kvp.Key);
                            docBuilder.WriteStartObject();
                            foreach (var objKvp in dict)
                            {
                                docBuilder.WritePropertyName(objKvp.Key);
                                objKvp.Value.WriteTo(docBuilder);
                            }
                            docBuilder.WriteEndObject();
                        }
                        else if (kvp.Value is JsonElement elem)
                        {
                            docBuilder.WritePropertyName(kvp.Key);
                            elem.WriteTo(docBuilder);
                        }
                    }
                    docBuilder.WriteEndObject();
                    docBuilder.Flush();

                    File.WriteAllText(targetFile, Encoding.UTF8.GetString(memoryStream.ToArray()));
                }

                debuglog("INFO GCstudio, IntelliSenseGCB.json compiled successfully.");


                // VScode IntelliSenseGCB.json deployment
                if (Config.GCstudio.IDE == "VScode")
                {
                    try
                    {

                        string appDataCodeUserPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".vscode", "extensions", "mierengineering.gcbasic-1.0.1", "dist", "IntelliSenseGCB.json");
                        debuglog($"INFO GCstudio, Deploying IntelliSenseGCB.json to VScode: {appDataCodeUserPath}");
                        File.Copy(targetFile, appDataCodeUserPath, true);
                        debuglog("INFO GCstudio, IntelliSenseGCB.json deployed successfully.");
                    }
                    catch (Exception ex)
                    {
                        debuglog($"ERROR GCstudio, failed to deploy IntelliSenseGCB.json to VScode: {ex.Message} @ {ex.StackTrace}");
                    }
                }


            }
            catch (Exception ex)
            {
                debuglog("ERROR GCstudio, an error occurred while compiling IntelliSenseGCB.json" + " > " + ex.Message + " @ " + ex.StackTrace);
            }



            ///Auto architecture detection
            CompilerArchitecture();


            ///Window size and position
            this.Size = new Size(Config.Window.sizeW, Config.Window.sizeH);

            this.Location = new Point(Config.Window.locx, Config.Window.locy);

            if (Config.Window.maximized)
            {
                MaxBounds();
                this.WindowState = FormWindowState.Maximized;
            }

            ///Load current modules.json
            // Load modules.json
            string modulesJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", "modules.json");
            if (File.Exists(modulesJsonPath))
            {
                try
                {
                    string json = File.ReadAllText(modulesJsonPath);
                    modulesList = JsonSerializer.Deserialize<List<Module>>(json) ?? new List<Module>();
                    debuglog("INFO GCstudio, Loaded modules.json successfully.");
                }
                catch (Exception ex)
                {
                    modulesList = new List<Module>();
                    debuglog("ERROR GCstudio, Failed to deserialize modules.json. > " + ex.Message);
                }
            }
            else
            {
                modulesList = new List<Module>();
                debuglog("INFO GCstudio, modules.json not found, starting with empty list.");
            }

            ///post updater and Module deployment
            if (File.Exists("post.dat"))
            {
                debuglog("INFO GCstudio, post update flag detected, launching process...");

                try
                {
                    ProcessStartInfo p = new ProcessStartInfo();
                    p.FileName = "postupdate.exe";
                    p.Arguments = "/S";
                    p.WindowStyle = ProcessWindowStyle.Normal;
                    p.UseShellExecute = true;
                    p.Verb = "runas";
                    Process x = Process.Start(p);
                    try
                    {
                        File.Delete("post.dat");
                    }
                    catch (Exception ex)
                    {
                        debuglog("ERROR GCstudio, could not clear post update flag." + " > " + ex.Message + " @ " + ex.StackTrace);
                    }
                }
                catch (Exception ex2)
                {
                    debuglog("CRITICAL GCstudio, an error occurred while launching post update process, exiting GCstudio." + " > " + ex2.Message + " @ " + ex2.StackTrace);

                    MessageBox.Show("Error starting the post updater.");
                    Environment.Exit(0);
                }

                // Extract and deploy all enabled modules
                debuglog("INFO GCstudio, post update flag detected, extracting all enabled modules...");
                foreach (var module in modulesList.Where(m => m.Enabled))
                {

                    DeleteCurrentModuleScripts();

                    string mpkPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", module.Name);
                    if (File.Exists(mpkPath))
                    {
                        try
                        {
                            ProcessStartInfo p = new ProcessStartInfo();
                            p.FileName = "minidump.exe";
                            p.Arguments = $"x \"{mpkPath}\" -y";
                            p.WindowStyle = ProcessWindowStyle.Hidden;
                            p.CreateNoWindow = true;
                            Process x = Process.Start(p);
                            x.WaitForExit();

                            module.Deployed = true;
                            debuglog($"INFO GCstudio, Extracted and deployed module '{module.Name}'.");

                            // Check for deployment scripts
                            string scriptBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", "Scripts", "Script_dep");
                            string[] scriptExtensions = new[] { ".exe", ".ps1", ".bat" };
                            bool scriptExecuted = false;

                            foreach (var ext in scriptExtensions)
                            {
                                string scriptPath = scriptBase + ext;
                                if (File.Exists(scriptPath))
                                {
                                    try
                                    {
                                        ProcessStartInfo scriptProc = new ProcessStartInfo();
                                        scriptProc.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                                        if (ext == ".exe")
                                        {
                                            scriptProc.FileName = scriptPath;
                                            scriptProc.Arguments = "";
                                        }
                                        else if (ext == ".ps1")
                                        {
                                            scriptProc.FileName = "powershell.exe";
                                            scriptProc.Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"";
                                        }
                                        else if (ext == ".bat")
                                        {
                                            scriptProc.FileName = "cmd.exe";
                                            scriptProc.Arguments = $"/c \"{scriptPath}\"";
                                        }
                                        scriptProc.WindowStyle = ProcessWindowStyle.Hidden;
                                        scriptProc.CreateNoWindow = true;
                                        Process scriptProcess = Process.Start(scriptProc);
                                        scriptProcess.WaitForExit();
                                        debuglog($"INFO GCstudio, Executed deployment script '{scriptPath}' for module '{module.Name}'.");
                                        scriptExecuted = true;
                                        break; // Only execute the first found script
                                    }
                                    catch (Exception ex)
                                    {
                                        debuglog($"ERROR GCstudio, Failed to execute deployment script '{scriptPath}' for module '{module.Name}'. > {ex.Message}");
                                    }
                                }
                            }
                            if (!scriptExecuted)
                            {
                                debuglog($"INFO GCstudio, No deployment script found for module '{module.Name}'.");
                            }

                        }
                        catch (Exception ex)
                        {
                            debuglog($"ERROR GCstudio, Failed to extract module '{module.Name}'. > {ex.Message}");
                        }
                    }
                    else
                    {
                        debuglog($"WARN GCstudio, Module file '{mpkPath}' not found for extraction.");
                    }
                }
                // Update modules.json after extraction
                try
                {
                    string updatedJson = JsonSerializer.Serialize(modulesList, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(modulesJsonPath, updatedJson);
                    debuglog("INFO GCstudio, Updated modules.json after deployment.");
                }
                catch (Exception ex)
                {
                    debuglog("ERROR GCstudio, Failed to update modules.json after deployment. > " + ex.Message);
                }
                DeleteCurrentModuleScripts();
            }
            else
            {
                // Extract and deploy all enabled but not yet deployed modules
                debuglog("INFO GCstudio, post update flag not detected, extracting enabled but not deployed modules...");
                foreach (var module in modulesList.Where(m => m.Enabled && !m.Deployed))
                {

                    DeleteCurrentModuleScripts();

                    string mpkPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", module.Name);
                    if (File.Exists(mpkPath))
                    {
                        try
                        {
                            ProcessStartInfo p = new ProcessStartInfo();
                            p.FileName = "minidump.exe";
                            p.Arguments = $"x \"{mpkPath}\" -y";
                            p.WindowStyle = ProcessWindowStyle.Hidden;
                            p.CreateNoWindow = true;
                            Process x = Process.Start(p);
                            x.WaitForExit();

                            module.Deployed = true;
                            debuglog($"INFO GCstudio, Extracted and deployed module '{module.Name}'.");

                            // Check for deployment scripts

                            string scriptBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", "Scripts", "Script_dep");
                            string[] scriptExtensions = new[] { ".exe", ".ps1", ".bat" };
                            bool scriptExecuted = false;

                            foreach (var ext in scriptExtensions)
                            {
                                string scriptPath = scriptBase + ext;
                                if (File.Exists(scriptPath))
                                {
                                    try
                                    {
                                        ProcessStartInfo scriptProc = new ProcessStartInfo();
                                        scriptProc.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                                        if (ext == ".exe")
                                        {
                                            scriptProc.FileName = scriptPath;
                                            scriptProc.Arguments = "";
                                        }
                                        else if (ext == ".ps1")
                                        {
                                            scriptProc.FileName = "powershell.exe";
                                            scriptProc.Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"";
                                        }
                                        else if (ext == ".bat")
                                        {
                                            scriptProc.FileName = "cmd.exe";
                                            scriptProc.Arguments = $"/c \"{scriptPath}\"";
                                        }
                                        scriptProc.WindowStyle = ProcessWindowStyle.Hidden;
                                        scriptProc.CreateNoWindow = true;
                                        Process scriptProcess = Process.Start(scriptProc);
                                        scriptProcess.WaitForExit();
                                        debuglog($"INFO GCstudio, Executed deployment script '{scriptPath}' for module '{module.Name}'.");
                                        scriptExecuted = true;
                                        break; // Only execute the first found script
                                    }
                                    catch (Exception ex)
                                    {
                                        debuglog($"ERROR GCstudio, Failed to execute deployment script '{scriptPath}' for module '{module.Name}'. > {ex.Message}");
                                    }
                                }
                            }
                            if (!scriptExecuted)
                            {
                                debuglog($"INFO GCstudio, No deployment script found for module '{module.Name}'.");
                            }

                        }
                        catch (Exception ex)
                        {
                            debuglog($"ERROR GCstudio, Failed to extract module '{module.Name}'. > {ex.Message}");
                        }
                    }
                    else
                    {
                        debuglog($"WARN GCstudio, Module file '{mpkPath}' not found for extraction.");
                    }
                }
                // Update modules.json after extraction
                try
                {
                    string updatedJson = JsonSerializer.Serialize(modulesList, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(modulesJsonPath, updatedJson);
                    debuglog("INFO GCstudio, Updated modules.json after deployment.");
                }
                catch (Exception ex)
                {
                    debuglog("ERROR GCstudio, Failed to update modules.json after deployment. > " + ex.Message);
                }
                DeleteCurrentModuleScripts();

            }


            ///CLI Commands
            ///

            debuglog("INFO GCstudio, looking for arguments...");

            arguments = Environment.GetCommandLineArgs();
            if (arguments.Length > 1)
            {
                debuglog("INFO GCstudio, arguments detected, executing commands...");

                // Check if any argument has a .mpk extension
                string[] mpkFiles = arguments.Skip(1).Where(arg => arg.EndsWith(".mpk", StringComparison.OrdinalIgnoreCase)).ToArray();

                string targetDirectory = AppDomain.CurrentDomain.BaseDirectory + "Modules";

                if (mpkFiles.Length > 0)
                {
                    foreach (string mpkFile in mpkFiles)
                    {
                        try
                        {
                            string destPath = Path.Combine(targetDirectory, Path.GetFileName(mpkFile));
                            File.Copy(mpkFile, destPath, true); // true = overwrite if exists
                            debuglog($"INFO GCstudio, copied .mpk file: {mpkFile} to {destPath}");
                        }
                        catch (Exception ex)
                        {
                            debuglog($"ERROR GCstudio, failed to copy .mpk file: {mpkFile} > {ex.Message}");
                            MessageBox.Show($"Failed to copy {mpkFile} to {targetDirectory}\n{ex.Message}", "Copy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    // Skip the switch statement if .mpk files were found and processed and start modules form
                    Form modules = new GC_Studio.Modules();
                    modules.ShowDialog();
                    return;
                }

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

                        debuglog("INFO GCstudio, running pkp tool and exiting...");

                        this.Visible = false;
                        pkptool();
                        Environment.Exit(0);
                        break;

                    case "/firststart" or "-fs" or "--firststart":
                        debuglog("INFO GCstudio, overriding first start");

                        Config.GCstudio.Firstrun = true;

                        break;

                    case "/resetsize" or "-rs" or "--resetsize":

                        debuglog("INFO GCstudio, overriding a reset size...");

                        ResetSize();

                        break;

                    case "/settings" or "-s" or "--settings":

                        debuglog("INFO GCstudio, overriding config panel view only...");

                        panelmain.Visible = false;
                        panelnewproj.Visible = false;
                        panelconfig.Visible = true;
                        button9.Visible = false;
                        button12.Visible = false;

                        break;

                    case "/about" or "-a" or "--about":

                        debuglog("INFO GCstudio, overriding about box view only...");

                        Form about = new GC_Studio.AboutBox();
                        about.ShowDialog();
                        this.Close();

                        break;


                    default:

                        debuglog("INFO GCstudio, parsing arguments to IDE and stay hidden...");

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
            {
                debuglog("INFO GCstudio, first run flag detected, launching accordingly...");

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
                debuglog("INFO GCstudio, hidden donate button flag detected, hiding...");

                buttondonate.Visible = false;
            }

            debuglog("INFO GCstudio, populating recent files...");

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
            debuglog("INFO GCstudio, resetting window size...");

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

            debuglog("INFO GCstudio, Load configuration started...");

            try
            {
                if (File.Exists("GCstudio.config.json"))
                {
                    debuglog("INFO GCstudio, GCstudio.config.json detected, loading and deserializing...");

                    dfe.LoadRead("GCstudio.config.json");
                    Config = json.DeserializeObject<ConfigSchema>(dfe.ReadAll());
                    dfe.CloseRead();
                }
            }
            catch (Exception ex)
            {

                debuglog("ERROR GCstudio, an error occurred while loading configuration." + " > " + ex.Message + " @ " + ex.StackTrace);

                MessageBox.Show("Error loading config.");
            }

        }


        /// <summary>
        /// Save configuration of the app 
        /// </summary>
        private void SaveConfig()
        {
            debuglog("INFO GCstudio, save configuration started...");

            try
            {
                debuglog("INFO GCstudio, serializing configuration and saving GCstudio.config.json...");

                //save current config
                dfe.LoadWrite("GCstudio.config.json");
                dfe.RecordData(json.SerializeObject(Config));
                dfe.CloseWrite();
            }
            catch (Exception ex)
            {
                debuglog("ERROR GCstudio, an error occurred while saving configuration." + " > " + ex.Message + " @ " + ex.StackTrace);

                MessageBox.Show("Error saving config.");
            }


        }



        /// <summary>
        /// Load the Recent List 
        /// </summary>
        private void LoadRecent()
        {
            debuglog("INFO GCstudio, Loading recent file list started...");

            try
            {
                //Load recent list
                if (File.Exists("GCstudio.mrf.json"))
                {
                    debuglog("INFO GCstudio, GCstudio.mrf.json detected, loading and deserializing...");

                    dfe.LoadRead("GCstudio.mrf.json");
                    RecentFiles = json.DeserializeObject<RecentFile>(dfe.ReadAll());
                    dfe.CloseRead();

                }
            }
            catch (Exception ex)
            {
                debuglog("INFO GCstudio, an error occurred while loading recent file list." + " > " + ex.Message + " @ " + ex.StackTrace);

                MessageBox.Show("Error loading recent files.");
            }
        }


        /// <summary>
        /// Save recent list
        /// </summary>
        private void SaveRecent()
        {
            debuglog("INFO GCstudio, save recent file list started...");

            try
            {

                debuglog("INFO GCstudio, serializing and saving GCstudio.mrf.json...");

                dfe.LoadWrite("GCstudio.mrf.json");
                dfe.RecordData(json.SerializeObject(RecentFiles));
                dfe.CloseWrite();
            }
            catch (Exception ex)
            {
                debuglog("INFO GCstudio, an error occurred while saving recent file list." + " > " + ex.Message + " @ " + ex.StackTrace);

                MessageBox.Show("Error saving recent files.");
            }

        }

        /// <summary>
        /// Add file to recent list 
        /// </summary>
        private void addrecent(string FileName, string FullPath)
        {
            debuglog("INFO GCstudio, adding file/directory to recent list...");

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
            debuglog("INFO GCstudio, Exiting GCstudio by user request...");

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
            debuglog("INFO GCstudio, minimizing window by user request...");

            this.WindowState = FormWindowState.Minimized;
        }

        private void BtnMini_Click(object sender, EventArgs e)
        {
            debuglog("INFO GCstudio, executing maximize function by user request...");

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

        private void buttondemonstrations_MouseHover(object sender, EventArgs e)
        {
            buttondemonstrations.ForeColor = Color.White;
        }

        private void buttondemonstrations_MouseLeave(object sender, EventArgs e)
        {
            buttondemonstrations.ForeColor = Color.CornflowerBlue;
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
                debuglog("INFO GCstudio, creating new project...");

                Config.GCstudio.LastDirectory = textBox2.Text;
                SaveConfig();

                try
                {
                    debuglog("INFO GCstudio, expanding project data on new process...");

                    ProcessStartInfo p = new ProcessStartInfo();
                    p.FileName = "minidump.exe";
                    p.Arguments = "x \"GCBprog.tpl\" -o\"" + textBox2.Text + "\\" + textBox1.Text + "\" * -r -y";
                    p.WindowStyle = ProcessWindowStyle.Hidden;
                    p.CreateNoWindow = true;
                    Process x = Process.Start(p);
                    x.WaitForExit();
                }
                catch (Exception ex)
                {
                    debuglog("ERROR GCstudio, an error occurred while expanding project data..." + " > " + ex.Message + " @ " + ex.StackTrace);

                    MessageBox.Show("Error creating project.");
                }

                if (File.Exists(textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace") == true)
                {
                    debuglog("INFO GCstudio, IDE launch requested...");

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


                        case "VScode":
                            addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace");

                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace" + "\"", Config.GCstudio.IDE);
                            break;
                    }
                }
            }
            else
            {
                debuglog("WARING GCstudio, a directory with the same name of the project already exists, warning user...");

                MessageBox.Show("A folder of the same name already exists on the location, please use a valid project name.", "Folder already exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox2.Text + "\\" + textBox1.Text) == false)
            {
                debuglog("INFO GCstudio, Creating new project...");

                Config.GCstudio.LastDirectory = textBox2.Text;
                SaveConfig();

                try
                {

                    debuglog("INFO GCstudio, expanding project data on new process...");

                    ProcessStartInfo p = new ProcessStartInfo();
                    p.FileName = "minidump.exe";
                    p.Arguments = "x \"GCBLib.tpl\" -o\"" + textBox2.Text + "\\" + textBox1.Text + "\" * -r -y";
                    p.WindowStyle = ProcessWindowStyle.Hidden;
                    p.CreateNoWindow = true;
                    Process x = Process.Start(p);
                    x.WaitForExit();
                }
                catch (Exception ex)
                {
                    debuglog("ERROR GCstudio, an error occurred while expanding project data." + " > " + ex.Message + " @ " + ex.StackTrace);

                    MessageBox.Show("Error creating project.");
                }

                if (File.Exists(textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace") == true)
                {
                    debuglog("INFO GCstudio, IDE launch requested...");

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


                        case "VScode":
                            addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace");

                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace" + "\"", Config.GCstudio.IDE);
                            break;
                    }
                }
            }
            else
            {

                debuglog("WARING GCstudio, a directory with the same name of the project already exists, warning user...");

                MessageBox.Show("A folder of the same name already exists on the location, please use a valid project name.", "Folder already exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox2.Text + "\\" + textBox1.Text) == false)
            {

                debuglog("INFO GCstudio, creating new project...");

                Config.GCstudio.LastDirectory = textBox2.Text;
                SaveConfig();

                try
                {

                    debuglog("INFO GCstudio, expanding project data on new process...");

                    ProcessStartInfo p = new ProcessStartInfo();
                    p.FileName = "minidump.exe";
                    p.Arguments = "x \"FBasicProg.tpl\" -o\"" + textBox2.Text + "\\" + textBox1.Text + "\" * -r -y";
                    p.WindowStyle = ProcessWindowStyle.Hidden;
                    p.CreateNoWindow = true;
                    Process x = Process.Start(p);
                    x.WaitForExit();
                }
                catch (Exception ex)
                {

                    debuglog("ERROR GCstudio, an error occurred while expanding project data." + " > " + ex.Message + " @ " + ex.StackTrace);

                    MessageBox.Show("Error creating project.");
                }


                if (File.Exists(textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace") == true)
                {

                    debuglog("INFO GCstudio, IDE launch requested...");

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


                        case "VScode":
                            addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace");

                            LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + "\\Visual Studio Project.code-workspace" + "\"", Config.GCstudio.IDE);
                            break;
                    }
                }
            }
            else
            {
                debuglog("WARING GCstudio, a directory with the same name of the project already exists, warning user...");

                MessageBox.Show("A folder of the same name already exists on the location, please use a valid project name.", "Folder already exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox2.Text + "\\" + textBox1.Text + ".gcb") == false)
            {

                debuglog("INFO GCstudio, creating new project...");

                Config.GCstudio.LastDirectory = textBox2.Text;
                SaveConfig();


                try
                {
                    debuglog("INFO GCstudio, expanding project data...");

                    File.Copy("GCBFile.tpl", textBox2.Text + "\\" + textBox1.Text + ".gcb");
                }

                catch (Exception ex)
                {
                    debuglog("ERROR GCstudio, an error occurred while expanding project data..." + " > " + ex.Message + " @ " + ex.StackTrace);

                    MessageBox.Show("Error creating project.");
                }


                if (File.Exists(textBox2.Text + "\\" + textBox1.Text + ".gcb") == true)
                {
                    debuglog("INFO GCstudio, IDE launch requested...");

                    addrecent(textBox1.Text, textBox2.Text + "\\" + textBox1.Text + ".gcb");

                    LaunchIDE("\"" + textBox2.Text + "\\" + textBox1.Text + ".gcb\"", Config.GCstudio.IDE);
                }
            }
            else
            {
                debuglog("WARNING GCstudio, a file of the same name as the project already exist, warning user...");

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
                MessageBox.Show("User is on a 32bit OS, switching architecture to x86.", "Architecture Switch", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            Config.GCstudio.Architecture = comboarch.Text;
            SaveConfig();
            CompilerArchitecture();
        }


        public void LaunchIDE(string Args, string IDE)
        {

            debuglog("INFO GCstudio, IDE Launch started, selecting the right one...");

            ProcessStartInfo p = new ProcessStartInfo();
            Process x;
            switch (IDE)
            {
                case "GCcode":

                    debuglog("INFO GCstudio, launching GCcode...");

                    try
                    {
                        debuglog("INFO GCstudio, getting GCcode version...");
                        dfe.LoadRead(AppDomain.CurrentDomain.BaseDirectory + "vscode\\version.txt");
                        GCcodeVer = dfe.ReadData();
                        dfe.CloseRead();
                        debuglog("DEBUG GCstudio, GCcodeVer=" + GCcodeVer);
                    }
                    catch (Exception ex)
                    {
                        debuglog("INFO GCstudio, an error occurred while getting GCcode version." + " > " + ex.Message + " @ " + ex.StackTrace);

                    }


                    try
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

                        debuglog("INFO GCstudio, starting GCstudio process and log daemon...");
                        this.ShowInTaskbar = false;
                        this.Hide();

                        p.FileName = AppDomain.CurrentDomain.BaseDirectory + "vscode\\code.exe";
                        p.Arguments = Args;
                        p.WindowStyle = ProcessWindowStyle.Normal;
                        p.RedirectStandardOutput = true;
                        p.RedirectStandardError = true;
                        x = Process.Start(p);
                        SetForegroundWindow(x.MainWindowHandle);
                        x.OutputDataReceived += GCcode_OutputDataReceived;
                        x.ErrorDataReceived += GCcode_ErrorDataReceived;
                        x.BeginOutputReadLine();
                        x.BeginErrorReadLine();
                        x.WaitForExit();

                        debuglog("INFO GCstudio, GCcode process terminated, exiting daemon...");

                        Environment.Exit(0);
                        break;
                    }
                    catch (Exception ex)
                    {
                        debuglog("INFO GCstudio, an error occurred while launching GCcode; expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "vscode\\code.exe" + " > " + ex.Message + " @ " + ex.StackTrace);

                        MessageBox.Show("An error occurred when launching the IDE, expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "vscode\\code.exe");
                        break;
                    }

                case "SynWrite":
                    debuglog("INFO GCstudio, launching SynWrite...");

                    try
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

                        p.FileName = AppDomain.CurrentDomain.BaseDirectory + "SynWrite\\Syn.exe";
                        p.Arguments = Args;
                        p.WindowStyle = ProcessWindowStyle.Normal;
                        x = Process.Start(p);
                        SetForegroundWindow(x.MainWindowHandle);

                        Environment.Exit(0);
                        break;
                    }
                    catch (Exception ex)
                    {
                        debuglog("INFO GCstudio, an error occurred while launching SynWrite; expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "SynWrite\\Syn.exe" + " > " + ex.Message + " @ " + ex.StackTrace);

                        MessageBox.Show("An error occurred when launching the IDE, expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "SynWrite\\Syn.exe");
                        break;
                    }

                case "GCgraphical":
                    debuglog("INFO GCstudio, launching GCgraphical...");

                    try
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

                        p.FileName = AppDomain.CurrentDomain.BaseDirectory + "gcbasic\\great cow graphical basic.exe";
                        p.Arguments = Args;
                        p.WindowStyle = ProcessWindowStyle.Normal;
                        x = Process.Start(p);
                        SetForegroundWindow(x.MainWindowHandle);

                        Environment.Exit(0);
                        break;
                    }
                    catch (Exception ex)
                    {
                        debuglog("INFO GCstudio, an error occurred while launching GCgraphical; expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "gcbasic\\great cow graphical basic.exe" + " > " + ex.Message + " @ " + ex.StackTrace);

                        MessageBox.Show("An error occurred when launching the IDE, expected ide location: " + AppDomain.CurrentDomain.BaseDirectory + "gcbasic\\great cow graphical basic.exe");
                        break;
                    }



                case "VScode":
                    debuglog("INFO GCstudio, launching VScode (Experimental)...");


                    try
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

                        string vsCodePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Microsoft VS Code", "Code.exe");

                        debuglog("INFO GCstudio, starting GCstudio process and log daemon...");
                        this.ShowInTaskbar = false;
                        this.Hide();

                        p.FileName = vsCodePath;
                        p.Arguments = Args;
                        p.WindowStyle = ProcessWindowStyle.Normal;
                        p.RedirectStandardOutput = true;
                        p.RedirectStandardError = true;
                        x = Process.Start(p);
                        SetForegroundWindow(x.MainWindowHandle);
                        x.OutputDataReceived += VScode_OutputDataReceived;
                        x.ErrorDataReceived += VScode_ErrorDataReceived;
                        x.BeginOutputReadLine();
                        x.BeginErrorReadLine();
                        x.WaitForExit();

                        debuglog("INFO GCstudio, VScode process terminated, exiting daemon...");

                        Environment.Exit(0);
                        break;
                    }
                    catch (Exception ex)
                    {
                        debuglog("INFO GCstudio, an error occurred while launching VScode; is in PATH?  > " + ex.Message + " @ " + ex.StackTrace);

                        MessageBox.Show("An error occurred when launching the IDE.");
                        break;
                    }

                default:

                    break;
            }
        }


        /// <summary>
        /// GCcode Log Daemon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void GCcode_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            DataFileEngine dl = new DataFileEngine();
            try
            {
                dl.StreamW = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Log/GCcode" + GCcodeVer + ".log", true);
                if (e.Data == null || e.Data == "")
                {
                    dl.RecordData("");
                }
                else
                {
                    dl.RecordData(e.Data + "<out");
                }
                dl.CloseWrite();
            }
            catch { }
        }

        private void GCcode_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            DataFileEngine dl = new DataFileEngine();
            try
            {
                dl.StreamW = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Log/GCcode" + GCcodeVer + ".log", true);
                if (e.Data == null || e.Data == "")
                {
                    dl.RecordData("");
                }
                else
                {
                    dl.RecordData(e.Data + "<err");
                }
                dl.CloseWrite();
            }
            catch { }
        }



        /// <summary>
        /// VScode Log Daemon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void VScode_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            DataFileEngine dl = new DataFileEngine();
            try
            {
                dl.StreamW = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Log/VScode.log", true);
                if (e.Data == null || e.Data == "")
                {
                    dl.RecordData("");
                }
                else
                {
                    dl.RecordData(e.Data + "<out");
                }
                dl.CloseWrite();
            }
            catch { }
        }

        private void VScode_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            DataFileEngine dl = new DataFileEngine();
            try
            {
                dl.StreamW = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Log/VScode.log", true);
                if (e.Data == null || e.Data == "")
                {
                    dl.RecordData("");
                }
                else
                {
                    dl.RecordData(e.Data + "<err");
                }
                dl.CloseWrite();
            }
            catch { }
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
            debuglog("INFO GCstudio, clearing recent file list...");

            try
            {
                if (File.Exists("GCstudio.mrf.json"))
                {
                    File.Delete("GCstudio.mrf.json");
                }
                listViewRecent.Items.Clear();
            }
            catch (Exception ex)
            {
                debuglog("INFO GCstudio, an error occurred while clearing recent file list." + " > " + ex.Message + " @ " + ex.StackTrace);

                MessageBox.Show("Error clearing recent list.");
            }

        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            pkptool();
        }

        private void pkptool()
        {
            debuglog("INFO GCstudio, starting pkp tool...");

            MessageBox.Show("This tool will clone an existing installation of PICKitPlus on your current GC Studio installation.\r\n\r\nPress Ok and select a directory with a working installation of PICKitPlus to clone.", "PICKitPlus Clone Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            folderBrowserDialog.ShowDialog();
            if (File.Exists(folderBrowserDialog.SelectedPath + "\\PICkit2Plus.exe"))
            {
                try
                {
                    FileSystem.CopyDirectory(folderBrowserDialog.SelectedPath, AppDomain.CurrentDomain.BaseDirectory + "PICKitPlus", UIOption.AllDialogs);
                    MessageBox.Show("PICKitPlus Cloned successfully!", "PICKitPlus Clone Tool", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                    debuglog("INFO GCstudio, pkp tool clone success.");

                }
                catch (Exception ex)
                {
                    debuglog("ERROR GCstudio, pkp tool an error occurred while cloning." + " > " + ex.Message + " @ " + ex.StackTrace);

                    MessageBox.Show("An error occurred while cloning.", "PICKitPlus Clone Tool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                debuglog("WARNING GCstudio, pickitplus wasn´t found, clone aborted, warning user...");

                MessageBox.Show("PICKitPlus wasn’t found on selected directory, clone aborted.", "PICKitPlus Clone Tool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            folderBrowserDialog.SelectedPath = "";
        }

        private void CompilerArchitecture()
        {
            debuglog("INFO GCstudio, identifying OS architecture...");

            if (Environment.Is64BitOperatingSystem)
            {
                debuglog("DEBUG GCstudio, x64 OS detected.");

                labelarch.Text = "x64";
            }
            else
            {
                debuglog("DEBUG GCstudio, x86 OS detected.");

                labelarch.Text = "x86";
            }

            debuglog("INFO GCstudio, selecting compiler...");

            switch (Config.GCstudio.Architecture)
            {
                case "Auto":
                    try
                    {
                        if (Environment.Is64BitOperatingSystem)
                        {
                            debuglog("INFO GCstudio, automatic selection enabled, choosing 64bit...");

                            File.Copy("gcbasic\\gcbasic64.exe", "gcbasic\\gcbasic.exe", true);
                        }
                        else
                        {
                            debuglog("INFO GCstudio, automatic selection enabled, choosing 32bit...");

                            File.Copy("gcbasic\\gcbasic32.exe", "gcbasic\\gcbasic.exe", true);
                        }
                    }
                    catch (Exception ex)
                    {

                        debuglog("ERROR GCstudio, an error occurred while selecting compiler architecture..." + " > " + ex.Message + " @ " + ex.StackTrace);

                    }
                    break;

                case "x86":
                    try
                    {
                        debuglog("INFO GCstudio, manual architecture enabled, selecting 32bit...");

                        File.Copy("gcbasic\\gcbasic32.exe", "gcbasic\\gcbasic.exe", true);
                    }
                    catch (Exception ex)
                    {
                        debuglog("ERROR GCstudio, an error occurred while selecting compiler architecture..." + " > " + ex.Message + " @ " + ex.StackTrace);

                    }

                    break;

                case "x64":
                    try
                    {
                        debuglog("INFO GCstudio, manual architecture enabled, selecting 64bit...");

                        File.Copy("gcbasic\\gcbasic64.exe", "gcbasic\\gcbasic.exe", true);
                    }
                    catch (Exception ex)
                    {
                        debuglog("ERROR GCstudio, an error occurred while selecting compiler architecture..." + " > " + ex.Message + " @ " + ex.StackTrace);

                    }

                    break;

                case "Developer":
                    debuglog("INFO GCstudio, developer override detected, leaving current...");

                    break;

                default:
                    debuglog("WARNING GCstudio, no option detected, forcing auto...");

                    try
                    {
                        if (Environment.Is64BitOperatingSystem)
                        {
                            debuglog("INFO GCstudio, selecting 64bit...");

                            File.Copy("gcbasic\\gcbasic64.exe", "gcbasic\\gcbasic.exe", true);
                        }
                        else
                        {
                            debuglog("INFO GCstudio, selecting 32bit...");

                            File.Copy("gcbasic\\gcbasic32.exe", "gcbasic\\gcbasic.exe", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        debuglog("ERROR GCstudio, an error occurred while selecting compiler architecture..." + " > " + ex.Message + " @ " + ex.StackTrace);

                    }

                    break;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                debuglog("INFO GCstudio, resetting programmer preferences...");

                File.Copy("use_in_master\\use.ini", "gcbasic\\use.ini", true);
                MessageBox.Show("The Programmer Preferences has been reset successfully.", "Reset Programmer Preferences", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                debuglog("ERROR GCstudio, an error occurred while resetting programmer preferences." + " > " + ex.Message + " @ " + ex.StackTrace);

                MessageBox.Show("Error while resetting programmer preferences.");
            }
        }


        private void button12_Click(object sender, EventArgs e)
        {
            debuglog("INFO GCstudio, launching force update...");

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
            catch (Exception ex)
            {
                debuglog("ERROR GCstudio, an error occurred when launching the force update..." + " > " + ex.Message + " @ " + ex.StackTrace);
                MessageBox.Show("An error occurred when launching the Force Update");
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to reset GC Studio to factory settings? This will clear all IDE user configurations, reset programmer preferences, remove all installed extensions and set GC Studio to its default settings.", "Reset To Factory Settings.", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                debuglog("INFO GCstudio, restore GCstudio factory settings started...");

                try
                {
                    debuglog("INFO GCstudio, resetting programmer preferences...");

                    File.Copy("use_in_master\\use.ini", "gcbasic\\use.ini", true);

                }
                catch (Exception ex)
                {
                    debuglog("ERROR GCstudio, an error occurred while resetting programmer preferences." + " > " + ex.Message + " @ " + ex.StackTrace);

                    MessageBox.Show("Error while resetting programmer preferences.");
                }


                debuglog("INFO GCstudio, starting reset to factory tool...");

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
                catch (Exception ex)
                {
                    debuglog("ERROR GCstudio, an error occurred when launching the reset to factory tool." + " > " + ex.Message + " @ " + ex.StackTrace);

                    MessageBox.Show("An error occurred when launching the Reset To Factory Tool");
                }
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            debuglog("INFO GCstudio, starting the prefseditor tool...");

            ProcessStartInfo p = new ProcessStartInfo();
            Process x;
            try
            {
                p.FileName = AppDomain.CurrentDomain.BaseDirectory + "gcbasic\\Programmer Editor.exe";
                p.Arguments = AppDomain.CurrentDomain.BaseDirectory + "gcbasic\\use.INI";
                p.WindowStyle = ProcessWindowStyle.Maximized;
                x = Process.Start(p);
            }
            catch (Exception ex)
            {
                debuglog("ERROR GCstudio, an error occurred when launching the prefseditor tool." + " > " + ex.Message + " @ " + ex.StackTrace);

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
            debuglog("INFO GCstudio, opening donate link...");

            Process.Start("explorer", DonateLink);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            debuglog("INFO GCstudio, starting GCdebug tool...");

            ProcessStartInfo p = new ProcessStartInfo();
            Process x;
            try
            {
                p.FileName = "GCdebug.exe";
                p.Arguments = "";
                p.WindowStyle = ProcessWindowStyle.Maximized;
                x = Process.Start(p);
                SetForegroundWindow(x.MainWindowHandle);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                debuglog("ERROR GCstudio, an error occurred when launching the GCdebug tool." + " > " + ex.Message + " @ " + ex.StackTrace);

                MessageBox.Show("An error occurred when launching the GCdebug tool");
            }
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

        private void buttondemonstrations_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", Demonstrations);
        }


        private void buttonmodules_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form modules = new GC_Studio.Modules();
            modules.ShowDialog();
        }

        private void buttonmodules_MouseHover(object sender, EventArgs e)
        {
            buttonmodules.ForeColor = Color.White;
        }

        private void buttonmodules_MouseLeave(object sender, EventArgs e)
        {
            buttonmodules.ForeColor = Color.CornflowerBlue;
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            string targetFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vscode", "data", "user-data", "user", "settings.json");
            string appDataCodeUserPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Code", "User", "settings.json");

            try
            {
                debuglog($"INFO GCstudio, Exporting settings.json to VScode user path: {appDataCodeUserPath}");
                File.Copy(targetFile, appDataCodeUserPath, true);
                debuglog("INFO GCstudio, settings exported successfully.");
            }
            catch (Exception ex)
            {
                debuglog($"ERROR GCstudio, failed to export settings.json to VScode user path: {ex.Message} @ {ex.StackTrace}");
                MessageBox.Show($"Failed to export settings.json:\n{ex.Message}", "Copy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Add default extensions to VSCode 
            string vsCodePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Programs", "Microsoft VS Code", "bin", "Code.cmd"
            );

            // Extensions to install
            string[] extensions = new[] { "mierengineering.gcbasic", "aaron-bond.better-comments", "atishay-jain.all-autocomplete", "github.copilot", "grapecity.gc-excelviewer", "rioj7.command-variable", "tomoki1207.pdf" };

            // Build the install command
            string args = "--install-extension " + string.Join(" --install-extension ", extensions);

            try
            {
                var p = new ProcessStartInfo
                {
                    FileName = vsCodePath,
                    Arguments = args,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true
                };
                Process.Start(p);
                debuglog("INFO GCstudio, VSCode extensions install command executed: " + args);
            }
            catch (Exception ex)
            {
                debuglog("ERROR GCstudio, failed to run VSCode extension install: " + ex.Message + " @ " + ex.StackTrace);
                MessageBox.Show("Failed to run VSCode extension install:\n" + ex.Message, "VSCode Extension Install Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Show success message after all operations
            MessageBox.Show("GC Code settings exported to VS Code.", "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        /// <summary>
        /// Deletes all current Script_dep and Script_rem files in Modules\Scripts.
        /// </summary>
        private void DeleteCurrentModuleScripts()
        {
            string[] scriptExtensions = new[] { ".exe", ".ps1", ".bat" };
            string[] scriptBases = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", "Scripts", "Script_dep"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", "Scripts", "Script_rem")
            };

            foreach (var scriptBase in scriptBases)
            {
                foreach (var ext in scriptExtensions)
                {
                    string scriptPath = scriptBase + ext;
                    try
                    {
                        if (File.Exists(scriptPath))
                        {
                            File.Delete(scriptPath);
                            debuglog($"INFO GCstudio, Deleted script file '{scriptPath}' in DeleteCurrentModuleScripts.");
                        }
                    }
                    catch (Exception ex)
                    {
                        debuglog($"ERROR GCstudio, Failed to delete script file '{scriptPath}' in DeleteCurrentModuleScripts. > {ex.Message}");
                    }
                }
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