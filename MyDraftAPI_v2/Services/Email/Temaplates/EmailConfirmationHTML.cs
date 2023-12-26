namespace Email.Temaplate
{
    public class EmailConfirmationHTML
    {
        public static string GetHTML(string vUserName, string vToken, string vEmail)
        {

            //string html = @"
            //    <html>
            //        <head>
            //            <title>MyDraft Email Confirmation</title>
            //        </head>
            //        <body>
            //            <p>Hi " + vUserName + @",</p>
            //            <p>Click the link below to confirm your email.</p>
            //            <p>https://localhost:3000/EmailConfirmed?token="" + vToken + @""'>Confirm Email</a></p>
            //            <p>Thanks,</p>
            //            <p>Erish Faggett</p>
            //        </body>
            //    </html>
            //";

            string html = string.Format(@"<html>
                                             <head>
                                                <title>MyDraft Email Confirmation</title>
                                             </head>
                                             <body>
                                                 <p>Hi {0},</p>
                                                 <p>Click the link below to confirm your email.</p>
                                                 <p><a href=""https://red-sand-08dbc5010.4.azurestaticapps.net/emailverified?token={1}&email={2}"">Confirm Email</a></p>
                                                 <p>Thanks,</p>
                                                 <p>Erish Faggett</p>
                                             </body>
                                          </html>", vUserName, vToken, vEmail);
            return html;
        }
    }
}
