using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SocialApp.API.Models;

namespace SocialApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private ApplicationDBContext _context;
        public AuthRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public Task<User> Login(User user, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public Task<bool> UserExists(string username)
        {
            throw new NotImplementedException();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
