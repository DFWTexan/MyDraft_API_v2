namespace ViewModel
{
    public class FanTeamPick
    {
        public string? PositionGroup { get; set; }
        public string? PlayerName { get; set; }
        public int? @int { get; set; }
        public int SortOrder { get; set; }

        public FanTeamPick() { }
        public FanTeamPick(string positionGroup, string playerName, int sortOrder)
        {
            PositionGroup = positionGroup;
            PlayerName = playerName;
            SortOrder = sortOrder;
        }
    }
}
