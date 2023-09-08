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
        public DraftController(AppDataContext db, IConfiguration config, ILogger<DraftController> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
        }

        /// <summary>
        ///     
        /// Get All DraftStatus
        ///
        //[HttpGet("{id}")]
        //public ActionResult GetDraftStatus(int id)
        //{
        //    var service = new DraftService.DraftSvc(_db, _config, null, null);

        //    var result = service.GetDraftStatus(id);

        //    return StatusCode(result.StatusCode, result.ObjData);
        //}

        /// <summary>
        ///     
        /// Get All Daft Picks for League
        ///
        [HttpGet("{id}")]
        public ActionResult GetDraftPicksForLeague(int id)
        {
            var service = new DraftService.DraftSvc(_db, _config, null, null);

            var result = service.GetDraftPicksForLeague(id);

            return StatusCode(result.StatusCode, result.ObjData);
        }


    }
}
