using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static FormHome myFormHome;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
             myFormHome = new FormHome();
            Application.Run(myFormHome);
        }
    }
}
