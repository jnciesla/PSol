using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Bindings;
using PSol.Data.Models;
using PSol.Data.Repositories;
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

        public User RegisterUser(int index, string username, string password)
        {
            ClearPlayer(index);
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
            Types.Player[index] = newUser;
            return newUser;
        }

        public void ClearPlayer(int index)
        {
            Types.Player[index].Login = "";
            Types.Player[index].Password = "";
            Types.Player[index].Name = "";
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
