using MyDraftLib.Utilities;

namespace MyDraftAPI_v2.Managers
{
    public class UpdateManager
    {

        public class Val
        {
            public String value { get; set; }
        }

        public static async Task<String> getUpdateIDForDataType(String type)
        {
            String updateID = "0";

            await Task.Delay(2000);
            //List<Val> values = await DBAdapter.dbAPP.QueryAsync<Val>("SELECT update_id as value FROM updates WHERE type = ?", type);
            List<Val> values = new List<Val>();

            if (values.Count > 0)
            {
                Val value = values.ElementAt(0);
                updateID = value.value;
            }
            return updateID;
        }

        public static async Task<String> getUpdateIDLimitForDataType(String type)
        {
            await Task.Delay(2000);
            //List<Val> values = await DBAdapter.executeQuery<Val>("SELECT update_id as value FROM server_api_status WHERE api_key = ?", type);
            List<Val> values = new List<Val>();

            String updateID = "0";

            if (values.Count > 0)
            {
                Val value = values.ElementAt(0);
                updateID = value.value;
            }

            return updateID;
        }

        public static async Task saveUpdateID(String type, String updateID)
        {
            //Log.i(TAG, "saveUpdateID: type: " + type + ", updateID: " + updateID);
            if (updateID == null)
                return;

            double timestamp = TNUtility.DateTimeToUnixTimestamp(DateTime.Now);

            await Task.Delay(2000);
            //await DBAdapter.executeUpdate("INSERT OR REPLACE INTO updates (update_id, type, timestamp) VALUES (?, ?, ?)", updateID, type, timestamp);
        }

        public static async Task<double> getLatestTimeStampForType(String type)
        {
            await Task.Delay(2000);
            //List<Val> values = await DBAdapter.executeQuery<Val>("SELECT MAX(timestamp) as value FROM updates WHERE type = ?", type);
            List<Val> values = new List<Val>();

            double timestamp = 0;
            if (values.Count() > 0)
            {
                timestamp = Convert.ToDouble(values[0].value);
            }

            return timestamp;
        }

        public static async Task<double> getLatestTimeStampForType(String type, int segment)
        {
            String query = String.Format("SELECT MAX(timestamp) as value FROM updates WHERE type LIKE '%s%%_%d'", type, segment);
            await Task.Delay(2000);
            //List<Val> values = await DBAdapter.executeQuery<Val>(query);
            List<Val> values = new List<Val>();

            double timestamp = 0;
            if (values.Count() > 0)
            {
                timestamp = Convert.ToDouble(values[0].value);
            }

            return timestamp;
        }

        public static async Task<double> getLatestTimeStampForType(String type, int year, int segment)
        {
            String query = String.Format("SELECT MAX(timestamp) as value FROM updates WHERE type LIKE '%s%%%d_%d'", type, year, segment);
            await Task.Delay(2000);
            //List<Val> values = await DBAdapter.executeQuery<Val>(query);
            List<Val> values = new List<Val>();

            double timestamp = 0;
            if (values.Count() > 0)
            {
                timestamp = Convert.ToDouble(values[0].value);
            }

            return timestamp;
        }

        public static async Task<double> getTimeStampForPointsWithLeagueID(int leagueID, String type, int year, int segment)
        {
            return await getTimeStampForPointsWithLeagueID(leagueID, type, year, segment, "default");
        }

        public static async Task<double> getTimeStampForPointsWithLeagueID(int leagueID, String type, int year, int segment, String tag)
        {
            await Task.Delay(2000);
            //List<Val> values = await DBAdapter.executeQuery<Val>("SELECT timestamp AS value FROM points_updates WHERE league_id = ? AND year = ? AND segment = ? AND tag = ?", leagueID, year, segment, tag);
            List<Val> values = new List<Val>();

            double timestamp = 0;
            if (values.Count() > 0)
            {
                timestamp = Convert.ToDouble(values[0].value);
            }

            return timestamp;
        }

        public static async Task resetTimeStampForPointsWithLeagueID(int leagueID)
        {
            await resetTimeStampForPointsWithLeagueID(leagueID, "default");
        }

        public static async Task resetTimeStampForPointsWithLeagueID(int leagueID, String tag)
        {
            await Task.Delay(2000);
            //await DBAdapter.executeUpdate("DELETE FROM points_updates WHERE league_id = ? AND tag = ?", leagueID, tag);
        }

        public static async Task resetTimeStampForPointsForAllLeagues()
        {
            await Task.Delay(2000);
            //await DBAdapter.executeUpdate("DELETE FROM points_updates");
        }
    }
}
