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
        private static General general;
        public static ServerData shd;
        private static IGameService _gameService;
        private static IMobService _mobService;
        private static bool pause = true;

        private static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel(new ServerModule());
            _gameService = kernel.Get<IGameService>();
            _mobService = kernel.Get<IMobService>();
            dataLoader = new DataLoader();
            general = new General(kernel);

            shd = general.InitializeServer();
            dataLoader.Initialize();
            dataLoader.DownloadGalaxy();
            dataLoader.DownloadItems();
            consoleThread = new Thread(ConsoleThread);
            consoleThread.Start();
        }

        private static void ConsoleThread()
        {
            var saveTimer = new Timer(e =>
                {
                    if (!pause) { _gameService.SaveGame(Types.Player.ToList()); }
                    else { pause = !pause; }
                },
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(5));

            var mobLogicTimer = new Timer(e =>
            {
                _mobService.WanderMobs();
                _mobService.CheckAggro();
                _mobService.DoCombat();
            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromMilliseconds(50));

            var rechargeTimer = new Timer(e =>
                {
                    Transactions.Charge(Types.Player.ToList());
                }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(500));

            var nebulaTimer = new Timer(e =>
            {
                Transactions.GenerateNebulae();
                shd.SendNebulae();
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));

            var pulseTimer = new Timer(e =>
                {
                    shd.PreparePulseBroadcast();
                    if (Globals.FullData) { shd.PrepareStaticBroadcast(); }
                },
                null,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromMilliseconds(25));

            var repopTimer = new Timer(e => _mobService.RepopGalaxy(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(30));

            var debrisTimer = new Timer(e => Transactions.ClearDebris(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(15));

            var command = "";
            while (command != "end" && command != "e" && command != "exit" && command != "q" && command != "quit")
            {
                command = Console.ReadLine();

                if (command == "save")
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    _gameService.SaveGame(Types.Player.ToList());
                }
                else if (command != "end" && command != "e" && command != "exit" && command != "q" && command != "quit")
                {
                    Console.WriteLine(@"Unknown Command");
                }
            }

            nebulaTimer.Dispose();
            saveTimer.Dispose();
            pulseTimer.Dispose();
            debrisTimer.Dispose();
            repopTimer.Dispose();
            mobLogicTimer.Dispose();
            rechargeTimer.Dispose();
            _gameService.SaveGame(Types.Player.ToList());
        }
    }
}
