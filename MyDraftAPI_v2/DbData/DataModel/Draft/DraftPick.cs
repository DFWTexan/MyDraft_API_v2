using System.ComponentModel.DataAnnotations.Schema;
using static MyDraftAPI_v2.FantasyDataModel.FantasyLeage;

namespace MyDraftAPI_v2.FantasyDataModel.Draft
{
    public class DraftPick
    {
        [Column("league_id")]
        public int leagueID { get; set; }
        [Column("overall")]
        public int overall { get; set; }
        [Column("round")]
        public int round { get; set; }
        [Column("pick_in_round")]
        public int pickInRound { get; set; }
        [Column("team_id")]
        public int teamID { get; set; }
        [Column("player_id")]
        public String playerID { get; set; }
        [Column("auction_value")]
        public float auctionValue { get; set; }
        [Column("is_keeper")]
        public bool isKeeper { get; set; }

        public FantasyLeague league { get; set; }
        public FantasyTeam team
        {
            get
            {
                return league.teamWithID(teamID);
            }
        }

        public DraftPick() { }
        public DraftPick(int leagueID, int overall, int round, int pickInRound, int teamID, String playerID, float auctionValue, bool isKeeper)
        {
            this.leagueID = leagueID;
            this.overall = overall;
            this.round = round;
            this.pickInRound = pickInRound;
            this.teamID = teamID;
            this.playerID = playerID;
            this.auctionValue = auctionValue;
            this.isKeeper = isKeeper;
        }

        public DraftPickMemento getState()
        {
            return new DraftPickMemento(this);
        }

        public void setState(DraftPickMemento memento)
        {
            this.overall = memento.overall;
            this.playerID = memento.playerID;
            this.auctionValue = memento.auctionValue;
            this.isKeeper = memento.isKeeper;
        }

    }
}
