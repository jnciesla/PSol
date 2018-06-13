using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSol.Data.Models;

namespace PSol.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User GetUserById(string id);
        User Add(User user);
        void SavePlayer(User user);
        User LoadPlayer(string username);
        bool AccountExists(string username);
        bool PasswordOK(string username, string passwordHash);
    }
}
