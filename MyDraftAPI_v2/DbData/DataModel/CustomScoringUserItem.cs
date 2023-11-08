using System.ComponentModel;
#pragma warning disable 

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class CustomScoringUserItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _leagueID;
        public int leagueID
        {
            get
            {
                return _leagueID;
            }

            set
            {
                if (_leagueID != value)
                {
                    _leagueID = value;
                    RaisePropertyChanged("leagueID");
                }
            }
        }
        private String _position;
        public String position
        {
            get
            {
                return _position;
            }

            set
            {
                if (_position != value)
                {
                    _position = value;
                    RaisePropertyChanged("position");
                }
            }
        }
        private String _key;
        public String key
        {
            get
            {
                return _key;
            }

            set
            {
                if (_key != value)
                {
                    _key = value;
                    RaisePropertyChanged("key");
                }
            }
        }
        private float _value;
        public float value
        {
            get
            {
                return _value;
            }

            set
            {
                if (_value != value)
                {
                    _value = value;
                    RaisePropertyChanged("value");
                }
            }
        }

        public String title { get; set; }
        public String typeValue { get; set; }
        public CustomScoringUserItem()
        {

        }

        public CustomScoringUserItem(int leagueID, String position, String key, float value)
        {
            this.leagueID = leagueID;
            this.position = position;
            this.key = key;
            this.value = value;
        }

        public float getValue()
        {
            return value;
        }

        public void setValue(float value)
        {
            this.value = value;
        }

        public void setLeagueID(int leagueID)
        {
            this.leagueID = leagueID;
        }

        public int getLeagueID()
        {
            return leagueID;
        }

        public String getPosition()
        {
            return position;
        }

        public String getKey()
        {
            return key;
        }

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
