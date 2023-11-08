using Microsoft.AspNetCore.Mvc;
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

        private DraftEngine_v2 _draftEngine;

        public DraftController(AppDataContext db, IConfiguration config, ILogger<DraftController> logger, DraftEngine_v2 draftEngine)
        {
            _db = db;
            _config = config;
            _logger = logger;
            _draftEngine = draftEngine;
        }

        /// <summary>
        ///     
        /// Get All DraftStatus
        ///
        [HttpGet]
        public ActionResult GetDraftStatus()
        {
            var result = new DataModel.Response.ReturnResult();

            result.ObjData = _draftEngine.draftStatus;

            return StatusCode(result.StatusCode = 200, result.ObjData);
        }

        /// <summary>
        ///     
        /// Get All Daft Picks for League
        ///
        [HttpPut]
        public ActionResult GetDraftPicksForLeague([FromBody] ViewModel.ActiveLeague vInput)
        {
            var service = new DraftService.DraftSvc(_db, _config, null, null, _draftEngine);

            var result = service.GetDraftPicksForLeague(vInput);

            return StatusCode(result.StatusCode, result.ObjData);
        }

        /// <summary>
        ///     
        /// Get All Daft Picks for League
        ///
        [HttpGet]
        public ActionResult GetDraftPicksForLeague_v2()
        {
            var service = new DraftService.DraftSvc(_db, _config, null, null, _draftEngine);

            var result = service.GetDraftPicksForLeague_v2();

            return StatusCode(result.StatusCode, result.ObjData);
        }

        /// <summary>
        ///     
        /// Get All Daft Picks for Fantasy Team
        ///
        [HttpGet("{fanTeamID}")]
        public ActionResult GetDraftPicksByFanTeam(int fanTeamID)
        {
            var service = new DraftService.DraftSvc(_db, _config, null, null, _draftEngine);

            var result = service.GetDraftPicksByFanTeam(fanTeamID);

            return StatusCode(result.StatusCode, result.ObjData);
        }

        /// <summary>
        ///     
        /// Get All Position Daft Picks for League
        ///
        [HttpGet]
        public ActionResult GetDraftPicksByPosition()
        {
            var service = new DraftService.DraftSvc(_db, _config, null, null, _draftEngine);

            var result = service.GetDraftPicksByPosition();

            return StatusCode(result.StatusCode, result.ObjData);
        }
    }
}
