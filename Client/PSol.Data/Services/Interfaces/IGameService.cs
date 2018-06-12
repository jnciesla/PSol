using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSol.Data.Models;

namespace PSol.Data.Services.Interfaces
{
    public interface IGameService
    {
        void SaveGame(List<User> users);
    }
}
