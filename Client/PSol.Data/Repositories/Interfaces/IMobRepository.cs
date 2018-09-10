using System;
using System.Collections.Generic;
using PSol.Data.Models;

namespace PSol.Data.Repositories.Interfaces
{
    public interface IMobRepository
    {
        Mob GetMobById(string id);
        ICollection<Mob> GetAllMobs();
        Mob Add(Mob mob);
        void SaveMob(Mob mob);
        ICollection<MobType> GetAllMobTypes();
    }
}
