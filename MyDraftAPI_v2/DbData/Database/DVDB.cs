using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public class DVDB
    {
        public int? PlayerID { get; set; }
        public int? LeagueID { get; set; }
        public decimal? Value { get; set; }
        public int? Year { get; set; }
        public int? Segment { get; set; }

        #region Foreign Keys
        [ForeignKey("PlayerID")]
        public virtual Player? Player { get; set; }
        #endregion
    }
}
