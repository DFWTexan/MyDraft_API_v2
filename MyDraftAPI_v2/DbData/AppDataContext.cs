using Microsoft.EntityFrameworkCore;

namespace MyDraftAPI_v2.DbData
{
    public class AppDataContext : DbContext
    {
        public AppDataContext()
        {
        }

        public AppDataContext(DbContextOptions<AppDataContext> options) : base(options) { }

    }
}
