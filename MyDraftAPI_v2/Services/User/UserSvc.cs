using DbData;
using MyDraftAPI_v2;

namespace UserService
{
    public class UserSvc
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        
        private readonly UtilityService.Utility _utility;
        private DraftEngine_v2 _draftEngine;

        public UserSvc(AppDataContext db, IConfiguration config, IWebHostEnvironment env, UtilityService.Utility utility, DraftEngine_v2 draftEngine)
        {
            _db = db;
            _config = config;
            _env = env;
            _utility = utility;
            _draftEngine = draftEngine;
        }

        public DataModel.Response.ReturnResult UserInfoStatus()
        {
            try
            {
                return _utility.SuccessResult(_draftEngine.MyDraftUser);
            }
            catch (Exception ex)
            {
                return _utility.ExceptionReturnResult(ex);
            }
        }

        //public DataModel.Response.ReturnResult Login_v2(ViewModel.UserInfo vInput)
        //{
        //    var service = new LeagueService.LeagueSvc(_db, _config, null, null, _draftEngine);

        //    try
        //    {
        //        ///
        //        // TBD: Convert to LOGIN with provided Credentials...
        //        ///
        //        var userInfo = new ViewModel.UserInfo() { UserID = 1, UserName = "EMFTest_User", IsLoggedIn = true };
        //        var activeLeague = service.GetActiveLeague(userInfo.UserID);

        //        _draftEngine.InitializeLeagueData_v2();

        //        return _utility.SuccessResult(userInfo);
        //    }
        //    catch (Exception ex)
        //    {
        //        return _utility.ExceptionReturnResult(ex);
        //    }
        //}
    }
}
