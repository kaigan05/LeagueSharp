using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Font = SharpDX.Direct3D9.Font;

namespace KaiHelper
{
    public static class SkillBar
    {
        public static Sprite Sprite;
        public static Texture HudTexture;
        public static Texture FrameLevelTexture;
        public static Texture ButtonRedTexture;
        private static readonly Dictionary<string, Texture> SummonerSpellTextures = new Dictionary<string, Texture>(StringComparer.InvariantCultureIgnoreCase);

        public static Font SmallText;
        public static SpellSlot[] SummonerSpellSlots = { SpellSlot.Summoner1, SpellSlot.Summoner2 };
        public static SpellSlot[] SpellSlots = { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };

        public static Menu MenuSkillBar;

        static SkillBar()
        {
            Sprite = new Sprite(Drawing.Direct3DDevice);
                HudTexture = Texture.FromMemory(
                    Drawing.Direct3DDevice,
                    (byte[])new ImageConverter().ConvertTo(new Bitmap(LeagueSharpFolder.HudFolder("main")), typeof(byte[])), 127, 41, 0,
                    Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
                FrameLevelTexture = Texture.FromMemory(
                    Drawing.Direct3DDevice,
                    (byte[])new ImageConverter().ConvertTo(new Bitmap(LeagueSharpFolder.HudFolder("spell_level")), typeof(byte[])), 2, 3, 0,
                    Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
                ButtonRedTexture = Texture.FromMemory(
                    Drawing.Direct3DDevice,
                    (byte[])new ImageConverter().ConvertTo(new Bitmap(LeagueSharpFolder.HudFolder("button_red")), typeof(byte[])), 14,14, 0,
                    Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
                SmallText = new Font(
                    Drawing.Direct3DDevice,
                    new FontDescription
                    {
                        FaceName = "Calibri",
                        Height = 13,
                        OutputPrecision = FontPrecision.Default,
                        Quality = FontQuality.Default,
                    });
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            foreach (var hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                foreach (var summonerSpellSlot in SummonerSpellSlots)
                {
                    var spell = hero.SummonerSpellbook.GetSpell(summonerSpellSlot);
                    if (!SummonerSpellTextures.ContainsKey(spell.Name))
                    {
                        SummonerSpellTextures.Add(spell.Name, GetTexture(hero.ChampionName, summonerSpellSlot, spell.Name));
                    }
                }
                foreach (var spellSlot in SpellSlots)
                {
                    var spell = hero.Spellbook.GetSpell(spellSlot);
                    if (!SummonerSpellTextures.ContainsKey(hero.ChampionName + "_" + spellSlot))
                    {
                        SummonerSpellTextures.Add(hero.ChampionName + "_" + spellSlot, GetTexture(hero.ChampionName, spellSlot, spell.Name));
                    }
                }
            }
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnPreReset += DrawingOnOnPreReset;
            Drawing.OnPostReset += DrawingOnOnPostReset;
        }

        public static void AttachMenu(Menu menu)
        {
            MenuSkillBar = menu.AddSubMenu(new Menu("Skill Bar", "SkillBar"));
            MenuSkillBar.AddItem(new MenuItem("OnAllies", "Active On Allies").SetValue(false));
            MenuSkillBar.AddItem(new MenuItem("OnEnemies", "Active On Enemies").SetValue(true));
        }

        private static Texture GetTexture(string heroName, SpellSlot spellSlot, string name)
        {
            Bitmap bitmap;
            if (name.Contains("summoner"))
            {
                bitmap = new Bitmap(LeagueSharpFolder.SummonerSpellFolder(name));
                return Texture.FromMemory(
                Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(bitmap, typeof(byte[])), 12, 240, 0,
                Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            }
            bitmap = new Bitmap(LeagueSharpFolder.SpellFolder(heroName + "_" + spellSlot));
            return Texture.FromMemory(
                Drawing.Direct3DDevice, (byte[])new ImageConverter().ConvertTo(bitmap, typeof(byte[])), 14, 14, 0,
                Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
        }

        private static void CurrentDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            SmallText.Dispose();
            Sprite.Dispose();
        }

        private static void DrawingOnOnPostReset(EventArgs args)
        {
            SmallText.OnResetDevice();
            Sprite.OnResetDevice();
        }

        private static void DrawingOnOnPreReset(EventArgs args)
        {
            SmallText.OnLostDevice();
            Sprite.OnLostDevice();
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
                foreach (var hero in ObjectManager.Get<Obj_AI_Hero>() .Where(hero =>hero.IsValid && !hero.IsMe &&
                                hero.IsHPBarRendered &&
                                (hero.IsEnemy && MenuSkillBar.Item("OnEnemies").GetValue<bool>() ||
                                 hero.IsAlly && MenuSkillBar.Item("OnAllies").GetValue<bool>())))
                {
                    Vector2 skillStateBarPos;
                    if (hero.IsEnemy)
                    {
                        skillStateBarPos = hero.HPBarPosition + new Vector2(-9, 17);
                    }
                    else
                    {
                        skillStateBarPos = hero.HPBarPosition + new Vector2(-10, 14);
                    }
                    var x = (int) skillStateBarPos.X;
                    var y = (int) skillStateBarPos.Y;
                    Sprite.Begin();
                    Sprite.Draw(HudTexture, new ColorBGRA(255, 255, 255, 255), null, new Vector3(-x, -y, 0));
                    for (var index = 0; index < SummonerSpellSlots.Length; index++)
                    {
                        var summonerSpell = hero.SummonerSpellbook.GetSpell(SummonerSpellSlots[index]);
                        var t = summonerSpell.CooldownExpires - Game.Time;
                        var percent = (Math.Abs(summonerSpell.Cooldown) > float.Epsilon) ? t / summonerSpell.Cooldown : 1f;
                        var n = (t > 0) ? (int) (19*(1f - percent)) : 19;
                        var s = string.Format(t < 1f ? "{0:0.0}" : "{0:0}", t);
                        if (t > 0)
                        {
                            SmallText.DrawText(null, s, x - 5 - s.Length * 5, y + 2 + 19 * index, new ColorBGRA(255, 255, 255, 255));
                        }
                        Sprite.Draw(SummonerSpellTextures[summonerSpell.Name], new ColorBGRA(255, 255, 255, 255),
                            new SharpDX.Rectangle(0, 12 * n, 12, 12), new Vector3(-x - 3, -y - 3 - 18 * index, 0));
                        
                    }
                    for (var index = 0; index < SpellSlots.Length; index++)
                    {
                        var spellSlot = SpellSlots[index];
                        var spell = hero.Spellbook.GetSpell(spellSlot);
                        for (var i = 1; i <= 5; i++)
                        {
                            if (spell.Level == i)
                            {
                                for (int j = 1; j <= i; j++)
                                {
                                    Sprite.Draw(FrameLevelTexture, new ColorBGRA(255, 255, 255, 255), new SharpDX.Rectangle(0, 0, 2, 3), new Vector3(-x - 18 - index * 17 - j * 3, -y - 36, 0));
                                }
                            }
                        }
                        Sprite.Draw(
                            SummonerSpellTextures[hero.ChampionName + "_" + spellSlot],
                            new ColorBGRA(255, 255, 255, 255), new SharpDX.Rectangle(0, 0, 14, 14),
                            new Vector3(-x - 21 - index * 17, -y - 20, 0));
                        if (spell.State == SpellState.Cooldown || spell.State == SpellState.NotLearned)
                        {
                            Sprite.Draw(ButtonRedTexture,
                            new ColorBGRA(0, 0, 0, 180), new SharpDX.Rectangle(0, 0, 14, 14),
                            new Vector3(-x - 21 - index * 17, -y - 20, 0));
                        }

                    }
                    Sprite.End();
                    for (var index = 0; index < SpellSlots.Length; index++)
                    {
                        var spellSlot = SpellSlots[index];
                        var spell = hero.Spellbook.GetSpell(spellSlot);
                        var t = spell.CooldownExpires - Game.Time;
                        if (!(t > 0) || !(t < 100)) continue;
                        var s = string.Format(t < 1f ? "{0:0.0}" : "{0:0}", t);
                        SmallText.DrawText(
                            null, s, x+16+index*17 + (23 - s.Length*4)/2, y+21, new ColorBGRA(255, 255, 255, 255));
                    }
                }
            }
        }
    }
