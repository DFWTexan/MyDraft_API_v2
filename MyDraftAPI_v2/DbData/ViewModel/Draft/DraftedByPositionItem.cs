namespace ViewModel
{
    public class DraftedByPositionItem
    {
        public string? PositionGroup { get; set; }
        public int? Count { get; set; }
        public List<ViewModel.DraftPick>? DraftPicks { get; set; }
        public Dictionary<int, Dictionary<int,List<ViewModel.DraftPositionPick>>>? RoundPicks { get; set; }
        public DraftedByPositionItem() { }
        public DraftedByPositionItem(string positionGroup, int count, List<ViewModel.DraftPick> draftPicks, Dictionary<int, Dictionary<int, List<ViewModel.DraftPositionPick>>> roundPicks) {
            PositionGroup = positionGroup;
            Count = count;  
            DraftPicks = draftPicks;
            RoundPicks = roundPicks;
        }
    }
}
