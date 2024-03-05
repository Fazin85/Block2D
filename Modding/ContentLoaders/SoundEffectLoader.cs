using System.IO;
using Block2D.Common;

namespace Block2D.Modding.ContentLoaders
{
    public class SoundEffectLoader : IContentLoader
    {
        public Mod Mod { get; set; }
        public string FilesPath { get; set; }

        public SoundEffectLoader(Mod mod)
        {
            Mod = mod;
            FilesPath = Main.ModsDirectory + Mod.DisplayName + "SoundEffects";
        }

        public string[] GetFilePaths()
        {
            string[] files = Directory.GetFiles(FilesPath);
            if (files.Length == 0)
            {
                Main.Logger.Warn("There Are No Files To Get!");
                return null;
            }
            return files;
        }

        public bool ModContains()
        {
            if (Directory.Exists(FilesPath))
            {
                string[] tileFiles = Directory.GetFiles(FilesPath);
                return tileFiles.Length > 0;
            }
            return false;
        }

        public bool TryLoadSoundEffects(out CustomSoundEffect[] soundEffects)
        {
            soundEffects = null;
            if (!ModContains())
            {
                return false;
            }

            string[] soundFilePaths = GetFilePaths();
            soundEffects = new CustomSoundEffect[soundFilePaths.Length];

            for (int i = 0; i < soundFilePaths.Length; i++)
            {
                StreamReader sr = new(soundFilePaths[i]);
                soundEffects[i].Name = sr.ReadLine();
                soundEffects[i].Path = soundFilePaths[i];
                if (float.TryParse(sr.ReadLine(), out float result))
                {
                    soundEffects[i].Volume = result;
                }
                else
                {
                    Main.Logger.Warn("Tried To Load Sound Effect With Invalid Volume!");
                }
            }
            return true;
        }
    }
}
