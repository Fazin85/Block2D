using Block2D.Common;

namespace Block2D.Server
{
    public class ServerAssetManager : AssetManager
    {
        public ServerAssetManager() : base(ProgramSide.Server)
        {
        }
    }
}
