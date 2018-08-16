using System.Collections.Generic;
using PSol.Data.Models;
using PSol.Data.Repositories.Interfaces;
using PSol.Data.Services.Interfaces;

namespace PSol.Data.Services
{
    public class ItemService: IItemService
    {
        private readonly IItemRepository _itemRep;
        private ICollection<Item> _items;

        public ItemService(IItemRepository itemRep)
        {
            _itemRep = itemRep;
        }

        public ICollection<Item> LoadItems()
        {
            if (_items != null) return _items;
            _items = _itemRep.LoadItems();
            return _items;
        }
    }
}
