using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.Threading;

namespace SerialPortListener
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool instanceCountOne = false;
            using (Mutex mtex = new Mutex(true, "appPeso", out instanceCountOne))
            {
                if (instanceCountOne)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm());
                    mtex.ReleaseMutex();
                }
                else
                {
                    MessageBox.Show("Il programma \"appPeso\" risulta già aperto!\n\nControlla nella barra delle applicazioni di Windows.","Applicazione già aperta",MessageBoxButtons.OK,MessageBoxIcon.Stop);
                }
            }
            /* 
            //VERSIONE ORIGINALE CODICE
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            */
        }
    }
}
