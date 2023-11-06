using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public partial class UserDraftSelections
    {
        [Required]
        public int UniverseID { get; set; }
        [Required]
        public int LeagueID { get; set; }
        [Required]
        public int TeamID { get; set; }
        [Required]
        public int? Round { get; set; }
        public int PlayerID { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool? IsKeeper { get; set; }
        public int? PickInRound { get; set; }
        public int? OverallPick { get; set; }
        public int? PositionPick { get; set; }
        public int? PositionRound { get; set; }

        //#region Foreign Keys
        //[ForeignKey("LeagueID")]
        //public virtual UserLeague? League { get; set; }
        //[ForeignKey("TeamID")]
        //public virtual UserLeagueTeams? LeagueTeam { get; set; }
        //[ForeignKey("PlayerID")]
        //public virtual Player? Player { get; set; }
        //#endregion
    }
}
