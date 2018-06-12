using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSol.Data.Models;

namespace PSol.Data.Repositories
{
    public class UserRepository : IDisposable
    {
        private readonly PSolDataContext _context;

        public UserRepository(PSolDataContext context)
        {
            _context = context;
        }

        public User GetUser(string id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public User Add(User user)
        {
            user.Id = Guid.NewGuid().ToString();
            var dbUser = _context.Users.Add(user);
            _context.SaveChanges();
            return dbUser;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
