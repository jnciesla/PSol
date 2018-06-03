﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bindings;

namespace Server
{
	class General
	{
		private ServerTCP stcp;
		private HandleData shd;

		public HandleData InitializeServer()
		{
			stcp = new ServerTCP();
			shd = new HandleData();

			shd.InitializeMesssages();

			for (var i = 1; i < Constants.MAX_PLAYERS; i++)
			{
				ServerTCP.Clients[i] = new Client();
				ServerTCP.tempPlayer[i] = new TempPlayer();
				Types.Player[i] = new Types.PlayerStruct();
			}
			stcp.InitializeNetwork();

			Console.WriteLine("Server has started");
			return shd;
		}
	}
}
