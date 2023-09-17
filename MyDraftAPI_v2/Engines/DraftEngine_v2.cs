using Database.Model;
using DataModel.Response;
using DbData;
using DraftService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyDraftAPI_v2.FantasyDataModel;
using MyDraftAPI_v2.FantasyDataModel.Draft;
using MyDraftAPI_v2.Managers;
using static MyDraftAPI_v2.FantasyDataModel.FantasyLeage;

namespace MyDraftAPI_v2
{
    public class DraftEngine_v2 : IDisposable
    {
        private int _timerCount = 0;

        private IConfiguration _config;
        private readonly string _connectionString;
        private readonly DbContextOptionsBuilder<AppDataContext> _dbOptionsBuilder;
        private System.Threading.Timer _timer;

        private List<DraftPick> _draftPicks;
        private DraftStatus _draftStatus;
        private IDictionary<int, DraftPick> _draftPickMap;

        #region Properties
        public UserLeague? ActiveLeague { get; set; } = null;
        public IList<DraftPick> draftPicks
        {
            get
            {
                return _draftPicks;
            }
        }
        #endregion

        public DraftEngine_v2(ILogger<DraftEngine_v2> logger, Microsoft.Extensions.Hosting.IHostApplicationLifetime appLifetime, IConfiguration config)
        {
            _config = config;
            string? connString = _config.GetConnectionString("DefaultConnection");
            _connectionString = connString;

            _dbOptionsBuilder = new DbContextOptionsBuilder<AppDataContext>();
            _dbOptionsBuilder.UseSqlServer(_connectionString);

            _draftPicks = new List<DraftPick>();
            _draftPickMap = new Dictionary<int, DraftPick>();

            ActiveLeague = new UserLeague();
        }

        public void InitializeLeagueData_v2(Database.Model.UserLeague vInput)
        {
            using (var db = new AppDataContext(_dbOptionsBuilder.Options)) {
               
                try
                {
                    #region DraftStatus  
                    //_draftStatus = await _draftSvc.DraftStatus(leagueID);
                    var result = new DraftStatus();

                    var _draftStatus = db.UserDraftStatus
                        .Where(x => x.LeagueID == vInput.ID)
                        .Select(i => new DraftStatus()
                        {
                            leagueID = i.LeagueID,
                            onTheClock = i.CurrentPick,
                            isComplete = i.IsComplete,
                        })
                        .FirstOrDefault();
                    #endregion

                    #region DraftPicks  
                    //_draftPicks = _draftSvc.draftPicksForLeague(leagueID).ToList();
                    var _draftPicks = db.UserDraftSelection
                            .Where(x => x.LeagueID == vInput.ID)
                            .Select(i => new DraftPick()
                            {
                                leagueID = i.LeagueID,
                                overall = i.OverallPick,
                                round = i.Round,
                                pickInRound = i.Pick,
                                teamID = i.TeamID,
                                playerID = i.PlayerID.ToString(),
                            })
                            .ToList();
                    #endregion

                    #region DraftPickMap
                    _draftPickMap = new Dictionary<int, DraftPick>(_draftPicks.Count);

                    foreach (DraftPick draftPick in _draftPicks)
                    {
                        _draftPickMap.Add((int)draftPick.overall, draftPick);
                    }
                    #endregion

                }
                catch (Exception ex)
                {

                    throw;
                }
            };

            //_typeAuction = await DraftManager.isAuctionDraft();

            //await Task.Run(() => calculateCustomScoringAsync());
        }

        //public async Task calculateCustomScoringAsync()
        //{
        //    //int week = AppSettings.getProjectionStatsSegment();
        //    int week = 1;
        //    //if (!(await _league.isCustomScoringCalculated(week)))
        //    //{
        //    //    if (WillCalculateCustomScoring != null)
        //    //        WillCalculateCustomScoring(this, league);

        //    //    int year = AppSettings.getProjectionStatsYear();
        //    //    await _league.processCustomRankings(year, week);

        //    //    if (DidCalculateCustomScoring != null)
        //    //        DidCalculateCustomScoring(this, league);
        //    //}
        //}

        public async Task<IList<FantasyTeam>> TeamsForLeague()
        {
            using (var db = new AppDataContext(_dbOptionsBuilder.Options))
            {
                var teams =  await db.UserLeagueTeam
                                    .Where(q => q.LeagueID == ActiveLeague.ID)
                                    .OrderBy(q => q.DraftPosition)
                                    .Select(i => new FantasyTeam()
                                    {
                                        identifier = i.ID,
                                        leagueID = i.LeagueID,
                                        name = i.Name ?? "",
                                        abbr = i.Abbreviation ?? "",
                                        draftPosition = i.DraftPosition,
                                        owner = i.Owner ?? ""   
                                    })
                                    .ToListAsync();
                
                return teams;
            }
        }

        public async Task Initialize()
        {
            await Task.Run(() => StartTimer());
        }
        private void StartTimer()
        {
            //_timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
