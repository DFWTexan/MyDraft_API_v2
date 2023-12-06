using DbData;
using Microsoft.EntityFrameworkCore;
using MyDraftAPI_v2;

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
                        PubDate = plyr.PubDate,
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
