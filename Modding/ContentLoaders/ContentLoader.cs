using Block2D.Common;
using System.IO;

namespace Block2D.Modding.ContentLoaders
{
    public abstract class ContentLoader
    {
        protected Mod Mod { get; set; }
        protected string FilesPath { get; set; }
        protected const string LUA_FILE_EXTENSION = ".lua";

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
    }
}
