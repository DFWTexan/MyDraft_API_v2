using DataModel.Response;
using DraftService;
using Microsoft.AspNetCore.SignalR;
using MyDraftAPI_v2.FantasyDataModel;
using MyDraftAPI_v2.FantasyDataModel.Draft;
using MyDraftAPI_v2.Managers;

namespace MyDraftAPI_v2
{
    public class DraftEngine_v2 : IDisposable
    {
        //private readonly DraftSvc _draftSvc;
        private List<DraftPick> _draftPicks;
        private DraftStatus _draftStatus;
        private IDictionary<int, DraftPick> _draftPickMap;

        public DraftEngine_v2(ILogger<DraftEngine_v2> logger, Microsoft.Extensions.Hosting.IHostApplicationLifetime appLifetime, IConfiguration config)
        {
            _draftPicks = new List<DraftPick>();
            _draftPickMap = new Dictionary<int, DraftPick>();
            //_draftSvc = draftSvc;
        }

        public async Task InitializeLeagueData_v2()
        {
            int leagueID = 1;

            //_draftStatus = await _draftSvc.DraftStatus(leagueID);
            //_draftPicks = _draftSvc.draftPicksForLeague(leagueID).ToList();
            //_draftPickMap = new Dictionary<int, DraftPick>(_draftPicks.Count);

            //foreach (DraftPick draftPick in _draftPicks)
            //{
            //    _draftPickMap.Add(draftPick.overall, draftPick);
            //}

            //_typeAuction = await DraftManager.isAuctionDraft();

            //await Task.Run(() => calculateCustomScoringAsync());

            //var result = new ReturnResult
            //{
            //    Success = true,
            //    StatusCode = 200
            //};

            //return result;
        }

        //public async Task calculateCustomScoringAsync()
        //{
        //    //int week = AppSettings.getProjectionStatsSegment();
        //    int week = 1;
        //    //if (!(await _league.isCustomScoringCalculated(week)))
        //    //{
        //    //    if (WillCalculateCustomScoring != null)
        //    //        WillCalculateCustomScoring(this, league);

        //    //    int year = AppSettings.getProjectionStatsYear();
        //    //    await _league.processCustomRankings(year, week);

        //    //    if (DidCalculateCustomScoring != null)
        //    //        DidCalculateCustomScoring(this, league);
        //    //}
        //}

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
