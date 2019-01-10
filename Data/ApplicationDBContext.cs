using Microsoft.EntityFrameworkCore;
using SocialApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialApp.API.Data
{
    /**
     * After Updating ApplicationDBContext
     * 1- Add-Migration "Migration Name"
     * 2- Update-Database
     * 3- Remove-Migration (to remove latest migration)
     */
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base (options) { }
        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}
