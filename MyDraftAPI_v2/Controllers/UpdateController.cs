using Microsoft.AspNetCore.Mvc;
using DbData;

namespace MyDraftAPI_v2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UpdateController : Controller
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<UpdateController> _logger;
        public UpdateController(AppDataContext db, IConfiguration config, ILogger<UpdateController> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
        }
        
        /// <summary>
        /// 

    }
}
