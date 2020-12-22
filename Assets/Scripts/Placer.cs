using System.Collections.Generic;
using System.Threading.Tasks;
using Caves.Cells;
using Caves.Chunks;
using UnityEngine;

public class Placer : MonoBehaviour
{
	public CaveChunkManager ChunkManager;
	public Vector3Int DestinationChunkCoordinate;
	public int ChunkGenerationRadius; //TODO merge with PlayerChunkTracker

	public async Task PlaceInRandomCave()
	{
		await GenerateNearbyChunksAsync(DestinationChunkCoordinate);
		CaveChunk destinationChunk = await ChunkManager.CreateChunkAsync(DestinationChunkCoordinate);

		int randomCaveIndex = Random.Range(0, destinationChunk.CellData.Hollows.Count - 1);

		await PlaceInCave(destinationChunk.CellData.Hollows[randomCaveIndex]);
	}

	public async Task PlaceInCave(int caveIndex)
	{
		await GenerateNearbyChunksAsync(DestinationChunkCoordinate);
		CaveChunk destinationChunk = await ChunkManager.CreateChunkAsync(DestinationChunkCoordinate);

		await PlaceInCave(destinationChunk.CellData.Hollows[caveIndex]);
	}

	public async Task PlaceInCave(HollowGroup cave) //TODO different placements
	{
		Vector3Int chunkPosition = cave.GetLowestPoint();

		await GenerateNearbyChunksAsync(DestinationChunkCoordinate);
		CaveChunk destinationChunk = await ChunkManager.CreateChunkAsync(DestinationChunkCoordinate);

		Vector3 position = destinationChunk.GetWorldPosition(chunkPosition);
		transform.position = position;
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