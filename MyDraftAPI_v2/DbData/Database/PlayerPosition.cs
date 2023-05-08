using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public class PlayerPosition
    {
        public int? PlayerID{ get; set; }
        public int? PositionID { get; set; }
    }
}
