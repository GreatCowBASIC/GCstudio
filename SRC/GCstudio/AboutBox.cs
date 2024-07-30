﻿using Ngine;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace GC_Studio
{
    partial class AboutBox : Form
    {
        DataFileEngine dfe = new DataFileEngine();
        public AboutBox()
        {
            debuglog("GCstudio About, initializing main panel...");

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


            labelver.Text = Loader.AppVer.ToString();
            labelinstall.Text = AppDomain.CurrentDomain.BaseDirectory;

            debuglog("GCstudio About, loading gcbasic version...");
            try
            {
                dfe.LoadRead("gcbasic\\version.txt");
                labelcomp.Text = dfe.ReadData();
                dfe.CloseRead();
                debuglog(labelcomp.Text);
            }
            catch (Exception ex)
            {
                debuglog("ERROR GCstudio About, an error occurred while loading gcbasic version file." + " > " + ex.Message + " @ " + ex.StackTrace);
                MessageBox.Show("Error when loading an about file.");
            }

            debuglog("GCstudio About, loading toolchain version...");
            try
            {
                dfe.LoadRead("toolchainversion.txt");
                labeltoolchain.Text = dfe.ReadData();
                dfe.CloseRead();
            }
            catch (Exception ex)
            {
                debuglog("ERROR GCstudio About, an error occurred while loading toochain version file." + " > " + ex.Message + " @ " + ex.StackTrace);
                MessageBox.Show("Error when loading an about file.");
            }

            debuglog("GCstudio About, loading gccode version...");
            try
            {
                dfe.LoadRead("vscode\\version.txt");
                labelgccode.Text = dfe.ReadData();
                dfe.CloseRead();
            }
            catch (Exception ex)
            {
                debuglog("ERROR GCstudio About, an error occurred while loading gccode version file." + " > " + ex.Message + " @ " + ex.StackTrace);
                MessageBox.Show("Error when loading an about file.");
            }

            debuglog("GCstudio About, loading fbasic version...");
            try
            {
                dfe.LoadRead("FBasic\\version.txt");
                labelfbas.Text = dfe.ReadData();
                dfe.CloseRead();
            }
            catch (Exception ex)
            {
                debuglog("ERROR GCstudio About, an error occurred while loading fbasic version file." + " > " + ex.Message + " @ " + ex.StackTrace);
                MessageBox.Show("Error when loading an about file.");
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            debuglog("GCstudio About, loading acknowlewdgements...");
            try
            {
                dfe.LoadRead("gcbasic\\Documentation\\acknowledgements.txt");
                textBoxDescription.Text = dfe.ReadAll();
                dfe.CloseRead();
                textBoxDescription.Visible = true;
            }
            catch (Exception ex)
            {
                debuglog("ERROR GCstudio About, an error occurred while loading acknowlegements file." + " > " + ex.Message + " @ " + ex.StackTrace);
                MessageBox.Show("Error when loading an about file.");
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            debuglog("GCstudio About, loading license...");
            try
            {
                dfe.LoadRead("gcbasic\\Documentation\\license.txt");
                textBoxDescription.Text = dfe.ReadAll();
                dfe.CloseRead();
                textBoxDescription.Visible = true;
            }
            catch (Exception ex)
            {
                debuglog("ERROR GCstudio About, an error occurred while loading license file." + " > " + ex.Message + " @ " + ex.StackTrace);
                MessageBox.Show("Error when loading an about file.");
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            debuglog("GCstudio About, loading readme...");
            try
            {
                dfe.LoadRead("gcbasic\\Documentation\\readme.txt");
                textBoxDescription.Text = dfe.ReadAll();
                dfe.CloseRead();
                textBoxDescription.Visible = true;
            }
            catch (Exception ex)
            {
                debuglog("ERROR GCstudio About, an error occurred while loading readme file." + " > " + ex.Message + " @ " + ex.StackTrace);
                MessageBox.Show("Error when loading an about file.");
            }
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            debuglog("GCstudio About, launching changelog in the explorer...");
            Process.Start("explorer", "https://www.gcbasic.com/bugtracking/changelog_page.php");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            debuglog("GCstudio About, launching roadmap in the explorer...");
            Process.Start("explorer", "https://www.gcbasic.com/bugtracking/roadmap_page.php");
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            debuglog("GCstudio About, launching bugreport in the explorer...");
            Process.Start("explorer", "https://www.gcbasic.com/bugtracking/bug_report_page.php");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (textBoxDescription.Visible)
            {
                textBoxDescription.Visible = false;
            }
            else
            {
                this.Close();
            }
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            debuglog("GCstudio About, launching gcbasic.com in the explorer...");
            Process.Start("explorer", "https://www.gcbasic.com");
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
                dl.RecordData(DateTime.UtcNow.ToString("[yyyy-MM-dd][HH:mm:ss.fff]") + ">>>\t" + logstr);
                dl.CloseWrite();
            }
            catch { }
        }

    }
}
