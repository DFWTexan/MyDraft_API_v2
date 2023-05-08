namespace DataModel.Response
{
    public class ReturnResult
    {
        public bool Success { get; set; }
        public string StatusCode { get; set; }
        public object Data { get; set; }
        public string ErrMessage { get; set; }
        public ReturnResult(bool success, string statusCode, object data, string errMessage)
        {
            Success = false;
            StatusCode = statusCode;
            Data = data;
            ErrMessage = errMessage;
        }

    }
}
