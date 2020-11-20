using Caves;
using Caves.Cells;
using Caves.Chunks;
using UnityEngine;

public class Placer : MonoBehaviour
{
	public CaveChunkManager ChunkManager;
	public Vector3Int DestinationChunkCoordinate;

	public void PlaceInRandomCave()
	{
		CaveChunk destinationChunk = ChunkManager.CreateChunk(DestinationChunkCoordinate);

		int randomCaveIndex = Random.Range(0, destinationChunk.CellData.Hollows.Count - 1);

		PlaceInCave(destinationChunk.CellData.Hollows[randomCaveIndex]);
	}

	public void PlaceInCave(int caveIndex)
	{
		CaveChunk destinationChunk = ChunkManager.CreateChunk(DestinationChunkCoordinate);

		PlaceInCave(destinationChunk.CellData.Hollows[caveIndex]);
	}

	public void PlaceInCave(HollowGroup cave) //TODO different placements
	{
		Vector3Int chunkPosition = cave.GetLowestPoint();

		CaveChunk destinationChunk = ChunkManager.CreateChunk(DestinationChunkCoordinate);

		Vector3 position = destinationChunk.GetWorldPosition(chunkPosition);
		transform.position = position;
	}
}