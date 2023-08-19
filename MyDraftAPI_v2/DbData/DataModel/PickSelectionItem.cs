using System.ComponentModel.DataAnnotations.Schema;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class PickSelectionItem
    {
        [Column("round")]
        public int round { get; set; }
        [Column("overall")]
        public int overall { get; set; }
    }
}
