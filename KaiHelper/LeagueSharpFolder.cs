using LeagueSharp.Common;

namespace KaiHelper
{
    public static class LeagueSharpFolder
    {
        public static string MainFolder { get { return Config.LeagueSharpDirectory; } }

        public static string SummonerSpellFolder(string fileName)
        {
            return string.Format(@"{0}\Images\AlternateSS\{1}.png", MainFolder, fileName);
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
