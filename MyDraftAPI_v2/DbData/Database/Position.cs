using System.ComponentModel.DataAnnotations;

namespace Database.Model
{
    public class Position
    {
        public int ID { get; set; }
        [MaxLength(3)]
        public string? Abbr { get; set; }
        [MaxLength(50)]
        public string? FullName { get; set; }
    }
}
