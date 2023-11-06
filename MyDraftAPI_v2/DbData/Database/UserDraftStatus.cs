using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public class UserDraftStatus
    {
        public int UniverseID { get; set; }
        public int LeagueID { get; set; }
        public int CurrentPick { get; set; }
        public bool IsComplete { get; set; }

        #region Foreign Keys
        [ForeignKey("LeagueID")]
        public virtual UserLeague? League { get; set; }
        #endregion

    }
}
