using System.Security.Cryptography;
using System.Text;
using PSol.Data.Models;
using PSol.Data.Repositories.Interfaces;
using PSol.Data.Services.Interfaces;

namespace PSol.Data.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public User LoadPlayer(string username)
        {
            return _userRepo.LoadPlayer(username);
        }

        public void SavePlayer(User user)
        {
            _userRepo.SavePlayer(user);
        }

        public User RegisterUser(string username, string password)
        {
            var newUser = new User
            {
                Login = username,
                Password = CalculateMD5Hash(password),
                Name = username,
                X = 10,
                Y = 10,
                Rotation = 135,
                Health = 100,
                MaxHealth = 100,
                Shield = 100,
                MaxShield = 100,
                Rank = "2LT"
            };
            _userRepo.Add(newUser);
            return newUser;
        }

        public bool AccountExists(string username)
        {
            return _userRepo.AccountExists(username);
        }

        public bool PasswordOK(string username, string password)
        {
            return _userRepo.PasswordOK(username, CalculateMD5Hash(password));
        }

        private string CalculateMD5Hash(string input)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var t in hash)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
