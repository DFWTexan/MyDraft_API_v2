using Database.Model;

namespace ViewModel
{
    public class DraftStatus : UserDraftStatus
    {
        public string? fanTeam { get; set; }
        public DraftStatus() { }
        public DraftStatus(int universeID, int leagueID, int currentPick, bool isComplete)
        {
            UniverseID = universeID;
            LeagueID = leagueID;
            CurrentPick = currentPick;
            IsComplete = isComplete;
        }
    }
}
