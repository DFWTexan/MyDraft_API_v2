namespace ViewModel
{
    public class DraftPositionPick
    {
        public string? PositionGroup { get; set; }
        public string? PlayerName { get; set; }
        public int Round { get; set; }
        public int PickInRound { get; set; }

        public DraftPositionPick() { }
        public DraftPositionPick(string positionGroup, string playerName, int round, int pickInRound)
        {
            PositionGroup = positionGroup;
            PlayerName = playerName;
            Round = round;
            PickInRound = pickInRound;
        }
    }
}
