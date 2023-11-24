namespace DataModel.Enums
{
    public enum Position
    {
        QB = 1,
        RB = 2,
        WR = 4,
        TE = 3,
        PK = 5,
    }
    public enum PlayerSortValue
    {
        ADP = 1,
        AAV = 2,
        DVBD = 3,
        DVBD_Rank = 4,
        Custom = 5,
        Stats = 6,
        Ratings = 7,
    }
    public enum PlayerSortDirection
    {
        Ascending = 1,
        Descending = 2,
    }
    public enum PlayerDraftStatus
    {
        Undrafted = 1,
        Drafted = 2,
        WatchList = 3,
        Removed = 4,
    }
    public enum ProTeams
    {
        ARI = 1,
        ATL = 2,
        BAL = 3,
        BUF = 4,
        CAR = 5,
        CHI = 6,
        CIN = 7,
        CLE = 8,
        DAL = 9,
        DEN = 10,
        DET = 11,
        GB = 12,
        HOU = 13,
        IND = 14,
        JAX = 15,
        KC = 16,
        MIA = 17,
        MIN = 18,
        NE = 19,
        NO = 20,
        NYG = 21,
        NYJ = 22,
        OAK = 23,
        PHI = 24,
        PIT = 25,
        LAC = 26,
        SEA = 27,
        SF = 28,
        LAR = 29,
        TB = 30,
        TEN = 31,
        WAS = 32
    }
   
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
}
