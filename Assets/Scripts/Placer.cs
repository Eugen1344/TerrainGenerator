using Caves;
using Caves.Cells;
using UnityEngine;

public class Placer : MonoBehaviour
{
	public CaveChunkManager ChunkManager;
	public Vector2Int DestinationChunkCoordinate;

	public void PlaceInRandomCave()
	{
		CaveChunk destinationChunk = ChunkManager.GeneratedChunks[DestinationChunkCoordinate];

		int randomCaveIndex = Random.Range(0, destinationChunk.CellData.Hollows.Count - 1);
		PlaceInCave(randomCaveIndex);
	}

	public void PlaceInCave(int caveIndex)
	{
		CaveChunk destinationChunk = ChunkManager.GeneratedChunks[DestinationChunkCoordinate];

		PlaceInCave(destinationChunk.CellData.Hollows[caveIndex]);
	}

	public void PlaceInCave(CaveHollowGroup cave) //TODO different placements
	{
		int randomPositionIndex = Random.Range(0, cave.GroundCells.Count);
		Vector3Int chunkPosition = cave.GroundCells[randomPositionIndex];

		CaveChunk destinationChunk = ChunkManager.GeneratedChunks[DestinationChunkCoordinate];

		Vector3 position = destinationChunk.GetWorldPosition(chunkPosition);
		transform.position = position;
	}
}