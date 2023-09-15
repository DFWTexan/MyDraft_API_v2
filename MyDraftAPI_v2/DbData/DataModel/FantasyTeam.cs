using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using static MyDraftAPI_v2.FantasyDataModel.FantasyLeage;
using MyDraftAPI_v2.Managers;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class FantasyTeam : Team
    {
        [Column("_id")]
        public int identifier { get; set; }
        [Column("name")]
        public String name { get; set; }
        [Column("abbr")]
        public String abbr { get; set; }
        [Column("owner")]
        public String owner { get; set; }
        [Column("draft_position")]
        public int draftPosition { get; set; }
        [Column("league_id")]
        public int leagueID { get; set; }
        public double budgetAmount { get; set; }
        public int auctionRosterCount { get; set; }

        public String rosterDisplayKey { get; set; }
        //public bool isMyTeam
        //{
        //    get
        //    {
        //        return league.isMyTeam(league.teamWithID(identifier));
        //    }
        //}
        public FantasyLeague league { get; set; }
        //        DraftStrategyVBD draftStrategy;

        public FantasyTeam() { }

        //public FantasyTeam(int identifier)
        //{
        //    this.identifier = identifier;
        //    this.rosterDisplayKey = ROSTER_DISPLAY_MYTEAM;
        //}

        public FantasyTeam(int identifier, String name, String abbr, FantasyLeague league)
        {
            this.identifier = identifier;
            this.name = name;
            this.abbr = abbr;
            this.league = league;
            this.rosterDisplayKey = ROSTER_DISPLAY_MYTEAM;
        }

        //public FantasyLeague getLeague()
        //{
        //    return league;
        //}

        //public void setLeague(FantasyLeague league)
        //{
        //    this.league = league;
        //}

        //public String getOwner()
        //{
        //    return owner;
        //}

        //public void setOwner(String owner)
        //{
        //    this.owner = owner;
        ////}

        //public int getDraftPosition()
        //{
        //    return draftPosition;
        //}

        //public void setDraftPosition(int draftPosition)
        //{
        //    this.draftPosition = draftPosition;
        //}

        //public async Task<IList<String>> getPlayerIDs()
        //{
        //    return await LeagueManager.getPlayerIDsForTeam(this);
        //}

        /* TODO
        public DraftStrategyVBD draftStrategy()
        {
            if (draftStrategy == null)
                draftStrategy = new DraftStrategyVBD(league, this);

            return draftStrategy;
        }

        public String autoPickPlayerForDraftPick(DraftPick draftPick)
        {
            return draftStrategy().autoPickPlayerIDForOverallPick(draftPick.getPickOverall());
        }
          * */
        public async Task<Double> getAuctionAmountSpent()
        {
            return await DraftManager.auctionAmountSpentForTeam(this);
        }

        public async Task<Double> getAuctionAmountAvailable()
        {
            return await DraftManager.auctionAmountAvailableForTeam(this);
        }

        public async Task<Double> getMaxBid()
        {
            return await DraftManager.getMaxBid(identifier);
        }

        public async Task auctionAssign(Player player, string amount)
        {
            await DraftManager.auctionAssignPlayer(this, player, amount);
        }

        public async Task<int> getAuctionRosterCount()
        {
            return await DraftManager.auctionRosterCnt(this);
        }
    }
}
