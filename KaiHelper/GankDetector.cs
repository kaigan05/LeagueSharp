using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace KaiHelper
{
    public class Time
    {
        public bool CalledInvisible = false;
        public bool CalledVisible = false;
        public int InvisibleTime;
        public bool Pinged = false;
        public int StartInvisibleTime;
        public int StartVisibleTime;
        public int VisibleTime;
    }

    public static class GankDetector
    {
        public static Menu MenuGank;
        private static readonly Dictionary<Obj_AI_Hero, Time> Enemies = new Dictionary<Obj_AI_Hero, Time>();

        static GankDetector()
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
            CustomEvents.Game.OnGameLoad += (args =>
            {
                foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy))
                {
                    Enemies.Add(hero, new Time());
                }
                Game.PrintChat("<font color = \"#FD00FF\">KaiHelper</font> by <font color = \"#00FF2B\">kaigan</font>");
                Game.PrintChat(
                    "<font color = \"#0092FF\">Feel free to donate via Paypal to:</font> <font color = \"#F0FF00\">ntanphat2406@gmail.com</font>");
                Game.PrintChat("KaiHelper - Loaded");
            });
            Drawing.OnDraw += Drawing_OnDraw;
        }

        public static void AttachMenu(Menu menu)
        {
            MenuGank = menu.AddSubMenu(new Menu("GankDetector", "GDetect"));
            MenuGank.AddItem(new MenuItem("InvisibleTime", "Invisisble Time").SetValue(new Slider(5, 1, 10)));
            MenuGank.AddItem(new MenuItem("VisibleTime", "Visible Time").SetValue(new Slider(3, 1, 5)));
            MenuGank.AddItem(new MenuItem("TriggerRange", "Trigger Range").SetValue(new Slider(3000, 1, 3000)));
            MenuGank.AddItem(new MenuItem("CircalRange", "Circal Range").SetValue(new Slider(2500, 1, 3000)));
            MenuGank.AddItem(new MenuItem("Ping", "Ping").SetValue(new StringList(new[] {"Local Ping", "Server Ping"})));
            MenuGank.AddItem(new MenuItem("LagFree", "Lagg Free").SetValue(false));
            MenuGank.AddItem(new MenuItem("Active", "Active").SetValue(true));
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!IsActive()) return;
            int triggerGank = MenuGank.Item("TriggerRange").GetValue<Slider>().Value;
            int circalGank = MenuGank.Item("CircalRange").GetValue<Slider>().Value;
            int invisibleTime = MenuGank.Item("InvisibleTime").GetValue<Slider>().Value;
            int visibleTime = MenuGank.Item("VisibleTime").GetValue<Slider>().Value;
            foreach (
                Obj_AI_Hero hero in
                    Enemies.Select(enemy => enemy.Key)
                        .Where(
                            hero =>
                                !hero.IsDead && hero.IsVisible && Enemies[hero].InvisibleTime >= invisibleTime &&
                                Enemies[hero].VisibleTime <= visibleTime &&
                                hero.Distance(ObjectManager.Player.Position) <= triggerGank))
            {
                Utility.DrawCircle(hero.Position, circalGank, Color.Red, 20);
                if(!MenuGank.Item("LagFree").GetValue<bool>())
                    Utility.DrawCircle(hero.Position, circalGank, Color.FromArgb(25, Color.Red), -142857);
            }
        }

        public static bool IsActive()
        {
            return MenuGank.Item("Active").GetValue<bool>();
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive())
                return;
            int triggerGank = MenuGank.Item("TriggerRange").GetValue<Slider>().Value;
            int invisibleTime = MenuGank.Item("InvisibleTime").GetValue<Slider>().Value;
            int visibleTime = MenuGank.Item("VisibleTime").GetValue<Slider>().Value;
            foreach (var enemy in Enemies)
            {
                UpdateTime(enemy);
                Obj_AI_Hero hero = enemy.Key;
                if (hero.IsDead || !hero.IsVisible || Enemies[hero].InvisibleTime < invisibleTime ||
                    Enemies[hero].VisibleTime > visibleTime ||
                    !(hero.Distance(ObjectManager.Player.Position) <= triggerGank)) continue;
                var t = MenuGank.Item("Ping").GetValue<StringList>();
                if (!Enemies[hero].Pinged)
                {
                    Enemies[hero].Pinged = true;
                    switch (t.SelectedIndex)
                    {
                        case 0:
                            Packet.S2C.Ping.Encoded(new Packet.S2C.Ping.Struct(hero.Position.X, hero.Position.Y,
                                0, 0, Packet.PingType.Danger)).Process();
                            break;
                        case 1:
                            Packet.C2S.Ping.Encoded(
                                new Packet.C2S.Ping.Struct(hero.Position.X + new Random(10).Next(-200, 200),
                                    hero.Position.Y + new Random(10).Next(-200, 200), 0, Packet.PingType.Danger))
                                .Send();
                            break;
                    }
                    Game.PrintChat("<font color = \"#FF0000\">Gank: </font>{0}", hero.ChampionName);
                    Utility.DelayAction.Add(visibleTime*1000, () => { Enemies[hero].Pinged = false; });
                }
            }
        }

        private static void UpdateTime(KeyValuePair<Obj_AI_Hero, Time> enemy)
        {
            Obj_AI_Hero hero = enemy.Key;
            if (!hero.IsValid)
                return;
            if (hero.IsVisible)
            {
                if (!Enemies[hero].CalledVisible)
                {
                    Enemies[hero].CalledVisible = true;
                    Enemies[hero].StartVisibleTime = Environment.TickCount;
                }
                Enemies[hero].CalledInvisible = false;
                Enemies[hero].VisibleTime = (Environment.TickCount - Enemies[hero].StartVisibleTime)/1000;
            }
            else
            {
                if (!Enemies[hero].CalledInvisible)
                {
                    Enemies[hero].CalledInvisible = true;
                    Enemies[hero].StartInvisibleTime = Environment.TickCount;
                }
                Enemies[hero].CalledVisible = false;
                Enemies[hero].InvisibleTime = (Environment.TickCount - Enemies[hero].StartInvisibleTime)/1000;
            }
        }
    }
}