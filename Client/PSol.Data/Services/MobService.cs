using Bindings;
using PSol.Data.Models;
using PSol.Data.Repositories.Interfaces;
using PSol.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PSol.Data.Services
{
    public class MobService: IMobService
    {
        private readonly IMobRepository _mobRepo;

        public MobService(IMobRepository mobRepo)
        {
            _mobRepo = mobRepo;
        }

        public ICollection<Mob> GetMobs(int minX = 0, int maxX = Constants.PLAY_AREA_WIDTH, int minY = 0, int maxY = Constants.PLAY_AREA_HEIGHT)
        {
            return _mobRepo.GetAllMobs(minX, maxX, minY, maxY);
        }

        public void repopGalaxy(bool forceAll = false)
        {
            Console.WriteLine(@"Repop");
            // Get all mobs and count them
            var activeMobs = GetMobs();
            var countOfAllMobs = activeMobs.GroupBy(g => g.MobTypeId)
                .Select(s => new Tuple<string, int>(s.Key, s.Count())).ToList();
            // Get all mob types
            var mobTypes = _mobRepo.GetAllMobTypes();
            // Go through all mob types and see if any are missing and add them if so
            foreach (var mobType in mobTypes)
            {
                var tuple = countOfAllMobs.Find(a => a.Item1 == mobType.Id);
                // Found a tuple so at least some of the mobs exists
                if (tuple != null)
                {
                    // If the number that exist match the type's max spawned allowed, skip this type
                    if (tuple.Item2 >= mobType.MaxSpawned) continue;
                    // Not enough are out there so add more
                    for (var i = tuple.Item2; i != mobType.MaxSpawned; i++)
                    {
                        AddDeadMob(mobType);
                    }
                }
                else
                {
                    // Tuple wasn't found so add all mobs as dead so they will spawn
                    for (var i = 0; i != mobType.MaxSpawned; i++)
                    {
                        AddDeadMob(mobType);
                    }
                }
            }
            
            // Now we know that all of the possible mobs exist in the db (dead or alive). Look at the
            // dead ones and see if any are able to be spawned.
            var deadMobs = _mobRepo.GetAllDeadMobs();
            // Go through all the dead mobs and check their spawn timer against their death time
            var random = new Random();
            foreach (var mob in deadMobs)
            {
                // If difference is less than min spawn time, skip them
                if (DateTime.UtcNow.Subtract(mob.KilledDate).CompareTo(new TimeSpan(0, 0, mob.MobType.SpawnTimeMin)) < 0) continue;

                // If difference is between min and max, flip a coin and see if they want to spawn
                var coin = random.Next(0, 2) == 0;
                if (DateTime.UtcNow.Subtract(mob.KilledDate).CompareTo(new TimeSpan(0, 0, mob.MobType.SpawnTimeMax)) < 0 && coin) continue;
                
                // Otherwise the coin flip passed or we've passed max spawn time
                Console.WriteLine(@"Spawning " + mob.MobType.Name);
                mob.Alive = true;
                mob.Health = mob.MobType.MaxHealth;
                mob.Rotation = random.Next(0, 360);
                mob.Shield = mob.MobType.MaxShield;
                mob.SpawnDate = DateTime.UtcNow;
                var xMod = random.Next(-1 * mob.MobType.SpawnRadius, mob.MobType.SpawnRadius);
                var yMod = random.Next(-1 * mob.MobType.SpawnRadius, mob.MobType.SpawnRadius);
                mob.X = mob.MobType.Star.X * 100 + xMod;
                mob.X = mob.X < 0 ? mob.X * -1 : mob.X;
                mob.Y = mob.MobType.Star.Y * 100 + yMod;
                mob.Y = mob.Y < 0 ? mob.Y * -1 : mob.Y;
                _mobRepo.SaveMob(mob);
            }
        }

        private void AddDeadMob(MobType type)
        {
            var mob = new Mob()
            {
                Alive = false,
                KilledDate = DateTime.UtcNow,
                MobTypeId = type.Id
            };
            _mobRepo.Add(mob);
        }
    }
}
