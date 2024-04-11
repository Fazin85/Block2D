using Block2D.Modding.DataStructures;

namespace Block2D.Modding
{
    public class Mod
    {
        public static readonly Mod Base;
        public string DisplayName { get; private set; }
        public string InternalName { get; private set; }
        public string Version { get; private set; }
        public bool Loaded { get; set; }
        public ModLogger Logger { get; private set; }
        public ModContentManager ContentManager { get; private set; }

        public Mod(string displayName, string internalName, string version)
        {
            DisplayName = displayName;
            InternalName = internalName;
            Version = version;
            ContentManager = new(this);
            Logger = new(DisplayName);
        }

        static Mod()
        {
            Base ??= new("base", "base", "1.0");

            Base.Logger ??= new("BASE");
        }

        public void LoadContent(ModContent content)
        {
            ContentManager.LoadContent(content);
            Loaded = true;
        }

        public void UnloadContent()
        {
            ContentManager.UnloadContent();
            Loaded = false;
        }
    }
}
