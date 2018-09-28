using System;
using System.Collections.Generic;
using System.Linq;
using PSol.Data.Models;
using PSol.Data.Repositories.Interfaces;
using System.Data.Entity;
using System.Data.Entity.Migrations;

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
            return _context.Users.AsNoTracking().FirstOrDefault(u => u.Name == username);
        }

        public void SavePlayer(User user)
        {
            var dbUser = GetUserById(user.Id);
            user.Inventory?.ToList().ForEach(inv =>
            {
                if (inv.Id == null)
                {
                    inv.Id = Guid.NewGuid().ToString();
                    _context.Inventory.Add(inv);
                }
                else
                {
                    var dbInv = _context.Inventory.FirstOrDefault(i => i.Id == inv.Id);
                    _context.Entry(dbInv).CurrentValues.SetValues(inv);
                }
            });

            dbUser.Inventory?.ToList().ForEach(inv =>
            {
                // If the new user's inventory doesnt contain an inventory that was on the db user, remove it
                if (!user.Inventory.ToList().Exists(i => i.Id == inv.Id))
                {
                    _context.Inventory.Remove(inv);
                }
            });
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
