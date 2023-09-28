using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public class vw_PlayerListItem
    {
        public int ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set;}
        public string? PhotoURL { get; set; }
        public string? Position { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal PointsVal { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal PPRVal { get; set; }
    }
}
