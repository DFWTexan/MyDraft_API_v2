using MyDraftAPI_v2.FantasyDataModel.Draft;

namespace MyDraftAPI_v2.FantasyDataModel.Draft
{
    public class DraftMemento
    {
        public int leagueID { get; set; }
        public int onTheClock { get; set; }
        public DraftPickMemento draftPickMemento { get; set; }
    }
}
