using Block2D.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Steamworks;
using System.Collections.Generic;

namespace Block2D.Client.UI
{
    public class PlayerListUI
    {
        private readonly Dictionary<string, PlayerListEntry> _playerList;
        private readonly ClientAssetManager _assetManager;
        private readonly Client _client;

        public PlayerListUI(Client client)
        {
            _playerList = [];
            _assetManager = client.AssetManager;
            _client = client;
        }

        public void AddPlayer(PlayerListEntry playerEntry)
        {
            _playerList.Add(playerEntry.Name, playerEntry);
        }

        public void RemovePlayer(string playerName)
        {
            _playerList.Remove(playerName);
        }

        public void RemovePlayer(ushort id)
        {
            _playerList.Remove(_client.CurrentWorld.Players[id].Name);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPos, int windowWidth, KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Tab))
            {
                const int fontSize = 22;
                int yOffset = 0;
                Vector2 iconOffset = new(64, -4);
                Vector2 xOffset = new(-32, 0);

                foreach (var item in _playerList.Values)
                {
                    if (item.Texture == null)
                    {
                        break;
                    }

                    Vector2 position = new(windowWidth, yOffset);

                    spriteBatch.Draw(item.Texture, cameraPos + position - iconOffset, null, Color.White, 0, Vector2.Zero, 0.35f, SpriteEffects.None, 0);//draw icon
                    spriteBatch.DrawString(_assetManager.Font, item.Name, cameraPos + position + xOffset + new Vector2(2, 0), Color.White);//draw username
                    spriteBatch.DrawString(_assetManager.Font, item.Ping.ToString(), cameraPos + position + xOffset + new Vector2(fontSize * item.Name.Length / 2, 0), Color.White);//draw ping

                    yOffset += 32;
                }
            }
        }

        public void Update()
        {
            ClientPlayer[] players = [.. _client.CurrentWorld.Players.Values];

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
