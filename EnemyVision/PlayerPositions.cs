using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace EnemyVision
{
    class PlayerPositions
    {
        readonly Dictionary<Vector2, List<Vector3>> _playerPositionList = new Dictionary<Vector2, List<Vector3>>();
        private const double TamNhin = 1300;

        public PlayerPositions()
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        void Game_OnGameLoad(EventArgs args)
        {
            Console.WriteLine("Starting Calculate!");
            for (short x = 10; x <= 282; x++)
            {
                for (short y = 10; y <= 282; y++)
                {
                    var v2 = new Vector2(x, y);
                    var curPos = NavMesh.GridToWorld(x, y);
                    var listPoint = new List<Vector3>();
                    for (int i = 0; i <= 360; i++)
                    {
                        var vongngoai = new Vector3(
                            (float)(curPos.X + TamNhin * Math.Cos(i * Math.PI / 180)),
                            (float)(curPos.Y + TamNhin * Math.Sin(i * Math.PI / 180)),
                            ObjectManager.Player.Position.Z);
                        for (int j = 0; j < TamNhin; j += 100)
                        {
                            var vongtrong = new Vector3(
                                (float)(curPos.X + j * Math.Cos(i * Math.PI / 180)),
                                (float)(curPos.Y + j * Math.Sin(i * Math.PI / 180)),
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
                    _playerPositionList.Add(v2, listPoint);
                }
            }
            var serializer = new XmlSerializer(typeof(SerializableDictionary<string, List<string>>));
            TextWriter textWriter = new StreamWriter(@"DictionarySerialized.xml");
            serializer.Serialize(textWriter, _playerPositionList);
            textWriter.Close();
            Console.WriteLine("Saved!");
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
    }
}
