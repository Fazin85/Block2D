using Block2D.Common;
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

        public ModTile GetTile(string name)
        {
            if (!_content.Tiles.ContainsKey(name))
            {
                Main.Logger.Warn("Tried To Get Mod Tile That Doesn't Exist.");
                return new();
            }
            return _content.Tiles[name];
        }

        public bool TryGetTexture(string name, out ModTexture texture)
        {
            if (_content.Textures.ContainsKey(name))
            {
                texture = _content.Textures[name];
                return true;
            }
            else
            {
                texture = default;
                return false;
            }
        }

        public ModTexture GetTexture(string name)
        {
            if (!_content.Textures.ContainsKey(name))
            {
                Main.Logger.Warn("Tried To Get Mod Texture That Doesn't Exist.");
                return new();
            }
            return _content.Textures[name];
        }

        public ModSoundEffect GetSoundEffect(string name)
        {
            if (!_content.SoundEffects.ContainsKey(name))
            {
                Main.Logger.Warn("Tried To Get Mod Sound Effect That Doesn't Exist.");
                return new();
            }
            return _content.SoundEffects[name];
        }

        public void UnloadContent() { }
    }
}
