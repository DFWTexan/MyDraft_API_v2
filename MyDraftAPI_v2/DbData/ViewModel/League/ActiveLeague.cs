using Database.Model;
using System.ComponentModel.DataAnnotations;

namespace ViewModel
{
    public class ActiveLeague
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Abbr { get; set; }
        public int Mode { get; set; }
        public int DraftType { get; set; }
        public string? DraftOrder { get; set; }
        public int NumberOfTeams { get; set; }
        public int NumberOfRounds { get; set; }
        
        public List<UserLeageTeamItem> teams { get; set; }
        public ActiveLeague()
        {
            teams = new List<UserLeageTeamItem>();
        }
    }

    public class UserLeageTeamItem
    {
        public int UniversalID { get; set; }    
        public int ID { get; set; }
        public int LeagueID { get; set; }
        public string? Name { get; set; }
        public string? Abbreviation { get; set; }
        public int DraftPosition { get; set; }
        public string? Owner { get; set; }
    }
}
