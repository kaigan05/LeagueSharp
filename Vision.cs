using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using Color = System.Drawing.Color;
using Packet = LeagueSharp.Network.Packets.Packet;

namespace EnemyVision
{
    internal class Vision
    {
        private readonly Menu _menu;
        readonly List<Vector2> listPoint = new List<Vector2>(360);
        public Vision()
        {
            _menu = new Menu("Enemy Vision", "Enemy vision",true);
            _menu.AddItem(new MenuItem("DoTron", "Roundness").SetValue(new Slider(11, 1, 20)));
            _menu.AddItem(new MenuItem("DoChinhXac", "Acttuacy").SetValue(new Slider(1, 1)));
            _menu.AddItem(new MenuItem("Thickness", "Thickness lines").SetValue(new Slider(1, 1)));
            _menu.AddItem(new MenuItem("TrenManHinh", "Only draw when enemys on screen").SetValue(false));
            _menu.AddItem(new MenuItem("VongTron", "Only Circle").SetValue(false));
            _menu.AddItem(new MenuItem("NguoiChoiTest", "Test by me").SetValue(false));
            _menu.AddItem(new MenuItem("Active", "Active").SetValue(true));
            _menu.AddToMainMenu();
            CustomEvents.Game.OnGameLoad += Game_Onload;
            Game.OnGameUpdate += Game_OnGameUpdate;
        }
        private void Game_Onload(EventArgs args)
        {
            Game.PrintChat("<font color = \"#00FF2B\">Enemy vision</font> by kaigan <font color = \"#FD00FF\">Loaded!</font>");
            //GetPlayerPositions();
            //using (StreamReader streamReader = new StreamReader(@"C:\DictionarySerialized.xml"))
            //{
            //    XmlSerializer xmlSerializer = new XmlSerializer(_playerPositionList.GetType());
            //    _playerPositionList = (SerializableDictionary<Vector2, List<Vector3>>)xmlSerializer.Deserialize(streamReader);
            //}

            Drawing.OnDraw += Game_OnDraw;
        }

        private float _nextTime;
        Obj_AI_Base result = new Obj_AI_Base();
        private int tamNhin;
        SerializableDictionary<Vector2, List<Vector3>> _playerPositionList = new SerializableDictionary<Vector2, List<Vector3>>();
        private const double TamNhin = 1300;

