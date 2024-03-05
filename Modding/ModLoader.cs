using Block2D.Common;
using System.IO;

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

            DirectoryInfo[] directoryInfo = new DirectoryInfo[modDirectories.Length];
            for (int i = 0; i < directoryInfo.Length; i++)
            {
                directoryInfo[i] = new(modDirectories[i]);
                LoadMod(directoryInfo[i]);
            }
        }

        public void LoadMod(DirectoryInfo modDirectory)
        {
            if (!File.Exists(modDirectory.FullName + "/ModInfo.txt"))
            {
                return;
            }

            FileInfo fileInfo = new(modDirectory.FullName + "/ModInfo.txt");
            StreamReader reader = fileInfo.OpenText();
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
