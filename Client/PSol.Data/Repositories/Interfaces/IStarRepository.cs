using PSol.Data.Models;
using System.Collections.Generic;

namespace PSol.Data.Repositories.Interfaces
{
    public interface IStarRepository
    {
        ICollection<Star> LoadStars();
    }
}
