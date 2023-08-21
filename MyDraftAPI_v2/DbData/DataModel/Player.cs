using Database.Model;
using MyDraftAPI_v2.DbData.DataModel;
using MyDraftAPI_v2.Engines;
using MyDraftAPI_v2.FantasyDataModel.Draft;
using MyDraftAPI_v2.Managers;
using MyDraftLib.Utilities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class Player
    {
        [Column("player_id")]
        public String? identifier { get; set; }

        [Column("first_name")]
        public String firstName { get; set; }

        [Column("last_name")]
        public String lastName { get; set; }

        [Column("position")]
        public String position { get; set; }

        [Column("height")]
        public String heightString { get; set; }
        public int height
        {
            get
            {
                String[] heightComponents = heightString.Split('-');
                if (heightComponents.Length == 2)
                {
                    int heightInches = Convert.ToInt16(heightComponents[0]) * 12 + Convert.ToInt16(heightComponents[1]);
                    return heightInches;
                }
                else
                {
                    return Convert.ToInt32(heightString);
                }
            }
        }

        [Column("weight")]
        public int weight { get; set; }

        [Column("birthdate")]
        public String birthdateString { get; set; }
        public DateTime birthdate
        {
            get
            {
                if (_birthDate == DateTime.MinValue)
                {
                    try
                    {
                        _birthDate = Convert.ToDateTime(birthdateString);
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("Invalid birthDate string for player [{0}]", identifier);
                    }
                }
                return _birthDate;
            }
        }
        private DateTime _birthDate;
        public int age
        {
            get
            {
                DateTime today = DateTime.Today;
                int age = today.Year - birthdate.Year;
                if (birthdate > today.AddYears(-age)) age--;
                return age;
            }
        }

        [Column("college")]
        public String college { get; set; }

        [Column("experience")]
        public int experience { get; set; }

        [Column("jersey")]
        public int jersey { get; set; }

        public String photoURL
        {
            get
            {
                return AppUtility.transparentPlayerImageURLForPlayerID(this.identifier);
            }
        }

        [Column("team_abbr")]
        public String teamAbbr { get; set; }

        public String teamID { get; set; }
        [Column("team_abbr")]
        public string team_ID { get; set; }
        public IList<DepthChartStats> stats { get; set; }
        private ProTeam _team;
        private Injury _injurystatus;
        private PlayerValueItem _playervalues;
        private PlayerNote _playerNote;
        private List<PlayerSchedule> _playerSchedule;
        public async Task<ISet<Position>> positions()
        {
            if (_positions == null)
                _positions = (ISet<Position>?)await PlayerManager.positionsForPlayerID(Convert.ToString(this.identifier));

            return _positions;
        }
        private ISet<Position> _positions;

        public async Task<ProTeam> getTeam(string teamID)
        {
            await Task.Delay(2000);

            //if (_team == null)
            //    _team = await DBAdapter.getProTeamWithID(teamID);

            return _team;
        }
        public async Task<ProTeam> getPlayerTeam()
        {
            await Task.Delay(2000);

            //if (_team == null)
            //    _team = await DBAdapter.getProTeamWithID(teamAbbr);

            return _team;
        }

        public async Task<Injury> getInjuryStatus(string playerID)
        {
            if (_injurystatus == null)
                _injurystatus = await PlayerManager.getPlayerInjury(playerID);

            return _injurystatus;
        }

        public async Task<PlayerNote> getPlayerNote(string playerID)
        {
            if (_playerNote == null)
                _playerNote = await PlayerManager.getPlayerNote(playerID);

            return _playerNote;
        }

        public async Task<bool> isHasNote(string _playerID)
        {
            _isPlayerNews = await PlayerManager.isPlayerNote(_playerID);
            return _isPlayerNews;
        }

        public async Task<bool> isWishlist()
        {
            return await DraftManager.getWishlisted(this.identifier, MyDraftEngine.Instance.league);
        }

        public async Task<PlayerValueItem> getPlayerValues(string playerID)
        {
            if (_playervalues == null)
                _playervalues = await PlayerManager.getPlayerValues(playerID);

            return _playervalues;
        }

        public Boolean hasPosition(String position)
        {
            foreach (Position pos in _positions)
            {
                if (pos.name.Equals(position))
                    return true;
            }

            return false;
        }

        public Boolean hasPrimaryPosition(String position)
        {
            if (getPrimaryPositionWithRank(false).Equals(position))
                return true;

            return false;
        }

        public async Task<String> getPrimaryPositionWithRank(Boolean showRank)
        {
            ISet<Position> positions = await this.positions();
            if (positions == null || positions.Count() == 0)
                return "";

            IList<Position> sortedPositions = new List<Position>(positions);
            sortedPositions = sortedPositions.OrderBy(p => p.rank).ToList();

            String result = null;
            if (sortedPositions != null && sortedPositions.Count() > 0)
            {
                // Find the first position that qualifies as a primary. Start off
                // assuming the first position is primary.
                Position primaryPosition = sortedPositions[0];
                foreach (Position eachPos in sortedPositions)
                {
                    if (AppSettings.getPrimaryPositions().Contains(eachPos.name))
                    {
                        primaryPosition = eachPos;
                        break;
                    }
                }

                if (primaryPosition == null)
                    primaryPosition = sortedPositions[0];

                if (showRank && !AppSettings.isIDPPositionName(primaryPosition.name)
                        && !AppSettings.isTeamPosition(primaryPosition.name) && primaryPosition.rank > 0)
                    result = String.Format("{0}{1}", primaryPosition.name, primaryPosition.rank);
                else
                    result = String.Format("{0}", primaryPosition.name);
            }

            return result;
        }

        public async Task<int> getRankForPrimaryPosition()
        {
            return getRankForPosition(await getPrimaryPositionWithRank(false));
        }

        public async Task<List<PlayerSchedule>> getSchedule()
        {
            _playerSchedule = await PlayerManager.getPlayerSchedule(this.identifier, this.teamAbbr);
            return _playerSchedule;
        }

        public int getRankForPosition(String posName)
        {
            foreach (Position pos in _positions)
            {
                if (pos.name.Equals(posName))
                    return pos.rank;
            }

            return -1;
        }

        private static bool _isPlayerNews;
        public async Task<bool> isHasNews(string _playerID)
        {
            _isPlayerNews = await PlayerManager.isPlayerNews(_playerID);
            return _isPlayerNews;
        }
        public async Task<Boolean> isKeeper()
        {
            return await DraftManager.isKeeper(this.identifier, MyDraftEngine.Instance.league);
        }
        public async Task<Boolean> isPositionMax()
        {
            return await PlayerManager.checkPlayerPositionMax(MyDraftEngine.Instance.league, this.position);
        }
        public async Task<bool> isOnMyTeam()
        {
            return await DraftManager.isOnMyTeam(this.identifier, MyDraftEngine.Instance.league);
        }
        public async Task<bool> isDrafted()
        {
            return await DraftManager.isDrafted(this.identifier, MyDraftEngine.Instance.league);
        }
        public async Task<IList<DepthChartStats>> getPlayerStats()
        {
            return (IList<DepthChartStats>)await PlayerManager.getDepthChartStats(int.Parse(this.identifier));
        }
        public async Task<DraftPick> getPlayerDraftInfo()
        {
            return await DraftManager.draftPickForPlayer(this.identifier, MyDraftEngine.Instance.league);
        }
        public async Task<PickSelectionItem> getPlayerWishlistInfo()
        {
            return await DraftManager.getWishlistInfo(this.identifier, int.Parse(MyDraftEngine.Instance.league.identifier.ToString()));
        }
        /*
	String primaryPosition;
	ISet<Position> positions;
	String teamID;
	ProTeam team;
	DateTime birthdate;
	int height;
	int weight;
	String college;
	double sortValue;

	public long getIdentifier() {
		return identifier;
	}
	public String getFirstName() {
		return firstName;
	}
	public void setFirstName(String firstName) {
		this.firstName = firstName;
	}
	public String getLastName() {
		return lastName;
	}
	public void setLastName(String lastName) {
		this.lastName = lastName;
	}
	public String getFullName()
	{
	    return String.Format("%s %s", firstName, lastName);
	}
	public String getPrimaryPosition() {
		return getPrimaryPositionWithRank(false);
	}
	public ISet<Position> getPositions() {
		return positions;
	}
	public void setPositions(ISet<Position> positions) {
		this.positions = positions;
	}
	public String getTeamID() {
		return teamID;
	}
	public void setTeamID(String teamID) {
		this.teamID = teamID;
	}

	public DateTime getBirthdate()
	{
		return birthdate;
	}
	public void setBirthdate(DateTime birthdate)
	{
		this.birthdate = birthdate;
	}
	public int getHeight()
	{
		return height;
	}
	public void setHeight(int height)
	{
		this.height = height;
	}
	public int getWeight()
	{
		return weight;
	}
	public void setWeight(int weight)
	{
		this.weight = weight;
	}
	public String getCollege()
	{
		return college;
	}
	public void setCollege(String college)
	{
		this.college = college;
	}
         * */
        /* TODO player image URL
	public String getPhotoURL()
	{
	    return TNUtility.getPlayerImageURLForPlayerID(Convert.ToString(this.identifier));
	}
        */
        /*
        */
        /*
	public Boolean isInjured()
	{
	    return DBAdapter.isInjuredForPlayerID(this.identifier);
	}
	public String getInjuryStatus()
	{
	    return DBAdapter.getInjuryStatusForPlayerID(this.identifier);
	}
	public String getInjuryType()
	{
	    return DBAdapter.getInjuryTypeForPlayerID(this.identifier);
	}
	public int getInjuryStatusImageResource()
	{
        IDictionary<String, String> status = DBAdapter.getInjuryStatus(this.identifier);
        if (status != null)
            return Integer.parseInt(status.get("imageID"));
        else
            return 0;
	}
    public DraftPick getDraftPick()
    {
        return DraftManager.getDraftPickForPlayerID(String.valueOf(this.identifier), AppSettings.getActiveFantasyLeague());
    }
    public Boolean isDrafted()
    {
        return DraftManager.isDraftedPlayerID(String.valueOf(this.identifier), AppSettings.getActiveFantasyLeague());
    }
    public Boolean isTagged()
    {
        return DraftManager.isTaggedPlayerID(String.valueOf(this.identifier), AppSettings.getActiveFantasyLeague());
    }
    
    public float getValueForSort(FilterSort sort, FantasyLeague league)
    {
        return DBAdapter.getSortValueForPlayerID(String.valueOf(this.identifier), sort, league);
    }
    
    public void setSortValue(double sortValue)
    {
        this.sortValue = sortValue;
    }
    
    public double getSortValue()
    {
        return sortValue;
    }
    
    public float getAAV(FantasyLeague league, boolean adjustedValue)
    {
        return DBAdapter.getAAVForPlayerID(String.valueOf(this.getIdentifier()), league, adjustedValue);
    }
         * */
    }
}
