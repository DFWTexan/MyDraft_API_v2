using Microsoft.AspNetCore.Mvc;
using DbData;

namespace MyDraftAPI_v2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StatsController : Controller
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<StatsController> _logger;
        public StatsController(AppDataContext db, IConfiguration config, ILogger<StatsController> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
        }
        
        /// <summary>
        /// 
    }
}
