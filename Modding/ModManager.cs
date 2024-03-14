using Block2D.Common;
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
            _mods = new();
        }

        public bool TryGetMod(string modName, out Mod mod)
        {
            mod = null;
            if (!_mods.ContainsKey(modName))
            {
                Main.Logger.Warn("Tried To Get A Mod That Doesn't Exist!");
                return false;
            }
            mod = _mods[modName];
            return true;
        }

        public void AddMod(Mod mod)
        {
            if (!mod.Loaded)
            {
                Main.Logger.Warn("Tried To Add An Unloaded Mod!");
                return;
            }
            _mods.Add(mod.DisplayName, mod);
        }

        public Dictionary<string, Mod> GetAllMods()
        {
            return _mods;
        }

        public void UnloadAndRemoveMod(string modName)
        {
            if (!_mods.ContainsKey(modName))
            {
                Main.Logger.Warn("Tried To Remove A Mod That Doesn't Exist!");
                return;
            }

            _mods[modName].UnloadContent();
            _mods.Remove(modName);
            Main.Logger.Info("Removed Mod: " + modName);
        }

        public void UnloadAndRemoveAllMods()
        {
            if (_mods.Count == 0)
            {
                Main.Logger.Warn("Tried To Unload Mods, But None Are Loaded!");
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
