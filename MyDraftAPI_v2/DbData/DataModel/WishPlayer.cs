using System.ComponentModel.DataAnnotations.Schema;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class WishPlayer
    {
        [Column("player_id")]
        public string playerID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string position { get; set; }
        [Column("round")]
        public string pickround { get; set; }
        public string proteam { get; set; }
    }
}
