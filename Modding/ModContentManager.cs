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
        private readonly Mod _mod;
        public ModContent Content { get; private set; }

        public ModContentManager(Mod mod)
        {
            _mod = mod;
            _tileLoader = new(_mod);
            _soundEffectLoader = new(_mod);
            _textureLoader = new(_mod);
        }

        public void LoadContent(ModContent content)
        {
            if (_textureLoader.TryLoadTextures(out ModTexture[] textures))
            {
                foreach (ModTexture texture in textures)
                {
                    Content.Textures.Add(texture.Name, texture);
                    content.Textures.Add(texture.Name, texture);
                }
            }

            if (_soundEffectLoader.TryLoadSoundEffects(out ModSoundEffect[] soundEffects))
            {
                foreach (ModSoundEffect soundEffect in soundEffects)
                {
                    Content.SoundEffects.Add(soundEffect.Name, soundEffect);
                    content.SoundEffects.Add(soundEffect.Name, soundEffect);
                }
            }

            if (_tileLoader.TryLoadTiles(out ModTile[] tiles))
            {
                foreach (ModTile tile in tiles)
                {
                    Content.Tiles.Add(tile.Name, tile);
                    content.Tiles.Add(tile.Name, tile);
                }
            }
        }

        public ModTile[] GetModTiles()
        {
            return Content.Tiles.Values.ToArray();
        }

        public ModTile GetTile(string name)
        {
            if (!Content.Tiles.ContainsKey(name))
            {
                Main.Logger.Warn("Tried To Get Mod Tile That Doesn't Exist.");
                return new();
            }
            return Content.Tiles[name];
        }

        public bool TryGetTexture(string name, out ModTexture texture)
        {
            if (Content.Textures.ContainsKey(name))
            {
                texture = Content.Textures[name];
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
            if (!Content.Textures.ContainsKey(name))
            {
                Main.Logger.Warn("Tried To Get Mod Texture That Doesn't Exist.");
                return new();
            }
            return Content.Textures[name];
        }

        public ModSoundEffect GetSoundEffect(string name)
        {
            if (!Content.SoundEffects.ContainsKey(name))
            {
                Main.Logger.Warn("Tried To Get Mod Sound Effect That Doesn't Exist.");
                return new();
            }
            return Content.SoundEffects[name];
        }

        public void UnloadContent() { }
    }
}
