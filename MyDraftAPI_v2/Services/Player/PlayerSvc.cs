using Database.Model;
using DbData;
using Microsoft.EntityFrameworkCore;
using MyDraftAPI_v2;
using MyDraftAPI_v2.Services.Utility;
using ViewModel;
using static ViewModel.PlayerInfo;

namespace PlayerService
{
    public class PlayerSvc
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly UtilityService.Utility _utility;
        private DraftEngine_v2 _draftEngine;
        //private readonly IMapper _mapper;
        //private readonly ILogger _logger;

        public PlayerSvc(AppDataContext db, IConfiguration config, IWebHostEnvironment env, UtilityService.Utility utility, DraftEngine_v2 draftEngine)
        {
            _db = db;
            _config = config;
            _env = env;
            _utility = new UtilityService.Utility(_db, _config);
            //_mapper = mapper;
            //_logger = logger;
            _draftEngine = draftEngine;
        }

        public DataModel.Response.ReturnResult GetPlayers(ViewModel.FilterSortPlayer vInput)
        {
            var result = new DataModel.Response.ReturnResult();
            try
            {
                var players = _db.vw_PlayerListItem
                .Select(plyr => new ViewModel.PlayerListItem
                {
                    PlayerID = plyr.PlayerID,
                    FirstName = plyr.FirstName,
                    LastName = plyr.LastName,
                    FullName = plyr.FullName,
                    PhotoURL = plyr.PhotoURL,
                    Position = plyr.Position,
                    TeamAbbr = plyr.TeamAbbr,
                    PointsVal = plyr.PointsVal,
                    AAVPoints = plyr.AAVPoints,
                    ADPPoints = plyr.ADPPoints,
                    IsDrafted = _db.UserDraftSelection
                                .Any(ud => ud.LeagueID == _draftEngine.ActiveMyDraftLeague.ID && ud.PlayerID == plyr.PlayerID)
                });

                // FilterSORT: Point, AAV or ADP Value
                if (vInput.pointValue != null)
                {
                    switch (vInput.pointValue)
                    {
                        case "[POINTS]":
                            players = players.OrderByDescending(q => q.PointsVal);
                            break;
                        case "[ADP]":
                            players = players.OrderByDescending(q => q.ADPPoints);
                            break;
                        case "[AAV]":
                            players = players.OrderByDescending(q => q.AAVPoints);
                            break;
                        case "[DVDB]":
                            players = players.OrderByDescending(q => q.DVDBVal);
                            break;
                        default:
                            players = players.OrderByDescending(q => q.PointsVal);
                            break;
                    }
                }

                // Filter: Position Value
                if (vInput.positionValue != null)
                {
                    players = players.Where(q => q.Position == vInput.positionValue);
                }

                // Filter: Draft Status Value
                if (vInput.draftStatus != null)
                {
                    if (vInput.draftStatus == "[DRAFTED]")
                    {
                        players = players.Where(q => q.IsDrafted == true);
                    }
                    else if (vInput.draftStatus == "[AVAILABLE]")
                    {
                        players = players.Where(q => q.IsDrafted == false);
                    }
                }

                // TBD: SET the isDrafted value


                result.ObjData = players;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.StatusCode = 500;
                result.ErrMessage = ex.Message;
            }
            return result;
        }

