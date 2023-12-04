namespace Email.Temaplate
{
    public class ResetCodeHTML
    {
        public static string GetHTML(string vCode)
        {
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
                        <p>Reset Code: {vCode}</p>
                        <p>Use this code to reset your password.</p>
                        <p>Thank you,</p>
                        <p>Erish Faggett</p>
                    </body>
                </html>
            ";
               
        }
    }
}
