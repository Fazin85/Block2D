using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Block2D.Client
{
    public class ClientSettings
    {
        public const int SCREEN_WIDTH = 800;
        
        public int ResoloutionX { get;set; }
        public int ResoloutionY { get; set; }
        public List<string> EnabledMods { get; private set; }
        public bool FullScreen { get; set; }

        public ClientSettings()
        {
            ResoloutionX = 800;
            ResoloutionY = 480;
            FullScreen = false;
        }

        public void Load()
        {
            if (File.Exists("ClientSettings.json"))
            {
                ClientSettings settings = JsonSerializer.Deserialize<ClientSettings>(File.ReadAllText("ClientSettings.json"));
                ResoloutionX = settings.ResoloutionX;
                ResoloutionY = settings.ResoloutionY;
                EnabledMods = settings.EnabledMods;
                FullScreen = settings.FullScreen;
            }
            else
            {
                Save();
            }
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(this);

            File.WriteAllText("ClientSettings.json", json);
        }
    }
}
