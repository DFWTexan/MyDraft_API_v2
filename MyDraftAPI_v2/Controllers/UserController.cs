using DbData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyDraftAPI_v2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<DraftController> _logger;

        private DraftEngine_v2 _draftEngine;

        public UserController(AppDataContext db, IConfiguration config, ILogger<DraftController> logger, DraftEngine_v2 draftEngine)
        {
            _db = db;
            _config = config;
            _logger = logger;
            _draftEngine = draftEngine;
        }

        /// <summary>
        ///     
        /// Login User
        ///
        [HttpPut]
        public ActionResult Login([FromBody] ViewModel.UserInfo vInput)
        {
            var service = new UserService.UserSvc(_db, _config, null, null, _draftEngine);

            var result = service.Login(vInput);

            return StatusCode(result.StatusCode, result.ObjData);
        }
    }
}
