using DraftService;
using Microsoft.Data.SqlClient;
using MyDraftAPI_v2.DbData.DataModel;
using MyDraftAPI_v2.Engines;
using MyDraftAPI_v2.FantasyDataModel;
using MyDraftAPI_v2.FantasyDataModel.Draft;
using MyDraftLib.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;
using Windows.Data.Json;
using Windows.Storage;
using static MyDraftAPI_v2.FantasyDataModel.FantasyLeage;

namespace MyDraftAPI_v2.Managers
{
    public class DraftManager
    {
        public enum PlayerDraftStatus
        {
            DraftStatusAvailable,
            DraftStatusDrafted,
            DraftStatusWishlist,
            DraftStatusKeeper,
            DraftStatusAll
        }

        public static readonly String TABLE_USER_LEAGUES = "user_leagues";
        public static readonly String TABLE_USER_DRAFT_RESULTS = "user_draft_selections";
        public static readonly String TABLE_USER_DRAFT_STATUS = "user_draft_status";
        public static readonly String TABLE_USER_DRAFT_ASSIGN = "user_draft_assignments";
        public static readonly String TABLE_USER_TAGS = "player_tags";
        public static readonly String TABLE_USER_TEAMS = "user_teams";
        public static readonly String TABLE_USER_RANKINGS = "custom_rankings";
        public static readonly String TABLE_USER_UNDO = "user_undo_stack";
        public static readonly String TABLE_USER_REDO = "user_redo_stack";

        #region Player Info

