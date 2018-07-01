using Bindings;
using System;
using System.Linq;
using System.Threading;
using Ninject;
using PSol.Data.Services.Interfaces;

namespace PSol.Server
{
    internal class Program
    {
        private static Thread consoleThread;
        private static DataLoader dataLoader;
        private static PacketBuffer packet;
        private static General general;
        private static HandleData shd;
        private static IGameService _gameService;
        private static IMobService _mobService;

        private static void Main(string[] args)
        {
            packet = new PacketBuffer();
            IKernel kernel = new StandardKernel(new ServerModule());
            _gameService = kernel.Get<IGameService>();
            _mobService = kernel.Get<IMobService>();
            dataLoader = new DataLoader();
            general = new General(kernel);
            
            shd = general.InitializeServer();
            dataLoader.initialize();
            dataLoader.DownloadGalaxy();
            consoleThread = new Thread(ConsoleThread);
            consoleThread.Start();
        }

        private static void ConsoleThread()
        {
            var saveTimer = new Timer(e => _gameService.SaveGame(Types.Player.ToList()),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(5));

            var pulseTimer = new Timer(e =>
                {
                    shd.PreparePulseBroadcast();
                    if (Globals.FullData) { shd.PrepareStaticBroadcast(); }
                },
                null,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromMilliseconds(50));

            var repopTimer = new Timer(e => _mobService.RepopGalaxy(),
                null,
                TimeSpan.Zero, 
                TimeSpan.FromSeconds(30));

            string command = "";
            while (command != "end" && command != "e" && command != "exit" && command != "q" && command != "quit")
            {
                command = Console.ReadLine();

                if (command == "save")
                {
                    Console.SetCursorPosition(0,Console.CursorTop -1);
                    _gameService.SaveGame(Types.Player.ToList());
                }
                else if (command != "end")
                {
                    Console.WriteLine(@"Unknown Command");
                }
            }
            saveTimer.Dispose();
            pulseTimer.Dispose();
            repopTimer.Dispose();
            _gameService.SaveGame(Types.Player.ToList());
        }
    }
}
