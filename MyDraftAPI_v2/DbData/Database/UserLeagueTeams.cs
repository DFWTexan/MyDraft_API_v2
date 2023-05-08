using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public partial class UserLeagueTeams
    {
        public int ID { get; set; }
        [Required]
        public int LeagueID { get; set; }
        public string? Name { get; set; }
        public string? Abbreviation { get; set; }
        public int DraftPosition { get; set; }  
        public string? Owner { get; set; }

        #region Foreign Keys
        [ForeignKey("LeagueID")]
        public virtual UserLeague? League { get; set; }
        #endregion
    }
}
