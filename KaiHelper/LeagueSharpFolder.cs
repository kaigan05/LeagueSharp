using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using LeagueSharp.Common;

namespace KaiHelper
{
    public static class LeagueSharpFolder
    {
        public static string MainFolder
        {
            get
            {
                var configFile = Path.Combine(Config.LeagueSharpDirectory, "config.xml");
                try
                {
                    if (File.Exists(configFile))
                    {
                        var config = new XmlDocument();
                        config.Load(configFile);
                        var node = config.DocumentElement.SelectSingleNode(
                            "/Config/SelectedProfile/InstalledAssemblies");
                        foreach (
                            var element in
                                node.ChildNodes.Cast<XmlElement>()
                                    .Where(
                                        element =>
                                            element.ChildNodes.Cast<XmlElement>()
                                                .Any(e => e.Name == "Name" && e.InnerText == "KaiHelper")))
                        {
                            return
                                Path.GetDirectoryName(
                                    element.ChildNodes.Cast<XmlElement>()
                                        .First(e => e.Name == "PathToProjectFile")
                                        .InnerText);
                        }
                    }
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee.ToString());
                }
                return null;
            }
        }

        public static string SummonerSpellFolder(string fileName = null)
        {
            return fileName == null
                ? string.Format(@"{0}\Images\AlternateSS\", MainFolder)
                : string.Format(@"{0}\Images\AlternateSS\{1}.png", MainFolder, fileName);
        }

        public static string SpellFolder(string fileName)
        {
            return string.Format(@"{0}\Images\SkillsSmall\{1}.png", MainFolder, fileName);
        }

        public static string MiniMapFolder(string fileName)
        {
            return string.Format(@"{0}\Images\Minimap\{1}.png", MainFolder, fileName);
        }

        public static string HudFolder(string fileName)
        {
            return string.Format(@"{0}\Images\HUD\{1}.png", MainFolder, fileName);
        }
    }
}