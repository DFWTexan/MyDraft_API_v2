namespace Email.Temaplate
{
    public class PasswordWasReset
    {
        public static string GetHTML(string vUserName)
        {
            string html = @"
                <html>
                    <head>
                        <title>MyDraft Password Reset</title>
                    </head>
                    <body>
                        <p>Hi " + vUserName + @",</p>
                        <p>Your password was successfully reset.</p>
                        <p>Thanks,</p>
                        <p>MyDraft Team</p>
                    </body>
                </html>
            ";
            return html;
        }
    }
}
