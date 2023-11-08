namespace ViewModel
{
    public class DraftPositionPick
    {
        public string? PositionGroup { get; set; }
        public string? PlayerName { get; set; }
        public int SortOrder { get; set; }

        public DraftPositionPick() { }
        public DraftPositionPick(string positionGroup, string playerName, int sortOrder)
        {
            PositionGroup = positionGroup;
            PlayerName = playerName;
            SortOrder = sortOrder;
        }
    }
}
