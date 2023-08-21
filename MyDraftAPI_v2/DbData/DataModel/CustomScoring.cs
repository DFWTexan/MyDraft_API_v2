using CodeTitans.Core.Generics;
using MyDraftAPI_v2.FantasyDataModel;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class CustomScoring
    {
        private String version;
        private IList<CustomScoringGroup> groups;

        public CustomScoring(PropertyList dict)
        {
            this.version = dict["customScoringVersion"].StringValue;
            this.groups = new List<CustomScoringGroup>();

            IEnumerable<IPropertyListDictionary> items = dict["scoring_groups"].ArrayItems;
            foreach (IPropertyListDictionary groupDict in items)
            {
                CustomScoringGroup group = new CustomScoringGroup(groupDict);
                this.groups.Add(group);
            }
        }

        public CustomScoring(IDictionary<String, Object> dict)
        {
            this.version = (String)dict["customScoringVersion"];
            this.groups = new List<CustomScoringGroup>();

            IList<IDictionary<String, Object>> items = (IList<IDictionary<String, Object>>)dict["scoring_groups"];
            foreach (IDictionary<String, Object> groupDict in items)
            {
                CustomScoringGroup group = new CustomScoringGroup(groupDict);
                this.groups.Add(group);
            }
        }

        public String getVersion()
        {
            return version;
        }

        public IList<CustomScoringGroup> getGroups()
        {
            return groups;
        }

        public CustomScoringGroup getCustomScoringGroupWithID(String groupID)
        {
            foreach (CustomScoringGroup group in getGroups())
            {
                if (group.getIdentifier().Equals(groupID, StringComparison.CurrentCultureIgnoreCase))
                    return group;
            }

            return null;
        }
    }
}
