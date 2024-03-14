using Block2D.Modding.DataStructures;

namespace Block2D.Common
{
    public abstract class World
    {
        protected abstract void LoadAllTiles();
        protected abstract void LoadDefaultTiles();
        protected abstract void LoadModTiles(ModTile[] modTiles);
    }
}
