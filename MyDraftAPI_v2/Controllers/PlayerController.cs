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
        /// </summary>
        /// 
        [HttpGet]
        public async Task<ActionResult> GetPlayers()
        {
            var service = new PlayerService.Player(_db, _config, null, null);

            var result = await service.GetPlayers();

            return StatusCode(result.StatusCode, result.ObjData);
        }

    }
}
