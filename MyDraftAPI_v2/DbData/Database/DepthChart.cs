using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public partial class DepthChart
    {
        [Required]
        public int PlayerID { get; set; }
        [Required]
        public int PositionID { get; set; }
        [Required]
        public int TeamID { get; set; }
        public int Rank{ get; set; }
        [MaxLength(3)]
        public string? TeamAbbr { get; set; }

        #region Foreign Keys
        [ForeignKey("PlayerID")]
        public virtual Player? Player { get; set; }
        [ForeignKey("PositionID")]
        public virtual Position? Position { get; set; }
        [ForeignKey("TeamID")]
        public virtual ProTeam? ProTeam { get; set; }
        #endregion
    }
}
