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
                Rotation = 0,
                Health = 100,
                MaxHealth = 100,
                Shield = 100,
                MaxShield = 100
            };
            _userRepo.Add(newUser);
            return newUser;
        }

        private string CalculateMD5Hash(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
