namespace ViewModel
{
    public class DraftPick
    {
        public int UniverseID { get; set; }
        public int leagueID { get; set; }
        public int? overallPick { get; set; }
        public int? round { get; set; }
        public int? pickInRound { get; set; }
        public int teamID { get; set; }
        public int playerID { get; set; }
        public float auctionValue { get; set; }
        public bool isKeeper { get; set; }

        public DraftPick() { }
        public DraftPick(int leagueID, int overall, int round, int pickInRound, int teamID, int playerID, float auctionValue, bool isKeeper)
        {
            this.leagueID = leagueID;
            this.overallPick = overall;
            this.round = round;
            this.pickInRound = pickInRound;
            this.teamID = teamID;
            this.playerID = playerID;
            this.auctionValue = auctionValue;
            this.isKeeper = isKeeper;
        }
    }
}
