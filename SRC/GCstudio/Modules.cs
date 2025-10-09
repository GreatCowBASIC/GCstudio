using Ngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace GC_Studio
{
    public partial class Modules : Form
    {
        private List<Module> modulesList = new List<Module>();
        private string modulesDirectory;
        private string jsonFilePath;
        private readonly string[] scriptExtensions = new[] { ".exe", ".ps1", ".bat" };



        public Modules()
        {
            InitializeComponent();
        }



        private void Modules_Load(object sender, EventArgs e)
        {
            modulesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules");
            jsonFilePath = Path.Combine(modulesDirectory, "modules.json");

            LoadModulesList();

            // Scan directory for .mpk files and add new ones to modulesList
            if (Directory.Exists(modulesDirectory))
            {
                string[] files = Directory.GetFiles(modulesDirectory, "*.mpk");
                debuglog($"INFO GCstudio Modules, Found {files.Length} .mpk files in directory.");
                
                // Get list of existing .mpk file names
                var existingMpkFiles = files.Select(f => Path.GetFileName(f)).ToHashSet();
                
                // Remove modules from modulesList that no longer have corresponding .mpk files
                var modulesToRemove = modulesList.Where(m => !existingMpkFiles.Contains(m.Name)).ToList();
                foreach (var moduleToRemove in modulesToRemove)
                {
                    modulesList.Remove(moduleToRemove);
                    debuglog($"INFO GCstudio Modules, Removed missing module '{moduleToRemove.Name}' from list.");
                }
                
                // Add new .mpk files to modulesList
                foreach (var file in files)
                {
                    string fileName = Path.GetFileName(file);
                    if (!modulesList.Any(m => m.Name == fileName))
                    {
                        modulesList.Add(new Module
                        {
                            Name = fileName,
                            Enabled = true,
                            Deployed = false
                        });
                        debuglog($"INFO GCstudio Modules, Added new module '{fileName}' to list.");
                    }
                }
            }
            else
            {
                debuglog("ERROR GCstudio Modules, Modules directory not found. > " + modulesDirectory);
                MessageBox.Show("Directory not found: " + modulesDirectory);
                return;
            }

            SaveModulesList();

            // Add modules to checkedListBoxModules
            checkedListBoxModules.Items.Clear();
            foreach (var module in modulesList)
            {
                int idx = checkedListBoxModules.Items.Add(module.Name);
                checkedListBoxModules.SetItemChecked(idx, module.Enabled);
                debuglog($"INFO GCstudio Modules, Added module '{module.Name}' to checkedListBoxModules (Enabled={module.Enabled}).");
            }

            // Subscribe to ItemCheck event
            checkedListBoxModules.ItemCheck += CheckedListBoxModules_ItemCheck;
        }



        private void CheckedListBoxModules_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Get the module name for the item being checked/unchecked
            string moduleName = checkedListBoxModules.Items[e.Index].ToString();

            // Find the corresponding module in modulesList
            var module = modulesList.FirstOrDefault(m => m.Name == moduleName);
            if (module != null)
            {
                // Update the Enabled property based on the new check state
                module.Enabled = (e.NewValue == CheckState.Checked);
                debuglog($"INFO GCstudio Modules, Module '{moduleName}' Enabled set to {module.Enabled}.");

                // If the item is being disabled, only extract and run script if Deployed == true
                if (e.NewValue == CheckState.Unchecked && module.Deployed)
                {
                    DeleteCurrentModuleScripts();

                    // Save modules.json before extraction and script execution
                    module.Deployed = false;
                    module.Enabled = false;
                    SaveModulesList();

                    // Get just the Script folder from the .mpk file to Modules\Scripts
                    string mpkPath = Path.Combine(modulesDirectory, moduleName);
                    string scriptsOutputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules");

                    if (File.Exists(mpkPath))
                    {
                        try
                        {
                            ProcessStartInfo extractProc = new ProcessStartInfo
                            {
                                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "minidump.exe"),
                                Arguments = $"x \"{mpkPath}\" \"Modules\\*\" -o\"{AppDomain.CurrentDomain.BaseDirectory}\" -y",
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true
                            };
                            Process extractProcess = Process.Start(extractProc);
                            extractProcess.WaitForExit();
                            debuglog($"INFO GCstudio Modules, Extracted Script folder from '{mpkPath}' to '{scriptsOutputDir}'.");
                        }
                        catch (Exception ex)
                        {
                            debuglog($"ERROR GCstudio Modules, Failed to extract Script folder from '{mpkPath}'. > {ex.Message}");
                        }
                    }
                    else
                    {
                        debuglog($"WARN GCstudio Modules, .mpk file '{mpkPath}' not found for Script extraction.");
                    }

                    // Execute removal script if it exists
                    string removalScriptBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", "Scripts", "Script_rem");
                    bool scriptExecuted = false;
                    foreach (var ext in scriptExtensions)
                    {
                        string scriptPath = removalScriptBase + ext;
                        if (File.Exists(scriptPath))
                        {
                            try
                            {
                                ProcessStartInfo scriptProc = new ProcessStartInfo
                                {
                                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                                    WindowStyle = ProcessWindowStyle.Hidden,
                                    CreateNoWindow = true
                                };
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
                                Process scriptProcess = Process.Start(scriptProc);
                                scriptProcess.WaitForExit();
                                debuglog($"INFO GCstudio Modules, Executed removal script '{scriptPath}' for module '{moduleName}'.");
                                scriptExecuted = true;
                                break; // Only execute the first found script
                            }
                            catch (Exception ex)
                            {
                                debuglog($"ERROR GCstudio Modules, Failed to execute removal script '{scriptPath}' for module '{moduleName}'. > {ex.Message}");
                            }
                        }
                    }
                    if (!scriptExecuted)
                    {
                        debuglog($"INFO GCstudio Modules, No removal script found for module '{moduleName}'.");
                    }

                    DeleteCurrentModuleScripts();
                }
                else if (e.NewValue == CheckState.Unchecked && !module.Deployed)
                {
                    debuglog($"INFO GCstudio Modules, Skipped extraction and script execution for '{moduleName}' because Deployed == false.");
                    // Still update the JSON to reflect the new Enabled state
                    SaveModulesList();
                }
                else
                {
                    // Save the updated list to JSON for other state changes
                    SaveModulesList();
                }
            }
        }




        private void buttonRem_Click(object sender, EventArgs e)
        {
            // Ensure an item is selected
            if (checkedListBoxModules.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a module to remove.");
                debuglog("WARN GCstudio Modules, Remove attempted with no selection.");
                return;
            }

            // Get the selected module name
            string moduleName = checkedListBoxModules.SelectedItem.ToString();
            debuglog($"INFO GCstudio Modules, Attempting to remove module '{moduleName}'.");

            // Find the module in the list
            var moduleToRemove = modulesList.FirstOrDefault(m => m.Name == moduleName);
            if (moduleToRemove == null)
            {
                debuglog($"WARN GCstudio Modules, Module '{moduleName}' not found in modulesList.");
                MessageBox.Show("Module not found in list.");
                return;
            }
            else 
            {
                modulesList.Remove(moduleToRemove);
                checkedListBoxModules.Items.Remove(moduleName);
                debuglog($"INFO GCstudio Modules, Removed module '{moduleName}' from modulesList.");
                SaveModulesList();
            }

            // Only proceed with extraction and script execution if Deployed == true
            if (moduleToRemove.Deployed)
            {
                // Build the file path
                string filePath = Path.Combine(modulesDirectory, moduleName);

                DeleteCurrentModuleScripts();

                // Get just the Script folder from the .mpk file to Modules\Scripts
                string mpkPath = Path.Combine(modulesDirectory, moduleName);
                string scriptsOutputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules");

                if (File.Exists(mpkPath))
                {
                    try
                    {
                        ProcessStartInfo extractProc = new ProcessStartInfo
                        {
                            FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "minidump.exe"),
                            Arguments = $"x \"{mpkPath}\" \"Modules\\*\" -o\"{AppDomain.CurrentDomain.BaseDirectory}\" -y",
                            WindowStyle = ProcessWindowStyle.Hidden,
                            CreateNoWindow = true
                        };
                        Process extractProcess = Process.Start(extractProc);
                        extractProcess.WaitForExit();
                        debuglog($"INFO GCstudio Modules, Extracted Script folder from '{mpkPath}' to '{scriptsOutputDir}'.");
                    }
                    catch (Exception ex)
                    {
                        debuglog($"ERROR GCstudio Modules, Failed to extract Script folder from '{mpkPath}'. > {ex.Message}");
                    }
                }
                else
                {
                    debuglog($"WARN GCstudio Modules, .mpk file '{mpkPath}' not found for Script extraction.");
                }

                // Execute removal script
                string removalScriptBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", "Scripts", "Script_rem");
                bool scriptExecuted = false;
                foreach (var ext in scriptExtensions)
                {
                    string scriptPath = removalScriptBase + ext;
                    if (File.Exists(scriptPath))
                    {
                        try
                        {
                            ProcessStartInfo scriptProc = new ProcessStartInfo
                            {
                                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true
                            };
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
                            Process scriptProcess = Process.Start(scriptProc);
                            scriptProcess.WaitForExit();
                            debuglog($"INFO GCstudio Modules, Executed removal script '{scriptPath}' for module '{moduleName}'.");
                            scriptExecuted = true;
                            break; // Only execute the first found script
                        }
                        catch (Exception ex)
                        {
                            debuglog($"ERROR GCstudio Modules, Failed to execute removal script '{scriptPath}' for module '{moduleName}'. > {ex.Message}");
                        }
                    }
                }
                if (!scriptExecuted)
                {
                    debuglog($"INFO GCstudio Modules, No removal script found for module '{moduleName}'.");
                }

                // Delete all possible deployment and removal scripts for the module
                DeleteCurrentModuleScripts();
            }
            else
            {
                debuglog($"INFO GCstudio Modules, Skipped extraction and script execution for '{moduleName}' because Deployed == false.");
            }

            // Try to delete the .mpk file
            string filePathToDelete = Path.Combine(modulesDirectory, moduleName);
            try
            {
                if (File.Exists(filePathToDelete))
                {
                    File.Delete(filePathToDelete);
                    debuglog($"INFO GCstudio Modules, Deleted file '{filePathToDelete}'.");
                }
                else
                {
                    debuglog($"WARN GCstudio Modules, File '{filePathToDelete}' does not exist.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting file: {ex.Message}");
                debuglog($"ERROR GCstudio Modules, Exception deleting file '{filePathToDelete}': {ex.Message}");
                return;
            }


        }



        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Modules_FormClosing(object sender, FormClosingEventArgs e)
        {
            debuglog("INFO GCstudio Modules, Restarting application.");

            ProcessStartInfo p = new ProcessStartInfo();
            Process x;
            try
            {
                p.FileName = "gcstudio.exe";
                p.WindowStyle = ProcessWindowStyle.Normal;
                x = Process.Start(p);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                debuglog("ERROR GCstudio  Modules, an error occurred when restarting application. > " + ex.Message + " @ " + ex.StackTrace);
                MessageBox.Show("An error occurred when restarting application.");
            }
        }


        /// <summary>
        /// Deletes all current Script_dep and Script_rem files in Modules\Scripts.
        /// </summary>
        private void DeleteCurrentModuleScripts()
        {
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
                            debuglog($"INFO GCstudio, Deleted script file '{scriptPath}'");
                        }
                    }
                    catch (Exception ex)
                    {
                        debuglog($"ERROR GCstudio, Failed to delete script file '{scriptPath}' > {ex.Message}");
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


        private void LoadModulesList()
        {
            if (File.Exists(jsonFilePath))
            {
                try
                {
                    string json = File.ReadAllText(jsonFilePath);
                    modulesList = JsonSerializer.Deserialize<List<Module>>(json) ?? new List<Module>();
                    debuglog("INFO GCstudio Modules, Loaded modules.json successfully.");
                }
                catch
                {
                    modulesList = new List<Module>();
                    debuglog("ERROR GCstudio Modules, Failed to deserialize modules.json.");
                }
            }
            else
            {
                modulesList = new List<Module>();
                debuglog("INFO GCstudio Modules, modules.json not found, starting with empty list.");
            }
        }

        private void SaveModulesList()
        {
            var jsonOut = JsonSerializer.Serialize(modulesList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonFilePath, jsonOut);
            debuglog("INFO GCstudio Modules, Saved modules list to modules.json.");
        }

    }
}
