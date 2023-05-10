using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public class Injury
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        [MaxLength(50)]
        public string? BodyPart { get; set; }
        [MaxLength(50)]
        public string? Status { get; set; }
        [MaxLength(50)]
        public string? Details { get; set; }

        #region Foreign Keys
        [ForeignKey("PlayerId")]
        public virtual Player? Player { get; set; }
        #endregion
    }
}
