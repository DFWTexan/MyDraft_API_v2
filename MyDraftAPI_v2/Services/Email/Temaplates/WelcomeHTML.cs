namespace Email.Temaplate
{
    public class WelcomeHTML
    {
        public static string GetHTML(string userName)
        {
            // return welcomeHTML.Replace("{userName}", userName);
            return $@"
                 <html>
                      <head>
                            <style>
                             .button {{
                                  background-color: #4CAF50; /* Green */
                                  border: none;
                                  color: white;
                                  padding: 15px 32px;
                                  text-align: center;
                                  text-decoration: none;
                                  display: inline-block;
                                  font-size: 16px;
                                  margin: 4px 2px;
                                  cursor: pointer;
                             }}
                            </style>
                      </head>
                      <body>
                            <h1>MyDraft</h1>
                            <p>Welcome {userName}!</p>
                            <p>Thank you,</p>
                            <p>Erish Faggett</p>
                      </body>
                 </html>
             ";
        }
    }
}
