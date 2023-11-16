using CodeTitans.Core.Generics;
using DraftService;
using MyDraftAPI_v2.FantasyDataModel;
using MyDraftAPI_v2.Managers;
using MyDraftLib.Utilities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using ViewModel;
#pragma warning disable 

namespace FanDataModel
{
    public class FantasyLeague
    {
        public int UniverseID { get; set; }   
        public int identifier { get; set; }
        public String name { get; set; }
        public String abbr { get; set; }
        public bool isPPR
        {
            get
            {
                //foreach (CustomScoringUserItem item in customScoringValues)
                //{
                //    if (item.getKey().Equals("recs", StringComparison.CurrentCultureIgnoreCase) && item.getValue() > 0)
                //        return true;
                //}

                return false;
            }
        }
        public int siteID { get; set; }
        public int numTeams { get; set; }
        public bool isIncludeIDP { get; set; }
        public bool isCombineWRTE { get; set; }
        public int userTeamID { get; set; }
        public IList<UserLeageTeamItem> teams { get; set; }
        public IDictionary<String, IDictionary<String, int>> rosterValues { get; set; }
        public String playerUniverse { get; set; }
        public String url { get; set; }
        public String source { get; set; }
        public Double auction_salaryCap { get; set; }
        public Double auction_player_min { get; set; }
        public Double leagueMode { get; set; }
        public string mockDraft { get; set; }
        public int isPositionLimitEnabled { get; set; }

        public List<FantasyTeam> fanTeams { get; set; }


        #region DraftConfiguration
        //public int draftConfigID { get; set; }
        //public DraftConfiguration draftConfig { get; set; }
        public int rounds { get; set; }
        public bool draftByTeamEnabled { get; set; }
        public bool mockDraftEnabled { get; set; }
        public int draftType { get; set; }
        public DraftOrderType draftOrderType { get; set; }

        public float auctionBudget { get; set; }
        public float auctionMinimumBid { get; set; }

        public FantasyTeam myTeam
        {
            get
            {
                return teamWithID(userTeamID);
            }
        }

        public enum DraftType
        {
            Standard
            , Auction
        };

        public enum DraftOrderType
        {
            snake, straight, thirdRoundReversal, thirdRoundFlip, auction
        };
        #endregion

        private IList<CustomScoringUserItem> customScoringValues;
        bool processCustomScoringInProgress;

        bool teamDEFRankingsSet;

        static String kRosterLimitsSectionArray = "sections";
        static String kRosterLimitsTitle = "title";
        static String kRosterLimitsItems = "items";
        static String kRosterLimitsValue = "value";

        // Constructor
        public FantasyLeague()
        {
            fanTeams = new List<FantasyTeam>();
        }

        public FantasyLeague(int identifier)
        {
            this.identifier = identifier;
            // TODO this.rosterValues = LeagueManager.getRosterLimits(Convert.ToString(identifier));
            //Task task = Init_RosterValues();
        }

        public async Task rosterInitialize(int identifier)
        {
            await init_RosterValues(identifier);
        }
        private async Task init_RosterValues(int identifier)
        {
            this.rosterValues = await LeagueManager.getRosterLimits(Convert.ToString(identifier));
        }

        //public static async Task<FantasyLeague> leagueWithID(String leagueID)
        //{
        //    return await LeagueManager.getLeagueWithID(Convert.ToInt32(leagueID));
        //}

        //public async Task initializeAsyncValues()
        //{
        //    await getScoringValues();

        //    if (this.rosterValues == null || this.rosterValues.Count() == 0)
        //        if (AppSettings.isRosterValueNew)
        //        {
        //            await loadDefaultRosterLimitsData();
        //            this.rosterValues = await LeagueManager.getRosterLimits(Convert.ToString(identifier));
        //        }
        //}

        public FantasyTeam teamWithID(int teamID)
        {
            //foreach (FantasyTeam team in teams)
            //{
            //    if (team.identifier == teamID)
            //    {
            //        return team;
            //    }
            //}
            return null;
        }

       
        public bool isTeamDEFRankingsSet()
        {
            return teamDEFRankingsSet;
        }

        public void setTeamDEFRankingsSet(bool teamDEFRankingsSet)
        {
            this.teamDEFRankingsSet = teamDEFRankingsSet;
        }

