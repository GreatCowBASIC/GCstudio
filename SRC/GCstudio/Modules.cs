using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GC_Studio
{
    public partial class Modules : Form
    {
        public Modules()
        {
            InitializeComponent();
        }

        private void Modules_Load(object sender, EventArgs e)
        {
            string ModulesDirectory = AppDomain.CurrentDomain.BaseDirectory + "Modules";

            if (Directory.Exists(ModulesDirectory))
            {
                string[] files = Directory.GetFiles(ModulesDirectory, "*.mpk");

                string[] fileNames = Array.ConvertAll(files, Path.GetFileName);
                checkedListBoxModules.Items.AddRange(fileNames);
            }
            else
            {
                MessageBox.Show("Directory not found: " + ModulesDirectory);
            }
        }
    }
}
