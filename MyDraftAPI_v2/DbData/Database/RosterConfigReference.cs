using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace Database.Model
{
    public class RosterConfigReference
    {
        [Key]
        [MaxLength(256)]
        public string? PositionKey { get; set; }
        [MaxLength(50)]
        public string? KeyAbbr { get; set; }
        [MaxLength(256)]
        public string? KeyFullName { get; set; }
        public int SortValue { get; set; }
        [MaxLength(50)]
        public string? TypeValue { get; set; }
    }
}
