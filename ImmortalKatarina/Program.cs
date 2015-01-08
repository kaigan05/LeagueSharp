using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace ImmortalSerials
{
    class Program
    {
        
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            var championName = ObjectManager.Player.ChampionName;
            //Champion champion = new Champion();
            switch (championName)
            {
                case "Katarina":
                    Champion champion = new Katarina();
                    break;
            }
        }
    }
}
