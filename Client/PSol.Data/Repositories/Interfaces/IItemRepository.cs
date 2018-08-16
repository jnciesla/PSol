using System.Collections.Generic;
using PSol.Data.Models;

namespace PSol.Data.Repositories.Interfaces
{
    public interface IItemRepository
    {
        ICollection<Item> LoadItems();
    }
}
