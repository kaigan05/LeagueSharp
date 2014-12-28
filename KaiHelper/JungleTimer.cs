using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;

namespace KaiHelper
{

    public static class JungleTimer
    {
        private static readonly List<JungleCamp> JungleCamps = new List<JungleCamp>();

        private static readonly Font Font;

        static JungleTimer()
        {
            JungleInit();
            Font = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Times New Roman", 8));
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }
        public static void JungleInit()
        {
            JungleCamps.Add(new JungleCamp("wightBot", -66254122, 115, 100, new Vector3(1688, 8248, 54), new Vector3(2263.463f, 8571.541f, 1136.772f)));
            JungleCamps.Add(new JungleCamp("wightBot", -60814630, 115, 100, new Vector3(1688, 8248, 54), new Vector3(2263.463f, 8571.541f, 1136.772f)));
            JungleCamps.Add(new JungleCamp("wightBot", -56882472, 115, 100, new Vector3(1688, 8248, 54), new Vector3(2263.463f, 8571.541f, 1136.772f)));
            JungleCamps.Add(new JungleCamp("wightBot", -55571760, 115, 100, new Vector3(1688, 8248, 54), new Vector3(2263.463f, 8571.541f, 1136.772f)));

            JungleCamps.Add(new JungleCamp("wight", -52032808, 115, 100, new Vector3(12266, 6215, 54), new Vector3(12671.58f, 6617.756f, 1118.074f)));
            JungleCamps.Add(new JungleCamp("wight", -59766064, 115, 100, new Vector3(12266, 6215, 54), new Vector3(12671.58f, 6617.756f, 1118.074f)));
            JungleCamps.Add(new JungleCamp("wight", -59503910, 115, 100, new Vector3(12266, 6215, 54), new Vector3(12671.58f, 6617.756f, 1118.074f)));
            JungleCamps.Add(new JungleCamp("wight", -52032808, 115, 100, new Vector3(12266, 6215, 54), new Vector3(12671.58f, 6617.756f, 1118.074f)));
            JungleCamps.Add(new JungleCamp("wight", -66188586, 115, 100, new Vector3(12266, 6215, 54), new Vector3(12671.58f, 6617.756f, 1118.074f)));

            JungleCamps.Add(new JungleCamp("blueBot", 1274, 115, 300, new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f)));

            JungleCamps.Add(new JungleCamp("blue", -6553, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -6409, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -6664, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));
            JungleCamps.Add(new JungleCamp("blue", -5190, 115, 300, new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f)));

            JungleCamps.Add(new JungleCamp("wolvesBot", 1274809044, 115, 100, new Vector3(3430, 6300, 56), new Vector3(3730.419f, 6744.748f, 1100.24f)));
            JungleCamps.Add(new JungleCamp("wolves", -60028198, 115, 100, new Vector3(10570, 8150, 63), new Vector3(11233.96f, 8789.653f, 1051.235f)));
            JungleCamps.Add(new JungleCamp("wolves", -52294952, 115, 100, new Vector3(10570, 8150, 63), new Vector3(11233.96f, 8789.653f, 1051.235f)));
            JungleCamps.Add(new JungleCamp("wolves", -51311920, 115, 100, new Vector3(10570, 8150, 63), new Vector3(11233.96f, 8789.653f, 1051.235f)));
            JungleCamps.Add(new JungleCamp("wolves", -66581802, 115, 100, new Vector3(10570, 8150, 63), new Vector3(11233.96f, 8789.653f, 1051.235f)));
            JungleCamps.Add(new JungleCamp("wraithsBot", -63960360, 115, 100, new Vector3(6540, 7230, 56), new Vector3(7069.483f, 5800.1f, 1064.815f)));
            JungleCamps.Add(new JungleCamp("wraithsBot", -66581798, 115, 100, new Vector3(6540, 7230, 56), new Vector3(7069.483f, 5800.1f, 1064.815f)));
            JungleCamps.Add(new JungleCamp("wraithsBot", -64025904, 115, 100, new Vector3(6540, 7230, 56), new Vector3(7069.483f, 5800.1f, 1064.815f)));
            JungleCamps.Add(new JungleCamp("wraithsBot", -66909482, 115, 100, new Vector3(6540, 7230, 56), new Vector3(7069.483f, 5800.1f, 1064.815f)));

