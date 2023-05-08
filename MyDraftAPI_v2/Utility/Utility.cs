using DbData;

namespace UtilityService
{
    public class Utility
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;

        public Utility(AppDataContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }
    }
}
