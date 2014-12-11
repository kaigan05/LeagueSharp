using LeagueSharp.Common;

namespace KaiHelper
{
    internal class Program
    {
        public static Menu Config;

        private static void Main(string[] args)
        {
            Config = new Menu("KaiHelper", "KaiHelp", true);
            SkillBar.AttachMenu(Config);
            GankDetector.AttachMenu(Config);
            WardDetector.AttachMenu(Config);
            Config.AddToMainMenu();
        }
    }
}