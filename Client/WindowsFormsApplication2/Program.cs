using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static string ip;
        public static string myName; 
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                ip = args[0];
                myName = args[1];
            }
            else
            {
                ip = "10.245.16.15";
                myName = "Test";
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new Form1());
            }
            catch
            {
                MessageBox.Show("Server Unavailable");
                Application.Exit();
            }
        }
    }
}
