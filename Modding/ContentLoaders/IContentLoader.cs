namespace Block2D.Modding.ContentLoaders
{
    public interface IContentLoader
    {
        public Mod Mod { get; set; }
        public string FilesPath { get; set; }
        public bool ModContains();
        public string[] GetFilePaths();
    }
}
