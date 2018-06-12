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
        User GetUser(string id);

        User Add(User user);
    }
}
