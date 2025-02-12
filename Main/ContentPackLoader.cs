using Microsoft.Xna.Framework.Graphics;
using Portraiture;
using StardewModdingAPI;

namespace PortraiturePlus.Main;

internal static class ContentPackLoader
{
    private static IModHelper _helper = null!;

    internal static void AddContentPackTextures(List<string> folders, Dictionary<string, Texture2D> pTextures)
    {
        var contentPacks = _helper.ContentPacks.GetOwned();
        foreach (var pack in contentPacks)
        {
            var folderName = pack.Manifest.UniqueID;
            var folderPath = pack.DirectoryPath;

            folders.Add(folderName);
            foreach (var file in Directory.EnumerateFiles(pack.DirectoryPath, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".xnb")))
            {
                var fileName = file.Replace(folderPath + "\\", "");
                var name = Path.GetFileNameWithoutExtension(file);
                var extension = Path.GetExtension(file).ToLower();
                if (extension == "xnb")
                    fileName = name;
                var texture = pack.ModContent.Load<Texture2D>(fileName);
                var tileWith = Math.Max(texture.Width / 2, 64);
                var scale = tileWith / 64f;
                var scaled = new ScaledTexture2D(texture, scale);
                if (!pTextures.ContainsKey(folderName + ">" + name))
                    pTextures.Add(folderName + ">" + name, scaled);
                else
                    pTextures[folderName + ">" + name] = scaled;
            }
        }
    }

    public static void Init(IModHelper help)
    {
        _helper = help;
    }
}