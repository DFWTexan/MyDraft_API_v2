using System.ComponentModel.DataAnnotations.Schema;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class LineupItem
    {
        [Column("player_id")]
        public string player_id { get; set; }
        [Column("position")]
        public string position { get; set; }
        [Column("value")]
        public int starters { get; set; }
        [Column("sort_value")]
        public int sortValue { get; set; }
    }
}
