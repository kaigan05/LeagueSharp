using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using LeagueSharp.Common;

namespace KaiHelper
{
    class ResourceImages
    {
        private const string ImagePath = "http://ddragon.leagueoflegends.com/cdn/5.4.1/img/";
        private const string ChampionImageDownloadPath = ImagePath + "champion/";
        private static readonly string ChampionImageSavePath = String.Format("{0}\\ChampionImages\\", Config.AppDataDirectory);

        static ResourceImages()
        {
            if (!Directory.Exists(ChampionImageSavePath))
            {
                Directory.CreateDirectory(ChampionImageSavePath);
            }
        }
        public static Bitmap GetChampionSquare(string championName)
        {
            if (File.Exists(string.Format("{0}{1}.png", ChampionImageSavePath, championName)))
            {
                return new Bitmap(string.Format("{0}{1}.png", ChampionImageSavePath, championName));
            }
            var result = Helper.CropCircleImage(DownloadChampionSquare(championName) ?? DownloadChampionSquare("Aatrox"));
            result.Save(string.Format("{0}{1}.png", ChampionImageSavePath, championName), ImageFormat.Png);
            return result;
        }
        private static Bitmap DownloadChampionSquare(string championName)
        {
            Console.WriteLine("Downloading: "+championName);
            Bitmap result = new Bitmap(Helper.Download(string.Format("{0}{1}.png", ChampionImageDownloadPath, championName)));
            return result;
        }
    }
}
