using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using Portraiture;
using StardewModdingAPI;
using StardewValley;
using System.Reflection;
namespace PortraiturePlus
{
	internal class PortraiturePlusFix
	{
		private static IMonitor _monitor = null!;
		
		internal static void Initialize(IMonitor monitor)
		{
			_monitor = monitor;
		}
		internal static MethodInfo GetPortrait()
		{
			return AccessTools.Method("TextureLoader:getPortrait", new[]
			{
				typeof(NPC), typeof(Texture2D)
			});
		}
		
		internal static MethodInfo LoadAllPortraits()
		{
			return AccessTools.Method("TextureLoader:loadAllPortraits");
		}

		internal static void LoadAllPortraits_Postfix()
		{
			var folders = Traverse.Create(typeof(PortraitureMod).Assembly.GetType("Portraiture.TextureLoader")).Field<List<string>>("folders").Value;
			var pTextures = Traverse.Create(typeof(PortraitureMod).Assembly.GetType("Portraiture.TextureLoader")).Field<Dictionary<string, Texture2D>>("pTextures").Value;
			PortraiturePlusMod.AddContentPackTextures(folders, pTextures);
		}

		[SuppressMessage("ReSharper", "InconsistentNaming")]
		internal static bool GetPortrait_Prefix(NPC npc, Texture2D tex, ref Texture2D? __result)
		{
			var folders = Traverse.Create(typeof(PortraitureMod).Assembly.GetType("Portraiture.TextureLoader")).Field<List<string>>("folders").Value;
			var presets = Traverse.Create(typeof(PortraitureMod).Assembly.GetType("Portraiture.TextureLoader")).Field<PresetCollection>("presets").Value;
			var activeFolder = Traverse.Create(typeof(PortraitureMod).Assembly.GetType("Portraiture.TextureLoader")).Field<int>("activeFolder").Value;
			var pTextures = Traverse.Create(typeof(PortraitureMod).Assembly.GetType("Portraiture.TextureLoader")).Field<Dictionary<string, Texture2D>>("pTextures").Value;
			if (folders is { Count: <= 0 })
				return true;
			try
			{
				__result = PortraiturePlusMod.GetPortrait(npc, tex, folders, presets, activeFolder, pTextures);
				return false;
			}
			catch (Exception ex)
			{
				_monitor.Log($"Failed in {nameof(GetPortrait_Prefix)}:\n{ex}", LogLevel.Error);
				return true;
			}
		}
	}
}
