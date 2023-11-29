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
                                to: new MailAddress("EMFTest@mailinator.COM", "")
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
