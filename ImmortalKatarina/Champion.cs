using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace ImmortalSerials
{
    public class Champion
    {
        public Spell Q, W, E, R;
        public Orbwalking.Orbwalker Orbwalker;
        public Menu MainMenu;
        public Obj_AI_Hero Player = ObjectManager.Player;
        public Champion()
        {
            LoadMenu();
        }
        public void LoadMenu()
        {
            MainMenu = new Menu("Immortal " + Player.ChampionName, "ImmortalChampions", true);
            var targetSelector = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelector);
            MainMenu.AddSubMenu(targetSelector);
            Orbwalker = new Orbwalking.Orbwalker(MainMenu.AddSubMenu(new Menu("Orbwalking", "Orbwalking")));
        }
    }
}