        public void GetPlayerPositions()
        {
            try
            {
                Console.WriteLine("Starting Calculate!");
                for (short x = 10; x <= 282; x++)
                {
                    for (short y = 10; y <= 282; y++)
                    {
                        var v2 = new Vector2(x, y);
                        var curPos = NavMesh.GridToWorld(x, y);
                        var list = new List<Vector3>();
                        for (int i = 0; i <= 360; i++)
                        {
                            var vongngoai = new Vector3(
                                (float)(curPos.X + TamNhin * Math.Cos(i * Math.PI / 180)),
                                (float)(curPos.Y + TamNhin * Math.Sin(i * Math.PI / 180)),
                                ObjectManager.Player.Position.Z);
                            for (int j = 0; j < TamNhin; j += 1)
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
                            list.Add(vongngoai);
                        }
                        _playerPositionList.Add(v2, list);
                    }
                }
                using (StreamWriter streamWriter = new StreamWriter(@"C:\DictionarySerialized.xml"))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(_playerPositionList.GetType());
                    xmlSerializer.Serialize(streamWriter, _playerPositionList);
                }
                //XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<Vector2, List<Vector3>>));
                //TextWriter textWriter = new StreamWriter(@"C:\DictionarySerialized.xml");
                //serializer.Serialize(textWriter, _playerPositionList);
                //textWriter.Close();
                Console.WriteLine("DONE!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (_menu.Item("NguoiChoiTest").GetValue<bool>())
            {
                result = ObjectManager.Player;
            }
            else
            {
                var posPlayer = ObjectManager.Player.Position.To2D();
                var dist = float.MaxValue;
                foreach (var objectEnemy in
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(o => ObjectManager.Player.Team != o.Team && o.IsVisible && !o.IsDead && o.IsValid))
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
            tamNhin = result is Obj_AI_Hero || result is Obj_AI_Turret ? 1300 : 1200;

            int doTron = 21 - (_menu.Item("DoTron").GetValue<Slider>().Value);
            int doChinhXac = 101 - (_menu.Item("DoChinhXac").GetValue<Slider>().Value);
            //int radius = 0;
            //for (; radius <= tamNhin; radius++)
            //{
            //    if (NavMesh.IsWallOfGrass(result.Position, radius))
            //        break;
            //}
            listPoint.Clear();
            for (int i = 0; i <= 360; i += doTron)
            {
                var cosX = Math.Cos(i * Math.PI / 180);
                var sinY = Math.Sin(i * Math.PI / 180);
                var vongngoai = new Vector3(
                    (float) (result.Position.X + tamNhin * cosX), (float) (result.Position.Y + tamNhin * sinY),
                    result.Position.Z);
                for (int j = 0; j < tamNhin; j += doChinhXac)
                {
                    var vongtrong = new Vector3(
                        (float) (result.Position.X + j * cosX), (float) (result.Position.Y + j * sinY),
                        result.Position.Z);
                    if (!LaVatCan(vongtrong))
                    {
                        continue;
                    }
                    vongngoai = vongtrong;
                    break;
                }
                listPoint.Add(Drawing.WorldToScreen(vongngoai));
            }
        }

        private void Game_OnDraw(EventArgs args)
        {
            if (!_menu.Item("Active").GetValue<bool>())
            {
                return;
            }
            //Vector3 vectorPlayer =ObjectManager.Player.Position;
            //Vector3 vectorCursor =Game.CursorPos;
            //var cosX = Math.Cos(90 * Math.PI / 180);
            //var sinY = Math.Sin(90 * Math.PI / 180);
            //var vongngoai = new Vector3(
            //            (float)(vectorPlayer.X + tamNhin * cosX),
            //            (float)(vectorPlayer.Y + tamNhin * sinY),
            //            vectorPlayer.Z);
            //Vector3 pos = Vector3.Subtract(vectorCursor,vectorPlayer );
            //Utility.DrawCircle(pos, 20, Color.Crimson);
            //var screenPlayerPos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            //var screenCursorPos = Drawing.WorldToScreen(Game.CursorPos);
            //Console.WriteLine(listPoint.Count);
            for (int i = 0; i < listPoint.Count - 1; i++)
            {

                //Vector2 v1 = Drawing.WorldToScreen(listPoint[i]);
                //Vector2 v2 = Drawing.WorldToScreen(listPoint[i + 1]);
                Drawing.DrawLine(listPoint[i], listPoint[i + 1], 1, Color.PaleVioletRed);
            }
            ////if (_menu.Item("VongTron").GetValue<bool>())
            ////{
            ////    var pos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            //////    var grid = NavMesh.WorldToGrid(Game.CursorPos.X, Game.CursorPos.Y);
            ////    var gridH = NavMesh.WorldToGrid(ObjectManager.Player.Position.X, ObjectManager.Player.Position.Y);
            ////    //var world = NavMesh.GridToWorld(2, 2);
            ////    Drawing.DrawText(pos.X, pos.Y, System.Drawing.Color.White, gridH.ToString());
            ////    //Drawing.DrawText(Drawing.WorldToScreen(Game.CursorPos).X + 20, Drawing.WorldToScreen(Game.CursorPos).Y, System.Drawing.Color.White, grid.ToString());
            ////    //Drawing.DrawText(Drawing.WorldToScreen(Game.CursorPos).X + 20, Drawing.WorldToScreen(Game.CursorPos).Y - 10, System.Drawing.Color.White, Game.CursorPos.ToString());
            ////    //Drawing.DrawText(Drawing.WorldToScreen(Game.CursorPos).X + 20, Drawing.WorldToScreen(Game.CursorPos).Y - 20, System.Drawing.Color.White, world.ToString());
            ////    //Utility.DrawCircle(result.Position, tamNhin, System.Drawing.Color.Crimson);
            ////    //return;
            ////}
            //var v2 = NavMesh.WorldToGrid(ObjectManager.Player.Position.X, ObjectManager.Player.Position.Y);
            ////if (_playerPositionList.ContainsKey(v2))
            ////{
            //List<Vector3> v3 = _playerPositionList[v2];
            //    for (int i = 0; i < _playerPositionList[v2].Count - 1; i++)
            //    {
            //        //Drawing.DrawLine(Drawing.WorldToScreen(listPoint[i]), Drawing.WorldToScreen(listPoint[i + 1]), _menu.Item("Thickness").GetValue<Slider>().Value, System.Drawing.Color.PaleVioletRed);
            //        Drawing.DrawLine(Drawing.WorldToScreen(v3[i]), Drawing.WorldToScreen(v3[i + 1]), 1, System.Drawing.Color.Crimson);
            //    }
            ////}
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
    }

}