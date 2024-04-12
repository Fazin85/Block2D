using Block2D.Client;
using Block2D.Modding;
using Block2D.Server;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Block2D.Common
{
    public abstract class WorldData
    {
        public Dictionary<string, ushort> LoadedTiles { get; private set; }
        public string Name { get; set; }
        protected ushort NextTileIdToLoad;
        protected Block2DLogger Logger;
        private readonly AssetManager _assetManager;

        protected WorldData(AssetManager assetManager, ProgramSide side)
        {
            _assetManager = assetManager;

            if (side == ProgramSide.Client)
            {
                Logger = new ClientLogger();
            }
            else
            {
                Logger = new ServerLogger();
            }

            LoadedTiles = [];
            NextTileIdToLoad = 0;
        }

        public void LoadTiles()
        {
            foreach (Mod currentMod in _assetManager.LoadedMods)
            {
                var tiles = currentMod.ContentManager.GetModTiles();

                for (int i = 0; i < tiles.Length; i++)
                {
                    LoadedTiles.Add(tiles[i].Name, NextTileIdToLoad);
                    NextTileIdToLoad++;
                }
            }
        }

        public string GetTileName(ushort id)
        {
            var reversed = LoadedTiles.ToDictionary(x => x.Value, x => x.Key);
            return reversed[id];
        }

        public void SetTile(string dimensionId, Point worldPosition, string id)
        {
            Point chunkPosition = worldPosition.ToChunkCoords();
            if (GetChunkLoaded(dimensionId, chunkPosition, out ServerChunk chunk))
            {
                int x = Math.Abs(chunkPosition.X - worldPosition.X);
                int y = Math.Abs(chunkPosition.Y - worldPosition.Y);

                chunk.SetTile(new(x, y), id);
            }
            else
            {
                Logger.LogWarning("Tried To Set Tile In A Chunk That Doesn't Exist.");
            }
        }

        public bool TryGetTile(string dimensionID, Point worldPosition, out ServerTile tile)
        {
            Point chunkPosition = worldPosition.ToChunkCoords();
            if (GetChunkLoaded(dimensionID, chunkPosition, out ServerChunk chunk))
            {
                int x = Math.Abs(chunkPosition.X - worldPosition.X);
                int y = Math.Abs(chunkPosition.Y - worldPosition.Y);

                tile = chunk.GetTile(new(x, y));
                return true;
            }
            tile = new();
            return false;
        }

        public abstract bool GetChunkLoaded(string dimensionID, Point chunkPosition, out ServerChunk chunk);
    }
}
