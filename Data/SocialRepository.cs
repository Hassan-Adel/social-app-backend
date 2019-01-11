using Microsoft.EntityFrameworkCore;
using SocialApp.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialApp.API.Data
{
    public class SocialRepository : ISocialRepository
    {
        private ApplicationDBContext _context;
        public SocialRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(db_user => db_user.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _context.Users.Include(db_user => db_user.Photos).ToListAsync();
            return users;
        }

        public async Task<bool> SaveAll()
        {
            return (await _context.SaveChangesAsync() > 0);
        }
    }
}
