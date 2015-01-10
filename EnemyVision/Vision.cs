using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace EnemyVision
{
    internal class Vision
    {
        private readonly Menu _menu;

        public Vision()
        {
            _menu = new Menu("Enemy vision", "Enemyvision",true);
            _menu.AddItem(new MenuItem("DoTron", "Roundness").SetValue(new Slider(20, 1, 20)));
            _menu.AddItem(new MenuItem("DoChinhXac", "Accuracy").SetValue(new Slider(1, 1)));
            _menu.AddItem(new MenuItem("TrenManHinh", "Only draw when enemys on screen").SetValue(false));
            _menu.AddItem(new MenuItem("VongTron", "Only Circle").SetValue(false));
            _menu.AddItem(new MenuItem("NguoiChoiTest", "Test by me").SetValue(false));
            _menu.AddItem(new MenuItem("Active", "Active").SetValue(false));
            _menu.AddToMainMenu();
            Drawing.OnDraw += Game_OnDraw;
            Game.PrintChat("Enemy vision by kaigan Loaded!");
        }
        public static bool UnitTrenManHinh(Obj_AI_Base o)
        {
            var viTri = Drawing.WorldToScreen(o.Position);
            return viTri.X > 0 && viTri.X < Drawing.Width && viTri.Y > 0 && viTri.Y < Drawing.Height;
        }
        public static bool LaVatCan(Vector3 position)
        {
            if (!NavMesh.GetCollisionFlags(position).HasFlag(CollisionFlags.Grass))
            {
                return !NavMesh.GetCollisionFlags(position).HasFlag(CollisionFlags.Building) &&
                       NavMesh.GetCollisionFlags(position).HasFlag(CollisionFlags.Wall);
            }
            return true;
        }

        private void Game_OnDraw(EventArgs args)
        {
            if (!_menu.Item("Active").GetValue<bool>())
            {
                return;
            }
            var result = new Obj_AI_Base();
            if (_menu.Item("NguoiChoiTest").GetValue<bool>())
            {
                result = ObjectManager.Player;
            }
            else
            {
                var posPlayer = ObjectManager.Player.Position.To2D();
                var dist = float.MaxValue;
                foreach (var objectEnemy in ObjectManager.Get<Obj_AI_Base>().Where(o => o.IsEnemy && o.IsVisible && !o.IsDead && o.IsValid && o.Team!=ObjectManager.Player.Team))
                {
                    var distance = Vector2.DistanceSquared(posPlayer, objectEnemy.Position.To2D());
                    if (!(distance < dist))
                    {
                        continue;
                    }
                    dist = distance;
                    result = objectEnemy;
                }
            }
            if (_menu.Item("TrenManHinh").GetValue<bool>())
            {
                if (!UnitTrenManHinh(result))
                    return;
            }
            int tamNhin = result is Obj_AI_Hero || result is Obj_AI_Turret ? 1300 : 1200;
            if (_menu.Item("VongTron").GetValue<bool>())
            {
                Render.Circle.DrawCircle(result.Position, tamNhin, Color.PaleVioletRed,5);
                return;
            }
            var listPoint = new List<Vector2>();
            int doTron = 21 - (_menu.Item("DoTron").GetValue<Slider>().Value);
            int doChinhXac = 101 - (_menu.Item("DoChinhXac").GetValue<Slider>().Value);
            for (int i = 0; i <= 360; i += doTron)
            {
                var cosX = Math.Cos(i * Math.PI / 180);
                var sinY = Math.Sin(i * Math.PI / 180);
                var vongngoai = new Vector3((float)(result.Position.X + tamNhin * cosX),(float)(result.Position.Y + tamNhin * sinY),ObjectManager.Player.Position.Z);
                for (int j = 100; j < tamNhin; j += doChinhXac)
                {
                    var vongtrong = new Vector3((float)(result.Position.X + j * cosX),(float)(result.Position.Y + j * sinY),ObjectManager.Player.Position.Z);
                    if (!LaVatCan(vongtrong))
                    {
                        continue;
                    }
                    vongngoai = vongtrong;
                    break;
                }
                listPoint.Add(Drawing.WorldToScreen(vongngoai));
            }
            for (int i = 0; i < listPoint.Count - 1; i++)
            {
                Drawing.DrawLine(listPoint[i], listPoint[i + 1], 1, Color.PaleVioletRed);
            }
        }

    }

}