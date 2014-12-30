using System;
using System.Diagnostics;
using System.Reflection;
using LeagueSharp;
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
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            bool hasUpdate = Helper.HasNewVersion(Assembly.GetExecutingAssembly().GetName().Name);
            Game.PrintChat("-------------------------------------------------------------------------------------------"); 
            if (hasUpdate)
            {
                Game.PrintChat("<font color = \"#ff002b\">A new version of KaiHelper is available. Please check for updates!</font>");
            }
            Game.PrintChat("<font color = \"#00FF2B\">KaiHelper</font> by <font color = \"#FD00FF\">kaigan</font>");
            Game.PrintChat(
                "<font color = \"#0092FF\">Feel free to donate via Paypal to:</font> <font color = \"#F0FF00\">ntanphat2406@gmail.com</font>");
            Game.PrintChat("KaiHelper - Loaded!");
            Game.PrintChat("-------------------------------------------------------------------------------------------"); 
        }
    }
}