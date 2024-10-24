﻿namespace JWTAuthentication.NET6._0.Auth
{
    public class Response
    {
        
        public string? Status { get; set; }
        public string? Message { get; set; }

        public Response()
        {
        }
        public Response(string status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}