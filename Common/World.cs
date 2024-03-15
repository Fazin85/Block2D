using Block2D.Modding.DataStructures;

namespace Block2D.Common
{
    public abstract class World
    {
        public abstract void LoadAllTiles();
        protected abstract void LoadDefaultTiles();
        protected abstract void LoadModTiles(ModTile[] modTiles);
    }
}
