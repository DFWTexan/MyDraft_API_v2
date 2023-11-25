using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public class UserLeague
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        //public int UniverseID { get; set; }
        [MaxLength(450)]
        public string? UserUniqueID { get; set; }
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
                   
        public ICollection<UserLeagueTeams>? LeagueTeams { get; set; }
    }
}
