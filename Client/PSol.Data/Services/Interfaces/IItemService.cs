using System.Collections.Generic;
using PSol.Data.Models;

namespace PSol.Data.Services.Interfaces
{
    public interface IItemService
    {
        ICollection<Item> LoadItems();
    }
}