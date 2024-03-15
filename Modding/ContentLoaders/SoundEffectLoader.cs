using Block2D.Common;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace Block2D.Modding.ContentLoaders
{
    public class SoundEffectLoader : ContentLoader
    {
        public SoundEffectLoader(Mod mod)
        {
            Mod = mod;
            FilesPath = Main.ModsDirectory + "/" + Mod.InternalName + "/SoundEffects";
        }

        public bool TryLoadSoundEffects(out ModSoundEffect[] soundEffects)
        {
            soundEffects = null;
            if (!ModContains())
            {
                return false;
            }

            string[] soundFilePaths = GetFilePaths();
            soundEffects = new ModSoundEffect[soundFilePaths.Length];

            for (int i = 0; i < soundFilePaths.Length; i++)
            {
                if (!soundFilePaths[i].Contains(".txt"))
                {
                    return false;
                }

                StreamReader sr = new(soundFilePaths[i]);
                string name = sr.ReadLine();
                string soundFilePath = sr.ReadLine();

                if (name.Length == 0 || soundFilePath.Length == 0)
                {
                    Main.Logger.Fatal(
                        "Sound Effect Name Length Or FilePath Length Was Zero.\nBroken Sound Effect Path: "
                            + soundFilePaths[i]
                    );
                    Main.ForceQuitModloader();
                    return false;
                }
                sr.Dispose();
                soundEffects[i].Name = name;
                soundEffects[i].SoundEffect = SoundEffect.FromFile(FilesPath + soundFilePath);
            }
            return true;
        }
    }
}
