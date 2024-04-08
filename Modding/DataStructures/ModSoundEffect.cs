using Microsoft.Xna.Framework.Audio;

namespace Block2D.Modding.DataStructures
{
    public readonly struct ModSoundEffect
    {
        public readonly string Name
        {
            get => _name;
        }
        public readonly SoundEffect SoundEffect
        {
            get => _soundEffect;
        }

        private readonly string _name;
        private readonly SoundEffect _soundEffect;

        public ModSoundEffect(string name, SoundEffect soundEffect)
        {
            _name = name;
            _soundEffect = soundEffect;
        }
    }
}
