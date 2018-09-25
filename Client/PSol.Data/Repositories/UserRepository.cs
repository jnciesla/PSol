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

        public Inventory GetInventoryById(string id)
        {
            return _context.Inventory.FirstOrDefault(i => i.Id == id);
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
            //return _context.Users.Include(i => i.Inventory).ToList().FirstOrDefault(u => u.Name == username);
            return _context.Users.FirstOrDefault(u => u.Name == username);
        }

        public void SavePlayer(User user)
        {
            var temp = user.Inventory;
            var dbUser = GetUserById(user.Id);
            user.Inventory = new List<Inventory>();
            _context.Entry(dbUser).CurrentValues.SetValues(user);
            _context.SaveChanges();
            user.Inventory = temp;
            SaveInventory(user);
        }

        public void SaveInventory(User user)
        {
            user.Inventory.ToList().ForEach(inv =>
            {
                _context.Database.ExecuteSqlCommand(
                    "UPDATE Inventories SET [Slot] = " + inv.Slot + " WHERE [Id] = '" + inv.Id + "'"
                );
            });
        }

        public void SaveInventory2(User user)
        {
            Console.WriteLine(user.Inventory.Count);
            user.Inventory.ToList().ForEach(inv =>
            {
                if (user.Id == null)
                {
                    inv.Id = new Guid().ToString();
                    _context.Inventory.Add(inv);
                }
                else
                {
                    _context.Inventory.AddOrUpdate(inv);
                }
                Console.WriteLine(user.Inventory.Count);
            });
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
