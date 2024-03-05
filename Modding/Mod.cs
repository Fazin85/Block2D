using System;

namespace Block2D.Modding
{
    public class Mod
    {
        public string DisplayName { get; private set; }
        public string InternalName { get; private set; }
        public string Version { get; private set; }
        public bool Loaded { get; set; }
        public Version GameVersion { get; private set; }
        public ModContentManager ContentManager { get; private set; }

        public Mod(string displayName, string internalName, string version)
        {
            DisplayName = displayName;
            InternalName = internalName;
            Version = version;
            ContentManager = new(this);
            GameVersion = new();
        }

        public void LoadContent()
        {
            ContentManager.LoadContent();
            Loaded = true;
        }

        public void UnloadContent() { }
    }
}
