using System.Collections.Generic;
using UnityEngine;

public class CaveTunnel
{
	public List<Vector3Int> CellChunkCoordinates;
	public CaveHollowGroup PreviousCave;
	public CaveHollowGroup NextCave;
	public Vector3Int PreviousCaveConnectionPoint;
	public Vector3Int NextCaveConnectionPoint;

	public CaveTunnel(List<Vector3Int> cellChunkCoordinates, CaveHollowGroup firstCave, CaveHollowGroup secondCave, Vector3Int firstCaveConnectionPoint, Vector3Int secondCaveConnectionPoint)
	{
		CellChunkCoordinates = cellChunkCoordinates;
		PreviousCave = firstCave;
		NextCave = secondCave;
		PreviousCaveConnectionPoint = firstCaveConnectionPoint;
		NextCaveConnectionPoint = secondCaveConnectionPoint;
	}
}