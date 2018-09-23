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
        private readonly IMobService _mobService;
        public GameService(IUserRepository userRepository, IMobService mobService)
        {
            _userRepository = userRepository;
            _mobService = mobService;
        }
        public void SaveGame(List<User> users)
        {
            Console.WriteLine(@"Saving database");
            users.Where(u => u?.Id != null).ToList().ForEach(user =>
            {
                _userRepository.SavePlayer(user);
            });
            _mobService.SaveMobs();
        }
    }
}
