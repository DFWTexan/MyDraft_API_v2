using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public partial class Schedule
    {
        public int ID { get; set; }
        public int? Season { get; set; }
        public int? Week { get; set; }
        public int? HomeTeamID { get; set; }
        //[MaxLength(5)]
        //public string? HomeTeamAbbr { get; set; }
        public int? AwayTeamID { get; set; }
        //[MaxLength(5)]
        //public string? AwayTeamAbbr { get; set; }
        [MaxLength(50)]
        public DateTime? GameDate { get; set; }

        #region Foreign Keys
        [ForeignKey("HomeTeamID")]
        public virtual ProTeam? HomeTeam { get; set; }
        [ForeignKey("AwayTeamID")]
        public virtual ProTeam? AwayTeam { get; set; }
        #endregion
    }
}
