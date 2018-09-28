using System;
using Bindings;
using Ninject;
using PSol.Data.Models;

namespace PSol.Server
{
    internal class General
	{
		private ServerTCP _stcp;
		private readonly ServerData _shd;
	    private readonly IKernel _kernel;

	    public General(IKernel kernel)
	    {
	        _kernel = kernel;
	        _shd = new ServerData(_kernel);
	    }

		public ServerData InitializeServer()
		{
		    _stcp = new ServerTCP();
            _shd.InitializeMessages();

			for (var i = 1; i < Constants.MAX_PLAYERS; i++)
			{
				ServerTCP.Clients[i] = new Client(_shd, _kernel);
				ServerTCP.tempPlayer[i] = new TempPlayer();
				Types.Player[i] = new User();
			}
			_stcp.InitializeNetwork();
		    Console.WriteLine(@"*********************************************");
            Console.WriteLine(@"Server has started");
		    Console.WriteLine(@"*********************************************");
            return _shd;
		}
	}
}
