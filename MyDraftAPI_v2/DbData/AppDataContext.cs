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
        public DbSet<Database.Model.PlayerNews> PlayerNews { get; set; }
        public DbSet<Database.Model.Points> Points { get; set; }
        public DbSet<Database.Model.ProTeam> ProTeams { get; set; }
        public DbSet<Database.Model.Position> Positions { get; set; }
        public DbSet<Database.Model.UserLeague> UserLeague { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Database.Model.Player>().ToTable("Players");
            modelBuilder.Entity<Database.Model.PlayerNews>().ToTable("PlayerNews");
            modelBuilder.Entity<Database.Model.Points>().ToTable("Points");
            modelBuilder.Entity<Database.Model.ProTeam>().ToTable("ProTeams");
            modelBuilder.Entity<Database.Model.Position>().ToTable("Positions");
            modelBuilder.Entity<Database.Model.UserLeague>().ToTable("UserLeague");
        }

    }
}
