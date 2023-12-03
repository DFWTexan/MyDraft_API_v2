using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using MyDraft.FantasyLib.Database;
using Windows.Storage;
using System.Reflection;
using System;
//-------------------------------//
using CodeTitans.Core.Generics;

namespace MyDraftAPI_v2.Services.Utility.FanAppUtilities
{
    public static class TNUtility
    {
        public static long ONE_MINUTE = 60;
        public static long ONE_HOUR = 60 * ONE_MINUTE;
        public static long ONE_DAY = 24 * ONE_HOUR;
        public static long ONE_WEEK = 7 * ONE_DAY;
        public static long ONE_YEAR = 365 * ONE_DAY;

        public static async Task<string> ReadFile(string filePath)
        {
            //this verse is loaded for the first time so fill it from the text file

            //            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(
            //                new Uri("ms-appx:///Data/" + filePath));

            StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = await installedLocation.GetFileAsync("MyDraftAPI_v2\\Resources\\" + filePath);

            StringBuilder text = new StringBuilder();
            var lines = await FileIO.ReadLinesAsync(file);
            foreach (string line in lines)
            {
                text.Append(line);
            }

            return text.ToString();

            //string text = Windows.Storage.FileIO.ReadTextAsync(file);
            //var ResourceStream = Application.GetResourceStream(new Uri(filePath, UriKind.Relative));
            //if (ResourceStream != null)
            //{
            //    Stream myFileStream = ResourceStream.Stream;
            //    if (myFileStream.CanRead)
            //    {
            //        StreamReader myStreamReader = new StreamReader(myFileStream);

            //        //read the content here
            //        return myStreamReader.ReadToEnd();
            //    }
            //}
            //return "NULL";
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        public static object GetPropValue(this object obj, string name)
        {
            foreach (string part in name.Split('.'))
            {
                if (obj == null) { return null; }

                TypeInfo type = obj.GetType().GetTypeInfo();
                PropertyInfo info = type.GetDeclaredProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        public static T GetPropValue<T>(this object obj, string name)
        {
            object retval = obj.GetPropValue(name);
            if (retval == null) { return default; }

            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }

        public static IList<string> listFromPropertyListArrayItems(IEnumerable<IPropertyListItem> positionItems)
        {
            IList<string> positions = new List<string>();
            foreach (IPropertyListItem positionItem in positionItems)
                positions.Add(positionItem.StringValue);

            return positions;
        }

        public static int getScheduleWeek()
        {
            return getScheduleWeek(DateTime.Now);
        }

        public static int getScheduleWeek(DateTime date)
        {
            int week = 17;
            //DateTime firstSunday = (DateTime)AppSettings.getDateTimeForSettingsKey("season_first_sunday");
            //DateTime today = date;
            //if (today.CompareTo(firstSunday) < 0)
            //{
            //    week = 1;
            //}
            //else
            //{
            //    double firstSundayTimestamp = TNUtility.DateTimeToUnixTimestamp(firstSunday);
            //    double weekStartTimestamp = firstSundayTimestamp - ONE_DAY * 5;
            //    DateTime weekStart = TNUtility.UnixTimeStampToDateTime(weekStartTimestamp);
            //    for (int i = 1; i <= 17; i++)
            //    {
            //        DateTime nextWeek = TNUtility.UnixTimeStampToDateTime(weekStartTimestamp + ONE_WEEK * i);
            //        if (today.CompareTo(nextWeek) < 0)
            //        {
            //            week = i;
            //            break;
            //        }
            //    }
            //}

            return week;
        }

        /*
    public static int getAge(DateTime birthdate)
    {
    	Calendar now = Calendar.getInstance();
    	Calendar dob = Calendar.getInstance();
    	dob.setTime(birthdate);
    	if (dob.after(now)) {
    	    //throw new IllegalArgumentException("Can't be born in the future");
    	    return 0;
    	}
    	int year1 = now.get(Calendar.YEAR);
    	int year2 = dob.get(Calendar.YEAR);
    	int age = year1 - year2;
    	int month1 = now.get(Calendar.MONTH);
    	int month2 = dob.get(Calendar.MONTH);
    	if (month2 > month1) {
    	  age--;
    	} else if (month1 == month2) {
    	  int day1 = now.get(Calendar.DAY_OF_MONTH);
    	  int day2 = dob.get(Calendar.DAY_OF_MONTH);
    	  if (day2 > day1) {
    	    age--;
    	  }
    	}
    	
    	return age;
    }

    public static IDictionary<String, Object> getTeamViewForTeam(Team team, IList<IDictionary<String, Object>> positionSections, Boolean isDepthchart)
    {
        IList<String> playerIDs = team.getPlayerIDs();
        IList<Player> players = DBAdapter.getPlayersWithIDs(playerIDs);
        IDictionary<String, Object> teamViewDict = new Dictionary<String, Object>();
        
        if (team == null || positionSections == null)
            return teamViewDict;
        
        foreach (IDictionary<String, Object> eachView in positionSections) {
            // create empty array entries in the view Dictionary for each position
            teamViewDict.Add((String)eachView.get("title"), new List<Player>());
        }
        
        FantasyLeague league = AppSettings.getActiveFantasyLeague();
        foreach (Player eachPlayer in players) 
        {
            foreach (IDictionary<String, Object> eachView in positionSections) 
            {
                IList<String> positions = (IList<String>)eachView.get("positions");
                
                // Skip IDP if not enabled
                if (!league.isIncludeIDP() && !team.getRosterDisplayKey().equals("depthchart") && AppSettings.isIDPPositionGroup(positions))
                    continue;
                
                foreach (String eachPosition in positions) 
                {
                    Boolean hasPosition = false;
                    if (isDepthchart)
                        hasPosition = eachPlayer.hasPosition(eachPosition);
                    else
                        hasPosition = eachPlayer.hasPrimaryPosition(eachPosition);
                    
                    if (hasPosition) 
                    {
                        IList<Player> players2 = (IList<Player>)teamViewDict.get(eachView.get("title"));
                        players2.Add(eachPlayer);
                        
                        if (team.getRosterDisplayKey().Equals("my_team"))
                            Collections.sort(players2, new Comparator<Player>(){
                                public int compare(Player obj1, Player obj2) {
                                    float points1 = LeagueManager.getPointsForPlayerID(String.valueOf(obj1.getIdentifier()), AppSettings.getProjectionStatsYear(), AppSettings.getProjectionStatsSegment(), league.getIdentifier());
                                    float points2 = LeagueManager.getPointsForPlayerID(String.valueOf(obj2.getIdentifier()), AppSettings.getProjectionStatsYear(), AppSettings.getProjectionStatsSegment(), league.getIdentifier());
                                  Number result = points1 - points2;
                                  return result.intValue();
                                }
                              });
                        else
                            Collections.sort(players2, new Comparator<Player>(){
                                public int compare(Player obj1, Player obj2) {
                                    int rank1 = obj1.getRankForPosition(eachPosition);
                                    int rank2 = obj2.getRankForPosition(eachPosition);
                                  return rank1 - rank2;
                                }
                              });
                        
                        break;
                    }
                }
            }
        }
        
        return teamViewDict;
    }
    
    public static Spanned getFormattedPositionStatusText(Map<String, Object> teamViewDict, ArrayList<Map<String, Object>>positionSections)
    {
        StringBuffer status = new StringBuffer();
        int total = 0;
        for (Map<String, Object> posDict : positionSections)
        {
            ArrayList<Player> players = (ArrayList<Player>)teamViewDict.get(posDict.get("title"));
            int count = players.size();
            total += count;
            status.append(String.format("&nbsp;%s <b>%d</b> ", posDict.get("abbr"), count));
        }
        
        status.append(String.format("&nbsp;TOT <b>%d</b>", total));
        
        return Html.fromHtml(status.toString());
    }
         * */

        public static string getKeyFromArrayOfStrings(IList<string> array)
        {
            IList<string> orderedList = array.OrderBy(x => x).ToList();
            string valueKey = arrayToString(orderedList, "-", false);

            return valueKey;
        }

        public static string arrayToString(IList<string> array, string separator, bool useQuotes)
        {
            StringBuilder result = new StringBuilder();

            string quote = null;
            if (useQuotes)
                quote = "'";
            else
                quote = "";

            if (array.Count() > 0)
            {
                result.Append(quote + array[0] + quote);
                for (int i = 1; i < array.Count(); i++)
                {
                    result.Append(separator);
                    result.Append(quote + array[i] + quote);
                }
            }
            return result.ToString();
        }

        public static string md5(string input)
        {
            var md5Hash = MD5Core.GetHashString(input);
            return md5Hash.ToLower();

            //var myStringBytes = input.GetBytes(); // this will get the UTF8 bytes for the string
            //var md5Hash2 = myStringBytes.ComputeMD5Hash().ToBase64String();
            //return md5Hash2;
        }

        /*
    public static Map<String, Object> loadPlistFromResource(int resourceID)
    {
        Map<String, Object> plistData = null;
        String dataStr;
        try
        {
            dataStr = TNUtility.readFileAsString(AppSettings.getContext(), resourceID);
            plistData = Plist.fromXml(dataStr);
        }
        catch (IOException e)
        {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }
        catch (XmlParseException e)
        {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }
        
        return plistData;
    }
    
    public static int getLogoResourceIDForTeam(Context context, ProTeam team)
    {
        int resID = context.getResources()
                .getIdentifier(team.getAbbr().toLowerCase() + "_icon", "drawable", context.getPackageName());
        if (resID == 0)
            resID = R.drawable.nfl_icon;
        
        return resID;
    }
    
 // Structure:  http://static.nfl.com/static/content/public/image/getty/headshot/R/O/D/ROD339293.jpg
    public static String getPlayerImageURLForPlayerID(String playerID)
    {
        String eliasID = DBAdapter.getPlayerIDFromPlayerID(playerID, DBAdapter.PlayerIDType.PlayerIDTypeStats, DBAdapter.PlayerIDType.PlayerIDTypeElias);

        String url = null;
        if (eliasID != null && eliasID.length() > 3)
        {
            String baseURL = "http://static.nfl.com/static/content/public/image/getty/headshot";
            String fileExt = ".jpg";
            url = String.Format("%s/%c/%c/%c/%s%s", 
                   baseURL, 
                   eliasID.charAt(0),
                   eliasID.charAt(1),
                   eliasID.charAt(2),
                   eliasID,
                   fileExt);
        }

        Debug.WriteLine("PHOTO_URL for playerID: " + playerID + " - " + url);
        return url;
    }

    public static String md5(String s)
    {
        try
        {
            // Create MD5 Hash
            MessageDigest digest = java.security.MessageDigest.getInstance("MD5");
            digest.update(s.getBytes());
            byte messageDigest[] = digest.digest();

            // Create Hex String
            StringBuffer hexString = new StringBuffer();
            for (int i = 0; i < messageDigest.length; i++)
                //hexString.append(Integer.toHexString(0xFF & messageDigest[i]));
                hexString.append(String.format("%02x", messageDigest[i]));
            return hexString.toString();

        }
        catch (NoSuchAlgorithmException e)
        {
            e.printStackTrace();
        }
        return "";
    }
         * */
    }
}
