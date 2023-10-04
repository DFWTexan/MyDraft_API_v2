using Database.Model;
using DbData;
using Microsoft.EntityFrameworkCore;
using ViewModel;
using MyDraftAPI_v2.FantasyDataModel;
using System.Text;
using MyDraftAPI_v2.FantasyDataModel.Draft;
using System.Diagnostics;
using MyDraftAPI_v2.Services.Algorithms;

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

        public ViewModel.DraftStatus DraftStatus(int vUniversID, int vleagueID)
        {
            var result = new ViewModel.DraftStatus();

            var draftStatus = _db.UserDraftStatus
                .Where(x => x.LeagueID == vleagueID)
                .AsNoTracking()
                .FirstOrDefault();

            if (draftStatus != null)
            {
                result.UniverseID = draftStatus.UniverseID; 
                result.LeagueID = draftStatus.LeagueID;
                result.CurrentPick = draftStatus.CurrentPick;
                result.IsComplete = draftStatus.IsComplete;

                var teamInfo = _db.UserLeagueTeam
                    .Where(q => q.ID == draftStatus.CurrentPick)
                    .AsNoTracking()
                    .FirstOrDefault();

                if (teamInfo != null)
                    result.fanTeam = teamInfo.Name;

                return result;
            }
            else
            {
                return new ViewModel.DraftStatus(vleagueID, 0, 0, false);
            }
        }
        
        public List<ViewModel.DraftPick> draftPicksForLeague(int leagueID)
        {
            var draftPicks = _db.UserDraftSelection
                    .Where(x => x.LeagueID == leagueID)
                    .Select(q => new ViewModel.DraftPick()
                    {
                        leagueID = q.LeagueID,
                        overallPick = q.OverallPick,
                        round = q.Round,
                        pickInRound = q.PickInRound,
                        teamID = q.TeamID,
                        playerID = q.PlayerID
                    })
                    .OrderBy(q => q.overallPick)
                    .AsNoTracking()
                    .ToList();
                     
            return (List<ViewModel.DraftPick>)draftPicks;
        }
        public void saveDraftPicks(IList<ViewModel.DraftPick> draftPicks)
        {
            if (draftPicks.Count() == 0)
                return;

            try
            {
                List<UserDraftSelections> userDraftSelections = new List<UserDraftSelections>();
                foreach (var i in draftPicks)
                {
                    //var usrSelection = new UserDraftSelections()
                    //{
                    //    UniverseID = i.UniverseID,
                    //    LeagueID = i.leagueID,
                    //    TeamID = i.teamID,
                    //    PlayerID = i.playerID,
                    //    Round = i.round,
                    //    PickInRound = i.pickInRound,
                    //    OverallPick = i.overallPick,

                    //};
                    //userDraftSelections.Add(usrSelection);
                    _db.UserDraftSelection.Add(new UserDraftSelections()
                    {
                        UniverseID = i.UniverseID,
                        LeagueID = i.leagueID,
                        TeamID = i.teamID,
                        PlayerID = i.playerID,
                        Round = i.round,
                        PickInRound = i.pickInRound,
                        OverallPick = i.overallPick,
                    });
                }

                //_db.UserDraftSelection.AddRange(userDraftSelections);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public DataModel.Response.ReturnResult GetDraftPicksForLeague(ViewModel.ActiveLeague vInput)
        {
            var result = new DataModel.Response.ReturnResult();

            try
            {
                FantasyLeague fanLeague = new FantasyLeague()
                {
                    identifier = vInput.ID,
                    numTeams = vInput.NumberOfTeams,
                    rounds = vInput.NumberOfRounds,
                    draftByTeamEnabled = true,
                };

                foreach(var i in vInput.teams)
                {
                    var addItem = new FantasyTeam()
                    {
                        identifier = i.ID,
                        name = i.Name ?? "",
                        abbr = i.Abbreviation ?? "",
                    };
                    fanLeague.fanTeams.Add(addItem);
                }

                //var draftPicks = _db.UserDraftSelection
                //    .Where(x => x.LeagueID == vInput.ID)
                //    .AsNoTracking()
                //    .ToList();
                var draftPicks = draftPicksForLeague(vInput.ID);

                int totalPicks = vInput.NumberOfTeams * vInput.NumberOfRounds;
                if (draftPicks.Count > totalPicks)
                {
                    int startIndex = totalPicks;
                    int length = draftPicks.Count - totalPicks;
                    draftPicks.RemoveRange(startIndex, length);
                }
                else
                {
                    List<ViewModel.DraftPick> generatedDraftPicks = (List<ViewModel.DraftPick>)DraftPickGenerator_v2.generateDraftPicks(fanLeague);
                    int startIndex = draftPicks.Count;
                    int length = generatedDraftPicks.Count - draftPicks.Count;
                    IList<ViewModel.DraftPick> addedPicks = generatedDraftPicks.GetRange(startIndex, length);
                    draftPicks.AddRange(addedPicks);
                }

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
