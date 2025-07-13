using Microsoft.Xna.Framework.Graphics;
using Portraiture;
using Portraiture.HDP;
using StardewModdingAPI;
using StardewValley;

namespace PortraiturePlus.Main;

internal class PortraitManager
{
	internal static Dictionary<string, Texture2D> PTextures = new();
	
    public static Texture2D? GetPortrait(NPC npc, Texture2D tex, List<string> folders, PresetCollection presets,
		int activeFolder, Dictionary<string, Texture2D> pTextures)
	{
		var name = npc.Name;

		if (!Context.IsWorldReady || folders.Count == 0)
			return null;

		activeFolder = Math.Max(activeFolder, 0);

		if (presets.Presets.FirstOrDefault(pr => pr.Character == name) is { } pre)
			activeFolder = Math.Max(folders.IndexOf(pre.Portraits), 0);

		var folder = folders[activeFolder];

		if (activeFolder == 0 || folders.Count <= activeFolder || folder == "none" || folder == "HDP" && PortraitureMod.helper.ModRegistry.IsLoaded("tlitookilakin.HDPortraits"))
			return null;

		if (folder == "HDP" && !PortraitureMod.helper.ModRegistry.IsLoaded("tlitookilakin.HDPortraits"))
			return GetHdpPortrait(tex, name);
			
		var season = Game1.currentSeason ?? "spring";
		var npcDictionary = pTextures.Keys
			.Where(key => key.Contains(name) && key.Contains(folder))
			.ToDictionary(k => k.ToLowerInvariant(), l => pTextures[l]);
		var dayOfMonth = Game1.dayOfMonth.ToString();
		var festival = GetDayEvent();
		var gl = Game1.currentLocation.Name ?? "";
		var isOutdoors = Game1.currentLocation.IsOutdoors ? "Outdoor" : "Indoor";
		var week = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" }[
				Game1.dayOfMonth % 7];
		var raining = Game1.isRaining ? "Rain" : "";
		var year = Game1.year.ToString();
		name = folder + ">" + name;
		var eventID = "N/A";
		if (Game1.CurrentEvent is not null && Game1.CurrentEvent.id is not null)
			eventID = "event" + Game1.CurrentEvent.id;

		var queryScenarios = new List<string[]>
		{
			new[] {name, eventID},
			new[] {name, festival, year},
			new[] {name, festival},
			new[] {name, gl, season, year, dayOfMonth}, new[] {name, gl, season, year, week},
			new[] {name, gl, season, dayOfMonth}, new[] {name, gl, season, week},
			new[] {name, gl, season},
			new[] {name, gl, dayOfMonth}, new[] {name, gl, week},
			new[] {name, gl},
			new[] {name, season, raining},
			new[] {name, season, isOutdoors},
			new[] {name, season, year, dayOfMonth}, new[] {name, season, year, week},
			new[] {name, season, dayOfMonth}, new[] {name, season, week},
			new[] {name, season},
			new[] {name, raining},
			new[] {name}
		};

		foreach (var result in queryScenarios.Select(args => GetTexture2D(npcDictionary, args)).OfType<Texture2D>())
		{
			return result;
		}

		return pTextures.ContainsKey(folder + ">" + name) ? pTextures[folder + ">" + name] : null;
	}

	private static Texture2D? GetTexture2D(Dictionary<string, Texture2D> npcDictionary, params string[] values)
	{
		var key = values.Aggregate((current, next) => current + "_" + next).ToLowerInvariant().TrimEnd('_');
		return values.Any(text => text == "") ? null : npcDictionary!.GetValueOrDefault(key, null);
	}
	
	private static string GetDayEvent()
	{
		if (SaveGame.loaded?.weddingToday ?? Game1.weddingToday || Game1.CurrentEvent != null && Game1.CurrentEvent.isWedding)
			return "Wedding";

		if (Game1.CurrentEvent != null && Game1.CurrentEvent.isFestival) {
			var festival = FestivalHelper.FestivalDates.TryGetValue($"{Game1.currentSeason}{Game1.dayOfMonth}", out
				var festivalName) ? festivalName : "";
			return festival;
		}

		return "";
	}
	
	private static Texture2D? GetHdpPortrait(Texture2D tex, string name)
	{
		try
		{
			var portraits = PortraitureMod.helper.GameContent.Load<MetadataModel>("Mods/HDPortraits/" + name);
			switch (portraits)
			{
				case null:
					return null;
				case var _ when portraits.TryGetTexture(out var texture):
				{
					if (portraits.Animation == null || portraits.Animation.VFrames == 1 && portraits.Animation.HFrames == 1)
						return ScaledTexture2D.FromTexture(tex, texture, portraits.Size / 64f);
					portraits.Animation.Reset();
					return new AnimatedTexture2D(texture, texture.Width / portraits.Animation.VFrames, texture.Height / portraits.Animation.HFrames, 6, true, portraits.Size / 64f);
				}
				default:
					return null;
			}
		}
		catch
		{
			return null;
		}
	}
}
