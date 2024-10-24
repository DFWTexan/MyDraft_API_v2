﻿using MyDraftAPI_v2.Engines;
using MyDraftAPI_v2.FantasyDataModel;
using MyDraftAPI_v2.Services.Utility.FanAppUtilities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyDraftAPI_v2.Managers
{
    public class PlayerManager
    {
        public static readonly String TABLE_AAV = "aav";
        public static readonly String TABLE_ADP = "adp";
        public static readonly String TABLE_VBD = "dvbd";
        public static readonly String TABLE_POINTS = "points";
        public static readonly String TABLE_USER_NOTE = "user_note";

        public enum PlayerExperience
        {
            experienceAll
           , experienceVeteran
           , experienceRookie
        }

        public enum HealthStatus
        {
            HealthStatusAll,
            HealthStatusInjured,
            HealthStatusNotInjured,
        }
        static List<string> _HealthFilter = new List<string>();
        public List<string> getHealthFilter
        {
            get
            {
                return _HealthFilter;
            }
            set
            {
                _HealthFilter = value;
            }
        }
        private class PlayerID
        {
            [Column("player_id")]
            public String playerID { get; set; }
        }
        private class DBValue
        {
            [Column("value")]
            public String value { get; set; }
        }
        private class iBoolDBValue
        {
            [Column("position_max_status")]
            public int value { get; set; }
        }
        private class PlayerSortVal
        {
            public String playerID { get; set; }
            public float sortValue { get; set; }
            public String position { get; set; }
            //public String pickround { get; set; }
        }

        public class Val
        {
            public String value { get; set; }
            public double doubleValue { get; set; }
        }

        public static async Task<IList<PlayerItem>> getPlayerFullNames()
        {
            String query = String.Format(@"SELECT distinct a1.player_id, a1.first_name ||' '|| a1.last_name ||' '|| a1.position ||'|'|| a1.team_abbr as fullname 
                                        FROM players a1 ");
            await Task.Delay(2000);
            //IList<PlayerItem> items = await DBAdapter.executeQuery<PlayerItem>(query);
            IList<PlayerItem> items = new List<PlayerItem>();

            return items;
        }
                
        public static async Task<Player> getPlayerWithID(String playerID)
        {
            string query = string.Format(@"SELECT DISTINCT * FROM players WHERE player_id = '{0}' ", playerID);
            await Task.Delay(2000);
            //IList<Player> values = await DBAdapter.executeQuery<Player>(query);
            IList<Player> values = new List<Player>();

            return values.FirstOrDefault();
        }

        public static async Task<IList<String>> getPlayerIDsWithHealthStatus(IList<String> playerIDs, HealthStatus status)
        {
            StringBuilder query = new StringBuilder("SELECT player_id FROM injury");
            String playerIDStr = TNUtility.arrayToString(playerIDs, ",", true);
            query.Append(String.Format(" WHERE player_id IN (%s)", playerIDStr));

            switch (status)
            {
                case HealthStatus.HealthStatusAll:
                    return playerIDs;

                case HealthStatus.HealthStatusInjured:
                    query.Append(" AND injured = 'YES'");
                    break;

                case HealthStatus.HealthStatusNotInjured:
                    query.Append(" AND injured = 'NO'");
                    break;
            }

            await Task.Delay(2000);
            //IList<int> values = await DBAdapter.executeQuery<int>(query.ToString());
            IList<int> values = new List<int>();

            ISet<String> filteredPlayerIDs = new HashSet<String>();

            foreach (int item in values)
            {
                String playerID = String.Format("%i", item);
                filteredPlayerIDs.Add(playerID);
            }

            return filteredPlayerIDs.ToArray<String>();
        }

        public static async Task<IDictionary<String, String>> getPlayerIDToPositionMap()
        {
            IDictionary<String, String> results = new Dictionary<String, String>();

            // Filter out all positions that are not primary or IDP (i.e. PR, KR)
            IList<String> specialTeamsPositions = AppSettings.getSpecialTeamsPositions();

            await Task.Delay(2000);
            //List<Dictionary<String, String>> values = await DBAdapter.executeQuery<Dictionary<String, String>>("SELECT player_id, position FROM depthchart");
            List<Dictionary<String, String>> values = new List<Dictionary<String, String>>();

            foreach (Dictionary<String, String> item in values)
            {
                String playerID = item["player_id"];
                String position = item["position"];

                if (!specialTeamsPositions.Contains(position))
                    results.Add(position, playerID);
            }

            return results;
        }

        public static async Task<ISet<Position>> positionsForPlayerID(String playerID)
        {
            String playerIDColumn = "player_id";

            String query = String.Format("SELECT position AS name, rank " +
                               "FROM depthchart WHERE {0} = '{1}' ORDER BY rank",
                               playerIDColumn, playerID);

            await Task.Delay(2000);
            //IList<Position> values = await DBAdapter.executeQuery<Position>(query);
            IList<Position> values = new List<Position>();

            ISet<Position> positions = new HashSet<Position>(values);

            return positions;
        }

        public static async Task<double> adpValue(String playerID, FantasyLeague league)
        {
            String source = league.isPPR ? "ppr" : "stand";
            await Task.Delay(2000);
            //IList<Val> items = await DBAdapter.executeQuery<Val>(string.Format("SELECT value_standard as doubleValue FROM adp WHERE player_id = {0}", playerID));
            IList<Val> items = new List<Val>();

            if (items.Count > 0)
            {
                return items[0].doubleValue;
            }

            return 0;
        }

        public static async Task<double> aavValue(String playerID)
        {
            await Task.Delay(2000);
            //IList<Val> items = await DBAdapter.executeQuery<Val>("SELECT averageValue AS doubleValue FROM aav WHERE player_id = ?", playerID);
            IList<Val> items = new List<Val>();

            if (items.Count > 0)
            {
                return items[0].doubleValue;
            }

            return 0;
        }

        public static async Task<Injury> getPlayerInjury(string playerID)
        {
            String query = String.Format(@"SELECT a1.player_id, a1.injury_status, injury_type
                                        FROM injury a1 
                                        join players a2 ON a1.player_id = a2.player_id
                                        WHERE a1.player_id = {0} LIMIT 1", playerID);
            await Task.Delay(2000);
            //IList<Injury> items = await DBAdapter.executeQuery<Injury>(query);
            IList<Injury> items = new List<Injury>();

            return items.FirstOrDefault();
        }
        public static async Task<PlayerNote> getPlayerNote(string playerID)
        {
            String query = String.Format(@"SELECT *
                                        FROM user_note a1 
                                        join players a2 ON a1.player_id = a2.player_id
                                        WHERE a1.player_id = {0} LIMIT 1", playerID);
            await Task.Delay(2000);
            //IList<PlayerNote> items = await DBAdapter.executeQuery<PlayerNote>(query);
            IList<PlayerNote> items = new List<PlayerNote>();

            return items.FirstOrDefault();
        }
        public static async Task<bool> isPlayerNote(string playerID)
        {
            StringBuilder query = new StringBuilder();
            query.Append(String.Format(@"SELECT player_id as value 
                                        FROM user_note 
                                        WHERE player_id = '{0}' ", playerID));
            await Task.Delay(2000);
            //IList<DBValue> items = await DBAdapter.executeQuery<DBValue>(query.ToString());
            IList<DBValue> items = new List<DBValue>();

            return items.Count > 0;
        }
        public static async Task<bool> checkPlayerPositionMax(FantasyLeague league, string position)
        {
            bool result = false;
            StringBuilder query = new StringBuilder();
            query.Append(String.Format(@"select 
                                            case 
                                            when ((count(a3.player_id) < ifnull(a2.maximum,0)) or (a2.maximum = -1)) then
                                                0
                                            else
                                                1
                                            end position_max_status
                                        from players a1
                                        left join user_roster_config a2 on a1.position = a2.key_abbr
                                            and a2.league_id = {0}
                                        left join user_draft_selections a3 on a2.league_id = a3.league_id
                                            and a1.player_id = a3.player_id
                                            and a3.team_id = {1}
                                            and a3.player_id is not null
                                        where a1.position = '{2}' ", league.identifier, league.myTeam.identifier, position));
            await Task.Delay(2000);
            //IList<iBoolDBValue> items = await DBAdapter.executeQuery<iBoolDBValue>(query.ToString());
            IList<iBoolDBValue> items = new List<iBoolDBValue>();

            if (items[0].value == 1)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }
        public static async Task<PlayerValueItem> getPlayerValues(string playerID)
        {
            /* PPR Check */
            string CheckPPR = string.Empty;
            if (MyDraftEngine.Instance.league.isPPR)
            {
                CheckPPR = "value_ppr";
            }
            else
            {
                CheckPPR = "value_standard";
            }
            String query = String.Format(@"select 
                                            {1}.{5} as aav
                                            ,{2}.{5} as adp
                                            ,{3}.value as vbd  
                                            ,{4}.points as points
                                        from players a1
                                        left join depthchart dc on a1.player_id = dc.player_id
                                        left join {1} on a1.player_id = {1}.player_id
                                            and {1} not null
                                        left join {2} on a1.player_id = {2}.player_id
                                            and {2} not null
                                        left join {3} on a1.player_id = {3}.player_id
                                            and value not null
                                        inner join {4} on a1.player_id = {4}.player_id
                                        WHERE a1.player_id = {0} 
                                            and {4}.league_id = {6} LIMIT 1", playerID, TABLE_AAV, TABLE_ADP, TABLE_VBD, TABLE_POINTS, CheckPPR, MyDraftEngine.Instance.league.identifier);
            await Task.Delay(2000);
            //IList<PlayerValueItem> items = await DBAdapter.executeQuery<PlayerValueItem>(query);
            IList<PlayerValueItem> items = new List<PlayerValueItem>();

            return items.FirstOrDefault();
        }

        #region // News //
        public static async Task<bool> isPlayerNews(string playerID)
        {
            StringBuilder query = new StringBuilder();
            query.Append(String.Format(@"SELECT player_id as value 
                                        FROM player_news 
                                        WHERE player_id = '{0}' ", playerID));
            await Task.Delay(2000);
            //IList<DBValue> items = await DBAdapter.executeQuery<DBValue>(query.ToString());
            IList<DBValue> items = new List<DBValue>();

            return items.Count > 0;
        }
        public static async Task<List<NewsItem>> getNews(string abbr)
        {
            List<NewsItem> newsItems = new List<NewsItem>();

            StringBuilder query = new StringBuilder();
            query.Append(String.Format(@"select distinct
                                            a1.*
                                        from {0}_news a1
                                        join {0}_player a2 on a1.player_id = a2.player_id ", AppSettings.SportsLeague.ToLower()));
            if (abbr != null && abbr != "ALL")
                query.Append(string.Format("where a2.team_abbr = '{0}'", abbr));
            if (abbr == null)
                query.Append(string.Format(@"JOIN usr_lineups a3 ON a1.player_id = a3.player_id
                                            AND a3.sports_league = '{0}' 
                                            AND a3.source_data = '{1}'
                                            AND a1.type = '{2} Lineups'
                                            AND date(a1.pub_date) = '{3}'", AppSettings.SportsLeague.ToLower(), AppSettings.SourceDataTXT.ToLower(), AppSettings.SportsLeague.ToUpper(), AppSettings.appDate));
            query.Append("ORDER BY a1.news_id DESC");

            await Task.Delay(2000);
            //IList<NewsItem> news = await DBAdapter.executeQuery<NewsItem>(query.ToString());
            IList<NewsItem> news = new List<NewsItem>();

            foreach (NewsItem item in news)
            {
                newsItems.Add(item);
            }
            return newsItems;
        }
        public static async Task<List<NewsItem>> getNewsForPlayers(string playerID)
        {
            List<NewsItem> newsItems = new List<NewsItem>();

            StringBuilder query = new StringBuilder();
            query.Append(String.Format(@"select 
                                            a1.*
                                        from player_news a1
                                        where a1.player_id = '{0}'
                                        order by a1.news_id desc", playerID));
            //query.Append("ORDER BY a1.uid ASC");

            await Task.Delay(2000);
            //IList<NewsItem> news = await DBAdapter.executeQuery<NewsItem>(query.ToString());
            IList<NewsItem> news = new List<NewsItem>();

            foreach (NewsItem item in news)
            {
                newsItems.Add(item);
            }
            return newsItems;
        }
        public static async Task<List<NewsItem>> getNewsForPlayers(string playerID, FantasyTeam fanTeam)
        {
            List<NewsItem> newsItems = new List<NewsItem>();

            string query = string.Empty;
            if (!AppSettings.isDraftTypeAuction)
            {
                query = String.Format(@"select 
                                            a1.*
                                        from player_news a1
                                        join user_draft_selections a2 on a1.player_id = a2.player_id
                                        where a2.team_id = '{0}'
                                        order by a1.news_id desc", fanTeam.identifier);
            }
            else
            {
                query = String.Format(@"select 
                                            a1.*
                                        from player_news a1
                                        join user_draft_assignments a2 on a1.player_id = a2.player_id
                                        where a2.team_id = '{0}'
                                        order by a1.news_id desc", fanTeam.identifier);
            }

            //query.Append("ORDER BY a1.uid ASC");

            await Task.Delay(2000);
            //IList<NewsItem> news = await DBAdapter.executeQuery<NewsItem>(query);
            IList<NewsItem> news = new List<NewsItem>();

            foreach (NewsItem item in news)
            {
                newsItems.Add(item);
            }
            return newsItems;
        }
        
        #endregion // News //

        #region // DepthChart //
        public static async Task<List<DepthChartItem>> getDepthChart(string teamAbbr, string pos)
        {
            List<DepthChartItem> depthchartItems = new List<DepthChartItem>();

            StringBuilder query = new StringBuilder();
            query.Append(String.Format(@"select 
                                            a1.*
                                        from depthchart a1
                                        join players a2 on a1.player_id = a2.player_id
                                        where a1.team_abbr = '{0}'
                                            and a1.position in ('{1}')
                                            and rank > 0
                                        order by rank asc", teamAbbr, pos.Replace("-", "', '")));
            //query.Append("ORDER BY a1.uid ASC");

            await Task.Delay(2000);
            //IList<DepthChartItem> items = await DBAdapter.executeQuery<DepthChartItem>(query.ToString());
            IList<DepthChartItem> items = new List<DepthChartItem>();

            foreach (DepthChartItem item in items)
            {
                IList<DepthChartStats> stats = await getDepthChartStats(item.playerID);
                item.stats = stats;
                depthchartItems.Add(item);
            }

            return depthchartItems;
        }
        public static async Task<IList<DepthChartStats>> getDepthChartStats(int playerID)
        {
            IList<DepthChartStats> depthchartStatItems = new List<DepthChartStats>();

            StringBuilder query = new StringBuilder();
            query.Append(String.Format(@"select 
                                            2017 as year
                                            , ifnull(pass_yards,0) as pass_yards
                                            , ifnull(pass_tds,0) as pass_tds
                                            , ifnull(pass_ints,0) as pass_ints
                                            , ifnull(rush_yards,0) as rush_yards
                                            , ifnull(rush_tds,0) as rush_tds
                                            , ifnull(rush_attempts,0) as rush_attempts
                                            , ifnull(rec_yards,0) as rec_yards
                                            , ifnull(rec_tds,0) as rec_tds
                                            , ifnull(rec,0) as rec
                                            , ifnull(fg_made,0) as fg_made
                                            , ifnull(fg_a,0) as fg_a
                                            , ifnull(xp_made,0) as xp_made
                                            , ifnull(points_allowed,0) as points_allowed
                                            , ifnull(def_ints,0) as def_ints
                                            , ifnull(fum_rec,0) as fum_rec
                                            , ifnull(sacks,0) as sacks
                                            , ifnull(int_td,0) as int_td
                                        from player_projections_season
                                        where player_id = {0}
                                        UNION
                                        select
                                            season as year
                                            , ifnull(sum(pass_yards),0) as pass_yards
                                            , ifnull(sum(pass_tds),0) as pass_tds
                                            , ifnull(sum(pass_ints),0) as pass_ints
                                            , ifnull(sum(rush_yards),0) as rush_yards
                                            , ifnull(sum(rush_tds),0) as rush_tds
                                            , ifnull(sum(rush_attempts),0) as rush_attempts
                                            , ifnull(sum(rec_yards),0) as rec_yards
                                            , ifnull(sum(rec_tds),0) as rec_tds
                                            , ifnull(sum(rec),0) as rec
                                            , ifnull(sum(fg_made),0) as fg_made
                                            , ifnull(sum(fg_a),0) as fg_a
                                            , ifnull(sum(xp_made),0) as xp_made
                                            , null as points_allowed
                                            , null as def_ints
                                            , null as fum_rec
                                            , null as sacks
                                            , null as int_td
                                        from player_stats_season
                                        where player_id = {0}
                                        group by season, player_id
                                        order by year desc", playerID));

            await Task.Delay(2000);
            //IList<DepthChartStats> items = await DBAdapter.executeQuery<DepthChartStats>(query.ToString());
            IList<DepthChartStats> items = new List<DepthChartStats>();

            foreach (DepthChartStats item in items)
            {
                depthchartStatItems.Add(item);
            }

            return depthchartStatItems;
        }
        #endregion // DepthChart //

        #region // Schedule //
        public static async Task<List<PlayerSchedule>> getPlayerSchedule(string playerID, string teamAbbr)
        {
            string query = string.Format(@"select distinct
                                                a1.*
                                                , (sum(a2.stat_value)/3) as player_week_projection
                                            from (
                                                select distinct
                                                    a1.game_id
                                                    , a1.week
                                                    ,'vs' as homway
                                                    , a1.away_team as opponent
                                                from schedule a1
                                                where home_team = '{1}'
                                                union
                                                select distinct
                                                    a1.game_id
                                                    , a1.week
                                                    , '@'
                                                    , a1.home_team
                                                from schedule a1
                                                where away_team = '{1}'
                                            ) a1
                                            join projections_weekly a2 on a1.week = a2.week
                                            where a2.player_id = {0}
                                                and a2.year = {2}
                                            group by a2.player_id, a2.week
                                            order by a1.week asc", playerID, teamAbbr, AppSettings.getProjectionStatsYear());

            await Task.Delay(2000);
            //List<PlayerSchedule> items = await DBAdapter.executeQuery<PlayerSchedule>(query);
            List<PlayerSchedule> items = new List<PlayerSchedule>();

            for (int i = 1; i < 18; i++)
            {
                if (!items.Exists(x => x.week == i))
                    items.Add(new PlayerSchedule() { week = i, homway = null, opponent = "BYE", player_week_projection = 0 });
            }

            return items;
        }
        #endregion // Schedule //

        #region // Notes //
        public static async Task savePlayerNote(Player player, int leagueID, string note)
        {
            string query = string.Format(@"insert or replace into {0} (player_id, league_id, note) values ({1}, {2}, '{3}')", TABLE_USER_NOTE, player.identifier, leagueID, note.Replace("'", "''"));
            await Task.Delay(2000);
            //await DBAdapter.executeUpdate(query);
        }
        #endregion // Notes //

        #region // Network //
        public static async Task updateAPIStatus(string api_key, int maxuid)
        {
            StringBuilder query = new StringBuilder();

            if (maxuid > 0)
            {
                query.Append(String.Format(@"INSERT OR REPLACE INTO server_api_status (api_key, update_id) 
                                        VALUES ('{0}', {1})", api_key, maxuid));
                await Task.Delay(2000);
                //await DBAdapter.executeUpdate(query.ToString());
            }
        }
        public static async Task<int> getMaxapiID(string api_key)
        {
            int iMaxID = 0;
            StringBuilder query = new StringBuilder();
            query.Append(String.Format(@"SELECT ifnull(max(update_id),0) as value
                                        FROM server_api_status
                                        WHERE api_key = '{0}'", api_key));
            await Task.Delay(2000);
            //IList<DBValue> value = await DBAdapter.executeQuery<DBValue>(query.ToString());
            IList<DBValue> value = new List<DBValue>();

            var item = value.FirstOrDefault();
            if (item != null)
                iMaxID = int.Parse(item.value);
            else
                iMaxID = 0;

            return iMaxID;
        }
        #endregion
    }
}
