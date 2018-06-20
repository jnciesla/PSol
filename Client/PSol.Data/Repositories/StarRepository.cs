using System;
using System.Linq;
using PSol.Data.Models;
using PSol.Data.Repositories.Interfaces;
using PSol.Data.Services;

namespace PSol.Data.Repositories
{
    public class StarRepository: IDisposable, IStarRepository
    {
        private readonly PSolDataContext _context;

        public StarRepository(PSolDataContext context)
        {
            _context = context;
        }

        public Star[] LoadStars()
        {
            return _context.Stars.ToArray();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
