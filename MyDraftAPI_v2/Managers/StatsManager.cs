using MyDraftAPI_v2.FantasyDataModel;
using MyDraftLib.Utilities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;

namespace MyDraftAPI_v2.Managers
{
    public class StatsManager
    {
        public static String TABLE_STAT_MAP = "stat_map";
        public static String kCustomScoringSectionArray = "sections";
        public static String kCustomScoringTitle = "title";
        public static String kCustomScoringItems = "items";
        public static String kCustomScoringValue = "value";
        public static String kCustomScoringDBTable = "dbTable";
        public static String kCustomScoringDBColumn = "dbKey";
        public static String kCustomScoringDivide = "divide";

        public static int kFullSeason = 18;

        public static IDictionary<String, Object> getDisplayDataForType(String type, String position)
        {
            IDictionary<String, Object> groupData = null;
            try
            {
                IDictionary<String, Object> groupMap = (Dictionary<String, Object>)AppSettings.getStatsDisplayData()["position_map"];
                String positionKey = (String)groupMap[position];

                IDictionary<String, Object> displayGroup = (Dictionary<String, Object>)AppSettings.getStatsDisplayData()[type];
                groupData = (Dictionary<String, Object>)displayGroup[positionKey];
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to get display data for type: " + type + ", position: " + position + "/n" + ex.ToString());
            }
            finally
            {
                if (groupData == null)
                    groupData = new Dictionary<String, Object>();
            }

            return groupData;
        }

        public static StatsDisplay getStatsDisplayForGroup(String group, String position)
        {
            IDictionary<String, String> groupMap = (IDictionary<String, String>)AppSettings.getStatsDisplayData()["position_map"];
            String positionKey = groupMap[position];
            IDictionary<String, Object> groupData = getDisplayDataForType(group, position);

            if (groupData == null || groupData.Count() == 0)
                return null;

            StatsDisplay statsDisplay = new StatsDisplay((String)groupData["title"], positionKey, (IList<IDictionary<String, String>>)groupData["stats"]);

            return statsDisplay;
        }

        public static String getTablePrefixForStatsType(AppSettings.StatsType type)
        {
            String typeStr = null;

            switch (type)
            {
                case AppSettings.StatsType.StatsTypeActual:
                    typeStr = "stats";
                    break;

                case AppSettings.StatsType.StatsTypeProjections:
                    typeStr = "projections";
                    break;
            }

            return typeStr;
        }

        public static String getTableForType(AppSettings.StatsType type, String group)
        {
            String typeStr = getTablePrefixForStatsType(type);

            if (group != null)
                return String.Format("{0}_{1}", typeStr, group);
            else
                return typeStr;
        }

        public static async Task<IList<float>> getStatsForPlayerID(String playerID, AppSettings.StatsType type, int year, StatsDisplay statsDisplay)
        {
            return await getStatsForPlayerID(playerID, type, year, kFullSeason, statsDisplay);
        }

