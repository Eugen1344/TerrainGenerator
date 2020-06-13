using Caves;
using Caves.Chunks;
using UnityEngine;

public class PlayerChunkTracker : MonoBehaviour
{
	public int ChunkGenerationRadius;
	public CaveChunkManager ChunkManager;
	public Vector2Int CurrentChunkCoordinate;

	private void Start()
	{
		GenerateNearbyChunks();
	}

	private void Update()
	{
		Vector2Int chunkCoordinate = ChunkManager.GetChunkCoordinate(transform.position);

		if (CurrentChunkCoordinate == chunkCoordinate)
			return;

		CurrentChunkCoordinate = chunkCoordinate;
		GenerateNearbyChunks();
	}

	private void GenerateNearbyChunks()
	{
		for (int i = CurrentChunkCoordinate.x - ChunkGenerationRadius + 1; i < CurrentChunkCoordinate.x + ChunkGenerationRadius; i++)
		{
			for (int j = CurrentChunkCoordinate.y - ChunkGenerationRadius + 1; j < CurrentChunkCoordinate.y + ChunkGenerationRadius; j++)
			{
				Vector2Int newChunkCoordinate = new Vector2Int(i, j);

				ChunkManager.CreateChunk(newChunkCoordinate);
			}
		}
	}
}
