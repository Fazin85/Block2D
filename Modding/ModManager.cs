using System.Collections.Generic;
using System.Linq;

namespace Block2D.Modding
{
    /// <summary>
    /// A class that stores all loaded mods
    /// </summary>
    public class ModManager
    {
        private readonly Dictionary<string, Mod> _mods;

        public ModManager()
        {
            _mods = [];
        }

        public bool TryGetMod(string modName, out Mod mod)
        {
            mod = null;
            if (!_mods.ContainsKey(modName))
            {
                ModLoader.LogWarning("Tried To Get A Mod That Doesn't Exist.");
                return false;
            }
            mod = _mods[modName];
            return true;
        }

        public void AddMod(Mod mod)
        {
            if (!mod.Loaded)
            {
                ModLoader.LogWarning("Tried To Add An Unloaded Mod.");
                return;
            }
            _mods.Add(mod.DisplayName, mod);
        }

        public void UnloadAndRemoveMod(string modName)
        {
            if (!_mods.ContainsKey(modName))
            {
                ModLoader.LogWarning("Tried To Remove A Mod That Doesn't Exist.");
                return;
            }

            _mods[modName].UnloadContent();
            _mods.Remove(modName);
            ModLoader.LogInfo("Removed Mod: " + modName);
        }

        public void UnloadAndRemoveAllMods()
        {
            if (_mods.Count == 0)
            {
                ModLoader.LogWarning("Tried To Unload Mods, But None Are Loaded.");
                return;
            }

            string[] modNames = _mods.Keys.ToArray();
            foreach (string modName in modNames)
            {
                UnloadAndRemoveMod(modName);
            }
        }
    }
}
