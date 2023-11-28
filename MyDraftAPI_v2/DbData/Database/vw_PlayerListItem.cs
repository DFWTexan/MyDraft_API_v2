using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public class vw_PlayerListItem
    {
        public int LeagueID { get; set; }
        public int PlayerID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set;}
        public string? FullName { get; set; }
        public string? PhotoURL { get; set; }
        public string? Position { get; set; }
        public string? TeamAbbr { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal PointsVal { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal AAVPoints { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal ADPPoints { get; set; }
        public bool IsDrafted { get; set; }
    }
}
