namespace MyDraftAPI_v2.Middleware
{
    public static class CustomMiddleware
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder, IConfiguration config)
        {
            return builder.UseMiddleware<RequestMiddleware>(config);
        }

    }
}
