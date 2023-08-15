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

        public PlayerController(AppDataContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        /// <summary>
        /// 
        /// Get All Players
        /// 
        [HttpGet]
        public async Task<ActionResult> GetPlayers()
        {
            var service = new PlayerService.Player(_db, _config, null, null);

            var result = await service.GetPlayers();

            return StatusCode(result.StatusCode, result.ObjData);
        }

        /// <summary>
        /// 
        /// Get Player by ID
        /// 
        [HttpGet]
        public ActionResult GetPlayerByID(int id)
        {
            var service = new PlayerService.Player(_db, _config, null, null);

            var result =  service.GetPlayerByID(id);

            return StatusCode(result.StatusCode, result.ObjData);
        }
        
    }
}
