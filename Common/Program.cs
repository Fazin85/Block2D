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
            if (!Directory.Exists(Common.Main.ModsDirectory))
            {
                Directory.CreateDirectory(Common.Main.ModsDirectory);
            }
            if (!Directory.Exists(Common.Main.ModsDirectory + "/Base"))
            {
                Helper.CopyDirectory("Content/Base", Common.Main.ModsDirectory + "/Base", true);
            }
            if (!Directory.Exists(Common.Main.WorldsDirectory))
            {
                Directory.CreateDirectory(Common.Main.WorldsDirectory);
            }
        }
    }
}