        private class PlayerSortVal
        {
            public String playerID { get; set; }
            public float sortValue { get; set; }
            public String position { get; set; }
            public String pickround { get; set; }
        }
        private static String getJoinQueryForDraftStatus(PlayerDraftStatus status, FantasyLeague league)
        {
            String query = "";

            switch (status)
            {
                case PlayerDraftStatus.DraftStatusAvailable:
                    {
                        if (!AppSettings.isDraftTypeAuction)
                        {
                            query = String.Format(" LEFT JOIN " + TABLE_USER_DRAFT_RESULTS + " ON (" + TABLE_USER_DRAFT_RESULTS + ".league_id = {0} AND " + TABLE_USER_DRAFT_RESULTS + ".player_id = players.player_id)",
                                 league.identifier);
                        }
                        else
                        {
                            query = String.Format(" LEFT JOIN " + TABLE_USER_DRAFT_ASSIGN + " ON (" + TABLE_USER_DRAFT_ASSIGN + ".league_id = {0} AND " + TABLE_USER_DRAFT_ASSIGN + ".player_id = players.player_id)",
                                 league.identifier);
                        }
                        break;
                    }

                case PlayerDraftStatus.DraftStatusDrafted:
                    {
                        if (!AppSettings.isDraftTypeAuction)
                        {
                            query = String.Format(" LEFT JOIN " + TABLE_USER_DRAFT_RESULTS + " ON (" + TABLE_USER_DRAFT_RESULTS + ".league_id = {0} AND " + TABLE_USER_DRAFT_RESULTS + ".player_id = players.player_id)",
                                 league.identifier);
                        }
                        else
                        {
                            query = String.Format(" LEFT JOIN " + TABLE_USER_DRAFT_ASSIGN + " ON (" + TABLE_USER_DRAFT_ASSIGN + ".league_id = {0} AND " + TABLE_USER_DRAFT_ASSIGN + ".player_id = players.player_id)",
                                 league.identifier);
                        }
                        break;
                    }

                case PlayerDraftStatus.DraftStatusWishlist:
                    {
                        query = String.Format(" LEFT JOIN player_tags ON (player_tags.league_id = {0} AND player_tags.player_id = players.player_id)",
                                 league.identifier);

                        break;
                    }

                case PlayerDraftStatus.DraftStatusKeeper:
                    {
                        query = String.Format(" LEFT JOIN " + TABLE_USER_DRAFT_RESULTS + " ON (" + TABLE_USER_DRAFT_RESULTS + ".league_id = {0} AND " + TABLE_USER_DRAFT_RESULTS + ".player_id = players.player_id)",
                                 league.identifier);

                        break;
                    }

                default:
                    break;
            }

            return query;
        }
        private static String getSubQueryForDraftStatus(PlayerDraftStatus status, FantasyLeague league)
        {
            String query = "";

            switch (status)
            {
                case PlayerDraftStatus.DraftStatusAvailable:
                    {
                        if (!AppSettings.isDraftTypeAuction)
                        {
                            query = String
                                .Format(" AND NOT EXISTS (SELECT * FROM " + TABLE_USER_DRAFT_RESULTS + " WHERE " + TABLE_USER_DRAFT_RESULTS + ".league_id = {0} AND " + TABLE_USER_DRAFT_RESULTS + ".player_id = players.player_id)",
                                        league.identifier);
                        }
                        else
                        {
                            query = String
                                .Format(" AND {0}.league_id is null", TABLE_USER_DRAFT_ASSIGN, league.identifier);
                        }
                        break;
                    }

                case PlayerDraftStatus.DraftStatusDrafted:
                    {
                        if (!AppSettings.isDraftTypeAuction)
                        {
                            query = String
                                .Format(" AND EXISTS (SELECT * FROM " + TABLE_USER_DRAFT_RESULTS + " WHERE " + TABLE_USER_DRAFT_RESULTS + ".league_id = {0} AND " + TABLE_USER_DRAFT_RESULTS + ".player_id = players.player_id)",
                                        league.identifier);
                        }
                        else
                        {
                            query = String
                                .Format(" AND {0}.league_id is not null", TABLE_USER_DRAFT_ASSIGN, league.identifier);
                        }
                        break;
                    }

                case PlayerDraftStatus.DraftStatusWishlist:
                    {
                        //query = getSubQueryForDraftStatus(PlayerDraftStatus.DraftStatusAvailable, league);
                        query = String.Format("{0} AND EXISTS (SELECT * FROM user_wishlist WHERE user_wishlist.player_id = players.player_id AND league_id = {1})", query, league.identifier);

                        break;
                    }

                case PlayerDraftStatus.DraftStatusKeeper:
                    {
                        //query = String.Format(" AND " + TABLE_USER_DRAFT_RESULTS + ".is_keeper = 1 AND " + TABLE_USER_DRAFT_RESULTS + ".league_id = {0}", league.identifier);
                        query = " AND is_keeper = 1";

                        break;
                    }

                default:
                    break;
            }

            return query;
        }
        public static async Task<IDictionary<String, Object>> getPlayerIDs(FilterSort sort, String[] positions, PlayerDraftStatus status,
                PlayerManager.HealthStatus health, String division, FantasyLeague league, int limit)
        {
            IList<String> playerIDs = new List<String>(limit);
            IList<float> sortValues = new List<float>(limit);
            IList<String> playerPositions = new List<String>(limit);
            IList<String> pickround = new List<String>(limit);

            IDictionary<String, Object> result = new Dictionary<String, Object>(2);
            result.Add("playerIDs", playerIDs);
            result.Add("sortValues", sortValues);
            result.Add("positions", playerPositions);
            result.Add("pickround", pickround);

            if (sort == null)
                return result;

            StringBuilder query = new StringBuilder();
            string valQuery = string.Empty;
            if (sort.getTable() == "points")
            {
                valQuery = string.Format("sum({0}.{1})", sort.getTable(), sort.getSortBy());
            }
            else if (sort.getTable() == "adp")
            {
                valQuery = string.Format("{0}.{1}{2}", sort.getTable(), sort.getSortBy(), sort.getSource());
            }
            else
            {
                valQuery = string.Format("{0}.{1}", sort.getTable(), sort.getSortBy());
            }


            String wishlistPickInRoundSubQuery = status == PlayerDraftStatus.DraftStatusWishlist ? ", pick_in_round as pickround" : "";
            query.Append(String.Format(@"SELECT distinct players.player_id AS playerID, 
                        {1} AS sortValue, players.position AS position {2} 
                    FROM players 
                    LEFT JOIN injury ON players.player_id = injury.player_id
                    LEFT JOIN {0} ON players.player_id = {0}.player_id
                    LEFT JOIN depthchart a1 ON players.player_id = a1.player_id", sort.getTable(), valQuery, wishlistPickInRoundSubQuery));

            query.Append(getJoinQueryForDraftStatus(status, league));
            
            if (division != null)
                query.Append(" LEFT JOIN pro_team ON pro_team.abbr = players.team_abbr ");

            query.Append(" WHERE players.player_id > 0 ");

            // for (FilterOption * option in filters)
            // [query appendString:[option subQuery]];

            if (positions != null && positions.Length > 0)
            {
                String positionStr = TNUtility.arrayToString(positions, ",", true);
                query.Append(String.Format(" AND players.position IN ({0})", positionStr));
            }

            switch (health)
            {
                case PlayerManager.HealthStatus.HealthStatusAll:
                    // Do nothing
                    break;

                case PlayerManager.HealthStatus.HealthStatusInjured:
                    query.Append(" AND injury.injured = 'YES'");
                    break;

                case PlayerManager.HealthStatus.HealthStatusNotInjured:
                    query.Append(" AND injury.injured = 'NO'");
                    break;

                default:
                    break;
            }

            if (sort.starterOnly)
                query.Append(" AND a1.rank = 1");

            query.Append(" AND players.player_id > 0"); // Never load a player
            // without a valid ID

            if (sort.isExcludeZero())
            {
                string valSort = string.Format("{0}.{1}", sort.getTable(), sort.getSortBy());
                if (sort.getTable() == "adp")
                {
                    valSort = string.Format("{0}.{1}{2}", sort.getTable(), sort.getSortBy(), sort.getSource());
                }

                query.Append(String.Format(" AND {0} > 0 ", valSort));
            }

            if (division != null && division.Length > 0)
            {
                query.Append(String.Format(" AND teams.div1 = '{0}'", division));
            }

            switch (sort.playerExperience)
            {
                case PlayerManager.PlayerExperience.experienceAll:
                    break;
                case PlayerManager.PlayerExperience.experienceVeteran:
                    query.Append(String.Format(" AND players.experience > 0", division));
                    break;
                case PlayerManager.PlayerExperience.experienceRookie:
                    query.Append(String.Format(" AND players.experience = 0", division));
                    break;
            }

            query.Append(getSubQueryForDraftStatus(status, league));

            query.Append(" GROUP BY players.player_id");

            // Setup the ORDER BY clause (must be added after filter options)
            string sortValue = "sortValue";
            if (sort.sortType == FilterSort.SortType.SortTypeADP)
            {
                sortValue = sort.getSortBy() + sort.getSource();
            }
            query.Append(String.Format(" ORDER BY {0} {1}", sortValue, sort.getDirection()));

            if (limit > 0)
                query.Append(String.Format(" LIMIT {0}", limit));

            await Task.Delay(2000);
            //IList<PlayerSortVal> items = await DBAdapter.executeQuery<PlayerSortVal>(query.ToString());
            IList<PlayerSortVal> items = new List<PlayerSortVal>();

            foreach (PlayerSortVal val in items)
            {
                playerIDs.Add(val.playerID);
                sortValues.Add(val.sortValue);
                playerPositions.Add(val.position);
                pickround.Add(val.pickround);
            }

            Debug.WriteLine("getPlayerIDs - query:\n" + query.ToString() + "\n count(" + playerIDs.Count + ")");

            return result;
        }
        public static async Task<IList<PlayerSortItem>> getPlayerSortItems(FilterSort sort)
        {
            IDictionary<String, Object> dict = await getPlayerIDs(sort, sort.positions, sort.playerDraftStatus, sort.healthStatus, sort.division, sort.league, sort.limit);
            IList<string> playerIDs = (IList<string>)dict["playerIDs"];
            IList<float> sortValues = (IList<float>)dict["sortValues"];
            IList<string> positions = (IList<string>)dict["positions"];
            IList<string> pickround = (IList<string>)dict["pickround"];

            if (playerIDs.Count != sortValues.Count || sortValues.Count != positions.Count)
                return null;

            int itemCount = playerIDs.Count;

            IList<PlayerSortItem> items = new List<PlayerSortItem>(itemCount);
            for (int i = 0; i < itemCount; i++)
            {
                PlayerSortItem item = new PlayerSortItem();
                item.playerID = playerIDs[i];
                item.sortValue = sortValues[i];
                item.position = positions[i];
                item.pickround = pickround[i];
                item.sortType = sort.sortType;
                items.Add(item);
            }

            return items;
        }
        #endregion

        #region Draft Picks
        private class DBValue
        {
            [Column("value")]
            public int value { get; set; }
            public String stringValue { get; set; }
        }
        public static async Task<int> maxOverall(int leagueID)
        {
            StringBuilder query = new StringBuilder();
            query.Append(string.Format(@"SELECT MAX(overall) AS value 
                                        FROM {0}
                                        WHERE league_id = {1} ", TABLE_USER_DRAFT_RESULTS, leagueID));
            await Task.Delay(2000);
            //IList<DBValue> items = await DBAdapter.executeQuery<DBValue>(query.ToString());
            IList<DBValue> items = new List<DBValue>();

            if (items.Count > 0)
                return items[0].value;
            else
                return 0;
        }
        public static async Task<DraftStatus> draftStatus(int leagueID)
        {

            StringBuilder query = new StringBuilder();
            query.Append(string.Format(@"SELECT * 
                                        FROM {0} 
                                        WHERE league_id = {1}", TABLE_USER_DRAFT_STATUS, leagueID));

            await Task.Delay(2000);
            //IList<DraftStatus> items = await DBAdapter.executeQuery<DraftStatus>(query.ToString());
            IList<DraftStatus> items = new List<DraftStatus>();

            if (items.Count > 0)
            {
                return items[0];
            }
            else
            {
                return new DraftStatus(leagueID, 0, 0, false);
            }
        }
        public static async Task<IList<DraftPick>> draftPicksForLeague(FantasyLeague league)
        {
            Debug.WriteLine("LOAD_DRAFT_PICKS_FOR_LEAGUE [{0}]", league.identifier);
            StringBuilder query = new StringBuilder();
            query.Append(string.Format(@"SELECT * FROM {0} WHERE league_id = {1}", TABLE_USER_DRAFT_RESULTS, league.identifier));

            await Task.Delay(2000);
            //IList<DraftPick> items = await DBAdapter.executeQuery<DraftPick>(query.ToString());
            IList<DraftPick> items = new List<DraftPick>();

            foreach (DraftPick draftPick in items)
            {
                draftPick.league = league;
            }
            return items;
        }
        public static async Task<bool> isDrafted(String playerID, FantasyLeague league)
        {
            String query;
            if (!await MyDraftEngine.Instance.league.isDraftAuction())
            {
                query = string.Format(@"SELECT player_id as value FROM {2} WHERE player_id = {0} AND league_id = {1}", playerID, league.identifier, TABLE_USER_DRAFT_RESULTS);
            }
            else
            {
                query = string.Format(@"SELECT player_id FROM {3} WHERE player_id = {0} AND league_id = {1}", playerID, league.identifier, TABLE_USER_DRAFT_ASSIGN);
            }

            await Task.Delay(2000);
            //IList<DBValue> items = await DBAdapter.executeQuery<DBValue>(query);
            IList<DBValue> items = new List<DBValue>();

            return items.Count > 0;
        }
        public static async Task<bool> isKeeper(String playerID, FantasyLeague league)
        {
            string query = string.Format(@"select a1.player_id as value
                                            from {0} a1
                                            where a1.player_id = {1}
                                                and a1.league_id = {2}
                                                and a1.is_keeper = 1", TABLE_USER_DRAFT_RESULTS, playerID, league.identifier);
            await Task.Delay(2000);
            //IList<DBValue> items = await DBAdapter.executeQuery<DBValue>(query);
            IList<DBValue> items = new List<DBValue>();

            return items.Count > 0;
        }
        public static async Task<bool> isOnMyTeam(String playerID, FantasyLeague league)
        {
            return await isOnTeam(playerID, league.myTeam, league);
        }
        public static async Task<bool> isOnTeam(String playerID, FantasyTeam team, FantasyLeague league)
        {
            string query = string.Format(@"select
                                                a1.player_id as value
                                            from {0} a1
                                            where a1.player_id = {1}
                                                and league_id = {2}
                                                and team_id = {3}", TABLE_USER_DRAFT_RESULTS, playerID, league.identifier, team.identifier);
            await Task.Delay(2000);
            //IList<DBValue> items = await DBAdapter.executeQuery<DBValue>(query);
            IList<DBValue> items = new List<DBValue>();

            return items.Count > 0;
        }
        public static async Task<bool> isTagged(String playerID, FantasyLeague league)
        {
            await Task.Delay(2000);
            //IList<DBValue> items = await DBAdapter.executeQuery<DBValue>("SELECT * FROM player_tags WHERE player_id = ? AND league_id = ?",
            //            playerID, league.identifier);
            IList<DBValue> items = new List<DBValue>();

            return items.Count > 0;
        }
        public static async Task<FantasyTeam> fantasyTeamForPlayer(String playerID, FantasyLeague league)
        {
            DraftPick draftPick = await draftPickForPlayer(playerID, league);
            if (draftPick != null)
            {
                return league.teamWithID(draftPick.teamID);
            }

            await Task.Delay(2000);
            //IList<DBValue> items = await DBAdapter.executeQuery<DBValue>("SELECT team_id AS value FROM " + TABLE_USER_DRAFT_ASSIGN + " WHERE player_id = ? AND league_id = ?", playerID, league.identifier);
            IList<DBValue> items = new List<DBValue>();

            if (items.Count > 0)
            {
                int teamID = items[0].value;
                return league.teamWithID(teamID);
            }

            return null;
        }
        public static async Task<DraftPick> draftPickForPlayer(String playerID, FantasyLeague league)
        {
            await Task.Delay(2000);
            //IList<DraftPick> items = await DBAdapter.executeQuery<DraftPick>("SELECT * FROM " + TABLE_USER_DRAFT_RESULTS + " WHERE player_id = ? AND league_id = ?", playerID, league.identifier);
            IList<DraftPick> items = new List<DraftPick>();

            if (items.Count > 0)
            {
                DraftPick draftPick = items[0];
                draftPick.league = league;
                return draftPick;
            }
            return null;
        }
        public static async Task tagPlayer(String playerID, FantasyLeague league, int pickround)
        {
            await tagPlayer(playerID, -1, league, pickround);
        }
        public static async Task tagPlayer(String playerID, int listID, FantasyLeague league, int pickround)
        {
            //await DBAdapter.executeUpdate("INSERT OR REPLACE INTO " + TABLE_USER_TAGS + " (player_id, league_id, list_id, rank) VALUES (?, ?, ?, (SELECT MAX(rank) FROM " + TABLE_USER_TAGS + " WHERE league_id = ?))",
            await Task.Delay(2000);
            //await DBAdapter.executeUpdate("INSERT OR REPLACE INTO " + TABLE_USER_TAGS + " (player_id, league_id, list_id, rank, pick_in_round) VALUES (?, ?, ?, ?, ?)",
            //            playerID, league.identifier, listID, null, pickround);
        }
        public static async Task unTagPlayer(String playerID, FantasyLeague league)
        {
            await Task.Delay(2000);
            //await DBAdapter.executeUpdate("DELETE FROM player_tags WHERE player_id = ? AND league_id = ?", playerID, league.identifier);
        }
        public static async Task<Boolean> saveDraftPick(DraftPick draftPick)
        {
            double timestamp = TNUtility.DateTimeToUnixTimestamp(DateTime.Now);
            await Task.Delay(2000);
            return true;
            //return await DBAdapter.executeUpdate("INSERT OR REPLACE INTO " + TABLE_USER_DRAFT_RESULTS +
            //        " (player_id, league_id, team_id, overall, round, pick_in_round, auction_value, is_keeper, timestamp) " +
            //        " VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)",
            //        draftPick.playerID, draftPick.leagueID, draftPick.teamID, draftPick.overall, draftPick.round, draftPick.pickInRound, draftPick.auctionValue, draftPick.isKeeper ? 1 : 0, timestamp);
        }
        private static void saveDraftPick(DraftPick draftPick, SqlConnection str)
        {
            double timestamp = TNUtility.DateTimeToUnixTimestamp(DateTime.Now);
            //connection.Execute("INSERT OR REPLACE INTO " + TABLE_USER_DRAFT_RESULTS +
            //        " (player_id, league_id, team_id, overall, round, pick_in_round, auction_value, is_keeper, timestamp) " +
            //        " VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)",
            //        draftPick.playerID, draftPick.leagueID, draftPick.teamID, draftPick.overall, draftPick.round, draftPick.pickInRound, draftPick.auctionValue, draftPick.isKeeper ? 1 : 0, timestamp);
        }
        public static async Task saveDraftPicks(IList<DraftPick> draftPicks)
        {
            if (draftPicks == null)
                return;

            await Task.Delay(2000);
            //await DBAdapter.dbAPP.RunInTransactionAsync((SQLiteConnection connection) =>
            //{
            //    foreach (DraftPick draftPick in draftPicks)
            //    {
            //        saveDraftPick(draftPick, connection);
            //    }

            //    Debug.WriteLine("SAVE_DRAFT_PICKS_DONE");
            //});
        }
        public static async Task saveDraftStatus(DraftStatus draftStatus)
        {
            await Task.Delay(2000);
            //await DBAdapter.executeUpdate("INSERT OR REPLACE INTO " + TABLE_USER_DRAFT_STATUS + " (league_id, current_pick, seconds_remaining, is_complete) VALUES (?, ?, ?, ?)",
            //    draftStatus.leagueID, draftStatus.onTheClock, draftStatus.secondsRemaining, draftStatus.isComplete);
        }
        public static async Task<IList<PickSelectionItem>> getAvailableWishlistPicks(int fanTeamID, int leagueID)
        {
            string query = string.Format(@"SELECT 
                                                round
                                                , overall               
                                            FROM user_draft_selections 
                                            WHERE league_id = {0} 
                                                and team_id = {1}
                                                and player_id is null", MyDraftEngine.Instance.league.identifier, MyDraftEngine.Instance.league.myTeam.identifier);
            await Task.Delay(2000);
            //IList<PickSelectionItem> result = await DBAdapter.executeQuery<PickSelectionItem>(query);
            IList<PickSelectionItem> result = new List<PickSelectionItem>();

            return result;
        }
        //public static async Task saveWishlist(int leagueID, string playerID, int round, int overall)
        //{
        //    await DBAdapter.executeUpdate("INSERT OR REPLACE INTO user_wishlist (player_id, league_id, round, overall) VALUES (?, ?, ?, ?)",
        //        playerID, leagueID, round, overall);
        //}
        public static async Task<bool> getWishlisted(String playerID, FantasyLeague league)
        {
            await Task.Delay(2000);
            //IList<DBValue> items = await DBAdapter.executeQuery<DBValue>("SELECT * FROM user_wishlist WHERE player_id = ? AND league_id = ?",
            //            playerID, league.identifier);
            IList<DBValue> items = new List<DBValue>();

            return items.Count > 0;
        }
        public static async Task<PickSelectionItem> getWishlistInfo(string playerID, int leagueID)
        {
            PickSelectionItem result = new PickSelectionItem();
            string query = string.Format(@"SELECT 
                                                round
                                                , overall               
                                            FROM user_wishlist 
                                            WHERE league_id = {0} 
                                                and player_id = {1}", MyDraftEngine.Instance.league.identifier, playerID);
            await Task.Delay(2000);
            //IList<PickSelectionItem> item = await DBAdapter.executeQuery<PickSelectionItem>(query);
            IList<PickSelectionItem> item = new List<PickSelectionItem>();

            result = item.FirstOrDefault();

            return result;
        }
        public static async Task<bool> isWishlistTargeted(int round, int leagueID)
        {
            //Debug.WriteLine("LOAD_DRAFT_PICKS_FOR_LEAGUE [{0}]", fanleague.identifier);
            string query = string.Format(@"select
                                                player_id as value
                                            from user_wishlist
                                            where round = {0}
                                                and league_id = {1}", round, MyDraftEngine.Instance.league.identifier);
            await Task.Delay(2000);
            //IList<DBValue> item = await DBAdapter.executeQuery<DBValue>(query);
            IList<DBValue> item = new List<DBValue>();

            return item.Count() > 0;
        }
        public static async Task<List<WishPlayer>> getWishlistTargeted(int round, int leagueID)
        {
            //Debug.WriteLine("LOAD_DRAFT_PICKS_FOR_LEAGUE [{0}]", fanleague.identifier);
            string query = string.Format(@"select
                                                a1.*
                                            from user_wishlist a1
                                            left join user_draft_selections a2 on a1.player_id = a2.player_id
                                            where a1.round = {0}
                                                and a1.league_id = {1}
                                                and a2.player_id is null", round, MyDraftEngine.Instance.league.identifier);
            await Task.Delay(2000);
            //List<WishPlayer> result = await DBAdapter.executeQuery<WishPlayer>(query);
            List<WishPlayer> result = new List<WishPlayer>();

            return result;
        }
        // -- Position Totals -- //
        public static async Task<DraftPositionTotal> getPositionTotal(FantasyLeague league, string position)
        {
            string pos = position;
            if (position == "RB" || position == "FB")
            {
                position = "FB-RB";
            }
            else if (position == "ILB" || position == "OLB")
            {
                position = "ILB-OLB";
            }
            string query;
            if (!AppSettings.isDraftTypeAuction)
            {
                query = string.Format(@"SELECT '{3}' as position, count(a1.player_id) as total 
                                        FROM {2} a1
                                        JOIN players a2 ON a1.player_id = a2.Player_id
                                        WHERE a1.league_id = {0}
                                            AND a2.position in ('{1}')", league.identifier, position.Replace("-", "','"), TABLE_USER_DRAFT_RESULTS, pos);
            }
            else
            {
                query = string.Format(@"SELECT '{3}' as position, count(a1.player_id) as total 
                                        FROM {2} a1
                                        JOIN players a2 ON a1.player_id = a2.Player_id
                                        WHERE a1.league_id = {0}
                                            AND a2.position in ('{1}')", league.identifier, position.Replace("-", "','"), TABLE_USER_DRAFT_ASSIGN, pos);
            }

            await Task.Delay(2000);
            //IList<DraftPositionTotal> items = await DBAdapter.executeQuery<DraftPositionTotal>(query);
            IList<DraftPositionTotal> items = new List<DraftPositionTotal>();

            DraftPositionTotal item = items.FirstOrDefault();

            return item;
        }
        public static async Task<DraftPositionTotal> getPositionTotal(FantasyLeague league, string position, FantasyTeam fanTeam)
        {
            string pos = position;
            if (position == "RB" || position == "FB")
            {
                position = "FB-RB";
            }
            else if (position == "ILB" || position == "OLB")
            {
                position = "ILB-OLB";
            }
            string query;
            if (!AppSettings.isDraftTypeAuction)
            {
                query = string.Format(@"SELECT '{4}' as position, count(a1.player_id) as total 
                                        FROM {2} a1
                                        JOIN players a2 ON a1.player_id = a2.Player_id
                                        WHERE a1.league_id = {0}
                                            AND a2.position in ('{1}')
                                            AND a1.team_id = {3}", league.identifier, position.Replace("-", "','"), TABLE_USER_DRAFT_RESULTS, fanTeam.identifier, pos);
            }
            else
            {
                query = string.Format(@"SELECT '{4}' as position, count(a1.player_id) as total 
                                        FROM {2} a1
                                        JOIN players a2 ON a1.player_id = a2.Player_id
                                        WHERE a1.league_id = {0}
                                            AND a2.position in ('{1}')
                                            AND a1.team_id = {3}", league.identifier, position.Replace("-", "','"), TABLE_USER_DRAFT_ASSIGN, fanTeam.identifier, pos);
            }

            await Task.Delay(2000);
            //IList<DraftPositionTotal> items = await DBAdapter.executeQuery<DraftPositionTotal>(query);
            IList<DraftPositionTotal> items = new List<DraftPositionTotal>();

            return items.FirstOrDefault();
        }
        public static async Task<bool> checkPositionLimits()
        {
            string query = String.Format(@"select distinct
                                            position_key 
                                            , maximum as value
                                        from user_roster_config
                                        where maximum > 0
                                            and league_id = {0}
                                        order by sort_value asc");
            await Task.Delay(2000);
            //List<DBValue> items = await DBAdapter.executeQuery<DBValue>(query);
            List<DBValue> items = new List<DBValue>();

            return items.Count() > 0;
        }
        #endregion

        #region Delete Data
        public static async Task deleteDraftPick(DraftPick draftPick)
        {
            await Task.Delay(2000);
            //await DBAdapter.executeUpdate("DELETE FROM " + TABLE_USER_DRAFT_RESULTS + " WHERE league_id = ? AND overall = ?",
            //        draftPick.leagueID, draftPick.overall);
        }
        public static async Task deleteDraftPicksForLeague(FantasyLeague league)
        {
            await Task.Delay(2000);
            //await DBAdapter.executeUpdate("DELETE FROM " + TABLE_USER_DRAFT_RESULTS + " WHERE league_id = ?",
            //        league.identifier);
        }
        public static async Task clearPlayerDraftDataForLeague(FantasyLeague league, bool removeKeepers)
        {
            if (league.draftByTeamEnabled)
            {
                if (removeKeepers)
                {
                    await Task.Delay(2000);
                    //await DBAdapter.executeUpdate("UPDATE " + TABLE_USER_DRAFT_RESULTS + " SET player_id = NULL WHERE league_id = ?", league.identifier);
                }
                else
                {
                    await Task.Delay(2000);
                    //await DBAdapter.executeUpdate("UPDATE " + TABLE_USER_DRAFT_RESULTS + " SET player_id = NULL WHERE league_id = ? AND is_keeper = 0", league.identifier);
                }

                await Task.Delay(2000);
                //await DBAdapter.executeUpdate("DELETE FROM " + TABLE_USER_DRAFT_STATUS + " WHERE league_id = ?", league.identifier);
            }
            else
            {
                await Task.Delay(2000);
                //await DBAdapter.executeUpdate("DELETE FROM " + TABLE_USER_DRAFT_RESULTS + " WHERE league_id = ?", league.identifier);
            }

            await clearDraftMementoUndoStack(league.identifier);
            await clearDraftMementoRedoStack(league.identifier);
        }
        public static async Task updateDraftPicks(FantasyLeague league)
        {
            if (!league.draftByTeamEnabled)
            {
                return;
            }

            List<DraftPick>? draftPicks = await draftPicksForLeague(league) as List<DraftPick>;
            int totalPicks = league.numTeams * league.rounds;
            if (draftPicks.Count > totalPicks)
            {
                int startIndex = totalPicks;
                int length = draftPicks.Count - totalPicks;
                draftPicks.RemoveRange(startIndex, length);
            }
            else
            {
                List<DraftPick> generatedDraftPicks = (List<DraftPick>)DraftPickGenerator.generateDraftPicks(league);
                int startIndex = draftPicks.Count;
                int length = generatedDraftPicks.Count - draftPicks.Count;
                IList<DraftPick> addedPicks = generatedDraftPicks.GetRange(startIndex, length);
                draftPicks.AddRange(addedPicks);

                await DraftManager.deleteDraftPicksForLeague(league);
                await DraftManager.saveDraftPicks(generatedDraftPicks);
                return;
            }

            await DraftManager.deleteDraftPicksForLeague(league);
            await DraftManager.saveDraftPicks(draftPicks);
        }
        public static async Task resetDraftData(FantasyLeague league)
        {
            //await DBAdapter.executeUpdate("DELETE FROM " + TABLE_USER_DRAFT_RESULTS + " WHERE league_id = ?", league.identifier);
            //await DBAdapter.executeUpdate("DELETE FROM " + TABLE_USER_DRAFT_STATUS + " WHERE league_id = ?", league.identifier);

            await clearDraftMementoRedoStack(league.identifier);
            await clearDraftMementoUndoStack(league.identifier);

            IList<DraftPick> draftPicks = DraftPickGenerator.generateDraftPicks(league);
            await saveDraftPicks(draftPicks);
        }
        //public static async Task deleteWishlist(Player player, int leagueID)
        //{
        //    await DBAdapter.executeUpdate("DELETE FROM user_wishlist WHERE league_id = ? AND player_id = ?",
        //            leagueID, player.identifier);
        //}
        #endregion

        #region // Auction //
        private static int intOverall = 0;
        private class doubleVal
        {
            [Column("value")]
            public double value { get; set; }
        }
        public static async Task<bool> isAuctionDraft()
        {
            string query = string.Format(@"select
                                                *
                                            from user_leagues a1
                                            where a1._id = {0}
                                                and a1.draft_type = 2", MyDraftEngine.Instance.league.identifier);
            await Task.Delay(2000);
            //IList<DBValue> items = await DBAdapter.executeQuery<DBValue>(query);
            IList<DBValue> items = new List<DBValue>();

            if (items.Count > 0)
            {
                AppSettings.isDraftTypeAuction = true;
            }
            else
            {
                AppSettings.isDraftTypeAuction = false;
            }

            return items.Count > 0;
        }
        public static async Task<double> auctionAmountAvailableForTeam(FantasyTeam fanTeam)
        {
            double result = 0;
            string query = string.Format(@"select 
                                                a1.auction_budget - ifnull(sum(a2.auction_value), 0) as value
                                            from user_leagues a1
                                            left join user_draft_assignments a2 on a1._id = a2.league_id
                                                and a2.team_id = {1}
                                            where a1._id = {0}
                                            group by a2.league_id, a2.team_id", fanTeam.leagueID, fanTeam.identifier);
            await Task.Delay(2000);
            //IList<doubleVal> items = await DBAdapter.executeQuery<doubleVal>(query);
            IList<doubleVal> items = new List<doubleVal>();

            if (items.Count() > 0)
            {
                result = items[0].value;
                return result;
            }
            else
            {
                return 0;
            }

        }
        public static async Task<double> auctionAmountSpentForTeam(FantasyTeam fanTeam)
        {
            double result = 0;
            string query = string.Format(@"select 
                                                ifnull(sum(a1.auction_value), 0) as value
                                            from user_draft_assignments a1
                                            where a1.league_id = {0}
                                                and a1.team_id = {1}", MyDraftEngine.Instance.league.identifier, fanTeam.identifier);
            await Task.Delay(2000);
            //IList<doubleVal> items = await DBAdapter.executeQuery<doubleVal>(query);
            IList<doubleVal> items = new List<doubleVal>();

            if (items.Count() > 0)
            {
                result = items[0].value;
                return result;
            }
            else
            {
                return 0;
            }

        }
        public static async Task<double> getMaxBid(int fanTeamID)
        {
            double result = 0;
            string query = string.Format(@"select 
                                                a1.auction_budget - (a1.num_rounds * a1.auction_player_min) - ifnull(sum(a2.auction_value),0) as value
                                            from user_leagues a1
                                            left join user_draft_assignments a2 on a1._id = a2.league_id
                                                and a2.team_id = {1}
                                            where a1._id = {0}
                                            group by a2.league_id, a2.team_id", MyDraftEngine.Instance.league.identifier, fanTeamID);
            await Task.Delay(2000);
            //IList<doubleVal> items = await DBAdapter.executeQuery<doubleVal>(query);
            IList<doubleVal> items = new List<doubleVal>();

            if (items.Count() > 0)
            {
                return result = items[0].value;
            }
            else
            {
                return 0;
            }
        }
        public static async Task auctionAssignPlayer(FantasyTeam fanTeam, Player player, string amount)
        {
            string query = string.Format(@"insert into user_draft_assignments 
                                                (league_id, team_id, player_id, is_keeper, auction_value) 
                                            values 
                                                ({0},{1},'{2}','false',{3})", fanTeam.leagueID, fanTeam.identifier, player.identifier, amount);
            await Task.Delay(2000);
            //await DBAdapter.executeUpdate(query);
        }
        public static async Task<IList<DraftPick>> auctionPicksForLeague(FantasyLeague fanleague)
        {
            List<DraftPick> result = new List<DraftPick>();
            
            StringBuilder query = new StringBuilder();
            query.Append(string.Format(@"SELECT * FROM {0} WHERE league_id = {1} order by timestamp asc", TABLE_USER_DRAFT_ASSIGN, fanleague.identifier));
            await Task.Delay(2000);
            //IList<DraftPick> items = await DBAdapter.executeQuery<DraftPick>(query.ToString());
            IList<DraftPick> items = new List<DraftPick>();

            foreach (DraftPick draftPick in items)
            {
                intOverall = intOverall + 1;
                result.Add(new DraftPick() { league = fanleague, leagueID = draftPick.leagueID, playerID = draftPick.playerID, overall = intOverall, isKeeper = draftPick.isKeeper, auctionValue = draftPick.auctionValue, teamID = draftPick.teamID });
            }
            intOverall = 0;
            return result;
        }
        public static async Task<int> auctionRosterCnt(FantasyTeam fanTeam)
        {
            ObservableCollection<DraftPick> _items;
            FantasyTeam _fanteam;
            _items = new ObservableCollection<DraftPick>();
            _fanteam = fanTeam;
            var picks = await DraftManager.auctionPicksForLeague(fanTeam.league);
            foreach (DraftPick team in picks)
            {
                _items.Add(team);
            }
            int result = (from team in _items
                          where team.teamID == _fanteam.identifier
                          select team).Count();
            return result;
        }
        #endregion // Auction //

        #region //  Mementos  //
        
        public static async Task clearDraftMementoUndoStack(int leagueID)
        {
            await Task.Delay(2000);
            //await DBAdapter.executeUpdate("DELETE FROM " + TABLE_USER_UNDO + " WHERE league_id = ?", leagueID);
        }
        public static async Task clearDraftMementoRedoStack(int leagueID)
        {
            await Task.Delay(2000);
            //await DBAdapter.executeUpdate("DELETE FROM " + TABLE_USER_REDO + " WHERE league_id = ?", leagueID);
        }
        #endregion //  Momentos  //

        #region // Config File //
        private static async Task<JsonObject> GetConfigFile()
        {

            Uri dataUri = new Uri("ms-appx:///MyDraftLib/Resources/config.json");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject jsonObject = JsonObject.Parse(jsonText);

            return jsonObject;
        }
        public static async Task<Boolean> checkSeasonActive()
        {
            bool bActiveStatus = true;
            JsonObject jsonObject = await GetConfigFile();
            JsonObject sourceObject = jsonObject.GetNamedObject(AppSettings.SportsLeague.ToString().ToUpper(), null);
            bActiveStatus = sourceObject.GetNamedBoolean("SeasonActiveStatus", true);

            return bActiveStatus;
        }
        
        public static async Task<List<Position>> getPositions()
        {
            List<Position> list = new List<Position>();

            //await Task.Delay(1000);
            list.Clear();
            try
            {
                JsonObject jsonObject = await GetConfigFile();

                JsonObject sourceObject = jsonObject.GetNamedObject(AppSettings.SportsLeague.ToUpper(), null);
                JsonArray posArray = sourceObject.GetNamedArray(String.Format("{0}Positions", AppSettings.SportsLeague.ToLower()), null);
                foreach (JsonValue groupValue in posArray)
                {
                    list.Add(new Position() { name = groupValue.GetString() });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace.ToString());
            }
            return list;
        }
        public static async Task<string[]> getNbrTeamsRounds()
        {
            string[] list = new string[20];
            int i = 0;
            //await Task.Delay(1000);
            //list.Clear();
            try
            {
                JsonObject jsonObject = await GetConfigFile();

                JsonObject sourceObject = jsonObject.GetNamedObject(AppSettings.SportsLeague.ToUpper(), null);
                JsonArray posArray = sourceObject.GetNamedArray("NumberTeamsRounds", null);
                foreach (JsonValue groupValue in posArray)
                {
                    list[i] = groupValue.GetString();
                    i = i + 1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace.ToString());
            }
            return list;
        }
        private class val
        {
            public String value { get; set; }

        }
        public static async Task<bool> CheckIDP()
        {
            string query = string.Format(@"select
                                                *
                                            from user_roster_config a1
                                            where a1.league_id = {0}
                                                and ((a1.position_key = 'DE1-DE2-DT1-DT2' and a1.starters > 0)
                                                or (a1.position_key = 'ILB1-ILB2-OLB1-OLB2' and a1.starters > 0)
                                                or (a1.position_key = 'CB1-CB2-FS-SS' and a1.starters > 0)
                                                or (a1.position_key = 'CB1-CB2-DE1-DE2-DT1-DT2-FS-ILB1-ILB2-OLB1-OLB2-SS' and a1.starters > 0))", MyDraftEngine.Instance.league.identifier);
            await Task.Delay(2000);
            //List<val> values = await DBAdapter.executeQuery<val>(query);
            List<val> values = new List<val>();

            return values.Count() > 0;

        }
        
        #endregion // Config File //
    }
}
