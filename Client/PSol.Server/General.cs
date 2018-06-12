using System;
using Bindings;
using Ninject;
using PSol.Data.Models;

namespace PSol.Server
{
    internal class General
	{
		private ServerTCP stcp;
		private readonly HandleData _shd;

	    public General(IKernel kernel)
	    {
	        _shd = new HandleData(kernel);
	    }

		public HandleData InitializeServer()
		{
		    stcp = new ServerTCP();

			_shd.InitializeMessages();

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
