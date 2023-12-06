using DbData;
using Microsoft.EntityFrameworkCore;
using MyDraftAPI_v2;
using ViewModel;

namespace ProTeamService
{
    public class ProTeamSvc
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly UtilityService.Utility _utility;
        private DraftEngine_v2 _draftEngine;
        //private readonly IMapper _mapper;
        //private readonly ILogger _logger;

        public ProTeamSvc(AppDataContext db, IConfiguration config, IWebHostEnvironment env, UtilityService.Utility utility, DraftEngine_v2 draftEngine)
        {
            _db = db;
            _config = config;
            _env = env;
            _utility = new UtilityService.Utility(_db, _config);
            //_mapper = mapper;
            //_logger = logger;
            _draftEngine = draftEngine;
        }

        public DataModel.Response.ReturnResult News(int vProTeamID)
        {
            var result = new DataModel.Response.ReturnResult();
            try
            {
                var news = _db.vw_ProTeamNewsItem.Where(plyr => plyr.ProTeamID == vProTeamID)
                    .Select(plyr => new ViewModel.ProTeamNewsItem
                    {
                        ProTeamID = plyr.ProTeamID,
                        DateString = plyr.PubDate.ToString("dddd, dd MMMM yyyy"),
                        Title = plyr.Title,
                        NewsDescription = plyr.NewsDescription
                    });

                return _utility.SuccessResult(news);
            }
            catch (Exception ex)
            {
                return _utility.ExceptionReturnResult(ex);
            }
        }
        public DataModel.Response.ReturnResult Schedule(int vProTeamID)
        {
            var result = new DataModel.Response.ReturnResult();
            try
            {
                Dictionary<int, ProTeamScheduleItem> mapSchedule = new Dictionary<int, ProTeamScheduleItem>();

                for (int i = 1; i <= 16; i++)
                {
                    mapSchedule.Add(i, new ProTeamScheduleItem());
                }

                var Schedule = _db.Schedule.Where(i => i.HomeTeamID == vProTeamID || i.AwayTeamID == vProTeamID)
                    .Select(team => new ViewModel.ProTeamScheduleItem
                    {
                        Week = (int)team.Week,
                        Designation = team.HomeTeamID == vProTeamID ? "VS" : "@",
                        HomeTeamName = team.HomeTeamID == vProTeamID ? null : _db.ProTeam.Where(q => q.ID == team.HomeTeamID).FirstOrDefault().NickName,
                        AwayTeamName = team.AwayTeamID == vProTeamID ? null : _db.ProTeam.Where(q => q.ID == team.AwayTeamID).FirstOrDefault().NickName,
                    })
                    .OrderBy(i => i.Week)
                    .ToList();

                foreach (var item in Schedule)
                {
                    mapSchedule[item.Week] = item;
                }

                return _utility.SuccessResult(mapSchedule.ToArray());
            }
            catch (Exception ex)
            {
                return _utility.ExceptionReturnResult(ex);
            }
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
