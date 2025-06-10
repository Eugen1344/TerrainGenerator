using System.Collections.Generic;
using Caves.Chunks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerChunkTracker : MonoBehaviour
{
    public int ChunkGenerationRadius;
    public CaveChunkManager ChunkManager;
    public Vector3Int CurrentChunkCoordinate;

    private void Start()
    {
        GenerateNearbyChunksAsync(CurrentChunkCoordinate).Forget();
    }

    private void Update()
    {
        Vector3Int chunkCoordinate = ChunkManager.GetChunkCoordinate(transform.position);

        if (CurrentChunkCoordinate == chunkCoordinate)
            return;

        CurrentChunkCoordinate = chunkCoordinate;
        _ = GenerateNearbyChunksAsync(CurrentChunkCoordinate);
    }

    private async UniTask GenerateNearbyChunksAsync(Vector3Int chunkCoordinate)
    {
        List<UniTask<CaveChunk>> chunkTasks = new List<UniTask<CaveChunk>>(9);

        for (int i = chunkCoordinate.x - ChunkGenerationRadius + 1; i < chunkCoordinate.x + ChunkGenerationRadius; i++)
        {
            for (int j = chunkCoordinate.y - ChunkGenerationRadius + 1; j < chunkCoordinate.y + ChunkGenerationRadius; j++)
            {
                for (int k = chunkCoordinate.z - ChunkGenerationRadius + 1; k < chunkCoordinate.z + ChunkGenerationRadius; k++)
                {
                    Vector3Int newChunkCoordinate = new Vector3Int(i, j, k);

                    chunkTasks.Add(ChunkManager.CreateChunkAsync(newChunkCoordinate));
                }
            }
        }

        await UniTask.WhenAll(chunkTasks);
    }
}