        public bool isMyTeam(FantasyTeam team)
        {
            if (team != null)
            {
                return team.identifier == this.userTeamID;
            }
            else
            {
                return false;
            }
        }
               
        //public async Task reloadScoringValues()
        //{
        //    customScoringValues = null;
        //    await getScoringValues();
        //}

        //public async Task<IList<CustomScoringUserItem>> getScoringValues()
        //{
        //    if (customScoringValues == null)
        //        customScoringValues = await LeagueManager.getCustomScoringValuesForLeague(this);

        //    return customScoringValues;
        //}

        public void setScoringValues(IList<CustomScoringUserItem> scoringValues)
        {
            this.customScoringValues = scoringValues;
        }

        //public async Task<bool> hasCustomValues()
        //{
        //    IList<string> groupIDs = await LeagueManager.customScoringGroupIDs(this);
        //    return customScoringValues.Count() > 0 || groupIDs.Count() > 0;
        //}

        //public async Task<bool> hasCustomValuesInGroup(CustomScoringGroup group)
        //{
        //    IDictionary<String, CustomScoringUserItem> scoringValuesMap = await customScoringValuesMap();
        //    foreach (CustomScoringSection section in group.getSections())
        //    {
        //        foreach (CustomScoringDefaultItem defaultItem in section.getScoringItems())
        //        {
        //            if (scoringValuesMap.ContainsKey(defaultItem.getKey()))
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    // Still here so check if position specific scoring is enabled for the group
        //    return await isCustomScoringEnabledForGroup(group);
        //}

        //public async Task<bool> isCustomScoringEnabledForGroup(CustomScoringGroup group)
        //{
        //    return await LeagueManager.isCustomScoringEnabledForGroup(group, this);
        //}

        public async Task<bool> isOtheLeagues()
        {
            return await LeagueManager.isOtherLeagues();
        }

        public async Task<bool> isDraftAuction()
        {
            return await DraftManager.isAuctionDraft();
        }

        //public async Task<IDictionary<String, CustomScoringUserItem>> customScoringValuesMap()
        //{
        //    IDictionary<String, CustomScoringUserItem> userItemMap = await customScoringValuesMapForPosition("ALL");
        //    if (userItemMap == null)
        //        userItemMap = new Dictionary<String, CustomScoringUserItem>();

        //    return userItemMap;
        //}

        //public async Task<CustomScoringUserItem?> customScoringUserItemForKey(String key)
        //{
        //    IDictionary<String, CustomScoringUserItem> userItemMap = await customScoringValuesMap();
        //    return userItemMap.ContainsKey(key) ? userItemMap[key] : null;
        //}

        //public async Task<IDictionary<String, CustomScoringUserItem>> customScoringValuesMapForPosition(String position)
        //{
        //    if (position == null)
        //        position = "ALL";

        //    IDictionary<String, CustomScoringUserItem> results = new Dictionary<String, CustomScoringUserItem>();
        //    foreach (CustomScoringUserItem item in await getScoringValues())
        //    {
        //        if (item.getPosition().Equals(position))
        //            results.Add(item.getKey(), item);
        //    }

        //    // If no matches are found then return values for the default position
        //    if (results.Count() == 0 && !position.Equals("ALL"))
        //        return await customScoringValuesMapForPosition("ALL");
        //    else
        //        return results;
        //}

        //public async Task saveScoringValue(float value, String key, String position)
        //{
        //    await LeagueManager.saveCustomScoringUserItem(value, key, position, this);
        //    this.customScoringValues = await LeagueManager.getCustomScoringValuesForLeague(this);
        //}

