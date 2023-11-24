using Database.Model;
using DbData;
using Microsoft.EntityFrameworkCore;
using MyDraftAPI_v2.FantasyDataModel;
//using MyDraftAPI_v2.FantasyDataModel.Draft;
using MyDraftAPI_v2.Managers;
using MyDraftLib.Utilities;
using ViewModel;
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
        private List<MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick>? _draftPicks;
        private ViewModel.DraftStatus? _draftStatus;
        private IDictionary<int, MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick> _draftPickMap;
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
        public List<MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick>? draftPicks
        {
            get
            {
                return _draftPicks;
            }
            set { _draftPicks = (List<MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick>?)value; }
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
        public Dictionary<DataModel.Enums.ProTeams, Dictionary<DataModel.Enums.Position, List<ViewModel.DepthChartPlayer>>> teamDepthChart
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

            _draftPicks = new List<MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick>();
            _draftPickMap = new Dictionary<int, MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick>();
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

                    _draftStatus = db.UserDraftStatus
                        .Where(x => x.LeagueID == _activeMyDraftLeague.ID)
                        .Select(i => new ViewModel.DraftStatus()
                        {
                            LeagueID = i.LeagueID,
                            CurrentPick = i.CurrentPick,
                            CurrentRound = i.CurrentRound,
                            fanTeamName = i.fanTeamName,
                            onTheClock = i.onTheClock,
                            IsComplete = i.IsComplete,
                        })
                        .AsNoTracking()
                        .FirstOrDefault();

                    if (_draftStatus != null)
                    {
                        var teamInfo = db.UserLeagueTeam
                                        .Where(q => q.ID == draftStatus.onTheClock)
                                        .AsNoTracking()
                                        .FirstOrDefault();

                        if (teamInfo != null)
                            _draftStatus.fanTeamName = teamInfo.Name;
                    }
                    #endregion

                    #region DraftPicks  
                    _draftPicks = db.UserDraftSelection
                            .Where(x => x.LeagueID == _activeMyDraftLeague.ID)
                            .Select(i => new MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick()
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
                    _draftPickMap = new Dictionary<int, MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick>(_draftPicks.Count);

                    foreach (MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick draftPick in _draftPicks)
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
        public IList<MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick> draftPicksForMyTeam()
        {
            IList<MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick> draftPicks = new List<MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick>();
            foreach (MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick draftPick in _draftPicks)
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

            using (var db = new AppDataContext(_dbOptionsBuilder.Options))
            {
                foreach (MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick draftPick in _draftPicks)
                {
                    if (draftPick.teamID == fanTeamID)
                    {
                        if (draftPick.playerID > 0)
                        {
                            draftPick.player = draftPick.playerID != 0 ? db.Player
                                                                        .Where(q => q.ID == draftPick.playerID)
                                                                        .FirstOrDefault() : null;
                        }

                        ViewModel.DraftPick viewModelDraftPick = new ViewModel.DraftPick
                        {
                            round = draftPick.round,
                            pickInRound = draftPick.pickInRound,
                            overallPick = draftPick.overallPick,
                            teamID = draftPick.teamID,
                            playerID = draftPick.playerID,
                            player = draftPick.player 
                        };

                        draftPicks.Add(viewModelDraftPick);
                    }
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
        public MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick onTheClockDraftPick()
        {
            return draftPickForOverall(_draftStatus.CurrentPick);
        }
        public async Task<Boolean> saveDraftPick(MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick draftPick)
        {
            //return await DBAdapter.executeUpdate("INSERT OR REPLACE INTO " + TABLE_USER_DRAFT_RESULTS +
            //        " (player_id, league_id, team_id, overall, round, pick_in_round, auction_value, is_keeper, timestamp) " +
            //        " VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)",
            //        draftPick.playerID, draftPick.leagueID, draftPick.teamID, draftPick.overall, draftPick.round, draftPick.pickInRound, draftPick.auctionValue, draftPick.isKeeper ? 1 : 0, timestamp);
            try
            {
                using (var db = new AppDataContext(_dbOptionsBuilder.Options))
                {
                    var userDraftSelecton = db.UserDraftSelection
                        .Where(x => x.LeagueID == draftPick.leagueID && x.OverallPick == draftPick.overallPick)
                        .FirstOrDefault();

                    if (userDraftSelecton != null)
                    {
                        userDraftSelecton.PlayerID = draftPick.playerID;
                        userDraftSelecton.LeagueID = draftPick.leagueID;
                        userDraftSelecton.TeamID = (int)draftPick.teamID;
                        userDraftSelecton.OverallPick = draftPick.overallPick;
                        userDraftSelecton.Round = draftPick.round;
                        userDraftSelecton.PickInRound = draftPick.pickInRound;
                        userDraftSelecton.IsKeeper = draftPick.isKeeper;
                        userDraftSelecton.DraftedTimeStamp = DateTime.Now;

                        db.Update(userDraftSelecton);
                        await db.SaveChangesAsync();

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick? nextAvailableDraftPickAfterOverall(int overall)
        {
            int totalPicks = _draftPicks.Count;
            if (totalPicks <= overall)
                return null;


            // Iterate over all draft picks after the overall pick. Stop when an empty spot (playerID == nil) is found.
            int startIndex = overall >= 0 ? overall : 0; // Start at 0 or greater
            for (int i = startIndex; i < totalPicks; i++)
            {
                MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick draftPick = _draftPicks[i];
                if (draftPick.player == null)
                    return draftPick;
            }
            return null;
        }
        public MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick draftPickForOverall(int overall)
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
                    return;
            }

            MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick otcPick = draftPickForOverall(overall);
            if (otcPick != null)
            {
                _draftStatus.CurrentPick = (int)otcPick.overallPick;
                saveDraftStatus(_draftStatus);
            }
        }
        public async Task changeDraftPickToTeam(int overall, FantasyTeam team)
        {
            MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick draftPick = draftPickForOverall(overall);
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
        public void updateOnTheClock(MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick vOtcPick)
        {
            MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick otcPick = onTheClockDraftPick();
            if (otcPick != null && otcPick.playerID == 0)
                return;

            int otcOverall = otcPick != null ? (int)otcPick.overallPick : 0;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick nextOTC = nextAvailableDraftPickAfterOverall(otcOverall);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            if (nextOTC == null)
            {
                setDraftComplete();
                return;
            }

            if (otcPick == null || nextOTC.overallPick != otcPick.overallPick)
            {
                _draftStatus.CurrentPick = (int)nextOTC.overallPick;
                otcPick = onTheClockDraftPick();

                // Update the OTC pick. If none can be found then declare the draft complete.
                if (otcPick != null)
                {
                    _draftStatus = new ViewModel.DraftStatus(_activeMyDraftLeague.UniverseID, _activeMyDraftLeague.ID, _draftStatus.CurrentPick, false);
                    saveDraftStatus(_draftStatus, otcPick);
                }
                else
                {
                    setDraftComplete();
                }
            }
        }
        private void setDraftComplete()
        {
            _draftStatus = new ViewModel.DraftStatus(_activeMyDraftLeague.UniverseID, _activeMyDraftLeague.ID, _league.rounds * _league.numTeams + 1, true); // Set on the clock to 1 pick beyond the end of the draft
            saveDraftStatus(_draftStatus);
        }
        public void saveDraftStatus(ViewModel.DraftStatus vDraftStatus, MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick vOtcPick = null)
        {
            using (var db = new AppDataContext(_dbOptionsBuilder.Options))
            {
                var draftStatus = db.UserDraftStatus
                    .Where(x => x.UniverseID == vDraftStatus.UniverseID && x.LeagueID == vDraftStatus.LeagueID)
                    .FirstOrDefault();

                if (draftStatus != null)
                {
                    draftStatus.CurrentPick = vDraftStatus.CurrentPick;
                    draftStatus.CurrentRound = (int)vOtcPick.round;
                    draftStatus.IsComplete = vDraftStatus.IsComplete;
                    draftStatus.onTheClock = (int)(vOtcPick != null ? vOtcPick.teamID : null);
                    draftStatus.fanTeamName = vOtcPick != null ? db.UserLeagueTeam
                                                                    .Where(q => q.ID == vOtcPick.teamID)
                                                                    .Select(i => i.Name)
                                                                    .FirstOrDefault() : null;
                    
                    _draftStatus = new ViewModel.DraftStatus()
                    {
                        UniverseID = draftStatus.UniverseID,
                        LeagueID = draftStatus.LeagueID,
                        CurrentPick = draftStatus.CurrentPick,
                        CurrentRound = draftStatus.CurrentRound,
                        fanTeamName = draftStatus.fanTeamName,
                        onTheClock = draftStatus.onTheClock,
                        IsComplete = draftStatus.IsComplete
                    };
                    
                    db.Update(draftStatus);
                    db.SaveChangesAsync();
                }
                else
                {
                    draftStatus = new Database.Model.UserDraftStatus()
                    {
                        LeagueID = vDraftStatus.LeagueID,
                        CurrentPick = vDraftStatus.CurrentPick,
                        IsComplete = vDraftStatus.IsComplete
                    };

                    db.Add(draftStatus);
                    db.SaveChangesAsync();
                }
            }
        }
        #endregion //  Draft Pick Manipulation  //

        #region //  Draft Event  //
        public void executeDraftPick(int overall, int playerID)
        {
            MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick draftPick = draftPickForOverall(overall);
            if (draftPick == null)
                return;
            try
            {
                MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPickMemento draftPickMemento = draftPick.getState();
                MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftMemento draftMemento = new MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftMemento();

                ////TODO:  NOT SURE IF THIS IS NEEDED
                //draftMemento.leagueID = _league.identifier;
                //draftMemento.onTheClock = _draftStatus.onTheClock;
                //draftMemento.MyDraftPickMemento = draftPickMemento;
                ////await DraftManager.pushDraftMementoToUndoStack(draftMemento);
                //await DraftManager.clearDraftMementoRedoStack(_league.identifier);

                draftPick.playerID = playerID;
                saveDraftPick(draftPick);

                MyDraftAPI_v2.FantasyDataModel.Draft.MyDraftPick otcPick = onTheClockDraftPick();
                if (otcPick.playerID != 0)
                {
                    updateOnTheClock(otcPick);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        #endregion

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
