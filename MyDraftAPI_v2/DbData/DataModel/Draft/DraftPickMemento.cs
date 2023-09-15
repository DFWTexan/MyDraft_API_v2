using MyDraftAPI_v2.FantasyDataModel.Draft;
using MyDraftLib.Utilities;

namespace MyDraftAPI_v2.FantasyDataModel.Draft
{
    public class DraftPickMemento
    {
        //Ignore teamID since we don't want to undo trades.
        public double timestamp { get; set; }
        public int overall { get; set; }
        public String playerID { get; set; }
        public float auctionValue { get; set; }
        public bool isKeeper { get; set; }

        public DraftPickMemento(DraftPick draftPick)
        {
            this.timestamp = TNUtility.DateTimeToUnixTimestamp(DateTime.Now);
            this.overall = (int)draftPick.overall;
            this.playerID = draftPick.playerID;
            this.auctionValue = draftPick.auctionValue;
            this.isKeeper = draftPick.isKeeper;
        }
    }
}
