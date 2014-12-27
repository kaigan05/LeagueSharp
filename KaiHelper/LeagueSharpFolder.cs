using System;
using System.IO;
using System.Net;

namespace KaiHelper
{
    public static class LeagueSharpFolder
    {
        public static string MainFolder
        {
            get
            {
                return
                    string.Format(
                        !Directory.Exists(
                            string.Format("{0}\\LeagueSharp\\Repositories\\4A862CBB\\trunk\\KaiHelper\\Images",
                                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)))
                            ? "{0}\\LeagueSharp\\Repositories\\4A862CBB\\trunk\\KaiHelper\\Images"
                            : "{0}\\LeagueSharp\\Repositories\\DC0C31BB\\trunk\\KaiHelper\\Images",
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            }
        }

        public static string SummonerSpellFolder(string fileName=null)
        {
            if (fileName == null)
            {
                return string.Format(@"{0}\AlternateSS\", MainFolder);
            }
            return string.Format(@"{0}\AlternateSS\{1}.png", MainFolder, fileName);
        }

        public static string SpellFolder(string fileName)
        {
            return string.Format(@"{0}\SkillsSmall\{1}.png", MainFolder, fileName);
        }

        public static string MiniMapFolder(string fileName)
        {
            return string.Format(@"{0}\Minimap\{1}.png", MainFolder, fileName);
        }

        public static string HudFolder(string fileName)
        {
            return string.Format(@"{0}\HUD\{1}.png", MainFolder, fileName);
        }

        public static Stream Download(string url)
        {
            WebRequest req = WebRequest.Create(url);
            WebResponse response = req.GetResponse();
            return response.GetResponseStream();
        }
    }
}