using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using Portraiture;
using PortraiturePlus.Main;
using StardewValley;
using StardewValley.GameData.Shops;

namespace PortraiturePlus.Patches;

[SuppressMessage("ReSharper", "InconsistentNaming")]
internal class ShopMenuPatch
{
    internal static MethodInfo TryLoadPortrait() => AccessTools.Method("ShopMenu:TryLoadPortrait", new[]
    {
        typeof(ShopOwnerData), typeof(NPC)
    });

    internal static bool TryLoadPortrait_Prefix(ShopOwnerData ownerData, NPC owner, ref Texture2D? __result)
    {
        var folders = Traverse.Create(typeof(PortraitureMod).Assembly.GetType("Portraiture.TextureLoader")).Field<List<string>>("folders").Value;
        var activeFolder = Traverse.Create(typeof(PortraitureMod).Assembly.GetType("Portraiture.TextureLoader")).Field<int>("activeFolder").Value;
        var pTextures = PortraitManager.PTextures
            .Where(x => x.Key.Contains("_Shop") && x.Key.Contains(folders[activeFolder]))
            .ToDictionary(x => x.Key, x => x.Value);
        
        var matchingKey = pTextures.Keys.FirstOrDefault(k => k.Contains(ownerData.Name))
                          ?? pTextures.Keys.FirstOrDefault(k => k.Contains(owner.Name));

        if (matchingKey == null) return true;
        __result = pTextures[matchingKey];
        return false;
    }
}