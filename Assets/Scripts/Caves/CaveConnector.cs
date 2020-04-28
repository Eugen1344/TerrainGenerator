using System.Collections.Generic;
using UnityEngine;

public class CaveConnector
{
	public static List<CaveTunnel> ConnectCaves(ref CellType[,,] cells, List<CaveHollowGroup> hollows, CaveSettings settings)
	{
		LinkedList<CaveHollowGroup> connectedCaves = new LinkedList<CaveHollowGroup>();

		List<CaveTunnel> tunnels = new List<CaveTunnel>();

		foreach (CaveHollowGroup hollow in hollows)
		{
			(CaveHollowGroup cave, Vector3Int firstPoint, Vector3Int secondPoint) = ClosestHollow(hollow, hollows);

			int firstX = firstPoint.x < secondPoint.x ? firstPoint.x : secondPoint.x;
			int secondX = secondPoint.x > firstPoint.x ? secondPoint.x : firstPoint.x;

			if(firstPoint.x == secondPoint.x) //TODO temp
				continue;

			for (int x = firstX; x <= secondX; x++)
			{
				float y = (x - firstPoint.x) * (secondPoint.y - firstPoint.y) / (secondPoint.x - firstPoint.x) + firstPoint.y;

				int minY = (int)Mathf.Floor(y);
				int maxY = (int)Mathf.Ceil(y);
				 
				int maxHeight = Mathf.Min(settings.TunnelHeight, settings.TerrainCubicSize.z);

				for (int k = 0; k < maxHeight; k++)
				{
					cells[x, minY, k] = CellType.Hollow;
					cells[x, maxY, k] = CellType.Hollow;
				}
			}
		}

		return tunnels;
	}

	private static (CaveHollowGroup hollow, Vector3Int firstPoint, Vector3Int secondPoint) ClosestHollow(CaveHollowGroup hollow, List<CaveHollowGroup> hollows)
	{
		Vector3Int firstPoint = default;
		Vector3Int secondPoint = default;
		float minDistance = 0;
		CaveHollowGroup closestHollow = null;

		foreach (CaveHollowGroup nextHollow in hollows)
		{
			if(nextHollow == hollow)
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