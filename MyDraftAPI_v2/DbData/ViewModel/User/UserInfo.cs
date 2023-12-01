namespace ViewModel
{
    public class UserInfo
    {
        public string? UserName {  get; set; }
        public string? UserEmail { get; set; }
        public bool? IsLoggedIn { get; set; }
        public List<UserLeagueItem>? UserLeagues { get; set; }

        public UserInfo() {
            UserLeagues = new List<UserLeagueItem>();
        }
        public UserInfo(string userName, string userEmail, bool isLoggedIn) 
        {
            UserName = userName;
            UserEmail = userEmail;
            IsLoggedIn = isLoggedIn;
            UserLeagues = new List<UserLeagueItem>();
        }
    }

    public class UserLeagueItem
    {
        public int Value { get; set; }
        public string? Label { get; set; }
    }
}
