using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public partial class UserDraftSelection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        //[Required]
        //[MaxLength(450)]
        //public string? UserUniqueID { get; set; }
        //[Required]
        //public int UniverseID { get; set; }
        [Required]
        public int LeagueID { get; set; }
        public int? TeamID { get; set; }
        public int? PlayerID { get; set; }
        public int? Round { get; set; }
        public int? OverallPick { get; set; }
        public int? PickInRound { get; set; }
        public int? PositionPick { get; set; }
        public int? PositionRound { get; set; }
        public DateTime? DraftedTimeStamp { get; set; }
        public bool? IsKeeper { get; set; }

        #region Foreign Keys
        [ForeignKey("LeagueID")]
        public virtual UserLeague? League { get; set; }
        //[ForeignKey("TeamID")]
        //public virtual UserLeagueTeam? LeagueTeam { get; set; }
        //[ForeignKey("PlayerID")]
        //public virtual Player? Player { get; set; }
        #endregion
    }
}
