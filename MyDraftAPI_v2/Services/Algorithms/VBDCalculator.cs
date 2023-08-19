using MyDraftAPI_v2.DbData.DataModel;
using MyDraftAPI_v2.Managers;
using MyDraftLib.Utilities;
using System.Diagnostics;
using static MyDraftAPI_v2.FantasyDataModel.FantasyLeage;

namespace DraftService
{
    public class VBDCalculator
    {
        public static async Task<IList<IDictionary<String, Object>>> calculateForLeague(FantasyLeague league)
        {
            IList<IDictionary<String, Object>> items = await AppSettings.filteredPositionSectionsForPositionGroup("position_selector", league);

            List<IDictionary<String, Object>> results = new List<IDictionary<String, object>>();
            foreach (IDictionary<String, Object> item in items)
            {
                if (item.ContainsKey("useTier") && (Boolean)item["useTier"])
                {
                    IList<String> positions = (IList<String>)item["positions"];
                    IList<IDictionary<String, Object>> positionResults = await calculateForLeague(league, positions);

                    if (positionResults != null && positionResults.Count > 0)
                    {
                        foreach (IDictionary<String, Object> positionItem in positionResults)
                            results.Add(positionItem);
                    }
                }
            }

            results.Sort(delegate (IDictionary<String, Object> x, IDictionary<String, Object> y)
            {
                float lhValue = (float)x["value"];
                float rhValue = (float)y["value"];
                return (int)(rhValue * 10 - lhValue * 10); // Multiply by 10 to make sure fractional difference is maintained
            });

            if (AppSettings.useAdjustedVBD())
            {
                // Using a try / catch block just to be extra careful. If algorithm throws exception or returns no results use default VBD
                try
                {
                    IList<IDictionary<String, Object>> adjustedResults = await adjustForADP(results, league);
                    if (adjustedResults != null && adjustedResults.Count == results.Count)
                        return adjustedResults;
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("ERROR during ADP + VBD calc: " + exception.ToString());
                }
            }

            return results;
        }

        private static async Task<IList<IDictionary<String, Object>>> calculateForLeague(FantasyLeague league, IList<String> positions)
        {
            FilterSort pointsSort = new FilterSort(FilterSort.SortType.SortTypePoints, league);

            String positionKey = TNUtility.getKeyFromArrayOfStrings(positions);
            int starters = league.getRosterValueForKey(positionKey, "starters");
            if (starters == 0)
                return null;

            int numTeams = league.numTeams;
            int baselineIndex = starters * numTeams - 1;

            String[] positionsArray = new String[positions.Count];
            positions.CopyTo(positionsArray, 0);

            IDictionary<String, Object> playerData = await DraftManager.getPlayerIDs(pointsSort, positionsArray,
                    DraftManager.PlayerDraftStatus.DraftStatusAll, PlayerManager.HealthStatus.HealthStatusAll, null, league, 0);
            IList<String> playerIDArray = (IList<String>)playerData["playerIDs"];
            IList<float> pointsArray = (IList<float>)playerData["sortValues"];

            IList<IDictionary<String, Object>> results = new List<IDictionary<String, Object>>(playerIDArray.Count);

            if (playerIDArray.Count == 0 || pointsArray.Count == 0 || playerIDArray.Count != pointsArray.Count || baselineIndex < 0)
                return results;

            if (baselineIndex > pointsArray.Count - 1)
                baselineIndex = pointsArray.Count - 1;

            float baselineValue = pointsArray[baselineIndex];
            for (int index = 0; index < playerIDArray.Count; index++)
            {
                String playerID = playerIDArray[index];
                float points = pointsArray[index];
                float VBD = points - baselineValue;
                IDictionary<String, Object> data = new Dictionary<String, Object>(2);
                data.Add("value", VBD);
                data.Add("player_id", playerID);
                results.Add(data);
            }

            return results;
        }

        private static async Task<IList<IDictionary<String, Object>>> adjustForADP(IList<IDictionary<String, Object>> vbdData, FantasyLeague league)
        {
            if (vbdData == null || vbdData.Count == 0)
                return null;

            IList<IDictionary<String, Object>> results = new List<IDictionary<String, Object>>(vbdData.Count);

            float minValue = (float)vbdData[vbdData.Count - 1]["value"];
            float maxValue = (float)vbdData[0]["value"];
            float pointScale = maxValue - minValue;
            float adpVBDAdjustmentRatio = AppSettings.getAdpVBDAdjustmentRatio();

            FilterSort adpSort = new FilterSort(FilterSort.SortType.SortTypeADP, league);

            IDictionary<String, Object> adpData = await DraftManager.getPlayerIDs(adpSort, null,
                    DraftManager.PlayerDraftStatus.DraftStatusAll, PlayerManager.HealthStatus.HealthStatusAll, null, league, 290);

            IList<String> playerIDArray = (IList<String>)adpData["playerIDs"];
            IList<float> valueArray = (IList<float>)adpData["sortValues"];

            if (playerIDArray.Count == 0 || valueArray.Count == 0 || playerIDArray.Count != valueArray.Count)
                return results;

            IDictionary<String, float> adpDict = new Dictionary<String, float>(adpData.Count);
            for (int index = 0; index < playerIDArray.Count; index++)
            {
                adpDict.Add(playerIDArray[index], valueArray[index]);
            }

            float maxAdpValue = valueArray[valueArray.Count - 1];

            if (maxAdpValue <= 0)
                return results;

            for (int index = 0; index < vbdData.Count; index++)
            {
                IDictionary<String, Object> playerData = vbdData[index];
                float vbdValue = (float)playerData["value"];
                String playerID = (String)playerData["player_id"];

                // Get the ADP value adjusted to the VBD point scale

                float adpValue = adpDict.ContainsKey(playerID) ? (float)adpDict[playerID] : 0;
                float adpRatio = adpValue > 0 ? adpValue / maxAdpValue : 1;
                float adjustmentValue = pointScale - pointScale * adpRatio;

                // Adjust the VBD value using the difference in the ADP value and the VBD value scaled by the adjustment ratio. 0.5 means equal amount ADP & VBD
                float adjustedVBD = vbdValue - (vbdValue - adjustmentValue) * adpVBDAdjustmentRatio;

                IDictionary<String, Object> adjustedPlayerData = new Dictionary<String, Object>(2);
                adjustedPlayerData.Add("player_id", playerID);
                adjustedPlayerData.Add("value", (float)adjustedVBD);
                results.Add(adjustedPlayerData);
            }

            return results;
        }
    }
}
