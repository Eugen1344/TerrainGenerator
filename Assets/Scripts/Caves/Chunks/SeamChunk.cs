using Caves.Cells.SimplexNoise;
using UnityEngine;

namespace Caves.Chunks
{
    public class SeamChunk : Chunk
    {
        public void Setup(Vector3Int seamCoordinate)
        {
            name = $"{seamCoordinate.ToString()}+";
            gameObject.SetActive(false);
        }

        public void InsertCellData(ChunkCellData cellData, Vector3Int chunkCoordinate)
        {
        }

        public void InsertMeshData(Vector3Int chunkCoordinate)
        {
        }
    }
}