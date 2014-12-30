using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using Version = LeagueSharp.Common.Version;

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

    public class GankDetector
    {
        public Menu MenuGank;
        private readonly Dictionary<Obj_AI_Hero, Time> _enemies = new Dictionary<Obj_AI_Hero, Time>();

        public GankDetector(Menu config)
        {
            MenuGank = config.AddSubMenu(new Menu("Gank Detector", "GDetect"));
            MenuGank.AddItem(new MenuItem("InvisibleTime", "Invisisble Time").SetValue(new Slider(5, 1, 10)));
            MenuGank.AddItem(new MenuItem("VisibleTime", "Visible Time").SetValue(new Slider(3, 1, 5)));
            MenuGank.AddItem(new MenuItem("TriggerRange", "Trigger Range").SetValue(new Slider(3000, 1, 3000)));
            MenuGank.AddItem(new MenuItem("CircalRange", "Circal Range").SetValue(new Slider(2500, 1, 3000)));
            //MenuGank.AddItem(new MenuItem("Ping", "Ping").SetValue(new StringList(new[] {"Local Ping", "Server Ping"})));
            MenuGank.AddItem(new MenuItem("Fill", "Fill").SetValue(false));
            MenuGank.AddItem(new MenuItem("GankActive", "Active").SetValue(true));
            Game.OnGameUpdate += Game_OnGameUpdate;
            CustomEvents.Game.OnGameLoad += (args =>
            {
                foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsEnemy))
                {
                    _enemies.Add(hero, new Time());
                }
            });
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!IsActive())
            {
                return;
            }
            try
            {
                int triggerGank = MenuGank.Item("TriggerRange").GetValue<Slider>().Value;
                int circalGank = MenuGank.Item("CircalRange").GetValue<Slider>().Value;
                int invisibleTime = MenuGank.Item("InvisibleTime").GetValue<Slider>().Value;
                int visibleTime = MenuGank.Item("VisibleTime").GetValue<Slider>().Value;
                foreach (Obj_AI_Hero hero in
                    _enemies.Select(enemy => enemy.Key)
                        .Where(
                            hero =>
                                !hero.IsDead && hero.IsVisible && _enemies[hero].InvisibleTime >= invisibleTime &&
                                _enemies[hero].VisibleTime <= visibleTime &&
                                hero.Distance(ObjectManager.Player.Position) <= triggerGank))
                {
                    Utility.DrawCircle(hero.Position, circalGank, Color.Red, 20);
                    if (MenuGank.Item("Fill").GetValue<bool>())
                    {
                        Utility.DrawCircle(hero.Position, circalGank, Color.FromArgb(15, Color.Red), -142857);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't OnDraw " + ex.Message);
            }
        }

        public bool IsActive()
        {
            return MenuGank.Item("GankActive").GetValue<bool>();
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive())
            {
                return;
            }
            try
            {
                int triggerGank = MenuGank.Item("TriggerRange").GetValue<Slider>().Value;
                int invisibleTime = MenuGank.Item("InvisibleTime").GetValue<Slider>().Value;
                int visibleTime = MenuGank.Item("VisibleTime").GetValue<Slider>().Value;
                foreach (var enemy in _enemies)
                {
                    UpdateTime(enemy);
                    Obj_AI_Hero hero = enemy.Key;
                    if (hero.IsDead || !hero.IsVisible || _enemies[hero].InvisibleTime < invisibleTime ||
                        _enemies[hero].VisibleTime > visibleTime ||
                        !(hero.Distance(ObjectManager.Player.Position) <= triggerGank))
                    {
                        continue;
                    }
                    //var t = MenuGank.Item("Ping").GetValue<StringList>();
                    if (!_enemies[hero].Pinged)
                    {
                        _enemies[hero].Pinged = true;
                        Game.PrintChat("<font color = \"#FF0000\">Gank: </font>" + hero.ChampionName);
                        //switch (t.SelectedIndex)
                        //{
                        //    case 0:
                        //        Packet.S2C.Ping.Encoded(new Packet.S2C.Ping.Struct(hero.Position.X, hero.Position.Y,
                        //            0, 0, Packet.PingType.Danger)).Process();
                        //        break;
                        //    case 1:
                        //        Packet.C2S.Ping.Encoded(
                        //            new Packet.C2S.Ping.Struct(hero.Position.X + new Random(10).Next(-200, 200),
                        //                hero.Position.Y + new Random(10).Next(-200, 200), 0, Packet.PingType.Danger))
                        //            .Send();
                        //        break;
                        //}
                        Utility.DelayAction.Add((visibleTime + 1) * 1000, () => { _enemies[hero].Pinged = false; });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't Update " + ex.Message);
            }
        }

        private void UpdateTime(KeyValuePair<Obj_AI_Hero, Time> enemy)
        {
            Obj_AI_Hero hero = enemy.Key;
            if (!hero.IsValid)
            {
                return;
            }
            if (hero.IsVisible)
            {
                if (!_enemies[hero].CalledVisible)
                {
                    _enemies[hero].CalledVisible = true;
                    _enemies[hero].StartVisibleTime = Environment.TickCount;
                }
                _enemies[hero].CalledInvisible = false;
                _enemies[hero].VisibleTime = (Environment.TickCount - _enemies[hero].StartVisibleTime) / 1000;
            }
            else
            {
                if (!_enemies[hero].CalledInvisible)
                {
                    _enemies[hero].CalledInvisible = true;
                    _enemies[hero].StartInvisibleTime = Environment.TickCount;
                }
                _enemies[hero].CalledVisible = false;
                _enemies[hero].InvisibleTime = (Environment.TickCount - _enemies[hero].StartInvisibleTime) / 1000;
            }
        }
    }
}