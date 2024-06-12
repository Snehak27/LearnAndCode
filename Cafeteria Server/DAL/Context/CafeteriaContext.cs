using CafeteriaServer.DAL.Models;
using CafeteriaServer.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaServer.Context
{
    public class CafeteriaContext : DbContext
    {
        public CafeteriaContext(DbContextOptions<CafeteriaContext> options)
        : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<MenuItem> MenuItem { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
    }
}
