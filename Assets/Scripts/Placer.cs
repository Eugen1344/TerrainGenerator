using Caves;
using Caves.Cells;
using UnityEngine;

public class Placer : MonoBehaviour
{
	public CaveChunk DestinationChunk;

	public void PlaceInRandomHollowGroup()
	{
		int randomCaveIndex = Random.Range(0, DestinationChunk.CellData.Hollows.Count);
		PlaceInCave(randomCaveIndex);
	}

	public void PlaceInCave(int caveIndex)
	{
		PlaceInCave(DestinationChunk.CellData.Hollows[caveIndex]);
	}

	public void PlaceInCave(CaveHollowGroup cave) //TODO different placements
	{
		int randomPositionIndex = Random.Range(0, cave.GroundCells.Count);
		Vector3Int chunkPosition = cave.GroundCells[randomPositionIndex];

		Vector3 position = DestinationChunk.GetWorldPosition(chunkPosition);
		transform.position = position;
	}
}