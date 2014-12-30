using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
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

        private static string GetLastVersion(string assemblyName)
        {
            var request = WebRequest.Create(String.Format("https://raw.githubusercontent.com/kaigan05/LeagueSharp/master/{0}/Properties/AssemblyInfo.cs", assemblyName));
            var response = request.GetResponse();
            var data = response.GetResponseStream();
            string version;
            using (var sr = new StreamReader(data))
            {
                version = sr.ReadToEnd();
            }
            const string pattern = @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}";
            return new Regex(pattern).Match(version).Groups[0].Value;
        }
        public static bool HasNewVersion(string assemblyName)
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString() != GetLastVersion(assemblyName);
        }
    }
}