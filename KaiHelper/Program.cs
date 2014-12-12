using System;
using LeagueSharp.Common;

namespace KaiHelper
{
    internal class Program
    {
        public static Menu Config;

        private static void Main(string[] args)
        {
            try
            {
                Config = new Menu("KaiHelper", "KaiHelp", true);
                SkillBar.AttachMenu(Config);
                GankDetector.AttachMenu(Config);
                WardDetector.AttachMenu(Config);
                HealthTurret.AttachMenu(Config);
                JungleTimer.AttachMenu(Config);
                Config.AddToMainMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}