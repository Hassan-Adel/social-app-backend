using Newtonsoft.Json;
using SocialApp.API.Models;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace SocialApp.API.Data
{
    public class Seed
    {
        private readonly ApplicationDBContext _context;
        public Seed(ApplicationDBContext context)
        {
            _context = context;
        }

        public void SeedUsers()
        {
            var userData = File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            foreach (var user in users)
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash("password", out passwordHash, out passwordSalt);
                //user.PasswordHash = passwordHash;
                //user.PasswordSalt = passwordSalt;
                user.UserName = user.UserName.ToLower();
                _context.Users.Add(user);
            }
            _context.SaveChanges();
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
