using Database.Model;
using System.ComponentModel.DataAnnotations;

namespace DbData.ViewModel
{
    public class PlayerInfo
    {
        public int ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? TeamAbbr { get; set; }
        public int? ProTeamID { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? Experience { get; set; }
        public string? Position { get; set; }
        public string? PositionGroup { get; set; }
        public int? Weight { get; set; }
        public string? Height { get; set; }
        public string? College { get; set; }
        public bool? IsRookie { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Status { get; set; }
        public List<DepthChartItem> DepthChart { get; set; }
        public List<PlayerNewsItem> PlayerNews { get; set; }

        public PlayerInfo()
        {
            DepthChart = new List<DepthChartItem>();
            PlayerNews = new List<PlayerNewsItem>();
        }

        public class DepthChartItem
        {
            public int? Rank { get; set; }
            public string? PositionName { get; set; }
            public string? PlayerName { get; set; }
            public string? TeamName { get; set; }
        }

        public class PlayerNewsItem
        {
            public DateTime? PubDate { get; set; }
            public string? Title { get; set; }
            public string? NewsDescription { get; set; }
            public string? Reccomendation { get; set; }
            public string? ImageURL { get; set; }
            public string? Analysis { get; set; }
            public string? InjuryType { get; set; }
        }
    }
}
