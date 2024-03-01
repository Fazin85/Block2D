using Block2D.Server.WorldGen;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Block2D.Server
{
    public class ServerWorld : ITickable
    {
        public bool IsLoaded { get; }

        public Chunk[] Chunks
        {
            get => _chunks.Values.ToArray();
        }

        public TerrainGenerator TerrainGenerator
        {
            get => _terrainGenerator;
        }

        private readonly Dictionary<Vector2, Chunk> _chunks;
        private readonly List<ServerPlayer> _players;
        private readonly TerrainGenerator _terrainGenerator;
        private int _tickCounter;
        private int _seed;

        public ServerWorld()
        {
            _chunks = new();
            _players = new();
            _terrainGenerator = new(_seed);
            _tickCounter = 0;
        }

        public void Tick()
        {
            _tickCounter++;
        }

        public void AddPlayer(ServerPlayer player)
        {
            _players.Add(player);
            Main.Logger.Info("Added Player" + player.Name);
        }

        public bool RemovePlayer(ushort id)
        {
            try
            {
                for (int i = 0; i < _players.Count; i++)
                {
                    ServerPlayer player = _players[i];
                    if (player.ID == id)
                    {
                        _players.RemoveAt(i);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Main.Logger.Warn(e);
            }
            return false;
        }

        public bool TryAddChunk(Chunk chunk)
        {
            if (_chunks.TryAdd(chunk.Position, chunk))
            {
                _chunks[chunk.Position] = chunk;
                return true;
            }
            return false;
        }

        public bool IsChunkLoaded(Vector2 position)
        {
            return _chunks.TryGetValue(position, out Chunk chunk)
                && chunk.LoadAmount != ChunkLoadAmount.Unloaded;
        }
    }
}
