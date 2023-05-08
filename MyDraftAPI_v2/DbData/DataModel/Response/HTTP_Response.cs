namespace DataModel.Response
{
    public class HTTP_Response
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object ObjData { get; set; }
        public HTTP_Response(int statusCode, string message, object data)
        {
            StatusCode = statusCode;
            Message = message;
            ObjData = data;
        }
    }
}
