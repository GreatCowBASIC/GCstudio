using Ngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GC_Studio
{
    public partial class Modules : Form
    {
        List<Module> modulesList = new List<Module>();
        string modulesDirectory;
        string jsonFilePath;

        public Modules()
        {
            InitializeComponent();
        }

        private void Modules_Load(object sender, EventArgs e)
        {
            modulesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules");
            jsonFilePath = Path.Combine(modulesDirectory, "modules.json");

            // Load existing modules.json if it exists
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

            // Scan directory for .mpk files and add new ones to modulesList
            if (Directory.Exists(modulesDirectory))
            {
                string[] files = Directory.GetFiles(modulesDirectory, "*.mpk");
                debuglog($"INFO GCstudio Modules, Found {files.Length} .mpk files in directory.");
                foreach (var file in files)
                {
                    string fileName = Path.GetFileName(file);
                    if (!modulesList.Any(m => m.Name == fileName))
                    {
                        modulesList.Add(new Module
                        {
                            Name = fileName,
                            Enabled = false,
                            Deployed = false
                        });
                        debuglog($"INFO GCstudio Modules, Added new module '{fileName}' to list.");
                    }
                }
            }
            else
            {
                debuglog("ERROR GCstudio Modules, Modules directory not found." + " > " + modulesDirectory);
                MessageBox.Show("Directory not found: " + modulesDirectory);
                return;
            }

            // Save updated modulesList back to modules.json
            var jsonOut = JsonSerializer.Serialize(modulesList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonFilePath, jsonOut);
            debuglog("INFO GCstudio Modules, Saved modules list to modules.json.");

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

                // Save the updated list to JSON
                var jsonOut = JsonSerializer.Serialize(modulesList, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonFilePath, jsonOut);
                debuglog("INFO GCstudio Modules, Updated modules.json after ItemCheck.");
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

            // Build the file path
            string filePath = Path.Combine(modulesDirectory, moduleName);

            // Try to delete the .mpk file
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    debuglog($"INFO GCstudio Modules, Deleted file '{filePath}'.");
                }
                else
                {
                    debuglog($"WARN GCstudio Modules, File '{filePath}' does not exist.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting file: {ex.Message}");
                debuglog($"ERROR GCstudio Modules, Exception deleting file '{filePath}': {ex.Message}");
                return;
            }

            // Remove from modulesList
            var moduleToRemove = modulesList.FirstOrDefault(m => m.Name == moduleName);
            if (moduleToRemove != null)
            {
                modulesList.Remove(moduleToRemove);
                debuglog($"INFO GCstudio Modules, Removed module '{moduleName}' from modulesList.");
            }
            else
            {
                debuglog($"WARN GCstudio Modules, Module '{moduleName}' not found in modulesList.");
            }

            // Remove from checkedListBoxModules
            checkedListBoxModules.Items.Remove(moduleName);
            debuglog($"INFO GCstudio Modules, Removed module '{moduleName}' from checkedListBoxModules.");

            // Update the JSON file
            var jsonOut = JsonSerializer.Serialize(modulesList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(jsonFilePath, jsonOut);
            debuglog("INFO GCstudio Modules, Updated modules.json after removal.");
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.Close();
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
                debuglog("ERROR GCstudio  Modules, an error occurred when restarting application." + " > " + ex.Message + " @ " + ex.StackTrace);
                MessageBox.Show("An error occurred when restarting application.");
            }
        }
    }
}
