using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public partial class Points
    {
        public int ID { get; set; }
        public int PlayerID { get; set; }
        public int LeagueID { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        [MaxLength(10)]
        public string? Tag { get; set; }
        public DateTime? TimeStamp { get; set; }
        public decimal? Value { get; set; }
        [MaxLength(5)]
        public string? GroupAbbr { get; set; }
        
        #region Foreign Keys
        [ForeignKey("PlayerID")]
        public virtual Player? Player { get; set; }
        [ForeignKey("LeagueID")]
        public virtual UserLeague? League { get; set; }
        #endregion
    }
}
