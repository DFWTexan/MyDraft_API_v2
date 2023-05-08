using System.ComponentModel.DataAnnotations;

namespace Database.Model
{
    public partial class Schedule
    {
        public int ID { get; set; }
        public int? Season { get; set; }
        public int? Week { get; set; }
        [MaxLength(5)]
        public string? HomeTeam { get; set; }
        [MaxLength(5)]
        public string? AwayTeam { get; set; }
        public DateTime? GameDate { get; set; }
    }
}
