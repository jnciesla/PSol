using System;
using System.Linq;
using PSol.Data.Models;
using PSol.Data.Repositories.Interfaces;

namespace PSol.Data.Repositories
{
    public class UserRepository : IDisposable, IUserRepository
    {
        private readonly PSolDataContext _context;

        public UserRepository(PSolDataContext context)
        {
            _context = context;
        }

        public User GetUserById(string id)
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

        public User LoadPlayer(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Name == username);
        }

        public void SavePlayer(User user)
        {
            var dbUser = GetUserById(user.Id);
            _context.Entry(dbUser).CurrentValues.SetValues(user);
            _context.SaveChanges();
        }

        public bool AccountExists(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Name == username) != null;
        }

        public bool PasswordOK(string username, string passwordHash)
        {
            return _context.Users.FirstOrDefault(u => u.Name == username && u.Password == passwordHash) != null;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
