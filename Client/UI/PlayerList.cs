using Block2D.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System.Collections.Generic;

namespace Block2D.Client.UI
{
    public struct PlayerListEntry
    {
        public short Ping;
        public ulong SteamID;
        public string Name;
        public Texture2D Texture;
    }

    public class PlayerList
    {
        private readonly Dictionary<string, PlayerListEntry> _playerList;
        private readonly ClientAssetManager _assetManager;

        public PlayerList(ClientAssetManager assetManager)
        {
            _playerList = [];
            _assetManager = assetManager;
        }

        public void Draw(SpriteBatch spriteBatch, int windowWidth)
        {
            int yOffset = 0;

            foreach (var item in _playerList.Values)
            {
                spriteBatch.Draw(item.Texture, new Vector2(windowWidth, yOffset), Color.White);
            }
        }

        public void Update(Dictionary<string, PlayerListEntry> players)
        {
            foreach (var player in players)
            {
                string name = player.Key;
                bool flag = false;

                if (!_playerList.TryGetValue(player.Key, out PlayerListEntry entry))
                {
                    _playerList.Add(player.Key, entry);
                    flag = true;
                }

                if (!flag)
                {
                    _playerList[player.Key] = entry;
                }

                if (_playerList[player.Key].Texture == null)
                {
                    LoadPlayerProfilePicture(name, player.Value.SteamID);
                }
            }
        }

        private void LoadPlayerProfilePicture(string playerName, ulong steamID)
        {
            CSteamID id = new(steamID);
            int iconInt = SteamFriends.GetMediumFriendAvatar(id);

            Texture2D tex = default;
            bool isValid = SteamUtils.GetImageSize(iconInt, out uint imageWidth, out uint imageHeight);

            if (isValid)
            {
                byte[] imageData = new byte[imageWidth * imageHeight * 4];

                isValid = SteamUtils.GetImageRGBA(iconInt, imageData, (int)(imageWidth * imageHeight * 4));

                if (isValid)
                {
                    tex = new(Main.GraphicsDevice, (int)imageWidth, (int)imageHeight);
                    tex.SetData(imageData);
                }
                else
                {
                    tex = _assetManager.AnonymousUserProfile;
                }
            }
            else
            {
                tex = _assetManager.AnonymousUserProfile;
            }

            PlayerListEntry e = _playerList[playerName];
            e.Texture = tex;
            _playerList[playerName] = e;
        }
    }
}
