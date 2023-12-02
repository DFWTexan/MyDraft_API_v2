using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web.Resource;

namespace MyDraftAPI_v2.Controllers
{
    //[Authorize]
    //[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]    //[Authorize]
    //[Authorize]    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        static readonly string[] scopeRequiredByApi = new string[] { "access_as_user" };
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _config;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
//HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            //using (var client = new SmtpClient())
            //{
            //    client.Host = "smtp.gmail.com";
            //    client.Port = 587;
            //    client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //    client.UseDefaultCredentials = false;
            //    client.EnableSsl = true;
            //    client.Credentials = new NetworkCredential("ErishMF@GMAIL.COM", "drfc cfap rzsx ohsf");
            //    using (var message = new MailMessage(
            //        from: new MailAddress("ErishMF@GMAIL.COM", "TxNum5"),
            //        to: new MailAddress("EMFTest@mailinator.COM", "EMFTest-Email-Message")
            //        ))
            //    {

            //        message.Subject = "Hello from code!";
            //        message.Body = "Loremn ipsum dolor sit amet ...";

            //        client.Send(message);
            //    }
            //}

            var body = string.Format("<div><p> Hello {0}</p><p>Need to Complete email body content.</p></div>", "DFWTexan");

            var service = new EmailService.EmailSvs(_config);
            service.SendEmail("EMFTest@mailinator.com", "EMF-Tester", "Welcome to MyDraft!", body);

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}