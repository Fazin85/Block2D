using Block2D.Modding.ContentLoaders;
using System.Collections.Generic;

namespace Block2D.Modding
{
    /// <summary>
    /// A class that stores the content of a mod
    /// </summary>
    public class ModContentManager : IModContentLoader
    {
        private readonly TileLoader _tileLoader;
        public Dictionary<string, ModTile> ModTiles { get; set; }

        private readonly Mod _mod;

        public ModContentManager(Mod mod)
        {
            ModTiles = new();
            _mod = mod;
            _tileLoader = new(_mod);
        }

        public void LoadContent()
        {
            if (_tileLoader.TryLoadTiles(out ModTile[] tiles))
            {
                foreach (var tile in tiles)
                {
                    ModTiles.Add(tile.Name, tile);
                }
            }
        }

        public void UnloadContent()
        {
            ModTiles.Clear();
        }
    }
}
