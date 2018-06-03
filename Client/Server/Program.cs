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
		private static HandleData shd;
		static void Main(string[] args)
		{
			general = new General();
			db = new Database();
			consoleThread = new Thread(new ThreadStart(ConsoleThread));
			consoleThread.Start();
			shd = general.InitializeServer();
		}

		static void ConsoleThread()
		{
			var saveTimer = new Timer(e => db.SaveGame(),
				null,
				TimeSpan.Zero,
				TimeSpan.FromMinutes(5));

			var pulseTimer = new Timer(e => shd.PreparePulseBroadcast(),
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
