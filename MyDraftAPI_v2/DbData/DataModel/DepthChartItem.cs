using System.ComponentModel.DataAnnotations.Schema;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class DepthChartItem
    {
        [Column("player_id")]
        public int playerID { get; set; }
        [Column("position")]
        public String position { get; set; }
        [Column("rank")]
        public String rank { get; set; }
        [Column("team_abbr")]
        public String teamAbbr { get; set; }
        public int sortValue { get; set; }
        public IList<DepthChartStats> stats { get; set; }
    }
}
