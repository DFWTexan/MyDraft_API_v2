using Database.Model;
using DbData;
using Microsoft.EntityFrameworkCore;
using MyDraftAPI_v2.FantasyDataModel;
using MyDraftAPI_v2.FantasyDataModel.Draft;
using MyDraftAPI_v2.Managers;
using MyDraftLib.Utilities;
#pragma warning disable 

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
        private ViewModel.ActiveLeague? _activeMyDraftLeague;
        private List<ViewModel.DraftPick>? _draftPicks;
        private ViewModel.DraftStatus? _draftStatus;
        private IDictionary<int, ViewModel.DraftPick> _draftPickMap;
        private int _myDraftFanTeamID = 1;
        private Dictionary<string, ViewModel.DraftPick> _rosterDict = new Dictionary<string, ViewModel.DraftPick>() {
            {"QB", new ViewModel.DraftPick() },
            {"RB1", new ViewModel.DraftPick() },
            {"RB2", new ViewModel.DraftPick() },
            {"WR1", new ViewModel.DraftPick() },
            {"WR2", new ViewModel.DraftPick() },
            {"TE", new ViewModel.DraftPick() },
            {"K1", new ViewModel.DraftPick() },
            {"D1", new ViewModel.DraftPick() },
            {"B1", new ViewModel.DraftPick() },
            {"B2", new ViewModel.DraftPick() },
            {"B3", new ViewModel.DraftPick() },
            {"B4", new ViewModel.DraftPick() }
        };
        private List<Database.Model.UserLeagueTeams> _userLeagueTeams = new List<UserLeagueTeams>();
        private Dictionary<DataModel.Enums.ProTeams, Dictionary<DataModel.Enums.Position, List<ViewModel.DepthChartPlayer>>> _teamDepthChart = new Dictionary<DataModel.Enums.ProTeams, Dictionary<DataModel.Enums.Position, List<ViewModel.DepthChartPlayer>>>();

        #region Properties
        public int MyDraftFanTeamID
        {
            get { return _myDraftFanTeamID; }
            set { _myDraftFanTeamID = value; }
        }
        public ViewModel.ActiveLeague ActiveMyDraftLeague
        {
            get { return _activeMyDraftLeague; }
            set { _activeMyDraftLeague = value; }
        }
        //public ViewModel.ActiveLeague ActiveMyDraftLeague 
        //{
        //    get { return _activeMyDraftLeague;  } 
        //}
        public FanDataModel.FantasyLeague league
        {
            get { return _league; }
            set { _league = value; }
        }
        public List<ViewModel.DraftPick>? draftPicks
        {
            get
            {
                return _draftPicks;
            }
            set { _draftPicks = (List<ViewModel.DraftPick>?)value; }
        }
        public ViewModel.DraftStatus? draftStatus
        {
            get { return _draftStatus; }
        }
        public List<ViewModel.UserLeageTeamItem>? fantasyTeams
        {
            get
            {
                return (List<ViewModel.UserLeageTeamItem>?)_league.teams;
            }
            //set => _league.teams = (List<ViewModel.UserLeageTeamItem>)value;
        }
        public Dictionary<DataModel.Enums.ProTeams, Dictionary<DataModel.Enums.Position, List<ViewModel.DepthChartPlayer>>>teamDepthChart
        {
            get { return _teamDepthChart; }
            set { _teamDepthChart = value; }
        }
        #endregion

        public DraftEngine_v2(ILogger<DraftEngine_v2> logger, Microsoft.Extensions.Hosting.IHostApplicationLifetime appLifetime, IConfiguration config)
        {
            _config = config;
            string? connString = _config.GetConnectionString("DefaultConnection");
            _connectionString = connString;

            _dbOptionsBuilder = new DbContextOptionsBuilder<AppDataContext>();
            _dbOptionsBuilder.UseSqlServer(_connectionString);

            _draftPicks = new List<ViewModel.DraftPick>();
            _draftPickMap = new Dictionary<int, ViewModel.DraftPick>();
        }

        public void InitializeLeagueData_v2()
        {
            //_activeMyDraftLeague = vInput;

            _league = new FanDataModel.FantasyLeague()
            {
                UniverseID = _activeMyDraftLeague.UniverseID,
                identifier = _activeMyDraftLeague.ID,
                draftType = _activeMyDraftLeague.DraftType,
                //draftOrderType = vInput.DraftOrderType
            };
            _league.teams = (List<ViewModel.UserLeageTeamItem>)_activeMyDraftLeague.teams;

            using (var db = new AppDataContext(_dbOptionsBuilder.Options))
            {
                try
                {
                    #region DraftStatus  
                    var result = new ViewModel.DraftStatus();

                    var draftStatus = db.UserDraftStatus
                        .Where(x => x.LeagueID == _activeMyDraftLeague.ID)
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
                    _draftPicks = db.UserDraftSelection
                            .Where(x => x.LeagueID == _activeMyDraftLeague.ID)
                            .Select(i => new ViewModel.DraftPick()
                            {
                                leagueID = i.LeagueID,
                                overallPick = i.OverallPick,
                                round = i.Round,
                                pickInRound = i.PickInRound,
                                teamID = i.TeamID,
                                playerID = i.PlayerID,
                            })
                            .OrderBy(q => q.overallPick)
                            .AsNoTracking()
                            .ToList();
                    #endregion

                    #region DraftPickMap
                    _draftPickMap = new Dictionary<int, ViewModel.DraftPick>(_draftPicks.Count);

                    foreach (ViewModel.DraftPick draftPick in _draftPicks)
                    {
                        _draftPickMap.Add((int)draftPick.overallPick, draftPick);
                    }
                    #endregion

                    #region TeamDepthChart
                    _teamDepthChart = db.ProTeam
                                        .ToDictionary(
                                            q => (DataModel.Enums.ProTeams)Enum.Parse(typeof(DataModel.Enums.ProTeams), q.Abbr),
                                            q => new Dictionary<DataModel.Enums.Position, List<ViewModel.DepthChartPlayer>>()
                                            {
                                                { DataModel.Enums.Position.QB, GetPlayersForPositionAndTeam(3, q.ID, DataModel.Enums.Position.QB) },
                                                { DataModel.Enums.Position.RB, GetPlayersForPositionAndTeam(4, q.ID, DataModel.Enums.Position.RB) },
                                                { DataModel.Enums.Position.WR, GetPlayersForPositionAndTeam(6, q.ID, DataModel.Enums.Position.WR) },
                                                { DataModel.Enums.Position.TE, GetPlayersForPositionAndTeam(3, q.ID, DataModel.Enums.Position.TE) },
                                                { DataModel.Enums.Position.PK, GetPlayersForPositionAndTeam(2, q.ID, DataModel.Enums.Position.PK) },
                                            }
                                        );
                    #endregion

                }
                catch (Exception ex)
                {
                    throw;
                }
            };
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
                                    .Where(q => q.LeagueID == ActiveMyDraftLeague.ID)
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
        public IList<ViewModel.DraftPick> draftPicksForMyTeam()
        {
            IList<ViewModel.DraftPick> draftPicks = new List<ViewModel.DraftPick>();
            foreach (ViewModel.DraftPick draftPick in _draftPicks)
            {
                if (draftPick.teamID == _myDraftFanTeamID)
                {
                    draftPicks.Add(draftPick);
                }
            }
            return draftPicks;
        }
        public IList<ViewModel.DraftPick> draftPicksForTeam(int fanTeamID)
        {
            IList<ViewModel.DraftPick> draftPicks = new List<ViewModel.DraftPick>();
            foreach (ViewModel.DraftPick draftPick in _draftPicks)
            {
                if (draftPick.teamID == fanTeamID)
                {
                    if (draftPick.playerID > 0)
                    {
                        using (var db = new AppDataContext(_dbOptionsBuilder.Options))
                        {
                            draftPick.player = draftPick.playerID != 0 ? db.Player
                                                                        .Where(q => q.ID == draftPick.playerID)
                                                                        .FirstOrDefault() : null;
                        }
                    }

                    draftPicks.Add(draftPick);
                }
            }
            return draftPicks;
        }
        public Dictionary<string, ViewModel.DraftPick> draftPicksForTeam_v2(int vFanTeamID)
        {
            Dictionary<string, ViewModel.DraftPick> draftPicks = _rosterDict;
            HashSet<ViewModel.DraftPick> uniqueValues = new HashSet<ViewModel.DraftPick>();

            var picks = draftPicksForTeam(vFanTeamID);
            foreach (var pick in picks)
            {
                if (pick.player != null)
                {
                    foreach (var draftPickKey in _rosterDict.Keys.ToList())
                    {
                        string positionPrefix = draftPickKey.Substring(0, 2);

                        if (positionPrefix == pick.player.Position.Trim() &&
                            (!draftPicks.ContainsKey(draftPickKey) || !uniqueValues.Contains(pick)))
                        {
                            draftPicks[draftPickKey] = pick;
                            uniqueValues.Add(pick);
                        }
                        else if (!draftPicks.ContainsKey(draftPickKey) || draftPicks[draftPickKey].teamID != vFanTeamID)
                        {
                            draftPicks[draftPickKey] = new ViewModel.DraftPick();
                        }
                    }
                }
            }
            return draftPicks;
        }
        // Helper method to get players for a specific position and team
        private List<ViewModel.DepthChartPlayer> GetPlayersForPositionAndTeam(int limitTake, int teamId, DataModel.Enums.Position position)
        {
            using (var db = new AppDataContext(_dbOptionsBuilder.Options))
            {
                return db.DepthChart
                    .OrderBy(DepthChart => DepthChart.Rank)
                    .Include(player => player.Player)
                    .Where(player => player.TeamID == teamId && player.PositionID == (int)position)
                    .Select(i => new ViewModel.DepthChartPlayer
                    {
                        PlayerID = i.PlayerID,
                        Name = i.Player.LastName,
                        Position = i.Player.Position,
                        Team = i.ProTeam.Abbr,
                    })
                    .AsNoTracking()
                    .Take(limitTake)
                    .ToList();
            }
        }
        #endregion

        #region //  Draft Pick Manipulation  //
        public ViewModel.DraftPick onTheClockDraftPick()
        {
            return draftPickForOverall(_draftStatus.CurrentPick);
        }
        public static async Task<Boolean> saveDraftPick(ViewModel.DraftPick draftPick)
        {
            double timestamp = TNUtility.DateTimeToUnixTimestamp(DateTime.Now);
            await Task.Delay(2000);
            return true;
            //return await DBAdapter.executeUpdate("INSERT OR REPLACE INTO " + TABLE_USER_DRAFT_RESULTS +
            //        " (player_id, league_id, team_id, overall, round, pick_in_round, auction_value, is_keeper, timestamp) " +
            //        " VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)",
            //        draftPick.playerID, draftPick.leagueID, draftPick.teamID, draftPick.overall, draftPick.round, draftPick.pickInRound, draftPick.auctionValue, draftPick.isKeeper ? 1 : 0, timestamp);
        }
        public ViewModel.DraftPick? nextAvailableDraftPickAfterOverall(int overall)
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
                ViewModel.DraftPick draftPick = _draftPicks[i];
                if (draftPick.playerID == null)
                {
                    return draftPick;
                }
            }
            return null;
        }
        public ViewModel.DraftPick draftPickForOverall(int overall)
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
            }
            else
            {
                if (_draftStatus.CurrentPick == overall)
                {
                    return;
                }
            }

            ViewModel.DraftPick otcPick = draftPickForOverall(overall);
            if (otcPick != null)
            {
                _draftStatus.CurrentPick = (int)otcPick.overallPick;
                await saveDraftStatus(_draftStatus);
            }
        }
        public async Task changeDraftPickToTeam(int overall, FantasyTeam team)
        {
            ViewModel.DraftPick draftPick = draftPickForOverall(overall);
            draftPick.teamID = team.identifier;
            await saveDraftPick(draftPick);
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
            ViewModel.DraftPick otcPick = onTheClockDraftPick();
            if (otcPick != null && otcPick.playerID == null)

                return;

            int otcOverall = otcPick != null ? (int)otcPick.overallPick : 0;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            ViewModel.DraftPick nextOTC = nextAvailableDraftPickAfterOverall(otcOverall);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            if (nextOTC == null)
            {
                await setDraftComplete();
                return;
            }

            if (otcPick == null || nextOTC.overallPick != otcPick.overallPick)
            {
                _draftStatus.CurrentPick = (int)nextOTC.overallPick;
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

        #region //  Draft Munipulation  //
        public async Task executeDraftPick(int overall, int playerID)
        {
            //DraftPick draftPick = draftPickForOverall(overall);
            //if (draftPick == null)
            //    return;

            //DraftPickMemento draftPickMemento = draftPick.getState();
            //DraftMemento draftMemento = new DraftMemento();
            //draftMemento.leagueID = _league.identifier;
            //draftMemento.onTheClock = _draftStatus.onTheClock;
            //draftMemento.draftPickMemento = draftPickMemento;
            ////await DraftManager.pushDraftMementoToUndoStack(draftMemento);
            //await DraftManager.clearDraftMementoRedoStack(_league.identifier);

            //draftPick.playerID = playerID;
            //draftPick.isKeeper = isKeeper;
            //await DraftManager.saveDraftPick(draftPick);

            //DraftPick otcPick = onTheClockDraftPick();
            //if (otcPick.playerID != null)
            //{
            //    await updateOnTheClock();
            //}

            //if (DidChangeDraftPick != null)
            //{
            //    DidChangeDraftPick(this, draftPick);
            //}
        }
        #endregion //  Draft Picks  //  

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