        public async Task<bool> isCustomScoringCalculated(int week)
        {
            if (this.processCustomScoringInProgress)
                return true;

            int year = AppSettings.getProjectionStatsYear();
            int segment = week;

            double lastUpdateTimestamp = await UpdateManager.getTimeStampForPointsWithLeagueID(this.identifier, "projections", year, segment);
            if (lastUpdateTimestamp == 0)
            {
                return false;
            }

            double dataTimestamp;
            if (segment == StatsManager.kFullSeason)
            {
                dataTimestamp = await UpdateManager.getLatestTimeStampForType("rotowire_seasonprojections");
            }
            else
            {
                dataTimestamp = await UpdateManager.getLatestTimeStampForType("rotowire_weeklyprojections", segment);
            }

            double rosterTimestamp = await UpdateManager.getLatestTimeStampForType("nfl_roster");
            bool isOutOfDate = lastUpdateTimestamp < dataTimestamp || lastUpdateTimestamp < rosterTimestamp;

            Debug.WriteLine(String.Format("isCustomScoringCalculated: {0}", !isOutOfDate ? "YES" : "NO"));

            return !isOutOfDate;
        }

        //public async Task processCustomRankings(int year, int week)
        //{
        //    if (this.processCustomScoringInProgress)
        //        return;

        //    processCustomScoringInProgress = true;

        //    Debug.WriteLine(String.Format("PROCESS_CUSTOM_RANKINGS_START year: {0}, week: {1}", year, week));
        //    await processPoints(year, week);

        //    IList<IDictionary<String, Object>> vbdData = await VBDCalculator.calculateForLeague(this);
        //    await LeagueManager.savePlayerVBD(vbdData, year, week, this.identifier);

        //    Debug.WriteLine(String.Format("PROCESS_CUSTOM_RANKINGS_COMPLETE year: {0}, week: {1}", year, week));

        //    processCustomScoringInProgress = false;
        //}

        public async Task processPoints(int year, int week)
        {
            //DLog(@"Calculate custom rankings");
            // TODO reset all player points to 0

            IDictionary<String, float> results = new Dictionary<String, float>();
            IDictionary<String, String> positionMap = null;
            CustomScoring customScoring = await AppSettings.getCustomScoring();
            foreach (CustomScoringGroup group in customScoring.getGroups())
            {
                Debug.WriteLine(String.Format("Processing Group: {0}", group.title));
                IDictionary<String, Object> groupStats = await StatsManager.loadGroupStats(group, year, week);

                if (groupStats != null && groupStats.Keys.Count > 0)
                {
                    Debug.WriteLine(String.Format("Loaded {0} stat items for {1}", groupStats.Keys.Count, group.title));
                    foreach (String statsID in groupStats.Keys)
                    {
                        IDictionary<String, float> playerStats = (IDictionary<String, float>)groupStats[statsID];

                        IList<CustomScoringDefaultItem> defaultPointsMapping = new List<CustomScoringDefaultItem>();

                        foreach (CustomScoringSection eachSection in group.getSections())
                        {
                            IList<CustomScoringDefaultItem> items = eachSection.getScoringItems();
                            foreach (CustomScoringDefaultItem item in items)
                                defaultPointsMapping.Add(item);
                        }

                        // Get the user's custom scoring values for the stats group
                        String position;
                        //if (await isCustomScoringEnabledForGroup(group))
                        //{
                        //    // Load the playerID to position map since we have one or more groups that need it
                        //    if (positionMap == null)
                        //        positionMap = await PlayerManager.getPlayerIDToPositionMap();

                        //    position = positionMap[statsID];
                        //}
                        //else
                        //{
                            position = "ALL";
                        //}

                        //IDictionary<String, CustomScoringUserItem> userValues = await customScoringValuesMapForPosition(position);

                        // Accumulate points for each stats section
                        float playerPoints = results.ContainsKey(statsID) ? results[statsID] : 0;
                        //playerPoints += LeagueScoringCalculator.calculatePoints(playerStats, defaultPointsMapping, userValues);

                        results[statsID] = playerPoints;
                    }
                }
            }

            await LeagueManager.savePlayerPoints(results, year, week, Convert.ToString(this.identifier));

            /* TODO
            IList<IDictionary<String, dynamic>> vbdData = VBDCalculator.calculateForLeague(this);
            LeagueManager.savePlayerVBD(vbdData, year, week, identifier);
            LeagueManager.saveTeamOffensiveRankingsForLeague(this);
            LeagueManager.saveTeamDefensiveRankingsForLeague(this);
            LeagueManager.saveScheduleFPARankingsForLeague(this);
             * */
        }

        public int getRosterValueForKey(String positionKey, String valueType)
        {
            return this.rosterValues[positionKey][valueType];
        }

