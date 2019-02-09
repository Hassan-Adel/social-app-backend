using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using SocialApp.API.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SocialApp.API.Data
{
    public class Seed
    {
        private UserManager<User> _userManager;

        public Seed(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // Will not declare as async void because that'll causee issues with SQL
        public void SeedUsers()
        {
            if (!_userManager.Users.Any())
            {
                var userData = File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                foreach (var user in users)
                {
                    _userManager.CreateAsync(user, "password").Wait();
                }
            }
        }
    }
}
