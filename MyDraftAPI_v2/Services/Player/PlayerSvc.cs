﻿using Database.Model;
using DbData;
using Microsoft.EntityFrameworkCore;
using DbData.ViewModel;

namespace PlayerService
{
    public class PlayerSvc
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly UtilityService.Utility _utility;
        //private readonly IMapper _mapper;
        //private readonly ILogger _logger;

        public PlayerSvc(AppDataContext db, IConfiguration config, IWebHostEnvironment env, UtilityService.Utility utility)
        {
            _db = db;
            _config = config;
            _env = env;
            _utility = utility;
            //_mapper = mapper;
            //_logger = logger;
        }

        public async Task<DataModel.Response.ReturnResult> GetPlayers()
        {
            var result = new DataModel.Response.ReturnResult();
            try
            {
                var players = await _db.Player.Take(100).ToListAsync();
                result.ObjData = players;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrMessage = ex.Message;
            }
            return result;
        }

        public DataModel.Response.ReturnResult GetPlayerByID(int id)
        {
            var result = new DataModel.Response.ReturnResult();
            try
            {
                var player = _db.Player.Where(x => x.ID == id).SingleOrDefault();
                
                var playerInfo = new PlayerInfo
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
                    Status = player.Status
                };  
                
                var position = _db.Positions.Where(x => x.Abbr == player.Position).SingleOrDefault();
                var playerNews = _db.PlayerNews.Where(x => x.PlayerID == id).ToList();
                var depthChart = _db.DepthChart
                    .Include(x => x.Player)
                    .Where(x => x.PositionID == position.ID && x.TeamID == player.ProTeamID).ToList();

                foreach(var i in depthChart.OrderBy(s => s.Rank))
                {
                    if (position != null && i.PositionID == position.ID)
                        playerInfo.DepthChart.Add(new PlayerInfo.DepthChartItem 
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
                        playerInfo.PlayerNews.Add(new PlayerInfo.PlayerNewsItem
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