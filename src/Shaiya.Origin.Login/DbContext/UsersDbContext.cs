using Microsoft.EntityFrameworkCore;
using Shaiya.Origin.Database.Model;

namespace Shaiya.Origin.Database
{
    public class UsersDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO: move connection string to comfig file.
            optionsBuilder.UseSqlServer("Data Source=127.0.0.1;Initial Catalog=PS_UserData;User ID=Shaiya;Password=Shaiya123");
        }

        public DbSet<User> Users { get; set; }
    }
}
