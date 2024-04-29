using System.Collections.Generic;

namespace Block2D.Modding.DataStructures
{
    public struct ModContent
    {
        public Dictionary<string, ModTile> Tiles { get; set; }
        public Dictionary<string, ModSoundEffect> SoundEffects { get; set; }
        public Dictionary<string, ModTexture> Textures { get; set; }

        public ModContent()
        {
            Tiles = [];
            SoundEffects = [];
            Textures = [];
        }

        public readonly void Clear()
        {
            Tiles.Clear();
            SoundEffects.Clear();
            Textures.Clear();
        }
    }
}
