using System.IO;
using Block2D.Common;

namespace Block2D.Modding
{
    public class ModLoader
    {
        private readonly ModManager _modManager;

        public ModLoader()
        {
            _modManager = new();
        }

        public void LoadAllMods()
        {
            string[] modDirectories = Directory.GetDirectories(Main.ModsDirectory);

            if (modDirectories.Length == 0)
            {
                Main.Logger.Info("No Mods Detected, Skipping Mod Loading Process.");
                return;
            }

            for (int i = 0; i < modDirectories.Length; i++)
            {
                LoadMod(modDirectories[i]);
            }
        }

        public void LoadMod(string modDirectoryPath)
        {
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
                Main.Logger.Warn("Tried To Load A Corrupted Mod!");
                return;
            }

            Mod mod = new(modNameFromFile, modDirectory.Name, modVersion);
            mod.LoadContent();
            _modManager.AddMod(mod);
            reader.Dispose();
            Main.Logger.Info("Loaded Mod: " + mod.DisplayName);
        }

        public void UnloadMod(string modName)
        {
            _modManager.UnloadAndRemoveMod(modName);
        }

        public void UnloadAllMods()
        {
            _modManager.UnloadAndRemoveAllMods();
        }
    }
}
