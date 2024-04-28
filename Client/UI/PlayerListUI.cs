using Block2D.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System.Collections.Generic;

namespace Block2D.Client.UI
{
    public class PlayerListUI
    {
        private readonly Dictionary<string, PlayerListEntry> _playerList;
        private readonly ClientAssetManager _assetManager;
        private readonly ClientLogger _logger;

        public PlayerListUI(ClientAssetManager assetManager, ClientLogger logger)
        {
            _playerList = [];
            _assetManager = assetManager;
            _logger = logger;
        }

        public void AddPlayer(PlayerListEntry playerEntry)
        {
            _playerList.Add(playerEntry.Name, playerEntry);
        }

        public void RemovePlayer(string playerName)
        {
            _playerList.Remove(playerName);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPos, int windowWidth)
        {
            int yOffset = 0;

            foreach (var item in _playerList.Values)
            {
                if (item.Texture == null)
                {
                    break;
                }

                spriteBatch.Draw(item.Texture, cameraPos, Color.White);
            }
        }

        public void Update(ClientPlayer[] players)
        {
            for (int i = 0; i < players.Length; i++)
            {
                PlayerListEntry e = _playerList[players[i].Name];

                e.Ping = players[i].Ping;
                e.Texture ??= LoadPlayerProfilePicture(players[i].SteamID);

                _playerList[players[i].Name] = e;
            }
        }

        private Texture2D LoadPlayerProfilePicture(ulong steamID)
        {
            Texture2D tex = _assetManager.AnonymousUserProfile;

            if (!Main.OfflineMode)
            {
                CSteamID id = new(steamID);
                int iconInt = SteamFriends.GetMediumFriendAvatar(id);
                bool isValid = SteamUtils.GetImageSize(iconInt, out uint imageWidth, out uint imageHeight);

                if (isValid && steamID != 0)
                {
                    byte[] imageData = new byte[imageWidth * imageHeight * 4];

                    isValid = SteamUtils.GetImageRGBA(iconInt, imageData, (int)(imageWidth * imageHeight * 4));

                    if (isValid)
                    {
                        tex = new(Main.GraphicsDevice, (int)imageWidth, (int)imageHeight);
                        tex.SetData(imageData);
                    }
                }
            }

            return tex;
        }
    }
}
