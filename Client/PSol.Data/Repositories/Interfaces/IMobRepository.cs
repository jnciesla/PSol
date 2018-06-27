using System;
using System.Collections.Generic;
using PSol.Data.Models;

namespace PSol.Data.Repositories.Interfaces
{
    public interface IMobRepository
    {
        Mob GetMobById(string id);
        ICollection<Mob> GetAllMobs(int minX, int maxX, int minY, int maxY);
        ICollection<Mob> GetAllDeadMobs();
        Mob Add(Mob mob);
        void SaveMob(Mob mob);
        ICollection<MobType> GetAllMobTypes();
    }
}