            JungleCamps.Add(new JungleCamp("wraiths", -66516266, 115, 100, new Vector3(7465, 9220, 56), new Vector3(7962.764f, 10028.573f, 1023.06f)));
            JungleCamps.Add(new JungleCamp("wraiths", -59766054, 115, 100, new Vector3(7465, 9220, 56), new Vector3(7962.764f, 10028.573f, 1023.06f)));
            JungleCamps.Add(new JungleCamp("wraiths", -52163880, 115, 100, new Vector3(7465, 9220, 56), new Vector3(7962.764f, 10028.573f, 1023.06f)));
            JungleCamps.Add(new JungleCamp("wraiths", -66188586, 115, 100, new Vector3(7465, 9220, 56), new Vector3(7962.764f, 10028.573f, 1023.06f)));
            JungleCamps.Add(new JungleCamp("wraiths", -55506224, 115, 100, new Vector3(7465, 9220, 56), new Vector3(7962.764f, 10028.573f, 1023.06f)));

            JungleCamps.Add(new JungleCamp("redBot", -6684, 115, 300, new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("redBot", -6527, 115, 300, new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));
            JungleCamps.Add(new JungleCamp("redBot", -5177, 115, 300, new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f)));

            JungleCamps.Add(new JungleCamp("red", -6645, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -6055, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -5970, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));
            JungleCamps.Add(new JungleCamp("red", -5150, 115, 300, new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f)));

            JungleCamps.Add(new JungleCamp("golemsBot", -51639592, 115, 100, new Vector3(7990, 2550, 54), new Vector3(8419.813f, 3239.516f, 1280.222f)));
            JungleCamps.Add(new JungleCamp("golemsBot", -67106086, 115, 100, new Vector3(7990, 2550, 54), new Vector3(8419.813f, 3239.516f, 1280.222f)));
            JungleCamps.Add(new JungleCamp("golemsBot", -66778410, 115, 100, new Vector3(7990, 2550, 54), new Vector3(8419.813f, 3239.516f, 1280.222f)));
            JungleCamps.Add(new JungleCamp("golemsBot", -55702832, 115, 100, new Vector3(7990, 2550, 54), new Vector3(8419.813f, 3239.516f, 1280.222f)));

            JungleCamps.Add(new JungleCamp("golems", -60290342, 115, 100, new Vector3(6010, 11920, 40), new Vector3(6508.562f, 12127.83f, 1185.667f)));
            JungleCamps.Add(new JungleCamp("golems", -63894832, 115, 100, new Vector3(6010, 11920, 40), new Vector3(6508.562f, 12127.83f, 1185.667f)));
            JungleCamps.Add(new JungleCamp("golems", -66385194, 115, 100, new Vector3(6010, 11920, 40), new Vector3(6508.562f, 12127.83f, 1185.667f)));
            JungleCamps.Add(new JungleCamp("golems", -52426024, 115, 100, new Vector3(6010, 11920, 40), new Vector3(6508.562f, 12127.83f, 1185.667f)));

            JungleCamps.Add(new JungleCamp("crabBot", -53802278, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(4535.956f, 10104.067f, 1029.071f)));
            JungleCamps.Add(new JungleCamp("crabBot", -66057514, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(4535.956f, 10104.067f, 1029.071f)));
            JungleCamps.Add(new JungleCamp("crabBot", -53802278, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(4535.956f, 10104.067f, 1029.071f)));
            JungleCamps.Add(new JungleCamp("crabBot", -53802278, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(4535.956f, 10104.067f, 1029.071f)));
            JungleCamps.Add(new JungleCamp("crabBot", -57537832, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(4535.956f, 10104.067f, 1029.071f)));
            JungleCamps.Add(new JungleCamp("crabBot", -51705136, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(4535.956f, 10104.067f, 1029.071f)));
            JungleCamps.Add(new JungleCamp("crab", -57144616, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(10557.22f, 5481.414f, 1068.042f)));
            JungleCamps.Add(new JungleCamp("crab", -63960368, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(10557.22f, 5481.414f, 1068.042f)));
            JungleCamps.Add(new JungleCamp("crab", -66123050, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(10557.22f, 5481.414f, 1068.042f)));
            JungleCamps.Add(new JungleCamp("crab", -59241766, 2 * 60 + 30, 180, new Vector3(12266, 6215, 54), new Vector3(10557.22f, 5481.414f, 1068.042f)));

            JungleCamps.Add(new JungleCamp("dragon", -6671, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -6579, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -5989, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -5098, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -6579, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));
            JungleCamps.Add(new JungleCamp("dragon", -6579, 2 * 60 + 30, 360, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f)));

            JungleCamps.Add(new JungleCamp("nashor", -5897, 20 * 60, 420, new Vector3(4620, 10265, -63), new Vector3(4951.034f, 10831.035f, 1027.482f)));
            //JungleCamps.Add(new JungleCamp("nashor", -6671, 20 * 60, 420, new Vector3(4620, 10265, -63), new Vector3(4951.034f, 10831.035f, 1027.482f)));
            JungleCamps.Add(new JungleCamp("nashor", -5701, 20 * 60, 420, new Vector3(4620, 10265, -63), new Vector3(4951.034f, 10831.035f, 1027.482f)));
            JungleCamps.Add(new JungleCamp("nashor", -5137, 20 * 60, 420, new Vector3(4620, 10265, -63), new Vector3(4951.034f, 10831.035f, 1027.482f)));
            //RedBot -6684 -6527 -5177
            //Dragon -6671 -6579 -5989
            //Red 6645 -6055
            //Baron -6632 -5897 -6671 -5701 -5137
            //List<JungleCamp> JungleList = JungleCamps.OrderBy(o => o.CampId).ToList();
            //using (StreamWriter sw = new StreamWriter("C:\\Jungle.txt"))
            //{
            //    foreach (var jungle in JungleList)
            //    {
            //        sw.WriteLine(
            //            "JungleCamps.Add(new JungleCamp(\"{0}\", {1}, {2}, {3}, new Vector3({4}, {5}, {6}), new Vector3({7}f, {8}f, {9}f)));",
            //            jungle.Name, jungle.CampId, jungle.SpawnTime, jungle.RespawnTime, jungle.MapPosition.X, jungle.MapPosition.Y, jungle.MapPosition.Z, jungle.MinimapPosition.X, jungle.MinimapPosition.Y, jungle.MinimapPosition.Z);
            //    }
            //}
            //Console.WriteLine("Saved");
        }
        
        private static void Drawing_OnEndScene(EventArgs args)
        {
            try{
            foreach (JungleCamp jungleCamp in JungleCamps)
            {
                if (jungleCamp.NextRespawnTime <= 0)
                    continue;
                Vector2 sPos = Drawing.WorldToMinimap(jungleCamp.MinimapPosition);
                Helper.DrawText(Font, (jungleCamp.NextRespawnTime - (int)Game.ClockTime).ToString(CultureInfo.InvariantCulture),
                    (int)sPos[0], (int)sPos[1], Color.White);
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Jungle: Can't OnDraw " + ex.Message);
            }
        }

        private static JungleCamp GetJungleCampById(int id)
        {
            JungleCamp result=JungleCamps.Find(jm => jm.CampId == id);
            if (result == null)
            {
                Console.WriteLine("Not Found: " + id);
                int i=GetNFromInt(id);
                Console.WriteLine("Next Found: " + i);
                result = JungleCamps.Find(jm => jm.CampId == i);
            }
            return result;
        }

        private static int GetNFromInt(int x)
        {
            int i = Math.Abs(x);
            while (i >= 10000)
                i /= 10;
            if (x < 0)
                i *= -1;
            return i;
        }
        

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive())
                return;
            foreach (JungleCamp jungleCamp in JungleCamps.Where(jungleCamp => (jungleCamp.NextRespawnTime - (int)Game.ClockTime) < 0)) {
                jungleCamp.NextRespawnTime = 0;
            }
        }

        private static void UpdateCamps(int campId)
        {
            JungleCamp jungleCamp = GetJungleCampById(campId);
            if (jungleCamp != null)
            {
                Console.WriteLine("Update " + jungleCamp.Name);
                jungleCamp.NextRespawnTime = (int)Game.ClockTime + jungleCamp.RespawnTime;
            }
        } 
        private static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            if (!IsActive())
                return;
            try
            {
                if (args.PacketData[0] == Packet.S2C.EmptyJungleCamp.Header)
                {
                    var packet = new GamePacket(args.PacketData);
                    Console.WriteLine(Game.Time+" ID " + packet.ReadInteger(6));
                    Console.WriteLine();
                    if (GetNFromInt(packet.ReadInteger(6)) == -5150)
                    {
                        foreach (var b in args.PacketData)
                        {
                            Console.Write(b+"\t");
                        }
                    }
                    Console.WriteLine();
                    UpdateCamps(packet.ReadInteger(6));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loi Packet: " + ex.Message);
            }
        }

        public class JungleCamp
        {
            public int CampId;
            public Vector3 MapPosition;
            public Vector3 MinimapPosition;
            public String Name;
            public int NextRespawnTime;
            public int RespawnTime;
            public int SpawnTime;

            public JungleCamp(String name, int campId, int spawnTime, int respawnTime, Vector3 mapPosition, Vector3 minimapPosition)
            {
                Name = name;
                CampId = campId;
                SpawnTime = spawnTime;
                RespawnTime = respawnTime;
                MapPosition = mapPosition;
                MinimapPosition = minimapPosition;
                NextRespawnTime = 0;
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