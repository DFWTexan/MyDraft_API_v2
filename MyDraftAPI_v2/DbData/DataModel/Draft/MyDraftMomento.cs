﻿
namespace MyDraftAPI_v2.FantasyDataModel
{
    public class MyDraftMemento
    {
        public int leagueID { get; set; }
        public int onTheClock { get; set; }
        public MyDraftPickMemento? MyDraftPickMemento { get; set; }
    }
}
