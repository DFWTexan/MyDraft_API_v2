using System.ComponentModel.DataAnnotations.Schema;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class NewsItem
    {
        //[Column("update_id"), PrimaryKey]
        [Column("news_id")]
        public int newsID { get; set; }
        [Column("player_id")]
        public String playerID { get; set; }
        [Column("pub_date")]
        public String pubDate { get; set; }
        //public DateTime date
        //{
        //    get
        //    {
        //        return UIHelper.UnixTimeStampToDateTime(dateUnixTimestamp);
        //    }
        //}
        [Column("title")]
        public String title { get; set; }
        [Column("news_description")]
        public string newsDescription { get; set; }
        [Column("news_url")]
        public String newsUrl { get; set; }
        [Column("source")]
        public String source { get; set; }
        [Column("terms")]
        public String terms { get; set; }
        [Column("UID")]
        public int uid { get; set; }
    }
}
