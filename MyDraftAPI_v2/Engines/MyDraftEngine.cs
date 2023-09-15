using DataModel.Response;
using DraftService;
using MyDraftAPI_v2.FantasyDataModel;
using MyDraftAPI_v2.FantasyDataModel.Draft;
using MyDraftAPI_v2.Managers;
using Windows.Storage;
using static MyDraftAPI_v2.FantasyDataModel.FantasyLeage;

namespace MyDraftAPI_v2.Engines
{
    public class MyDraftEngine
    {

        #region // Public Properties //
        public FantasyLeague league
        {
            get
            {
                return _league;
            }
        }
        public IList<DraftPick> draftPicks
        {
            get
            {
                return _draftPicks;
            }
        }
        public DraftStatus draftStatus
        {
            get
            {
                return _draftStatus;
            }
        }
        public string sDisplayMode { get; set; }
        public IList<LineupItem> lineup
        {
            get
            {
                return _lineup;
            }
        }
        private List<LineupItem> _lineup;
        #endregion // Public Properties //

        private FantasyLeague _league;
        private List<DraftPick> _draftPicks;
        private IDictionary<int, DraftPick> _draftPickMap;
        private DraftStatus _draftStatus;
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static bool _typeAuction;

        private static MyDraftEngine instance;

        private MyDraftEngine() { }

