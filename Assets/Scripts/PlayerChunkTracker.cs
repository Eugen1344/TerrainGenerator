using Caves;
using Caves.Chunks;
using UnityEngine;

public class PlayerChunkTracker : MonoBehaviour
{
	public int ChunkGenerationRadius;
	public CaveChunkManager ChunkManager;
	public Vector3Int CurrentChunkCoordinate;

	private void Start()
	{
		GenerateNearbyChunks();
	}

	private void Update()
	{
		Vector3Int chunkCoordinate = ChunkManager.GetChunkCoordinate(transform.position);

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
				for (int k = CurrentChunkCoordinate.z - ChunkGenerationRadius + 1; k < CurrentChunkCoordinate.z + ChunkGenerationRadius; k++)
				{
					Vector3Int newChunkCoordinate = new Vector3Int(i, j, k);

					ChunkManager.CreateChunk(newChunkCoordinate);
				}
			}
		}
	}
}
