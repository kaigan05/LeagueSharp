using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace KaiHelper.Misc
{
    internal class Vision
    {
        private readonly Menu _menu;

        public Vision(Menu menu)
        {
            _menu = menu.AddSubMenu(new Menu("Enemy vision", "Enemyvision"));
            _menu.AddItem(new MenuItem("VongTron", "Only Circle").SetValue(false));
            _menu.AddItem(new MenuItem("NguoiChoiTest", "Test by me").SetValue(false));
            _menu.AddItem(new MenuItem("Active", "Active").SetValue(false));
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
                Vector2 posPlayer = ObjectManager.Player.Position.To2D();
                float dist = float.MaxValue;
                foreach (Obj_AI_Base objectEnemy in
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(
                            o =>
                                o.Team != ObjectManager.Player.Team && o.IsVisible && !o.IsDead &&
                                !o.Name.ToUpper().StartsWith("SRU")))
                {
                    float distance = Vector2.DistanceSquared(posPlayer, objectEnemy.Position.To2D());
                    if (!(distance < dist))
                    {
                        continue;
                    }
                    dist = distance;
                    result = objectEnemy;
                }
            }
            int tamNhin = result is Obj_AI_Hero || result is Obj_AI_Turret ? 1300 : 1200;
            if (_menu.Item("VongTron").GetValue<bool>())
            {
                Render.Circle.DrawCircle(result.Position, tamNhin, Color.PaleVioletRed);
                return;
            }
            var listPoint = new List<Vector2>();
            for (int i = 0; i <= 360; i += 1)
            {
                double cosX = Math.Cos(i * Math.PI / 180);
                double sinX = Math.Sin(i * Math.PI / 180);
                var vongngoai = new Vector3(
                    (float) (result.Position.X + tamNhin * cosX), (float) (result.Position.Y + tamNhin * sinX),
                    ObjectManager.Player.Position.Z);
                for (int j = 0; j < tamNhin; j += 100)
                {
                    var vongtrong = new Vector3(
                        (float) (result.Position.X + j * cosX), (float) (result.Position.Y + j * sinX),
                        ObjectManager.Player.Position.Z);
                    if (!LaVatCan(vongtrong))
                    {
                        continue;
                    }
                    if (j != 0)
                    {
                        int left = j - 99, right = j;
                        do
                        {
                            int middle = (left + right) / 2;
                            vongtrong = new Vector3(
                                (float) (result.Position.X + middle * cosX), (float) (result.Position.Y + middle * sinX),
                                ObjectManager.Player.Position.Z);
                            if (LaVatCan(vongtrong))
                            {
                                right = middle;
                            }
                            else
                            {
                                left = middle + 1;
                            }
                        } while (left < right);
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