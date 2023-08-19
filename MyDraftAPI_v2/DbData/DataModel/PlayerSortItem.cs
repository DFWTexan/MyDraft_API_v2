namespace MyDraftAPI_v2.DbData.DataModel
{
    public class PlayerSortItem
    {
        public string playerID { get; set; }
        public string position { get; set; }
        public float sortValue { get; set; }
        public FilterSort.SortType sortType { get; set; }
        public string pickround { get; set; }
    }
}
