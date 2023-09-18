using GbbConnect.Configuration;

namespace GbbConnect
{
    internal static class Program
    {

        // =========================================
        // Parameters
        // =========================================


        public static Exception? ExDuringLoading { get; set; }

        internal static Configuration.Parameters Parameters { get; set; } = new();

        internal static void Parameters_Save()
        {
            if (Parameters != null)
                Parameters.Save(Parameters_GetFileName());
        }


        internal static void Parameters_Load()
        {
            var FileName = Parameters_GetFileName();
            try
            {
                Parameters = Configuration.Parameters.Load(Parameters_GetFileName());
            }
            catch (ApplicationException)
            {
                System.IO.File.Move(FileName, $"{FileName}_Bad_{DateTime.Now.ToString("yyMMdd_HHmmss")}.xml");

                throw;
            }


        }

        public static string OurGetMainDataDir()
        {
            string mainDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Gbb Software", "GbbConnect");
            Directory.CreateDirectory(mainDir);
            return mainDir;

        }

        public static string Parameters_GetFileName()
        {
            return System.IO.Path.Combine(OurGetMainDataDir(), "Parameters.xml");
        }

        // =========================================

        private static Form? m_MainForm;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.ThreadException += Application_ThreadException;

            m_MainForm = new MainForm();
            Application.Run(m_MainForm);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (m_MainForm != null)
                if (e.Exception.StackTrace == null || !e.Exception.StackTrace.StartsWith("   at System.Windows.Forms.Control.GetSafeHandle(IWin32Window window)"))
                    GbbLibWin.Log.ErrMsgBox(m_MainForm, e.Exception);

        }

    }
}