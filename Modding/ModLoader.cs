using Block2D.Common;
using Block2D.Modding.DataStructures;
using System.Collections.Generic;
using System.IO;

namespace Block2D.Modding
{
    public class ModLoader
    {
        private readonly ModManager _modManager;
        private bool _forceQuit;

        /// <summary>
        /// Searches through all loaded mods for ModTexture with name <paramref name="name"/>
        /// </summary>
        public ModTexture? GetTexture(string name)
        {
            
        }


        public ModLoader()
        {
            _modManager = new();
            _forceQuit = false;
        }

        public void LoadAllMods()
        {
            if (_forceQuit)
            {
                Main.Logger.Fatal("Terminating Modloader.");
                return;
            }

            string[] modDirectories = Directory.GetDirectories(Main.ModsDirectory);

            if (modDirectories.Length == 0)
            {
                Main.Logger.Info("No Mods Detected, Skipping Mod Loading Process.");
                return;
            }

            for (int i = 0; i < modDirectories.Length; i++)
            {
                if (_forceQuit)
                {
                    Main.Logger.Fatal("Terminating Modloader.");
                    return;
                }

                LoadMod(modDirectories[i]);
            }
        }


        public void LoadMod(string modDirectoryPath)
        {
            if (_forceQuit)
            {
                Main.Logger.Fatal("Terminating Modloader.");
                return;
            }

            DirectoryInfo modDirectory = new(modDirectoryPath);

            if (!File.Exists(modDirectory.FullName + "/ModInfo.txt"))
            {
                return;
            }

            StreamReader reader = new(modDirectory.FullName + "/ModInfo.txt");
            string modNameFromFile = reader.ReadLine();
            string modVersion = reader.ReadLine();

            if (modNameFromFile.Length == 0 || modVersion.Length == 0)
            {
                Main.Logger.Warn("Tried To Load A Corrupted Mod.");
                return;
            }

            Mod mod = new(modNameFromFile, modDirectory.Name, modVersion);
            mod.LoadContent();
            _modManager.AddMod(mod);
            reader.Dispose();
            Main.Logger.Info("Loaded Mod: " + mod.DisplayName);
        }

        public Dictionary<string, Mod> GetAllMods()
        {
            return _modManager.GetAllMods();
        }

        public void UnloadMod(string modName)
        {
            _modManager.UnloadAndRemoveMod(modName);
        }

        public void UnloadAllMods()
        {
            _modManager.UnloadAndRemoveAllMods();
        }

        public void ForceQuit()
        {
            _forceQuit = true;
            UnloadAllMods();
            Main.Logger.Fatal("Force Quit Modloader Due To Broken Mod.");
        }
    }
}
