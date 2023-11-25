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
        private UtilityService.Utility _utility;

        private DraftEngine_v2 _draftEngine;

        public LeagueController(AppDataContext db, IConfiguration config, ILogger<LeagueController> logger, DraftEngine_v2 draftEngine)
        {
            _db = db;
            _config = config;
            _logger = logger;
            _draftEngine = draftEngine;
            _utility = new UtilityService.Utility(_db, _config);
        }

        /// <summary>
        /// 
        /// Get Active League
        ///
        [HttpGet("{myDraftUserID}")]
        public async Task<ActionResult> GetActiveLeague(int? myDraftUserID = 1)
        {
            var service = new LeagueService.LeagueSvc(_db, _config, null, _utility, _draftEngine);

            var result = await Task.Run(() => service.GetActiveLeague(myDraftUserID));

            return StatusCode(result.StatusCode, result.ObjData);
        }

        /// <summary>
        /// 
        /// Get Initialize League Data
        ///
        [HttpGet]
        public async Task<ActionResult> InitLeageData()
        {
            await Task.Run(() => _draftEngine.InitializeLeagueData_v2());

            return Ok();
        }


        /// <summary>
        /// 
        /// Get Active League
        ///
        [HttpGet("{id}")]
        public async Task<ActionResult> TeamsForLeague(int id)
        {
            var service = new LeagueService.LeagueSvc(_db, _config, null, _utility, _draftEngine);

            var result = await Task.Run(() => service.TeamsForLeague(id));

            return StatusCode(result.StatusCode, result.ObjData);
        }
    }
}
