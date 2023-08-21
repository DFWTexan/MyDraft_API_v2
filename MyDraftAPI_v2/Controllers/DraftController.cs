using Microsoft.AspNetCore.Mvc;
using DbData;

namespace MyDraftAPI_v2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DraftController : Controller
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<DraftController> _logger;
        public DraftController(AppDataContext db, IConfiguration config, ILogger<PlayerController> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
        }

        /// <summary>
        ///     
        /// Get All Drafts
        /// 


    }
}
