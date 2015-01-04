using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Enemy_Vision
{
    internal class Vision
    {
        private readonly Menu _menu;

        public Vision()
        {
            _menu = new Menu("Enemy Vision", "Enemy vision",true);
            _menu.AddItem(new MenuItem("DoTron", "Roundness").SetValue(new Slider(11, 1, 20)));
            _menu.AddItem(new MenuItem("DoChinhXac", "Accuracy").SetValue(new Slider(1, 1)));
            _menu.AddItem(new MenuItem("TrenManHinh", "Only draw when enemys on screen").SetValue(false));
            _menu.AddItem(new MenuItem("VongTron", "Only Circle").SetValue(false));
            _menu.AddItem(new MenuItem("NguoiChoiTest", "Test by me").SetValue(false));
            _menu.AddItem(new MenuItem("Active", "Active").SetValue(false));
            _menu.AddToMainMenu();
            Drawing.OnDraw += Game_OnDraw;
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
        public static bool UnitTrenManHinh(Obj_AI_Base o)
        {
            var viTri = Drawing.WorldToScreen(o.Position);
            return viTri.X > 0 && viTri.X < Drawing.Width && viTri.Y > 0 && viTri.Y < Drawing.Height;
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
                foreach (var objectEnemy in
                    ObjectManager.Get<Obj_AI_Base>().Where(o => o.IsEnemy && o.IsVisible && !o.IsDead && o.IsValid&&!o.Name.ToUpper().StartsWith("SRU")))
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
                if(!UnitTrenManHinh(result))
                    return;
            }
            int tamNhin = result is Obj_AI_Hero || result is Obj_AI_Turret ? 1300 : 1200;
            if (_menu.Item("VongTron").GetValue<bool>())
            {
                Utility.DrawCircle(result.Position, tamNhin, System.Drawing.Color.PaleVioletRed);
                return;
            }
            var listPoint = new List<Vector3>();
            int doTron = 21 - (_menu.Item("DoTron").GetValue<Slider>().Value);
            int doChinhXac = 101 - (_menu.Item("DoChinhXac").GetValue<Slider>().Value);
            for (int i = 0; i <= 360; i += doTron)
            {
                var vongngoai = new Vector3(
                    (float) (result.Position.X + tamNhin * Math.Cos(i * Math.PI / 180)),
                    (float) (result.Position.Y + tamNhin * Math.Sin(i * Math.PI / 180)),
                    ObjectManager.Player.Position.Z);
                for (int j = 100; j < tamNhin; j += doChinhXac)
                {
                    var vongtrong = new Vector3(
                        (float) (result.Position.X + j * Math.Cos(i * Math.PI / 180)),
                        (float) (result.Position.Y + j * Math.Sin(i * Math.PI / 180)),
                        ObjectManager.Player.Position.Z);
                    if (!LaVatCan(vongtrong))
                    {
                        continue;
                    }
                    vongngoai = vongtrong;
                    break;
                }
                listPoint.Add(vongngoai);
            }
            for (int i = 0; i < listPoint.Count - 1; i++)
            {
                Vector2 v1 = Drawing.WorldToScreen(listPoint[i]);
                Vector2 v2 = Drawing.WorldToScreen(listPoint[i + 1]);
                Drawing.DrawLine(v1, v2, 1, System.Drawing.Color.PaleVioletRed);
            }
        }

    }

}