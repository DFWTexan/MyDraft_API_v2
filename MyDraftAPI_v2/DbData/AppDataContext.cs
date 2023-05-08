using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace DbData
{
    public class AppDataContext : DbContext
    {
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDataContext()
        {
        }

        public AppDataContext(DbContextOptions<AppDataContext> options) : base(options) { }

        public DbSet<Database.Model.Player> Players { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Database.Model.Player>().ToTable("Players");
        }

    }
}
