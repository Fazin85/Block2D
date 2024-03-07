using Block2D.Common;
using Block2D.Server;

namespace Fazin
{
    public struct ChunkSectionData
    {
        public ushort[] Tiles { get; private set; }
        public byte Offset { get; private set; }

        public ChunkSectionData(ushort[] tiles, byte offset)
        {
            Tiles = tiles;
            Offset = offset;
        }

        public static ChunkSectionData[] GetChunkSections(ServerChunk chunk)
        {
            Helper.SplitMidPoint(chunk.GetChunkTileIds(), out ushort[] b1, out ushort[] b2);
            Helper.SplitMidPoint(b1, out ushort[] b3, out ushort[] b4);
            Helper.SplitMidPoint(b2, out ushort[] b5, out ushort[] b6);

            ChunkSectionData d1 = new(b3, 0);
            ChunkSectionData d2 = new(b4, 1);
            ChunkSectionData d3 = new(b5, 2);
            ChunkSectionData d4 = new(b6, 3);

            ChunkSectionData[] sections = new ChunkSectionData[Chunk.CHUNK_SECTION_COUNT] { d1, d2, d3, d4 };

            return sections;
        }
    }
}
