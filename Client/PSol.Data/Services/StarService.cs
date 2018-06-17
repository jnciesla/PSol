using PSol.Data.Models;
using PSol.Data.Repositories.Interfaces;
using PSol.Data.Services.Interfaces;

namespace PSol.Data.Services
{
    public class StarService: IStarService
    {
        private readonly IStarRepository _starRep;

        public StarService(IStarRepository starRep)
        {
            _starRep = starRep;
        }

        public Star LoadStar(string id)
        {
            return _starRep.LoadStar(id);
        }
    }
}
