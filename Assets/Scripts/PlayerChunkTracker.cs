using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caves;
using Caves.Chunks;
using UnityEngine;

public class PlayerChunkTracker : MonoBehaviour
{
	public int ChunkGenerationRadius;
	public CaveChunkManager ChunkManager;
	public Vector3Int CurrentChunkCoordinate;

	private async void Start()
	{
		await GenerateNearbyChunksAsync(CurrentChunkCoordinate);
	}

	private async void Update()
	{
		Vector3Int chunkCoordinate = ChunkManager.GetChunkCoordinate(transform.position);

		if (CurrentChunkCoordinate == chunkCoordinate)
			return;

		CurrentChunkCoordinate = chunkCoordinate;
		await GenerateNearbyChunksAsync(CurrentChunkCoordinate);
	}

	private async Task GenerateNearbyChunksAsync(Vector3Int chunkCoordinate)
	{
		List<Task<CaveChunk>> chunkTasks = new List<Task<CaveChunk>>(9);

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

		await Task.WhenAll(chunkTasks);
	}
}