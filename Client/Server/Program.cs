using Bindings;
using System;
using System.Threading;

namespace Server
{
    class Program
    {
        private static Thread consoleThread;
        private static General general;
        private static Database db;
        static void Main(string[] args)
        {
            general = new General();
            db = new Database();
            consoleThread = new Thread(new ThreadStart(ConsoleThread));
            consoleThread.Start();
            general.InitializeServer();
        }

        static void ConsoleThread()
        {
            string command = "";
            while (command.ToString() != "end")
            {
                command = Console.ReadLine();

                if (command.ToString() == "save")
                {
                    Console.WriteLine("Saving database...");
                    for (int i = 0; i < Constants.MAX_PLAYERS; i++)
                    {
                        if (Types.Player[i].Login != null)
                        {
                            db.SavePlayer(i);
                        }
                    }
                }
            }
        }
    }
}
