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

        public DataModel.Response.ReturnResult Login(ViewModel.UserInfo vInput)
        {
            var result = new DataModel.Response.ReturnResult();

            try
            {
                var userInfo = new ViewModel.UserInfo() { UserName = "EMFTest_User", IsLoggedIn = true };

                result.StatusCode = 200;
                result.ObjData = userInfo;
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
