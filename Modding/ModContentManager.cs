using Block2D.Modding.ContentLoaders;
using Block2D.Modding.DataStructures;
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
        public Dictionary<string, ModSoundEffect> SoundEffects { get; set; }
        private readonly TextureLoader _textureLoader;
        public Dictionary<string, ModTexture> Textures { get; set; }

        private readonly Mod _mod;

        public ModContentManager(Mod mod)
        {
            ModTiles = new();
            SoundEffects = new();
            Textures = new();
            _mod = mod;
            _tileLoader = new(_mod);
            _soundEffectLoader = new(_mod);
            _textureLoader = new(_mod);
        }

        public void LoadContent()
        {
            if (_textureLoader.TryLoadTextures(out ModTexture[] textures))
            {
                foreach (ModTexture texture in textures)
                {
                    Textures.Add(texture.Name, texture);
                }
            }

            if (_soundEffectLoader.TryLoadSoundEffects(out ModSoundEffect[] soundEffects))
            {
                foreach (ModSoundEffect soundEffect in soundEffects)
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

        public void UnloadContent() { }
    }
}
