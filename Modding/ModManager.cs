using System.Collections.Generic;
using Block2D.Common;

namespace Block2D.Modding
{
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
            _mods.Add(mod.Name, mod);
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
        }
    }
}
