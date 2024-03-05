using System;

namespace Block2D.Modding
{
    public class Mod : IModContentLoader
    {
        public string Name { get; private set; }
        public string Version { get; private set; }
        public bool Loaded { get; set; }
        public Version GameVersion { get; private set; }
        public ModContentManager ContentManager { get; private set; }

        public Mod(string name, string version)
        {
            Name = name;
            Version = version;
            ContentManager = new(this);
            GameVersion = new();
        }

        public void LoadContent()
        {
            ContentManager.LoadContent();
        }

        public void UnloadContent() { }
    }
}
