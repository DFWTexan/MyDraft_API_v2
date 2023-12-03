using Microsoft.AspNetCore.Mvc;
using DbData;
using MyDraftAPI_v2.Services.Utility;

namespace MyDraftAPI_v2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StatsController : Controller
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<StatsController> _logger;
        private UtilityService.Utility _utility;

        public StatsController(AppDataContext db, IConfiguration config, ILogger<StatsController> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
            _utility = new UtilityService.Utility(_db, _config);
        }
        
        /// <summary>
        /// 
    }
}
