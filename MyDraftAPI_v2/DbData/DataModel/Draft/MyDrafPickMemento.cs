using MyDraftLib.Utilities;

namespace MyDraftAPI_v2.FantasyDataModel.Draft
{
    public class MyDraftPickMemento
    {
        //Ignore teamID since we don't want to undo trades.
        public double? timestamp { get; set; }
        public int? overall { get; set; }
        public int? playerID { get; set; }
        public float? auctionValue { get; set; }
        public bool isKeeper { get; set; }

        public MyDraftPickMemento(MyDraftPick draftPick)
        {
            this.timestamp = TNUtility.DateTimeToUnixTimestamp(DateTime.Now);
            this.overall = draftPick.overallPick;
            this.playerID = draftPick.playerID;
            this.auctionValue = draftPick.auctionValue;
            //this.isKeeper = draftPick.isKeeper;
        }
    }
}
