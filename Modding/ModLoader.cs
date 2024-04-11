using Block2D.Common;
using Block2D.Modding.DataStructures;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.IO;

namespace Block2D.Modding
{
    public abstract class ModLoader
    {
        private readonly ModManager _modManager;
        private bool _forceQuit;
        private readonly ModloaderLogger _logger;
        private static ModLoader _instance;

        protected ModContent LoadedContent;

        public List<Mod> LoadedMods { get; private set; }

        protected ModLoader()
        {
            _instance = this;
            _modManager = new();
            _logger = new();
            _forceQuit = false;
            LoadedContent = new();
            LoadedMods = new();
        }

        protected void LoadAllMods()
        {
            if (_forceQuit)
            {
                _instance._logger.LogInfo("Terminating Modloader.");
                return;
            }

            string[] modDirectories = Directory.GetDirectories(Main.ModsDirectory);

            if (modDirectories.Length == 0)
            {
                _instance._logger.LogInfo("No Mods Detected, Skipping Mod Loading Process.");
                return;
            }

            for (int i = 0; i < modDirectories.Length; i++)
            {
                if (_forceQuit)
                {
                    _instance._logger.LogFatal("Terminating Modloader.");
                    return;
                }

                LoadMod(modDirectories[i]);
            }
        }

        private void LoadMod(string modDirectoryPath)
        {
            if (_forceQuit)
            {
                _instance._logger.LogFatal("Terminating Modloader.");
                return;
            }

            DirectoryInfo modDirectory = new(modDirectoryPath);

            if (!File.Exists(modDirectory.FullName + "/Mod.lua"))
            {
                return;
            }

            Script script = new();
            Main.SetupScript(script, null, false);
            script.DoFile(modDirectory.FullName + "/Mod.lua");

            DynValue modNameFromFileVal = script.Call(script.Globals["GetModName"]);

            string modNameFromFile = modNameFromFileVal.String;

            DynValue modVersionFromFileVal = script.Call(script.Globals["GetVersion"]);

            string modVersion = modVersionFromFileVal.String;

            if (modNameFromFile.Length == 0 || modVersion.Length == 0)
            {
                _instance._logger.LogWarning("Tried To Load A Corrupted Mod.");
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
            _instance._logger.LogInfo("Loaded Mod: " + mod.DisplayName);
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
                _instance._logger.LogFatal("Failed To Unload Mod: " + modName);
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
            _instance._logger.LogFatal("Force Quit Modloader Due To Broken Mod.");
        }

        public static void LogInfo(string message)
        {
            _instance._logger.LogInfo(message);
        }

        public static void LogWarning(string message)
        {
            _instance._logger.LogWarning(message);
        }

        public static void LogWarning(Exception exception)
        {
            _instance._logger.LogWarning(exception.Message);
        }

        public static void LogFatal(string message)
        {
            _instance._logger.LogFatal(message);
        }
    }
}
