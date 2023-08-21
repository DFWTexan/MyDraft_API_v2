using System.ComponentModel.DataAnnotations.Schema;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class DraftPositionTotal
    {
        [Column("position")]
        public string position { get; set; }
        [Column("total")]
        public int total { get; set; }
    }
}
