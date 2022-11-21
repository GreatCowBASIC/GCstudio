using System;
using System.Windows.Forms;

namespace GC_Studio
{
    static class Program
    {
        /// <summary>
        /// Run point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Loader());
            Application.Run(new ToolBoxLite());
        }
    }
}
