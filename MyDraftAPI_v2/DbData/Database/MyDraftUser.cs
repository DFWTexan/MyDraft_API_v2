using System.ComponentModel.DataAnnotations;

namespace Database.Model
{
    public class MyDraftUser
    {
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
