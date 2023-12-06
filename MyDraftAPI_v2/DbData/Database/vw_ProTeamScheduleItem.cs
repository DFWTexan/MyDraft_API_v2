namespace Database.Model
{
    public class vw_ProTeamScheduleItem
    {
        public int Season { get; set; }
        public int Week { get; set; }
        public int HomeTeamID { get; set; }
        public int AwayTeamID { get; set; }
    }
}
