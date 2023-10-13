using Microsoft.AspNetCore.Mvc;
using DbData;
using DataModel.Response;

namespace MyDraftAPI_v2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LeagueController : Controller
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<LeagueController> _logger;

        private DraftEngine_v2 _draftEngine;

        public LeagueController(AppDataContext db, IConfiguration config, ILogger<LeagueController> logger, DraftEngine_v2 draftEngine)
        {
            _db = db;
            _config = config;
            _logger = logger;
            _draftEngine = draftEngine;
        }

        /// <summary>
        /// 
        /// Get Active League
        ///
        [HttpGet]
        public ActionResult GetActiveLeague()
        {
            var service = new LeagueService.LeagueSvc(_db, _config, null, null, _draftEngine);

            var result = service.GetActiveLeague();

            _draftEngine.ActiveMyDraftLeague = result.ObjData as ViewModel.ActiveLeague;

            return StatusCode(result.StatusCode, result.ObjData);
        }

        /// <summary>
        /// 
        /// Get Initialize League Data
        ///
        [HttpGet]
        public ActionResult InitLeageData()
        {
            _draftEngine.InitializeLeagueData_v2();

            return Ok();
        }


        /// <summary>
        /// 
        /// Get Active League
        ///
        [HttpGet("{id}")]
        public ActionResult TeamsForLeague(int id)
        {
            var service = new LeagueService.LeagueSvc(_db, _config, null, null, _draftEngine);

            var result = service.TeamsForLeague(id);

            return StatusCode(result.StatusCode, result.ObjData);
        }
    }
}
