using System.Collections.Generic;
using PSol.Data.Models;

namespace PSol.Data.Services.Interfaces
{
    public interface ICombatService
    {
        Combat DoAttack(string targetId, string attackerId, Item weapon);
        ICollection<Combat> GetCombats(int x, int y);
        void CycleArrays();
    }
}
