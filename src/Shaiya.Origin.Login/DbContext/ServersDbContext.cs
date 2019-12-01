using Microsoft.EntityFrameworkCore;
using Shaiya.Origin.Login.Model;

namespace Shaiya.Origin.Database
{
    public class ServersDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO: move connection string to comfig file.
            optionsBuilder.UseSqlServer("Data Source=127.0.0.1;Initial Catalog=origin_gamedata;User ID=Shaiya;Password=Shaiya123");
        }

        public DbSet<Server> Servers { get; set; }
    }
}
