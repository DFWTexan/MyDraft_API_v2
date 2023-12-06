using DbData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyDraftAPI_v2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProTeamController : ControllerBase
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<LeagueController> _logger;
        private UtilityService.Utility _utility;

        private DraftEngine_v2 _draftEngine;

        public ProTeamController(AppDataContext db, IConfiguration config, ILogger<LeagueController> logger, DraftEngine_v2 draftEngine)
        {
            _db = db;
            _config = config;
            _logger = logger;
            _draftEngine = draftEngine;
            _utility = new UtilityService.Utility(_db, _config);
        }

        /// <summary>
        ///     
        /// ProTeam News
        /// 
        [HttpGet("{proTeamID}")]
        public async Task<ActionResult> News(int proTeamID)
        {
            try
            {
                var service = new ProTeamService.ProTeamSvc(_db, _config, null, _utility, _draftEngine);

                var result = await Task.Run(() => service.News(proTeamID));

                return StatusCode(result.StatusCode, result.ObjData);
            }
            catch (Exception)
            {
                return StatusCode(500, new List<string>() { "Server Error" });
            }
        }

        /// <summary>
        ///     
        /// ProTeam News
        /// 
        [HttpGet("{proTeamID}")]
        public async Task<ActionResult> Schedule(int proTeamID)
        {
            try
            {
                var service = new ProTeamService.ProTeamSvc(_db, _config, null, _utility, _draftEngine);

                var result = await Task.Run(() => service.Schedule(proTeamID));

                return StatusCode(result.StatusCode, result.ObjData);
            }
            catch (Exception)
            {
                return StatusCode(500, new List<string>() { "Server Error" });
            }
        }
    }
}
