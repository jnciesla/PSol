using System.Collections.Generic;
using Bindings;
using PSol.Data.Models;

namespace PSol.Data.Services.Interfaces
{
    public interface IMobService
    {
        ICollection<Mob> GetMobs(int minX = 0, int maxX = Constants.PLAY_AREA_WIDTH, int minY = 0, int maxY = Constants.PLAY_AREA_HEIGHT);
        void RepopGalaxy(bool forceAll = false);
        void SaveMobs();
    }
}
