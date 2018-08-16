using System;
using System.Collections.Generic;
using System.Linq;
using PSol.Data.Models;
using PSol.Data.Repositories.Interfaces;

namespace PSol.Data.Repositories
{
    public class ItemRepository : IDisposable, IItemRepository
    {
        private readonly PSolDataContext _context;

        public ItemRepository(PSolDataContext context)
        {
            _context = context;
        }

        public ICollection<Item> LoadItems()
        {
            return _context.Items.ToList();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
