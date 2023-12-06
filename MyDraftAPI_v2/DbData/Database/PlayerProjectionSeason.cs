using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace Database.Model
{
    public class PlayerProjectionSeason
    {
        [Key]
        public int PlayerID { get; set; }
        public double PassYards { get; set; }
        public double PassTD { get; set; }
        public double PassInt { get; set; }    
        public double RushYards { get; set; }
        public double RushTD { get; set; }
        public double RushAttempts { get; set; }
        public double RecYards { get; set; }
        public double RecTD { get; set; }
        public double Rec { get; set; }
        public double FgMade { get; set; }
        public double FgAtt { get; set; }
        public double XpMade { get; set; }
        public double PointsAllowed { get; set; }
        public double DefInts { get; set; }
        public double DefFumRec { get; set; }
        public double Sacks { get; set; }
    }
}
