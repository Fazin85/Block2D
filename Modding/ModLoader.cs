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

        public static string[] GetModNames() =>
            Directory.GetFiles(Main.AppDataDirectory + Main.GameName);

        public void LoadMod(string modDirectory)
        {
            if (!File.Exists(modDirectory + "/Mod.txt"))
            {
                return;
            }

            FileInfo fileInfo = new(modDirectory + "/Mod.txt");
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
        }

        public void UnloadMod(string modName)
        {
            _modManager.UnloadAndRemoveMod(modName);
        }
    }
}
