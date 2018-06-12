using PSol.Data.Models;

namespace PSol.Data.Services.Interfaces
{
    public interface IUserService
    {
        User RegisterUser(string username, string password);
        User LoadPlayer(string username);
        void SavePlayer(User user);
    }
}
