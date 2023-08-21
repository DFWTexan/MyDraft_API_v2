using System.ComponentModel.DataAnnotations.Schema;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class DepthChartStats
    {
        [Column("year")]
        public int year { get; set; }
        [Column("pass_yards")]
        public double passYards { get; set; }
        [Column("pass_tds")]
        public double passTD { get; set; }
        [Column("pass_ints")]
        public double passINT { get; set; }

        [Column("rush_yards")]
        public double rushYards { get; set; }
        [Column("rush_tds")]
        public double rushTDs { get; set; }
        [Column("rush_attempts")]
        public double rushAttempts { get; set; }

        [Column("rec_yards")]
        public double recYards { get; set; }
        [Column("rec_tds")]
        public double recTDs { get; set; }
        [Column("rec")]
        public double receptions { get; set; }

        [Column("fg_made")]
        public double fgMade { get; set; }
        [Column("fg_a")]
        public double fgAttempts { get; set; }
        [Column("xp_made")]
        public double xpMade { get; set; }

        [Column("points_allowed")]
        public double pointsAllowed { get; set; }
        [Column("def_ints")]
        public double defInts { get; set; }
        [Column("fum_rec")]
        public double fumRec { get; set; }
        [Column("sacks")]
        public double sacks { get; set; }
        [Column("int_td")]
        public double intTD { get; set; }
        [Column("tackles")]
        public double tackles { get; set; }
        [Column("defensive_touchdowns")]
        public double defTD { get; set; }
    }
}
