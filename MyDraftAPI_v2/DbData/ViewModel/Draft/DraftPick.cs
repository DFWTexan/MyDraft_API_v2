using Database.Model;

namespace ViewModel
{
    public class DraftPick
    {
        public int leagueID { get; set; }
        public int? playerID { get; set; }
        public int? overallPick { get; set; }
        public int? round { get; set; }
        public int? pickInRound { get; set; }
        public int? teamID { get; set; }
        public string? fanTeamName { get; set; }
        public float auctionValue { get; set; }
        public bool isKeeper { get; set; }
        public Player? player { get; set; }
        public bool isMyTeamPick { get; set; }

        public DraftPick() { }
        public DraftPick(int leagueID, int overall, int round, int pickInRound, int teamID, string fanTeamName, int playerID, float auctionValue, bool isKeeper, bool isMyTeamPick)
        {
            this.leagueID = leagueID;
            this.overallPick = overall;
            this.round = round;
            this.pickInRound = pickInRound;
            this.teamID = teamID;
            this.fanTeamName = fanTeamName;
            this.playerID = playerID;
            this.auctionValue = auctionValue;
            this.isMyTeamPick = isMyTeamPick;
            this.isKeeper = isKeeper;
        }
    }
}
