using Block2D.Common;
using Block2D.Server;
using Microsoft.Xna.Framework;

namespace Block2D.Modding
{
    public class ModWorld
    {
        public string Name
        {
            get => Main.InternalServer.World.Name;
        }

        private readonly Mod _mod;

        public ModWorld(Mod mod)
        {
            _mod = mod;
        }

        public bool TryGetTile(int x, int y, string dimensionId, out ServerTile tile)
        {
            Point position = new Point(x, y).ToChunkCoords();

            if (Main.InternalServer.World.TryGetTile(dimensionId, new(x, y), out tile))
            {
                return true;
            }
            else
            {
                _mod.Logger.LogFatal(
                    "Tried To Get Tile In A Chunk That Doesn't Exist. " + position.ToString()
                );
                return false;
            }
        }

        public void SetTile(string dimensionId, int x, int y, string id)
        {
            Main.InternalServer.World.SetTile(dimensionId, new(x, y), id);
        }
    }
}
