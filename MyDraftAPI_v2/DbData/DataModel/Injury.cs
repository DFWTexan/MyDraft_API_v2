using System.ComponentModel.DataAnnotations.Schema;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class Injury
    {
        [Column("player_id")]
        public String playerID { get; set; }
        [Column("injury_status")]
        public String status { get; set; }
        [Column("injury_type")]
        public String type { get; set; }
    }
}
