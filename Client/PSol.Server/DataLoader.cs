using System;
using Ninject;
using PSol.Data.Services.Interfaces;

namespace PSol.Server
{
    class DataLoader
    {
        private static IStarService _starService;
        private static IItemService _itemService;

        public void Initialize()
        {
            IKernel kernel = new StandardKernel(new ServerModule());
            _starService = kernel.Get<IStarService>();
            _itemService = kernel.Get<IItemService>();
        }

        public void DownloadGalaxy()
        {
            Console.WriteLine(@"Downloading galaxy data");
            Globals.Galaxy = _starService.LoadStars();
        }

        public void DownloadItems()
        {
            Console.WriteLine(@"Downloading item data");
            Globals.Items = _itemService.LoadItems();
        }
    }
}
