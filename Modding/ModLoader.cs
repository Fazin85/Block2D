using System.Collections.Generic;
using System.IO;
using Block2D.Common;
using Block2D.Modding.DataStructures;
using MoonSharp.Interpreter;

namespace Block2D.Modding
{
    public abstract class ModLoader
    {
        private readonly ModManager _modManager;
        private bool _forceQuit;

        protected ModContent LoadedContent;

        public List<Mod> LoadedMods { get; private set; }

        public ModLoader()
        {
            _modManager = new();
            _forceQuit = false;
            LoadedContent = new();
            LoadedMods = new();
        }

        protected void LoadAllMods()
        {
            if (_forceQuit)
            {
                Main.Logger.Fatal("(MODLOADER): Terminating Modloader.");
                return;
            }

            string[] modDirectories = Directory.GetDirectories(Main.ModsDirectory);

            if (modDirectories.Length == 0)
            {
                Main.Logger.Info("(MODLOADER): No Mods Detected, Skipping Mod Loading Process.");
                return;
            }

            for (int i = 0; i < modDirectories.Length; i++)
            {
                if (_forceQuit)
                {
                    Main.Logger.Fatal("(MODLOADER): Terminating Modloader.");
                    return;
                }

                LoadMod(modDirectories[i]);
            }
        }

        private void LoadMod(string modDirectoryPath)
        {
            if (_forceQuit)
            {
                Main.Logger.Fatal("(MODLOADER): Terminating Modloader.");
                return;
            }

            DirectoryInfo modDirectory = new(modDirectoryPath);

            if (!File.Exists(modDirectory.FullName + "/Mod.lua"))
            {
                return;
            }

            Script script = new();
            Main.SetupScript(script);
            script.DoFile(modDirectory.FullName + "/Mod.lua");

            DynValue modNameFromFileVal = script.Call(script.Globals["GetModName"]);

            string modNameFromFile = modNameFromFileVal.String;

            DynValue modVersionFromFileVal = script.Call(script.Globals["GetVersion"]);

            string modVersion = modVersionFromFileVal.String;

            if (modNameFromFile.Length == 0 || modVersion.Length == 0)
            {
                Main.Logger.Warn("(MODLOADER): Tried To Load A Corrupted Mod.");
                return;
            }

            Mod mod = new(modNameFromFile, modDirectory.Name, modVersion);

            if (script.Globals["Init"] != null)
            {
                //run init code for mod
                script.Call(script.Globals["Init"]);
            }

            mod.LoadContent(LoadedContent);
            _modManager.AddMod(mod);
            LoadedMods.Add(mod);
            Main.Logger.Info("(MODLOADER): Loaded Mod: " + mod.DisplayName);
        }

        public void UnloadMod(string modName)
        {
            _modManager.UnloadAndRemoveMod(modName);

            bool flag = false;

            foreach (Mod mod in LoadedMods)
            {
                if (mod.DisplayName == modName)
                {
                    LoadedMods.Remove(mod);
                    flag = true;
                }
            }

            if (!flag)
            {
                Main.Logger.Fatal("(MODLOADER): Failed To Unload Mod: " + modName);
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
            Main.Logger.Fatal("(MODLOADER): Force Quit Modloader Due To Broken Mod.");
        }
    }
}
