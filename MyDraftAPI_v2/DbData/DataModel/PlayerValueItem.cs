using System.ComponentModel.DataAnnotations.Schema;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class PlayerValueItem
    {
        [Column("aav")]
        public float aav { get; set; }
        [Column("adp")]
        public float adp { get; set; }
        [Column("vbd")]
        public float vbd { get; set; }
        [Column("points")]
        public float points { get; set; }
    }
}
