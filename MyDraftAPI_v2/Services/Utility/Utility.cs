using DbData;

namespace UtilityService
{
    public class Utility
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;

        public Utility(AppDataContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        #region // Api Responses //
        public DataModel.Response.ReturnResult ErrorReturnResult(int vStatus_ID, string vError)
        {
            return new DataModel.Response.ReturnResult() { StatusCode = vStatus_ID, ObjData = new List<string>() { vError } };
        }
        public DataModel.Response.ReturnResult ErrorReturnResult(int vStatus_ID, List<string> vError)
        {
            return new DataModel.Response.ReturnResult() { StatusCode = vStatus_ID, ObjData = vError };
        }
        public DataModel.Response.ReturnResult SuccessResult(object vObject)
        {
            return new DataModel.Response.ReturnResult() { StatusCode = 200, ObjData = vObject };
        }
        public DataModel.Response.ReturnResult ForbiddenResult()
        {
            return new DataModel.Response.ReturnResult() { StatusCode = 403, ObjData = new List<string>() { "Forbidden!" } };
        }
        public JWTAuthentication.NET6._0.Auth.Response Ok(object vObject)
        {
            return new JWTAuthentication.NET6._0.Auth.Response() {  };
            //return (JWTAuthentication.NET6._0.Auth.Response)((DataModel.Response.ReturnResult)vObject).ObjData;
        }
        public JWTAuthentication.NET6._0.Auth.Response Unauthorized()
        {
            return new JWTAuthentication.NET6._0.Auth.Response() {  Status = "FAILED", Message = "Username or password is incorrect..." };
        }
        public JWTAuthentication.NET6._0.Auth.Response BadRequest(string message)
        {
            return new JWTAuthentication.NET6._0.Auth.Response() { Status = "FAILED", Message = message };
        }
        public DataModel.Response.ReturnResult BadRequestResult(string vMessage)
        {
            return new DataModel.Response.ReturnResult() { StatusCode = 400, ObjData = new List<string>() { vMessage } };
        }
        public DataModel.Response.ReturnResult NotFoundResult(string vMessage)
        {
            return new DataModel.Response.ReturnResult() { StatusCode = 404, ObjData = new List<string>() { vMessage } };
        }
        public DataModel.Response.ReturnResult MessageResult(int vStatus_ID, string vMessage)
        {
            return new DataModel.Response.ReturnResult() { StatusCode = vStatus_ID, ObjData = new List<string>() { vMessage } };
        }
        public DataModel.Response.ReturnResult ExceptionReturnResult(Exception vException)
        {
            var errors = new List<string>() { vException.Message };

            if (vException.InnerException != null)
                errors.Add(vException.InnerException.Message);

            return new DataModel.Response.ReturnResult() { StatusCode = 500, ObjData = errors };
        }
        #endregion 
    }
}
