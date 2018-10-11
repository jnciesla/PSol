using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PSol.Data.Models;
using PSol.Data.Repositories.Interfaces;

namespace PSol.Data.Repositories
{
    public class StarRepository: IDisposable, IStarRepository
    {
        private readonly PSolDataContext _context;

        public StarRepository(PSolDataContext context)
        {
            _context = context;
        }

        public ICollection<Star> LoadStars()
        {
            return _context.Stars.Include(s => s.Planets).ToList();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
