using Database.Model;

namespace ViewModel
{
    public class ProTeamScheduleItem
    {
        public int Week { get; set; }
        public string? Designation { get; set; }
        public string? HomeTeamName { get; set; }
        public string? AwayTeamName { get; set; }
        public string? DateString { get; set; }
    }
}
