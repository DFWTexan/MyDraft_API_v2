﻿using DbData;
using Microsoft.Extensions.Configuration;
using MyDraftAPI_v2;
using MyDraftAPI_v2.Services.Utility;

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
            _utility = new UtilityService.Utility(_db, _config);
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
