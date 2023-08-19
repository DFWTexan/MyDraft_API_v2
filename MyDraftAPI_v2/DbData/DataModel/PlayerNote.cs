using System.ComponentModel.DataAnnotations.Schema;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class PlayerNote
    {
        [Column("player_id")]
        public String playerID { get; set; }
        [Column("league_id")]
        public String LeagueID { get; set; }
        [Column("note")]
        public String note { get; set; }
        [Column("modified")]
        public String modified { get; set; }
        [Column("created")]
        public String created { get; set; }
    }
}
