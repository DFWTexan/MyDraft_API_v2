namespace Database.Model
{
    public class vw_ProTeamNewsItem
    {
        public int ProTeamID { get; set; }
        public string? PlayerName { get; set; }
        public DateTime PubDate { get; set; }
        public string? Title { get; set; }
        public string? NewsDescription { get; set; }
        public int TeamID { get; set; }
        public int LeagueID { get; set; }
    }
}
