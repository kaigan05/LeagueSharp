using System;
using System.Collections.Generic;
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
        private static readonly List<JungleMob> JungleMobs = new List<JungleMob>();
        private static readonly List<JungleCamp> JungleCamps = new List<JungleCamp>();

        private static readonly Font Font;

        static JungleTimer()
        {
            Font = new Font(Drawing.Direct3DDevice, new System.Drawing.Font("Times New Roman", 8));
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnEndScene += Drawing_OnEndScene;
            InitJungleMobs();
            
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
            foreach (JungleCamp jungleCamp in JungleCamps)
            {
                if (jungleCamp.NextRespawnTime <= 0 || jungleCamp.MapType != GMap._MapType)
                    continue;
                Vector2 sPos = Drawing.WorldToMinimap(jungleCamp.MinimapPosition);
                DrawText(Font, (jungleCamp.NextRespawnTime - (int)Game.ClockTime).ToString(),
                    (int)sPos[0], (int)sPos[1], Color.White);
            }
        }

        private static JungleMob GetJungleMobByName(string name, Utility.Map.MapType mapType)
        {
            return JungleMobs.Find(jm => jm.Name == name && jm.MapType == mapType);
        }

        private static JungleCamp GetJungleCampById(int id, Utility.Map.MapType mapType)
        {
            return JungleCamps.Find(jm => jm.CampId == id && jm.MapType == mapType);
        }

        public static void InitJungleMobs()
        {
            JungleMobs.Add(new JungleMob("SRU_Blue", null, true, true, false, Utility.Map.MapType.SummonersRift));
            JungleMobs.Add(new JungleMob("SRU_Murkwolf", null, true, false, false, Utility.Map.MapType.SummonersRift));
            JungleMobs.Add(new JungleMob("SRU_Razorbeak", null, true, false, false, Utility.Map.MapType.SummonersRift));
            JungleMobs.Add(new JungleMob("SRU_Red", null, true, true, false, Utility.Map.MapType.SummonersRift));
            JungleMobs.Add(new JungleMob("SRU_Krug", null, true, false, false, Utility.Map.MapType.SummonersRift));
            JungleMobs.Add(new JungleMob("SRU_Baron", null, true, true, true, Utility.Map.MapType.SummonersRift));
            JungleMobs.Add(new JungleMob("SRU_Dragon", null, true, false, true, Utility.Map.MapType.SummonersRift));
            JungleMobs.Add(new JungleMob("SRU_Gromp", null, true, false, false, Utility.Map.MapType.SummonersRift));
            JungleMobs.Add(new JungleMob("SRU_Crab", null, true, false, false, Utility.Map.MapType.SummonersRift));
            JungleMobs.Add(new JungleMob("SRU_RedMini", null, false, false, false, Utility.Map.MapType.SummonersRift));
            JungleMobs.Add(new JungleMob("SRU_MurkwolfMini", null, false, false, false, Utility.Map.MapType.SummonersRift));
            JungleMobs.Add(new JungleMob("SRU_RazorbeakMini", null, false, false, false, Utility.Map.MapType.SummonersRift));
            JungleMobs.Add(new JungleMob("SRU_KrugMini", null, false, false, false, Utility.Map.MapType.SummonersRift));
            JungleMobs.Add(new JungleMob("SRU_BlueMini", null, false, false, false, Utility.Map.MapType.SummonersRift));
            JungleMobs.Add(new JungleMob("SRU_BlueMini2", null, false, false, false, Utility.Map.MapType.SummonersRift));
            
            JungleCamps.Add(new JungleCamp("blue", GameObjectTeam.Order, 1, 115, 300, Utility.Map.MapType.SummonersRift,
                new Vector3(3570, 7670, 54), new Vector3(3641.058f, 8144.426f, 1105.46f),
                new[]
                {
                    GetJungleMobByName("SRU_Blue", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_BlueMini", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_BlueMini2", Utility.Map.MapType.SummonersRift)
                }));
            JungleCamps.Add(new JungleCamp("wolves", GameObjectTeam.Order, 2, 115, 100, Utility.Map.MapType.SummonersRift,
                new Vector3(3430, 6300, 56), new Vector3(3730.419f, 6744.748f, 1100.24f),
                new[]
                {
                    GetJungleMobByName("SRU_Murkwolf", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_MurkwolfMini", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_MurkwolfMini", Utility.Map.MapType.SummonersRift)
                }));
            JungleCamps.Add(new JungleCamp("wraiths", GameObjectTeam.Order, 3, 115, 100, Utility.Map.MapType.SummonersRift,
                new Vector3(6540, 7230, 56), new Vector3(7069.483f, 5800.1f, 1064.815f),
                new[]
                {
                    GetJungleMobByName("SRU_Razorbeak", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_RazorbeakMini", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_RazorbeakMini", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_RazorbeakMini", Utility.Map.MapType.SummonersRift)
                }));
            JungleCamps.Add(new JungleCamp("red", GameObjectTeam.Order, 4, 115, 300, Utility.Map.MapType.SummonersRift,
                new Vector3(7370, 3830, 58), new Vector3(7710.639f, 3963.267f, 1200.182f),
                new[]
                {
                    GetJungleMobByName("SRU_Red", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_RedMini", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_RedMini", Utility.Map.MapType.SummonersRift)
                }));
            JungleCamps.Add(new JungleCamp("golems", GameObjectTeam.Order, 5, 115, 100, Utility.Map.MapType.SummonersRift,
                new Vector3(7990, 2550, 54), new Vector3(8419.813f, 3239.516f, 1280.222f),
                new[]
                {
                    GetJungleMobByName("SRU_Krug", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_KrugMini", Utility.Map.MapType.SummonersRift)
                }));
            JungleCamps.Add(new JungleCamp("wight", GameObjectTeam.Order, 13, 115, 100, Utility.Map.MapType.SummonersRift,
                new Vector3(1688, 8248, 54), new Vector3(2263.463f, 8571.541f, 1136.772f),
                new[] { GetJungleMobByName("SRU_Gromp", Utility.Map.MapType.SummonersRift) }));
            JungleCamps.Add(new JungleCamp("blue", GameObjectTeam.Chaos, 7, 115, 300, Utility.Map.MapType.SummonersRift,
                new Vector3(10455, 6800, 55), new Vector3(11014.81f, 7251.099f, 1073.918f),
                new[]
                {
                    GetJungleMobByName("SRU_Blue", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_BlueMini", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_BlueMini2", Utility.Map.MapType.SummonersRift)
                }));
            JungleCamps.Add(new JungleCamp("wolves", GameObjectTeam.Chaos, 8, 115, 100, Utility.Map.MapType.SummonersRift,
                new Vector3(10570, 8150, 63), new Vector3(11233.96f, 8789.653f, 1051.235f),
                new[]
                {
                    GetJungleMobByName("SRU_Murkwolf", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_MurkwolfMini", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_MurkwolfMini", Utility.Map.MapType.SummonersRift)
                }));
            JungleCamps.Add(new JungleCamp("wraiths", GameObjectTeam.Chaos, 9, 115, 100,
                Utility.Map.MapType.SummonersRift, new Vector3(7465, 9220, 56), new Vector3(7962.764f, 10028.573f, 1023.06f),
                new[]
                {
                    GetJungleMobByName("SRU_Razorbeak", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_RazorbeakMini", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_RazorbeakMini", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_RazorbeakMini", Utility.Map.MapType.SummonersRift)
                }));
            JungleCamps.Add(new JungleCamp("red", GameObjectTeam.Chaos, 10, 115, 300, Utility.Map.MapType.SummonersRift,
                new Vector3(6620, 10637, 55), new Vector3(7164.198f, 11113.5f, 1093.54f),
                new[]
                {
                    GetJungleMobByName("SRU_Red", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_RedMini", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_RedMini", Utility.Map.MapType.SummonersRift)
                }));
            JungleCamps.Add(new JungleCamp("golems", GameObjectTeam.Chaos, 11, 115, 100,
                Utility.Map.MapType.SummonersRift, new Vector3(6010, 11920, 40), new Vector3(6508.562f, 12127.83f, 1185.667f),
                new[]
                {
                    GetJungleMobByName("SRU_Krug", Utility.Map.MapType.SummonersRift),
                    GetJungleMobByName("SRU_KrugMini", Utility.Map.MapType.SummonersRift)
                }));
            JungleCamps.Add(new JungleCamp("wight", GameObjectTeam.Chaos, 14, 115, 100, Utility.Map.MapType.SummonersRift,
                new Vector3(12266, 6215, 54), new Vector3(12671.58f, 6617.756f, 1118.074f),
                new[] { GetJungleMobByName("SRU_Gromp", Utility.Map.MapType.SummonersRift) }));
            JungleCamps.Add(new JungleCamp("crab", GameObjectTeam.Neutral, 15, 2 * 60 + 30, 180, Utility.Map.MapType.SummonersRift,
                new Vector3(12266, 6215, 54), new Vector3(10557.22f, 5481.414f, 1068.042f),
                new[] { GetJungleMobByName("SRU_Crab", Utility.Map.MapType.SummonersRift) }));
            JungleCamps.Add(new JungleCamp("crab", GameObjectTeam.Neutral, 16, 2 * 60 + 30, 180, Utility.Map.MapType.SummonersRift,
                new Vector3(12266, 6215, 54), new Vector3(4535.956f, 10104.067f, 1029.071f),
                new[] { GetJungleMobByName("SRU_Crab", Utility.Map.MapType.SummonersRift) }));
            JungleCamps.Add(new JungleCamp("dragon", GameObjectTeam.Neutral, 6, 2 * 60 + 30, 360,
                Utility.Map.MapType.SummonersRift, new Vector3(9400, 4130, -61), new Vector3(10109.18f, 4850.93f, 1032.274f),
                new[] { GetJungleMobByName("Dragon", Utility.Map.MapType.SummonersRift) }));
            JungleCamps.Add(new JungleCamp("nashor", GameObjectTeam.Neutral, 12, 20 * 60, 420,
                Utility.Map.MapType.SummonersRift, new Vector3(4620, 10265, -63), new Vector3(4951.034f, 10831.035f, 1027.482f),
                new[] { GetJungleMobByName("Worm", Utility.Map.MapType.SummonersRift) }));
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

        private static void UpdateCamps(int networkId, int campId, byte emptyType)
        {
            if (emptyType != 3)
            {
                JungleCamp jungleCamp = GetJungleCampById(campId, GMap._MapType);
                if (jungleCamp != null)
                {
                    jungleCamp.NextRespawnTime = (int)Game.ClockTime + jungleCamp.RespawnTime;
                }
            }
        }

        private static void EmptyCamp(BinaryReader b)
        {
            byte[] h = b.ReadBytes(4);
            int nwId = BitConverter.ToInt32(h, 0);
            h = b.ReadBytes(4);
            int cId = BitConverter.ToInt32(h, 0);
            byte emptyType = b.ReadByte();
            UpdateCamps(nwId, cId, emptyType);
        }

        private static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            if (!IsActive())
                return;
            try
            {
                var stream = new MemoryStream(args.PacketData);
                using (var b = new BinaryReader(stream))
                {
                    int pos = 0;
                    var length = (int)b.BaseStream.Length;
                    while (pos < length)
                    {
                        int v = b.ReadInt32();
                        if (v == 195)
                        {
                            byte[] h = b.ReadBytes(1);
                            EmptyCamp(b);
                        }
                        pos += sizeof(int);
                    }
                }
            }
            catch (EndOfStreamException)
            {
            }
        }

        public class JungleCamp
        {
            public bool Called;
            public int CampId;
            public JungleMob[] Creeps;
            public Vector3 MapPosition;
            public Utility.Map.MapType MapType;
            public Vector3 MinimapPosition;
            public String Name;
            public int NextRespawnTime;
            public int RespawnTime;
            public int SpawnTime;
            public GameObjectTeam Team;

            public JungleCamp(String name, GameObjectTeam team, int campId, int spawnTime, int respawnTime,
                Utility.Map.MapType mapType, Vector3 mapPosition, Vector3 minimapPosition, JungleMob[] creeps)
            {
                Name = name;
                Team = team;
                CampId = campId;
                SpawnTime = spawnTime;
                RespawnTime = respawnTime;
                MapType = mapType;
                MapPosition = mapPosition;
                MinimapPosition = minimapPosition;
                Creeps = creeps;
                NextRespawnTime = 0;
                Called = false;
            }
        }

        public class JungleMob
        {
            public bool Boss;
            public bool Buff;
            public Utility.Map.MapType MapType;
            public String Name;
            public Obj_AI_Minion Obj;
            public bool Smite;

            public JungleMob(string name, Obj_AI_Minion obj, bool smite, bool buff, bool boss,
                Utility.Map.MapType mapType)
            {
                Name = name;
                Obj = obj;
                Smite = smite;
                Buff = buff;
                Boss = boss;
                MapType = mapType;
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