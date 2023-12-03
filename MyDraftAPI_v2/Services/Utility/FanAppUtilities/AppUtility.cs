using System;
using System.Collections.Generic;
using System.Text;

//---------------------------------//

namespace MyDraftAPI_v2.Services.Utility.FanAppUtilities
{
    public class AppUtility
    {
        //Tom Brady: http://s.nflcdn.com/static/content/public/image/fantasy/transparent/200x200/BRA371156.png
        //Drew Brees: http://s.nflcdn.com/static/content/public/image/fantasy/transparent/200x200/BRE229498.png
        //Joe Flacco: http://static.fantasydata.com/headshots/nfl/low-res/611.png
        public static string transparentPlayerImageURLForPlayerID(string playerID)
        {
            string eliasID = playerID;
            string url = null;
            if (eliasID.Length > 3)
            {
                string baseURL = "http://static.fantasydata.com/headshots/" + "nfl/low-res";
                string fileExt = ".png";
                url = string.Format("{0}/{1}{2}",
                       baseURL,
                       eliasID,
                       fileExt);
            }

            return url;
        }

        //public static void setupImage(Image imgTeamLogo, String imageType, String teamAbbr, int width, int height)
        //{
        //    BitmapImage logoImage = new BitmapImage();
        //    imgTeamLogo.Height = logoImage.DecodePixelHeight = width;
        //    imgTeamLogo.Width = logoImage.DecodePixelWidth = height; //natural px width of image source
        //    // don't need to set Height, system maintains aspect ratio, and calculates the other
        //    // dimension, so long as one dimension measurement is provided   
        //    //logoImage.UriSource = new Uri(imgTeamLogo.BaseUri, "ms-appx:///Images/NFL/" + _NewsItem.team + "/" + _NewsItem.team + "_icon_40x40@2x.png");
        //    if (teamAbbr == null || teamAbbr == "") {
        //        logoImage.UriSource = new Uri(imgTeamLogo.BaseUri, "ms-appx:///Images/NFL/nfl/nfl_icon_40x40@2x.png");
        //    } else {
        //        logoImage.UriSource = new Uri(imgTeamLogo.BaseUri, "ms-appx:///Images/NFL/nfl_" + teamAbbr.ToLower() + "_" + imageType + ".png");
        //    }
        //    imgTeamLogo.Stretch = Stretch.Uniform;
        //    imgTeamLogo.Source = logoImage;
        //}
    }
}
