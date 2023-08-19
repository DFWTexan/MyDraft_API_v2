using System.ComponentModel.DataAnnotations.Schema;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class PlayerSchedule
    {
        [Column("week")]
        public int week { get; set; }

        [Column("homway")]
        public String homway { get; set; }

        [Column("opponent")]
        public String opponent { get; set; }

        [Column("player_week_projection")]
        public double player_week_projection { get; set; }

        public string weeklabel
        {
            get
            {
                string _wkLabel;
                if (homway == "vs")
                {
                    _wkLabel = string.Format("{0} {1}", null, opponent);
                }
                else
                {
                    _wkLabel = string.Format("{0} {1}", homway, opponent);
                }
                return _wkLabel;
            }
        }
    }
}
