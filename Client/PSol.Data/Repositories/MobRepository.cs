using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PSol.Data.Models;
using PSol.Data.Repositories.Interfaces;

namespace PSol.Data.Repositories
{
    public class MobRepository : IMobRepository
    {
        private readonly PSolDataContext _context;

        public MobRepository(PSolDataContext context)
        {
            _context = context;
        }

        public Mob GetMobById(string id)
        {
            return _context.Mobs.FirstOrDefault(m => m.Id == id);
        }

        public ICollection<Mob> GetAllMobs(int minX, int maxX, int minY, int maxY)
        {
                return _context.Mobs.Where(m => m.X >= minX && m.X <= maxX && m.Y >= minY && m.Y <= maxY)
                    .Include(i => i.MobType).Include(i => i.MobType.Star).ToList();
        }

        public ICollection<Mob> GetAllDeadMobs()
        {
            return _context.Mobs.Where(m => !m.Alive).ToList();
        }

        public Mob Add(Mob mob)
        {
            mob.Id = Guid.NewGuid().ToString();
            _context.Mobs.Add(mob);
            _context.SaveChanges();
            return mob;
        }

        public void SaveMob(Mob mob)
        {
            var dbMob = GetMobById(mob.Id);
            _context.Entry(dbMob).CurrentValues.SetValues(mob);
            _context.SaveChanges();
        }

        public ICollection<MobType> GetAllMobTypes()
        {
            return _context.MobTypes.ToList();
        }
    }
}
