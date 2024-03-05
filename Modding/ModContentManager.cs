using Block2D.Common;
using Block2D.Modding.ContentLoaders;
using System.Collections.Generic;

namespace Block2D.Modding
{
    /// <summary>
    /// A class that stores the content of a mod
    /// </summary>
    public class ModContentManager
    {
        private readonly TileLoader _tileLoader;
        public Dictionary<string, ModTile> ModTiles { get; set; }
        private readonly SoundEffectLoader _soundEffectLoader;
        public Dictionary<string, CustomSoundEffect> SoundEffects { get; set; }

        private readonly Mod _mod;

        public ModContentManager(Mod mod)
        {
            ModTiles = new();
            _mod = mod;
            _tileLoader = new(_mod);
            _soundEffectLoader = new(_mod);
        }

        public void LoadContent()
        {
            if (_soundEffectLoader.TryLoadSoundEffects(out CustomSoundEffect[] soundEffects))
            {
                foreach (CustomSoundEffect soundEffect in soundEffects)
                {
                    SoundEffects.Add(soundEffect.Name, soundEffect);
                }
            }
            if (_tileLoader.TryLoadTiles(out ModTile[] tiles))
            {
                foreach (ModTile tile in tiles)
                {
                    ModTiles.Add(tile.Name, tile);
                }
            }
        }
    }
}
