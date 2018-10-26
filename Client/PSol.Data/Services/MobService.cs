using Bindings;
using Microsoft.Xna.Framework;
using PSol.Data.Models;
using PSol.Data.Repositories.Interfaces;
using PSol.Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PSol.Data.Services
{
    public class MobService : IMobService
    {
        private static readonly Random rnd = new Random();
        private readonly IMobRepository _mobRepo;
        private ICollection<Mob> _mobs = new List<Mob>();

        public MobService(IMobRepository mobRepo)
        {
            _mobRepo = mobRepo;
            _mobs = _mobRepo.GetAllMobs();
        }

        public ICollection<Mob> GetMobs(int minX = 0, int maxX = Constants.PLAY_AREA_WIDTH, int minY = 0, int maxY = Constants.PLAY_AREA_HEIGHT, bool getDead = false)
        {
            return getDead ? _mobs.Where(m => m.X >= minX && m.X <= maxX && m.Y >= minY && m.Y <= maxY).ToList()
                : _mobs.Where(m => m.X >= minX && m.X <= maxX && m.Y >= minY && m.Y <= maxY && m.Alive).ToList();
        }

        public void RepopGalaxy(bool forceAll = false)
        {
            Console.WriteLine(@"Repop");
            // Get all mobs and count them
            var activeMobs = GetMobs(getDead: true);
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
            var deadMobs = activeMobs.Where(m => m.Alive == false);
            // Go through all the dead mobs and check their spawn timer against their death time
            foreach (var mob in deadMobs)
            {
                // If difference is less than min spawn time, skip them
                if (DateTime.UtcNow.Subtract(mob.KilledDate).CompareTo(new TimeSpan(0, 0, mob.MobType.SpawnTimeMin)) < 0) continue;

                // If difference is between min and max, flip a coin and see if they want to spawn
                var coin = rnd.Next(0, 2) == 0;
                if (DateTime.UtcNow.Subtract(mob.KilledDate).CompareTo(new TimeSpan(0, 0, mob.MobType.SpawnTimeMax)) < 0 && coin) continue;

                // Otherwise the coin flip passed or we've passed max spawn time
                Console.WriteLine(@"Spawning " + mob.MobType.Name);

                // Determine if he's going to be special
                if (rnd.Next(0, 10) == 7) { mob.Special = true; }

                mob.Alive = true;
                mob.Health = mob.MobType.MaxHealth;
                mob.Rotation = rnd.Next(0, 360);
                mob.Shield = mob.MobType.MaxShield;
                mob.SpawnDate = DateTime.UtcNow;
                var xMod = rnd.Next(-1 * mob.MobType.SpawnRadius, mob.MobType.SpawnRadius);
                var yMod = rnd.Next(-1 * mob.MobType.SpawnRadius, mob.MobType.SpawnRadius);
                mob.X = mob.MobType.Star.X + xMod;
                mob.X = mob.X < 0 ? mob.X * -1 : mob.X;
                mob.Y = mob.MobType.Star.Y + yMod;
                mob.Y = mob.Y < 0 ? mob.Y * -1 : mob.Y;
                mob.Name = GenerateName(mob.Special);
                _mobs.Add(mob);
            }
        }

        private void AddDeadMob(MobType type)
        {
            var mob = new Mob()
            {
                Id = Guid.NewGuid().ToString(),
                Alive = false,
                KilledDate = DateTime.UtcNow,
                MobTypeId = type.Id,
                MobType = type
            };
            _mobs.Add(mob);
        }

        public void SaveMobs()
        {
            if (_mobs == null) return;
            foreach (var mob in _mobs)
            {
                _mobRepo.SaveMob(mob);
            }
        }

        public void WanderMobs()
        {
            // Go through mobs that aren't in combat or already wandering and wander them a little within their spawn radius
            GetMobs().Where(m => m.TargettingId == null && m.NavToX == null).ToList().ForEach(m =>
            {
                // 0.2% chance to wander
                if (rnd.Next(0, 999) > 1) return;
                var negX = rnd.Next(0, 2);
                var negY = rnd.Next(0, 2);
                var newX = m.X + rnd.Next(200, 500) * (negX == 0 ? 1f : -1f);
                var newY = m.Y + rnd.Next(200, 500) * (negY == 0 ? 1f : -1f);

                // Make sure they don't wander out of their zone
                if (newX > m.MobType.Star.X + 2 * m.MobType.SpawnRadius)
                {
                    newX = m.MobType.Star.X + 2 * m.MobType.SpawnRadius;
                }
                else if (newX < m.MobType.Star.X - 2 * m.MobType.SpawnRadius)
                {
                    newX = m.MobType.Star.X - 2 * m.MobType.SpawnRadius;
                }
                if (newY > m.MobType.Star.Y + 2 * m.MobType.SpawnRadius)
                {
                    newY = m.MobType.Star.Y + 2 * m.MobType.SpawnRadius;
                }
                else if (newY < m.MobType.Star.Y - 2 * m.MobType.SpawnRadius)
                {
                    newY = m.MobType.Star.Y - 2 * m.MobType.SpawnRadius;
                }

                m.NavToX = newX;
                m.NavToY = newY;
            });

            // Actually wander them
            GetMobs().Where(m => m.TargettingId == null && m.NavToX != null).ToList().ForEach(m =>
            {
                var start = new Vector2(m.X, m.Y);
                var destination = new Vector2(m.NavToX ?? m.X, m.NavToY ?? m.Y);
                var direction = Vector2.Normalize(destination - start);
                var distance = Vector2.Distance(start, destination);
                m.Rotation = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.ToRadians(90);

                m.X += direction.X * 4f;
                m.Y += direction.Y * 4f;
                if (distance.CompareTo(50F) > 0) return;
                m.NavToX = null;
                m.NavToY = null;
            });
        }

        public void CheckAggro()
        {

        }

        public void DoCombat()
        {

        }

        public string GenerateName(bool special)
        {
            string[] vowels = { "a", "e", "i", "o", "u", "a", "e", "i", "o", "u", "y", "oo", "ea" };
            string[] consonants = {
                "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "z",
                "ch", "nd", "qu", "rt", "ck", "st", "rr", "sl", "pl", "'", "ph"
            };
            string[] titles =
            {
                "angry","black-hearted","brooding","brute","dangerous","deadly","deathly","death-dealer","deceitful","despairing",
                "destroyer","devouring", "egregious","enraged","evil","fatal","fiery","fighter","foul","ghostly","harmful","hateful",
                "heathen","hectic","heinous","hopeless","hazardous","ignoble","ignorant","irate","jagged","jarring","jealous",
                "killer","livid","loathing","lunatic","lurker","lying","malignant","mendacious","mephitic","nag","nefarious",
                "nightmarish","objectionable","obscene","ominous","overwhelming","paradoxical","pejorative","perturbed","punisher",
                "quarrelsome","quick","resentful","sinister","sly","tank","taunting","torturous","traitorous","traumatic",
                "unholy","ungodly","unyielding","vanquisher","vengeful","violent","warrior","wicked","wretched","zealous"
            };
            string[] prefix =
            {
                "Anger",
                "Bad",
                "Black",
                "Bleak",
                "Blood",
                "Break",
                "Dare",
                "Dead",
                "Death",
                "Devil",
                "Dire",
                "Doubt",
                "Dread",
                "Empty",
                "Evil",
                "Fear",
                "Fire",
                "Flight",
                "Frost",
                "Ghost",
                "Hate",
                "Hell",
                "Hunger",
                "Ice",
                "Ire",
                "Jagged",
                "Jarring",
                "Kill",
                "Lust",
                "Metal",
                "Moon",
                "Night",
                "Null",
                "Quick",
                "Red",
                "Shadow",
                "Slander",
                "Smoke",
                "Spark",
                "Spiked",
                "Storm",
                "Strike",
                "Thorn",
                "Thunder",
                "Vile",
                "Void",
                "Wicked",
                "Zealous"
            };
            string[] suffix =
            {
                "adder",
                "blast",
                "blood",
                "breath",
                "crush",
                "death",
                "demon",
                "devil",
                "dusk",
                "ember",
                "fade",
                "fault",
                "fear",
                "fight",
                "fire",
                "flight",
                "hate",
                "jinx",
                "lightning",
                "matrix",
                "moon",
                "night",
                "nik",
                "nova",
                "null",
                "pit",
                "poison",
                "razor",
                "rex",
                "run",
                "seeker",
                "shadow",
                "smoke",
                "smolder",
                "snare",
                "soul",
                "spark",
                "spear",
                "spike",
                "star",
                "storm",
                "strike",
                "technic",
                "thunder",
                "thorn",
                "trance",
                "titan",
                "venom",
                "void",
                "wolf"
            };

            var length = rnd.Next(2, 5);

            // Given name
            var name = "";
            for (var i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0 when rnd.Next(0, 1) == 1:
                        name += vowels[rnd.Next(6)].ToUpper();
                        break;
                    case 0:
                        name += consonants[rnd.Next(19)].ToUpper();
                        break;
                    default:
                        name += vowels[rnd.Next(vowels.Length)];
                        name += consonants[rnd.Next(consonants.Length)];
                        break;
                }
            }

            // Surname
            var p = prefix[rnd.Next(prefix.Length)];
            var s = suffix[rnd.Next(suffix.Length)];

            while (string.Equals(s, p, StringComparison.CurrentCultureIgnoreCase))
            {
                s = suffix[rnd.Next(suffix.Length)];
            }

            name += " " + p + s;

            // Title
            if (special)
            {
                name += " the ";
                name += titles[rnd.Next(titles.Length)];
            }

            return name;
        }
    }
}