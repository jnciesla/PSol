using Bindings;
using System;
using System.Threading;
using Ninject;

namespace PSol.Server
{
    internal class Program
    {
        private static Thread consoleThread;
        private static General general;
        private static SQL db;
        private static HandleData shd;

        private static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel(new ServerModule());
            general = new General(kernel);
            db = new SQL();
            consoleThread = new Thread(ConsoleThread);
            consoleThread.Start();
            shd = general.InitializeServer();
        }

        private static void ConsoleThread()
        {
            var saveTimer = new Timer(e => db.SaveGame(),
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
                TimeSpan.FromMilliseconds(100));

            string command = "";
            while (command != "end" && command != "e" && command != "exit" && command != "q" && command != "quit")
            {
                command = Console.ReadLine();

                if (command == "save")
                {
                    db.SaveGame();
                }
                else if (command != "end")
                {
                    Console.WriteLine("Unknown Command");
                }
            }
            saveTimer.Dispose();
            db.SaveGame();
        }
    }
}
