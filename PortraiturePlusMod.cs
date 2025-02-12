using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using PortraiturePlus.Main;
using PortraiturePlus.Patches;
using StardewModdingAPI;

namespace PortraiturePlus;

[SuppressMessage("ReSharper", "UnusedType.Global")]
internal sealed class PortraiturePlusMod : Mod
{
	public override void Entry(IModHelper? help)
	{
		FestivalHelper.Init();
		ContentPackLoader.Init(help!);
		PortraiturePatch.Init(monitor: Monitor);
		var harmony = new Harmony(ModManifest.UniqueID);
		harmony.Patch(original: ShopMenuPatch.TryLoadPortrait(), prefix: new HarmonyMethod(AccessTools.Method(typeof(ShopMenuPatch), nameof(ShopMenuPatch.TryLoadPortrait_Prefix))));
		harmony.Patch(original: PortraiturePatch.GetPortrait(), prefix: new HarmonyMethod(AccessTools.Method(typeof(PortraiturePatch), nameof(PortraiturePatch.GetPortrait_Prefix))));
		harmony.Patch(original: PortraiturePatch.LoadAllPortraits(), postfix: new HarmonyMethod(AccessTools.Method(typeof(PortraiturePatch), nameof(PortraiturePatch.LoadAllPortraits_Postfix))));
	}
}