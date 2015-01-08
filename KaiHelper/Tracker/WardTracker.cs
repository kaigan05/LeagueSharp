using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace KaiHelper.Tracker
{
    internal class Ward
    {
        private const float Scale = 0.7f;
        private readonly WardDetector _wardDetector;
        private Render.Circle _circle;
        private Render.Sprite _minimapSprite;
        private Render.Text _timerText;

        public Ward(string skinName, int startTime, Obj_AI_Base objAiBase, WardDetector wardDetector)
        {
            _wardDetector = wardDetector;
            int duration;
            WardType type;
            GetWarInfo(skinName, out duration, out type);
            SkinName = skinName;
            StartTime = startTime;
            Duration = duration;
            EndTime = StartTime + Duration;
            Type = type;
            ObjAiBase = objAiBase;
            switch (Type)
            {
                case WardType.Green:
                    Color = Color.Lime;
                    break;
                case WardType.Pink:
                    Color = Color.Magenta;
                    break;
                default:
                    Color = Color.Red;
                    break;
            }
            DrawCircle();
        }

        public Bitmap Bitmap
        {
            get
            {
                switch (Type)
                {
                    case WardType.Green:
                        return _wardDetector.Ward;
                    case WardType.Pink:
                        return _wardDetector.Pink;
                    default:
                        return _wardDetector.Ward;
                }
            }
        }

        private Vector2 MinimapPosition
        {
            get
            {
                return Drawing.WorldToMinimap(ObjAiBase.Position) +
                       new Vector2(-Bitmap.Width / 2 * Scale, -Bitmap.Height / 2 * Scale);
            }
        }

        public Color Color { get; set; }
        public int StartTime { get; set; }
        public int Duration { get; set; }
        public int EndTime { get; set; }
        public string SkinName { get; set; }
        public WardType Type { get; set; }
        public Obj_AI_Base ObjAiBase { get; set; }

        public static bool IsWard(string skinName)
        {
            int duration;
            WardType type;
            return GetWarInfo(skinName, out duration, out type);
        }

        public static bool GetWarInfo(string skinName, out int duration, out WardType type)
        {
            switch (skinName)
            {
                case "YellowTrinket":
                    duration = 60 * 1000;
                    type = WardType.Green;
                    break;
                case "YellowTrinketUpgrade":
                    duration = 60 * 2 * 1000;
                    type = WardType.Green;
                    break;
                case "SightWard":
                    duration = 60 * 3 * 1000;
                    type = WardType.Green;
                    break;
                case "VisionWard":
                    duration = int.MaxValue;
                    type = WardType.Pink;
                    break;
                case "CaitlynTrap":
                    duration = 60 * 4 * 1000;
                    type = WardType.Trap;
                    break;
                case "TeemoMushroom":
                    duration = 60 * 10 * 1000;
                    type = WardType.Trap;
                    break;
                case "Nidalee_Spear":
                    duration = 60 * 2 * 1000;
                    type = WardType.Trap;
                    break;
                case "ShacoBox":
                    duration = 60 * 1 * 1000;
                    type = WardType.Trap;
                    break;
                default:
                    duration = 0;
                    type = WardType.None;
                    return false;
            }
            return true;
        }

        public void DrawCircle()
        {
            _circle = new Render.Circle(ObjAiBase.Position, 100, Color, 5, true);
            _circle.VisibleCondition +=
                sender => _wardDetector.IsActive() && Render.OnScreen(Drawing.WorldToScreen(ObjAiBase.Position));
            _circle.Add(0);

            if (Type != WardType.Trap)
            {
                _minimapSprite = new Render.Sprite(Bitmap, MinimapPosition) { Scale = new Vector2(Scale, Scale) };
                _minimapSprite.Add(0);
            }
            if (Duration == int.MaxValue)
            {
                return;
            }
            _timerText = new Render.Text(10, 10, "t", 18, new ColorBGRA(255, 255, 255, 255))
            {
                OutLined = true,
                PositionUpdate = () => Drawing.WorldToScreen(ObjAiBase.Position),
                Centered = true
            };
            _timerText.VisibleCondition +=
                sender => _wardDetector.IsActive() && Render.OnScreen(Drawing.WorldToScreen(ObjAiBase.Position));
            _timerText.TextUpdate = () => Utils.FormatTime((EndTime - Environment.TickCount) / 1000f);
            _timerText.Add(2);
        }

        public bool RemoveCircle()
        {
            _circle.Remove();
            if (_timerText != null)
            {
                _timerText.Remove();
            }
            if (_minimapSprite != null)
            {
                _minimapSprite.Remove();
            }
            return true;
        }
    }

    public class WardDetector
    {
        private readonly List<Ward> _detectedWards = new List<Ward>();
        public Menu MenuWard;
        public Bitmap Ward = new Bitmap(Helper.MiniMapFolder("ward"));
        public Bitmap Pink = new Bitmap(Helper.MiniMapFolder("pink"));
        public WardDetector(Menu config)
        {
            MenuWard = config;
            MenuWard.AddItem(new MenuItem("WardActive", "Ward")).SetValue(true);
            foreach (GameObject obj in ObjectManager.Get<GameObject>().Where(o => o is Obj_AI_Base))
            {
                Game_OnCreate(obj, null);
            }
            GameObject.OnCreate += Game_OnCreate;
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        public bool IsActive()
        {
            return MenuWard.Item("WardActive").GetValue<bool>();
        }

        private void Game_OnCreate(GameObject sender, EventArgs args)
        {
            if (!IsActive())
            {
                return;
            }
            var @base = sender as Obj_AI_Base;
            if (@base == null)
            {
                return;
            }
            Obj_AI_Base objAiBase = @base;
            if (objAiBase.IsAlly)
            {
                return;
            }
            if (!Tracker.Ward.IsWard(objAiBase.SkinName))
            {
                return;
            }
            int startTime = Environment.TickCount - (int) ((objAiBase.MaxMana - objAiBase.Mana) * 1000);
            _detectedWards.Add(new Ward(objAiBase.SkinName, startTime, objAiBase, this));
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive())
            {
                return;
            }
            _detectedWards.RemoveAll(w => w.ObjAiBase.IsDead && w.RemoveCircle());
        }
    }

    public enum WardType
    {
        None,
        Green,
        Pink,
        Trap,
    }
}