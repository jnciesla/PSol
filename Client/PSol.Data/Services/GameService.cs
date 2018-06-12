using System;
using System.Collections.Generic;
using System.Linq;
using PSol.Data.Models;
using PSol.Data.Repositories.Interfaces;
using PSol.Data.Services.Interfaces;

namespace PSol.Data.Services
{
    public class GameService : IGameService
    {
        private readonly IUserRepository _userRepository;
        public GameService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public void SaveGame(List<User> users)
        {
            Console.WriteLine("Saving database...");
            users.Where(u => u?.Id != null).ToList().ForEach(user =>
            {
                _userRepository.SavePlayer(user);
            });
        }
    }
}
