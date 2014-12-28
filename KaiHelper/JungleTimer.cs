using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;

namespace KaiHelper
{

    public static class JungleTimer
    {
        private static readonly Utility.Map GMap = Utility.Map.GetMap();
        private static readonly List<JungleCamp> JungleCamps = new List<JungleCamp>();

        private static readonly Font Font;

        static JungleTimer()
        {
            InitJungleMobs();
            Font = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Times New Roman", 8));
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
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
        private static void Drawing_OnEndScene(EventArgs args)
        {
            try{
            foreach (JungleCamp jungleCamp in JungleCamps)
            {
                //Utility.DrawCircle(jungleCamp.MapPosition, 100, System.Drawing.Color.Red);
                //Vector2 sPos = Drawing.WorldToMinimap(jungleCamp.MinimapPosition);
                //DrawText(Font, jungleCamp.Name, (int)sPos[0], (int)sPos[1], Color.White);
                if (jungleCamp.NextRespawnTime <= 0)
                    continue;
                Vector2 sPos = Drawing.WorldToMinimap(jungleCamp.MinimapPosition);
                DrawText(Font, (jungleCamp.NextRespawnTime - (int)Game.ClockTime).ToString(CultureInfo.InvariantCulture),
                    (int)sPos[0], (int)sPos[1], Color.White);
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Jungle: Can't OnDraw " + ex.Message);
            }
        }

        //private static JungleMob GetJungleMobByName(string name, Utility.Map.MapType mapType)
        //{
        //    return JungleMobs.Find(jm => jm.Name == name && jm.MapType == mapType);
        //}

        private static JungleCamp GetJungleCampById(int id)
        {
            return JungleCamps.Find(jm => jm.CampId == id);
        }

        public static void InitJungleMobs()
        {
            //JungleCamps.Add(new JungleCamp("blue", 1, 115, 300, Utility.Map.MapType.SummonersRift,
            //    new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f),;
            //JungleCamps.Add(new JungleCamp("wolves", GameObjectTeam.Order, 2, 115, 100, Utility.Map.MapType.SummonersRift,
            //    new Vector3(3430, 6300, 56), new Vector3(3730.419f, 6744.748f, 1100.24f)
            //    );
            //JungleCamps.Add(new JungleCamp("wraiths", GameObjectTeam.Order, 3, 115, 100, Utility.Map.MapType.SummonersRift,
            //    new Vector3(6540, 7230, 56), new Vector3(7069.483f, 5800.1f, 1064.815f),
            //    new[]
            //    {
            //        GetJungleMobByName("SRU_Razorbeak", Utility.Map.MapType.SummonersRift),
            //        GetJungleMobByName("SRU_RazorbeakMini", Utility.Map.MapType.SummonersRift),
            //        GetJungleMobByName("SRU_RazorbeakMini", Utility.Map.MapType.SummonersRift),
            //        GetJungleMobByName("SRU_RazorbeakMini", Utility.Map.MapType.SummonersRift)
            //    }));
            //JungleCamps.Add(new JungleCamp("red", GameObjectTeam.Order, 4, 115, 300, Utility.Map.MapType.SummonersRift,
            //   6,
            //    new[]
            //    {
            //        GetJungleMobByName("SRU_Red", Utility.Map.MapType.SummonersRift),
            //        GetJungleMobByName("SRU_RedMini", Utility.Map.MapType.SummonersRift),
            //        GetJungleMobByName("SRU_RedMini", Utility.Map.MapType.SummonersRift)
            //    }));
            //JungleCamps.Add(new JungleCamp("golems", GameObjectTeam.Order, 5, 115, 100, Utility.Map.MapType.SummonersRift,
            //    new Vector3(7990, 2550, 54), new Vector3(8419.813f, 3239.516f, 1280.222f),
            //    new[]
            //    {
            //        GetJungleMobByName("SRU_Krug", Utility.Map.MapType.SummonersRift),
            //        GetJungleMobByName("SRU_KrugMini", Utility.Map.MapType.SummonersRift)
            //    }));
            //JungleCamps.Add(new JungleCamp("wight", GameObjectTeam.Order, 13, 115, 100, Utility.Map.MapType.SummonersRift,
            //    new Vector3(1688, 8248, 54), new Vector3(2263.463f, 8571.541f, 1136.772f),
            //    new[] { GetJungleMobByName("SRU_Gromp", Utility.Map.MapType.SummonersRift) }));
            ///
            JungleCamps.Add(new JungleCamp("wight", -66254122, 115, 100,new Vector3(1688, 8248, 54), new Vector3(2263.463f, 8571.541f, 1136.772f)));
            JungleCamps.Add(new JungleCamp("wight", -60814630, 115, 100, new Vector3(1688, 8248, 54), new Vector3(2263.463f, 8571.541f, 1136.772f)));
            JungleCamps.Add(new JungleCamp("wight", -56882472, 115, 100, new Vector3(1688, 8248, 54), new Vector3(2263.463f, 8571.541f, 1136.772f)));
            JungleCamps.Add(new JungleCamp("wight", -55571760, 115, 100, new Vector3(1688, 8248, 54), new Vector3(2263.463f, 8571.541f, 1136.772f)));
            
            JungleCamps.Add(new JungleCamp("wight", -52032808, 115, 100, new Vector3(12266, 6215, 54), new Vector3(12671.58f, 6617.756f, 1118.074f)));
            JungleCamps.Add(new JungleCamp("wight", -59766064, 115, 100, new Vector3(12266, 6215, 54), new Vector3(12671.58f, 6617.756f, 1118.074f)));
            JungleCamps.Add(new JungleCamp("wight", -59503910, 115, 100, new Vector3(12266, 6215, 54), new Vector3(12671.58f, 6617.756f, 1118.074f)));
            JungleCamps.Add(new JungleCamp("wight", -52032808, 115, 100, new Vector3(12266, 6215, 54), new Vector3(12671.58f, 6617.756f, 1118.074f)));
            JungleCamps.Add(new JungleCamp("wight", -66188586, 115, 100, new Vector3(12266, 6215, 54), new Vector3(12671.58f, 6617.756f, 1118.074f)));
            
            JungleCamps.Add(new JungleCamp("blue", 1274808322, 115, 300,new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));
            JungleCamps.Add(new JungleCamp("blue", 1274808578, 115, 300, new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));
            JungleCamps.Add(new JungleCamp("blue", 1274809234, 115, 300, new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));
            JungleCamps.Add(new JungleCamp("blue", 1274809218, 115, 300, new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));
            JungleCamps.Add(new JungleCamp("blue", 1274808450, 115, 300, new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));
            JungleCamps.Add(new JungleCamp("blue", 1274808466, 115, 300, new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));
            JungleCamps.Add(new JungleCamp("blue", 1274808434, 115, 300, new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));
            JungleCamps.Add(new JungleCamp("blue", 1274808338, 115, 300, new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));
            JungleCamps.Add(new JungleCamp("blue", 1274808546, 115, 300, new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));
            JungleCamps.Add(new JungleCamp("blue", 1274808418, 115, 300, new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));
            JungleCamps.Add(new JungleCamp("blue", 1274808562, 115, 300, new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));
            JungleCamps.Add(new JungleCamp("blue", 1274809314, 115, 300, new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));
            JungleCamps.Add(new JungleCamp("blue", 1274808674, 115, 300, new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));
            JungleCamps.Add(new JungleCamp("blue", 1274809330, 115, 300, new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));
            JungleCamps.Add(new JungleCamp("blue", 1274808594, 115, 300, new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));
            JungleCamps.Add(new JungleCamp("blue", 1274808690, 115, 300, new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));

            JungleCamps.Add(new JungleCamp("blue", -51902200, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -51902456, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -65533590, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -64091792, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -51902328, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -66647834, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -59503910, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -51901560, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -65532934, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -66647066, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -64091248, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -51902088, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -65533030, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -64091904, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -64091920, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -65533030, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -66647946, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -66647930, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -64092048, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -65533926, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -65533814, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -65533574, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            
            
            
            JungleCamps.Add(new JungleCamp("wolves", 1274809044, 115, 100,new Vector3(3430, 6300, 56), new Vector3(3730.419f, 6744.748f, 1100.24f)));
            JungleCamps.Add(new JungleCamp("wolves", -60028198, 115, 100,  new Vector3(10570, 8150, 63), new Vector3(11233.96f, 8789.653f, 1051.235f)));
            JungleCamps.Add(new JungleCamp("wolves", -52294952, 115, 100, new Vector3(10570, 8150, 63), new Vector3(11233.96f, 8789.653f, 1051.235f)));
            JungleCamps.Add(new JungleCamp("wolves", -51311920, 115, 100, new Vector3(10570, 8150, 63), new Vector3(11233.96f, 8789.653f, 1051.235f)));
            JungleCamps.Add(new JungleCamp("wolves", -66581802, 115, 100, new Vector3(10570, 8150, 63), new Vector3(11233.96f, 8789.653f, 1051.235f)));
            JungleCamps.Add(new JungleCamp("wraiths", -63960360, 115, 100, new Vector3(6540, 7230, 56), new Vector3(7069.483f, 5800.1f, 1064.815f)));
            JungleCamps.Add(new JungleCamp("wraiths", -66581798, 115, 100, new Vector3(6540, 7230, 56), new Vector3(7069.483f, 5800.1f, 1064.815f)));
            JungleCamps.Add(new JungleCamp("wraiths", -64025904, 115, 100, new Vector3(6540, 7230, 56), new Vector3(7069.483f, 5800.1f, 1064.815f)));
            JungleCamps.Add(new JungleCamp("wraiths", -66909482, 115, 100, new Vector3(6540, 7230, 56), new Vector3(7069.483f, 5800.1f, 1064.815f)));

            JungleCamps.Add(new JungleCamp("wraiths", -66516266, 115, 100, new Vector3(7465, 9220, 56), new Vector3(7962.764f, 10028.573f, 1023.06f)));
            JungleCamps.Add(new JungleCamp("wraiths", -59766054, 115, 100, new Vector3(7465, 9220, 56), new Vector3(7962.764f, 10028.573f, 1023.06f)));
            JungleCamps.Add(new JungleCamp("wraiths", -52163880, 115, 100, new Vector3(7465, 9220, 56), new Vector3(7962.764f, 10028.573f, 1023.06f)));
            JungleCamps.Add(new JungleCamp("wraiths", -66188586, 115, 100, new Vector3(7465, 9220, 56), new Vector3(7962.764f, 10028.573f, 1023.06f)));
            JungleCamps.Add(new JungleCamp("wraiths", -55506224, 115, 100, new Vector3(7465, 9220, 56), new Vector3(7962.764f, 10028.573f, 1023.06f)));

            JungleCamps.Add(new JungleCamp("red", -51770392, 115, 300, new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -66843674, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -66844554, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -66844394, 115, 300, new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -66844538, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -51509232, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -51508256, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -65270886, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -51508352, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -51508240, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -51508880, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -51509232, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -51771240, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -51771144, 115, 300, new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -65271798, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -66844298, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -51771128, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -51171160, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -66843754, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -65270790, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -51771112, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -51508336, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -65271526, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -51771016, 115, 300,new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -66844314, 115, 300, new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -51771272, 115, 300, new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -65271702, 115, 300, new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -66844570, 115, 300, new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("red", -66844426, 115, 300, new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));

            JungleCamps.Add(new JungleCamp("red", -66451354, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -51509016, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -66451082, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -59700976, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -51508344, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -51508232, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -51509224, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -60553110, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -66451194, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -60552854, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -51508968, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -66450554, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -51508984, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -60553206, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -59701104, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -51509112, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -66844410, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -60552214, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -60552310, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -60552982, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));

            JungleCamps.Add(new JungleCamp("golems", -51639592, 115, 100, new Vector3(7990, 2550, 54), new Vector3(8419.813f, 3239.516f, 1280.222f)));
            JungleCamps.Add(new JungleCamp("golems", -67106086, 115, 100, new Vector3(7990, 2550, 54), new Vector3(8419.813f, 3239.516f, 1280.222f)));
            JungleCamps.Add(new JungleCamp("golems", -66778410, 115, 100, new Vector3(7990, 2550, 54), new Vector3(8419.813f, 3239.516f, 1280.222f)));
            JungleCamps.Add(new JungleCamp("golems", -55702832, 115, 100, new Vector3(7990, 2550, 54), new Vector3(8419.813f, 3239.516f, 1280.222f)));

            JungleCamps.Add(new JungleCamp("golems", -60290342, 115, 100, new Vector3(6010, 11920, 40), new Vector3(6508.562f, 12127.83f, 1185.667f)));
            JungleCamps.Add(new JungleCamp("golems", -63894832, 115, 100, new Vector3(6010, 11920, 40), new Vector3(6508.562f, 12127.83f, 1185.667f)));
            JungleCamps.Add(new JungleCamp("golems", -66385194, 115, 100, new Vector3(6010, 11920, 40), new Vector3(6508.562f, 12127.83f, 1185.667f)));
            JungleCamps.Add(new JungleCamp("golems", -52426024, 115, 100, new Vector3(6010, 11920, 40), new Vector3(6508.562f, 12127.83f, 1185.667f)));
            
            JungleCamps.Add(new JungleCamp("crab", -53802278, 2 * 60 + 30, 180,new Vector3(12266, 6215, 54), new Vector3(4535.956f, 10104.067f, 1029.071f)));
            JungleCamps.Add(new JungleCamp("crab", -66057514, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(4535.956f, 10104.067f, 1029.071f)));
            JungleCamps.Add(new JungleCamp("crab", -53802278, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(4535.956f, 10104.067f, 1029.071f)));
            JungleCamps.Add(new JungleCamp("crab", -53802278, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(4535.956f, 10104.067f, 1029.071f)));
            JungleCamps.Add(new JungleCamp("crab", -57537832, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(4535.956f, 10104.067f, 1029.071f)));
            JungleCamps.Add(new JungleCamp("crab", -51705136, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(4535.956f, 10104.067f, 1029.071f)));
            JungleCamps.Add(new JungleCamp("crab", -57144616, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(10557.22f, 5481.414f, 1068.042f)));
            JungleCamps.Add(new JungleCamp("crab", -63960368, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(10557.22f, 5481.414f, 1068.042f)));
            JungleCamps.Add(new JungleCamp("crab", -66123050, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(10557.22f, 5481.414f, 1068.042f)));
            JungleCamps.Add(new JungleCamp("crab", -59241766, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(10557.22f, 5481.414f, 1068.042f)));

            JungleCamps.Add(new JungleCamp("dragon", -50984840, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -66712586, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -59896848, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -59897760, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -65796070, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -50984952, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -65795958, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -50984808, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -66713354, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -65795974, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -66713482, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -66713594, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -66713578, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -65795718, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("nashor", -58979350, 20 * 60, 420, new Vector3(4620, 10265, -63), new Vector3(4951.034f, 10831.035f, 1027.482f)));
            JungleCamps.Add(new JungleCamp("nashor", -66320010, 20 * 60, 420, new Vector3(4620, 10265, -63), new Vector3(4951.034f, 10831.035f, 1027.482f)));
            JungleCamps.Add(new JungleCamp("nashor", -66713338, 20 * 60, 420, new Vector3(4620, 10265, -63), new Vector3(4951.034f, 10831.035f, 1027.482f)));
            JungleCamps.Add(new JungleCamp("nashor", -57014136, 20 * 60, 420, new Vector3(4620, 10265, -63), new Vector3(4951.034f, 10831.035f, 1027.482f)));
            JungleCamps.Add(new JungleCamp("nashor", -57013256, 20 * 60, 420, new Vector3(4620, 10265, -63), new Vector3(4951.034f, 10831.035f, 1027.482f)));
            //    new[] { GetJungleMobByName("Dragon", Utility.Map.MapType.SummonersRift) }));
            /// 
            //JungleCamps.Add(new JungleCamp("blue", GameObjectTeam.Chaos, 7, 115, 300, Utility.Map.MapType.SummonersRift,
            //    new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f),
            //    new[]
            //    {
            //        GetJungleMobByName("SRU_Blue", Utility.Map.MapType.SummonersRift),
            //        GetJungleMobByName("SRU_BlueMini", Utility.Map.MapType.SummonersRift),
            //        GetJungleMobByName("SRU_BlueMini2", Utility.Map.MapType.SummonersRift)
            //    }));
            //JungleCamps.Add(new JungleCamp("wolves", GameObjectTeam.Chaos, 8, 115, 100, Utility.Map.MapType.SummonersRift,
            //    new Vector3(10570, 8150, 63), new Vector3(11233.96f, 8789.653f, 1051.235f),
            //    new[]
            //    {
            //        GetJungleMobByName("SRU_Murkwolf", Utility.Map.MapType.SummonersRift),
            //        GetJungleMobByName("SRU_MurkwolfMini", Utility.Map.MapType.SummonersRift),
            //        GetJungleMobByName("SRU_MurkwolfMini", Utility.Map.MapType.SummonersRift)
            //    }));
            //JungleCamps.Add(new JungleCamp("wraiths", GameObjectTeam.Chaos, 9, 115, 100,
            //    Utility.Map.MapType.SummonersRift, new Vector3(7465, 9220, 56), new Vector3(7962.764f, 10028.573f, 1023.06f),
            //    new[]
            //    {
            //        GetJungleMobByName("SRU_Razorbeak", Utility.Map.MapType.SummonersRift),
            //        GetJungleMobByName("SRU_RazorbeakMini", Utility.Map.MapType.SummonersRift),
            //        GetJungleMobByName("SRU_RazorbeakMini", Utility.Map.MapType.SummonersRift),
            //        GetJungleMobByName("SRU_RazorbeakMini", Utility.Map.MapType.SummonersRift)
            //    }));
            //JungleCamps.Add(new JungleCamp("red", GameObjectTeam.Chaos, 10, 115, 300, Utility.Map.MapType.SummonersRift,
            //    new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f),
            //    new[]
            //    {
            //        GetJungleMobByName("SRU_Red", Utility.Map.MapType.SummonersRift),
            //        GetJungleMobByName("SRU_RedMini", Utility.Map.MapType.SummonersRift),
            //        GetJungleMobByName("SRU_RedMini", Utility.Map.MapType.SummonersRift)
            //    }));
            //JungleCamps.Add(new JungleCamp("golems", GameObjectTeam.Chaos, 11, 115, 100,
            //    Utility.Map.MapType.SummonersRift, new Vector3(6010, 11920, 40), new Vector3(6508.562f, 12127.83f, 1185.667f),
            //    new[]
            //    {
            //        GetJungleMobByName("SRU_Krug", Utility.Map.MapType.SummonersRift),
            //        GetJungleMobByName("SRU_KrugMini", Utility.Map.MapType.SummonersRift)
            //    }));
            //JungleCamps.Add(new JungleCamp("wight", GameObjectTeam.Chaos, 14, 115, 100, Utility.Map.MapType.SummonersRift,
            //    new Vector3(12266, 6215, 54), new Vector3(12671.58f, 6617.756f, 1118.074f),
            //    new[] { GetJungleMobByName("SRU_Gromp", Utility.Map.MapType.SummonersRift) }));
            //JungleCamps.Add(new JungleCamp("crab", GameObjectTeam.Neutral, 15, 2 * 60 + 30, 180, Utility.Map.MapType.SummonersRift,
            //    new Vector3(12266, 6215, 54), new Vector3(10557.22f, 5481.414f, 1068.042f),
            //    new[] { GetJungleMobByName("SRU_Crab", Utility.Map.MapType.SummonersRift) }));
            //JungleCamps.Add(new JungleCamp("crab", GameObjectTeam.Neutral, 16, 2 * 60 + 30, 180, Utility.Map.MapType.SummonersRift,
            //    new Vector3(12266, 6215, 54), new Vector3(4535.956f, 10104.067f, 1029.071f),
            //    new[] { GetJungleMobByName("SRU_Crab", Utility.Map.MapType.SummonersRift) }));
            //JungleCamps.Add(new JungleCamp("dragon", GameObjectTeam.Neutral, 6, 2 * 60 + 30, 360,
            //    Utility.Map.MapType.SummonersRift, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f),
            //    new[] { GetJungleMobByName("Dragon", Utility.Map.MapType.SummonersRift) }));
            //JungleCamps.Add(new JungleCamp("nashor", GameObjectTeam.Neutral, 12, 20 * 60, 420,
            //    Utility.Map.MapType.SummonersRift, new Vector3(4620, 10265, -63), new Vector3(4951.034f, 10831.035f, 1027.482f),
            //    new[] { GetJungleMobByName("Worm", Utility.Map.MapType.SummonersRift) }));
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive())
                return;
            foreach (JungleCamp jungleCamp in JungleCamps)
            {
                if ((jungleCamp.NextRespawnTime - (int)Game.ClockTime) < 0)
                {
                    jungleCamp.NextRespawnTime = 0;
                    jungleCamp.Called = false;
                }
            }   
        }

        private static void UpdateCamps(int campId, byte emptyType)
        {
            if (emptyType != 3)
            {
                JungleCamp jungleCamp = GetJungleCampById(campId);
                if (jungleCamp != null)
                {
                    Console.WriteLine("Update " + jungleCamp.Name);
                    jungleCamp.NextRespawnTime = (int)Game.ClockTime + jungleCamp.RespawnTime;
                }
            }
        }
        //Cho 212 10
        //Ech 216 10 156
        //blue 130
        //ga 218
        //red 0

        private static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            if (!IsActive())
                return;
            try
            {
                if (args.PacketData[0] == Packet.S2C.EmptyJungleCamp.Header)
                {
                    var packet = new GamePacket(args.PacketData);
                    //Console.WriteLine(Game.Time + " TEST " + packet.ReadInteger(1));
                    //Console.WriteLine(Game.Time + " TEST " + packet.ReadInteger(2));
                    //Console.WriteLine(Game.Time + " TEST " + packet.ReadInteger(3));
                    //Console.WriteLine(Game.Time + " TEST " + packet.ReadInteger(4));
                    //Console.WriteLine(Game.Time + " TEST " + packet.ReadInteger(5));
                    Console.WriteLine(Game.Time+" ID " + packet.ReadInteger(6));
                    UpdateCamps(packet.ReadInteger(6), 1);
                    //Packet.S2C.EmptyJungleCamp.Struct camp = Packet.S2C.EmptyJungleCamp.Decoded(args.PacketData);
                    //Console.WriteLine("DDDDDDDD: " + (byte)packet.ReadByte(6));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loi Packet: " + ex.Message);
            }
        }

        public class JungleCamp
        {
            public bool Called;
            public int CampId;
            //public JungleMob[] Creeps;
            public Vector3 MapPosition;
            public Utility.Map.MapType MapType;
            public Vector3 MinimapPosition;
            public String Name;
            public int NextRespawnTime;
            public int RespawnTime;
            public int SpawnTime;
            public GameObjectTeam Team;

            public JungleCamp(String name, int campId, int spawnTime, int respawnTime, Vector3 mapPosition, Vector3 minimapPosition)
            {
                Name = name;
                CampId = campId;
                SpawnTime = spawnTime;
                RespawnTime = respawnTime;
                MapPosition = mapPosition;
                MinimapPosition = minimapPosition;
                NextRespawnTime = 0;
                Called = false;
            }
        }

       public static void AttachMenu(Menu menu)
        {
            _menuJungle = menu;
            _menuJungle.AddItem(new MenuItem("JungleActive", "Jungle Timer").SetValue(true));
        }

        private static bool IsActive()
        {
            return _menuJungle.Item("JungleActive").GetValue<bool>();
        }
        private static Menu _menuJungle;
    }
}