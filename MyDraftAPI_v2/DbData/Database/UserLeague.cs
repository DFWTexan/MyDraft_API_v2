using System.ComponentModel.DataAnnotations;

namespace Database.Model
{
    public class UserLeague
    {
        public int ID { get; set; }
        [MaxLength(50)]
        public string? Name{ get; set; }
        [MaxLength(5)]
        public string? Abbr { get; set; }
        public int Mode { get; set; }
        public int DraftType { get; set; }
        [MaxLength(10)]
        public string? DraftOrder { get; set; }
        public int NumberOfTeams { get; set; }
        public int NumberOfRounds { get; set; }
        public DateTime LastActiveDate { get; set; }
    }
}
