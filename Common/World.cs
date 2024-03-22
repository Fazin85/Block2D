using Block2D.Modding.DataStructures;
using System.Collections.Generic;
using System.Linq;

namespace Block2D.Common
{
    public abstract class World
    {
        public Dictionary<string, ushort> LoadedTiles;
        protected abstract void LoadAllTiles();
        protected abstract void LoadModTiles(ModTile[] modTiles);

        public string GetTileName(ushort id)
        {
            var reversed = LoadedTiles.ToDictionary(x => x.Value, x => x.Key);
            return reversed[id];
        }
    }
}
