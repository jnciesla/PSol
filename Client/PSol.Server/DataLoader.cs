using System;
using Ninject;
using PSol.Data.Services.Interfaces;

namespace PSol.Server
{
    class DataLoader
    {
        private static IStarService _starService;

        public void initialize()
        {
            IKernel kernel = new StandardKernel(new ServerModule());
            _starService = kernel.Get<IStarService>();
        }

        public void DownloadGalaxy()
        {
            Console.WriteLine(@"Downloading galaxy data...");
            _starService.LoadStars();
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine(@"Downloading galaxy data... PASS");
        }
    }
}