        private bool isPlayerDrafted(int vPlayerID)
        {
            bool isDrafted = _db.UserDraftSelection
                            .AsNoTracking()
                            .Any(x => x.PlayerID == vPlayerID && x.LeagueID == _draftEngine.ActiveMyDraftLeague.ID);

            return isDrafted;
        }
        public DataModel.Response.ReturnResult GetPlayerByID(int id)
        {
            int limitTake = 1;

            var result = new DataModel.Response.ReturnResult();
            try
            {
                var player = _db.Player
                    .Include(x => x.ProTeam)
                    .Where(x => x.ID == id)
                    .AsNoTracking()
                    .SingleOrDefault();

                switch (player.Position)
                {
                    case "QB":
                        limitTake = 2;
                        break;
                    case "RB":
                        limitTake = 3;
                        break;
                    case "WR":
                        limitTake = 4;
                        break;
                    case "TE":
                        limitTake = 2;
                        break;
                    case "K":
                        limitTake = 1;
                        break;
                    case "DEF":
                        break;
                    default:
                        limitTake = 1;
                        break;
                }

                var playerInfo = new ViewModel.PlayerInfo
                {
                    ID = player.ID,
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    TeamAbbr = player.TeamAbbr,
                    ProTeamID = player.ProTeamID,
                    BirthDate = player.BirthDate,
                    Experience = player.Experience,
                    Position = player.Position,
                    PositionGroup = player.PositionGroup,
                    Weight = player.Weight,
                    Height = player.Height,
                    College = player.College,
                    IsRookie = player.IsRookie,
                    PhotoUrl = player.PhotoUrl,
                    Status = player.Status,
                    IsDrafted = isPlayerDrafted(player.ID),
                    ProTeamName = player.ProTeam != null ? player.ProTeam.City + " " + player.ProTeam.NickName : "N/A",
                    ProTeamNickname = player.ProTeam != null ? player.ProTeam.NickName : "N/A",
                };

                var position = _db.Positions.Where(x => x.Abbr == player.Position).SingleOrDefault();
                var playerNews = _db.PlayerNews.Where(x => x.PlayerID == id).ToList();
                bool isPlayerNews = _db.PlayerNews.Any(x => x.PlayerID == id);   
                
                var depthChart = _db.DepthChart
                    .Include(x => x.Player)
                    .Where(x => x.PositionID == position.ID && x.TeamID == player.ProTeamID)
                    .AsNoTracking()
                    .Take(limitTake)
                    .OrderBy(x => x.Rank)
                    .ToList();

                foreach (var i in depthChart.OrderBy(s => s.Rank))
                {
                    if (position != null && i.PositionID == position.ID)
                        playerInfo.DepthChart.Add(new ViewModel.PlayerInfo.DepthChartItem
                        {
                            PlayerName = (i.Player.FirstName == null ? "" : i.Player.FirstName) + ' ' + i.Player.LastName,
                            PositionName = player.Position,
                            Rank = i.Rank
                        });
                }
                if (isPlayerNews)
                {
                    foreach (var i in playerNews)
                    {
                        playerInfo.PlayerNews.Add(new ViewModel.PlayerInfo.PlayerNewsItem
                        {
                            Title = i.Title,
                            PubDate = i.PubDate,
                            NewsDescription = i.NewsDescription,
                            //Reccomendation = i.Reccomendation,
                            ImageURL = i.ImageURL,
                            Analysis = i.Analysis,
                            InjuryType = i.InjuryType
                        });
                    }
                }

                Dictionary<int, PlayerScheduleItem> mapSchedule = new Dictionary<int, PlayerScheduleItem>();

                for (int i = 1; i <= 16; i++)
                {
                    mapSchedule.Add(i, new PlayerScheduleItem());
                }

                var Schedule = _db.Schedule
                    .Where(x => x.HomeTeamID == player.ProTeamID || x.AwayTeamID == player.ProTeamID)
                    .Select(x => new ViewModel.PlayerInfo.PlayerScheduleItem
                    {
                        Week = (int)x.Week,
                        Designation = x.HomeTeamID == player.ProTeamID ? "VS" : "@",
                        HomeTeamName = x.HomeTeamID == player.ProTeamID ? null : _db.ProTeam.Where(q => q.ID == x.HomeTeamID).FirstOrDefault().NickName,
                        AwayTeamName = x.AwayTeamID == player.ProTeamID ? null : _db.ProTeam.Where(q => q.ID == x.AwayTeamID).FirstOrDefault().NickName,
                    })
                    .OrderBy(x => x.Week)
                    .AsNoTracking()
                    .ToList();

                foreach (var item in Schedule)
                {
                    mapSchedule[item.Week] = item;
                }

                playerInfo.PlayerSchedule = mapSchedule.Values.ToList();

                result.ObjData = playerInfo;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrMessage = ex.Message;
            }
            return result;
        }

        #region // Method Template //
        /// <summary>
        ///              TEMPLATE FOR NEW METHODS
        /// </summary>
        /// <param name="vVariable"></param>
        /// <returns></returns>
        public DataModel.Response.ReturnResult TemplateMethod(int vVariable)
        {
            var result = new DataModel.Response.ReturnResult();
            try
            {
                // Code Here

                return _utility.SuccessResult(new { EMFTest = new { Success = true } });
            }
            catch (Exception ex)
            {
                return _utility.ExceptionReturnResult(ex);
            }
        }
        #endregion
    }
}
