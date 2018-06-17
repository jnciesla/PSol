using System;
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

        public Star LoadStar(string id)
        {
            return _context.Stars.FirstOrDefault(u => u.Id == id);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
