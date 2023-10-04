using Database.Model;
using DbData;
using Microsoft.EntityFrameworkCore;
using MyDraftAPI_v2.FantasyDataModel;
using MyDraftAPI_v2.FantasyDataModel.Draft;
using MyDraftAPI_v2.Managers;

namespace MyDraftAPI_v2
{
    public class DraftEngine_v2 : IDisposable
    {
        private int _timerCount = 0;

        private IConfiguration _config;
        private readonly string? _connectionString;
        private readonly DbContextOptionsBuilder<AppDataContext> _dbOptionsBuilder;
        private System.Threading.Timer? _timer;

        private FanDataModel.FantasyLeague? _league;
        private List<DraftPick> _draftPicks;
        private ViewModel.DraftStatus? _draftStatus;
        private IDictionary<int, DraftPick> _draftPickMap;

        #region Properties
        public UserLeague? ActiveLeague { get; set; } = null;
        public FanDataModel.FantasyLeague league
        {
            get { return _league; }
            set { _league = value; }
        }
        public IList<DraftPick>? draftPicks
        {
            get
            {
                return _draftPicks;
            }
        }
        public ViewModel.DraftStatus? draftStatus
        {
            get { return _draftStatus; }
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

        public void InitializeLeagueData_v2(ViewModel.ActiveLeague vInput)
        {
            _league = new FanDataModel.FantasyLeague()
            {
                UniverseID = vInput.UniverseID,
                identifier = vInput.ID,
                draftType = vInput.DraftType,
                //draftOrderType = vInput.DraftOrderType
            };
            _league.teams = (List<ViewModel.UserLeageTeamItem>)vInput.teams;

            using (var db = new AppDataContext(_dbOptionsBuilder.Options))
            {

                try
                {
                    #region DraftStatus  
                    var result = new ViewModel.DraftStatus();

                    var draftStatus = db.UserDraftStatus
                        .Where(x => x.LeagueID == vInput.ID)
                        .Select(i => new ViewModel.DraftStatus()
                        {
                            LeagueID = i.LeagueID,
                            CurrentPick = i.CurrentPick,
                            IsComplete = i.IsComplete,
                        })
                        .AsNoTracking()
                        .FirstOrDefault();

                    if (draftStatus != null)
                    {
                        _draftStatus = new ViewModel.DraftStatus()
                        {
                            LeagueID = draftStatus.LeagueID,
                            CurrentPick = draftStatus.CurrentPick,
                            IsComplete = draftStatus.IsComplete
                        };

                        var teamInfo = db.UserLeagueTeam
                                        .Where(q => q.ID == draftStatus.CurrentPick)
                                        .AsNoTracking()
                                        .FirstOrDefault();

                        if (teamInfo != null)
                            _draftStatus.fanTeam = teamInfo.Name;
                    }
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
                                pickInRound = i.PickInRound,
                                teamID = i.TeamID,
                                playerID = i.PlayerID.ToString(),
                            })
                            .AsNoTracking()
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
                var teams = await db.UserLeagueTeam
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
                                    .AsNoTracking()
                                    .ToListAsync();

                return teams;
            }
        }
        public async Task Initialize()
        {
            await Task.Run(() => StartTimer());
        }

        #region // Data Helper //
        public IList<DraftPick> draftPicksForTeam(FantasyTeam team)
        {
            IList<DraftPick> draftPicks = new List<DraftPick>(_league.numTeams);
            foreach (DraftPick draftPick in _draftPicks)
            {
                if (draftPick.teamID == team.identifier)
                {
                    draftPicks.Add(draftPick);
                }
            }
            return draftPicks;
        }
        #endregion

