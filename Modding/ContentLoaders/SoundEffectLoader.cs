using Block2D.Common;
using Block2D.Modding.DataStructures;
using Microsoft.Xna.Framework.Audio;
using MoonSharp.Interpreter;
using System.Collections.Generic;

namespace Block2D.Modding.ContentLoaders
{
    public class SoundEffectLoader : ContentLoader
    {
        public SoundEffectLoader(Mod mod) : base(mod)
        {
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
            List<ModSoundEffect> modSoundEffectsList = [];

            for (int i = 0; i < soundFilePaths.Length; i++)
            {
                if (!soundFilePaths[i].Contains(LUA_FILE_EXTENSION))
                {
                    return false;
                }

                Script script = new();
                Main.SetupScript(script, Mod, true, ProgramSide.Server);

                script.DoFile(soundFilePaths[i]);

                DynValue nameVal = script.Call(script.Globals["GetName"]);

                string name = nameVal.String;

                DynValue pathVal = script.Call(script.Globals["GetPath"]);

                string soundFilePath = pathVal.String;

                if (name.Length == 0 || soundFilePath.Length == 0)
                {
                    ModLoader.LogFatal(
                        "Sound Effect Name Length Or FilePath Length Was Zero.\nBroken Sound Effect Path: "
                            + soundFilePaths[i]
                    );
                    //Main.ForceQuitModloader();
                    Main.ShouldExit = true;
                    return false;
                }

                SoundEffect soundEffect = SoundEffect.FromFile(FilesPath + soundFilePath);

                ModSoundEffect modSoundEffect = new(name, soundEffect);

                modSoundEffectsList.Add(modSoundEffect);
            }

            soundEffects = modSoundEffectsList.ToArray();

            return true;
        }
    }
}
