using System.Collections.Generic;
using UnityEngine;

public class CaveConnector
{
	public static List<CaveTunnel> ConnectCaves(ref CellType[,,] cells, List<CaveHollowGroup> hollows, CaveSettings settings)
	{
		List<CaveHollowGroup> alreadyConnectedCaves = new List<CaveHollowGroup>();
		List<CaveTunnel> tunnels = new List<CaveTunnel>();

		foreach (CaveHollowGroup firstHollow in hollows)
		{
			(CaveHollowGroup secondHollow, Vector3Int firstPoint, Vector3Int secondPoint) = ClosestHollow(firstHollow, hollows, alreadyConnectedCaves);

			int firstX = firstPoint.x < secondPoint.x ? firstPoint.x : secondPoint.x;
			int secondX = secondPoint.x > firstPoint.x ? secondPoint.x : firstPoint.x;

			List<Vector3Int> tunnelCells = new List<Vector3Int>();

			/*for (int x = firstX; x <= secondX; x++)
			{
				float y = (x - firstPoint.x) * (secondPoint.y - firstPoint.y) / (secondPoint.x - firstPoint.x) + firstPoint.y;

				int minY = (int)Mathf.Floor(y);
				int maxY = (int)Mathf.Ceil(y);

				int maxHeight = Mathf.Min(settings.TunnelHeight, settings.TerrainCubicSize.z);

				for (int k = 0; k < maxHeight; k++)
				{
					//cells[x, minY, k] = CellType.Hollow;
					//cells[x, maxY, k] = CellType.Hollow;

					tunnelCells.Add(new Vector3Int(x, minY, k));
					tunnelCells.Add(new Vector3Int(x, maxY, k));
				}
			}*/

			CaveTunnel tunnel = new CaveTunnel(tunnelCells, firstHollow, secondHollow, firstPoint, secondPoint);
			tunnels.Add(tunnel);

			alreadyConnectedCaves.Add(secondHollow);
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