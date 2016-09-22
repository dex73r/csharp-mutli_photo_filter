using System;
using System.Windows.Forms;

namespace school_color_changer {

    internal static class Program {

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main ( )
        {
            Application.EnableVisualStyles ( );
            Application.SetCompatibleTextRenderingDefault ( false );
            Application.Run ( new Schoolsucks ( ) );
        }

    }

}