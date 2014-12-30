using System;
using LeagueSharp.Common;

namespace KaiHelper
{
    internal class Program
    {
        public static Menu Config;

        private static void Main(string[] args)
        {
            Config = new Menu("KaiHelper", "KaiHelp", true);
            new SkillBar(Config);
            new GankDetector(Config);
            new WardDetector(Config);
            new HealthTurret(Config);
            new JungleTimer(Config);
            Config.AddToMainMenu();
        }
    }
}