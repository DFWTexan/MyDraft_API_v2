using Microsoft.AspNetCore.Mvc;
using DbData;
using DataModel.Response;
using MyDraftAPI_v2.Services.Utility;
using Microsoft.AspNetCore.Authorization;

namespace MyDraftAPI_v2.Controllers
{
    //[Authorize]
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
        [HttpGet]
        public async Task<ActionResult> GetActiveLeague()
        {
            try
            {
                var service = new LeagueService.LeagueSvc(_db, _config, null, _utility, _draftEngine);

                var result = await Task.Run(() => service.GetActiveLeague());

                return StatusCode(result.StatusCode, result.ObjData);
            }
            catch (Exception)
            {
                return StatusCode(500, new List<string>() { "Server Error" });
            }
            
        }

        /// <summary>
        /// 
        /// Create Fantasy League
        ///
        [HttpGet]
        public async Task<ActionResult> CreateLeague()
        {
            try
            {
                var service = new LeagueService.LeagueSvc(_db, _config, null, _utility, _draftEngine);

                var result = await Task.Run(() => service.CreateLeague());

                return StatusCode(result.StatusCode, result.ObjData);
            }
            catch (Exception)
            {
                return StatusCode(500, new List<string>() { "Server Error" });
            }

        }

        /// <summary>
        /// 
        /// Delete Fantasy League
        ///
        [HttpGet("{leagueID}")]
        public async Task<ActionResult> DeleteLeague(int leagueID)
        {
            try
            {
                var service = new LeagueService.LeagueSvc(_db, _config, null, _utility, _draftEngine);

                var result = await Task.Run(() => service.DeleteLeague(leagueID));

                return StatusCode(result.StatusCode, result.ObjData);
            }
            catch (Exception)
            {
                return StatusCode(500, new List<string>() { "Server Error" });
            }

        }

        /// <summary>
        ///     
        /// Change the Active League
        ///
        [HttpGet("{leagueID}")]
        public async Task<ActionResult> ChangeActiveLeague(int leagueID)
        {
            try
            {
                var service = new LeagueService.LeagueSvc(_db, _config, null, _utility, _draftEngine);

                var result = await Task.Run(() => service.ChangeActiveLeague(leagueID));

                return StatusCode(result.StatusCode, result.ObjData);
            }
            catch (Exception)
            {
                return StatusCode(500, new List<string>() { "Server Error" });
            }

        }

        /// <summary>
        /// 
        /// Get Active League
        ///
        [HttpGet("{id}")]
        public async Task<ActionResult> TeamsForLeague(int id)
        {
            try
            {
                var service = new LeagueService.LeagueSvc(_db, _config, null, _utility, _draftEngine);

                var result = await Task.Run(() => service.TeamsForLeague(id));

                return StatusCode(result.StatusCode, result.ObjData);
            }
            catch (Exception)
            {
                return StatusCode(500, new List<string>() { "Server Error" });
            }
            
        }
    }
}
