using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaiHelper
{
    class ResourceImages
    {
        private const string ImagePath = "http://ddragon.leagueoflegends.com/cdn/5.4.1/img/";
        private const string ChampionImagePath = ImagePath + "champion/";

        public static Bitmap GetChampionSquare(string championName)
        {
            Bitmap result = new Bitmap(Helper.Download(string.Format("{0}{1}.png", ChampionImagePath, championName)));
            return result;
        }
    }
}