        public static async Task<IList<float>> getStatsForPlayerID(String playerID, AppSettings.StatsType type, int year, int segment, StatsDisplay statsDisplay)
        {
            if (playerID == null || statsDisplay == null || statsDisplay.getGroup() == null)
                return null;

            ISet<String> uniqueTables = new HashSet<String>(statsDisplay.getTables());
            // Remove the table that matches the group since we need to handle that table in a special way
            uniqueTables.Remove(statsDisplay.getGroup());

            // Get the main table that the query will key off. Other tables will be added with LEFT JOIN
            String firstTable = getTableForType(type, statsDisplay.getGroup());
            StringBuilder uniqueTableStr = new StringBuilder(firstTable);

            foreach (String table in uniqueTables)
            {
                String tableFullName = getTableForType(type, table);
                uniqueTableStr.Append(String.Format(" LEFT JOIN {0} ON ({1}.player_id = {2}.player_id AND {3}.year = {4}.year)",
                 tableFullName, tableFullName, firstTable, tableFullName, firstTable));
            }

            StringBuilder columnStr = new StringBuilder();
            foreach (String column in statsDisplay.getColumns())
            {
                if (columnStr.Length > 0)
                    columnStr.Append(",");

                columnStr.Append(String.Format("{0}.{1}", statsDisplay.getTableForStatKey(column, type), column));
            }

            String segmentColumn = "segment";

            String query = String.Format("SELECT {0} FROM {1} WHERE {2}.player_id = {3} AND {4}.year = {5} AND {6}.{7} = {8}",
                                columnStr, uniqueTableStr, firstTable, playerID, firstTable, year, firstTable, segmentColumn, segment);

            Debug.WriteLine("StatsDBAdapter", String.Format("statsForPlayer ({0}) query: {1}", year, query));

            List<Dictionary<string, float>> values = await DBAdapter.executeQuery<Dictionary<string, float>>(query);

            IList<float> result = new List<float>(statsDisplay.getColumns().Count());
            if (values.Count() > 0)
            {
                Dictionary<string, float> item = values[0];
                foreach (String column in statsDisplay.getColumns())
                {
                    float value = item[column];
                    result.Add(value);
                }
            }
            else // Fill in the stats with zeros
            {
                for (int i = 0; i < statsDisplay.getColumns().Count(); i++)
                {
                    if (statsDisplay.getColumns()[i].Equals("year"))
                        result.Add(year);
                    else
                        result.Add(0);
                }
            }

            return result;
        }

        #region Points Calculations Data

        public static String tableForStatsType(AppSettings.StatsType statsType, int week)
        {
            String prefix = getTablePrefixForStatsType(statsType);
            String suffix = week == kFullSeason ? "season" : "weekly";
            return String.Format("{0}_{1}", prefix, suffix);
        }

        public static ISet<String> statKeysFromGroup(CustomScoringGroup group)
        {
            // Get desired columns for custom scoring section
            ISet<String> statKeys = new HashSet<String>();
            foreach (CustomScoringSection section in group.getSections())
            {
                IList<CustomScoringDefaultItem> items = section.getScoringItems();
                foreach (CustomScoringDefaultItem item in items)
                {
                    statKeys.Add(item.key);
                }
            }
            return statKeys;
        }

        public static async Task<IDictionary<String, Object>> loadGroupStats(CustomScoringGroup group, int year, int week)
        {
            String table = tableForStatsType(AppSettings.StatsType.StatsTypeProjections, week);
            ISet<String> statKeys = statKeysFromGroup(group);
            String statKeysStr = TNUtility.arrayToString(statKeys.ToList<String>(), ",", true);
            StringBuilder query = new StringBuilder(String.Format("SELECT DISTINCT p.player_id, p.stat_value, sm.stat_key FROM {0} AS p LEFT JOIN " + TABLE_STAT_MAP + " AS sm ON sm.stat_id = p.stat_id " +
                       " WHERE p.player_id > 0 AND p.player_id NOTNULL AND p.stat_id IN (SELECT sm.stat_id FROM " + TABLE_STAT_MAP + " AS sm WHERE stat_key COLLATE NOCASE IN ({1})) group by p.player_id, sm.stat_key", table, statKeysStr));

            if (week != kFullSeason)
            {
                query.AppendFormat(" AND p.week = {0}", week);
            }

            IDictionary<String, Object> stats = new Dictionary<String, Object>();

            IList<StatVal> values = await DBAdapter.executeQuery<StatVal>(query.ToString());
            foreach (StatVal item in values)
            {
                IDictionary<String, float> playerStats = stats.ContainsKey(item.playerID) ? (IDictionary<String, float>)stats[item.playerID] : null;
                if (playerStats == null)
                {
                    playerStats = new Dictionary<String, float>();
                    stats.Add(item.playerID, playerStats);
                }
                playerStats.Add(item.abbr.ToLower(), item.statValue);
            }

            return stats;
        }
        #endregion

        public class StatVal
        {
            [Column("player_id")]
            public String playerID { get; set; }
            [Column("stat_value")]
            public float statValue { get; set; }
            [Column("stat_key")]
            public String abbr { get; set; }
        }
    }
}
