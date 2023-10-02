//using System.ComponentModel.DataAnnotations.Schema;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class DraftStatus
    {
        //[Column("league_id")]
        public int leagueID { get; set; }
        //[Column("current_pick")]
        public int onTheClock { get; set; }
        public string? fanTeamName { get; set; }
        //[Column("seconds_remaining")]
        public int secondsRemaining { get; set; }
        //[Column("is_complete")]
        public Boolean isComplete { get; set; }

        public DraftStatus() { }

        public DraftStatus(int leagueID, int onTheClock, int secondsRemaining, Boolean isComplete)
        {
            this.leagueID = leagueID;
            this.onTheClock = onTheClock;
            this.secondsRemaining = secondsRemaining;
            this.isComplete = isComplete;
        }
    }
}
