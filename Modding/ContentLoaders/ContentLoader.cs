using Block2D.Common;
using MoonSharp.Interpreter;
using System.IO;

namespace Block2D.Modding.ContentLoaders
{
    public abstract class ContentLoader
    {
        protected Mod Mod { get; private set; }
        protected string FilesPath { get; set; }
        protected const string LUA_FILE_EXTENSION = ".lua";
        protected Script _script;

        public ContentLoader(Mod mod)
        {
            Mod = mod;
        }
        /// <summary>
        /// Set The Content Loader's Active Script
        /// </summary>
        protected void SetScript(Script script)
        {
            _script = script;
        }

        protected bool ModContains()
        {
            if (Directory.Exists(FilesPath))
            {
                string[] tileFiles = Directory.GetFiles(FilesPath);
                return tileFiles.Length > 0;
            }
            return false;
        }

        protected string[] GetFilePaths()
        {
            string[] files = Directory.GetFiles(FilesPath);
            if (files.Length == 0)
            {
                Main.Logger.Warn("There Are No Files To Get!");
                return null;
            }
            return files;
        }

        protected DynValue GetGlobal(string name)
        {
            return _script.Call(_script.Globals[name]);
        }
    }
}
