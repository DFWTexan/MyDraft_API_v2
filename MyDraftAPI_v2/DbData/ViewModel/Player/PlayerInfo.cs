using Database.Model;
using System.ComponentModel.DataAnnotations;

namespace ViewModel
{
    public class PlayerInfo : Player
    {
        public bool IsPlayerNews { get; set; }
        public bool IsDrafted { get; set; }
        public string? ProTeamName { get; set; }
        public string? ProTeamNickname { get; set; }
        public List<DepthChartItem> DepthChart { get; set; }
        public List<PlayerNewsItem> PlayerNews { get; set; }
        public List<PlayerScheduleItem> PlayerSchedule { get; set; }

        public PlayerInfo()
        {
            DepthChart = new List<DepthChartItem>();
            PlayerNews = new List<PlayerNewsItem>();
            PlayerSchedule = new List<PlayerScheduleItem>();
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

        public class PlayerScheduleItem : ProTeamScheduleItem
        {
        }
    }
}
