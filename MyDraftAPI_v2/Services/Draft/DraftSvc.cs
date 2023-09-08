using Database.Model;
using DbData;
using Microsoft.EntityFrameworkCore;
using DbData.ViewModel;
using MyDraftAPI_v2.FantasyDataModel;
using System.Text;
using MyDraftAPI_v2.FantasyDataModel.Draft;
using static MyDraftAPI_v2.FantasyDataModel.FantasyLeage;
using System.Diagnostics;

namespace DraftService
{
    public class DraftSvc
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly UtilityService.Utility _utility;
        //private readonly IMapper _mapper;
        //private readonly ILogger _logger;
        
        public DraftSvc(AppDataContext db, IConfiguration config, IWebHostEnvironment env, UtilityService.Utility utility)
        {
            _db = db;
            _config = config;
            _env = env;
            _utility = utility;
            //_mapper = mapper;
            //_logger = logger;
        }

        public async Task<DraftStatus> DraftStatus(int leagueID)
        {
            var result = new DraftStatus();

            var draftStatus = await _db.UserDraftStatus
                .Where(x => x.LeagueID == leagueID)
                .FirstOrDefaultAsync();
            
            if (draftStatus != null)
            {
                result.leagueID = leagueID;
                result.onTheClock = draftStatus.CurrentPick;
                result.isComplete = draftStatus.IsComplete;

                return result;
            }
            else
            {
                return new DraftStatus(leagueID, 0, 0, false);
            }
        }

        public IList<DraftPick> draftPicksForLeague(int leagueID)
        {
            var draftPicks = _db.UserDraftSelection
                    .Where(x => x.LeagueID == leagueID)
                    .ToList();

            return (IList<DraftPick>)draftPicks;
        }

        public DataModel.Response.ReturnResult GetDraftPicksForLeague(int leagueID)
        {
            var result = new DataModel.Response.ReturnResult();

            try
            {
                var draftPicks = _db.UserDraftSelection
                    .Where(x => x.LeagueID == leagueID)
                    .ToList();

                result.StatusCode = 200;
                result.ObjData = draftPicks;
            }
            catch (Exception ex)
            {
                result.StatusCode = 500;
                result.ErrMessage = ex.Message;
            }

            return result;
        }   
    }
}
