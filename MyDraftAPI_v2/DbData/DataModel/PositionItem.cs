using System.ComponentModel.DataAnnotations.Schema;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class PositionItem
    {
        [Column("league_id")]
        public int leagueID { get; set; }
        [Column("position_key")]
        public String positionKey { get; set; }
        [Column("starters")]
        public int starters { get; set; }
        [Column("minimum")]
        public int minimum { get; set; }
        [Column("maximum")]
        public int intMaximum { get; set; }
        public int maximum
        {
            get
            {
                int iMax;
                if (intMaximum == -1)
                {
                    iMax = 0;
                }
                else
                {
                    iMax = intMaximum;
                }
                return iMax;
            }
        }
        [Column("flex")]
        public int flex { get; set; }
        [Column("key_abbr")]
        public string keyAbbr { get; set; }
        [Column("key_full")]
        public string keyFull { get; set; }
        [Column("sort_value")]
        public int sortValue { get; set; }
        [Column("type_value")]
        public string typeValue { get; set; }
    }
}
