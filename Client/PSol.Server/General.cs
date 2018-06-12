using System;
using Bindings;
using PSol.Data;
using PSol.Data.Models;
using PSol.Data.Repositories;
using PSol.Data.Services;

namespace Server
{
    internal class General
	{
		private ServerTCP stcp;
		private readonly HandleData _shd;

	    public General(HandleData shd)
	    {
	        _shd = shd;
	    }

		public HandleData InitializeServer()
		{
		    stcp = new ServerTCP();

			_shd.InitializeMesssages();

			for (var i = 1; i < Constants.MAX_PLAYERS; i++)
			{
				ServerTCP.Clients[i] = new Client(_shd);
				ServerTCP.tempPlayer[i] = new TempPlayer();
				Types.Player[i] = new User();
			}
			stcp.InitializeNetwork();
			Console.WriteLine("Server has started");
			return _shd;
		}
	}
}
