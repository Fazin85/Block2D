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
            string programAppDataDirectory = Common.Main.AppDataDirectory + "/" + Common.Main.GameName;
            if (!Directory.Exists(programAppDataDirectory))//create "Block2D" directory in %AppData% for logging and mods and stuff.
            {
                Directory.CreateDirectory(programAppDataDirectory);
            }
            if (!Directory.Exists(programAppDataDirectory + "/Mods"))//create "Mods" directory.
            {
                Directory.CreateDirectory(programAppDataDirectory + "/Mods");
            }

        }
    }
}
