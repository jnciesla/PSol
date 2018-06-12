using PSol.Data.Models;

namespace PSol.Data.Services.Interfaces
{
    public interface IUserService
    {
        User RegisterUser(int index, string username, string password);
        void ClearPlayer(int index);
    }
}
