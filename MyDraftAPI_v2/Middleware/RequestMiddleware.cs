using Microsoft.Extensions.Primitives;

namespace MyDraftAPI_v2.Middleware
{
    public class RequestMiddleware
    {
        #region Private Properties
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        /// <summary>
        /// Request Milldleware Constructor
        /// </summary>
        /// <param name="next">Next step of a pipeline</param>
        /// <param name="configuration">Application configuration object</param>
        public RequestMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }
        #endregion

        #region Middleware Invoke

        /// <summary>
        /// This method will get called on each API call, before invoking your API.
        /// </summary>
        /// <param name="context">Your request HttpContext</param>
        /// <param name="customerFeatureMap">The CustomerFeatureMap (Singleton) object</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {

            //Customer ID - I am passing in header, in general you should read it from you authnetication object like token/cookie
            // int loggedInUserId = 0;
            if (context?.Request.Headers.TryGetValue("loggedInUserId", out StringValues loggedInUserId) == true)
            {
                int customerId = Convert.ToInt32(loggedInUserId);

                //If customer features dictionary is null then, we will initializes
                //if (customerFeatureMap.EnabledFeatures == null)
                //    customerFeatureMap.EnabledFeatures = new Dictionary<int, List<string>>();

                /*checking if loggedin user id data already present in singleton object
                If it present then we will read it from singleton object only, otherwise we will fetch the feature details for
                logged in user from SQl and put it in singleton object*/
                //if (!customerFeatureMap.EnabledFeatures.ContainsKey(customerId))
                //{
                //    //var features = GetCurrentCustomerFeatures(customerId);
                //    customerFeatureMap.EnabledFeatures.Add(customerId, features);
                //}

                await _next(context);
            }
        }

        #endregion

    }
}
