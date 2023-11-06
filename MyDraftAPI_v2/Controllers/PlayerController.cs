using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DbData;

namespace MyDraftAPI_v2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<PlayerController> _logger;

        private DraftEngine_v2 _draftEngine;

        public PlayerController(AppDataContext db, IConfiguration config, ILogger<PlayerController> logger, DraftEngine_v2 draftEngine)
        {
            _db = db;
            _config = config;
            _logger = logger;
            _draftEngine = draftEngine;
        }

        /// <summary>
        /// 
        /// Get Filtered Players
        /// 
        [HttpPut]
        public ActionResult GetPlayers([FromBody] ViewModel.FilterSortPlayer vInput)
        {
            var service = new PlayerService.PlayerSvc(_db, _config, null, null, _draftEngine);

            var result = service.GetPlayers(vInput);

            return StatusCode(result.StatusCode, result.ObjData);
        }

        /// <summary>
        /// 
        /// Get Player by ID
        /// 
        [HttpGet("{id}")]
        public ActionResult GetPlayerByID(int id)
        {
            var service = new PlayerService.PlayerSvc(_db, _config, null, null, _draftEngine);

            var result =  service.GetPlayerByID(id);

            return StatusCode(result.StatusCode, result.ObjData);
        }
        
    }
}
