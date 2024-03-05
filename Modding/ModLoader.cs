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
            foreach (string modFile in modDirectories)
            {
                LoadMod(modFile);
            }
        }

        public void LoadMod(string modDirectory)
        {
            if (!File.Exists(modDirectory + "/ModInfo.txt"))
            {
                return;
            }

            FileInfo fileInfo = new(modDirectory + "/ModInfo.txt");
            StreamReader reader = fileInfo.OpenText();
            string modNameFromFile = reader.ReadLine();
            string modVersion = reader.ReadLine();

            if (modNameFromFile.Length == 0 || modVersion.Length == 0)
            {
                Main.Logger.Warn("Tried To Load A Corrupted Mod!");
                return;
            }

            Mod mod = new(modNameFromFile, modVersion);
            mod.LoadContent();
            _modManager.AddMod(mod);
            reader.Dispose();
            Main.Logger.Info("Loaded Mod: " + mod.Name);
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
