using Database.Model;
using DbData;
using Microsoft.EntityFrameworkCore;
using ViewModel;
using MyDraftAPI_v2.FantasyDataModel;
using System.Text;
using MyDraftAPI_v2.FantasyDataModel.Draft;
using System.Diagnostics;
using MyDraftAPI_v2.Services.Algorithms;
using MyDraftAPI_v2;
using MyDraftLib.Utilities;
using Windows.UI.Xaml;
#pragma warning disable

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

        private DraftEngine_v2 _draftEngine;

        public DraftSvc(AppDataContext db, IConfiguration config, IWebHostEnvironment env, UtilityService.Utility utility, DraftEngine_v2 draftEngine)
        {
            _db = db;
            _config = config;
            _env = env;
            _utility = utility;
            //_mapper = mapper;
            //_logger = logger;
            _draftEngine = draftEngine;
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
            List<ViewModel.DraftPick> returnResult = new List<ViewModel.DraftPick>();

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

            foreach (var draftPick in draftPicks)
            {
                if (draftPick.playerID != 0)
                {
                    var playerInfo = _db.Player.Where(q => q.ID == draftPick.playerID).FirstOrDefault();
                    if (playerInfo != null)
                        draftPick.player = playerInfo;
                }
                returnResult.Add(draftPick);
            }

            _draftEngine.draftPicks = returnResult.ToList();

            return returnResult;
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
                        TeamID = (int)i.teamID,
                        PlayerID = (int)i.playerID,
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

                foreach (var i in vInput.teams)
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
        public DataModel.Response.ReturnResult GetDraftPicksForLeague_v2()
        {
            var result = new DataModel.Response.ReturnResult();

            try
            {
                FantasyLeague fanLeague = new FantasyLeague()
                {
                    identifier = _draftEngine.ActiveMyDraftLeague.ID,
                    numTeams = _draftEngine.ActiveMyDraftLeague.NumberOfTeams,
                    rounds = _draftEngine.ActiveMyDraftLeague.NumberOfRounds,
                    draftByTeamEnabled = true,
                };

                foreach (var i in _draftEngine.ActiveMyDraftLeague.teams)
                {
                    var addItem = new FantasyTeam()
                    {
                        identifier = i.ID,
                        name = i.Name ?? "",
                        abbr = i.Abbreviation ?? "",
                    };
                    fanLeague.fanTeams.Add(addItem);
                }

                var draftPicks = draftPicksForLeague(_draftEngine.ActiveMyDraftLeague.ID);

                int totalPicks = _draftEngine.ActiveMyDraftLeague.NumberOfTeams * _draftEngine.ActiveMyDraftLeague.NumberOfRounds;
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
        public DataModel.Response.ReturnResult GetDraftPicksByFanTeam(int vFanTeamID)
        {
            var result = new DataModel.Response.ReturnResult();
            List<FanTeamPick> fanTeamPicks = new List<FanTeamPick>();

            int cnt_QB = 0;
            int cnt_RB = 0;
            int cnt_WR = 0;
            int cnt_TE = 0;
            int cnt_K = 0;
            int cnt_DEF = 0;
            int cnt_BN = 0;

            try
            {
                result.StatusCode = 200;
                var resData = _draftEngine.draftPicksForTeam_v2(vFanTeamID);

                foreach (var item in resData)
                {
                    FanTeamPick pick = new FanTeamPick();
                    switch (item.Key.Substring(0, 2))
                    {
                        case "QB":
                            pick.@int = ++cnt_QB;
                            pick.PositionGroup = "QUARTERBACK";
                            pick.SortOrder = 1;
                            break;
                        case "RB":
                            pick.@int = ++cnt_RB;
                            pick.PositionGroup = "RUNNING BACK";
                            pick.SortOrder = 2;
                            break;
                        case "WR":
                            pick.@int = ++cnt_WR;
                            pick.PositionGroup = "WIDE RECIEVER";
                            pick.SortOrder = 3;
                            break;
                        case "TE":
                            pick.@int = ++cnt_TE;
                            pick.PositionGroup = "TIGHT END";
                            pick.SortOrder = 4;
                            break;
                        case "K1":
                            pick.@int = ++cnt_K;
                            pick.PositionGroup = "KICKER";
                            pick.SortOrder = 5;
                            break;
                        case "D1":
                            pick.@int = ++cnt_DEF;
                            pick.PositionGroup = "DEFENSE";
                            pick.SortOrder = 6;
                            break;
                        default:  // "BENCH" is the defaut
                            pick.@int = ++cnt_BN;
                            pick.PositionGroup = "BENCH";
                            pick.SortOrder = 7;
                            break;
                    }

                    pick.PlayerName = item.Value.player != null ? item.Value.player.FirstName + ' ' + item.Value.player.LastName : "";
                    fanTeamPicks.Add(pick);
                }

                result.ObjData = fanTeamPicks.OrderBy(o => o.SortOrder).ToList();
            }
            catch (Exception ex)
            {
                result.StatusCode = 500;
                result.ErrMessage = ex.Message;
            }

            return result;
        }
        public DataModel.Response.ReturnResult GetDraftPicksByPosition()
        {
            var result = new DataModel.Response.ReturnResult();
            string[] positionGroups = { "QB", "RB", "WR", "TE", "K1", "D1" };
            List<ViewModel.DraftedByPositionItem> draftedPositions = new List<DraftedByPositionItem>();

            List<ViewModel.DraftPick> drafted_QB = new List<ViewModel.DraftPick>();
            List<ViewModel.DraftPick> drafted_RB = new List<ViewModel.DraftPick>();
            List<ViewModel.DraftPick> drafted_WR = new List<ViewModel.DraftPick>();
            List<ViewModel.DraftPick> drafted_TE = new List<ViewModel.DraftPick>();
            List<ViewModel.DraftPick> drafted_K = new List<ViewModel.DraftPick>();
            List<ViewModel.DraftPick> drafted_D = new List<ViewModel.DraftPick>();

            try
            {
                result.StatusCode = 200;
                var draftPicks = draftPicksForLeague(_draftEngine.ActiveMyDraftLeague.ID);

                foreach (var item in draftPicks)
                {
                    FanTeamPick pick = new FanTeamPick();
                    if (item.player != null)
                    {
                        switch (item.player.Position)
                        {
                            case "QB":
                                drafted_QB.Add(item);
                                break;
                            case "RB":
                                drafted_RB.Add(item);
                                break;
                            case "WR":
                                drafted_WR.Add(item);
                                break;
                            case "TE":
                                drafted_TE.Add(item);
                                break;
                            case "K":
                                drafted_K.Add(item);
                                break;
                        }
                    }
                }

                Dictionary<string, List<ViewModel.DraftPick>> dictDraftedPlayerByPositions = new Dictionary<string, List<ViewModel.DraftPick>>() {
                    {"QB",drafted_QB },
                    {"RB",drafted_RB },
                    {"WR",drafted_WR },
                    {"TE",drafted_TE },
                    {"K1",drafted_K },
                    {"D1",drafted_D },
                };

                foreach (string posGroup in positionGroups)
                {
                    int round = 0;
                    var draftedPosItem = new ViewModel.DraftedByPositionItem()
                    {
                        PositionGroup = posGroup == "K1" ? "K" : posGroup == "D1" ? "DEF" : posGroup,
                        Count = dictDraftedPlayerByPositions[posGroup].Count,
                        RoundPicks = new Dictionary<int, Dictionary<int, List<DraftPositionPick>>>()
                    };

                    Dictionary<int, List<ViewModel.DraftPositionPick>> dict_PositionGroup = new Dictionary<int, List<DraftPositionPick>>();
                    for (int i = 1; i <= _draftEngine.ActiveMyDraftLeague.NumberOfRounds; i++)
                    {
                        dict_PositionGroup.Add(i, new List<ViewModel.DraftPositionPick>());
                    }

                    foreach (ViewModel.DraftPick item in dictDraftedPlayerByPositions[posGroup])
                    {
                        if (item.player.Position == posGroup)
                        {
                            dict_PositionGroup[(int)item.round].Add(new ViewModel.DraftPositionPick()
                            {
                                PositionGroup = item.player.Position,
                                PlayerName = item.player.FirstName + " " + item.player.LastName,
                                Round = (int)item.round,
                                PickInRound = (int)item.pickInRound,
                            });
                        }
                    }
                    draftedPosItem.RoundPicks.Add(round++, dict_PositionGroup);
                    draftedPositions.Add(draftedPosItem);
                }

                result.ObjData = draftedPositions.ToList();
            }
            catch (Exception ex)
            {
                result.StatusCode = 500;
                result.ErrMessage = ex.Message;
            }

            return result;
        }
        public DataModel.Response.ReturnResult GetRosterTotalPositionCount()
        {
            var result = new DataModel.Response.ReturnResult();

            try
            {
                var fanTeams = _draftEngine.fantasyTeams;

                Dictionary<string, Dictionary<string, int>> dictMasterFanTeams = new Dictionary<string, Dictionary<string, int>>();
                foreach (var team in fanTeams)
                {
                    var playerData = _draftEngine.draftPicksForTeam(team.ID);
                    if (playerData != null)
                    {
                        var rosterCount = BuildFanTeamPositionData((List<ViewModel.DraftPick>)playerData);
                        dictMasterFanTeams.Add(team.Name, rosterCount);
                    }
                }

                result.ObjData = dictMasterFanTeams.ToList();
                result.StatusCode = 200;
            }
            catch (Exception ex)
            {
                result.StatusCode = 500;
                result.ErrMessage = ex.Message;
            }

            return result;
        }
        private Dictionary<string, int> BuildFanTeamPositionData(List<ViewModel.DraftPick> data)
        {
            string[] positionGroups = { "QB", "RB", "WR", "TE", "K", "DEF" };
            int qb_Count = 0;
            int rb_Count = 0;
            int wr_Count = 0;
            int te_Count = 0;
            int k_Count = 0;
            int def_Count = 0;

            Dictionary<string, int> dictFanTeamPositions = new Dictionary<string, int>();
            foreach (ViewModel.DraftPick item in data)
            {
                if (item.player != null)
                    switch (item.player.Position)
                    {
                        case "QB":
                            qb_Count++;
                            break;
                        case "RB":
                            rb_Count++;
                            break;
                        case "WR":
                            wr_Count++;
                            break;
                        case "TE":
                            te_Count++;
                            break;
                        case "K":
                            k_Count++;
                            break;
                        case "DEF":
                            def_Count++;
                            break;
                    }
            }

            foreach (string pos in positionGroups)
            {
                switch (pos)
                {
                    case "QB":
                        dictFanTeamPositions.Add("QB", qb_Count);
                        break;
                    case "RB":
                        dictFanTeamPositions.Add("RB", rb_Count);
                        break;
                    case "WR":
                        dictFanTeamPositions.Add("WR", wr_Count);
                        break;
                    case "TE":
                        dictFanTeamPositions.Add("TE", te_Count);
                        break;
                    case "K":
                        dictFanTeamPositions.Add("K", k_Count);
                        break;
                    case "DEF":
                        dictFanTeamPositions.Add("DEF", def_Count);
                        break;
                }
            }

            return dictFanTeamPositions;
        }
        //public DataModel.Response.ReturnResult GetTeamRoster(int vFanTeamID)
        //{
        //    var result = new DataModel.Response.ReturnResult();
        //    Dictionary<string, int> dictStarters = new Dictionary<string, int>() {
        //        {"QB", 1 },
        //        {"RB", 2 },
        //        {"WR", 2 },
        //        {"TE", 1 },
        //        {"K1", 1 },
        //    };

        //    List<ViewModel.DraftPick> myTeam_QB = new List<ViewModel.DraftPick>();
        //    List<ViewModel.DraftPick> myTeam_RB = new List<ViewModel.DraftPick>();
        //    List<ViewModel.DraftPick> myTeam_WR = new List<ViewModel.DraftPick>();
        //    List<ViewModel.DraftPick> myTeam_TE = new List<ViewModel.DraftPick>();
        //    List<ViewModel.DraftPick> myTeam_K = new List<ViewModel.DraftPick>();
        //    List<ViewModel.DraftPick> myTeam_BN = new List<ViewModel.DraftPick>();

        //    try
        //    {
        //        result.StatusCode = 200;
        //        var draftPicks = GetDraftPicksByFanTeam(vFanTeamID);

        //    }
        //    catch (Exception ex)
        //    {
        //        result.StatusCode = 500;
        //        result.ErrMessage = ex.Message;
        //    }

        //    return result;
        //}
        public DataModel.Response.ReturnResult GetTeamSelections(int vFanTeamID)
        {
            var result = new DataModel.Response.ReturnResult();
            Dictionary<int, ViewModel.DraftPick> dictTeamSelections = new Dictionary<int, ViewModel.DraftPick>();

            try
            {
                result.StatusCode = 200;
                
                int cnt = 1;
                var teamPicks = _draftEngine.draftPicksForTeam(vFanTeamID).OrderBy(q => q.overallPick);
                foreach (var teamPick in teamPicks)
                {
                    dictTeamSelections.Add(cnt++, teamPick);
                }

                result.ObjData = dictTeamSelections.ToArray();
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
