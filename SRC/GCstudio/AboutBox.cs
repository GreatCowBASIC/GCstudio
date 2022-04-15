using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using DBSEngine;

namespace GC_Studio
{
    partial class AboutBox : Form
    {
        DBS dbs=new DBS();
        public AboutBox()
        {

            InitializeComponent();
            
            labelassembly.Text = AssemblyVersion;
   


        }

        #region Descriptores de acceso de atributos de ensamblado

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void AboutBox_Load(object sender, EventArgs e)
        {
            try
            {
                dbs.LoadRead("GreatCowBasic\\Documentation\\acknowledgements.txt");
                textBoxDescription.Text = dbs.ReadAll();
                dbs.CloseRead();
            }
            catch
            {
                MessageBox.Show("Error when loading an about file.");
            }

            try
            {
                dbs.LoadRead("GreatCowBasic\\version.txt");
                labelver.Text = dbs.ReadData();
                dbs.CloseRead();
            }
            catch
            {
                MessageBox.Show("Error when loading an about file.");
            }


            try
            {
                dbs.LoadRead("FBasic\\version.txt");
                labelfbas.Text = dbs.ReadData();
                dbs.CloseRead();
            }
            catch
            {
                MessageBox.Show("Error when loading an about file.");
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                dbs.LoadRead("GreatCowBasic\\Documentation\\acknowledgements.txt");
                textBoxDescription.Text = dbs.ReadAll();
                dbs.CloseRead();
            }
            catch
            {
                MessageBox.Show("Error when loading an about file.");
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                dbs.LoadRead("GreatCowBasic\\Documentation\\license.txt");
                textBoxDescription.Text = dbs.ReadAll();
                dbs.CloseRead();
            }
            catch
            {
                MessageBox.Show("Error when loading an about file.");
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                dbs.LoadRead("GreatCowBasic\\Documentation\\readme.txt");
                textBoxDescription.Text = dbs.ReadAll();
                dbs.CloseRead();
            }
            catch
            {
                MessageBox.Show("Error when loading an about file.");
            }
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer", "https://www.gcbasic.com/bugtracking/changelog_page.php");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer", "https://www.gcbasic.com/bugtracking/roadmap_page.php");
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer", "https://www.gcbasic.com/bugtracking/bug_report_page.php");
        }
    }
}
