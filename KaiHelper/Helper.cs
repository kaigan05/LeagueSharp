using System;
using System.IO;
using System.Net;
using SharpDX;
using SharpDX.Direct3D9;

namespace KaiHelper
{
    internal class Helper
    {
        public static void DrawText(Font font, String text, int posX, int posY, Color color)
        {
            Rectangle rec = font.MeasureText(null, text, FontDrawFlags.Center);
            font.DrawText(null, text, posX + 1 + rec.X, posY + 1, Color.Black);
            font.DrawText(null, text, posX + rec.X, posY + 1, Color.Black);
            font.DrawText(null, text, posX - 1 + rec.X, posY - 1, Color.Black);
            font.DrawText(null, text, posX + rec.X, posY - 1, Color.Black);
            font.DrawText(null, text, posX + rec.X, posY, color);
        }

        public static string FormatTime(double time)
        {
            var t = TimeSpan.FromSeconds(time);
            return string.Format("{0:D1}:{1:D2}", t.Minutes, t.Seconds);
        }

        public static Stream Download(string url)
        {
            WebRequest req = WebRequest.Create(url);
            WebResponse response = req.GetResponse();
            return response.GetResponseStream();
        }

        public static string ReadFile(string path)
        {
            using (var sr=new StreamReader(path))
            {
                return sr.ReadToEnd();
            }
        }

        public static string ReadFileFromUrl(string url)
        {
            using (var client = new WebClient())
            {
                using (var stream = client.OpenRead(url))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
        public static bool IsNewVersion(string newVersion)
        {
            string curVersion=ReadFile(Path.Combine(LeagueSharpFolder.MainFolder, "version.txt"));
            var o = curVersion.Split('.');
            var n=newVersion.Split('.');
            for (int i = n.Length - 1;i >= 0;  i--)
            {
                if (Convert.ToInt32(n[i]) > Convert.ToInt32(o[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}