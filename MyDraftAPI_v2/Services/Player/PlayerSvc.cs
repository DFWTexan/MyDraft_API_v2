using Database.Model;
using DbData;
using Microsoft.EntityFrameworkCore;
using MyDraftAPI_v2;

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
            _utility = utility;
            //_mapper = mapper;
            //_logger = logger;
            _draftEngine = draftEngine;
        }

        public DataModel.Response.ReturnResult GetPlayers(ViewModel.FilterSortPlayer vInput)
        {
            var result = new DataModel.Response.ReturnResult();
            try
            {
                //var players = from q in _db.vw_PlayerListItem.AsSplitQuery() select q;
                var players = _db.vw_PlayerListItem.AsSplitQuery();

                // FilterSORT: Point, AAV or ADP Value
                if (vInput.pointValue != null)
                {
                    if(vInput.pointValue == "[POINTS]")
                        players = players.OrderByDescending(q => q.PointsVal);
                    else if(vInput.pointValue == "[ADP]")
                        players = players.OrderByDescending(q => q.ADPPoints);
                    else if (vInput.pointValue == "[AAV]")
                        players = players.OrderByDescending(q => q.AAVPoints);
                }

                // Filter: Position Value
                if (vInput.positionValue != null)
                {
                    players = players.Where(q => q.Position == vInput.positionValue);
                }

                // Filter: Draft Status Value
                if (vInput.draftStatus != null)
                {
                    if(vInput.draftStatus == "[DRAFTED]")
                    {
                        players = players.Where(q => q.IsDrafted == true);
                    }
                    else if(vInput.draftStatus == "[AVAILABLE]")
                    {
                        players = players.Where(q => q.IsDrafted == false);
                    }
                }

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
                            .Any(x => x.PlayerID == vPlayerID);

            return isDrafted;
        }
        public DataModel.Response.ReturnResult GetPlayerByID(int id)
        {
            var result = new DataModel.Response.ReturnResult();
            try
            {
                var player = _db.Player
                    .Include(x => x.ProTeam)
                    .Where(x => x.ID == id)
                    .SingleOrDefault();
                
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
                    ProTeamName = player.ProTeam.NickName != null ? player.ProTeam.City + " " + player.ProTeam.NickName : "N/A"
                };  
                
                var position = _db.Positions.Where(x => x.Abbr == player.Position).SingleOrDefault();
                var playerNews = _db.PlayerNews.Where(x => x.PlayerID == id).ToList();
                var depthChart = _db.DepthChart
                    .Include(x => x.Player)
                    .Where(x => x.PositionID == position.ID && x.TeamID == player.ProTeamID).ToList();

                foreach(var i in depthChart.OrderBy(s => s.Rank))
                {
                    if (position != null && i.PositionID == position.ID)
                        playerInfo.DepthChart.Add(new ViewModel.PlayerInfo.DepthChartItem 
                        { 
                            PlayerName = (i.Player.FirstName == null ? "" : i.Player.FirstName) + ' ' + i.Player.LastName,
                            PositionName = player.Position,
                            Rank = i.Rank
                        });
                }
                if(playerNews != null)
                {
                    foreach (var i in playerNews)
                    {
                        playerInfo.PlayerNews.Add(new ViewModel.PlayerInfo.PlayerNewsItem
                        {
                            Title = i.Title,
                            PubDate = i.PubDate,
                            NewsDescription = i.NewsDescription,
                            Reccomendation = i.Reccomendation,
                            ImageURL = i.ImageURL,
                            Analysis = i.Analysis,
                            InjuryType = i.InjuryType
                        });
                    }
                }                

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
    }
}
