using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SHbit
{
    static class Program
    {
        private static Form1 myForm;

        public static Form1 MyForm
        {
            get { return Program.myForm; }
            set { Program.myForm = value; }
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Automation a = new Automation();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                myForm = new Form1();
                myForm.BringToFront();
                Application.Run(myForm);
            }
            catch (Exception e)
            {
                //write log
                Console.WriteLine(e);
                MessageBox.Show(e.Message + ": " + e.StackTrace);
            }
        }
    }
}