        public static MyDraftEngine Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MyDraftEngine();
                }
                return instance;
            }
        }

        #region // Delegates //
        /* Network */
        public delegate void DataDownloadStartEventHandler();
        public delegate void DataDownloadEndEventHandler();
        /* League */
        public delegate void OtherLeagueSelectEventHandler();
        public delegate void OtherLeagueCompleteEventHandle();
        public delegate void LeagueEditEventHandler();
        public delegate void ChangeLeagueEventHandler(MyDraftEngine engine, FantasyLeague league);
        public delegate void WillCalculateCustomScoringEventHandler(MyDraftEngine engine, FantasyLeague league);
        public delegate void DidCalculateCustomScoringEventHandler(MyDraftEngine engine, FantasyLeague league);
        public delegate void ChangeOnTheClockEventHandler(MyDraftEngine engine, DraftPick draftPick);
        public delegate void ChangeDraftPickEventHandler(MyDraftEngine engine, DraftPick draftPick);
        public delegate void TagPlayerEventHandler(MyDraftEngine engine, String playerID);
        public delegate void UnTagPlayerEventHandler(MyDraftEngine engine, String playerID);
        public delegate void CompeletedDraftEventHandler(MyDraftEngine engine);
        public delegate void ModifyTeamsEventHandler();
        public delegate void TeamsModifiedEventHandler();
        public delegate void ModifyTeamsBackEventHandler();
        /* Scoring */
        public delegate void DraftSettingBackEventHandler();
        public delegate void ScoringBackClickEventHandler();
        public delegate void ScoringButtonClickEventHandler();
        public delegate void ScoringCategorySelectedEventHandler(IDictionary<string, object> scoreObj);
        public delegate void ScoringCategorySavedEventHandler();
        public delegate void EditScoringItemBackEventHandler();
        /* Roster */
        public delegate void EditRosterSettingEventHandler();
        public delegate void EditRosterBackEventHandler();
        /* Player - Search */
        public delegate void PlayerSearchChoosenEventHandler(string playerID);
        /* Player Profile */
        public delegate void FanTeamAssignEventHandler(FantasyTeam fanTeam, Player player);
        public delegate void DraftButtonEventHandler();
        public delegate void AssignPlayerCancelEventHandler(Object sender);
        public delegate void ToDoPlayerListRefreshEventHandler();
        /* Draft Info */
        public delegate void DraftSelectionsLoadEventHandler();
        public delegate void FanTeamRosterSelectEventHandler(FantasyTeam fanTeam);
        public delegate void FantTeamRosterDetailBackEventHandler();
        public delegate void WishlistClickEventHandler(Player player);
        public delegate void WishlistUpdatedEventHandler(Player player);
        public delegate void WishlistRoundTargetEventHandler();
        public delegate void WishlistTargetSelectEventHandler(WishPlayer wishPlayer);
        /* Auction Info */
        public delegate void FanTeamAuctionBudgetInfoSelectEventHandler();
        public delegate void ViewAuctionPicksEventHandler();
        public delegate void AuctionAssignedEventHandler();
        /* NFL News & Info */
        public delegate void NFLInfoPlayerSelectEventHandler(string playerID);
        #endregion // Delegates //

        #region // EventHandlers //
        /* Network */
        public event DataDownloadStartEventHandler DataDownloadStarted;
        public event DataDownloadEndEventHandler DataDownloadEnded;
        /* League */
        public event OtherLeagueSelectEventHandler OtherLegueSelected;
        public event OtherLeagueCompleteEventHandle OtherLeagueCompleted;
        public event LeagueEditEventHandler LeagueEdit;
        public event ChangeLeagueEventHandler DidChangeLeague;
        public event WillCalculateCustomScoringEventHandler WillCalculateCustomScoring;
        public event DidCalculateCustomScoringEventHandler DidCalculateCustomScoring;
        public event ChangeOnTheClockEventHandler DidChangeOnTheClock;
        public event ChangeDraftPickEventHandler DidChangeDraftPick;
        public event TagPlayerEventHandler DidTagPlayer;
        public event UnTagPlayerEventHandler DidUnTagPlayer;
        public event CompeletedDraftEventHandler DidCompleteDraft;
        public event ModifyTeamsEventHandler ModifyTeamsClicked;
        public event TeamsModifiedEventHandler TeamsModifiyDone;
        public event ModifyTeamsBackEventHandler ModifyTeamsBack;
        /* Scoring */
        public event DraftSettingBackEventHandler DraftSettingBack;
        public event ScoringBackClickEventHandler ScoringCategoryBack;
        public event ScoringButtonClickEventHandler ScoringButtonClicked;
        public event ScoringCategorySelectedEventHandler ScoringCategorySelected;
        public event ScoringCategorySavedEventHandler ScoringCategorySaved;
        public event EditScoringItemBackEventHandler EditScoringItemBack;
        /* Roster */
        public event EditRosterSettingEventHandler EditRosterButtonClick;
        public event EditRosterBackEventHandler EditRosterButtonBackClick;
        /* Player - Search */
        public event PlayerSearchChoosenEventHandler PlayerSearchChoosen;
        /* Player Profile */
        public event FanTeamAssignEventHandler AssignTeamSelect;
        public event DraftButtonEventHandler DraftButtonClicked;
        public event AssignPlayerCancelEventHandler AssignPlayerCancelClicked;
        public event ToDoPlayerListRefreshEventHandler ToDoPlayerListRefresh;
        /* Draft Info */
        public event DraftSelectionsLoadEventHandler DraftSelectionsLoaded;
        public event FanTeamRosterSelectEventHandler FantasyTeamRosterSelect;
        public event FantTeamRosterDetailBackEventHandler FanTeamRosterDetailBack;
        public event WishlistClickEventHandler WishlistClick;
        public event WishlistUpdatedEventHandler WishlistUpdate;
        public event WishlistRoundTargetEventHandler WishlistRoundTarget;
        public event WishlistTargetSelectEventHandler WishlistTargetSelect;
        /* Auction Info */
        public event FanTeamAuctionBudgetInfoSelectEventHandler FantasyTeamAuctionInfoSelect;
        public event ViewAuctionPicksEventHandler ViewAuctionPiksSelect;
        public event AuctionAssignedEventHandler PlayerAuctionAssigned;
        /* NFL News & Info */
        public event NFLInfoPlayerSelectEventHandler NFLPlayerInfoSelect;
        #endregion // EventHandlers //

        #region // Raise Events //
        /* Network */
        private void RaiseDownloadStart()
        {
            DataDownloadStarted?.Invoke();
        }
        private void RaiseDownloadEnd()
        {
            //DataDownloadEnded?.Invoke();
            if (DataDownloadEnded != null)
                DataDownloadEnded();
        }
        /* League */
        private void RaiseOtherLeagueSelect()
        {
            OtherLegueSelected?.Invoke();
        }
        private void RaiseOtherLeagueComplete()
        {
            OtherLeagueCompleted?.Invoke();
        }
        private void RaiseLeagueEdit()
        {
            LeagueEdit?.Invoke();
        }
        private void RaiseLeagueChanged(FantasyLeague league)
        {
            if (DidChangeLeague != null)
                DidChangeLeague(this, league);
        }
        private void RaiseDidOTC()
        {
            if (DidChangeOnTheClock != null)
            {
                DidChangeOnTheClock(this, onTheClockDraftPick());
            }
        }
        private void RaiseModifyTeamClicked()
        {
            ModifyTeamsClicked?.Invoke();
        }
        private void RaiseTeamsModifiyDone()
        {
            TeamsModifiyDone?.Invoke();
        }
        private void RaiseModifyTeamsBack()
        {
            ModifyTeamsBack?.Invoke();
        }
        /* Scoring */
        private void RaiseDraftSettingBackClick()
        {
            DraftSettingBack?.Invoke();
        }
        private void RaiseScoringBackCliked()
        {
            ScoringCategoryBack?.Invoke();
        }
        private void RaiseScoringButtonCliked()
        {
            ScoringButtonClicked?.Invoke();
        }
        private void RaiseScoringCategorySelectedCliked(IDictionary<string, object> scoreObj)
        {
            if (ScoringCategorySelected != null)
                ScoringCategorySelected(scoreObj);
        }
        private void RaiseScoringCategorySavedCliked()
        {
            ScoringCategorySaved?.Invoke();
        }
        private void RaiseEditScoringItemBack()
        {
            EditScoringItemBack?.Invoke();
        }
        /* Roster */
        private void RaiseRosterButtonClick()
        {
            EditRosterButtonClick?.Invoke();
        }
        private void RaiseRosterBackButtonClick()
        {
            EditRosterButtonBackClick?.Invoke();
        }
        /* Player - Search */
        private void RaisePlayerSearchChoosen(string playerID)
        {
            if (PlayerSearchChoosen != null)
                PlayerSearchChoosen(playerID);
        }
        /* Player Profile */
        private void RaiseAssignTeamSelect(FantasyTeam fanTeam, Player player)
        {
            if (AssignTeamSelect != null)
                AssignTeamSelect(fanTeam, player);
        }
        private void RaiseDraftButtonClick()
        {
            DraftButtonClicked?.Invoke();
        }
        private void RaiseAssignPlayerCancelClick(Object sender)
        {
            if (AssignPlayerCancelClicked != null)
                AssignPlayerCancelClicked(sender);
        }
        private void RaisePlayerListRefresh()
        {
            ToDoPlayerListRefresh?.Invoke();
        }
        /* Draft Info */
        private void RaiseDraftSelectionsLoaded()
        {
            DraftSelectionsLoaded?.Invoke();
        }
        private void RaiseFanTeamRosterSelect(FantasyTeam fanTeam)
        {
            if (FantasyTeamRosterSelect != null)
                FantasyTeamRosterSelect(fanTeam);
        }
        private void RaisFanTeamDetailRosterBack()
        {
            FanTeamRosterDetailBack?.Invoke();
        }
        private void RaiseWishlist(Player player)
        {
            if (WishlistClick != null)
                WishlistClick(player);
        }
        private void RaiseWishlistUpdate(Player player)
        {
            if (WishlistUpdate != null)
                WishlistUpdate(player);
        }
        private void RaiseWishlistRoundTarget()
        {
            if (WishlistRoundTarget != null)
                WishlistRoundTarget();
        }
        private void RaiseWishlistTargetSelect(WishPlayer wishPlayer)
        {
            if (WishlistTargetSelect != null)
                WishlistTargetSelect(wishPlayer);
        }
        /* Auction Info */
        private void RaiseFanTeamAuctionInfo()
        {
            FantasyTeamAuctionInfoSelect?.Invoke();
        }
        private void RaiseViewAuctionPicks()
        {
            ViewAuctionPiksSelect?.Invoke();
        }
        private void RaisePlayerAuctionAssigned()
        {
            PlayerAuctionAssigned?.Invoke();
        }
        /* NFL News & Info */
        private void RaiseNFLInfoPlayerSelec(string playerID)
        {
            if (NFLPlayerInfoSelect != null)
                NFLPlayerInfoSelect(playerID);
        }
        #endregion // Raise Events //

        #region // Network //

        #endregion // Network //

        #region // League Edit //
        public void OtherLeagueClick()
        {
            RaiseOtherLeagueSelect();
        }
        public void OtherLeagueSelectComplete()
        {
            RaiseOtherLeagueComplete();
        }
        public void RaiseChangeLeague(FantasyLeague league)
        {
            RaiseLeagueChanged(league);
        }
        public void LeagueEditClick()
        {
            RaiseLeagueEdit();
        }
        public void ModifyTeamsBackClick()
        {
            RaiseModifyTeamsBack();
        }
        public void ModifyTeamsDone()
        {
            RaiseTeamsModifiyDone();
        }
        public void ModifyTeamsClick()
        {
            RaiseModifyTeamClicked();
        }
        #endregion // League Edit //

        #region // Scoring Edit //
        public void DraftSettingBackClick()
        {
            RaiseDraftSettingBackClick();
        }
        public void ScoringEditBackClick()
        {
            RaiseScoringBackCliked();
        }
        public void ScoringEditButtonClick()
        {
            RaiseScoringButtonCliked();
        }
        public void ScoringCategorySelect(IDictionary<string, object> scoreObj)
        {
            RaiseScoringCategorySelectedCliked(scoreObj);
        }
        public void ScoringCategorySave()
        {
            RaiseScoringCategorySavedCliked();
        }
        public void EditScoringItemBackClick()
        {
            RaiseEditScoringItemBack();
        }
        #endregion // Scoring Edit //

        #region // Roster Edit //
        public void RosterEditButtonClick()
        {
            RaiseRosterButtonClick();
        }
        public void RosterButtonBackClick()
        {
            RaiseRosterBackButtonClick();
        }
        #endregion // Roster Edit //

        #region // Player - Search //
        public void SearchPlayerSelected(string playerID)
        {
            RaisePlayerSearchChoosen(playerID);
        }
        #endregion // Player - Search //

        #region // Player Profile //
        public void AssignPlayerTeamSelect(FantasyTeam fanTeam, Player player)
        {
            RaiseAssignTeamSelect(fanTeam, player);
        }
        public void AssignPlayerCancel(object sender)
        {
            RaiseAssignPlayerCancelClick(sender);
        }
        public void DraftButtonClick()
        {
            RaiseDraftButtonClick();
        }
        public void ToDoPlayerList()
        {
            RaisePlayerListRefresh();
        }
        #endregion // Player Profile //

        #region // Auction Info //
        public void FantasyTeamAuctionInfoSelected()
        {
            RaiseFanTeamAuctionInfo();
        }
        public void ViewAuctionPicksSelected()
        {
            RaiseViewAuctionPicks();
        }
        public void PlayerAssignedAuction()
        {
            RaisePlayerAuctionAssigned();
        }
        #endregion // Auction Info //

        #region // NFL News * Info //
        public void NFLInfoPlayerSelected(string playerID)
        {
            RaiseNFLInfoPlayerSelec(playerID);
        }
        #endregion // NFL News * Info //

        #region // Draft Info //
        public void DraftPickSeletionsLoaded()
        {
            RaiseDraftSelectionsLoaded();
        }
        public void FantasyTeamRosterSelected(FantasyTeam fanTeam)
        {
            RaiseFanTeamRosterSelect(fanTeam);
        }
        public void FanTeamDetailRosterBackClicked()
        {
            RaisFanTeamDetailRosterBack();
        }
        public void WishlistClicked(Player player)
        {
            RaiseWishlist(player);
        }
        public void WishlistUpdated(Player player)
        {
            RaiseWishlistUpdate(player);
        }
        public void WishlistRoundTargeted()
        {
            RaiseWishlistRoundTarget();
        }
        public void WishlistTargetSelected(WishPlayer wishPlayer)
        {
            RaiseWishlistTargetSelect(wishPlayer);
        }
        #endregion // Draft Selections //
               
        #region // Initialization //
        public async Task InitRosterLineup(FantasyTeam fanteam)
        {
            await rosterLineup(fanteam);
        }
        #endregion // Initialization //

        #region //  Setup  //
        public async Task Initialize()
        {
            FantasyLeague league = await getActiveFantasyLeague();
            if (league == null)
            {
                league = await LeagueManager.createLeague();
            }
            await SetActiveLeague(league);

        }
        public async Task SetActiveLeague(FantasyLeague league)
        {
            if (league == _league)
                return;

            _league = league;
            localSettings.Values["last_active_league"] = _league.identifier;
            await initializeLeagueData();

            RaiseLeagueChanged(league);

            DraftPick otcDraftPick = onTheClockDraftPick();
            if (otcDraftPick == null || otcDraftPick.playerID != null || (!_draftStatus.isComplete && otcDraftPick.overall > 0))
            {
                await updateOnTheClock();
            }
            RaiseDownloadStart();
        }
        private async Task<FantasyLeague> getActiveFantasyLeague()
        {
            if (_league != null)
            {
                return _league;
            }

            int activeLeagueID = localSettings.Values.ContainsKey("last_active_league") ? (int)localSettings.Values["last_active_league"] : 0;
            FantasyLeague fan = new FantasyLeague();
            await fan.rosterInitialize(activeLeagueID);
            if (activeLeagueID != 0)
            {
                AppSettings.isRosterValueNew = false;
            }
            else
            {
                AppSettings.isRosterValueNew = true;
            }
            return await LeagueManager.getLeagueWithID(activeLeagueID);
        }
        private async Task initializeLeagueData()
        {
            _draftStatus = await DraftManager.draftStatus(_league.identifier);
            _draftPicks = (List<DraftPick>)await DraftManager.draftPicksForLeague(_league);
            _draftPickMap = new Dictionary<int, DraftPick>(_draftPicks.Count);

            foreach (DraftPick draftPick in _draftPicks)
            {
                _draftPickMap.Add((int)draftPick.overall, draftPick);
            }
            _typeAuction = await DraftManager.isAuctionDraft();
            await Task.Run(() => calculateCustomScoringAsync());
        }
        public ReturnResult InitializeLeagueData_v2(Database.Model.UserLeague vInput)
        {
            var result = new ReturnResult
            {
                Success = true,
                StatusCode = 200
            };

            return result;
        }
        public async Task reload()
        {
            await initializeLeagueData();
            if (DidChangeLeague != null)
            {
                DidChangeLeague(this, _league);
            }
            await updateOnTheClock();
        }
        public async Task calculateCustomScoringAsync()
        {
            int week = AppSettings.getProjectionStatsSegment();
            if (!(await _league.isCustomScoringCalculated(week)))
            {
                if (WillCalculateCustomScoring != null)
                    WillCalculateCustomScoring(this, league);

                int year = AppSettings.getProjectionStatsYear();
                await _league.processCustomRankings(year, week);

                if (DidCalculateCustomScoring != null)
                    DidCalculateCustomScoring(this, league);
            }
        }
        #endregion //  Setup  //

        #region //  Data Helpers  //
        public DraftPick onTheClockDraftPick()
        {
            return draftPickForOverall(_draftStatus.onTheClock);
        }
        public DraftPick draftPickForPlayerID(String playerID)
        {
            if (playerID == null)
            {
                return null;
            }

            foreach (DraftPick draftPick in _draftPicks)
            {
                if (playerID.Equals(draftPick.playerID))
                {
                    return draftPick;
                }
            }
            return null;
        }
        public bool hasDraftedPlayers()
        {
            foreach (DraftPick pick in this.draftPicks)
            {
                if (pick.playerID != null)
                {
                    return true;
                }
            }
            return false;
        }
        public DraftPick nextAvailableDraftPickAfterOverall(int overall)
        {
            int totalPicks = _draftPicks.Count;
            if (totalPicks <= overall)
            {
                return null;
            }

            // Iterate over all draft picks after the overall pick. Stop when an empty spot (playerID == nil) is found.
            int startIndex = overall >= 0 ? overall : 0; // Start at 0 or greater
            for (int i = startIndex; i < totalPicks; i++)
            {
                DraftPick draftPick = _draftPicks[i];
                if (draftPick.playerID == null)
                {
                    return draftPick;
                }
            }
            return null;
        }
        public DraftPick nextAvailableDraftPickForTeam(FantasyTeam team)
        {
            return nextAvailableDraftPickForTeam(team, _draftStatus.onTheClock);
        }
        public DraftPick nextAvailableDraftPickForTeam(FantasyTeam team, int startingAtOverall)
        {
            IList<DraftPick> draftPicksForTeam = this.draftPicksForTeam(team);

            foreach (DraftPick draftPick in draftPicksForTeam)
            {
                int teamOverallPick = (int)draftPick.overall;
                if ((teamOverallPick >= startingAtOverall) && draftPick.playerID == null)
                {
                    return draftPick;
                }
            }

            return null;
        }
        public IList<DraftPick> draftPicksForTeam(FantasyTeam team)
        {
            IList<DraftPick> draftPicks = new List<DraftPick>(_league.numTeams);
            foreach (DraftPick draftPick in _draftPicks)
            {
                if (draftPick.teamID == team.identifier)
                {
                    draftPicks.Add(draftPick);
                }
            }
            return draftPicks;
        }
        public DraftPick draftPickForOverall(int overall)
        {
            return _draftPickMap.ContainsKey(overall) ? _draftPickMap[overall] : null;
        }
        #endregion //  Data Helpers  //

        #region //  Draft Pick Manipulation  //
        public async Task setOnTheClock(int overall)
        {
            if (_draftStatus.onTheClock == overall)
            {
                return;
            }

            DraftPick otcPick = draftPickForOverall(overall);
            if (otcPick != null)
            {
                _draftStatus.onTheClock = (int)otcPick.overall;
                await DraftManager.saveDraftStatus(_draftStatus);

                //if (DidChangeOnTheClock != null)
                //{
                //    DidChangeOnTheClock(this, onTheClockDraftPick());
                //}
                RaiseDidOTC();
            }
        }
        public async Task changeDraftPickToTeam(int overall, FantasyTeam team)
        {
            DraftPick draftPick = draftPickForOverall(overall);
            draftPick.teamID = team.identifier;
            await DraftManager.saveDraftPick(draftPick);

            if (DidChangeDraftPick != null)
            {
                DidChangeDraftPick(this, draftPick);
            }
        }
        public async Task resetDraftPick(String playerID)
        {
            DraftPick draftPick = draftPickForPlayerID(playerID);
            if (draftPick == null)
            {
                return;
            }

            if (_league.draftByTeamEnabled && _league.draftOrderType != FantasyLeague.DraftOrderType.auction)
            {
                resetDraftPick(draftPick);
                await DraftManager.saveDraftPick(draftPick);
            }
            else
            {
                await DraftManager.deleteDraftPick(draftPick);
                _draftPicks.Remove(draftPick);
                _draftPickMap.Remove((int)draftPick.overall);
            }

            if (DidChangeDraftPick != null)
            {
                DidChangeDraftPick(this, draftPick);
            }
        }
        public void resetDraftPick(DraftPick draftPick)
        {
            draftPick.playerID = null;
            draftPick.isKeeper = false;
            draftPick.auctionValue = 0;
        }
        /*
        public void resetDraftData()
        {
            DraftManager.resetDraftData(_league);
            initializeLeagueData();
            updateOnTheClock();

            if (DidResetLeague != null) {
                DidResetLeague();
            }
        }
         * */
        public async Task updateOnTheClock()
        {
            DraftPick otcPick = onTheClockDraftPick();
            if (otcPick != null && otcPick.playerID == null)
                return;

            int otcOverall = otcPick != null ? (int)otcPick.overall : 0;
            DraftPick nextOTC = nextAvailableDraftPickAfterOverall(otcOverall);
            if (nextOTC == null)
            {
                await setDraftComplete();
                return;
            }

            if (otcPick == null || nextOTC.overall != otcPick.overall)
            {
                _draftStatus.onTheClock = (int)nextOTC.overall;
                otcPick = onTheClockDraftPick();

                // Update the OTC pick. If none can be found then declare the draft complete.
                if (otcPick != null)
                {
                    _draftStatus = new DraftStatus(_league.identifier, _draftStatus.onTheClock, 0, false);
                    await DraftManager.saveDraftStatus(_draftStatus);
                }
                else
                {
                    await setDraftComplete();
                }
            }
        }
        private async Task setDraftComplete()
        {
            _draftStatus = new DraftStatus(_league.identifier, _league.rounds * _league.numTeams + 1, 0, true); // Set on the clock to 1 pick beyond the end of the draft
            await DraftManager.saveDraftStatus(_draftStatus);

            if (DidCompleteDraft != null)
            {
                DidCompleteDraft(this);
            }
        }
        #endregion //  Draft Pick Manipulation  //

        #region //  Draft Picks  //
        public async Task executeDraftPickOnTheClock(String playerID)
        {
            await executeDraftPick(_draftStatus.onTheClock, playerID);
        }
        public async Task executeDraftPick(int overall, String playerID)
        {
            await executeDraftPick(overall, playerID, false);
        }
        public async Task executeDraftPick(FantasyTeam team, String playerID)
        {
            DraftPick draftPick = nextAvailableDraftPickForTeam(team);
            await executeDraftPick((int)draftPick.overall, playerID);
        }
        public async Task executeDraftPick(int overall, String playerID, bool isKeeper)
        {
            DraftPick draftPick = draftPickForOverall(overall);
            if (draftPick == null)
                return;

            DraftPickMemento draftPickMemento = draftPick.getState();
            DraftMemento draftMemento = new DraftMemento();
            draftMemento.leagueID = _league.identifier;
            draftMemento.onTheClock = _draftStatus.onTheClock;
            draftMemento.draftPickMemento = draftPickMemento;
            //await DraftManager.pushDraftMementoToUndoStack(draftMemento);
            await DraftManager.clearDraftMementoRedoStack(_league.identifier);

            draftPick.playerID = playerID;
            draftPick.isKeeper = isKeeper;
            await DraftManager.saveDraftPick(draftPick);

            DraftPick otcPick = onTheClockDraftPick();
            if (otcPick.playerID != null)
            {
                await updateOnTheClock();
            }

            if (DidChangeDraftPick != null)
            {
                DidChangeDraftPick(this, draftPick);
            }
        }
        public async Task executeAuctionPick(int teamID, String playerID, float auctionValue)
        {
            await executeAuctionPick(teamID, playerID, auctionValue, false);
        }
        public async Task executeAuctionPick(int teamID, String playerID, float auctionValue, bool isKeeper)
        {
            if (teamID <= 0 || playerID == null)
                return;

            DraftPick draftPick = new DraftPick();
            draftPick.overall = await DraftManager.maxOverall(_league.identifier) + 1;
            draftPick.teamID = teamID;
            draftPick.playerID = playerID;
            draftPick.auctionValue = auctionValue;
            draftPick.isKeeper = isKeeper;

            await DraftManager.saveDraftPick(draftPick);
            _draftPicks.Add(draftPick);
            _draftPickMap.Add((int)draftPick.overall, draftPick);

            if (DidChangeDraftPick != null)
            {
                DidChangeDraftPick(this, draftPick);
            }
        }
        public async Task generateDraftPicks()
        {
            IList<DraftPick> draftPicks = (List<DraftPick>)DraftPickGenerator.generateDraftPicks(_league);
            await DraftManager.deleteDraftPicksForLeague(_league);
            await DraftManager.saveDraftPicks(draftPicks);
            await initializeLeagueData();
        }
        private async Task rosterLineup(FantasyTeam fanteam)
        {
            _lineup = (List<LineupItem>)await LeagueManager.getLineup(fanteam);
        }
        #endregion //  Draft Picks  //

        #region //  Tag  //
        public async Task tagPlayer(String playerID, int pickround)
        {
            await DraftManager.tagPlayer(playerID, _league, pickround);

            if (DidTagPlayer != null)
            {
                DidTagPlayer(this, playerID);
            }
        }
        public async Task unTagPlayer(String playerID)
        {
            await DraftManager.unTagPlayer(playerID, _league);

            if (DidUnTagPlayer != null)
            {
                DidUnTagPlayer(this, playerID);
            }
        }
        #endregion //  Tag  //
                
        #region // Player Note //
        public static async Task playerNoteSave(Player player, int leagueID, string note)
        {
            await PlayerManager.savePlayerNote(player, leagueID, note);
        }
        #endregion // Player Note //

    }
}
