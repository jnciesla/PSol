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
            return _context.Users.Include(i => i.Inventory).ToList().FirstOrDefault(u => u.Name == username);
        }

        public void SavePlayer(User user)
        {
            var dbUser = GetUserById(user.Id);
            //SaveInventory(user);
            _context.Entry(dbUser).CurrentValues.SetValues(user);
            _context.SaveChanges();
        }

        private void SaveInventory(User user)
        {
            foreach (var inv in user.Inventory)
            {
                _context.Inventory.AddOrUpdate(new Inventory
                {
                    Id = inv.Id,
                    ItemId = inv.ItemId,
                    Quantity = inv.Quantity,
                    Slot = inv.Slot,
                    UserId = inv.UserId
                });
            }
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
