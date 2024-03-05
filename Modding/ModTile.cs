namespace Block2D.Modding
{
    public class ModTile
    {
        public string Name { get; set; }
        public string TexturePath { get; set; }
        public float TextureScale { get; set; }
        public string HitSoundEffectPath { get; set; }

        public ModTile(
            string name,
            string texturePath,
            float textureScale,
            string hitSoundEffectPath
        )
        {
            Name = name;
            TexturePath = texturePath;
            TextureScale = textureScale;
            HitSoundEffectPath = hitSoundEffectPath;
        }
    }
}
