using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public class UserDraftStatus
    {
        public int UniverseID { get; set; }
        public int LeagueID { get; set; }
        public int CurrentPick { get; set; }
        public int onTheClock { get; set; }
        [MaxLength(50)]
        public string? fanTeamName { get; set; }
        public bool IsComplete { get; set; }

        #region Foreign Keys
        [ForeignKey("LeagueID")]
        public virtual UserLeague? League { get; set; }
        #endregion

    }
}
