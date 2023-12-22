namespace Email.Temaplate
{
    public class ResetPasswordHTML
    {
        public static string GetHTML(string vUserName, string vCode)
        {
            string html = @"
                <html>
                    <head>
                        <title>MyDraft Password Reset</title>
                    </head>
                    <body>
                        <p>Hi " + vUserName + @",</p>
                        <p>Click the link below to reset your password.</p>
                        <p><a href='https://red-sand-08dbc5010.4.azurestaticapps.net/" + vCode + @"'>Reset Password</a></p>
                        <p>Thanks,</p>
                        <p>Erish Faggett</p>
                    </body>
                </html>
            ";
            return html;
        }
    }
}
