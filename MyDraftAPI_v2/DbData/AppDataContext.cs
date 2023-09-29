using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace DbData
{
    public class AppDataContext : DbContext
    {
        private readonly int _fanUniverse_ID;

        private readonly IHttpContextAccessor _httpContextAccessor;

        //public AppDataContext()
        //{
        //}

        public AppDataContext(DbContextOptions<AppDataContext> options) : base(options) 
        {
            //if (httpContextAccessor != null && httpContextAccessor.HttpContext != null)
            //{
            //    _httpContextAccessor = httpContextAccessor;

            //    if (_httpContextAccessor.HttpContext.User.Claims.Any())
            //    {
            //        //var claim = _httpContextAccessor.HttpContext.User.Claims.Where(c => c.Type == "Tenant_ID").SingleOrDefault();
            //        //if (claim != null)
            //        //{
            //        //    _ = int.TryParse(claim.Value.ToString(), out _tenant_ID);
            //        //}
            //    }
            //    else
            //    {
            //        //var headers = _httpContextAccessor.HttpContext.Request.Headers;

            //        //headers.TryGetValue("Key", out Microsoft.Extensions.Primitives.StringValues key);
            //        //headers.TryGetValue("Secret", out Microsoft.Extensions.Primitives.StringValues secret);

            //        _fanUniverse_ID = 101;
            //    }
            //} else
            //{
            //    _fanUniverse_ID = 101;
            //}
        }
        #region TABLES
        public DbSet<Database.Model.Player> Player { get; set; }
        public DbSet<Database.Model.PlayerNews> PlayerNews { get; set; }
        public DbSet<Database.Model.Points> Points { get; set; }
        public DbSet<Database.Model.ProTeam> ProTeam { get; set; }
        public DbSet<Database.Model.Position> Positions { get; set; }
        public DbSet<Database.Model.UserLeague> UserLeague { get; set; }
        public DbSet<Database.Model.DepthChart> DepthChart { get; set; }
        public DbSet<Database.Model.UserLeagueTeams> UserLeagueTeam { get; set; }
        public DbSet<Database.Model.UserDraftSelections> UserDraftSelection { get; set; }
        public DbSet<Database.Model.UserDraftStatus> UserDraftStatus { get; set; }
        public DbSet<Database.Model.Schedule> Schedule { get; set; }
        public DbSet<Database.Model.PlayerPosition> PlayerPosition { get; set; }
        public DbSet<Database.Model.Injury> Injuries { get; set; }
        public DbSet<Database.Model.DVDB> DVDB { get; set; }
        public DbSet<Database.Model.AAV> AAV { get; set; }
        public DbSet<Database.Model.ADP> ADP { get; set; }
        #endregion

        #region Views
        public DbSet<Database.Model.vw_PlayerListItem> vw_PlayerListItem { get; set; }
        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Database.Model.Player>().ToTable("Players");
            modelBuilder.Entity<Database.Model.PlayerNews>().ToTable("PlayerNews");
            modelBuilder.Entity<Database.Model.Points>().ToTable("Points").HasNoKey();
            modelBuilder.Entity<Database.Model.ProTeam>().ToTable("ProTeams");
            modelBuilder.Entity<Database.Model.Position>().ToTable("Positions");
            modelBuilder.Entity<Database.Model.UserLeague>().ToTable("UserLeague");
            modelBuilder.Entity<Database.Model.DepthChart>().ToTable("DepthChart");
            modelBuilder.Entity<Database.Model.UserLeagueTeams>().ToTable("UserLeagueTeams");
            modelBuilder.Entity<Database.Model.UserDraftSelections>().ToTable("UserDraftSelections").HasNoKey();
            modelBuilder.Entity<Database.Model.UserDraftStatus>().ToTable("UserDraftStatus").HasNoKey();
            modelBuilder.Entity<Database.Model.Schedule>().ToTable("Schedule");
            modelBuilder.Entity<Database.Model.PlayerPosition>().ToTable("PlayerPosition");
            modelBuilder.Entity<Database.Model.Injury>().ToTable("Injuries");
            modelBuilder.Entity<Database.Model.DVDB>().ToTable("DVDB").HasNoKey();
            modelBuilder.Entity<Database.Model.AAV>().ToTable("AAV").HasNoKey();
            modelBuilder.Entity<Database.Model.ADP>().ToTable("ADP").HasNoKey();

            #region Bridge Table Keys
            modelBuilder.Entity<Database.Model.DepthChart>()
                .HasKey(k => new { k.PlayerID, k.PositionID, k.TeamID });

            modelBuilder.Entity<Database.Model.UserDraftSelections>()
                .HasKey(k => new { k.LeagueID, k.PlayerID, k.TeamID });

            modelBuilder.Entity<Database.Model.PlayerPosition>()
                .HasKey(k => new { k.PlayerID, k.PositionID });
            #endregion

            #region Views
            modelBuilder.Entity<Database.Model.vw_PlayerListItem>().ToView("vw_PlayerListItem").HasNoKey();
            #endregion

            #region Property Descr
            modelBuilder.Entity<Database.Model.Points>()
                .Property(p => p.Value)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Database.Model.DVDB>()
                .Property(p => p.Value)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Database.Model.AAV>()
                .Property(p => p.StandardValue)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Database.Model.AAV>()
                .Property(p => p.PPRValue)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Database.Model.ADP>()
                .Property(p => p.StandardValue)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Database.Model.ADP>()
                .Property(p => p.PPRValue)
                .HasColumnType("decimal(5,2)");
            #endregion
        }

    }
}
