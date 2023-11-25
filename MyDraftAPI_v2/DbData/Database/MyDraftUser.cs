using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public class MyDraftUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [MaxLength(450)]
        public string? UserUniqueID { get; set; }
        [MaxLength(256)]
        public string? UserName { get; set; }
        [MaxLength(256)]
        public string? UserEmail { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
