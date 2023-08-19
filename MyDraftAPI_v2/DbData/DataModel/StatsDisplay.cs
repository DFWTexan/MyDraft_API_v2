using MyDraftAPI_v2.Managers;

namespace MyDraftAPI_v2.FantasyDataModel
{
    public class StatsDisplay
    {
        String title;
        String group;
        IList<String> columns;
        IList<String> titles;
        IList<String> tables;

        IDictionary<String, String> numberFormatters;

        public StatsDisplay(String title, String group, IList<IDictionary<String, String>> statsItems)
        {
            this.title = title;
            this.group = group;

            IList<String> columns = new List<String>(statsItems.Count());
            IList<String> titles = new List<String>(statsItems.Count());
            IList<String> tables = new List<String>(statsItems.Count());

            this.numberFormatters = new Dictionary<String, String>();

            foreach (IDictionary<String, String> item in statsItems)
            {
                titles.Add(item["title"]);

                String column = item["db_column"];
                columns.Add(column);

                String table = item["table"];
                if (table != null)
                    tables.Add(item["table"]);

                numberFormatters.Add(column, item["number_format"]);
            }

            if (columns.Count() != tables.Count())
            {
                //[TNAnalytics logEvent:@"STATS_DISPLAY_DATA_MISMATCH" withParameters:[NSDictionary dictionaryWithObject:group forKey:@"stats_group"]];

                this.columns = null;
                this.titles = null;
                this.tables = null;
            }
            else
            {
                this.columns = columns;
                this.titles = titles;
                this.tables = tables;
            }
        }

        public String getTitleForStatKey(String key)
        {
            int index = columns.IndexOf(key);

            if (index >= 0)
                return titles[index];

            return null;
        }

        public String getTableForStatKey(String key, AppSettings.StatsType type)
        {
            String tablePrefix = StatsManager.getTablePrefixForStatsType(type);
            int index = columns.IndexOf(key);

            if (index >= 0)
                return String.Format("%s_%s", tablePrefix, tables[index]);

            return null;
        }

        public String getFormatterForStatKey(String key)
        {
            return numberFormatters[key];
        }

        public String getTitle()
        {
            return title;
        }

        public String getGroup()
        {
            return group;
        }

        public IList<String> getColumns()
        {
            return columns;
        }

        public IList<String> getTitles()
        {
            return titles;
        }

        public IList<String> getTables()
        {
            return tables;
        }

        public IDictionary<String, String> getNumberFormatters()
        {
            return numberFormatters;
        }
    }
}
