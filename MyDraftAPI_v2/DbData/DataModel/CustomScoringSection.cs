using CodeTitans.Core.Generics;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class CustomScoringSection
    {
        private String title;
        private IList<CustomScoringDefaultItem> scoringItems;

        public CustomScoringSection(IPropertyListDictionary item)
        {
            this.title = item["title"].StringValue;
            this.scoringItems = new List<CustomScoringDefaultItem>();

            IEnumerable<IPropertyListDictionary> items = item["items"].ArrayItems;
            foreach (IPropertyListDictionary eachItem in items)
            {
                string key = eachItem["dbKey"].StringValue;
                string title = eachItem["title"].StringValue;
                double value = eachItem["value"].DoubleValue;
                IPropertyListItem divideItem = eachItem.Contains("divide") ? eachItem["divide"] : null;
                Boolean divide = divideItem != null ? eachItem["divide"].BooleanValue : false;
                CustomScoringDefaultItem scoringItem = new CustomScoringDefaultItem(title, key, (float)value, divide);
                this.scoringItems.Add(scoringItem);
            }
        }

        public CustomScoringSection(IDictionary<String, object> dict)
        {
            object dynamicValue;
            dict.TryGetValue("title", out dynamicValue);
            this.title = Convert.ToString(dynamicValue);
            this.scoringItems = new List<CustomScoringDefaultItem>();

            dict.TryGetValue("items", out dynamicValue);
            IList<IDictionary<String, Object>> items = (IList<IDictionary<String, Object>>)dynamicValue;
            foreach (IDictionary<String, Object> item in items)
            {
                item.TryGetValue("dbKey", out dynamicValue);
                String key = (string)dynamicValue;
                item.TryGetValue("title", out dynamicValue);
                String title = (string)dynamicValue;
                item.TryGetValue("value", out dynamicValue);
                float value = (float)dynamicValue;
                item.TryGetValue("divide", out dynamicValue);
                Boolean divide = dynamicValue != null ? (Boolean)dynamicValue : false;
                CustomScoringDefaultItem scoringItem = new CustomScoringDefaultItem(title, key, (float)Convert.ToDouble(value), divide);
                this.scoringItems.Add(scoringItem);
            }
        }

        public String getTitle()
        {
            return title;
        }

        public IList<CustomScoringDefaultItem> getScoringItems()
        {
            return scoringItems;
        }
    }
}
