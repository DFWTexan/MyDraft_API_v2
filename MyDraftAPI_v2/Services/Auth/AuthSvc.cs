using DbData;
using Microsoft.AspNetCore.Identity;
using MyDraftAPI_v2;

namespace AuthService
{
    public class AuthSvc
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        private readonly AppDataContext _db;
        private DraftEngine_v2 _draftEngine;
        private UtilityService.Utility _utility;

        public AuthSvc(
                       UserManager<IdentityUser> userManager,
                                  RoleManager<IdentityRole> roleManager,
                                             IConfiguration configuration,
                                                        AppDataContext db,
                                                                   DraftEngine_v2 draftEngine)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _db = db;
            _draftEngine = draftEngine;
            _utility = new UtilityService.Utility(_db, _configuration);
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
