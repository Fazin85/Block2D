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

        public ModContent LoadedContent { get; private set; }

        public int LoadedModCount
        {
            get => LoadedMods.Count;
        }

        public List<Mod> LoadedMods { get; private set; }

        public ModTexture GetTexture(string name)
        {
            return LoadedContent.Textures[name];
        }

        public ModLoader()
        {
            _modManager = new();
            _forceQuit = false;
            LoadedContent = new();
            LoadedMods = new();
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
            mod.LoadContent(LoadedContent);
            _modManager.AddMod(mod);
            LoadedMods.Add(mod);
            reader.Dispose();
            Main.Logger.Info("Loaded Mod: " + mod.DisplayName);
        }

        public void UnloadMod(string modName)
        {
            _modManager.UnloadAndRemoveMod(modName);

            bool flag = false;

            for (int i = 0; i < LoadedMods.Count; i++)
            {
                Mod mod = LoadedMods[i];
                if (mod.DisplayName == modName)
                {
                    LoadedMods.RemoveAt(i);
                    flag = true;
                }
            }

            if (!flag)
            {
                Main.Logger.Fatal("Failed To Unload Mod: " + modName);
                Main.ShouldExit = true;
            }
        }

        public void UnloadAllMods()
        {
            _modManager.UnloadAndRemoveAllMods();
            LoadedMods.Clear();
        }

        public void ForceQuit()
        {
            _forceQuit = true;
            UnloadAllMods();
            Main.Logger.Fatal("Force Quit Modloader Due To Broken Mod.");
        }
    }
}