        public IDictionary<String, IDictionary<String, int>> getRosterValues()
        {
            return rosterValues;
        }

        public IDictionary<String, RosterPosition> getRosterPositions()
        {
            IDictionary<String, RosterPosition> positionMap = new Dictionary<String, RosterPosition>();
            foreach (String positionKey in rosterValues.Keys)
            {
                IDictionary<String, int> posEntry = rosterValues[positionKey];
                int starters = posEntry["starters"];
                int max = posEntry["max"];
                int flex = posEntry["flex"];
                String[] positions = positionKey.Split(new String[] { "-" }, StringSplitOptions.None);
                RosterPosition rosterPosition = new RosterPosition(new List<String>(positions), starters, max, flex == 1);
                positionMap.Add(positionKey, rosterPosition);
            }

            return positionMap;
        }

        //public async Task saveRosterValue(int value, String type, IList<String> positions)
        //{
        //    String positionKey = TNUtility.getKeyFromArrayOfStrings(positions);
        //    IDictionary<String, int> positionData = this.rosterValues.ContainsKey(positionKey) ? this.rosterValues[positionKey] : null;
        //    if (positionData == null)
        //    {
        //        positionData = new Dictionary<String, int>();
        //        this.rosterValues.Add(positionKey, positionData);
        //    }
        //    positionData.Add(type, value);

        //    await LeagueManager.saveRosterLimits(this);
        //    await setDirty();
        //}

        //public async Task loadDefaultRosterLimitsData()
        //{
        //    rosterValues = new Dictionary<String, IDictionary<String, int>>();
        //    IPropertyListDictionary starters = await AppSettings.getRosterStarters();
        //    IEnumerable<IPropertyListItem> sectionArray = starters[kRosterLimitsSectionArray].ArrayItems;
        //    foreach (IPropertyListItem section in sectionArray)
        //    {
        //        IEnumerable<IPropertyListItem> items = section[kRosterLimitsItems].ArrayItems;
        //        foreach (IPropertyListItem item in items)
        //        {
        //            IEnumerable<IPropertyListItem> positions = item["positions"].ArrayItems;

        //            // Set the default value as a placeholder
        //            int defaultValue = item[kRosterLimitsValue].Int32Value;

        //            String valueKey = TNUtility.getKeyFromArrayOfStrings(TNUtility.listFromPropertyListArrayItems(positions));
        //            if (!rosterValues.ContainsKey(valueKey))
        //            {
        //                IDictionary<String, int> positionMap = new Dictionary<String, int>();
        //                positionMap.Add("starters", defaultValue);
        //                // positionLimit = defaultValue;
        //                rosterValues.Add(valueKey, positionMap);
        //            }
        //        }
        //    }

        //    await LeagueManager.saveRosterLimits(this);
        //}

        public static async Task<DraftType> getTypeDraft()
        {
            DraftType typeResult = new DraftType();
            if (await LeagueManager.getDraft_Type() == 1)
            {
                typeResult = DraftType.Standard;
            }
            else
            {
                typeResult = DraftType.Auction;
            }
            return typeResult;
        }

        public static async Task<DraftOrderType> getDraftOrder()
        {
            DraftOrderType typeResult = new DraftOrderType();
            string orderType = await LeagueManager.getDraft_Order();
            switch (orderType)
            {
                case "snake":
                    typeResult = DraftOrderType.snake;
                    break;
                case "straight":
                    typeResult = DraftOrderType.straight;
                    break;
                case "thirdRoundReversal":
                    typeResult = DraftOrderType.thirdRoundReversal;
                    break;
                case "thirdRoundFlip":
                    typeResult = DraftOrderType.thirdRoundFlip;
                    break;
                case "auction":
                    typeResult = DraftOrderType.auction;
                    break;
            }
            return typeResult;
        }

        //public static async Task<IList<MyDraftAPI_v2.FantasyDataModel.Draft.DraftPick>> getAuctionSelections(FantasyLeague league)
        //{
        //    IList<MyDraftAPI_v2.FantasyDataModel.Draft.DraftPick> auctionSelections = await DraftManager.auctionPicksForLeague(league);

        //    return auctionSelections;
        //}
    }
}

