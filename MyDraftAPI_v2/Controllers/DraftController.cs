using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DbData;
#pragma warning disable 

namespace MyDraftAPI_v2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DraftController : Controller
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<DraftController> _logger;
        private UtilityService.Utility _utility;

        private DraftEngine_v2 _draftEngine;

        public DraftController(AppDataContext db, IConfiguration config, ILogger<DraftController> logger, DraftEngine_v2 draftEngine)
        {
            _db = db;
            _config = config;
            _logger = logger;
            _draftEngine = draftEngine;
            _utility = new UtilityService.Utility(_db, _config);
        }

        #region // Draft Data //
        /// <summary>
        ///     
        /// Get All DraftStatus
        ///
        [HttpGet]
        public async Task<ActionResult> GetDraftStatus()
        {
            var result = new DataModel.Response.ReturnResult();

            result.ObjData = await Task.Run(() => _draftEngine.draftStatus);

            return StatusCode(result.StatusCode = 200, result.ObjData);
        }

        /// <summary>
        ///     
        /// Get All Daft Picks for League
        ///
        [HttpPut]
        public async Task<ActionResult> GetDraftPicksForLeague([FromBody] ViewModel.ActiveLeague vInput)
        {
            var service = new DraftService.DraftSvc(_db, _config, null, null, _draftEngine);

            var result = await Task.Run(() => service.GetDraftPicksForLeague(vInput));

            return StatusCode(result.StatusCode, result.ObjData);
        }

        /// <summary>
        ///     
        /// Get All Daft Picks for League
        ///
        [HttpGet]
        public async Task<ActionResult> GetDraftPicksForLeague_v2()
        {
            var service = new DraftService.DraftSvc(_db, _config, null, null, _draftEngine);

            var result = await Task.Run(() => service.GetDraftPicksForLeague_v2());

            return StatusCode(result.StatusCode, result.ObjData);
        }

        /// <summary>
        ///     
        /// Get All Daft Picks for Fantasy Team
        ///
        [HttpGet("{fanTeamID}")]
        public async Task<ActionResult> GetDraftPicksByFanTeam(int fanTeamID)
        {
            var service = new DraftService.DraftSvc(_db, _config, null, null, _draftEngine);

            var result = await Task.Run(() => service.GetDraftPicksByFanTeam(fanTeamID));

            return StatusCode(result.StatusCode, result.ObjData);
        }

        /// <summary>
        ///     
        /// Get All Position Daft Picks for League
        ///
        [HttpGet]
        public async Task<ActionResult> GetDraftPicksByPosition()
        {
            var service = new DraftService.DraftSvc(_db, _config, null, null, _draftEngine);

            var result = await Task.Run(() => service.GetDraftPicksByPosition());

            return StatusCode(result.StatusCode, result.ObjData);
        }

        /// <summary>
        ///     
        /// Get All Roster position counts
        ///
        [HttpGet]
        public async Task<ActionResult> GetRosterTotalPositionCount()
        {
            var service = new DraftService.DraftSvc(_db, _config, null, null, _draftEngine);

            var result = await Task.Run(() => service.GetRosterTotalPositionCount());

            return StatusCode(result.StatusCode, result.ObjData);
        }

        /// <summary>
        ///     
        /// Get Roster for Fantasy Team
        ///
        //[HttpGet("{fanTeamID}")]
        //public ActionResult GetTeamRoster(int fanTeamID)
        //{
        //    var service = new DraftService.DraftSvc(_db, _config, null, null, _draftEngine);

        //    var result = service.GetTeamRoster(fanTeamID);

        //    return StatusCode(result.StatusCode, result.ObjData);
        //}

        /// <summary>
        ///     
        /// Get Fan Team Selections order
        ///
        [HttpGet("{fanTeamID}")]
        public async Task<ActionResult> GetTeamSelections(int fanTeamID)
        {
            var service = new DraftService.DraftSvc(_db, _config, null, null, _draftEngine);

            var result = await Task.Run(() => service.GetTeamSelections(fanTeamID));

            return StatusCode(result.StatusCode, result.ObjData);
        }

        /// <summary>
        ///     
        /// Get Position Depter Chart for Pro Teams
        ///
        [HttpGet("{position}")]
        public async Task<ActionResult> GetPositionDepthChart(string position)
        {
            var service = new DraftService.DraftSvc(_db, _config, null, null, _draftEngine);

            var result = await Task.Run(() => service.GetPositionDepthChart(position));

            return StatusCode(result.StatusCode, result.ObjData);
        }
        #endregion // Draft Data Methods //

        #region // Draft Event Methods //
        /// <summary>
        ///     
        /// Execute Draft Pick Selection
        ///
        [HttpPost("{overall}/{playerID}")]
        public async Task<ActionResult> ExecuteDraftPick(int overall, int playerID)
        {
            try
            {
                var service = new DraftService.DraftSvc(_db, _config, null, _utility, _draftEngine);

                var result = await Task.Run(() => service.ExecuteDraftPick(overall, playerID));

                return StatusCode(result.StatusCode, result.ObjData);
            }
            catch (Exception)
            {
                return StatusCode(500, new List<string>() { "Server Error" });
            }
            
        }
        #endregion // Draft Events //
    }
}
