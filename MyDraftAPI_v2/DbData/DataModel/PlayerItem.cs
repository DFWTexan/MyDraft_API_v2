using System.ComponentModel.DataAnnotations.Schema;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class PlayerItem
    {
        [Column("player_id")]
        public string playerid { get; set; }
        [Column("fullname")]
        public string fullname { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", fullname);
        }

    }
}
