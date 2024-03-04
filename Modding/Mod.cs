namespace Block2D.Modding
{
    public class Mod : IModContentLoader
    {
        public string Name { get; set; }
        public string Version { get; set; }

        public bool Loaded { get; set; }

        public Mod(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public void LoadContent()
        {

        }

        public void UnloadContent()
        {
            
        }
    }
}
