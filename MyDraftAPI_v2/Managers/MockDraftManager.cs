using MyDraftAPI_v2.Engines;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyDraftAPI_v2.Managers
{
    public static class MockDraftManager
    {
        private static List<MockDraftPlayerItem> _result = new List<MockDraftPlayerItem>();
        private static List<string> _resultStarter = new List<string>();

        public enum MockDraftStatus
        {
            notStarted,
            paused,
            autoPickInProgress,
            waitingForUserSelection,
            ended
        }
        private static int mockTeamID { get; set; }
        private static int pickRound { get; set; }
        private class MockDraftPlayerItem
        {
            [Column("player_id")]
            public String playerID { get; set; }
            [Column("value_standard")]
            public double value_standard { get; set; }
            [Column("value_ppr")]
            public double value_ppr { get; set; }
            [Column("position")]
            public String position { get; set; }
        }
        private class PositionCountItem
        {
            [Column("starters")]
            public double starterCount { get; set; }
            [Column("minimum")]
            public double minCount { get; set; }
            [Column("maximum")]
            public double maxCount { get; set; }
            [Column("flex")]
            public double flexCount { get; set; }
        }
        private class TeamRosterCountItem
        {
            [Column("roster_pos_count")]
            public double rosterPosCount { get; set; }
        }
        private class valCount
        {
            [Column("valCount")]
            public int valCnt { get; set; }
        }
        private class valStarterPositions
        {
            [Column("position")]
            public string pos { get; set; }
        }
        private class valStarterSlots
        {
            [Column("position_key")]
            public string pos { get; set; }
            [Column("starters")]
            public int startCount { get; set; }
        }
        public static async Task mockDraftAutoPickPlayer()
        {
            await Task.Delay(1500);
            var status = MyDraftEngine.Instance.onTheClockDraftPick();
            if (status == null)
            {
                AppSettings.isMockDraftActive = false;
                MockDraftEngine.Instance.MockDraftEnded();
                return;
            }
            mockTeamID = status.team.identifier;
            pickRound = status.round;
            if (status.team.isMyTeam)
            {
                AppSettings.mockDraftStatus = MockDraftStatus.waitingForUserSelection;
                return;
            }
            else
            {
                AppSettings.mockDraftStatus = MockDraftStatus.autoPickInProgress;

                var pickList = await autoPickPlayers(mockTeamID, pickRound);
                foreach (MockDraftPlayerItem item in pickList)
                {
                    if (await checkPosition(item))
                    {
                        await MyDraftEngine.Instance.executeDraftPickOnTheClock(item.playerID);
                        break;
                    }
                    else if (await isStarterPickComplete(mockTeamID))
                    {
                        await MyDraftEngine.Instance.executeDraftPickOnTheClock(item.playerID);
                        break;
                    }
                }
                int maxOverallPick = await DraftManager.maxOverall(MyDraftEngine.Instance.league.identifier);

            }
        }
        private static async Task<List<MockDraftPlayerItem>> autoPickPlayers(int teamID, int round)
        {
            if (await isfilterRemainingPicks(teamID, round) && !await isStarterPickComplete(mockTeamID))
            {
                List<string> starterSlots = await getStarterSlots(teamID);
                string Positions = string.Join("-", starterSlots.ToArray());
                return await Load_ADP(Positions);
            }
            else
            {
                return await Load_ADP();
            }
        }
        private static async Task<List<MockDraftPlayerItem>> Load_ADP()
        {
            _result.Clear();
            string query = String.Format(@"select 
                                                a1.player_id
                                                , a1.value_standard
                                                , a1.value_ppr
                                                , a3.position
                                            from adp a1
                                            left join user_draft_selections a2 on a1.player_id = a2.player_id
                                                and a2.league_id = {0}
                                            left join players a3 on a1.player_id = a3.player_id
                                            where a2.player_id is null
                                            order by a1.value_ppr asc
                                            LIMIT 100", MyDraftEngine.Instance.league.identifier);
            List<MockDraftPlayerItem> values = await DBAdapter.executeQuery<MockDraftPlayerItem>(query.ToString());

            foreach (MockDraftPlayerItem value in values)
            {
                _result.Add(value);
            }

            return _result;
        }
        private static async Task<List<MockDraftPlayerItem>> Load_ADP(string positions)
        {
            List<MockDraftPlayerItem> result = new List<MockDraftPlayerItem>();
            string query = String.Format(@"select 
                                                a1.player_id
                                                , a1.value_standard
                                                , a1.value_ppr
                                                , a3.position
                                            from adp a1
                                            left join user_draft_selections a2 on a1.player_id = a2.player_id
                                                and a2.league_id = {0}
                                            left join players a3 on a1.player_id = a3.player_id
                                            where a2.player_id is null
                                                and a3.position in ('{1}')
                                            order by a1.value_ppr asc
                                            LIMIT 5", MyDraftEngine.Instance.league.identifier, positions.Replace("-", "','"));
            List<MockDraftPlayerItem> values = await DBAdapter.executeQuery<MockDraftPlayerItem>(query.ToString());

            foreach (MockDraftPlayerItem value in values)
            {
                result.Add(value);
            }

            return result;
        }
        private static async Task<bool> isfilterRemainingPicks(int teamID, int round)
        {
            bool result = false;
            int remainingPickCount = MyDraftEngine.Instance.league.rounds - round;
            int remainingSlotCount;

            string query = String.Format(@"select 
                                                count(overall) as valCnt
                                            from user_draft_selections a1
                                            where a1.league_id = {0}
                                                and a1.team_id = {1}
                                                and player_id is null", MyDraftEngine.Instance.league.identifier, teamID);
            List<valCount> values = await DBAdapter.executeQuery<valCount>(query.ToString());

            var item = values.FirstOrDefault();
            remainingSlotCount = item.valCnt;

            if (remainingSlotCount <= remainingPickCount)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }
        private static async Task<bool> isStarterPickComplete(int teamID)
        {
            bool result = false;
            int starteCount;
            int pickCount;

            string queryStarter = String.Format(@"select 
                                                sum(a1.starters) as valCnt
                                            from user_roster_config  a1
                                            where a1.starters > 0
                                                and a1.league_id = 1", MyDraftEngine.Instance.league.identifier, teamID);
            List<valCount> valueStarters = await DBAdapter.executeQuery<valCount>(queryStarter.ToString());
            var itemStarter = valueStarters.FirstOrDefault();
            starteCount = itemStarter.valCnt;

            string query = String.Format(@"select 
                                                count(overall) as valCnt
                                            from user_draft_selections a1
                                            where a1.league_id = {0}
                                                and a1.team_id = {1}
                                                and player_id is not null", MyDraftEngine.Instance.league.identifier, teamID);
            List<valCount> values = await DBAdapter.executeQuery<valCount>(query.ToString());

            var item = values.FirstOrDefault();
            pickCount = item.valCnt;

            if (starteCount == pickCount)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }
        private static async Task<PositionCountItem> getPositionCount(MockDraftPlayerItem mockDraftPlayerItem)
        {
            PositionCountItem result = null;
            if (mockDraftPlayerItem.position == "RB" || mockDraftPlayerItem.position == "FB")
            {
                mockDraftPlayerItem.position = "FB-RB";
            }
            else if (mockDraftPlayerItem.position == "ILB" || mockDraftPlayerItem.position == "OLB")
            {
                mockDraftPlayerItem.position = "ILB-OLB";
            }
            string query = String.Format(@"select 
                                                a1.starters
                                                ,a1.minimum
                                                ,a1.maximum
                                                ,a1.flex    
                                            from user_roster_config a1
                                            where a1.league_id = {0}
                                                and a1.position_key = '{1}'", MyDraftEngine.Instance.league.identifier, mockDraftPlayerItem.position);
            List<PositionCountItem> values = await DBAdapter.executeQuery<PositionCountItem>(query);

            if (values.Count() > 0)
            {
                result = values[0];
            }

            return result;
        }
        private static async Task<TeamRosterCountItem> getTeamRosterCount(int teamID, MockDraftPlayerItem mockDraftPlayerItem)
        {
            TeamRosterCountItem result = null;
            if (mockDraftPlayerItem.position == "RB" || mockDraftPlayerItem.position == "FB")
            {
                mockDraftPlayerItem.position = "FB-RB";
            }
            else if (mockDraftPlayerItem.position == "ILB" || mockDraftPlayerItem.position == "OLB")
            {
                mockDraftPlayerItem.position = "ILB-OLB";
            }
            string query = String.Format(@"select 
                                                count(a1.player_id) as roster_pos_count   
                                            from user_draft_selections a1
                                            join players a2 on a1.player_id = a2.player_id
                                            where league_id = {0}
                                                and a1.team_id = {1}
                                                and a2.position in ('{2}')", MyDraftEngine.Instance.league.identifier, teamID, mockDraftPlayerItem.position.Replace("-", "','"));
            List<TeamRosterCountItem> values = await DBAdapter.executeQuery<TeamRosterCountItem>(query);

            if (values.Count() > 0)
            {
                result = values[0];
            }

            return result;
        }
        private static async Task<bool> checkPosition(MockDraftPlayerItem mockDraftPlayerItem)
        {
            bool result = false;
            PositionCountItem positionCountItem = await getPositionCount(mockDraftPlayerItem);
            var rosterCNT = await getTeamRosterCount(mockTeamID, mockDraftPlayerItem);
            if (rosterCNT.rosterPosCount < positionCountItem.starterCount)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }
        private static async Task<List<string>> getStarterSlots(int TeamID)
        {
            _resultStarter.Clear();
            string query = String.Format(@"select 
                                            a1.position_key
                                            , a1.starters
                                        from user_roster_config a1
                                        where a1.league_id = {0}
                                            and starters > 0", MyDraftEngine.Instance.league.identifier);
            List<valStarterSlots> values = await DBAdapter.executeQuery<valStarterSlots>(query.ToString());

            foreach (valStarterSlots value in values)
            {
                if (await getTeamPositionCount(TeamID, value.pos) < value.startCount)
                    _resultStarter.Add(value.pos);
            }

            return _resultStarter;
        }
        private static async Task<int> getTeamPositionCount(int teamID, string position)
        {
            int result = 0;
            if (position == "RB" || position == "FB")
            {
                position = "FB-RB";
            }
            else if (position == "ILB" || position == "OLB")
            {
                position = "ILB-OLB";
            }
            string query = String.Format(@"select distinct
                                                count(a1.player_id) as valCount   
                                            from user_draft_selections a1
                                            join players a2 on a1.player_id = a2.player_id
                                            where league_id = {0}
                                                and a1.team_id = {1}
                                                and a2.position in ('{2}')", MyDraftEngine.Instance.league.identifier, teamID, position.Replace("-", "','"));
            List<valCount> values = await DBAdapter.executeQuery<valCount>(query);

            if (values.Count() > 0)
            {
                result = int.Parse(values[0].valCnt.ToString());
            }

            return result;
        }
    }
}
