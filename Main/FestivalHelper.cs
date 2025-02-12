using StardewValley;

namespace PortraiturePlus.Main;

internal class FestivalHelper
{
    internal static readonly IDictionary<string, string> FestivalDates = 
        Game1.content.Load<Dictionary<string, string>>(@"Data\Festivals\FestivalDates", LocalizedContentManager.LanguageCode.en);
    
    public static void Init()
    {
        foreach (var key in FestivalDates.Keys.ToList())
        {
            var value = FestivalDates[key]
                .Replace(" ", "")
                .Replace("'", "")
                .Replace("EggFestival", "EggF")
                .Replace("DanceoftheMoonlightJellies", "Jellies")
                .Replace("StardewValleyFair", "Fair")
                .Replace("FestivalofIce", "Ice")
                .Replace("FeastoftheWinterStar", "WinterStar");
            
            FestivalDates[key] = value;
        }
    }
}