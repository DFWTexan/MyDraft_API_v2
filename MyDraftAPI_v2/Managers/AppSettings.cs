using CodeTitans.Core.Generics;
using CodeTitans.Core.Generics.Objects;
using MyDraftAPI_v2.DbData.DataModel;
using MyDraftAPI_v2.FantasyDataModel;
using MyDraftLib.Utilities;
using Windows.Storage;
#pragma warning disable 

namespace MyDraftAPI_v2.Managers
{
    public class AppSettings
    {
        private static PropertyList appConfig;
        private static PropertyList positionFilter;
        private static IDictionary<String, Object> statsDisplayData;

        public enum StatsType
        {
            StatsTypeActual,
            StatsTypeProjections,
        }
        public enum ScoringEditCategory
        {
            CategoryPassingEdit
            , CategoryRushingEdit
            , CategoryRecievingEdit
            , CategoryFumblesEdit
            , CategoryKickingEdit
            , CategoryKickReturnEdit
            , CategoryTeamDefenseSpecialEdit
            , CategoryIndividualDefenseEdit
        }
        public static async Task Initialize()
        {
            string plistTxt = await TNUtility.ReadFile("config.plist");
            appConfig = PropertyList.Read(plistTxt);

            //            IsolatedStorageSettings.ApplicationSettings.Add("some_property", "test");
            //              IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public static string getBaseURL()
        {
            string path = appConfig["apiPath"].StringValue;
            return string.Format("http://touchdown.290design.com/{0}", path);
        }

        public static String getSportsLeague()
        {
            return "nfl";
        }
        public static int benchSlots
        {
            get
            {
                return _benchSlots;
            }
        }
        private static int _benchSlots;
        public static string getApplicationVersion()
        {
            return "1.0";
        }
        private static bool _isMockDraftActive;
        public static bool isMockDraftActive
        {
            get
            {
                return _isMockDraftActive;
            }
            set
            {
                _isMockDraftActive = value;
            }
        }
        //private static MockDraftManager.MockDraftStatus _mockDraftStatus;
        //public static MockDraftManager.MockDraftStatus mockDraftStatus
        //{
        //    get
        //    {
        //        return _mockDraftStatus;
        //    }
        //    set
        //    {
        //        _mockDraftStatus = value;
        //    }
        //}
        private ScoringEditCategory _scoreEdit;
        public ScoringEditCategory scoreEdit
        {
            get
            {
                return _scoreEdit;
            }
            set
            {
                _scoreEdit = value;
            }
        }
        private static FantasyLeague.DraftType _leagueDraftType;
        public FantasyLeague.DraftType leagueDraftType
        {
            get
            {
                return _leagueDraftType;
            }
            set
            {
                _leagueDraftType = value;
            }
        }
        private static string _playerID = string.Empty;
        public static string depthPlayerID
        {
            get { return _playerID; }
            set { _playerID = value; }
        }

        public static int aavMaxID { get; set; }
        public static int adpMaxID { get; set; }
        public static int projectionsSeasonMaxID { get; set; }
        public static int playersMaxID { get; set; }
        public static int depthchartMaxID { get; set; }
        public static int injuryActiveMaxID { get; set; }
        public static int injuryDeleteMaxID { get; set; }
        private static int _newsMaxuid;
        public static int newsMaxID
        {
            get { return _newsMaxuid; }
            set { _newsMaxuid = value; }
        }
        public static int ProjectionsWeeklyMaxID { get; set; }
        private static int _PositionFilterIndex = -1;
        public static int PositionFilterIndex
        {
            get { return _PositionFilterIndex; }
            set { _PositionFilterIndex = value; }
        }
        private static string _assignPlayerView = string.Empty;
        public static string assignPlayerView
        {
            get { return _assignPlayerView; }
            set { _assignPlayerView = value; }
        }

        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static bool _isLiveData = true;
        private static string _appDate;
        public static string appDate
        {
            get
            {
                return _appDate;
            }
            set
            {
                _appDate = value;
            }
        }
        private static string _draftType;
        public static string draftType
        {
            get
            {
                return _draftType;
            }
            set
            {
                _draftType = value;
            }
        }
        private static double _apiUID;
        public static double apiUID
        {
            get
            {
                return _apiUID;
            }
            set
            {
                _apiUID = value;
            }
        }
        private static bool _draftleagueEdit;
        public static bool draftLeageEdit
        {
            get
            {
                return _draftleagueEdit;
            }
            set
            {
                _draftleagueEdit = value;
            }
        }
        private static bool _isMockEnabled = false;
        public static bool isMockEnabled
        {
            get
            {
                return _isMockEnabled;
            }
            set
            {
                _isMockEnabled = value;
            }
        }
        private static bool _isDraftTypeAuction;
        public static bool isDraftTypeAuction
        {
            get
            {
                return _isDraftTypeAuction;
            }
            set
            {
                _isDraftTypeAuction = value;
            }
        }
        private static bool _isRosterValueNEW;
        public static bool isRosterValueNew
        {
            get
            {
                return _isRosterValueNEW;
            }
            set
            {
                _isRosterValueNEW = value;
            }
        }
        private static bool _isPlayerNoteDirty;
        public static bool isPlayerNoteDirty
        {
            get
            {
                return _isPlayerNoteDirty;
            }
            set
            {
                _isPlayerNoteDirty = value;
            }
        }
        private static bool _isDraftSettingOpen;
        public static bool isDraftSettingOpen
        {
            get
            {
                return _isDraftSettingOpen;
            }
            set
            {
                _isDraftSettingOpen = value;
            }
        }
        private static bool _isPosLimitActive;
        public static bool isPosLimitActive
        {
            get
            {
                return _isPosLimitActive;
            }
            set
            {
                _isPosLimitActive = value;
            }
        }
        private static bool _isPosLimitEnabled;
        public static bool isPosLimitEnabled
        {
            get
            {
                return _isPosLimitEnabled;
            }
            set
            {
                _isPosLimitEnabled = value;
            }
        }
        private static bool _isDraftPageReload = true;
        public static bool isDraftPageReload
        {
            get
            {
                return _isDraftPageReload;
            }
            set
            {
                _isDraftPageReload = value;
            }
        }
        private static bool _playerProfileStatus;
        public static bool isPlayerProfileOpen
        {
            get
            {
                return _playerProfileStatus;
            }
            set
            {
                _playerProfileStatus = value;
            }
        }
        public static async Task<bool> isHasIDP()
        {
            bool result = await DraftManager.CheckIDP();
            return result;
        }
        //public static async Task<bool> isPositionLimit()
        //{
        //    bool result = await LeagueManager.checkPositionLimits();
        //    return result;
        //}
        static string _rosterView = "Offense";
        public static string rosterView
        {
            get
            {
                return _rosterView;
            }
            set
            {
                _rosterView = value;
            }
        }
        public static bool isMobile
        {
            get
            {
                var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
                return (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Mobile");
            }
        }
        static string _SportsLeague = "NFL";
        public static string SportsLeague
        {
            get
            {
                return _SportsLeague;
            }
            set
            {
                _SportsLeague = value;
            }
        }
        static String _SourceDataTXT = string.Empty;
        public static String SourceDataTXT
        {
            get
            {
                return _SourceDataTXT;
            }
            set
            {
                _SourceDataTXT = value;
            }
        }
        private static FilterSort _filterSort;
        public static FilterSort filterSort
        {
            get
            {
                return _filterSort;
            }
            set
            {
                _filterSort = value;
            }
        }
        //public static FilterSort getFilterSort()
        //{
        //    return _filterSort;
        //}
       
        public static void InitializeAppDate()
        {
            if (_isLiveData)
            {
                _appDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                _appDate = "2016-02-19";
            }
        }
        //public static void Initialize()
        //{
        //    _filterSort = new FilterSort(FilterSort.SortType.SortTypeSalary);

        //    //string plistTxt = await TNUtility.ReadFile("config.plist");
        //    //appConfig = PropertyList.Read(plistTxt);

        //    //            IsolatedStorageSettings.ApplicationSettings.Add("some_property", "test");
        //    //              IsolatedStorageSettings.ApplicationSettings.Save();
        //}

        public static async Task setBenchSlots(FantasyLeague league)
        {
            _benchSlots = (int)await LeagueManager.getBenchSlots(league);
        }

        public static int getProjectionStatsYear()
        {
            //int year = appConfig["current_season"].Int32Value;
            int year = 2017;
            return year;
        }

        public static int getNFLTeamStatsYear()
        {
            //int year = appConfig["current_season"].Int32Value;
            int year = 2014;
            return year;
        }

        public static void setProjectionStatsSegment(int segment)
        {
            localSettings.Values["projection_stats_segment"] = segment;
        }

        public static int getProjectionStatsSegment()
        {
            if (localSettings.Values["projection_stats_segment"] != null)
                return Convert.ToInt32(localSettings.Values["projection_stats_segment"]);
            else
                //return StatsManager.kFullSeason;
                return 18;
        }

        public static bool useAdjustedVBD()
        {
            return true;
        }

        public static async Task<CustomScoring> getCustomScoring()
        {
            string plistTxt = await TNUtility.ReadFile("custom_scoring.plist");
            PropertyList customScoringConfig = PropertyList.Read(plistTxt);
            return new CustomScoring(customScoringConfig);
        }

        public static async Task<PropertyList> getPositionFilter()
        {
            if (positionFilter == null)
            {
                string plistTxt = await TNUtility.ReadFile("position_filters.plist");
                positionFilter = PropertyList.Read(plistTxt);
            }

            return positionFilter;
        }

        public static IDictionary<String, Object> getRosterSpots()
        {
            /* TODO
            return TNUtility.loadPlistFromResource(R.raw.roster_limits);
             * */
            return null;
        }

        public static async Task<IPropertyListDictionary> getRosterStarters()
        {
            string plistTxt = await TNUtility.ReadFile("roster_limits.plist");
            PropertyList rosterConfig = PropertyList.Read(plistTxt);
            IPropertyListDictionary starters = rosterConfig["starters"];

            return starters;
        }

        public static float getAdpVBDAdjustmentRatio()
        {
            return (float)0.5;
        }

        public static IDictionary<String, Object> getStatsDisplayData()
        {
            /* TODO
            if (statsDisplayData == null)
            {
                String dataStr;
                try
                {
                    dataStr = TNUtility.readFileAsString(AppSettings.getContext(), R.raw.stats_display);
                
                    statsDisplayData = Plist.fromXml(dataStr);
                }
                catch (IOException e)
                {
                     TODO Auto-generated catch block
                    e.printStackTrace();
                }
                catch (XmlParseException e)
                {
                     TODO Auto-generated catch block
                    e.printStackTrace();
                }
            }
             * */
            return statsDisplayData;
        }

        public static IList<String> getStandardPositions()
        {
            IList<string> items = new List<string>();
            IEnumerable<IPropertyListItem> propertyListItems = appConfig["standard_positions"].ArrayItems;
            foreach (IPropertyListItem item in propertyListItems)
            {
                items.Add(item.StringValue);
            }

            return items;
        }

        public static IList<String> getIdpPositions()
        {
            IList<string> items = new List<string>();
            IEnumerable<IPropertyListItem> propertyListItems = appConfig["idp_positions"].ArrayItems;
            foreach (IPropertyListItem item in propertyListItems)
            {
                items.Add(item.StringValue);
            }

            return items;
        }

        public static IList<String> getPrimaryPositions()
        {
            IList<string> items = new List<string>();
            IEnumerable<IPropertyListItem> propertyListItems = appConfig["primary_positions"].ArrayItems;
            foreach (IPropertyListItem item in propertyListItems)
            {
                items.Add(item.StringValue);
            }

            return items;
        }

        public static IList<String> getSpecialTeamsPositions()
        {
            IList<string> items = new List<string>();
            IEnumerable<IPropertyListItem> propertyListItems = appConfig["special_teams_positions"].ArrayItems;
            foreach (IPropertyListItem item in propertyListItems)
            {
                items.Add(item.StringValue);
            }

            return items;
        }

        public static bool isIDPPositionName(String position)
        {
            IList<String> standardPositions = getStandardPositions();

            if (standardPositions.Contains(position))
                return false;
            else
                return true;
        }

        public static bool isIDPPositionGroup(List<String> positions)
        {
            foreach (String position in positions)
            {
                if (isIDPPositionName(position))
                    return true;
            }

            return false;
        }

        public static bool isTeamPosition(String position)
        {
            IList<string> items = new List<string>();
            IEnumerable<IPropertyListItem> propertyListItems = appConfig["team_position"].ArrayItems;
            foreach (IPropertyListItem item in propertyListItems)
            {
                items.Add(item.StringValue);
            }

            if (items.Contains(position))
                return true;
            else
                return false;
        }

        public static List<String> arrayWithFilteredIDPPositionsFromArray(List<String> positions)
        {
            List<String> result = new List<String>(positions.Count());
            foreach (String position in positions)
            {
                if (!isIDPPositionName(position))
                    result.Add(position);
            }

            return result;
        }

        public static async Task<IEnumerable<IPropertyListItem>> positionSectionsForGroup(String group)
        {
            PropertyList positionFilters = await AppSettings.getPositionFilter();
            return (IEnumerable<IPropertyListItem>)positionFilter[group].ArrayItems;
        }

        public static async Task<IList<IDictionary<String, Object>>> filteredPositionSectionsForPositionGroup(String group, FantasyLeague league)
        {
            PropertyList positionFilters = await AppSettings.getPositionFilter();
            IList<IDictionary<String, Object>> mutableArray = new List<IDictionary<String, Object>>();
            IEnumerable<IPropertyListItem> positionSections = (IEnumerable<IPropertyListItem>)positionFilters[group].ArrayItems;

            String wrteKey = TNUtility.getKeyFromArrayOfStrings(new List<String>(new String[] { "WR", "TE" }));
            String wrKey = TNUtility.getKeyFromArrayOfStrings(new List<String>(new String[] { "WR" }));
            String teKey = TNUtility.getKeyFromArrayOfStrings(new List<String>(new String[] { "TE" }));

            foreach (IPropertyListItem item in positionSections)
            {
                List<String> positions = new List<String>();

                IEnumerable<IPropertyListItem> positionItems = item["positions"].ArrayItems;
                foreach (IPropertyListItem positionItem in positionItems)
                    positions.Add(positionItem.StringValue);

                // Remove IDP positions if IDP is disabled. If resulting array is 0
                // length then skip the section
                if (!league.isIncludeIDP)
                    positions = arrayWithFilteredIDPPositionsFromArray(positions);

                if (positions.Count() == 0)
                    continue;

                // Replace the immutable dictionary with a mutable one containing
                // the filtered positions

                IDictionary<String, Object> itemDict = new Dictionary<String, Object>();
                foreach (String key in item.Keys)
                {
                    Type type = item[key].GetType();
                    if (type == typeof(PropertyListArray))
                        itemDict.Add(key, TNUtility.listFromPropertyListArrayItems(item[key].ArrayItems));
                    else if (type == typeof(PropertyListBooleanItem))
                        itemDict.Add(key, item[key].BooleanValue);
                    else if (type == typeof(PropertyListStringItem))
                        itemDict.Add(key, item[key].StringValue);
                }

                String currentKey = TNUtility.getKeyFromArrayOfStrings(positions);

                if (item.Contains("alwaysActive") && item["alwaysActive"].BooleanValue)
                {
                    mutableArray.Add(itemDict);
                }
                else if (currentKey.Equals(wrteKey) && league.isCombineWRTE)
                {
                    mutableArray.Add(itemDict);
                }
                else if ((currentKey.Equals(wrKey) && !league.isCombineWRTE) || (currentKey.Equals(teKey) && !league.isCombineWRTE))
                {
                    mutableArray.Add(itemDict);
                }
                else if (league.isIncludeIDP && isIDPPositionGroup(positions))
                {
                    mutableArray.Add(itemDict);
                }
                else if (league.getRosterValues().ContainsKey(currentKey)
                        && league.getRosterValues()[currentKey].ContainsKey("starters")
                        && (int)league.getRosterValues()[currentKey]["starters"] > 0)
                {
                    mutableArray.Add(itemDict);
                }
            }

            return mutableArray;
        }

    }
}
