using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace KaiHelper.Activator
{
    internal class Protector
    {
        private readonly Menu _menu;

        public Protector(Menu menu)
        {
            
            //Drawing.OnDraw += Game_OnDraw;
        }

        private void Game_OnDraw(EventArgs args)
        {
            //if (ObjectManager.Player.IsDead || ObjectManager.Player.InFountain())
            //{
            //    return;
            //}
            foreach (var objectEnemy in ObjectManager.Get<Obj_AI_Base>().Where(o =>o.IsAlly && o.IsVisible && !o.IsDead))
            {
                if (objectEnemy is Obj_AI_Hero || objectEnemy is Obj_AI_Turret)
                {
                    Utility.DrawCircle(objectEnemy.Position, 1300, Color.Aqua);
                }
                else
                {
                    Utility.DrawCircle(objectEnemy.Position, 1200, Color.Aqua);
                }
            }
        }
    }
}