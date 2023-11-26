namespace ViewModel
{
    public class UserInfo
    {
        //public int UserID { get; set; }
        public string? UserName {  get; set; }
        public string? UserEmail { get; set; }
        //public string? Password { get; set; }
        public bool? IsLoggedIn { get; set; }

        public UserInfo() { }
        public UserInfo(string userName, string userEmail, bool isLoggedIn) 
        {
            //UserID = universID;
            UserName = userName;
            UserEmail = userEmail;
            IsLoggedIn = isLoggedIn;
        }
    }
}
