using System;
using System.Collections.Generic;
using System.Text;

namespace Bindings
{
    public enum ServerPackets
    {
        SMessage = 1,
        SAckLogin = 2,
        SAckRegister = 3,
        SPlayerData = 4,
		SPulse = 5
    }

    public enum ClientPackets
    {
        CLogin = 1,
        CRegister = 2,
        CPlayerData = 3,
    }
}
