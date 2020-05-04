using System.Collections.Generic;
using UnityEngine;

namespace Caves.Cells
{
	public class CaveTunnel
	{
		public List<Vector3Int> CellChunkCoordinates;
		public CaveHollowGroup FirstCave;
		public CaveHollowGroup SecondCave;
		public Vector3Int FirstCaveConnectionPoint;
		public Vector3Int SecondCaveConnectionPoint;

		public CaveTunnel(List<Vector3Int> cellChunkCoordinates, CaveHollowGroup firstCave, CaveHollowGroup secondCave, Vector3Int firstCaveConnectionPoint, Vector3Int secondCaveConnectionPoint)
		{
			CellChunkCoordinates = cellChunkCoordinates;
			FirstCave = firstCave;
			SecondCave = secondCave;
			FirstCaveConnectionPoint = firstCaveConnectionPoint;
			SecondCaveConnectionPoint = secondCaveConnectionPoint;
		}
	}
}