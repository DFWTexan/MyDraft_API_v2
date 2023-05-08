using System.ComponentModel.DataAnnotations;

namespace Database.Model
{
    public partial class ProTeam
    {
        public int ID { get; set; }
        [MaxLength(3)]
        public string? Abbr { get; set; }
        [MaxLength(50)]
        public string? City { get; set; }
        [MaxLength(50)]
        public string? NickName { get; set; }
        public int? ByeWeek { get; set; }
        [MaxLength(50)]
        public string? Conference { get; set; }
        [MaxLength(50)]
        public string? Division { get; set; }
        [MaxLength(50)]
        public string? HeadCoach { get; set; }
    }
}
