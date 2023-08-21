using MyDraftAPI_v2.Managers;
using static MyDraftAPI_v2.FantasyDataModel.FantasyLeage;

namespace MyDraftAPI_v2.DbData.DataModel
{
    public class FilterSort
    {
        public enum SortType
        {
            SortTypeGeneral,
            SortTypePoints,
            SortTypeDVBD,
            SortTypeDVBDRank,
            SortTypeADP,
            SortTypeAAV,
            SortTypeCustom,
            SortTypeStats,
            SortTypeRatings,
        }

        String sortBy;
        String direction;
        String table;
        Boolean useLeagueAsQualifier;
        int year;
        int segment;
        String source;
        Boolean excludeZero;
        public Boolean starterOnly { get; set; }
        public Boolean rookieOnly { get; set; }
        public Boolean excludeInjury { get; set; }
        public PlayerManager.PlayerExperience playerExperience { get; set; }
        public String[] positions { get; set; }
        public PlayerManager.HealthStatus healthStatus { get; set; }
        public DraftManager.PlayerDraftStatus playerDraftStatus { get; set; }
        public SortType sortType
        {
            get
            {
                return _sortType;
            }
            set
            {
                _sortType = value;
                configureForSortType();
            }
        }
        private SortType _sortType;
        public String division { get; set; }
        public int limit { get; set; }

        String buttonTitle;
        String shortTitle;

        public FantasyLeague league { get; set; }

        /* String[] positions, DraftStatus status,
                DBAdapter.HealthStatus health, String division, FantasyLeague league, int limit)
         * */

        private void setDefaultValues()
        {
            this.healthStatus = PlayerManager.HealthStatus.HealthStatusAll;
            this.playerDraftStatus = DraftManager.PlayerDraftStatus.DraftStatusAll;
            this.positions = null;
            this.division = null;
            this.starterOnly = false;
            this.rookieOnly = false;
            this.excludeInjury = false;
            this.playerExperience = PlayerManager.PlayerExperience.experienceAll;
            this.limit = 400;
        }

        public FilterSort(String sortBy, String direction, String table)
        {
            setDefaultValues();
            this.sortBy = sortBy;
            this.direction = direction;
            this.table = table;
        }

        public FilterSort(SortType sortType, FantasyLeague league)
        {
            setDefaultValues();
            this.league = league;
            this.sortType = sortType;
        }
        public FilterSort(SortType sortType)
        {
            setDefaultValues();
            this.league = league;
            this.sortType = sortType;
        }

        private void configureForSortType()
        {
            switch (_sortType)
            {
                case SortType.SortTypePoints:
                    this.sortBy = "points";
                    this.direction = "DESC";
                    this.table = "points";
                    this.useLeagueAsQualifier = true;
                    this.excludeZero = false;
                    this.source = null;
                    this.year = AppSettings.getProjectionStatsYear();
                    this.segment = AppSettings.getProjectionStatsSegment();
                    break;

                case SortType.SortTypeDVBD:
                    this.sortBy = "value";
                    this.direction = "DESC";
                    this.table = "dvbd";
                    this.useLeagueAsQualifier = true;
                    this.excludeZero = false;
                    this.source = null;
                    this.year = AppSettings.getProjectionStatsYear();
                    this.segment = AppSettings.getProjectionStatsSegment();
                    break;

                case SortType.SortTypeADP:
                    this.sortBy = "value_";
                    this.direction = "ASC";
                    this.table = "adp";
                    this.useLeagueAsQualifier = false;
                    this.excludeZero = true;
                    this.source = league.isPPR ? "ppr" : "standard";
                    break;

                case SortType.SortTypeAAV:
                    this.sortBy = "value_standard";
                    this.direction = "DESC";
                    this.table = "aav";
                    this.useLeagueAsQualifier = false;
                    this.excludeZero = false;
                    this.source = null;
                    this.year = AppSettings.getProjectionStatsYear();
                    this.segment = AppSettings.getProjectionStatsSegment();
                    break;

                default:
                    break;
            }
        }

        public static String getTitleForSortType(SortType type)
        {
            String title = null;
            switch (type)
            {
                case SortType.SortTypeAAV:
                    title = "AAV";
                    break;

                case SortType.SortTypeADP:
                    title = "ADP";
                    break;

                case SortType.SortTypeDVBD:
                    title = "VBD";
                    break;

                case SortType.SortTypePoints:
                    title = "Points";
                    break;

                case SortType.SortTypeCustom:
                    title = "Custom";
                    break;

                default:
                    title = "Sort";
                    break;
            }

            return title;
        }

        public static String getShortTitleForSortType(SortType type)
        {
            String title = null;
            switch (type)
            {
                case SortType.SortTypeAAV:
                    title = "AAV";
                    break;

                case SortType.SortTypeADP:
                    title = "ADP";
                    break;

                case SortType.SortTypeDVBD:
                    title = "VBD";
                    break;

                case SortType.SortTypePoints:
                    title = "Pts";
                    break;

                case SortType.SortTypeCustom:
                    title = "CSTM";
                    break;

                default:
                    title = "Sort";
                    break;
            }

            return title;
        }

        public String getSortBy()
        {
            return sortBy;
        }
        public void setSortBy(String sortBy)
        {
            this.sortBy = sortBy;
        }
        public String getDirection()
        {
            return direction;
        }
        public void setDirection(String direction)
        {
            this.direction = direction;
        }
        public String getTable()
        {
            return table;
        }
        public void setTable(String table)
        {
            this.table = table;
        }
        public Boolean isUseLeagueAsQualifier()
        {
            return useLeagueAsQualifier;
        }
        public void setUseLeagueAsQualifier(Boolean useLeagueAsQualifier)
        {
            this.useLeagueAsQualifier = useLeagueAsQualifier;
        }
        public int getYear()
        {
            return year;
        }
        public void setYear(int year)
        {
            this.year = year;
        }
        public int getSegment()
        {
            return segment;
        }
        public void setSegment(int segment)
        {
            this.segment = segment;
        }
        public String getSource()
        {
            return source;
        }
        public void setSource(String source)
        {
            this.source = source;
        }
        public Boolean isExcludeZero()
        {
            return excludeZero;
        }
        public void setExcludeZero(Boolean excludeZero)
        {
            this.excludeZero = excludeZero;
        }
        public String getButtonTitle()
        {
            if (this.buttonTitle != null)
                return this.buttonTitle;
            else
                return FilterSort.getTitleForSortType(this.sortType);
        }
        public void setButtonTitle(String buttonTitle)
        {
            this.buttonTitle = buttonTitle;
        }
        public String getShortTitle()
        {
            if (this.shortTitle != null)
                return this.shortTitle;
            else
                return FilterSort.getShortTitleForSortType(this.sortType);
        }
        public void setShortTitle(String shortTitle)
        {
            this.shortTitle = shortTitle;
        }
    }
}
