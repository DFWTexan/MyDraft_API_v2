namespace ViewModel
{
    public class UserInfo
    {
        public string? UserName {  get; set; }
        public string? UserEmail { get; set; }
        public string? Password { get; set; }
        public bool? IsLoggedIn { get; set; }

        public UserInfo() { }
        public UserInfo(string userName, string userEmail, string? password = null) 
        {
            UserName = userName;
            UserEmail = userEmail;
            Password = password;
        }
    }
}
