﻿using System.IO;
using Block2D.Common;
using Microsoft.Xna.Framework.Audio;

namespace Block2D.Modding.ContentLoaders
{
    public class SoundEffectLoader : ContentLoader
    {
        public SoundEffectLoader(Mod mod)
        {
            Mod = mod;
            FilesPath = Main.ModsDirectory + Mod.DisplayName + "SoundEffects";
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
