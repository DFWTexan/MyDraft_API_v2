namespace Email.Temaplate
{
    public class EmailConfirmationHTML
    {
        public static string GetHTML(string vUserName, string vToken)
        {
            string html = @"
                <html>
                    <head>
                        <title>MyDraft Email Confirmation</title>
                    </head>
                    <body>
                        <p>Hi " + vUserName + @",</p>
                        <p>Click the link below to confirm your email.</p>
                        <p><a href='https://mydraft.net/confirmemail/" + vToken + @"'>Confirm Email</a></p>
                        <p>Thanks,</p>
                        <p>MyDraft Team</p>
                    </body>
                </html>
            ";
            return html;
        }
    }
}
