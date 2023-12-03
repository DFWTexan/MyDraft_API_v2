using MyDraftAPI_v2.Services.Utility.FanAppUtilities;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class RosterPosition
    {
        private String posKey;
        private IList<String> positions;
        private int starters;
        private int max;
        private Boolean flex;
        private IList<Player> players;
        private String title;

        public RosterPosition(IList<String> positions, int starters, int max, Boolean isFlex)
        {
            this.positions = positions;
            this.starters = starters;
            this.max = max;
            this.flex = isFlex;
        }
        public String positionKey()
        {
            return TNUtility.getKeyFromArrayOfStrings(positions);
        }
        public IList<String> getPositions()
        {
            return positions;
        }
        public void setPositions(IList<String> positions)
        {
            this.positions = positions;
        }
        public int getStarters()
        {
            return starters;
        }
        public void setStarters(int starters)
        {
            this.starters = starters;
        }
        public int getMax()
        {
            return max;
        }
        public void setMax(int max)
        {
            this.max = max;
        }
        public Boolean isFlex()
        {
            return this.flex;
        }
        public void setFlex(Boolean isFlex)
        {
            this.flex = isFlex;
        }
        public IList<Player> getPlayers()
        {
            return players;
        }
        public void setPlayers(IList<Player> players)
        {
            this.players = players;
        }
        public String getTitle()
        {
            return title;
        }
        public void setTitle(String title)
        {
            this.title = title;
        }
        public String getPositionKey()
        {
            return posKey;
        }
    }
}
