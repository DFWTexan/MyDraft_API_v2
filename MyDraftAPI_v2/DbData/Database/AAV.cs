using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public class AAV
    {
        public int? PlayerID { get; set; }   
        public decimal? StandardValue { get; set; }
        public decimal? PPRValue { get; set; }

        #region Foreign Keys
        [ForeignKey("PlayerID")]
        public virtual Player? Player { get; set; }
        #endregion
    }
}
