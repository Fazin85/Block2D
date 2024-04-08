using System.Collections.Generic;
using System.Linq;
using Block2D.Modding;

namespace Block2D.Common
{
    public abstract class World
    {
        public Dictionary<string, ushort> LoadedTiles;
        protected ushort NextTileIdToLoad;

        protected void LoadTiles()
        {
            foreach (Mod currentMod in Main.AssetManager.LoadedMods)
            {
                var tiles = currentMod.ContentManager.GetModTiles();

                for (int i = 0; i < tiles.Length; i++)
                {
                    LoadedTiles.Add(tiles[i].Name, NextTileIdToLoad);
                    NextTileIdToLoad++;
                }
            }
        }

        protected string GEtTileName(ushort id)
        {
            var reversed = LoadedTiles.ToDictionary(x => x.Value, x => x.Key);
            return reversed[id];
        }
    }
}
