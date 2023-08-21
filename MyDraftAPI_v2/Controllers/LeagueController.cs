using Microsoft.AspNetCore.Mvc;
using DbData;

namespace MyDraftAPI_v2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LeagueController : Controller
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<LeagueController> _logger;

        public LeagueController(AppDataContext db, IConfiguration config, ILogger<LeagueController> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// 

    }
}
