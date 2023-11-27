using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public class UserLeague
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int MyDraftUserID { get; set; }
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
        public int NumberOfStarters { get; set; }
        public DateTime LastActiveDate { get; set; }
        public ICollection<UserLeagueTeam>? LeagueTeams { get; set; }
        public ICollection<UserDraftSelection>? UserDraftSelections { get; set; }

        #region // Foreign Keys //
        [ForeignKey("MyDraftUserID")]
        public virtual MyDraftUser? MyDraftUser { get; set; }
        #endregion
    }
}
