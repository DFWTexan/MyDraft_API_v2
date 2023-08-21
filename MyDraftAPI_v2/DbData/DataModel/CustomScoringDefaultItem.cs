namespace MyDraftAPI_v2.FantasyDataModel
{
    public class CustomScoringDefaultItem
    {
        public String title { get; set; }
        public String key { get; set; }
        public float value { get; set; }
        public bool divide { get; set; }

        public CustomScoringDefaultItem(String title, String key, float value, bool divide)
        {
            this.title = title;
            this.key = key;
            this.value = value;
            this.divide = divide;
        }

        public String getTitle()
        {
            return title;
        }

        public String getKey()
        {
            return key;
        }

        public float getValue()
        {
            return value;
        }

        public bool isDivide()
        {
            return divide;
        }
    }
}
