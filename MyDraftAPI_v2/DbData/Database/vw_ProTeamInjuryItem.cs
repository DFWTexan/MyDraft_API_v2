namespace Database.Model
{
    public class vw_ProTeamInjuryItem
    {
        public int ProTeamID { get; set; }
        public int PlayerID { get; set; }
        public string? FirstNameInitial { get; set; }
        public string? LastName { get; set; }
        public string? Position { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
    }
}
