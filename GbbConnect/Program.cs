namespace GbbConnect
{
    internal static class Program
    {

        // =========================================
        // Parameters
        // =========================================


        internal static GbbEngine.Configuration.Parameters Parameters { get; set; } = new();

        internal static void Parameters_Save()
        {
            if (Parameters != null)
                Parameters.Save(GbbEngine.Configuration.Parameters.Parameters_GetFileName());
        }


        internal static void Parameters_Load()
        {
            var FileName = GbbEngine.Configuration.Parameters.Parameters_GetFileName();
            try
            {
                Parameters = GbbEngine.Configuration.Parameters.Load(FileName);
            }
            catch (ApplicationException)
            {
                // move bad configuration file as "archive" file.
                System.IO.File.Move(FileName, $"{FileName}_Bad_{DateTime.Now.ToString("yyMMdd_HHmmss")}.xml");

                throw;
            }


        }


        // =========================================

        private static Form? m_MainForm;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 2023-10-10: move parameters and subdirectories to new place
            var src = GbbEngine.Configuration.Parameters.OurGetMainDataDir_Old();
            if (Directory.Exists(Path.Combine(src, "Parameters.xml")))
            {
                var dest = GbbEngine.Configuration.Parameters.OurGetUserBaseDirectory();
                foreach (var file in Directory.GetFiles(src))
                    File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));

                foreach (var dir in Directory.GetDirectories(src))
                    Directory.Move(dir, Path.Combine(dest, Path.GetFileName(dir)));

                Directory.Delete(src, true);
            }



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