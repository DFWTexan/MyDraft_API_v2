using System.ComponentModel.DataAnnotations;

namespace Database.Model
{
    public partial class UserDraftSelections
    {
        [Required]
        public int? LeagueID { get; set; }
        [Required]
        public int? PlayerID { get; set; }
        [Required]
        public int? TeamID { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? IsKeeper { get; set; }
        public int? Round { get; set; }
        public int? Pick { get; set; }
        public int? OverallPick { get; set; }
        public int? PositionPick { get; set; }
        public int? PositionRound { get; set; }
    }
}
