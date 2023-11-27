
using MyDraftAPI_v2.DbData.DataModel;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class MyDraftPick
    {
        //public int leagueID { get; set; }
        //public int? overall { get; set; }
        //public int? round { get; set; }
        //public int? pickInRound { get; set; }
        //public int? teamID { get; set; }
        //public string? playerID { get; set; }
        //public float? auctionValue { get; set; }
        //public bool? isKeeper { get; set; }

        public int leagueID { get; set; }
        public int? playerID { get; set; }
        public int? overallPick { get; set; }
        public int? round { get; set; }
        public int? pickInRound { get; set; }
        public int? teamID { get; set; }
        public float auctionValue { get; set; }
        public bool isKeeper { get; set; }
        public Database.Model.Player? player { get; set; }

        public MyFantasyLeague? league { get; set; }
        //public FantasyTeam team
        //{
        //    get
        //    {
        //        return league.teamWithID((int)teamID);
        //    }
        //}

        public MyDraftPick() { }
        public MyDraftPick(int leagueID, int overall, int round, int pickInRound, int teamID, int playerID, float auctionValue, bool isKeeper)
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

        public MyDraftPickMemento getState()
        {
            return new MyDraftPickMemento(this);
        }

        public void setState(MyDraftPickMemento memento)
        {
            this.overallPick = memento.overall;
            //this.playerID = memento.playerID;
            //this.auctionValue = memento.auctionValue;
            this.isKeeper = memento.isKeeper;
        }
    }
}
