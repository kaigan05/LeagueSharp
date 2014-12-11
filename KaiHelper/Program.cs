#region

using System;
using LeagueSharp;
using LeagueSharp.Common;

#endregion

namespace KaiHelper
{
    
    internal class Program
    {
        public static Menu Config;

        static void Main(string[] args)
        {
            Config = new Menu("KaiHelper", "KaiHelp", true);
            WardDetector.AttachMenu(Config);
            GankDetector.AttachMenu(Config);
            SkillBar.AttachMenu(Config);
            Config.AddToMainMenu();
        }
    }

}
