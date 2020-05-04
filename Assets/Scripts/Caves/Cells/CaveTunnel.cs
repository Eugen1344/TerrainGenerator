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

		public static List<CaveTunnel> CreateTunnelsAndConnectCaves(ref CellType[,,] cells, List<CaveHollowGroup> hollows, CaveSettings settings)
		{
			List<CaveHollowGroup> alreadyConnectedCaves = new List<CaveHollowGroup>();
			List<CaveTunnel> tunnels = new List<CaveTunnel>();

			foreach (CaveHollowGroup firstHollow in hollows)
			{
				(CaveHollowGroup secondHollow, Vector3Int firstPoint, Vector3Int secondPoint) = ClosestHollow(firstHollow, hollows, alreadyConnectedCaves);

				int firstX = firstPoint.x < secondPoint.x ? firstPoint.x : secondPoint.x;
				int secondX = secondPoint.x > firstPoint.x ? secondPoint.x : firstPoint.x;
				int firstY = firstPoint.y < secondPoint.y ? firstPoint.y : secondPoint.y;
				int secondY = secondPoint.y > firstPoint.y ? secondPoint.y : firstPoint.y;

				List<Vector3Int> tunnelCells = new List<Vector3Int>();

				for (int x = firstX; x <= secondX; x++)
				{
					for (int y = firstY; y <= secondY; y++)
					{
						float distance = Mathf.Abs((secondPoint.y - firstPoint.y) * x - (secondPoint.x - firstPoint.x) * y + secondPoint.x * firstPoint.y - secondPoint.y * firstPoint.x) /
										 Mathf.Sqrt(Mathf.Pow(secondPoint.x - firstPoint.x, 2) + Mathf.Pow(secondPoint.y - firstPoint.y, 2));

						if (distance > settings.TunnelWidth)
							continue;

						int maxHeight = Mathf.Min(settings.TunnelHeight, settings.TerrainCubicSize.z);

						for (int z = 0; z < maxHeight; z++)
						{
							cells[x, y, z] = CellType.Hollow;

							tunnelCells.Add(new Vector3Int(x, y, z));
						}
					}
				}

				CaveTunnel tunnel = new CaveTunnel(tunnelCells, firstHollow, secondHollow, firstPoint, secondPoint);
				tunnels.Add(tunnel);

				alreadyConnectedCaves.Add(firstHollow);
			}

			return tunnels;
		}

		private static (CaveHollowGroup hollow, Vector3Int firstPoint, Vector3Int secondPoint) ClosestHollow(CaveHollowGroup hollow, List<CaveHollowGroup> hollows, List<CaveHollowGroup> alreadyConnectedCaves)
		{
			Vector3Int firstPoint = default;
			Vector3Int secondPoint = default;
			float minDistance = 0;
			CaveHollowGroup closestHollow = null;

			foreach (CaveHollowGroup nextHollow in hollows)
			{
				if (nextHollow == hollow || alreadyConnectedCaves.Contains(nextHollow))
					continue;

				foreach (Vector3Int firstCoordinate in hollow.CellChunkCoordinates)
				{
					foreach (Vector3Int secondCoordinate in nextHollow.CellChunkCoordinates)
					{
						float distance = Vector3Int.Distance(firstCoordinate, secondCoordinate);

						if (distance < minDistance || closestHollow == null)
						{
							firstPoint = firstCoordinate;
							secondPoint = secondCoordinate;
							minDistance = distance;
							closestHollow = nextHollow;
						}
					}
				}
			}

			return (closestHollow, firstPoint, secondPoint);
		}
	}
}