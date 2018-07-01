using System.Collections.Generic;
using PSol.Data.Models;
using PSol.Data.Repositories.Interfaces;
using PSol.Data.Services.Interfaces;

namespace PSol.Data.Services
{
    public class StarService: IStarService
    {
        private readonly IStarRepository _starRep;
        private ICollection<Star> _stars;

        public StarService(IStarRepository starRep)
        {
            _starRep = starRep;
        }

        public ICollection<Star> LoadStars()
        {
            if (_stars != null) return _stars;
            _stars = _starRep.LoadStars();
            return _stars;
        }
    }
}
