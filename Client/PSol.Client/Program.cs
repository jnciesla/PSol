using System;

namespace PSol.Client
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Settings();
            using (var game = new Game1())
                game.Run();
        }

        private static void Settings()
        {
            HandleData.LoadXml();
            Globals.Fullscreen = Boolean.Parse(HandleData.ReadFromXml("Application", "Fullscreen", "true"));
            HandleData.CloseXml(true);
        }
    }
}
