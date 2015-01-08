using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace ImmortalSerials
{
    class Katarina:Champion
    {
        private readonly List<Spell> spellList=new List<Spell>(); 
        public Katarina()
        {
            spellList.Add(Q = new Spell(SpellSlot.Q, 675));
            spellList.Add(W = new Spell(SpellSlot.W, 375));
            spellList.Add(E = new Spell(SpellSlot.E, 700));
            spellList.Add(R = new Spell(SpellSlot.R, 550));
            KatarinaMenu();
            Game.PrintChat(string.Format("{0} {1} Loaded!", Game.ClockTime/60, Player.ChampionName));
            Game.OnGameUpdate += new GameUpdate(Game_OnGameUpdate);
            Drawing.OnDraw += Drawing_OnDraw;
        }

        void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead) return;
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    Farm();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Farm(true);
                    break;
            }
            if (MainMenu.Item("AutoF").GetValue<bool>())
            {
                Farm();
            }
        }

        private void KillSteal()
        {
            
        }
        private void Combo()
        {
            var unit = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (unit == null)
                return;
            Console.WriteLine(unit.BoundingRadius);
            if (Q.IsReady() && Player.Distance(unit.Position) <= Q.Range + unit.BoundingRadius)
            {
                Q.CastOnUnit(unit,true);
            }
            if (E.IsReady() && Player.Distance(unit.Position) <= E.Range + unit.BoundingRadius)
            {
                E.CastOnUnit(unit, true);
            }
            if (W.IsReady() && Player.Distance(unit.Position) <= W.Range)
            {
                if(W.CastIfWillHit(unit))
                    W.Cast();
                else
                {
                    Game.PrintChat("W can't Hit");
                }
            }
            if (R.IsReady() && Player.Distance(unit.Position) <= R.Range)
            {
                R.Cast();
            }
        }

        public void Farm(bool laneClear=false)
        {
            var minions = MinionManager.GetMinions(Player.ServerPosition, Q.Range,MinionTypes.All,MinionTeam.NotAlly);
            var useQ = MainMenu.Item("UseQF").GetValue<bool>() && Q.IsReady();
            var useW = MainMenu.Item("UseWF").GetValue<bool>() && W.IsReady();
            if (laneClear)
            {

                foreach (var minion in minions)
                {
                    if (useQ)
                    {
                        Q.Cast(minion);
                    }
                    if (useW && Player.Distance(minion)<W.Range)
                    {
                        W.Cast();
                    }
                }
            }
            else
            {
                if (useW)
                {
                    Obj_AI_Base minion = minions.FirstOrDefault(o => Player.Distance(o) <=W.Range);
                    if (minion != null)
                    {
                        var dameQ = Player.GetSpellDamage(minion, SpellSlot.Q);
                        var dameQ1 = Player.GetSpellDamage(minion, SpellSlot.Q, 1);
                        var dameW = Player.GetSpellDamage(minion, SpellSlot.W, 1);
                        if ((int)minion.Armor == 0)
                        {
                            dameQ *= 0.931;
                            dameQ1 *= 0.931;
                            dameW *= 0.931;
                            if (Items.HasItem((int)ItemId.Sorcerers_Shoes))
                            {
                                dameQ = dameQ * 0.906;
                                dameQ1 = dameQ1 * 0.906;
                                dameW = dameW * 0.906;
                            }
                        }
                        InfoDame = string.Format("HP:{0} | {1}={2}", minion.Armor, Math.Round(minion.Health - dameQ), Math.Round(minion.Health - Player.GetSpellDamage(minion, SpellSlot.Q)));
                        if (minion.Buffs.Any(buff => buff.Name == "katarinaqmark") && minion.Health <= dameQ1 + dameW)
                        {
                            W.Cast();
                            Info = "Cast 2 Q + W Successful! ";
                            return;
                        }
                        if (minion.Health <= dameW)
                        {
                            W.Cast();
                            return;
                        }
                        if (useQ)
                        {
                            if (minion.Health <= dameQ + dameQ1 + dameW)
                            {
                                Q.CastOnUnit(minion);
                                target = minion;
                                return;
                            }
                        }
                    }
                }
                if (useQ)
                {
                    foreach (var minion in minions)
                    {
                        var dameQ = Player.GetSpellDamage(minion, SpellSlot.Q);
                        if ((int)minion.Armor == 0)
                        {
                            dameQ *= 0.931;
                            if (Items.HasItem((int)ItemId.Sorcerers_Shoes))
                            {
                                dameQ = dameQ * 0.906;
                            }
                        }
                        if (HealthPrediction.GetHealthPrediction(minion, (int)(Player.Distance(minion) / 1.4)) <= dameQ)
                        {
                            Q.CastOnUnit(minion);
                        }
                    }
                }
            }
        }

        private Obj_AI_Base target;
        private string Info = string.Empty, InfoDame = string.Empty;
        void Drawing_OnDraw(EventArgs args)
        {
            if (target != null && target.IsValid)
            {
                Render.Circle.DrawCircle(target.Position, target.BoundingRadius,System.Drawing.Color.Crimson,5);
                
            }
            var pos = Drawing.WorldToScreen(Player.Position);
            Drawing.DrawText(pos.X, pos.Y, Color.GreenYellow, Info);
            Drawing.DrawText(pos.X, pos.Y - 20, Color.Yellow, InfoDame);
            foreach (var spell in spellList)
            {
                var item = MainMenu.Item("Draw" + spell.Slot).GetValue<Circle>();
                if (item.Active)
                {
                    Render.Circle.DrawCircle(Player.Position,spell.Range,item.Color,5);
                }
            }
        }
        public void KatarinaMenu()
        {
            MainMenu.AddSubMenu(new Menu("Combo", "Combo"));
            MainMenu.SubMenu("Combo").AddItem(new MenuItem("UseEC","Use E").SetValue(true));
            MainMenu.SubMenu("Combo").AddItem(new MenuItem("ModeC", "Mode").SetValue(new StringList(new []{"Q E W","E Q W"})));
            MainMenu.AddSubMenu(new Menu("Harass", "Harass"));
            MainMenu.SubMenu("Harass").AddItem(new MenuItem("UseQH", "Use Q").SetValue(true));
            MainMenu.SubMenu("Harass").AddItem(new MenuItem("UseWH", "Use W").SetValue(true));
            MainMenu.SubMenu("Harass").AddItem(new MenuItem("UseEH", "Use E").SetValue(true));
            MainMenu.AddSubMenu(new Menu("Farm", "Farm"));
            MainMenu.SubMenu("Farm").AddItem(new MenuItem("UseQF", "Use Q").SetValue(true));
            MainMenu.SubMenu("Farm").AddItem(new MenuItem("UseWF", "Use W").SetValue(true));
            MainMenu.SubMenu("Farm").AddItem(new MenuItem("AutoF", "Auto Farm").SetValue(true));
            MainMenu.AddSubMenu(new Menu("LaneClear", "Clear"));
            MainMenu.SubMenu("Clear").AddItem(new MenuItem("UseQH", "Use Q").SetValue(true));
            MainMenu.SubMenu("Clear").AddItem(new MenuItem("UseWH", "Use W").SetValue(true));
            MainMenu.SubMenu("Clear").AddItem(new MenuItem("UseEH", "Use E").SetValue(true));
            MainMenu.AddSubMenu(new Menu("Drawing", "Drawing"));
            MainMenu.SubMenu("Drawing").AddItem(new MenuItem("DrawQ", "Q range").DontSave().SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 22, 255, 0))));
            MainMenu.SubMenu("Drawing").AddItem(new MenuItem("DrawW", "W range").DontSave().SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 194, 13, 0))));
            MainMenu.SubMenu("Drawing").AddItem(new MenuItem("DrawE", "E range").DontSave().SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 39, 0, 255))));
            MainMenu.SubMenu("Drawing").AddItem(new MenuItem("DrawR", "R range").DontSave().SetValue(new Circle(true, System.Drawing.Color.FromArgb(100, 255, 0, 0))));
            MainMenu.AddToMainMenu();
        }
    }
}
