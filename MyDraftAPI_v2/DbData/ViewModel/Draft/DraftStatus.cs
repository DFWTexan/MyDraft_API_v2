using Database.Model;

namespace ViewModel
{
    public class DraftStatus : UserDraftStatus
    {
        public bool IsMyTeamPick { get; set; }
        public DraftStatus() { }
        public DraftStatus(int leagueID, int currentPick, bool isComplete)
        {
            LeagueID = leagueID;
            CurrentPick = currentPick;
            IsComplete = isComplete;
        }
    }
}
