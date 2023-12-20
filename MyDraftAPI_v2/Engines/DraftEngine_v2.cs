using Database.Model;
using DbData;
using DraftService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MyDraftAPI_v2.FantasyDataModel;
//using MyDraftAPI_v2.FantasyDataModel.Draft;
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

        private ViewModel.UserInfo _myDraftUser = new ViewModel.UserInfo();
        private MyDraftAPI_v2.FantasyDataModel.MyFantasyLeague? _league;
        private ViewModel.ActiveLeague? _activeMyDraftLeague;
        private List<MyDraftAPI_v2.FantasyDataModel.MyDraftPick>? _draftPicks;
        private ViewModel.DraftStatus? _draftStatus;
        private IDictionary<int, MyDraftAPI_v2.FantasyDataModel.MyDraftPick> _draftPickMap;
        private int _myDraftFanTeamID;
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
        private List<Database.Model.UserLeagueTeam> _userLeagueTeams = new List<UserLeagueTeam>();
        private Dictionary<DataModel.Enums.ProTeams, Dictionary<DataModel.Enums.Position, List<ViewModel.DepthChartPlayer>>> _teamDepthChart = new Dictionary<DataModel.Enums.ProTeams, Dictionary<DataModel.Enums.Position, List<ViewModel.DepthChartPlayer>>>();

        #region Properties
        public ViewModel.UserInfo MyDraftUser
        {
            get { return _myDraftUser; }
            set { _myDraftUser = value; }
        }
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
        public MyDraftAPI_v2.FantasyDataModel.MyFantasyLeague league
        {
            get { return _league; }
            set { _league = value; }
        }
        public List<MyDraftAPI_v2.FantasyDataModel.MyDraftPick>? draftPicks
        {
            get
            {
                return _draftPicks;
            }
            set { _draftPicks = (List<MyDraftAPI_v2.FantasyDataModel.MyDraftPick>?)value; }
        }
        public ViewModel.DraftStatus? draftStatus
        {
            get { return _draftStatus; }
        }
        public List<ViewModel.UserLeagueTeamItem>? fantasyTeams
        {
            get
            {
                return (List<ViewModel.UserLeagueTeamItem>?)_league.teams;
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

            _draftPicks = new List<MyDraftAPI_v2.FantasyDataModel.MyDraftPick>();
            _draftPickMap = new Dictionary<int, MyDraftAPI_v2.FantasyDataModel.MyDraftPick>();
        }

        public void InitializeLeagueData_v2(int myDraftUserID)
        {
            using (var db = new AppDataContext(_dbOptionsBuilder.Options))
            {
                try
                {
                    bool isLeagueAvailable = db.UserLeague
                            .AsNoTracking()
                            .Any(x => x.MyDraftUserID == myDraftUserID);

                    #region LastActiveLeague
                    if (isLeagueAvailable)
                    {
                        //-- Get the last active league
                        _activeMyDraftLeague = db.UserLeague
                                             .Where(x => x.MyDraftUserID == myDraftUserID)
                                             .OrderByDescending(q => q.LastActiveDate)
                                             .Select(i => new ViewModel.ActiveLeague
                                             {
                                                 ID = i.ID,
                                                 Name = i.Name,
                                                 DraftType = i.DraftType,
                                                 DraftOrder = i.DraftOrder,
                                                 NumberOfRounds = i.NumberOfRounds,
                                                 NumberOfTeams = i.NumberOfTeams,
                                                 NumberOfStarters = i.NumberOfStarters,
                                                 LastActiveDate = DateTime.Now,
                                                 teams = i.LeagueTeams.Select(lt => new ViewModel.UserLeagueTeamItem
                                                 {
                                                     ID = lt.ID,
                                                     LeagueID = lt.LeagueID,
                                                     Name = lt.Name,
                                                     Abbreviation = lt.Abbreviation,
                                                     DraftPosition = lt.DraftPosition,
                                                     Owner = lt.Owner,
                                                     IsMyTeam = lt.IsMyTeam,
                                                 }).ToList()
                                             })
                                             .AsNoTracking()
                                             .FirstOrDefault();
                    }
                    else
                    {
                        _activeMyDraftLeague = createLeague(myDraftUserID);
                    }

                    var updUserLeague = new UserLeague
                    {
                        ID = _activeMyDraftLeague.ID,
                        LastActiveDate = DateTime.Now,
                    };
                    db.UserLeague.Attach(updUserLeague);
                    db.Entry(updUserLeague).Property(x => x.LastActiveDate).IsModified = true;
                    db.SaveChanges();

                    _myDraftFanTeamID = _activeMyDraftLeague.teams.Where(q => q.IsMyTeam == true).FirstOrDefault().ID;

                    _league = new MyDraftAPI_v2.FantasyDataModel.MyFantasyLeague()
                    {
                        identifier = _activeMyDraftLeague.ID,
                        draftType = _activeMyDraftLeague.DraftType,
                    };
                    _league.teams = (IList<UserLeagueTeamItem>?)_activeMyDraftLeague.teams;
                    #endregion

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
                            IsMyTeamPick = i.onTheClock == _myDraftFanTeamID ? true : false,
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
                            .Select(i => new MyDraftAPI_v2.FantasyDataModel.MyDraftPick()
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
                    _draftPickMap = new Dictionary<int, MyDraftAPI_v2.FantasyDataModel.MyDraftPick>(_draftPicks.Count);

                    foreach (MyDraftAPI_v2.FantasyDataModel.MyDraftPick draftPick in _draftPicks)
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
                                                { DataModel.Enums.Position.QB, GetPlayersForPositionAndTeam(2, q.ID, DataModel.Enums.Position.QB) },
                                                { DataModel.Enums.Position.RB, GetPlayersForPositionAndTeam(3, q.ID, DataModel.Enums.Position.RB) },
                                                { DataModel.Enums.Position.WR, GetPlayersForPositionAndTeam(4, q.ID, DataModel.Enums.Position.WR) },
                                                { DataModel.Enums.Position.TE, GetPlayersForPositionAndTeam(2, q.ID, DataModel.Enums.Position.TE) },
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

        #region // Draft Actions //
        public async Task<bool> changeActiveLeague(int vLeagueID)
        {
            using (var db = new AppDataContext(_dbOptionsBuilder.Options))
            {
                try
                {
                    // statement to update the last UserLeague active date
                    var updUserLeague = new UserLeague
                    {
                        ID = vLeagueID,
                        LastActiveDate = DateTime.Now,
                    };
                    db.UserLeague.Attach(updUserLeague);
                    db.Entry(updUserLeague).Property(x => x.LastActiveDate).IsModified = true;
                    db.SaveChanges();

                    // statement to get MyDraftUserID from UserLeague table where ID = vLeagueID
                    var myDraftUserID = db.UserLeague
                                        .Where(x => x.ID == vLeagueID)
                                        .Select(i => i.MyDraftUserID)
                                        .FirstOrDefault();

                    InitializeLeagueData_v2(myDraftUserID);

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region //  Draft Construction  //
        public ViewModel.ActiveLeague createLeague(int vMyDraftUserID)
        {
            using (var db = new AppDataContext(_dbOptionsBuilder.Options))
            {
                try
                {
                    String LEAGUE_TEMP_NAME = "My-League";
                    String LEAGUE_TEMP_ABBR = "ML";
                    int LEAGUE_TEMP_NUM_TEAMS = 12;
                    int LEAGUE_TEMP_ROSTER_SIZE = 16;
                    Boolean LEAGUE_TEMP_INCLUDE_IDP = false;
                    Boolean LEAGUE_TEMP_DRAFT_BY_TEAM = true;
                    MyFantasyLeague.DraftOrderType LEAGUE_TEMP_DRAFT_ORDER_TYPE = MyFantasyLeague.DraftOrderType.snake;
                    int LEAGUE_TEMP_DRAFT_TYPE = 1;

                    int uniqueIdentifier = db.UserLeague.Where(q => q.MyDraftUserID == vMyDraftUserID).Any() ? (int)db.UserLeague.Max(q => q.ID) : 0;

                    MyFantasyLeague leagueContainer = new MyFantasyLeague(0);
                    leagueContainer.name = String.Format("{0}-{1}", LEAGUE_TEMP_NAME, uniqueIdentifier + 1);
                    leagueContainer.abbr = String.Format("{0}{1}", LEAGUE_TEMP_ABBR, uniqueIdentifier + 1);
                    leagueContainer.numTeams = LEAGUE_TEMP_NUM_TEAMS;
                    leagueContainer.rounds = LEAGUE_TEMP_ROSTER_SIZE;
                    leagueContainer.isIncludeIDP = LEAGUE_TEMP_INCLUDE_IDP;
                    leagueContainer.draftByTeamEnabled = LEAGUE_TEMP_DRAFT_BY_TEAM;
                    leagueContainer.draftOrderType = LEAGUE_TEMP_DRAFT_ORDER_TYPE;
                    leagueContainer.draftType = LEAGUE_TEMP_DRAFT_TYPE;

                    var newLeague = new UserLeague
                    {
                        MyDraftUserID = vMyDraftUserID,
                        Name = leagueContainer.name,
                        Abbr = leagueContainer.abbr,
                        NumberOfTeams = leagueContainer.numTeams,
                        NumberOfRounds = leagueContainer.rounds,
                        DraftType = leagueContainer.draftType,
                        DraftOrder = leagueContainer.draftOrderType.ToString(),
                        LastActiveDate = DateTime.Now,
                    };

                    db.UserLeague.Add(newLeague);
                    db.SaveChanges();
                    int newLeagueID = newLeague.ID;

                    var resultObj = createTeams(newLeagueID, LEAGUE_TEMP_NUM_TEAMS);

                    var newMyFantasyLeague = new MyFantasyLeague(newLeague);

                    var result = db.UserLeague
                                .Where(q => q.ID == newLeagueID)
                                    .Select(i => new ViewModel.ActiveLeague
                                    {
                                        ID = i.ID,
                                        Name = i.Name,
                                        DraftType = i.DraftType,
                                        DraftOrder = i.DraftOrder,
                                        NumberOfRounds = i.NumberOfRounds,
                                        NumberOfTeams = i.NumberOfTeams,
                                        LastActiveDate = DateTime.Now,
                                        teams = i.LeagueTeams.Select(lt => new ViewModel.UserLeagueTeamItem
                                        {
                                            ID = lt.ID,
                                            LeagueID = lt.LeagueID,
                                            Name = lt.Name,
                                            Abbreviation = lt.Abbreviation,
                                            DraftPosition = lt.DraftPosition,
                                            Owner = lt.Owner,
                                            IsMyTeam = lt.IsMyTeam,
                                        }).ToList()
                                    })
                                    .AsNoTracking()
                                    .FirstOrDefault();

                    newMyFantasyLeague.teams = (IList<UserLeagueTeamItem>?)result.teams;
                    IList<MyDraftAPI_v2.FantasyDataModel.MyDraftPick> draftPicks = (IList<MyDraftAPI_v2.FantasyDataModel.MyDraftPick>)MyDraftPickGenerator.generateDraftPicks(newMyFantasyLeague);

                    foreach (MyDraftAPI_v2.FantasyDataModel.MyDraftPick draftPick in draftPicks)
                    {
                        var newDraftPick = new UserDraftSelection
                        {
                            LeagueID = draftPick.leagueID,
                            OverallPick = draftPick.overallPick,
                            Round = draftPick.round,
                            PickInRound = draftPick.pickInRound,
                            TeamID = draftPick.teamID,
                            PlayerID = draftPick.playerID,
                        };

                        db.UserDraftSelection.Add(newDraftPick);
                        db.SaveChanges();
                    }

                    //* Draft Status *//
                    var newDraftStatus = new UserDraftStatus
                    {
                        LeagueID = newLeagueID,
                        CurrentPick = 1,
                        CurrentRound = 1,
                        onTheClock = _myDraftFanTeamID,
                        fanTeamName = "My Team",
                        IsComplete = false,
                    };

                    db.UserDraftStatus.Add(newDraftStatus);
                    db.SaveChanges();

                    //* UserInfoStatus *//
                    _myDraftUser = db.MyDraftUser
                                    .Include(x => x.UserLeagues)
                                    .Where(x => x.ID == vMyDraftUserID)
                                    .Select(i => new ViewModel.UserInfo
                                    {
                                        UserName = i.UserName,
                                        UserEmail = i.UserEmail,
                                        IsLoggedIn = true,
                                        UserLeagues = i.UserLeagues.Select(l => new ViewModel.UserLeagueItem
                                        {
                                            Value = l.ID,
                                            Label = l.Name
                                        }).ToList()
                                    })
                                    .AsNoTracking()
                                    .FirstOrDefault();


                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public async Task<MyFantasyLeague> getLeagueWithID(int identifier)
        {
            using (var db = new AppDataContext(_dbOptionsBuilder.Options))
            {
                var league = db.UserLeague
                            .Where(q => q.ID == identifier)
                            .Select(i => new MyFantasyLeague()
                            {
                                identifier = i.ID,
                                name = i.Name,
                                abbr = i.Abbr,
                                numTeams = i.NumberOfTeams,
                                rounds = i.NumberOfRounds,
                                //isIncludeIDP = i.IsIncludeIDP,
                                //draftByTeamEnabled = i.DraftByTeamEnabled,
                                draftOrderType = (MyFantasyLeague.DraftOrderType)Enum.Parse(typeof(MyFantasyLeague.DraftOrderType), i.DraftOrder),
                                draftType = i.DraftType,
                                teams = (IList<UserLeagueTeamItem>)i.LeagueTeams.Select(lt => new UserLeagueTeamItem
                                {
                                    ID = lt.ID,
                                    LeagueID = lt.LeagueID,
                                    Name = lt.Name,
                                    Abbreviation = lt.Abbreviation,
                                    DraftPosition = lt.DraftPosition,
                                    Owner = lt.Owner
                                }).ToList()
                            })
                            .AsNoTracking()
                            .FirstOrDefault();

                //await Task.Delay(2000);
                //IList<FantasyLeague> values = await DBAdapter.executeQuery<FantasyLeague>("SELECT * FROM " + TABLE_USER_LEAGUES + " WHERE _id = ?", identifier);
                //IList<FantasyLeague> values = new List<FantasyLeague>();
                //IList<MyFantasyLeague> values = (IList<MyFantasyLeague>)await db.UserLeague.Where(q => q.ID == identifier).ToListAsync();

                //MyFantasyLeague league = null;
                //if (values.Count() > 0)
                //{
                //    league = values[0];
                //    league.teams = await teamsForLeague(league);
                //    //await league.initializeAsyncValues();
                //}

                return league;
            }
        }
        public async Task<IList<UserLeagueTeam>> teamsForLeague(MyFantasyLeague league)
        {
            using (var db = new AppDataContext(_dbOptionsBuilder.Options))
            {
                //await Task.Delay(2000);
                //IList<FantasyTeam> teams = await DBAdapter.executeQuery<FantasyTeam>("SELECT * FROM " + TABLE_USER_TEAMS + " WHERE league_id = ? ORDER BY draft_position ASC", league.identifier);
                //IList<FantasyTeam> teams = new List<FantasyTeam>();
                IList<UserLeagueTeam> teams = (IList<UserLeagueTeam>)await db.UserLeagueTeam.Where(q => q.LeagueID == league.identifier).OrderBy(q => q.DraftPosition).ToListAsync();

                //foreach (UserLeagueTeam team in teams)
                //{
                //    team.league = league;
                //    //team.auctionRosterCount = await team.getAuctionRosterCount();
                //    //team.budgetAmount = await team.getAuctionAmountAvailable();
                //}

                return teams;
            }
        }
        public async Task<bool> createTeams(int leagueId, int numTeams)
        {
            using (var db = new AppDataContext(_dbOptionsBuilder.Options))
            {
                var result = new DataModel.Response.ReturnResult();
                try
                {
                    int userTeamID = -1;
                    for (int i = 0; i < numTeams; i++)
                    {
                        String name;
                        String abbr;
                        String owner;
                        if (i == 0)
                        {
                            name = "My Team";
                            abbr = "MY";
                            owner = "Me";
                        }
                        else
                        {
                            int teamNameIdentifier = i + 1;
                            name = String.Format("Team {0}", teamNameIdentifier);
                            abbr = String.Format("TM{0}", teamNameIdentifier);
                            owner = String.Format("Owner {0}", teamNameIdentifier);
                        }

                        var newLeagueTeam = new UserLeagueTeam
                        {
                            LeagueID = leagueId,
                            Name = name,
                            Abbreviation = abbr,
                            Owner = owner,
                            DraftPosition = i + 1,
                            IsMyTeam = i == 0 ? true : false,
                        };
                        db.UserLeagueTeam.Add(newLeagueTeam);
                        db.SaveChanges();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        public async Task<int> maxTeamID()
        {
            using (var db = new AppDataContext(_dbOptionsBuilder.Options))
            {
                var maxTeamID = await db.UserLeagueTeam
                                        .MaxAsync(q => q.ID);

                return maxTeamID;
            }
        }
        private static List<ViewModel.UserLeagueTeamItem> GetUserLeageTeamItems(IList<Database.Model.UserLeagueTeam> leagueTeams)
        {
            List<ViewModel.UserLeagueTeamItem> items = new List<ViewModel.UserLeagueTeamItem>();
            foreach (var team in leagueTeams)
            {
                items.Add(new ViewModel.UserLeagueTeamItem()
                {
                    ID = team.ID,
                    LeagueID = team.LeagueID,
                    Name = team.Name,
                    Abbreviation = team.Abbreviation,
                    DraftPosition = team.DraftPosition,
                    Owner = team.Owner
                });
            }

            return items;
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
        #endregion

        #region //  Draft Information  //
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
        #endregion

        #region // Data Helper //
        public IList<MyDraftAPI_v2.FantasyDataModel.MyDraftPick> draftPicksForMyTeam()
        {
            IList<MyDraftAPI_v2.FantasyDataModel.MyDraftPick> draftPicks = new List<MyDraftAPI_v2.FantasyDataModel.MyDraftPick>();
            foreach (MyDraftAPI_v2.FantasyDataModel.MyDraftPick draftPick in _draftPicks)
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
                foreach (MyDraftAPI_v2.FantasyDataModel.MyDraftPick draftPick in _draftPicks)
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
                            fanTeamName = db.UserLeagueTeam.Where(x => x.ID == draftPick.teamID).FirstOrDefault().Name,
                            playerID = draftPick.playerID,
                            player = draftPick.player
                        };

                        draftPicks.Add(viewModelDraftPick);
                    }
                }
            }

            return draftPicks;
        }
        public Dictionary<string, ViewModel.DraftPick> DraftPicksForTeam_V2(int vFanTeamID)
        {
            var draftPicks = new Dictionary<string, ViewModel.DraftPick>(_rosterDict);
            var uniqueValues = new HashSet<ViewModel.DraftPick>();
            var uniquePositions = new HashSet<string>();

            var positionLimits = new Dictionary<string, int>
                {
                    {"QB", 1}, {"RB", 2}, {"WR", 2}, {"TE", 1}, {"PK", 1}, {"DEF", 1}
                };
            var positionCounts = new Dictionary<string, int>
                {
                    {"QB", 0}, {"RB", 0}, {"WR", 0}, {"TE", 0}, {"PK", 0}, {"DEF", 0}
                };

            foreach (var pick in draftPicksForTeam(vFanTeamID))
            {
                if (pick.player == null) continue;

                var playerPosition = pick.player.Position.Trim();
                if (!positionLimits.ContainsKey(playerPosition)) continue;

                var isStarter = positionCounts[playerPosition] < positionLimits[playerPosition];
                var rosterKey = FindRosterKeyForPick(draftPicks, uniqueValues, uniquePositions, playerPosition, isStarter);

                if (rosterKey != null)
                {
                    draftPicks[rosterKey] = pick;
                    uniqueValues.Add(pick);
                    uniquePositions.Add(rosterKey);
                    positionCounts[playerPosition]++;
                }
            }

            // Assign empty picks to remaining roster slots
            AssignEmptyPicksToRoster(draftPicks, vFanTeamID);

            return draftPicks;
        }

        private string FindRosterKeyForPick(Dictionary<string, ViewModel.DraftPick> draftPicks, HashSet<ViewModel.DraftPick> uniqueValues, HashSet<string> uniquePositions, string playerPosition, bool isStarter)
        {
            foreach (var key in _rosterDict.Keys)
            {
                if (!isStarter && key.StartsWith("B") && CanAssignPick(draftPicks, uniqueValues, uniquePositions, key))
                {
                    return key;
                }
                if (isStarter && key.StartsWith(playerPosition) && CanAssignPick(draftPicks, uniqueValues, uniquePositions, key))
                {
                    return key;
                }
            }

            return null;
        }

        private bool CanAssignPick(Dictionary<string, ViewModel.DraftPick> draftPicks, HashSet<ViewModel.DraftPick> uniqueValues, HashSet<string> uniquePositions, string key)
        {
            return !draftPicks.ContainsKey(key) || !uniqueValues.Contains(draftPicks[key]) || !uniquePositions.Contains(key);
        }

        private void AssignEmptyPicksToRoster(Dictionary<string, ViewModel.DraftPick> draftPicks, int vFanTeamID)
        {
            foreach (var key in _rosterDict.Keys)
            {
                if (!draftPicks.ContainsKey(key) || draftPicks[key].teamID != vFanTeamID)
                {
                    draftPicks[key] = new ViewModel.DraftPick();
                }
            }
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
        public MyDraftAPI_v2.FantasyDataModel.MyDraftPick onTheClockDraftPick()
        {
            return draftPickForOverall(_draftStatus.CurrentPick);
        }
        public async Task<Boolean> saveDraftPick(MyDraftAPI_v2.FantasyDataModel.MyDraftPick draftPick)
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
        public MyDraftAPI_v2.FantasyDataModel.MyDraftPick? nextAvailableDraftPickAfterOverall(int overall)
        {
            int totalPicks = _draftPicks.Count;
            if (totalPicks <= overall)
                return null;


            // Iterate over all draft picks after the overall pick. Stop when an empty spot (playerID == nil) is found.
            int startIndex = overall >= 0 ? overall : 0; // Start at 0 or greater
            for (int i = startIndex; i < totalPicks; i++)
            {
                MyDraftAPI_v2.FantasyDataModel.MyDraftPick draftPick = _draftPicks[i];
                if (draftPick.player == null)
                    return draftPick;
            }
            return null;
        }
        public MyDraftAPI_v2.FantasyDataModel.MyDraftPick draftPickForOverall(int overall)
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

            MyDraftAPI_v2.FantasyDataModel.MyDraftPick otcPick = draftPickForOverall(overall);
            if (otcPick != null)
            {
                _draftStatus.CurrentPick = (int)otcPick.overallPick;
                saveDraftStatus(_draftStatus);
            }
        }
        public async Task changeDraftPickToTeam(int overall, FantasyTeam team)
        {
            MyDraftAPI_v2.FantasyDataModel.MyDraftPick draftPick = draftPickForOverall(overall);
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
        public void updateOnTheClock(MyDraftAPI_v2.FantasyDataModel.MyDraftPick vOtcPick)
        {
            MyDraftAPI_v2.FantasyDataModel.MyDraftPick otcPick = onTheClockDraftPick();
            if (otcPick != null && otcPick.playerID == 0)
                return;

            int otcOverall = otcPick != null ? (int)otcPick.overallPick : 0;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            MyDraftAPI_v2.FantasyDataModel.MyDraftPick nextOTC = nextAvailableDraftPickAfterOverall(otcOverall);
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
                    _draftStatus = new ViewModel.DraftStatus(_activeMyDraftLeague.ID, _draftStatus.CurrentPick, false);
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
            _draftStatus = new ViewModel.DraftStatus(_activeMyDraftLeague.ID, _league.rounds * _league.numTeams + 1, true); // Set on the clock to 1 pick beyond the end of the draft
            saveDraftStatus(_draftStatus);
        }
        public void saveDraftStatus(ViewModel.DraftStatus vDraftStatus, MyDraftAPI_v2.FantasyDataModel.MyDraftPick vOtcPick = null)
        {
            using (var db = new AppDataContext(_dbOptionsBuilder.Options))
            {
                var draftStatus = db.UserDraftStatus
                    .Where(x => x.LeagueID == vDraftStatus.LeagueID)
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

        #region //  Draft Events  //
        public void executeDraftPick(int overall, int playerID)
        {
            MyDraftAPI_v2.FantasyDataModel.MyDraftPick draftPick = draftPickForOverall(overall);
            if (draftPick == null)
                return;
            try
            {
                MyDraftAPI_v2.FantasyDataModel.MyDraftPickMemento draftPickMemento = draftPick.getState();
                MyDraftAPI_v2.FantasyDataModel.MyDraftMemento draftMemento = new MyDraftAPI_v2.FantasyDataModel.MyDraftMemento();

                ////TODO:  NOT SURE IF THIS IS NEEDED
                //draftMemento.leagueID = _league.identifier;
                //draftMemento.onTheClock = _draftStatus.onTheClock;
                //draftMemento.MyDraftPickMemento = draftPickMemento;
                ////await DraftManager.pushDraftMementoToUndoStack(draftMemento);
                //await DraftManager.clearDraftMementoRedoStack(_league.identifier);

                draftPick.playerID = playerID;
                saveDraftPick(draftPick);

                MyDraftAPI_v2.FantasyDataModel.MyDraftPick otcPick = onTheClockDraftPick();
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
