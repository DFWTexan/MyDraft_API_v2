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

        //private DraftEngine_v2 _draftEngine;

        public LeagueController(AppDataContext db, IConfiguration config, ILogger<LeagueController> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
            //_draftEngine = draftEngine;
            //_draftEngine = draftEngine;
        }

        /// <summary>
        /// 
        /// Get Active League
        ///
        [HttpGet]
        public ActionResult GetActiveLeague()
        {
            var service = new LeagueService.LeagueSvc(_db, _config, null, null);

            var result = service.GetActiveLeague();

            return StatusCode(result.StatusCode, result.ObjData);
        }

        /// <summary>
        /// 
        /// Get Initialize League Data
        ///
        //[HttpPost]
        //public ActionResult InitLeageData([FromBody] Database.Model.UserLeague vInput)
        //{
        //    //var service = new LeagueService.LeagueSvc(_db, _config, null, null);

        //    var result = _draftEngine.InitializeLeagueData_v2(vInput);

        //    return StatusCode(result.StatusCode, result.ObjData);
        //}
    }
}
