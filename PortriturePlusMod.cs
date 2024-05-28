using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
namespace PortraiturePlus
{
	/// <summary>The mod entry point.</summary>
	internal sealed class PortriturePlusMod : Mod
	{
		private static readonly IDictionary<string, string> festivalDates = Game1.content.Load<Dictionary<string, string>>(@"Data\Festivals\FestivalDates", LocalizedContentManager.LanguageCode.en);
		
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="help">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper help)
		{
			festivalInit();
			harmonyFix();
		}

		private void harmonyFix()
		{
			PortraitFix.Initialize(monitor: Monitor);
			var harmony = new Harmony(ModManifest.UniqueID);
			harmony.Patch(original: PortraitFix.TargetMethod(), prefix: new HarmonyMethod(AccessTools.Method(typeof(PortraitFix), nameof(PortraitFix.getPortrait_Prefix))));
		}

		internal static string GetDayEvent()
		{
			if (SaveGame.loaded?.weddingToday ?? Game1.weddingToday)
				return "Wedding";

			var festival = festivalDates.TryGetValue($"{Game1.currentSeason}{Game1.dayOfMonth}", out var festivalName) ? festivalName : "";
			return festival;
		}

		private static void festivalInit()
		{
			foreach (var key in festivalDates.Keys)
			{
				if (festivalDates[key].Contains(' '))
				{
					festivalDates[key] = festivalDates[key].Replace(" ", "");
				}
				if (festivalDates[key].Contains('\''))
				{
					festivalDates[key] = festivalDates[key].Replace("'", "");
				}
				festivalDates[key] = festivalDates[key] switch
				{
					"EggFestival" => "EggF",
					"DanceoftheMoonlightJellies" => "Jellies",
					"StardewValleyFair" => "Fair",
					"FestivalofIce" => "Ice",
					"FeastoftheWinterStar" => "WinterStar",
					_ => festivalDates[key]
				};
			}
		}
	}
}
