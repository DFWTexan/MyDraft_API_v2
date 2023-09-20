using Database.Model;

namespace ViewModel
{
    public class ActiveLeague : UserLeague
    {
        public List<UserLeageTeamItem> teams { get; set; }
        public ActiveLeague()
        {
            teams = new List<UserLeageTeamItem>();
        }

    }

    public class UserLeageTeamItem
    {
        public int ID { get; set; }
        public int LeagueID { get; set; }
        public string? Name { get; set; }
        public string? Abbreviation { get; set; }
        public int DraftPosition { get; set; }
        public string? Owner { get; set; }
    }
}
