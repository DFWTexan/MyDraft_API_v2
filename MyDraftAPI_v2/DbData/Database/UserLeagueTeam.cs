using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public partial class UserLeagueTeam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int LeagueID { get; set; }
        public string? Name { get; set; }
        public string? Abbreviation { get; set; }
        public int DraftPosition { get; set; }  
        public string? Owner { get; set; }
        public bool IsMyTeam { get; set; }  

        #region Foreign Keys
        [ForeignKey("LeagueID")]
        public virtual UserLeague? League { get; set; }
        #endregion
    }
}
