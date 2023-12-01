using DbData;
using System.Net.Mail;
using System.Net;

namespace EmailService
{
    public class EmailSvs
    {
        private readonly IConfiguration _config;
        
        public EmailSvs(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string vTo, string vToName, string? vSubject, string? vBody)
        {
            // var statement to get port , smtpServer, username, password from appsettings.json
            var emailConfig = _config.GetSection("EmailConfiguration");
            var fromAddress = emailConfig["From"];
            var smtpServer = emailConfig["SmtpServer"];
            var port = Convert.ToInt32(emailConfig["Port"]);

            var username = emailConfig["Username"];
            var password = emailConfig["Password"];

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Host = smtpServer;
                    client.Port = port;
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(username, password);

                    using (var message = new MailMessage(
                                from: new MailAddress(fromAddress, "MyDraft - E.faggett "),
                                to: new MailAddress(vTo, vToName)
                                ))
                    {
                        message.Subject = vSubject;
                        message.Body = vBody;
                        message.IsBodyHtml = true;

                        client.Send(message);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
