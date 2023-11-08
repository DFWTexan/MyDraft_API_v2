using Database.Model;
using DbData;
using Microsoft.EntityFrameworkCore;
using MyDraftAPI_v2.FantasyDataModel;
using System.Text;
using MyDraftAPI_v2.Managers;
using MyDraftAPI_v2.Engines;
using System.ComponentModel.DataAnnotations.Schema;
using MyDraftLib.Utilities;
using System.Diagnostics;
using MyDraftAPI_v2.FantasyDataModel.Draft;
using DraftService;
using MyDraftAPI_v2;
#pragma warning disable 

namespace LeagueService
{
    public class LeagueSvc
    {
        private readonly AppDataContext _db;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private UtilityService.Utility _utility;
        private DraftEngine_v2 _draftEngine;

        private static String TABLE_USER_TEAMS = "user_teams";
        private static String TABLE_USER_LEAGUES = "user_leagues";
        private static String TABLE_USER_ROSTER = "user_roster";
        public static readonly String TABLE_USER_ROSTER_CONFIG = "user_roster_config";

        public LeagueSvc(AppDataContext db, IConfiguration config, IWebHostEnvironment env, UtilityService.Utility utility, DraftEngine_v2 draftEngine)
        {
            _db = db;
            _config = config;
            _env = env;
            _utility = utility;
            _draftEngine = draftEngine;
        }

        private class Val
        {
            public String value { get; set; }
        }
        private class boolVal
        {
            [Column("value")]
            public int value { get; set; }
        }
        private class stringVal
        {
            [Column("value")]
            public String value { get; set; }
        }
        private class ValueID
        {
            [Column("player_id")]
            public String playerID { get; set; }
        }
        private class intVal
        {
            [Column("benchslots")]
            public int benchslots { get; set; }
        }
        private class TypeVal
        {
            [Column("type_value")]
            public String typeVal { get; set; }
        }

        private class DraftTypeVal
        {
            [Column("draft_type")]
            public int drafttypeVal { get; set; }
        }

        public DataModel.Response.ReturnResult GetActiveLeague(int? vID = 1)
        {
            var result = new DataModel.Response.ReturnResult();

            try
            {
                var activeLeague = _db.UserLeague.Take(1).OrderByDescending(q => q.LastActiveDate)
                    .Where(q => q.UniverseID == vID)
                    .Select(i => new ViewModel.ActiveLeague()
                    {
                        ID = i.ID,
                        UniverseID = i.UniverseID,
                        Name = i.Name,
                        Abbr = i.Abbr,
                        Mode = i.Mode,
                        DraftType = i.DraftType,
                        DraftOrder = i.DraftOrder,
                        NumberOfTeams = i.NumberOfTeams,
                        NumberOfRounds = i.NumberOfRounds
                    })
                    .FirstOrDefault();
                   
                var leagueTeams = _db.UserLeagueTeam.Where(q => q.LeagueID == activeLeague.ID).ToList();
                foreach(var i in leagueTeams)
                {
                    var addItem = new ViewModel.UserLeageTeamItem()
                    {
                        ID = i.ID,
                        Name = i.Name,
                        Abbreviation = i.Abbreviation,
                        DraftPosition = i.DraftPosition,
                        Owner = i.Owner
                    };

                    activeLeague.teams.Add(addItem);
                }

                result.ObjData = activeLeague;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrMessage = ex.Message;
            }
            return result;
        }

        public async Task<FantasyLeague> createLeague()
        {
            String LEAGUE_TEMP_NAME = "My League";
            String LEAGUE_TEMP_ABBR = "ML";
            int LEAGUE_TEMP_NUM_TEAMS = 12;
            int LEAGUE_TEMP_ROSTER_SIZE = 16;
            Boolean LEAGUE_TEMP_INCLUDE_IDP = false;
            Boolean LEAGUE_TEMP_DRAFT_BY_TEAM = true;
            FantasyLeague.DraftOrderType LEAGUE_TEMP_DRAFT_ORDER_TYPE = FantasyLeague.DraftOrderType.snake;
            int LEAGUE_TEMP_DRAFT_TYPE = 1;

            int uniqueIdentifier = await getMaxLeagueID() + 1;

            FantasyLeague leagueContainer = new FantasyLeague(0);
            leagueContainer.name = String.Format("{0} {1}", LEAGUE_TEMP_NAME, uniqueIdentifier);
            leagueContainer.abbr = String.Format("{0}{1}", LEAGUE_TEMP_ABBR, uniqueIdentifier);
            leagueContainer.numTeams = LEAGUE_TEMP_NUM_TEAMS;
            leagueContainer.rounds = LEAGUE_TEMP_ROSTER_SIZE;
            leagueContainer.isIncludeIDP = LEAGUE_TEMP_INCLUDE_IDP;
            leagueContainer.draftByTeamEnabled = LEAGUE_TEMP_DRAFT_BY_TEAM;
            leagueContainer.draftOrderType = LEAGUE_TEMP_DRAFT_ORDER_TYPE;
            leagueContainer.draftType = LEAGUE_TEMP_DRAFT_TYPE;

            //await Task.Delay(2000);
            //Boolean success = await DBAdapter.executeUpdate("INSERT INTO " + TABLE_USER_LEAGUES + " (name, abbr, num_teams, num_rounds, draft_type, draft_by_team, include_idp, site_id, source) " +
            // "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)",
            // leagueContainer.name, leagueContainer.abbr, leagueContainer.numTeams, leagueContainer.rounds, leagueContainer.draftType, leagueContainer.draftByTeamEnabled, leagueContainer.isIncludeIDP, leagueContainer.siteID, leagueContainer.source);
            //Boolean success = true;

            var newLeague = new UserLeague
            {
                Name = leagueContainer.name,
                Abbr = leagueContainer.abbr,
                NumberOfTeams = leagueContainer.numTeams,
                NumberOfRounds = leagueContainer.rounds,
                DraftType = leagueContainer.draftType,
                DraftOrder = leagueContainer.draftOrderType.ToString(),
            };
            _db.UserLeague.Add(newLeague);
            _db.SaveChanges();

            //if (!success)
            //    return null;

            int leagueID = await getMaxLeagueID();
            var resultObj = await Task.Run(() => createTeams(leagueID, LEAGUE_TEMP_NUM_TEAMS));
            FantasyLeague league = await getLeagueWithID(leagueID);

            if (league.draftByTeamEnabled)
            {
                IList<DraftPick> draftPicks = (IList<DraftPick>)DraftPickGenerator.generateDraftPicks(league);
                await DraftManager.saveDraftPicks(draftPicks);
            }

            return league;
        }

