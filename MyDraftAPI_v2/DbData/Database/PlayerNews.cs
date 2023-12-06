using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public class PlayerNews
    {
        public int ID { get; set; }
        public int PlayerID { get; set; }
        public DateTime? PubDate { get; set; }
        //[MaxLength(250)]
        public string? Title { get; set; }
        //[MaxLength(550)]
        public string? NewsDescription{ get; set; }
        //public string? Reccomendation{ get; set; }
        //[MaxLength(300)]
        public string? ImageURL { get; set; }
        [MaxLength(350)]
        public string? Analysis { get; set; }
        [MaxLength(50)]
        public string? InjuryType{ get; set; }

        #region Foreign Keys
        [ForeignKey("PlayerID")]
        public virtual Player? Player { get; set; }
        #endregion
    }
}
