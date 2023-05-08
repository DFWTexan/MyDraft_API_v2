using Database.Model;
using DbData;
using Microsoft.EntityFrameworkCore;

namespace PlayerService
{
    public class Player
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly UtilityService.Utility _utility;
        //private readonly IMapper _mapper;
        //private readonly ILogger _logger;

        public Player(AppDataContext db, IConfiguration config, IWebHostEnvironment env, UtilityService.Utility utility)
        {
            _db = db;
            _config = config;
            _env = env;
            _utility = utility;
            //_mapper = mapper;
            //_logger = logger;
        }

        public async Task<DataModel.Response.ReturnResult> GetPlayers()
        {
            var result = new DataModel.Response.ReturnResult();
            try
            {
                var players = await _db.Players.ToListAsync();
                result.ObjData = players;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrMessage = ex.Message;
            }
            return result;
        }
    }
}