        #region //  Draft Pick Manipulation  //
        public DraftPick onTheClockDraftPick()
        {
            return draftPickForOverall(_draftStatus.CurrentPick);
        }
        public DraftPick? nextAvailableDraftPickAfterOverall(int overall)
        {
            int totalPicks = _draftPicks.Count;
            if (totalPicks <= overall)
            {
                return null;
            }

            // Iterate over all draft picks after the overall pick. Stop when an empty spot (playerID == nil) is found.
            int startIndex = overall >= 0 ? overall : 0; // Start at 0 or greater
            for (int i = startIndex; i < totalPicks; i++)
            {
                DraftPick draftPick = _draftPicks[i];
                if (draftPick.playerID == null)
                {
                    return draftPick;
                }
            }
            return null;
        }
        public DraftPick draftPickForOverall(int overall)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return _draftPickMap.ContainsKey(overall) ? _draftPickMap[overall] : null;
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task setOnTheClock(int overall)
        {
            if (_draftStatus == null)
            {
                return;
            } else
            {
                if (_draftStatus.CurrentPick == overall)
                {
                    return;
                }
            }

            DraftPick otcPick = draftPickForOverall(overall);
            if (otcPick != null)
            {
                _draftStatus.CurrentPick = (int)otcPick.overall;
                await saveDraftStatus(_draftStatus);
            }
        }
        public async Task changeDraftPickToTeam(int overall, FantasyTeam team)
        {
            DraftPick draftPick = draftPickForOverall(overall);
            draftPick.teamID = team.identifier;
            await DraftManager.saveDraftPick(draftPick);
        }
        //public async Task resetDraftPick(String playerID)
        //{
        //    DraftPick draftPick = draftPickForPlayerID(playerID);
        //    if (draftPick == null)
        //    {
        //        return;
        //    }

        //    if (_league.draftByTeamEnabled && _league.draftOrderType != FantasyLeague.DraftOrderType.auction)
        //    {
        //        resetDraftPick(draftPick);
        //        await DraftManager.saveDraftPick(draftPick);
        //    }
        //    else
        //    {
        //        await DraftManager.deleteDraftPick(draftPick);
        //        _draftPicks.Remove(draftPick);
        //        _draftPickMap.Remove((int)draftPick.overall);
        //    }

        //    if (DidChangeDraftPick != null)
        //    {
        //        DidChangeDraftPick(this, draftPick);
        //    }
        //}
        //public void resetDraftPick(DraftPick draftPick)
        //{
        //    draftPick.playerID = null;
        //    draftPick.isKeeper = false;
        //    draftPick.auctionValue = 0;
        //}
        ///*
        //public void resetDraftData()
        //{
        //    DraftManager.resetDraftData(_league);
        //    initializeLeagueData();
        //    updateOnTheClock();

        //    if (DidResetLeague != null) {
        //        DidResetLeague();
        //    }
        //}
        // * */
        public async Task updateOnTheClock()
        {
            DraftPick otcPick = onTheClockDraftPick();
            if (otcPick != null && otcPick.playerID == null)
                return;

            int otcOverall = otcPick != null ? (int)otcPick.overall : 0;
            DraftPick nextOTC = nextAvailableDraftPickAfterOverall(otcOverall);
            if (nextOTC == null)
            {
                await setDraftComplete();
                return;
            }

            if (otcPick == null || nextOTC.overall != otcPick.overall)
            {
                _draftStatus.CurrentPick = (int)nextOTC.overall;
                otcPick = onTheClockDraftPick();

                // Update the OTC pick. If none can be found then declare the draft complete.
                if (otcPick != null)
                {
                    _draftStatus = new ViewModel.DraftStatus(_league.identifier, _draftStatus.CurrentPick, 0, false);
                    await saveDraftStatus(_draftStatus);
                }
                else
                {
                    await setDraftComplete();
                }
            }
        }
        private async Task setDraftComplete()
        {
            _draftStatus = new ViewModel.DraftStatus(_league.identifier, _league.rounds * _league.numTeams + 1, 0, true); // Set on the clock to 1 pick beyond the end of the draft
            await saveDraftStatus(_draftStatus);
        }
        #endregion //  Draft Pick Manipulation  //

        public async Task saveDraftStatus(ViewModel.DraftStatus vDraftStatus)
        {
            using (var db = new AppDataContext(_dbOptionsBuilder.Options))
            {
                try
                {
                    db.Add(vDraftStatus);
                    await db.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
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
