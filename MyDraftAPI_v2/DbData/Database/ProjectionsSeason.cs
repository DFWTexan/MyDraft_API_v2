using System.ComponentModel.DataAnnotations;

namespace Database.Model
{
    public class ProjectionsSeason
    {
        [Key]
        public int PlayerID { get; set; }
        public int StatID { get; set; }
        public int Year { get; set; }
        public decimal? Value { get; set; }
    }
}
