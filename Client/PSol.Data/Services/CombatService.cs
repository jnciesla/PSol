using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using PSol.Data.Models;
using PSol.Data.Services.Interfaces;

namespace PSol.Data.Services
{
    public class CombatService : ICombatService
    {
        private readonly IMobService _mobService;
        private List<Combat> _pendingCombats;
        private List<Combat> _readyCombats;
        private const int CombatDistance = 2000;

        public CombatService(IMobService mobService)
        {
            _pendingCombats = new List<Combat>();
            _readyCombats = new List<Combat>();
            _mobService = mobService;
        }

        public Combat DoAttack(string targetId, string attackerId, string weaponId, List<User> allPlayers)
        {
            var mobs = _mobService.GetMobs().ToList();
            var combat = new Combat { SourceId = attackerId, TargetId = targetId, WeaponId = weaponId };
            var locale = new Vector2(0, 0);
            var sourcePlayer = allPlayers.Find(p => p?.Id == combat.SourceId);
            var sourceMob = mobs.Find(m => m.Id == combat.SourceId);
            var targetMob = mobs.Find(m => m.Id == combat.TargetId);

            // TODO: Get Weapon damage from weapon when implemented
            combat.WeaponDamage = new Random().Next(4, 8);
            // If target was a mob, do combat here. Otherwise do it in calling method cause players are annoying
            if (targetMob != null)
            {
                targetMob.Shield -= combat.WeaponDamage;
                if (targetMob.Shield < 0)
                {
                    targetMob.Health += targetMob.Shield;
                    targetMob.Shield = 0;
                }

                if (targetMob.Health <= 0)
                {
                    // TODO: EXPLODE
                    targetMob.Alive = false;
                    targetMob.KilledDate = DateTime.UtcNow;
                }
            }

            locale = sourcePlayer != null ? new Vector2(sourcePlayer.X, sourcePlayer.Y) : locale;
            locale = sourceMob != null ? new Vector2(sourceMob.X, sourceMob.Y) : locale;
            combat.X = (int)locale.X;
            combat.Y = (int)locale.Y;

            _pendingCombats.Add(combat);
            return combat;
        }

        public ICollection<Combat> GetCombats(int x, int y)
        {
            var minX = x - CombatDistance;
            var maxX = x + CombatDistance;
            var minY = y - CombatDistance;
            var maxY = y + CombatDistance;
            return _readyCombats.Where(c => c.X >= minX && c.X <= maxX && c.Y >= minY && c.Y <= maxY).ToList();
        }

        public void CycleArrays()
        {
            _readyCombats = _pendingCombats;
            _pendingCombats = new List<Combat>();
        }
    }
}
