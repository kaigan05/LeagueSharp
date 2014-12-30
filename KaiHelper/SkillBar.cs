using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Font = SharpDX.Direct3D9.Font;
using Rectangle = SharpDX.Rectangle;

namespace KaiHelper
{
    public class SkillBar
    {
        public Sprite Sprite;
        public Texture HudTexture;
        public Texture FrameLevelTexture;
        public Texture ButtonRedTexture;

        private readonly Dictionary<string, Texture> _summonerSpellTextures =
            new Dictionary<string, Texture>(StringComparer.InvariantCultureIgnoreCase);

        public Font SmallText;
        public SpellSlot[] SummonerSpellSlots = { SpellSlot.Summoner1, SpellSlot.Summoner2 };
        public SpellSlot[] SpellSlots = { SpellSlot.Q, SpellSlot.W, SpellSlot.E, SpellSlot.R };
        public Menu MenuSkillBar;

        public SkillBar(Menu config)
        {
            try
            {
                MenuSkillBar = config.AddSubMenu(new Menu("Skill Bar", "SkillBar"));
                MenuSkillBar.AddItem(new MenuItem("OnAllies", "On Allies").SetValue(false));
                MenuSkillBar.AddItem(new MenuItem("OnEnemies", "On Enemies").SetValue(true));
                Sprite = new Sprite(Drawing.Direct3DDevice);
                HudTexture = Texture.FromMemory(
                    Drawing.Direct3DDevice,
                    (byte[])
                        new ImageConverter().ConvertTo(new Bitmap(LeagueSharpFolder.HudFolder("main")), typeof(byte[])),
                    127, 41, 0, Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
                FrameLevelTexture = Texture.FromMemory(
                    Drawing.Direct3DDevice,
                    (byte[])
                        new ImageConverter().ConvertTo(
                            new Bitmap(LeagueSharpFolder.HudFolder("spell_level")), typeof(byte[])), 2, 3, 0, Usage.None,
                    Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
                ButtonRedTexture = Texture.FromMemory(
                    Drawing.Direct3DDevice,
                    (byte[])
                        new ImageConverter().ConvertTo(
                            new Bitmap(LeagueSharpFolder.HudFolder("button_red")), typeof(byte[])), 14, 14, 0,
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
                Drawing.OnPreReset += DrawingOnPreReset;
                Drawing.OnPostReset += DrawingOnPostReset;
                AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
                AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;
                CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Oc! " + ex.Message);
            }
        }

        private void CurrentDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            SmallText.Dispose();
            Sprite.Dispose();
        }

        private void DrawingOnPostReset(EventArgs args)
        {
            SmallText.OnResetDevice();
            Sprite.OnResetDevice();
        }

        private void DrawingOnPreReset(EventArgs args)
        {
            SmallText.OnLostDevice();
            Sprite.OnLostDevice();
        }

        private void Game_OnGameLoad(EventArgs args)
        {
            string[] filePaths =
                Directory.GetFiles(LeagueSharpFolder.SummonerSpellFolder(), "*.png")
                    .Select(Path.GetFileNameWithoutExtension)
                    .ToArray();
            foreach (var filePath in filePaths.Where(filePath => !_summonerSpellTextures.ContainsKey(filePath)))
            {
                _summonerSpellTextures.Add(filePath, GetTexture(null, SpellSlot.Summoner2, filePath));
            }
            foreach (Obj_AI_Hero hero in ObjectManager.Get<Obj_AI_Hero>())
            {
                foreach (SpellSlot spellSlot in SpellSlots)
                {
                    if (!_summonerSpellTextures.ContainsKey(hero.ChampionName + "_" + spellSlot))
                    {
                        _summonerSpellTextures.Add(
                            hero.ChampionName + "_" + spellSlot, GetTexture(hero.ChampionName, spellSlot));
                    }
                }
            }
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private Texture GetTexture(string heroName, SpellSlot spellSlot, string name = null)
        {
            Bitmap bitmap;
            if (name != null)
            {
                bitmap = new Bitmap(LeagueSharpFolder.SummonerSpellFolder(name));
                return Texture.FromMemory(
                    Drawing.Direct3DDevice, (byte[]) new ImageConverter().ConvertTo(bitmap, typeof(byte[])), 12, 240, 0,
                    Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
            }
            bitmap = new Bitmap(LeagueSharpFolder.SpellFolder(heroName + "_" + spellSlot));
            return Texture.FromMemory(
                Drawing.Direct3DDevice, (byte[]) new ImageConverter().ConvertTo(bitmap, typeof(byte[])), 14, 14, 0,
                Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            try
            {
                if (Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
                {
                    return;
                }
                if (Sprite.IsDisposed)
                {
                    return;
                }
                foreach (
                    Obj_AI_Hero hero in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(
                                hero =>
                                    hero.IsValid && !hero.IsMe && hero.IsHPBarRendered &&
                                    (hero.IsEnemy && MenuSkillBar.Item("OnEnemies").GetValue<bool>() ||
                                     hero.IsAlly && MenuSkillBar.Item("OnAllies").GetValue<bool>())))
                {
                    Vector2 skillStateBarPos;
                    if (hero.IsEnemy)
                    {
                        skillStateBarPos = hero.HPBarPosition + new Vector2(-10, 17);
                    }
                    else
                    {
                        skillStateBarPos = hero.HPBarPosition + new Vector2(-10, 14);
                    }
                    var x = (int) skillStateBarPos.X;
                    var y = (int) skillStateBarPos.Y;
                    Sprite.Begin();
                    Sprite.Draw(HudTexture, new ColorBGRA(255, 255, 255, 255), null, new Vector3(-x, -y, 0));
                    for (int index = 0; index < SummonerSpellSlots.Length; index++)
                    {
                        SpellDataInst summonerSpell = hero.Spellbook.GetSpell(SummonerSpellSlots[index]);
                        float t = summonerSpell.CooldownExpires - Game.Time;
                        float percent = (Math.Abs(summonerSpell.Cooldown) > float.Epsilon)
                            ? t / summonerSpell.Cooldown
                            : 1f;
                        int n = (t > 0) ? (int) (19 * (1f - percent)) : 19;
                        string s = string.Format(t < 1f ? "{0:0.0}" : "{0:0}", t);
                        if (t > 0)
                        {
                            Helper.DrawText(SmallText, s, x - 10, y + 2 + 19 * index, new ColorBGRA(255, 255, 255, 255));
                        }
                        Sprite.Draw(
                            _summonerSpellTextures[summonerSpell.Name], new ColorBGRA(255, 255, 255, 255),
                            new Rectangle(0, 12 * n, 12, 12), new Vector3(-x - 3, -y - 3 - 18 * index, 0));
                    }
                    for (int index = 0; index < SpellSlots.Length; index++)
                    {
                        SpellSlot spellSlot = SpellSlots[index];
                        SpellDataInst spell = hero.Spellbook.GetSpell(spellSlot);
                        for (int i = 1; i <= 5; i++)
                        {
                            if (spell.Level == i)
                            {
                                for (int j = 1; j <= i; j++)
                                {
                                    Sprite.Draw(
                                        FrameLevelTexture, new ColorBGRA(255, 255, 255, 255), new Rectangle(0, 0, 2, 3),
                                        new Vector3(-x - 18 - index * 17 - j * 3, -y - 36, 0));
                                }
                            }
                        }

                        Sprite.Draw(
                            _summonerSpellTextures[hero.ChampionName + "_" + spellSlot],
                            new ColorBGRA(255, 255, 255, 255), new Rectangle(0, 0, 14, 14),
                            new Vector3(-x - 21 - index * 17, -y - 20, 0));
                        if (spell.State == SpellState.Cooldown || spell.State == SpellState.NotLearned)
                        {
                            Sprite.Draw(
                                ButtonRedTexture, new ColorBGRA(0, 0, 0, 180), new Rectangle(0, 0, 14, 14),
                                new Vector3(-x - 21 - index * 17, -y - 20, 0));
                        }
                    }
                    Sprite.End();
                    for (int index = 0; index < SpellSlots.Length; index++)
                    {
                        SpellSlot spellSlot = SpellSlots[index];
                        SpellDataInst spell = hero.Spellbook.GetSpell(spellSlot);
                        float t = spell.CooldownExpires - Game.Time;
                        if (!(t > 0) || !(t < 100))
                        {
                            continue;
                        }
                        string s = string.Format(t < 1f ? "{0:0.0}" : "{0:0}", t);
                        Helper.DrawText(
                            SmallText, s, x + 16 + index * 17 + 12, y + 21, new ColorBGRA(255, 255, 255, 255));
                    }
                }
            }
            catch (Exception ex)
            {
                Sprite.End();
            }
        }
    }
}