using System;
using System.Collections.Generic;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class Team
    {
        public static String ROSTER_DISPLAY_MYTEAM = "my_team";
        public static String ROSTER_DISPLAY_DEPTHCHART = "depthchart";

        public IList<String> getPlayerIDs()
        {
            return new List<String>();
        }
    }
}
