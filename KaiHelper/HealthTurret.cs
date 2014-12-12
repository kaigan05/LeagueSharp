using System;
using System.Globalization;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;

namespace KaiHelper
{
    internal class HealthTurret
    {
        public static Font Text;
        public static Menu MenuHealthTurret;

        static HealthTurret()
        {
            Text = new Font(
                Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Calibri",
                    Height = 13,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.Default,
                });
            Drawing.OnEndScene += DrawTurrentHealth;
        }

        public static void DrawText(Font font, String text, int posX, int posY, Color color)
        {
            Rectangle rec = font.MeasureText(null, text, FontDrawFlags.Center);
            font.DrawText(null, text, posX + 1 + rec.X, posY + 1, Color.Black);
            font.DrawText(null, text, posX + rec.X, posY + 1, Color.Black);
            font.DrawText(null, text, posX - 1 + rec.X, posY - 1, Color.Black);
            font.DrawText(null, text, posX + rec.X, posY - 1, Color.Black);
            font.DrawText(null, text, posX + rec.X, posY, color);
        }
        public static void AttachMenu(Menu menu)
        {
            MenuHealthTurret = menu.AddSubMenu(new Menu("Health", "Health"));
            MenuHealthTurret.AddItem(
                new MenuItem("TurretHealth", "Turret Health").SetValue(new StringList(new[] {"Percent", "Health "})));
            MenuHealthTurret.AddItem(new MenuItem("Active", "Active").SetValue(true));
        }

        private static bool IsActive()
        {
            return MenuHealthTurret.Item("Active").GetValue<bool>();
        }

        private static void DrawTurrentHealth(EventArgs args)
        {
            if (!IsActive())
            {
                return;
            }
            foreach (var turret in ObjectManager.Get<Obj_AI_Turret>())
            {
                if (turret.IsDead || (turret.HealthPercentage() == 100)) continue;
                int health = 0;
                switch (MenuHealthTurret.Item("TurretHealth").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        health = (int)turret.HealthPercentage();
                        break;

                    case 1:
                        health = (int) turret.Health;
                        break;
                } 
                Vector2 pos = Drawing.WorldToMinimap(turret.Position);
                var perHealth=(int)turret.HealthPercentage();
                if (perHealth >= 75)
                {
                    DrawText(Text, health.ToString(CultureInfo.InvariantCulture), (int) pos[0], (int) pos[1],Color.LimeGreen);
                }
                else if (perHealth < 75 && perHealth>=50)
                {
                    DrawText(Text, health.ToString(CultureInfo.InvariantCulture), (int)pos[0], (int)pos[1], Color.YellowGreen);
                }
                else if (perHealth < 50 && perHealth>=25)
                {
                    DrawText(Text, health.ToString(CultureInfo.InvariantCulture), (int)pos[0], (int)pos[1], Color.Orange);
                }
                else if (perHealth < 25)
                {
                    DrawText(Text, health.ToString(CultureInfo.InvariantCulture), (int)pos[0], (int)pos[1], Color.Red);
                }
            }
        }
    }
}