        public async Task<DataModel.Response.ReturnResult> createTeams(int leagueId, int numTeams)
        {
            var result = new DataModel.Response.ReturnResult();
            try
            {
                int userTeamID = -1;
                for (int i = 0; i < numTeams; i++)
                {
                    String name;
                    String abbr;
                    String owner;
                    if (i == 0)
                    {
                        name = "My Team";
                        abbr = "MY";
                        owner = "Me";
                    }
                    else
                    {
                        int teamNameIdentifier = i + 1;
                        name = String.Format("Team {0}", teamNameIdentifier);
                        abbr = String.Format("TM{0}", teamNameIdentifier);
                        owner = String.Format("Owner {0}", teamNameIdentifier);
                    }

                    //connection.Execute("INSERT INTO " + TABLE_USER_TEAMS + " (league_id, name, abbr, owner, draft_position) VALUES (?, ?, ?, ?, ?)",
                    //    leagueID, name, abbr, owner, i + 1);
                    var newLeagueTeam = new UserLeagueTeams
                    {
                        LeagueID = leagueId,
                        Name = name,
                        Abbreviation = abbr,
                        Owner = owner,
                        DraftPosition = i + 1
                    };
                    _db.UserLeagueTeam.Add(newLeagueTeam);
                    _db.SaveChanges();

                    if (i == 0)
                    {
                        userTeamID = await maxTeamID();
                        //connection.Execute("UPDATE " + TABLE_USER_LEAGUES + " SET user_team_id = ? WHERE _id = ?", userTeamID, leagueID);
                        var league = _db.UserLeague.Where(q => q.ID == leagueId).FirstOrDefault();
                        league.ID = userTeamID;
                        _db.SaveChanges();

                    }
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrMessage = ex.Message;
            }
            return result;
        }

        public async Task<IList<FantasyTeam>> teamsForLeague(FantasyLeague league)
        {
            //await Task.Delay(2000);
            //IList<FantasyTeam> teams = await DBAdapter.executeQuery<FantasyTeam>("SELECT * FROM " + TABLE_USER_TEAMS + " WHERE league_id = ? ORDER BY draft_position ASC", league.identifier);
            //IList<FantasyTeam> teams = new List<FantasyTeam>();
            IList<FantasyTeam> teams = (IList<FantasyTeam>)await _db.UserLeagueTeam.Where(q => q.LeagueID == league.identifier).OrderBy(q => q.DraftPosition).ToListAsync();

            foreach (FantasyTeam team in teams)
            {
                team.league = league;
                //team.auctionRosterCount = await team.getAuctionRosterCount();
                //team.budgetAmount = await team.getAuctionAmountAvailable();
            }

            return teams;
        }
        public DataModel.Response.ReturnResult TeamsForLeague(int leagueID)
        {
            var result = new DataModel.Response.ReturnResult();
            try
            {
                var teams = _db.UserLeagueTeam
                            .Where(q => q.LeagueID == leagueID)
                            .OrderBy(q => q.DraftPosition)
                            .Select(i => new FantasyTeam()
                            {
                                identifier = i.ID,
                                leagueID = i.LeagueID,
                                name = i.Name ?? "",
                                abbr = i.Abbreviation ?? "",
                                draftPosition = i.DraftPosition,
                                owner = i.Owner ?? ""
                            })
                            .AsNoTracking()
                            .ToList();

                result.ObjData = teams.ToList();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrMessage = ex.Message;
            }

            return result;
        }

        public async Task<FantasyLeague> getLeagueWithID(int identifier)
        {
            //await Task.Delay(2000);
            //IList<FantasyLeague> values = await DBAdapter.executeQuery<FantasyLeague>("SELECT * FROM " + TABLE_USER_LEAGUES + " WHERE _id = ?", identifier);
            //IList<FantasyLeague> values = new List<FantasyLeague>();
            IList<FantasyLeague> values = (IList<FantasyLeague>)await _db.UserLeague.Where(q => q.ID == identifier).ToListAsync();

            FantasyLeague league = null;
            if (values.Count() > 0)
            {
                league = values[0];
                league.teams = await teamsForLeague(league);
                await league.initializeAsyncValues();
            }

            return league;
        }

        public static async Task<IList<string>> getLeagueIDs()
        {
            await Task.Delay(2000);
            //IList<Val> values = await DBAdapter.executeQuery<Val>("SELECT _id AS value FROM " + TABLE_USER_LEAGUES + " ORDER BY _id ASC");
            IList<Val> values = new List<Val>();

            IList<string> leagueIDs = new List<string>();
            foreach (Val value in values)
            {
                leagueIDs.Add(value.value);
            }

            return leagueIDs;
        }

        public static async Task<Boolean> deleteLeague(FantasyLeague league)
        {
            Boolean success = false;

            try
            {
                await Task.Delay(2000);
                //await DBAdapter.dbAPP.RunInTransactionAsync((SQLiteConnection connection) =>
                //{
                //    connection.Execute("DELETE FROM " + TABLE_USER_LEAGUES + " WHERE _id = ?",
                //            league.identifier);

                //    connection.Execute("DELETE FROM " + TABLE_USER_TEAMS + " WHERE league_id = ?",
                //                league.identifier);
                //});
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception (deleteLeague) : " + ex.ToString());
            }

            return success;
        }

        public static async Task updateLeague(FantasyLeague league)
        {
            string query = string.Format(@"UPDATE {0} SET 
                                                name = '{1}'
                                                , abbr = '{2}'
                                                , num_rounds = {3}
                                                , combine_wrte = '{4}'
                                                , include_idp = '{5}'
                                                , num_teams = {6}
                                                , site_id = {7}
                                                , draft_type = {8}
                                                , draft_order = '{9}' 
                                                , league_mode = {11}
                                                , mock_draft = {12}
                                                , position_limit_enabled = {13}
                                            WHERE _id = {10}", TABLE_USER_LEAGUES, league.name, league.abbr, league.rounds, league.isCombineWRTE,
                                            league.isIncludeIDP, league.numTeams, league.siteID, league.draftType, league.draftOrderType, league.identifier, league.leagueMode, league.mockDraft, league.isPositionLimitEnabled);

            await Task.Delay(2000);
            //await DBAdapter.executeUpdate(query);

        }

        public async Task<int> getMaxLeagueID()
        {
            IList<Val> values = (IList<Val>)await Task.Run(() => _db.UserLeague.OrderByDescending(s => s.ID).Select(q => q.ID).ToListAsync());

            int result = 0;
            if (values.Count() > 0)
            {
                result = Convert.ToInt32(values[0].value);
            }

            return result;
        }

        public static async Task<Boolean> isOtherLeagues()
        {
            await Task.Delay(2000);
            //IList<Val> results = await DBAdapter.executeQuery<Val>("SELECT COUNT(*) AS value FROM user_leagues");
            IList<Val> results = new List<Val>();

            Boolean result = Convert.ToInt32(results[0].value) > 1;
            return result;
        }

        public static async Task<Boolean> checkPositionLimits(FantasyLeague league)
        {
            await Task.Delay(2000);
            //IList<Val> results = await DBAdapter.executeQuery<Val>(string.Format("select * from user_roster_config where maximum > 0 and league_id = {0}", league.identifier));
            IList<Val> results = new List<Val>();

            Boolean result = results.Count() > 0;
            return result;
        }

        public static async Task<Boolean> isPositionLimitEnabled(FantasyLeague league)
        {
            bool result = false;
            await Task.Delay(2000);
            //IList<boolVal> results = await DBAdapter.executeQuery<boolVal>(string.Format("SELECT position_limit_enabled AS value FROM user_leagues where _id = {0}", league.identifier));
            IList<boolVal> results = new List<boolVal>();

            if (results[0].value == 1)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public async Task<FantasyTeam> createTeamFromTeam(FantasyTeam team, FantasyLeague league)
        {
            //await Task.Delay(2000);
            //await DBAdapter.executeUpdate("INSERT INTO " + TABLE_USER_TEAMS + " (name, abbr, league_id) VALUES (?, ?, ?)",
            //        team.name, team.abbr, league.identifier);

            int lastInsertID = await maxTeamID();
            FantasyTeam newTeam = new FantasyTeam();
            //FantasyTeam newTeam = await getTeamWithID(Convert.ToString(lastInsertID), league);
            return newTeam;
        }

        public async Task<int> maxTeamID()
        {
            IList<Val> values = (IList<Val>)await Task.Run(() => _db.UserLeagueTeam.OrderByDescending(s => s.ID).Select(q => q.ID).ToListAsync());

            int result = 0;
            if (values.Count() > 0)
            {
                result = Convert.ToInt32(values[0].value);
            }
            return result;
        }

        public async Task<FantasyTeam> getTeamWithID(String teamID, FantasyLeague league)
        {
            await Task.Delay(2000);
            //IList<FantasyTeam> values = await DBAdapter.executeQuery<FantasyTeam>("SELECT * FROM " + TABLE_USER_TEAMS + " WHERE _id = ?",
            //                         teamID);
            IList<FantasyTeam> values = new List<FantasyTeam>();

            FantasyTeam team = null;
            if (values.Count() > 0)
            {
                team = values[0];

                if (league == null)
                {
                    league = await getLeagueWithID(team.leagueID);
                }

                team.league = league;
            }

            return team;
        }

        //public static async Task updateTeam(FantasyTeam team)
        //{
        //    await DBAdapter.executeUpdate("UPDATE " + TABLE_USER_TEAMS + " SET name = ?, abbr = ?, draft_position = ?, owner = ? WHERE _id = ?",
        //     team.name, team.abbr, team.draftPosition, team.owner, team.identifier);
        //}

        public static async Task deleteTeam(FantasyTeam team)
        {
            FantasyLeague league = team.league;
            await Task.Delay(2000);
            //await DBAdapter.executeUpdate("DELETE FROM " + TABLE_USER_TEAMS + " WHERE _id = ?", team.identifier);
            league.teams.Remove(team);
            await DraftManager.resetDraftData(league);
        }

        public async Task<FantasyTeam> createTeamForLeague(FantasyLeague league)
        {
            FantasyTeam lastTeam = league.teams.Last();
            int nextDraftPosition = lastTeam.draftPosition + 1;
            String name = String.Format("Team {0}", nextDraftPosition);
            String abbr = String.Format("TM{0}", nextDraftPosition);
            String owner = String.Format("Owner {0}", nextDraftPosition);

            //await Task.Delay(2000);
            //bool success = await DBAdapter.executeUpdate("INSERT INTO " + TABLE_USER_TEAMS + " (league_id, name, abbr, owner, draft_position) VALUES (?, ?, ?, ?, ?)",
            //    league.identifier, name, abbr, owner, nextDraftPosition);
            bool success = true;
            if (success)
            {
                int nextTeamID = await maxTeamID();
                FantasyTeam team = await getTeamWithID(Convert.ToString(nextTeamID), league);
                league.teams.Add(team);

                await DraftManager.resetDraftData(league);
                return team;
            }
            else
            {
                return null;
            }
        }

        public async Task updateTeamsForLeague(FantasyLeague league)
        {
            if (!league.draftByTeamEnabled)
            {
                return;
            }

            List<FantasyTeam> teams = new List<FantasyTeam>(league.teams);
            int newTeamCount = league.numTeams;
            if (teams.Count() == newTeamCount)
            {
                return;
            }

            if (teams.Count() > newTeamCount)
            {
                // The user decreased the number of teams
                int startIndex = newTeamCount;
                int length = teams.Count() - newTeamCount;
                List<FantasyTeam> subArray = teams.GetRange(startIndex, length);

                foreach (FantasyTeam team in subArray)
                {
                    await deleteTeam(team);
                }
                teams.RemoveRange(startIndex, length);
            }
            else
            {
                // The user increased the number of teams
                int length = newTeamCount - teams.Count();
                for (int i = 0; i < length; i++)
                {
                    teams.Add(await createTeamForLeague(league));
                }
            }

            league.teams = teams;

            if (league.myTeam == null)
            {
                FantasyTeam team = league.teams.First<FantasyTeam>();
                league.userTeamID = team.identifier;
                await updateLeague(league);
            }
        }

        //public static async Task setMyTeam(FantasyTeam team, FantasyLeague league)
        //{
        //    await DBAdapter.executeUpdate("UPDATE " + TABLE_USER_LEAGUES + " SET user_team_id = ? WHERE _id = ?",
        //            team.identifier, league.identifier);
        //}

        public static async Task<int> getDraft_Type()
        {
            int result = 0;
            string query = string.Format(@"select distinct 
                                            a1.draft_type 
                                        from user_leagues a1
                                        where _id  = {0}", MyDraftEngine.Instance.league.identifier);
            await Task.Delay(2000);
            //List<DraftTypeVal> values = await DBAdapter.executeQuery<DraftTypeVal>(query.ToString());
            List<DraftTypeVal> values = new List<DraftTypeVal>();

            var item = values.FirstOrDefault();
            result = item.drafttypeVal;
            if (result == 1)
            {
                AppSettings.draftType = "Standard";
            }
            else
            {
                AppSettings.draftType = "Auction";
            }
            return result;
        }

        public static async Task<string> getDraft_Order()
        {
            string result = string.Empty;
            string query = string.Format(@"select distinct 
                                            a1.draft_order as value
                                        from user_leagues a1
                                        where _id  = {0}", MyDraftEngine.Instance.league.identifier);
            await Task.Delay(2000);
            //List<stringVal> values = await DBAdapter.executeQuery<stringVal>(query.ToString());
            List<stringVal> values = new List<stringVal>();

            var item = values.FirstOrDefault();
            result = item.value;

            return result;
        }

        public static async Task<FantasyTeam> getMyTeamForLeague(FantasyLeague league)
        {
            await Task.Delay(2000);
            //IList<FantasyTeam> values = await DBAdapter.executeQuery<FantasyTeam>("SELECT T._id, T.name, T.abbr FROM " + TABLE_USER_TEAMS + " as T, " + TABLE_USER_LEAGUES + " as L WHERE T.league_id = ? AND L.user_team_id = T._id",
            //                         league.identifier);
            IList<FantasyTeam> values = new List<FantasyTeam>();

            FantasyTeam team = null;

            if (values.Count() > 0)
            {
                team = values[0];
            }

            return team;
        }

        public async Task<IList<FantasyTeam>> getTeamsForPlayerID(String playerID)
        {
            await Task.Delay(2000);
            //IList<int> values = await DBAdapter.executeQuery<int>("SELECT team_id AS value FROM " + TABLE_USER_ROSTER + " WHERE player_id = ?",
            //        playerID);
            IList<int> values = new List<int>();

            IList<FantasyTeam> teams = new List<FantasyTeam>();
            foreach (int value in values)
            {
                String teamID = Convert.ToString(value);
                FantasyTeam team = await getTeamWithID(teamID, null);
                teams.Add(team);
            }

            return teams;
        }

        //public static async Task addPlayerToTeam(String playerID, FantasyTeam team, Boolean isKeeper)
        //{
        //    await DBAdapter.executeUpdate("INSERT OR REPLACE INTO " + TABLE_USER_ROSTER + " (player_id, team_id, keeper) VALUES (?, ?, ?)",
        //            playerID, team.identifier, isKeeper);
        //}

        //public static async Task removePlayerID(String playerID, int teamID)
        //{
        //    await DBAdapter.executeUpdate("DELETE FROM " + TABLE_USER_ROSTER + " WHERE player_id = ? AND team_id = ?",
        //            playerID, teamID);
        //}

        public static async Task<IList<String>> getPlayerIDsForTeam(FantasyTeam team)
        {
            return await getPlayerIDsForTeam(team, PlayerManager.HealthStatus.HealthStatusAll);
        }

        public static async Task<IList<String>> getPlayerIDsForTeam(FantasyTeam team, PlayerManager.HealthStatus status)
        {
            await Task.Delay(2000);
            //IList<Val> values = await DBAdapter.executeQuery<Val>("SELECT player_id AS value from user_draft_selections WHERE league_id = ? AND team_id = ? AND player_id NOT NULL " +
            //                 " UNION " +
            //                 " SELECT player_id FROM user_draft_assignments WHERE league_id = ? AND team_id = ?",
            //                 team.league.identifier, team.identifier, team.league.identifier, team.identifier);
            IList<Val> values = new List<Val>();

            IList<String> playerIDs = new List<String>();
            foreach (Val value in values)
            {
                String playerID = value.value;
                playerIDs.Add(playerID);
            }

            return await PlayerManager.getPlayerIDsWithHealthStatus(playerIDs, status);
        }

        public static async Task<int> getBenchSlots(FantasyLeague league)
        {
            int result = 0;
            string query = string.Format(@"select
                                                (a1.num_rounds - sum(a2.starters)) as benchslots
                                            from user_leagues a1
                                            join user_roster_config a2 on a1._id = a2.league_id
                                            where a1._id = {0}", league.identifier);
            await Task.Delay(2000);
            //List<intVal> values = await DBAdapter.executeQuery<intVal>(query.ToString());
            List<intVal> values = new List<intVal>();

            var item = values.FirstOrDefault();
            result = item.benchslots;

            return result;
        }
        public static async Task<String> getScoreTypeValue(string posKey)
        {
            string result = string.Empty;
            string query = string.Format(@"select
                                                ifnull(a1.type_value, 'Misc.') as type_value
                                            from stat_map a1
                                            where a1.stat_key = '{0}'", posKey.ToUpper());
            await Task.Delay(2000);
            //List<TypeVal> values = await DBAdapter.executeQuery<TypeVal>(query.ToString());
            List<TypeVal> values = new List<TypeVal>();

            if (values.Count() > 0)
            {
                var item = values.FirstOrDefault();
                result = item.typeVal;
            }
            return result;
        }

        #region // Auction //

        #endregion // Auction //

        #region Custom Scoring
        public static async Task saveCustomScoring(FantasyLeague league)
        {
            IList<CustomScoringUserItem> customScoring = await league.getScoringValues();
            foreach (CustomScoringUserItem item in customScoring)
            {
                await LeagueManager.saveCustomScoringUserItem(item.getValue(), item.getKey(), item.getPosition(), league);
            }

            // Refresh the active fantasy league if it's he same as the league being updated
            //        if (AppSettings.getActiveFantasyLeague().getIdentifier().equals(league.getIdentifier()))
            //            AppSettings.getActiveFantasyLeague().setScoringValues(customScoring);
        }

        public static async Task<IList<CustomScoringUserItem>> getCustomScoringValuesForLeague(FantasyLeague league)
        {
            await Task.Delay(2000);
            //List<CustomScoringUserItem> items = await DBAdapter.executeQuery<CustomScoringUserItem>("SELECT position, key, value FROM user_custom_scoring WHERE league_id = ?", league.identifier);
            List<CustomScoringUserItem> items = new List<CustomScoringUserItem>();

            foreach (CustomScoringUserItem item in items)
                item.setLeagueID(league.identifier);

            return items;
        }

        public static async Task saveCustomScoringUserItem(float value, String key, String position, FantasyLeague league)
        {
            if (position == null)
                position = "ALL";

            //if (value == 0)
            //{
            //    LeagueManager.deleteCustomScoringUserItem(key, position, league);
            //    return;
            //}

            await Task.Delay(2000);
            //await DBAdapter.executeUpdate("INSERT OR REPLACE INTO user_custom_scoring (league_id, position, key, value) VALUES (?, ?, ?, ?)",
            //        league.identifier, position, key, value);
        }

        public static async Task saveMinStarterMax(PositionItem positionItem, int newValue, string column)
        {
            string query = string.Format(@"update {0} set 
                                                {4} = {1} 
                                            where league_id = {2}
                                                and position_key = '{3}'", TABLE_USER_ROSTER_CONFIG, newValue, positionItem.leagueID, positionItem.positionKey, column);
            await Task.Delay(2000);
            //await DBAdapter.executeUpdate(query);
        }

        public static async Task<IList<String>> customScoringGroupIDs(FantasyLeague league)
        {
            await Task.Delay(2000);
            //IList<Val> values = await DBAdapter.executeQuery<Val>("SELECT group_id AS value FROM user_custom_scoring_by_position WHERE league_id = ?",
            //        league.identifier);
            IList<Val> values = new List<Val>();

            IList<string> result = new List<string>();
            foreach (Val value in values)
            {
                result.Add(value.value);
            }

            return result;
        }

        //public static async Task setCustomScoringEnabledForGroup(Boolean enabled, CustomScoringGroup group, FantasyLeague league)
        //{
        //    if (enabled)
        //        await DBAdapter.executeUpdate("INSERT OR REPLACE INTO user_custom_scoring_by_position (league_id, group_id) VALUES (?, ?)",
        //                league.identifier, group.getIdentifier());
        //    else
        //        await DBAdapter.executeUpdate("DELETE FROM user_custom_scoring_by_position WHERE league_id = ? AND group_id = ?",
        //                league.identifier, group.getIdentifier());
        //}

        public static async Task<Boolean> isCustomScoringEnabledForGroup(CustomScoringGroup group, FantasyLeague league)
        {
            await Task.Delay(2000);
            //IList<Val> results = await DBAdapter.executeQuery<Val>("SELECT COUNT(*) AS value FROM user_custom_scoring_by_position WHERE league_id = ? AND group_id = ?",
            //        league.identifier, group.getIdentifier());
            IList<Val> results = new List<Val>();

            Boolean result = Convert.ToInt32(results[0].value) > 0;
            return result;
        }


        // Delete all calculated points for the league. This ignores the 'tag' field and deletes all entries.
        //public static async Task clearAllPointsForLeagueID(int leagueID)
        //{
        //    await DBAdapter.executeUpdate("DELETE FROM points WHERE league_id = ?", leagueID);
        //    await UpdateManager.resetTimeStampForPointsWithLeagueID(leagueID);
        //}

        public static async Task savePlayerPoints(IDictionary<String, float> dict, int year, int segment, String leagueID)
        {
            await Task.Delay(2000);
            //await LeagueManager.savePlayerPoints(dict, year, segment, leagueID, "default");
        }

        public static async Task savePlayerPoints(IDictionary<String, float> dict, int year, int segment, String leagueID, String tag)
        {
            try
            {
                ICollection<String> playerIDs = dict.Keys;
                if (playerIDs.Count() == 0)
                    return;

                double timestamp = TNUtility.DateTimeToUnixTimestamp(DateTime.Now);

                await Task.Delay(2000);
                //await DBAdapter.dbAPP.RunInTransactionAsync((SQLiteConnection connection) =>
                //{
                //    // Clear out the old points since they are now out of date
                //    connection.Execute("DELETE FROM points WHERE league_id = ? AND year = ? AND segment = ? AND tag = ?",
                //        leagueID, year, segment, tag);

                //    connection.Execute("INSERT OR REPLACE INTO points_updates (league_id, year, segment, timestamp, tag) VALUES (?, ?, ?, ?, ?) ",
                //                    leagueID, year, segment, timestamp, "default");

                //    foreach (String playerID in playerIDs)
                //    {
                //        int success = connection.Execute("INSERT OR REPLACE INTO points (player_id, points, year, segment, league_id, tag) VALUES (?, ?, ?, ?, ?, ?)",
                //                        playerID, dict[playerID], year, segment, leagueID, tag);
                //        //Debug.WriteLine("INSERT OR REPLACE INTO points (player_id, points, year, segment, league_id, tag) VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                //        //                playerID, dict[playerID], year, segment, leagueID, tag);
                //    }
                //});

                Debug.WriteLine("POINTS_SAVED");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception (savePlayerPoints): " + ex.ToString());
            }
        }

        public static async Task savePlayerVBD(IList<IDictionary<String, Object>> items, int year, int segment, int leagueID)
        {
            if (items.Count == 0)
                return;

            ((List<IDictionary<String, Object>>)items).Sort(delegate (IDictionary<String, Object> x, IDictionary<String, Object> y)
            {
                float lhValue = (float)x["value"];
                float rhValue = (float)y["value"];
                return (int)(rhValue * 10 - lhValue * 10); // Multiply by 10 to make sure fractional difference is maintained
            });

            await Task.Delay(2000);
            //await DBAdapter.dbAPP.RunInTransactionAsync((SQLiteConnection connection) =>
            //{

            //    // Clear out the old points since they are now out of date
            //    connection.Execute("DELETE FROM dvbd WHERE league_id = ? AND year = ? AND segment = ? ",
            //            leagueID, year, segment);

            //    String rankingsSource = "dvbd";
            //    connection.Execute("DELETE FROM rankings WHERE league_id = ? AND year = ? AND segment = ? AND source = ? ",
            //            leagueID, year, segment, rankingsSource);

            //    int rank = 1;
            //    foreach (IDictionary<String, Object> playerData in items)
            //    {
            //        String playerID = (String)playerData["player_id"];
            //        float value = (float)playerData["value"];
            //        int success1 = connection.Execute(
            //                "INSERT OR REPLACE INTO dvbd (player_id, value, year, segment, league_id) VALUES (?, ?, ?, ?, ?)",
            //                        playerID, value, year, segment, leagueID);

            //        int success2 = connection.Execute(
            //                "INSERT OR REPLACE INTO rankings (player_id, rank, year, segment, league_id, source) VALUES (?, ?, ?, ?, ?, ?)",
            //                playerID, rank++, year, segment, leagueID, rankingsSource);

            //        //if (!success1 || !success2)
            //        //{
            //        //    DBAdapter.endTransaction();
            //        //    return;
            //        //}
            //    }
            //});
        }

        #endregion //  Custom Scoring  //

        #region //  Roster  /
        private class RosterVal
        {
            [Column("position_key")]
            public String positionKey { get; set; }
            public int starters { get; set; }
            public int maximum { get; set; }
            public int flex { get; set; }
            [Column("keyAbbr")]
            public String keyAbbr { get; set; }
            [Column("keyFull")]
            public String keyFull { get; set; }
            [Column("sortValue")]
            public int sortValue { get; set; }
            [Column("typeValue")]
            public String typeValue { get; set; }
        }
        private static RosterVal _rosterConfig;
        private static async Task<RosterVal> getRosterReference(string posKey)
        {
            //string query = string.Format(@"select
            //                                keyAbbr
            //                                ,keyFull
            //                                ,sortValue
            //                                ,typeValue
            //                            from roster_config_reference
            //                            where positionKey = '{0}'", posKey);
            //IList<RosterVal> item = await DBAdapter.executeQuery<RosterVal>(query);
            IList<RosterVal> item = new List<RosterVal>();

            return item.FirstOrDefault();
        }
        public static async Task saveRosterLimits(FantasyLeague league)
        {
            IDictionary<String, IDictionary<String, int>> rosterLimits = league.getRosterValues();
            foreach (KeyValuePair<String, IDictionary<String, int>> entry in rosterLimits)
            {
                String positionKey = entry.Key;
                _rosterConfig = await getRosterReference(positionKey);
                IDictionary<String, int> values = entry.Value;
                int starters = values.ContainsKey("starters") ? values["starters"] : -1; ;
                int max = values.ContainsKey("max") ? values["max"] : -1;
                int flex = values.ContainsKey("flex") ? values["flex"] : 0;

                await Task.Delay(2000);
                //await DBAdapter.executeUpdate("INSERT OR REPLACE INTO user_roster_config (league_id, position_key, starters, maximum, flex, key_abbr, key_full, sort_value, type_value) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)",
                //        league.identifier, positionKey, starters, max, flex, _rosterConfig.keyAbbr, _rosterConfig.keyFull, _rosterConfig.sortValue, _rosterConfig.typeValue);
            }
        }
        public static async Task<IDictionary<String, IDictionary<String, int>>> getRosterLimits(String leagueID)
        {
            IDictionary<String, IDictionary<String, int>> rosterValues = new Dictionary<String, IDictionary<String, int>>();

            await Task.Delay(2000);
            //IList<RosterVal> items = await DBAdapter.executeQuery<RosterVal>(string.Format("SELECT * FROM user_roster_config WHERE league_id = {0}", leagueID));
            IList<RosterVal> items = new List<RosterVal>();

            foreach (RosterVal item in items)
            {
                IDictionary<String, int>? sectionMap = rosterValues.ContainsKey(item.positionKey) ? rosterValues[item.positionKey] : null;
                if (sectionMap == null)
                {
                    sectionMap = new Dictionary<String, int>();
                    rosterValues.Add(item.positionKey, sectionMap);
                }

                sectionMap.Add("starters", item.starters);
                sectionMap.Add("max", item.maximum);
                sectionMap.Add("flex", item.flex);
                /* new items */
                sectionMap.Add("keyAbbr", item.starters);
                sectionMap.Add("sortValue", item.maximum);
                sectionMap.Add("typeValue", item.flex);
            }

            return rosterValues;
        }
        public static async Task<List<LineupItem>> getLineup(FantasyTeam fanteam)
        {
            List<LineupItem> list = new List<LineupItem>();
            string position = string.Empty;
            int benCnt = 0;
            list.Clear();
            var rosterConfig = await LeagueManager.getRosterConfig(MyDraftEngine.Instance.league.identifier.ToString());
            foreach (var itm in rosterConfig)
            {
                switch (itm.positionKey)
                {
                    case "CB1-CB2-DE1-DE2-DT1-DT2-FS-ILB1-ILB2-OLB1-OLB2-SS":
                        itm.positionKey = "CB-DE-DT-FS-ILB-OLB-SS";
                        break;
                    case "DE1-DE2-DT1-DT2":
                        itm.positionKey = "DE-DT";
                        break;
                    case "ILB1-ILB2-OLB1-OLB2":
                        itm.positionKey = "ILB-OLB";
                        break;
                    case "CB1-CB2-FS-SS":
                        itm.positionKey = "CB-FS-SS";
                        break;
                    default:
                        break;
                }

                string playerID = string.Empty;
                List<string> starterIDs = new List<string>();
                starterIDs = await getPositionStarters(fanteam, itm.positionKey, itm.value);
                List<string> benchIDs = new List<string>();
                benchIDs = await getRosterBench(fanteam, itm.positionKey, itm.value);
                position = itm.positionKey;
                if (itm.positionKey == "CB-DE-DT-FS-ILB-OLB-SS")
                {
                    position = "IDP";
                }
                else if (itm.positionKey == "FB-RB")
                {
                    position = "RB";
                }
                else if (itm.positionKey == "ILB-OLB")
                {
                    position = "LB";
                }
                else if (itm.positionKey == "DE-DT")
                {
                    position = "DL";
                }
                else if (itm.positionKey == "CB-FS-SS")
                {
                    position = "DB";
                }

                /* Starters */
                for (int i = 0; i < itm.value; i++)
                {
                    if (i < starterIDs.Count())
                    {
                        playerID = starterIDs[i];
                    }
                    else
                    {
                        playerID = null;
                    }

                    list.Add(new LineupItem() { player_id = playerID, position = position, starters = itm.value, sortValue = itm.sortValue });
                }
                /* Bench */
                for (int i = 0; i < AppSettings.benchSlots; i++)
                {
                    if (i < benchIDs.Count())
                    {
                        playerID = benchIDs[i];
                        list.Add(new LineupItem() { player_id = playerID, position = "Bench", starters = 0, sortValue = 99 });
                        benCnt = benCnt + 1;
                    }
                }
            }
            // Complete Empty Bench slots
            if (benCnt < AppSettings.benchSlots)
            {
                int fill = AppSettings.benchSlots - benCnt;
                for (int i = 1; i <= fill; i++)
                {
                    list.Add(new LineupItem() { player_id = null, position = "Bench", starters = 0, sortValue = 99 });
                }
            }
            return list;
        }
        public static async Task<List<string>> getPositionStarters(FantasyTeam fanteam, string position, int starterCount)
        {
            /* Used to complete LineupItem w/ PlayerID */
            List<string> playerIDs = new List<string>();
            string query_table;
            if (!AppSettings.isDraftTypeAuction)
            {
                query_table = "user_draft_selections";
            }
            else
            {
                query_table = "user_draft_assignments";
            }
            string query = String.Format(@"select distinct
                                                a1.player_id
                                            from {4} a1
                                            join players a2 on a1.player_id = a2.player_id
                                            left join points a3 on a1.player_id = a3.player_id
                                            where a1.league_id = {0}
                                                and a1.team_id = {1}
                                                and a2.position in ('{2}')
                                            order by a3.points desc
                                            limit {3}", MyDraftEngine.Instance.league.identifier, fanteam.identifier, position.Replace("-", "','"), starterCount, query_table);
            await Task.Delay(2000);
            //List<ValueID> values = await DBAdapter.executeQuery<ValueID>(query.ToString());

            List<ValueID> values = new List<ValueID>();

            foreach (ValueID value in values)
            {
                playerIDs.Add(value.playerID);
            }

            return playerIDs;
        }
        private static async Task<List<string>> getRosterBench(FantasyTeam fanteam, string position, int starterCount)
        {
            List<string> playerIDs = new List<string>();
            string query_table;
            if (!AppSettings.isDraftTypeAuction)
            {
                query_table = "user_draft_selections";
            }
            else
            {
                query_table = "user_draft_assignments";
            }
            string query = String.Format(@"select distinct
                                                a1.player_id 
                                                from {4} a1
                                            join players a2 on a1.player_id = a2.player_id
                                            left join points a3 on a1.player_id = a3.player_id
                                            left join (select distinct
                                                        b1.player_id
                                                    from {4} b1
                                                    join players a2 on b1.player_id = a2.player_id
                                                    left join points a3 on b1.player_id = a3.player_id
                                                    where b1.league_id = {0}
                                                        and b1.team_id = {1}
                                                        and a2.position in ('{2}')
                                                    order by a3.points desc
                                                    limit {3}    
                                            ) a4 on a1.player_id = a4.player_id
                                            where a1.league_id = {0}
                                                and a1.team_id = {1}
                                                and a2.position in ('{2}')
                                                and a4.player_id is null
                                            order by a3.points desc", MyDraftEngine.Instance.league.identifier, fanteam.identifier, position.Replace("-", "','"), starterCount, query_table);
            await Task.Delay(2000);
            //List<ValueID> values = await DBAdapter.executeQuery<ValueID>(query.ToString());
            List<ValueID> values = new List<ValueID>();

            foreach (ValueID value in values)
            {
                playerIDs.Add(value.playerID);
            }

            return playerIDs;
        }
        public static async Task<List<PositionItem>> getRosterList()
        {
            List<PositionItem> result = new List<PositionItem>();
            string query = String.Format(@"select *
                                            from {0}
                                            where league_id = {1}
                                                and starters > 0
                                            order by sort_value asc", TABLE_USER_ROSTER_CONFIG, MyDraftEngine.Instance.league.identifier);
            await Task.Delay(2000);
            //List<PositionItem> values = await DBAdapter.executeQuery<PositionItem>(query.ToString());
            List<PositionItem> values = new List<PositionItem>();

            foreach (PositionItem value in values)
            {
                result.Add(value);
            }

            return result;
        }
        public static async Task<List<PositionItem>> getRosterList(FantasyLeague league)
        {
            List<PositionItem> result = new List<PositionItem>();
            string query = String.Format(@"select *
                                            from {0}
                                            where league_id = {1}
                                            order by sort_value asc", TABLE_USER_ROSTER_CONFIG, league.identifier);
            await Task.Delay(2000);
            //List<PositionItem> values = await DBAdapter.executeQuery<PositionItem>(query.ToString());
            List<PositionItem> values = new List<PositionItem>();

            foreach (PositionItem value in values)
            {
                result.Add(value);
            }

            return result;
        }
        public static async Task<List<DepthChartItem>> getTeamDepthChart(string teamAbbr)
        {
            List<DepthChartItem> result = new List<DepthChartItem>();
            string position = string.Empty;
            result.Clear();
            var rosterConfig = await LeagueManager.getRosterConfig(MyDraftEngine.Instance.league.identifier.ToString());
            foreach (var itm in rosterConfig)
            {
                if (itm.positionKey == "FB-RB")
                    itm.keyFull = "Running Back";

                List<DepthChartItem> depthChartitems = new List<DepthChartItem>();
                depthChartitems = await PlayerManager.getDepthChart(teamAbbr, itm.positionKey);
                if (itm.positionKey != "Bench")
                {
                    foreach (DepthChartItem item in depthChartitems)
                    {
                        IList<DepthChartStats> stats = await PlayerManager.getDepthChartStats(item.playerID);
                        item.stats = stats;
                        result.Add(new DepthChartItem() { playerID = item.playerID, position = itm.keyFull, rank = item.rank, teamAbbr = item.teamAbbr, sortValue = itm.sortValue, stats = item.stats });
                    }
                }
            }
            return result;
        }
        #endregion //  Roster  //

        #region // Team Rank //
        public class RosterConfigItem
        {
            [Column("position_key")]
            public string positionKey { get; set; }
            [Column("value")]
            public int value { get; set; }
            [Column("sort_value")]
            public int sortValue { get; set; }
            [Column("key_full")]
            public string keyFull { get; set; }
        }
        public async Task<IDictionary<int, double>> getLeagueRankings()
        {
            IDictionary<int, double> teamTotPoints = new Dictionary<int, double>();
            IList<FantasyTeam> _fanTeams = await teamsForLeague(MyDraftEngine.Instance.league);
            var rosterConfig = await LeagueManager.getRosterConfig(MyDraftEngine.Instance.league.identifier.ToString());
            foreach (FantasyTeam team in _fanTeams)
            {
                Dictionary<string, double> _itm = new Dictionary<string, double>();
                foreach (var item in rosterConfig)
                {
                    var posTotal = await LeagueManager.getTeamPositionTotal(MyDraftEngine.Instance.league.identifier, team.identifier, item.positionKey, item.value);
                    foreach (var pts in posTotal)
                    {
                        _itm.Add(pts.Key, pts.Value);
                    }
                }
                teamTotPoints.Add(team.identifier, _itm.Sum(x => x.Value));
            }
            //var list = teamTotPoints.Values.ToList();
            //list.Sort();
            return teamTotPoints;
        }
        public static async Task<List<RosterConfigItem>> getRosterConfig(string leagueID)
        {
            List<RosterConfigItem> starterValues = new List<RosterConfigItem>();
            string query = String.Format(@"select distinct
                                            position_key 
                                            , starters as value
                                            , sort_value
                                            , key_full
                                        from user_roster_config
                                        where starters > 0
                                            and league_id = {0}
                                        order by sort_value asc", leagueID);
            await Task.Delay(2000);
            //List<RosterConfigItem> items = await DBAdapter.executeQuery<RosterConfigItem>(query);
            List<RosterConfigItem> items = new List<RosterConfigItem>();

            foreach (RosterConfigItem item in items)
            {
                starterValues.Add(item);
            }
            // Add Bench for grouping in Fantasy Team Roster display
            starterValues.Add(new RosterConfigItem { positionKey = "Bench", value = 0, sortValue = 99 });
            return starterValues;
        }
        public static async Task<Dictionary<string, double>> getTeamPositionTotal(int leagueID, int teamID, string position, int limit)
        {
            Dictionary<string, double> posTotal = new Dictionary<string, double>();
            string query_table;
            if (!AppSettings.isDraftTypeAuction)
            {
                query_table = "user_draft_selections";
            }
            else
            {
                query_table = "user_draft_assignments";
            }
            string query = String.Format(@"select 
                                                '{2}' as position_key
                                                ,sum(a1.points) as value
                                            from points a1
                                            join {4} a2 on a1.player_id = a2.player_id
                                            join players a3 on a1.player_id = a3.player_id
                                            Where a2.team_id = {1}
                                                and a2.league_id = {0}
                                                and a3.position in ('{2}')
                                            order by a1.points desc
                                            limit {3} ", leagueID, teamID, position.Replace("-", "','"), limit, query_table);
            await Task.Delay(2000);
            //IList<RosterConfigItem> items = await DBAdapter.executeQuery<RosterConfigItem>(query);
            IList<RosterConfigItem> items = new List<RosterConfigItem>();

            foreach (RosterConfigItem item in items)
            {
                posTotal.Add(item.positionKey, item.value);
            }
            return posTotal;
        }
        #endregion // Team Rank //

        #region // News //
        public static async Task<List<NewsItem>> getNews(string abbr)
        {
            List<NewsItem> newsItems = new List<NewsItem>();

            StringBuilder query = new StringBuilder();
            query.Append(String.Format(@"select distinct
                                            a1.*
                                        from player_news a1
                                        join players a2 on a1.player_id = a2.player_id "));
            if (abbr != null && abbr != "ALL")
                query.Append(string.Format("where a2.team_abbr = '{0}'", abbr));
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
        #endregion // News //


    }
}
