using CodeTitans.Core.Generics;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class CustomScoringGroup
    {
        private String identifier;
        private String _title;
        private String dbTable;
        private List<CustomScoringSection> sections;

        public string title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                }
            }
        }

        public CustomScoringGroup(IPropertyListDictionary item)
        {
            this.identifier = item["title"].StringValue;
            this.title = item["title"].StringValue;
            this.dbTable = item["dbTable"].StringValue;
            this.sections = new List<CustomScoringSection>();

            IEnumerable<IPropertyListDictionary> items = item["sections"].ArrayItems;

            foreach (IPropertyListDictionary sectionDict in items)
            {
                CustomScoringSection section = new CustomScoringSection(sectionDict);
                this.sections.Add(section);
            }
        }

        public CustomScoringGroup(IDictionary<String, Object> dict)
        {
            object outValue;
            dict.TryGetValue("title", out outValue);
            this.identifier = (String)outValue;
            dict.TryGetValue("title", out outValue);
            this.title = (String)outValue;
            dict.TryGetValue("dbTable", out outValue);
            this.dbTable = (String)outValue;
            this.sections = new List<CustomScoringSection>();

            dict.TryGetValue("sections", out outValue);
            IList<IDictionary<String, Object>> items = (IList<IDictionary<String, Object>>)outValue;

            foreach (IDictionary<String, Object> sectionDict in items)
            {
                CustomScoringSection section = new CustomScoringSection(sectionDict);
                this.sections.Add(section);
            }
        }

        public String getIdentifier()
        {
            return identifier;
        }

        public String getTitle()
        {
            return title;
        }

        public String getDbTable()
        {
            return dbTable;
        }

        public IList<CustomScoringSection> getSections()
        {
            return sections;
        }

        public IList<CustomScoringDefaultItem> getAllScoringItems()
        {
            IList<CustomScoringDefaultItem> scoringItems = new List<CustomScoringDefaultItem>();
            foreach (CustomScoringSection section in getSections())
            {
                scoringItems = scoringItems.Concat<CustomScoringDefaultItem>(section.getScoringItems()).ToList();
            }

            return scoringItems;
        }
    }
}
