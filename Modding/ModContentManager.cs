using Block2D.Modding.ContentLoaders;
using Block2D.Modding.DataStructures;
using System.Linq;

namespace Block2D.Modding
{
    /// <summary>
    /// A class that stores the content of a mod
    /// </summary>
    public class ModContentManager
    {
        private readonly TileLoader _tileLoader;
        private readonly SoundEffectLoader _soundEffectLoader;
        private readonly TextureLoader _textureLoader;
        private ModContent _content;

        public ModContentManager(Mod mod)
        {
            _content = new();
            _tileLoader = new(mod);
            _soundEffectLoader = new(mod);
            _textureLoader = new(mod);
        }

        public void LoadContent(ModContent content)
        {
            if (_textureLoader.TryLoadTextures(out ModTexture[] textures))
            {
                foreach (ModTexture texture in textures)
                {
                    _content.Textures.Add(texture.Name, texture);
                    content.Textures.Add(texture.Name, texture);
                }
            }

            if (_soundEffectLoader.TryLoadSoundEffects(out ModSoundEffect[] soundEffects))
            {
                foreach (ModSoundEffect soundEffect in soundEffects)
                {
                    _content.SoundEffects.Add(soundEffect.Name, soundEffect);
                    content.SoundEffects.Add(soundEffect.Name, soundEffect);
                }
            }

            if (_tileLoader.TryLoadTiles(out ModTile[] tiles))
            {
                foreach (ModTile tile in tiles)
                {
                    _content.Tiles.Add(tile.Name, tile);
                    content.Tiles.Add(tile.Name, tile);
                }
            }
        }

        public ModTile[] GetModTiles()
        {
            return _content.Tiles.Values.ToArray();
        }

        public void UnloadContent() { }
    }
}
