using MyDraftAPI_v2.FantasyDataModel;
using System.Diagnostics;

namespace DraftService
{
    class LeagueScoringCalculator
    {
        public static String kCustomScoringSectionArray = "sections";
        public static String kCustomScoringTitle = "title";
        public static String kCustomScoringItems = "items";
        public static String kCustomScoringValue = "value";
        public static String kCustomScoringDBTable = "dbTable";
        public static String kCustomScoringDBColumn = "dbKey";
        public static String kCustomScoringDivide = "divide";

        public static float calculatePoints(IDictionary<String, float> stats, IList<CustomScoringDefaultItem> pointsMapping,
                IDictionary<String, CustomScoringUserItem> userValues)
        {
            float total = 0;

            try
            {
                foreach (CustomScoringDefaultItem item in pointsMapping)
                {
                    String statKey = item.getKey();
                    float statValue = stats.ContainsKey(statKey) ? (float)stats[statKey] : 0;
                    float scoringValue = 0;

                    CustomScoringUserItem userItem = userValues != null && userValues.ContainsKey(statKey) ? userValues[statKey] : null;
                    if (userItem != null)
                        scoringValue = userItem.getValue();
                    else
                        scoringValue = item.getValue();

                    if (item.isDivide() == true && scoringValue != 0)
                        total += statValue / scoringValue;
                    else
                        total += statValue * scoringValue;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR during custom scoring calculation [{0}]", ex.ToString());
            }

            return total;
        }
    }
}
