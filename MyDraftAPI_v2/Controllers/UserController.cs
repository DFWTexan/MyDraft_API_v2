using DbData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyDraftAPI_v2.Services.Utility;

namespace MyDraftAPI_v2.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<DraftController> _logger;
        private UtilityService.Utility _utility;

        private DraftEngine_v2 _draftEngine;

        public UserController(AppDataContext db, IConfiguration config, ILogger<DraftController> logger, DraftEngine_v2 draftEngine)
        {
            _db = db;
            _config = config;
            _logger = logger;
            _draftEngine = draftEngine;
            _utility = new UtilityService.Utility(_db, _config);
        }

        /// <summary>
        ///     
        /// GET Login User
        ///
        [HttpGet]
        public async Task<ActionResult> UserInfoStatus()
        {
            try
            {
                var service = new UserService.UserSvc(_db, _config, null, _utility, _draftEngine);

                var result = await Task.Run(() => service.UserInfoStatus());

                return StatusCode(result.StatusCode, result.ObjData);
            }
            catch (Exception)
            {
                return StatusCode(500, new List<string>() { "Server Error" });
            }
        }
    }
}
