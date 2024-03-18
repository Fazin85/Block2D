using System.IO;

namespace Block2D.Common
{
    public class Program
    {
        static void Main(string[] args)
        {
            CreateNeccesaryFiles();
            using Main game = new();
            game.Run();
        }

        private static void CreateNeccesaryFiles()
        {
            string programAppDataDirectory = Common.Main.GameAppDataDirectory;
            if (!Directory.Exists(programAppDataDirectory))//create "Block2D" directory in %AppData% for logging and mods and stuff.
            {
                Directory.CreateDirectory(programAppDataDirectory);
            }
            if (!Directory.Exists(Common.Main.ModsDirectory))//create "Mods" directory.
            {
                Directory.CreateDirectory(Common.Main.ModsDirectory);
            }
            if (!Directory.Exists(programAppDataDirectory + "/Mods/Base"))
            {
                Helper.CopyDirectory("Content/Base", programAppDataDirectory + "/Mods/Base", true);
            }
            if (!Directory.Exists(Common.Main.WorldsDirectory))//create "Worlds" directory.
            {
                Directory.CreateDirectory(Common.Main.WorldsDirectory);
            }
        }
    }
}
